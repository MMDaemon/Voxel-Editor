using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using VoxelEditor.ViewModel;
using VoxelUtils;
using VoxelUtils.Registry.View;
using VoxelUtils.Shared;
using VoxelUtils.Visual;
using Zenseless.Base;
using Zenseless.Geometry;
using Zenseless.HLGL;
using Zenseless.OpenGL;
using OpenGl4 = OpenTK.Graphics.OpenGL4;

namespace VoxelEditor.View
{
    internal class EditorVisual
    {
        private readonly ViewRegistry _registry;

        private float _time;

        private IShader _voxelShader;
        private IShader _raytraceShader;
        private IShader _addShader;
        private IShader _ssaoShader;
        private IShader _depthShader;
        private IShader _geometryShader;

        /// <summary>
        /// Contains the textureIds for the different materials
        /// </summary>
        private readonly Dictionary<int, int> _textureIDs;

        private readonly Texture _materialTextureArray;
        private readonly ITexture _groundTexture;
        private readonly ITexture _crosshairs;
        private readonly TextureFont _font;

        private VAO _groundGeometry;
        private VAO _voxelGeometry;
        private VAO _raytraceGeometry;
        private VAO _depthGeometry;

        private BufferObject _amountBuffer;
        private BufferObject _idBuffer;

        private readonly FBO[] _renderToTexture;
        private readonly FBO[] _renderToTextureWithDepth;

        private int _width;
        private int _height;
        private float _aspect;

        private float _voxelSize = 1;
        private Vector3I _worldSize;
        private Vector3 _raytraceVoxelPosition;
        private readonly Dictionary<Vector3I, VoxelMesh> _chunkMeshes;
        private readonly Dictionary<Vector3I, VoxelMesh> _chunkPartMeshes;

        public string VoxelShaderName => nameof(_voxelShader);
        public string RaytraceShaderName => nameof(_raytraceShader);
        public string AddShaderName => nameof(_addShader);
        public string SsaoShaderName => nameof(_ssaoShader);
        public string DepthShaderName => nameof(_depthShader);
        public string GeometryShaderName => nameof(_geometryShader);

        public EditorVisual(ViewRegistry registry)
        {
            _registry = registry;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);

            _chunkMeshes = new Dictionary<Vector3I, VoxelMesh>();
            _chunkPartMeshes = new Dictionary<Vector3I, VoxelMesh>();
            _raytraceVoxelPosition = Vector3.Zero;
            _renderToTexture = new FBO[5];
            _renderToTextureWithDepth = new FBO[2];

            _textureIDs = new Dictionary<int, int>();
            _materialTextureArray = new Texture(OpenGl4.TextureTarget.Texture2DArray);
            _crosshairs = TextureLoader.FromBitmap(Resourcen.FadenkreuzBW);
            _groundTexture = TextureLoader.FromBitmap(_registry.GetMaterialInfo(1).Texture);
            _font = new TextureFont(TextureLoader.FromBitmap(Resourcen.Coders_Crux), 16, 0, 0.6f, 0.9f, 0.75f);

            InitializeMaterialTextures();
        }

        public void ShaderChanged(string name, IShader shader)
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
                        UpdateDepthMesh(CalculateVoxelMesh().DefaultMesh);
                    }
                    break;
                case nameof(_ssaoShader):
                    _ssaoShader = shader;
                    break;
                case nameof(_geometryShader):
                    _geometryShader = shader;
                    CreateWorldGround();
                    break;
            }
        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
            _aspect = (float)width / height;
            _renderToTexture[0] = new FBO(Texture2dGL.Create(width, height));
            _renderToTexture[1] = new FBO(Texture2dGL.Create(width, height));
            _renderToTexture[2] = new FBO(Texture2dGL.Create(width, height));
            _renderToTexture[3] = new FBO(Texture2dGL.Create(width / 2, height / 2));
            _renderToTexture[4] = new FBO(Texture2dGL.Create(width, height));
            _renderToTextureWithDepth[0] = new FBOwithDepth(Texture2dGL.Create(width, height));
            _renderToTextureWithDepth[1] = new FBOwithDepth(Texture2dGL.Create(width / 2, height / 2, 1, true));
        }

        public void Render(EditorViewModel viewModel)
        {
            _time = viewModel.Time;

            float[] cam = viewModel.CameraMatrix.ToArray();

            _voxelSize = viewModel.VoxelSize;
            _worldSize = viewModel.WorldSize;
            _raytraceVoxelPosition = viewModel.RaytraceVoxelPosition;

            Console.WriteLine(viewModel.CameraPosition);

            CalculateChunkMeshes(viewModel.Chunks);

            ITexture mainTexture = RenderOnTexture(delegate { RenderMain(viewModel.CameraPosition, cam, viewModel.RaytracedVoxel != null); }, 1);
            ITexture guiTexture = RenderOnTexture(delegate { RenderGui(viewModel.MaterialId, viewModel.MaterialAmount, viewModel.RaytracedVoxel); }, 2);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderAddedTextures(mainTexture, guiTexture, 1.0f);
        }

        private ITexture RenderOnTexture(Action render, int fboId)
        {
            _renderToTexture[fboId].Activate();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ActiveTexture(TextureUnit.Texture0);

            render();

            _renderToTexture[fboId].Deactivate();
            return _renderToTexture[fboId].Texture;
        }

        private ITexture RenderOnTextureWithDepth(Action render, int fboId)
        {
            _renderToTextureWithDepth[fboId].Activate();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ActiveTexture(TextureUnit.Texture0);

            render();

            _renderToTextureWithDepth[fboId].Deactivate();
            return _renderToTextureWithDepth[fboId].Texture;
        }

        private void RenderAddedTextures(ITexture texture1, ITexture texture2, float opaque)
        {
            _addShader.Activate();
            GL.ActiveTexture(TextureUnit.Texture0);
            texture1.Activate();
            GL.Uniform1(_addShader.GetResourceLocation(ShaderResourceType.Uniform, "image1"), TextureUnit.Texture0 - TextureUnit.Texture0);

            GL.ActiveTexture(TextureUnit.Texture1);
            texture2.Activate();
            GL.Uniform1(_addShader.GetResourceLocation(ShaderResourceType.Uniform, "image2"), TextureUnit.Texture1 - TextureUnit.Texture0);

            GL.Uniform1(_addShader.GetResourceLocation(ShaderResourceType.Uniform, "opaque"), opaque);

            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            texture1.Deactivate();
            texture2.Deactivate();
            _addShader.Deactivate();
        }

        private void RenderMain(Vector3 cameraPosition, float[] cam, bool raytraceCollided)
        {
            ITexture raytraceTexture = RenderOnTexture(delegate { RenderRaytrace(cam, raytraceCollided); }, 0);

            VoxelMesh mesh = CalculateVoxelMesh();

            UpdateVoxelMesh(mesh);

            CreateWorldGround();

            ITexture voxelTexture = RenderOnTextureWithDepth(delegate
            {
                RenderVoxels(cameraPosition, cam);
                RenderGround(cameraPosition, cam);
            }, 0);

            UpdateDepthMesh(mesh.DefaultMesh);
            ITexture depthTexture = RenderOnTextureWithDepth(delegate { RenderDepth(cameraPosition, cam); }, 1);

            ITexture ssaoTexture = RenderOnTexture(delegate { RenderSsao(depthTexture); }, 3);

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

        private void RenderGround(Vector3 cameraPosition, float[] cam)
        {
            _geometryShader.Activate();
            GL.ActiveTexture(TextureUnit.Texture0);
            _groundTexture.Activate();
            GL.Uniform1(_geometryShader.GetResourceLocation(ShaderResourceType.Uniform, "tex"), TextureUnit.Texture0 - TextureUnit.Texture0);
            GL.UniformMatrix4(_geometryShader.GetResourceLocation(ShaderResourceType.Uniform, "camera"), 1, false, cam);
            GL.Uniform3(_geometryShader.GetResourceLocation(ShaderResourceType.Uniform, "cameraPosition"), cameraPosition.ToOpenTK());
            GL.Uniform3(_geometryShader.GetResourceLocation(ShaderResourceType.Uniform, "ambientLightColor"), new OpenTK.Vector3(0.1f, 0.1f, 0.1f));
            GL.Uniform3(_geometryShader.GetResourceLocation(ShaderResourceType.Uniform, "lightDirection"), new OpenTK.Vector3(1f, 1.5f, -2f).Normalized());
            GL.Uniform3(_geometryShader.GetResourceLocation(ShaderResourceType.Uniform, "lightColor"), new OpenTK.Vector3(1f, 1f, 1f));

            _groundGeometry.Draw();

            _groundTexture.Deactivate();
            _geometryShader.Deactivate();
        }

        private void RenderVoxels(Vector3 cameraPosition, float[] cam)
        {
            _voxelShader.Activate();
            GL.ActiveTexture(TextureUnit.Texture0);
            _materialTextureArray.Activate();
            GL.Uniform1(_voxelShader.GetResourceLocation(ShaderResourceType.Uniform, "texArray"), TextureUnit.Texture0 - TextureUnit.Texture0);
            GL.UniformMatrix4(_voxelShader.GetResourceLocation(ShaderResourceType.Uniform, "camera"), 1, false, cam);
            GL.Uniform3(_voxelShader.GetResourceLocation(ShaderResourceType.Uniform, "cameraPosition"), cameraPosition.ToOpenTK());
            GL.Uniform3(_voxelShader.GetResourceLocation(ShaderResourceType.Uniform, "ambientLightColor"), new OpenTK.Vector3(0.1f, 0.1f, 0.1f));
            GL.Uniform3(_voxelShader.GetResourceLocation(ShaderResourceType.Uniform, "lightDirection"), new OpenTK.Vector3(1f, 1.5f, -2f).Normalized());
            GL.Uniform3(_voxelShader.GetResourceLocation(ShaderResourceType.Uniform, "lightColor"), new OpenTK.Vector3(1f, 1f, 1f));
            _amountBuffer.ActivateBind(3);
            _idBuffer.ActivateBind(4);
            if (_voxelGeometry.IDLength > 0)
            {
                _voxelGeometry.Draw();
            }
            _idBuffer.Deactivate();
            _amountBuffer.Deactivate();
            _materialTextureArray.Deactivate();
            _voxelShader.Deactivate();
        }

        private void RenderDepth(Vector3 cameraPosition, float[] cam)
        {
            GL.Color4(Color4.White);
            GL.ClearColor(Color.Red);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _depthShader.Activate();
            GL.UniformMatrix4(_depthShader.GetResourceLocation(ShaderResourceType.Uniform, "camera"), 1, false, cam);
            if (_depthGeometry.IDLength > 0)
            {
                _depthGeometry.Draw();
            }
            _depthShader.Deactivate();
            GL.ClearColor(Color.Transparent);
        }

        private void RenderSsao(ITexture depthTexture)
        {
            _ssaoShader.Activate();
            GL.Uniform2(_ssaoShader.GetResourceLocation(ShaderResourceType.Uniform, "iResolution"), new OpenTK.Vector2(_width, _height));
            GL.Uniform1(_ssaoShader.GetResourceLocation(ShaderResourceType.Uniform, "iGlobalTime"), _time);
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
                GL.UniformMatrix4(_raytraceShader.GetResourceLocation(ShaderResourceType.Uniform, "camera"), 1, false, cam);
                _raytraceGeometry.Draw();
                _raytraceShader.Deactivate();
            }
        }

        private VoxelMesh CalculateVoxelMesh()
        {
            VoxelMesh mesh = new VoxelMesh();

            foreach (KeyValuePair<Vector3I, VoxelMesh> chunkMesh in _chunkMeshes)
            {
                mesh.Add(chunkMesh.Value.Transform(Matrix4x4.CreateTranslation((Vector3)(chunkMesh.Key * Constant.ChunkSize))).Transform(Matrix4x4.CreateScale(_voxelSize)));
            }

            foreach (KeyValuePair<Vector3I, VoxelMesh> chunkPartMesh in _chunkPartMeshes)
            {
                mesh.Add(chunkPartMesh.Value.Transform(Matrix4x4.CreateTranslation((Vector3)chunkPartMesh.Key)).Transform(Matrix4x4.CreateScale(_voxelSize)));
            }

            return mesh;
        }

        private void UpdateVoxelMesh(VoxelMesh mesh)
        {
            _voxelGeometry = VAOLoader.FromMesh(mesh.DefaultMesh, _voxelShader);
            var loc = _voxelShader.GetResourceLocation(ShaderResourceType.Attribute, "uv3d");
            _voxelGeometry.SetAttribute(loc, mesh.TexCoord3D.ToArray(), OpenGl4.VertexAttribPointerType.Float, 3);
            loc = _voxelShader.GetResourceLocation(ShaderResourceType.Attribute, "voxelId");
            _voxelGeometry.SetAttribute(loc, mesh.VoxelId.ToArray(), OpenGl4.VertexAttribPointerType.Float, 1);

            _amountBuffer = new BufferObject(OpenGl4.BufferTarget.ShaderStorageBuffer);
            _amountBuffer.Set(mesh.MaterialAmount.ToArray(), OpenGl4.BufferUsageHint.StaticCopy);
            _idBuffer = new BufferObject(OpenGl4.BufferTarget.ShaderStorageBuffer);
            _idBuffer.Set(mesh.MaterialId.ToArray(), OpenGl4.BufferUsageHint.StaticCopy);

        }

        private void UpdateDepthMesh(DefaultMesh mesh)
        {
            _depthGeometry = VAOLoader.FromMesh(mesh, _depthShader);
        }

        private void UpdateRaytraceMesh()
        {
            DefaultMesh mesh = new DefaultMesh();
            mesh.Add(Meshes.CreateCubeWithNormals(_voxelSize).Transform(Matrix4x4.CreateTranslation(_raytraceVoxelPosition)));

            _raytraceGeometry = VAOLoader.FromMesh(mesh, _raytraceShader);
        }

        private void CreateWorldGround()
        {
            DefaultMesh mesh = new DefaultMesh();

            Vector3I negativeWorldSize = ((-_worldSize / 2) * Constant.ChunkSize);
            negativeWorldSize.Y = 0;
            Vector3I positiveWorldSize = ((_worldSize / 2) * Constant.ChunkSize);
            positiveWorldSize.Y = _worldSize.Y * Constant.ChunkSizeY;

            mesh.Position.Add(new Vector3(positiveWorldSize.X - 0.5f, -0.5f, negativeWorldSize.Z - 0.5f)); //0
            mesh.Position.Add(new Vector3(positiveWorldSize.X - 0.5f, -0.5f, positiveWorldSize.Z - 0.5f)); //1
            mesh.Position.Add(new Vector3(negativeWorldSize.X - 0.5f, -0.5f, positiveWorldSize.Z - 0.5f)); //2
            mesh.Position.Add(new Vector3(negativeWorldSize.X - 0.5f, -0.5f, negativeWorldSize.Z - 0.5f)); //3

            mesh.TexCoord.Add(new Vector2(0, 0));
            mesh.TexCoord.Add(new Vector2(_worldSize.X * Constant.ChunkSizeX, 0));
            mesh.TexCoord.Add(new Vector2(_worldSize.X * Constant.ChunkSizeX, _worldSize.Z * Constant.ChunkSizeZ));
            mesh.TexCoord.Add(new Vector2(0, _worldSize.Z * Constant.ChunkSizeZ));

            mesh.Normal.Add(-Vector3.UnitY);
            mesh.Normal.Add(-Vector3.UnitY);
            mesh.Normal.Add(-Vector3.UnitY);
            mesh.Normal.Add(-Vector3.UnitY);

            mesh.IDs.Add(0);
            mesh.IDs.Add(2);
            mesh.IDs.Add(1);
            mesh.IDs.Add(0);
            mesh.IDs.Add(3);
            mesh.IDs.Add(2);

            mesh = mesh.Transform(Matrix4x4.CreateScale(_voxelSize));

            _groundGeometry = VAOLoader.FromMesh(mesh, _geometryShader);
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

        private void InitializeMaterialTextures()
        {
            _materialTextureArray.Activate();
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.Rgba8, 512, 512, _registry.MaterialIds.Count);
            int textureId = 0;
            foreach (int materialId in _registry.MaterialIds)
            {
                _textureIDs.Add(materialId, textureId);
                LoadSubImage3D(_registry.GetMaterialInfo(materialId).Texture, textureId);
                textureId++;
            }
            _materialTextureArray.Filter = TextureFilterMode.Mipmap;
            _materialTextureArray.WrapFunction = TextureWrapFunction.Repeat;
            _materialTextureArray.Deactivate();
        }

        private void LoadSubImage3D(Bitmap bitmap, int level)
        {
            var buffer = bitmap.ToBuffer();
            OpenGl4.GL.TexSubImage3D(_materialTextureArray.Target, 0, 0, 0, level, bitmap.Width, bitmap.Height, 1, OpenGl4.PixelFormat.Bgra, OpenGl4.PixelType.UnsignedByte, buffer);
        }
    }
}
