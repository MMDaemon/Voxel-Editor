using System.Collections.Generic;
using System.Numerics;
using DMS.Geometry;
using DMS.OpenGL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using VoxelEditor.ViewModel;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.View
{
    internal class EditorVisual
    {
        private Shader _voxelShader;
        private Shader _raytraceShader;
        private Shader _addShader;

        private VAO _voxelGeometry;
        private VAO _raytraceGeometry;

        private FBO _renderToTexture;
        private FBO _renderToTextureWithDepth;

        private float _voxelSize;
        private readonly Dictionary<Vector3I, Mesh> _chunkMeshes;

        private bool _raytraceCollided;
        private Vector3 _raytraceCollisionPosition;

        public string VoxelShaderName => nameof(_voxelShader);
        public string RaytraceShaderName => nameof(_raytraceShader);
        public string AddShaderName => nameof(_addShader);

        public EditorVisual()
        {
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.CullFace);

            _chunkMeshes = new Dictionary<Vector3I, Mesh>();
            _raytraceCollided = false;
            _raytraceCollisionPosition = Vector3.Zero;
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
            }
        }

        public void Render(EditorViewModel viewModel)
        {
            float[] cam = viewModel.CameraMatrix.ToArray();
            _raytraceCollided = viewModel.RaytraceCollided;
            _raytraceCollisionPosition = viewModel.RayTraceCollisionPosition;

            Texture raytraceTexture = RenderRaytraceOnTexture(cam);

            _voxelSize = viewModel.VoxelSize;
            CalculateChunkMeshes(viewModel.Chunks);

            Texture voxelTexture = RenderVoxelTexture(cam);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _addShader.Activate();
            GL.ActiveTexture(TextureUnit.Texture0);
            voxelTexture.Activate();
            GL.Uniform1(_addShader.GetUniformLocation("image1"), TextureUnit.Texture0 - TextureUnit.Texture0);

            GL.ActiveTexture(TextureUnit.Texture1);
            raytraceTexture.Activate();
            GL.Uniform1(_addShader.GetUniformLocation("image2"), TextureUnit.Texture1 - TextureUnit.Texture0);

            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            raytraceTexture.Deactivate();
            voxelTexture.Deactivate();
            _addShader.Deactivate();
        }

        public void Resize(int width, int height)
        {
            _renderToTexture = new FBO(Texture.Create(width, height, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float));
            _renderToTextureWithDepth = new FBOwithDepth(Texture.Create(width, height, PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float));
        }

        private Texture RenderVoxelTexture(float[] cam)
        {
            UpdateVoxelMesh();

            _renderToTextureWithDepth.Activate();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Color4(Color4.Transparent);
            _voxelShader.Activate();
            GL.UniformMatrix4(_voxelShader.GetUniformLocation("camera"), 1, false, cam);
            if (_voxelGeometry.IDLength > 0)
            {
                _voxelGeometry.Draw();
            }
            _voxelShader.Deactivate();

            _renderToTextureWithDepth.Deactivate();
            return _renderToTextureWithDepth.Texture;
        }

        private Texture RenderRaytraceOnTexture(float[] cam)
        {
            UpdateRaytraceMesh();

            _renderToTexture.Activate();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (_raytraceCollided)
            {
                GL.Color4(Color4.Transparent);
                _raytraceShader.Activate();
                GL.UniformMatrix4(_voxelShader.GetUniformLocation("camera"), 1, false, cam);
                _raytraceGeometry.Draw();
                _raytraceShader.Deactivate();
            }

            _renderToTexture.Deactivate();
            return _renderToTexture.Texture;
        }

        private void UpdateVoxelMesh()
        {
            Mesh mesh = new Mesh();

            foreach (KeyValuePair<Vector3I, Mesh> chunkMesh in _chunkMeshes)
            {
                mesh.Add(chunkMesh.Value.Transform(Matrix4x4.CreateTranslation((Vector3)(chunkMesh.Key*Constant.ChunkSize))).Transform(Matrix4x4.CreateScale(_voxelSize)));
            }

            _voxelGeometry = VAOLoader.FromMesh(mesh, _voxelShader);
        }

        private void UpdateRaytraceMesh()
        {
            Mesh mesh = Meshes.CreateCubeWithNormals(_voxelSize).Transform(Matrix4x4.CreateTranslation(_raytraceCollisionPosition));
            _raytraceGeometry = VAOLoader.FromMesh(mesh, _raytraceShader);
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
