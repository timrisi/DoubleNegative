using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NegativeSpace
{
	public class MenuEntry
	{
		public string Text;
		float selectionFade;
		public Vector2 Position;
	
		public event EventHandler<PlayerIndexEventArgs> Selected;

		protected internal virtual void OnSelectEntry (PlayerIndex playerIndex)
		{
			if (Selected != null)
				Selected (this, new PlayerIndexEventArgs (playerIndex));
		}

		public MenuEntry (string text)
		{
			Text = text;
		}

		public virtual void Update (MenuScreen screen, bool isSelected, GameTime gameTime)
		{
			float fadeSpeed = (float) gameTime.ElapsedGameTime.TotalSeconds * 4;

			if (isSelected)
				selectionFade = Math.Min (selectionFade + fadeSpeed, 1);
			else
				selectionFade = Math.Max (selectionFade - fadeSpeed, 0);
		}

		public virtual void Draw (MenuScreen screen, bool isSelected, GameTime gameTime)
		{
			Color color = isSelected ? Color.Yellow : Color.White;
			double time = gameTime.ElapsedGameTime.TotalSeconds;
			float pulsate = (float) Math.Sin (time * 6) + 1;
			float scale = 1 + pulsate * 0.05f * selectionFade;

			color *= screen.TransitionAlpha;

			ScreenManager screenManager = screen.ScreenManager;
			SpriteBatch spriteBatch = screenManager.SpriteBatch;
			SpriteFont font = screenManager.Font;

			Vector2 origin = new Vector2 (0, font.LineSpacing / 2);

			spriteBatch.DrawString (font, Text, Position, color, 0,
			                        origin, scale, SpriteEffects.None, 0);
		}

		public virtual int GetHeight (MenuScreen screen)
		{
			return screen.ScreenManager.Font.LineSpacing;
		}

		public virtual int GetWidth(MenuScreen screen)
		{
			return (int) screen.ScreenManager.Font.MeasureString(Text).X;
		}
	}
}

