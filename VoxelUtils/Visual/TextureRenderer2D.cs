using System.Numerics;
using Zenseless.Geometry;
using Zenseless.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace VoxelUtils.Visual
{
    public class TextureRenderer2D
    {
        private readonly Texture _texture;
        private readonly Shader _shader;
        private readonly VAO _geometry;

        public TextureRenderer2D(Vector2 position, Vector2 size, Texture texture, Shader shader)
        {
            _texture = texture;
            _shader = shader;

            DefaultMesh mesh = new DefaultMesh();

            mesh.Position.Add(new Vector3(position.X, position.Y, 0)); //0
            mesh.Position.Add(new Vector3(position.X + size.X, position.Y, 0)); //1
            mesh.Position.Add(new Vector3(position.X + size.X, position.Y + size.Y, 0)); //2
            mesh.Position.Add(new Vector3(position.X, position.Y + size.Y, 0)); //3

            mesh.TexCoord.Add(new Vector2(0.0f, 0.0f));
            mesh.TexCoord.Add(new Vector2(1.0f, 0.0f));
            mesh.TexCoord.Add(new Vector2(1.0f, 1.0f));
            mesh.TexCoord.Add(new Vector2(0.0f, 1.0f));

            mesh.IDs.Add(0);
            mesh.IDs.Add(2);
            mesh.IDs.Add(1);
            mesh.IDs.Add(0);
            mesh.IDs.Add(3);
            mesh.IDs.Add(2);

            _geometry = VAOLoader.FromMesh(mesh, shader);
        }

        public void Render()
        {
            GL.Disable(EnableCap.CullFace);
            _shader.Activate();
            _texture.Activate();
            _geometry.Draw();
            _texture.Deactivate();
            _shader.Deactivate();
            GL.Enable(EnableCap.CullFace);
        }
    }
}
