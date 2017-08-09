using DMS.Geometry;

namespace DMS.OpenGL
{
	/// <summary>
	/// Supports sprite sheets with rectangular sprites
	/// </summary>
	public class SpriteSheet
	{
		public SpriteSheet(Texture tex, uint spritesPerRow, uint spritesPerColumn
			, float spriteBoundingBoxWidth = 1.0f, float spriteBoundingBoxHeight = 1.0f)
		{
			this.tex = tex;
			this.tex.FilterMipmap();
			this.spritesPerRow = spritesPerRow;
			this.spritesPerColumn = spritesPerColumn;
			this.spriteBoundingBoxWidth = spriteBoundingBoxWidth;
			this.spriteBoundingBoxHeight = spriteBoundingBoxHeight;
		}

		public SpriteSheet(Texture tex, uint spritesPerLine
			, float spriteBoundingBoxWidth = 1.0f, float spriteBoundingBoxHeight = 1.0f)
		{
			this.tex = tex;
			this.tex.FilterMipmap();
			this.spritesPerRow = spritesPerLine;
			this.spritesPerColumn = spritesPerLine;
			this.spriteBoundingBoxWidth = spriteBoundingBoxWidth;
			this.spriteBoundingBoxHeight = spriteBoundingBoxHeight;
		}

		public Box2D CalcSpriteTexCoords(uint spriteID)
		{
			return CalcSpriteTexCoords(spriteID, SpritesPerRow, SpritesPerColumn, SpriteBoundingBoxWidth, SpriteBoundingBoxHeight);
		}

		public static Box2D CalcSpriteTexCoords(uint spriteID, uint spritesPerRow, uint spritesPerColumn
			, float spriteBoundingBoxWidth = 1.0f, float spriteBoundingBoxHeight = 1.0f)
		{
			uint row = spriteID / spritesPerRow;
			uint col = spriteID % spritesPerRow;

			float centerX = (col + 0.5f) / spritesPerRow;
			float centerY = 1.0f - (row + 0.5f) / spritesPerColumn;
			float width = spriteBoundingBoxWidth / spritesPerRow;
			float height = spriteBoundingBoxHeight / spritesPerColumn;
			
			return new Box2D(centerX - 0.5f * width, centerY - 0.5f * height, width, height);
		}

		public void Activate()
		{
			tex.Activate();
		}

		public void Deactivate()
		{
			tex.Deactivate();
		}

		public void Draw(uint spriteID, Box2D rectangle)
		{
			Box2D texCoords = CalcSpriteTexCoords(spriteID);
			rectangle.DrawTexturedRect(texCoords);
		}

		public float SpriteBoundingBoxWidth
		{
			get
			{
				return spriteBoundingBoxWidth;
			}
		}

		public float SpriteBoundingBoxHeight
		{
			get
			{
				return spriteBoundingBoxHeight;
			}
		}

		public uint SpritesPerRow
		{
			get
			{
				return spritesPerRow;
			}
		}

		public uint SpritesPerColumn
		{
			get
			{
				return spritesPerColumn;
			}
		}

		public Texture Tex
		{
			get
			{
				return tex;
			}
		}

		private readonly float spriteBoundingBoxWidth;
		private readonly float spriteBoundingBoxHeight;
		private readonly uint spritesPerRow;
		private readonly uint spritesPerColumn;
		private readonly Texture tex;
	}
}
