using System.Numerics;
using DMS.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace VoxelUtils.Visual
{
    public static class RenderFunctions
    {
        public static void DrawTexturedRect(Vector2 position, Vector2 size, Texture tex)
        {
            //the texture has to be enabled before use
            tex.Activate();
            GL.Begin(PrimitiveType.Quads);
            //when using textures we have to set a texture coordinate for each vertex
            //by using the TexCoord command BEFORE the Vertex command
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(position.X, position.Y);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(position.X + size.X, position.Y);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(position.X + size.X, position.Y + size.Y);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(position.X, position.Y + size.Y);
            GL.End();
            //the texture is disabled, so no other draw calls use this texture
            tex.Deactivate();
        }
    }
}
