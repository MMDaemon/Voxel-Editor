using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DMS.Geometry;
using DMS.OpenGL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using VoxelEditor.ViewModel;
using VoxelUtils;
using VoxelUtils.Registry.View;
using VoxelUtils.Shared;
using VoxelUtils.Visual;

namespace VoxelEditor.View
{
    internal class EditorVisual
    {
        private readonly ViewRegistry _registry;

        private float _time;

        private Shader _voxelShader;
        private Shader _raytraceShader;
        private Shader _addShader;
        private Shader _ssaoShader;
        private Shader _depthShader;

        private readonly Texture _crosshairs;
        private readonly TextureFont _font;

        private VAO _voxelGeometry;
        private VAO _raytraceGeometry;
        private VAO _depthGeometry;

        private readonly FBO[] _renderToTexture;
        private readonly FBO[] _renderToTextureWithDepth;

        private int _width;
        private int _height;
        private float _aspect;

        private float _voxelSize;
        private Vector3I _worldSize;
        private Vector3 _raytraceVoxelPosition;
        private readonly Dictionary<Vector3I, Mesh> _chunkMeshes;
        private readonly Dictionary<Vector3I, Mesh> _chunkPartMeshes;

        public string VoxelShaderName => nameof(_voxelShader);
        public string RaytraceShaderName => nameof(_raytraceShader);
        public string AddShaderName => nameof(_addShader);
        public string SsaoShaderName => nameof(_ssaoShader);
        public string DepthShaderName => nameof(_depthShader);

        public EditorVisual(ViewRegistry registry)
        {
            _registry = registry;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            _chunkMeshes = new Dictionary<Vector3I, Mesh>();
            _chunkPartMeshes = new Dictionary<Vector3I, Mesh>();
            _raytraceVoxelPosition = Vector3.Zero;
            _renderToTexture = new FBO[5];
            _renderToTextureWithDepth = new FBO[2];

            _crosshairs = TextureLoader.FromBitmap(Resourcen.FadenkreuzBW);
            _font = new TextureFont(TextureLoader.FromBitmap(Resourcen.Coders_Crux), 16, 0, 0.6f, 0.9f, 0.75f);
        }

        public void ShaderChanged(string name, Shader shader)
        {
            switch (name)
            {
                case nameof(_voxelShader):
                    _voxelShader = shader;
                    if (!ReferenceEquals(shader, null))
                    {
                        UpdateVoxelMesh(CalculateVoxelMesh());
                    }
                    break;
                case nameof(_raytraceShader):
                    _raytraceShader = shader;
                    if (!ReferenceEquals(shader, null))
                    {
                        UpdateRaytraceMesh();
                    }
                    break;
                case nameof(_addShader):
                    _addShader = shader;
                    break;
                case nameof(_depthShader):
                    _depthShader = shader;
                    if (!ReferenceEquals(shader, null))
                    {
                        UpdateDepthMesh(CalculateVoxelMesh());
                    }
                    break;
                case nameof(_ssaoShader):
                    _ssaoShader = shader;
                    break;
            }
        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
            _aspect = (float)width / height;
            _renderToTexture[0] = new FBO(Texture.Create(width, height, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float));
            _renderToTexture[1] = new FBO(Texture.Create(width, height, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float));
            _renderToTexture[2] = new FBO(Texture.Create(width, height, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float));
            _renderToTexture[3] = new FBO(Texture.Create(width / 2, height / 2, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float));
            _renderToTexture[4] = new FBO(Texture.Create(width, height, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float));
            _renderToTextureWithDepth[0] = new FBOwithDepth(Texture.Create(width, height, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float));
            _renderToTextureWithDepth[1] = new FBOwithDepth(Texture.Create(width / 2, height / 2, PixelInternalFormat.R32f, PixelFormat.Red, PixelType.Float));
        }

        public void Render(EditorViewModel viewModel)
        {
            _time = viewModel.Time;

            float[] cam = viewModel.CameraMatrix.ToArray();

            _voxelSize = viewModel.VoxelSize;
            _worldSize = viewModel.WorldSize;
            _raytraceVoxelPosition = viewModel.RaytraceVoxelPosition;

            CalculateChunkMeshes(viewModel.Chunks);

            Texture mainTexture = RenderOnTexture(delegate { RenderMain(viewModel.CameraPosition, cam, viewModel.RaytracedVoxel != null); }, 1);
            Texture guiTexture = RenderOnTexture(delegate { RenderGui(viewModel.MaterialId, viewModel.MaterialAmount, viewModel.RaytracedVoxel); }, 2);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderAddedTextures(mainTexture, guiTexture, 1.0f);
        }

        private Texture RenderOnTexture(Action render, int fboId)
        {
            _renderToTexture[fboId].Activate();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ActiveTexture(TextureUnit.Texture0);

            render();

            _renderToTexture[fboId].Deactivate();
            return _renderToTexture[fboId].Texture;
        }

        private Texture RenderOnTextureWithDepth(Action render, int fboId)
        {
            _renderToTextureWithDepth[fboId].Activate();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ActiveTexture(TextureUnit.Texture0);

            render();

            _renderToTextureWithDepth[fboId].Deactivate();
            return _renderToTextureWithDepth[fboId].Texture;
        }

        private void RenderAddedTextures(Texture texture1, Texture texture2, float opaque)
        {
            _addShader.Activate();
            GL.ActiveTexture(TextureUnit.Texture0);
            texture1.Activate();
            GL.Uniform1(_addShader.GetUniformLocation("image1"), TextureUnit.Texture0 - TextureUnit.Texture0);

            GL.ActiveTexture(TextureUnit.Texture1);
            texture2.Activate();
            GL.Uniform1(_addShader.GetUniformLocation("image2"), TextureUnit.Texture1 - TextureUnit.Texture0);

            GL.Uniform1(_addShader.GetUniformLocation("opaque"), opaque);

            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            texture1.Deactivate();
            texture2.Deactivate();
            _addShader.Deactivate();
        }

        private void RenderMain(Vector3 cameraPosition, float[] cam, bool raytraceCollided)
        {
            Texture raytraceTexture = RenderOnTexture(delegate { RenderRaytrace(cam, raytraceCollided); }, 0);

            Mesh mesh = CalculateVoxelMesh();

            UpdateVoxelMesh(mesh);
            Texture voxelTexture = RenderOnTextureWithDepth(delegate { RenderVoxels(cameraPosition, cam); }, 0);

            UpdateDepthMesh(mesh);
            Texture depthTexture = RenderOnTextureWithDepth(delegate { RenderDepth(cameraPosition, cam); }, 1);

            Texture ssaoTexture = RenderOnTexture(delegate { RenderSsao(depthTexture); }, 3);

            voxelTexture = RenderOnTexture(delegate { RenderAddedTextures(voxelTexture, ssaoTexture, 0.2f); }, 4);

            RenderAddedTextures(voxelTexture, raytraceTexture, 0.5f);
        }

        private void RenderGui(int materialId, int materialAmount, Voxel raytracedVoxel)
        {
            GL.Color3(Color.White);
            float crosshairsSize = 70.0f;

            _crosshairs.Activate();
            RenderFunctions.DrawRect(-crosshairsSize / (2 * (float)_width), -crosshairsSize / (2 * (float)_height), crosshairsSize / (float)_width, crosshairsSize / (float)_height);
            _crosshairs.Deactivate();

            float centerPos =
                -CalculateVoxelContentGuiTextWidth(materialId, materialAmount, 0.07f) / _aspect / 2;
            RenderVoxelContentGui(materialId, materialAmount, 0.07f, new Vector2(centerPos, -1.0f), "Selected:");

            if (raytracedVoxel != null && raytracedVoxel.MaterialId != Constant.MaterialAir)
            {
                centerPos =
                -CalculateVoxelContentGuiTextWidth(raytracedVoxel.MaterialId, raytracedVoxel.Amount, 0.07f) / _aspect / 2;
                RenderVoxelContentGui(raytracedVoxel.MaterialId, raytracedVoxel.Amount, 0.07f, new Vector2(centerPos, 0.79f), "Looking at:");
            }
        }

        private void RenderVoxelContentGui(int materialId, int materialAmount, float fontSize, Vector2 position, string title = "")
        {
            GL.PushMatrix();
            GL.Scale(1 / _aspect, 1, 1);

            float width = CalculateVoxelContentGuiTextWidth(materialId, materialAmount, fontSize, true);
            GL.Color4(new Color4(1, 1, 1, 0.9f));
            RenderFunctions.DrawRect(_aspect * position.X, position.Y, width, (title == "" ? 2 : 2.8f) * fontSize);

            GL.Color3(Color.Black);
            if (title != "")
            {
                _font.Print(_aspect * position.X, position.Y + 2 * fontSize, 0, 0.8f * fontSize, title);
            }

            _font.Print(_aspect * position.X, position.Y + fontSize, 0, fontSize, "Material: " + _registry.GetMaterialInfo(materialId).Name);

            _font.Print(_aspect * position.X, position.Y, 0, fontSize, "Amount: " + GetTextForAmount(materialAmount));

            GL.PopMatrix();
        }

        private float CalculateVoxelContentGuiTextWidth(int materialId, int materialAmount, float fontSize, bool addSpace = false)
        {
            string text1 = "Material: " + _registry.GetMaterialInfo(materialId).Name;
            string text2 = "Amount: " + GetTextForAmount(materialAmount);

            if (addSpace)
            {
                text1 += " ";
                text2 += " ";
            }

            float width1 = _font.Width(text1, fontSize);
            float width2 = _font.Width(text2, fontSize);
            return width1 > width2 ? width1 : width2;
        }

        private string GetTextForAmount(float materialAmount)
        {
            materialAmount /= Constant.MaxMaterialAmount;

            string amountText = $"{materialAmount}";

            if (materialAmount != 0 && materialAmount != 1 && materialAmount % (1.0f / 128) == 0)
            {
                int value1 = (int)(materialAmount * 128);
                int value2 = 128;
                while (value1 % 2 == 0)
                {
                    value1 /= 2;
                    value2 /= 2;
                }
                amountText = $"{value1}/{value2} ({(int)(materialAmount * 128)}/128)";
            }
            if (materialAmount == 1)
            {
                amountText = "1 (128/128)";
            }

            return amountText;
        }

        private void RenderVoxels(Vector3 cameraPosition, float[] cam)
        {
            GL.Color4(Color4.Transparent);
            _voxelShader.Activate();
            GL.UniformMatrix4(_voxelShader.GetUniformLocation("camera"), 1, false, cam);
            GL.Uniform3(_voxelShader.GetUniformLocation("cameraPosition"), cameraPosition.ToOpenTK());
            GL.Uniform3(_voxelShader.GetUniformLocation("ambientLightColor"), new OpenTK.Vector3(0.1f, 0.1f, 0.1f));
            GL.Uniform3(_voxelShader.GetUniformLocation("lightDirection"), new OpenTK.Vector3(1f, 1.5f, -2f).Normalized());
            GL.Uniform3(_voxelShader.GetUniformLocation("lightColor"), new OpenTK.Vector3(1f, 1f, 1f));
            if (_voxelGeometry.IDLength > 0)
            {
                _voxelGeometry.Draw();
            }
            _voxelShader.Deactivate();
        }

        private void RenderDepth(Vector3 cameraPosition, float[] cam)
        {
            GL.Color4(Color4.White);
            GL.ClearColor(Color.Red);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _depthShader.Activate();
            GL.UniformMatrix4(_depthShader.GetUniformLocation("camera"), 1, false, cam);
            if (_depthGeometry.IDLength > 0)
            {
                _depthGeometry.Draw();
            }
            _depthShader.Deactivate();
            GL.ClearColor(Color.Transparent);
        }

        private void RenderSsao(Texture depthTexture)
        {
            _ssaoShader.Activate();
            GL.Uniform2(_ssaoShader.GetUniformLocation("iResolution"), new OpenTK.Vector2(_width, _height));
            GL.Uniform1(_ssaoShader.GetUniformLocation("iGlobalTime"), _time);
            GL.ActiveTexture(TextureUnit.Texture0);
            depthTexture.Activate();
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            _ssaoShader.Deactivate();
            depthTexture.Deactivate();
        }

        private void RenderRaytrace(float[] cam, bool raytraceCollided)
        {
            UpdateRaytraceMesh();

            if (raytraceCollided)
            {
                GL.Color4(Color4.Transparent);
                _raytraceShader.Activate();
                GL.UniformMatrix4(_raytraceShader.GetUniformLocation("camera"), 1, false, cam);
                _raytraceGeometry.Draw();
                _raytraceShader.Deactivate();
            }
        }

        private Mesh CalculateVoxelMesh()
        {
            Mesh mesh = CreateWorldGround();

            foreach (KeyValuePair<Vector3I, Mesh> chunkMesh in _chunkMeshes)
            {
                mesh.Add(chunkMesh.Value.Transform(Matrix4x4.CreateTranslation((Vector3)(chunkMesh.Key * Constant.ChunkSize))).Transform(Matrix4x4.CreateScale(_voxelSize)));
            }

            foreach (KeyValuePair<Vector3I, Mesh> chunkPartMesh in _chunkPartMeshes)
            {
                mesh.Add(chunkPartMesh.Value.Transform(Matrix4x4.CreateTranslation((Vector3)chunkPartMesh.Key)).Transform(Matrix4x4.CreateScale(_voxelSize)));
            }

            return mesh;
        }

        private void UpdateVoxelMesh(Mesh mesh)
        {
            _voxelGeometry = VAOLoader.FromMesh(mesh, _voxelShader);
        }

        private void UpdateDepthMesh(Mesh mesh)
        {
            _depthGeometry = VAOLoader.FromMesh(mesh, _depthShader);
        }

        private void UpdateRaytraceMesh()
        {
            Mesh mesh = new Mesh();
            mesh.Add(Meshes.CreateCubeWithNormals(_voxelSize).Transform(Matrix4x4.CreateTranslation(_raytraceVoxelPosition)));

            _raytraceGeometry = VAOLoader.FromMesh(mesh, _raytraceShader);
        }

        private Mesh CreateWorldGround()
        {
            Mesh mesh = new Mesh();

            Vector3I negativeWorldSize = ((-_worldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((_worldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = _worldSize.Y * Constant.ChunkSizeY;

            mesh.position.List.Add(new Vector3(positiveWorldSize.X - 0.5f, -0.5f, negativeWorldSize.Z - 0.5f)); //0
            mesh.position.List.Add(new Vector3(positiveWorldSize.X - 0.5f, -0.5f, positiveWorldSize.Z - 0.5f)); //1
            mesh.position.List.Add(new Vector3(negativeWorldSize.X - 0.5f, -0.5f, positiveWorldSize.Z - 0.5f)); //2
            mesh.position.List.Add(new Vector3(negativeWorldSize.X - 0.5f, -0.5f, negativeWorldSize.Z - 0.5f)); //3

            mesh.normal.List.Add(-Vector3.UnitY);
            mesh.normal.List.Add(-Vector3.UnitY);
            mesh.normal.List.Add(-Vector3.UnitY);
            mesh.normal.List.Add(-Vector3.UnitY);

            mesh.IDs.Add(0);
            mesh.IDs.Add(2);
            mesh.IDs.Add(1);
            mesh.IDs.Add(0);
            mesh.IDs.Add(3);
            mesh.IDs.Add(2);

            return mesh.Transform(Matrix4x4.CreateScale(_voxelSize));
        }

        private void CalculateChunkMeshes(IEnumerable<Chunk> viewModelChunks)
        {
            foreach (Chunk chunk in viewModelChunks)
            {
                if (_chunkMeshes.ContainsKey(chunk.Position))
                {
                    _chunkMeshes[chunk.Position] = new ChunkMesh(chunk);
                }
                else
                {
                    _chunkMeshes.Add(chunk.Position, new ChunkMesh(chunk));
                }

                CalculateChunkPartMeshes(chunk);
            }
        }

        private void CalculateChunkPartMeshes(Chunk chunk)
        {
            Vector3I borderChunkPosition = -(_worldSize / 2);
            borderChunkPosition.Y = 0; /*minimum height = 0*/

            if (chunk.Position.X == borderChunkPosition.X)
            {
                CalculateChunkPartMesh(chunk, new Vector3I(1, 0, 0));
                if (chunk.Position.Y == borderChunkPosition.Y)
                {
                    CalculateChunkPartMesh(chunk, new Vector3I(1, 1, 0));
                    if (chunk.Position.Z == borderChunkPosition.Z)
                    {
                        CalculateChunkPartMesh(chunk, new Vector3I(1, 1, 1));
                    }
                }
                if (chunk.Position.Z == borderChunkPosition.Z)
                {
                    CalculateChunkPartMesh(chunk, new Vector3I(1, 0, 1));
                }
            }
            if (chunk.Position.Y == borderChunkPosition.Y)
            {
                CalculateChunkPartMesh(chunk, new Vector3I(0, 1, 0));
                if (chunk.Position.Z == borderChunkPosition.Z)
                {
                    CalculateChunkPartMesh(chunk, new Vector3I(0, 1, 1));
                }
            }
            if (chunk.Position.Z == borderChunkPosition.Z)
            {
                CalculateChunkPartMesh(chunk, new Vector3I(0, 0, 1));
            }
        }

        private void CalculateChunkPartMesh(Chunk chunk, Vector3I direction)
        {
            Vector3I partChunkSize = Constant.ChunkSize;
            for (int i = 0; i < 3; i++)
            {
                if (direction[i] != 0)
                {
                    partChunkSize[i] = 1;
                }
            }
            Chunk partChunk = new Chunk(chunk.Position * Constant.ChunkSize - direction);

            for (int x = direction.X; x <= partChunkSize.X; x++)
            {
                for (int y = direction.Y; y <= partChunkSize.Y; y++)
                {
                    for (int z = direction.Z; z <= partChunkSize.Z; z++)
                    {
                        partChunk[x, y, z] = chunk[x - direction.X, y - direction.Y, z - direction.Z];
                    }
                }
            }

            if (_chunkPartMeshes.ContainsKey(partChunk.Position))
            {
                _chunkPartMeshes[partChunk.Position] = new ChunkMesh(partChunk, partChunkSize);
            }
            else
            {
                _chunkPartMeshes.Add(partChunk.Position, new ChunkMesh(partChunk, partChunkSize));
            }
        }

        private List<Vector3> CalculateVoxelPositions(IEnumerable<Chunk> chunks)
        {
            List<Vector3> voxelPositions = new List<Vector3>();
            foreach (Chunk chunk in chunks)
            {
                for (int x = 0; x < Constant.ChunkSizeX; x++)
                {
                    for (int y = 0; y < Constant.ChunkSizeY; y++)
                    {
                        for (int z = 0; z < Constant.ChunkSizeZ; z++)
                        {
                            if (chunk[x, y, z] != null && chunk[x, y, z].Exists)
                            {
                                voxelPositions.Add(_voxelSize * (Vector3)(chunk.Position * Constant.ChunkSize + new Vector3I(x, y, z)));
                            }
                        }
                    }
                }
            }
            return voxelPositions;
        }
    }
}
