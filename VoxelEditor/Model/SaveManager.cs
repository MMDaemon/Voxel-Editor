using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.Model
{
    public class SaveManager
    {
        private XElement[] _chunkElements;

        public SaveManager()
        {

        }

        public void SaveWorldToFile(IEnumerable<Chunk> chunks, string filePath)
        {
            XElement world = new XElement("World");

            foreach (var chunk in chunks)
            {
                world.Add(CreateChunkXml(chunk));
            }

            XDocument doc = new XDocument(world);

            doc.Save(filePath);
        }

        public bool InitializeLoadFromFile(string filePath, out List<Vector3I> positions)
        {
            const string pattern = @"(\d+:\d+,)+";

            positions = new List<Vector3I>();

            XElement world = XElement.Load(filePath);

            XElement[] chunkElements = world.Elements() as XElement[] ?? world.Elements().ToArray();

            foreach (XElement chunkElement in chunkElements)
            {
                XElement position = chunkElement.Element("Position");
                if (position == null || !Vector3I.TryParse(position.Value, out var pos)) return false;
                positions.Add(pos);
                XElement types = chunkElement.Element("Types");
                XElement amounts = chunkElement.Element("Amounts");
                if (types == null || amounts == null || !Regex.Match(types.Value, pattern).Value.Equals(types.Value) || !Regex.Match(amounts.Value, pattern).Value.Equals(amounts.Value)) return false;
            }

            _chunkElements = chunkElements;
            return true;
        }

        public Dictionary<Vector3I, Voxel> LoadWorldVoxelsFromFile()
        {
            Dictionary<Vector3I, Voxel> loadedVoxels = new Dictionary<Vector3I, Voxel>();

            foreach (XElement chunkElement in _chunkElements)
            {
                Vector3I.TryParse(chunkElement.Element("Position")?.Value, out var position);
                Dictionary<Vector3I, Voxel> loadedChunkVoxels = LoadChunkVoxels(position, chunkElement.Element("Types")?.Value, chunkElement.Element("Amounts")?.Value);

                foreach (var pair in loadedChunkVoxels)
                {
                    loadedVoxels.Add(pair.Key, pair.Value);
                }
            }

            return loadedVoxels;
        }

        private XElement CreateChunkXml(Chunk chunk)
        {
            string position = chunk.Position.ToString();
            string types = "";
            string amounts = "";

            if (chunk.Empty)
            {
                types += $"{Constant.MaxVoxelsPerChunk}:{Constant.MaterialAir},";
                amounts += $"{Constant.ChunkSizeX * Constant.ChunkSizeY * Constant.ChunkSizeZ}:{0},";
            }

            else
            {
                int typeCount = 0;
                int currentType = Constant.MaterialAir;
                int amountCount = 0;
                int currentAmount = 0;

                for (int z = 0; z < Constant.ChunkSizeX; z++)
                {
                    for (int y = 0; y < Constant.ChunkSizeY; y++)
                    {
                        for (int x = 0; x < Constant.ChunkSizeZ; x++)
                        {
                            if (chunk[x, y, z].MaterialId == currentType)
                            {
                                typeCount++;
                            }
                            else
                            {
                                if (typeCount > 0)
                                {
                                    types += $"{typeCount}:{currentType},";
                                }
                                currentType = chunk[x, y, z].MaterialId;
                                typeCount = 1;
                            }

                            if (chunk[x, y, z].Amount == currentAmount)
                            {
                                amountCount++;
                            }
                            else
                            {
                                if (amountCount > 0)
                                {
                                    amounts += $"{amountCount}:{currentAmount},";
                                }
                                currentAmount = chunk[x, y, z].Amount;
                                amountCount = 1;
                            }
                        }
                    }
                }

                types += $"{typeCount}:{currentType},";
                amounts += $"{amountCount}:{currentAmount},";
            }

            return
                new XElement("Chunk",
                    new XElement("Position", position),
                    new XElement("Types", types),
                    new XElement("Amounts", amounts));
        }

        private Dictionary<Vector3I, Voxel> LoadChunkVoxels(Vector3I chunkPosition, string typesString, string amountsString)
        {
            Dictionary<Vector3I, Voxel> loadedVoxels = new Dictionary<Vector3I, Voxel>();

            const string pattern = @"(\d+:\d+,)+";

            if (!Regex.Match(typesString, pattern).Value.Equals(typesString) || !Regex.Match(amountsString, pattern).Value.Equals(amountsString)) return loadedVoxels;

            if (typesString == $"{Constant.MaxVoxelsPerChunk}:{Constant.MaterialAir},"
                && amountsString == $"{Constant.ChunkSizeX * Constant.ChunkSizeY * Constant.ChunkSizeZ}:{0},") return loadedVoxels;

            List<Tuple<int, int>> types = GetValuePairsFromPatternString(typesString);
            List<Tuple<int, int>> amounts = GetValuePairsFromPatternString(amountsString);

            int typesCount = 0;
            int typesId = 0;
            int amountsCount = 0;
            int amountsId = 0;
            while (typesCount < Constant.MaxVoxelsPerChunk)
            {
                if (types[typesId].Item2 != Constant.MaterialAir)
                {
                    for (int i = 0; i < types[typesId].Item1; i++)
                    {
                        int x = (typesCount + i) % Constant.ChunkSizeY % Constant.ChunkSizeZ;
                        int y = (typesCount + i) / Constant.ChunkSizeX % Constant.ChunkSizeZ;
                        int z = (typesCount + i) / (Constant.ChunkSizeX * Constant.ChunkSizeY);
                        Vector3I globalPosition = chunkPosition * Constant.ChunkSize + new Vector3I(x, y, z);
                        while (amountsCount + amounts[amountsId].Item1 < typesCount + i + 1)
                        {
                            amountsCount += amounts[amountsId].Item1;
                            amountsId++;
                        }
                        loadedVoxels.Add(globalPosition, new Voxel(types[typesId].Item2, amounts[amountsId].Item2));
                    }
                }
                typesCount += types[typesId].Item1;
                typesId++;
            }

            return loadedVoxels;
        }

        private List<Tuple<int, int>> GetValuePairsFromPatternString(string patternString)
        {
            List<Tuple<int, int>> valuePairs = new List<Tuple<int, int>>();

            string[] pairStrings = patternString.TrimEnd(',').Split(',');

            foreach (string pair in pairStrings)
            {
                string[] split = pair.Split(':');
                valuePairs.Add(new Tuple<int, int>(int.Parse(split[0]), int.Parse(split[1])));
            }

            return valuePairs;
        }
    }
}
