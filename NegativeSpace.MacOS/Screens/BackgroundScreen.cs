using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NegativeSpace
{
	public class BackgroundScreen : GameScreen
	{
		ContentManager content;
		Texture2D backgroundTexture;

		public BackgroundScreen ()
		{
			TransitionOffTime = TimeSpan.FromSeconds (0.5);
			TransitionOnTime = TimeSpan.FromSeconds (0.5);
		}

		public override void LoadContent ()
		{
			if (content == null)
				content = new ContentManager (ScreenManager.Game.Services, "Content");

			backgroundTexture = content.Load <Texture2D> ("background");
		}

		public override void UnloadContent ()
		{
			base.UnloadContent ();
		}

		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update (gameTime, otherScreenHasFocus, false);
		}

		public override void Draw (GameTime gameTime)
		{
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
			Rectangle fullscreen = new Rectangle (0, 0, viewport.Width, viewport.Height);

			spriteBatch.Begin ();
			spriteBatch.Draw (backgroundTexture, fullscreen, 
			                  new Color (TransitionAlpha, TransitionAlpha, TransitionAlpha));
			spriteBatch.End ();
		}
	}
}

