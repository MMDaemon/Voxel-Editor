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
using VoxelUtils.Shared;
using VoxelUtils.Visual;

namespace VoxelEditor.View
{
    internal class EditorVisual
    {
        private Shader _voxelShader;
        private Shader _raytraceShader;
        private Shader _addShader;
        private Shader _crosshairsShader;

        private readonly Texture _crosshairs;
        private readonly TextureFont _font;

        private VAO _voxelGeometry;
        private VAO _raytraceGeometry;

        private readonly FBO[] _renderToTexture;
        private readonly FBO[] _renderToTextureWithDepth;

        private int _width;
        private int _height;
        private float _aspect;

        private float _voxelSize;
        private Vector3I _worldSize;
        private Vector3 _raytraceVoxelPosition;
        private Vector3 _raytraceHitPosition;
        private readonly Dictionary<Vector3I, Mesh> _chunkMeshes;

        public string VoxelShaderName => nameof(_voxelShader);
        public string RaytraceShaderName => nameof(_raytraceShader);
        public string AddShaderName => nameof(_addShader);
        public string CrosshairsShaderName => nameof(_crosshairsShader);

        public EditorVisual()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            _chunkMeshes = new Dictionary<Vector3I, Mesh>();
            _raytraceVoxelPosition = Vector3.Zero;
            _raytraceHitPosition = Vector3.Zero;
            _renderToTexture = new FBO[3];
            _renderToTextureWithDepth = new FBO[1];

            _crosshairs = TextureLoader.FromBitmap(Resourcen.FadenkreuzBW);
            _font = new TextureFont(TextureLoader.FromBitmap(Resourcen.Roboto), 16, 0, 1, 1, .9f);
        }

        public void ShaderChanged(string name, Shader shader)
        {
            switch (name)
            {
                case nameof(_voxelShader):
                    _voxelShader = shader;
                    if (!ReferenceEquals(shader, null))
                    {
                        UpdateVoxelMesh();
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
                case nameof(_crosshairsShader):
                    _crosshairsShader = shader;
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
            _renderToTextureWithDepth[0] = new FBOwithDepth(Texture.Create(width, height, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float));
        }

        public void Render(EditorViewModel viewModel)
        {
            float[] cam = viewModel.CameraMatrix.ToArray();

            _voxelSize = viewModel.VoxelSize;
            _worldSize = viewModel.WorldSize;
            _raytraceVoxelPosition = viewModel.RaytraceVoxelPosition;
            _raytraceHitPosition = viewModel.RaytraceHitPosition;

            CalculateChunkMeshes(viewModel.Chunks);

            Texture mainTexture = RenderOnTexture(delegate { RenderMain(cam, viewModel.RaytraceCollided); }, 1);
            Texture guiTexture = RenderOnTexture(delegate { RenderGUI(); }, 2);

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

        private Texture RenderOnTextureWithDepth(Action render)
        {
            _renderToTextureWithDepth[0].Activate();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ActiveTexture(TextureUnit.Texture0);

            render();

            _renderToTextureWithDepth[0].Deactivate();
            return _renderToTextureWithDepth[0].Texture;
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

        private void RenderMain(float[] cam, bool raytraceCollided)
        {
            Texture raytraceTexture = RenderOnTexture(delegate { RenderRaytrace(cam, raytraceCollided); }, 0);
            Texture voxelTexture = RenderOnTextureWithDepth(delegate { RenderVoxels(cam); });

            RenderAddedTextures(voxelTexture, raytraceTexture, 0.5f);
        }

        private void RenderGUI()
        {
            GL.Color3(Color.White);
            float crosshairsSize = 70.0f;

            _crosshairs.Activate();
            RenderFunctions.DrawRect(-crosshairsSize / (2 * (float)_width), -crosshairsSize / (2 * (float)_height), crosshairsSize / (float)_width, crosshairsSize / (float)_height);
            _crosshairs.Deactivate();

            GL.PushMatrix();
            GL.Scale(1 / _aspect, 1, 1);

            float width1 = _font.Width("Material: Stone", 0.07f);
            float width2 = _font.Width("Amount: 1/32", 0.07f);
            float width = width1 > width2 ? width1 : width2;
            GL.Color4(new Color4(1, 1, 1, 0.9f));
            RenderFunctions.DrawRect(-_aspect, 0.86f, width, 0.14f);

            GL.Color3(Color.Black);
            _font.Print(-_aspect, 0.93f, 0, 0.07f, "Material: Stone");
            _font.Print(-_aspect, 0.86f, 0, 0.07f, "Amount: 1/32");

            GL.PopMatrix();

        }

        private void RenderVoxels(float[] cam)
        {
            UpdateVoxelMesh();

            GL.Color4(Color4.Transparent);
            _voxelShader.Activate();
            GL.UniformMatrix4(_voxelShader.GetUniformLocation("camera"), 1, false, cam);
            if (_voxelGeometry.IDLength > 0)
            {
                _voxelGeometry.Draw();
            }
            _voxelShader.Deactivate();
        }

        private void RenderRaytrace(float[] cam, bool raytraceCollided)
        {
            UpdateRaytraceMesh();

            if (raytraceCollided)
            {
                GL.Color4(Color4.Transparent);
                _raytraceShader.Activate();
                GL.UniformMatrix4(_voxelShader.GetUniformLocation("camera"), 1, false, cam);
                _raytraceGeometry.Draw();
                _raytraceShader.Deactivate();
            }
        }

        private void UpdateVoxelMesh()
        {
            Mesh mesh = CreateWorldGround();

            foreach (KeyValuePair<Vector3I, Mesh> chunkMesh in _chunkMeshes)
            {
                mesh.Add(chunkMesh.Value.Transform(Matrix4x4.CreateTranslation((Vector3)(chunkMesh.Key * Constant.ChunkSize))).Transform(Matrix4x4.CreateScale(_voxelSize)));
            }

            _voxelGeometry = VAOLoader.FromMesh(mesh, _voxelShader);
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

            mesh.position.List.Add(new Vector3(positiveWorldSize.X, -0.5f, negativeWorldSize.Z)); //0
            mesh.position.List.Add(new Vector3(positiveWorldSize.X, -0.5f, positiveWorldSize.Z)); //1
            mesh.position.List.Add(new Vector3(negativeWorldSize.X, -0.5f, positiveWorldSize.Z)); //2
            mesh.position.List.Add(new Vector3(negativeWorldSize.X, -0.5f, negativeWorldSize.Z)); //3

            mesh.normal.List.Add(Vector3.UnitY);
            mesh.normal.List.Add(Vector3.UnitY);
            mesh.normal.List.Add(Vector3.UnitY);
            mesh.normal.List.Add(Vector3.UnitY);

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
