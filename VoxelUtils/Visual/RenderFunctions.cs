using OpenTK.Graphics.OpenGL;

namespace VoxelUtils.Visual
{
    public static class RenderFunctions
    {
        public static void DrawRect(float xPos, float yPos, float xSize, float ySize)
        {
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(xPos, yPos);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(xPos + xSize, yPos);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(xPos + xSize, yPos + ySize);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(xPos, yPos + ySize);
            GL.End();
        }
    }
}
