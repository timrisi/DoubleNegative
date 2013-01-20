using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Input;

namespace NegativeSpace
{
	public class GameplayScreen : GameScreen
	{
		ContentManager content;
		SpriteFont gameFont;
		
		Texture2D levelTexture;
		Texture2D stickmanTexture;
		Texture2D deformTexture;
		Color [] levelColor;
		Color [] deformData;
		Vector2 deformPosition;
		MouseState currentMouseState;
		Color deformColor;

		float pauseAlpha;

		public GameplayScreen ()
		{
			TransitionOnTime = TimeSpan.FromSeconds(1.5);
			TransitionOffTime = TimeSpan.FromSeconds(0.5);

			levelColor = new Color [800*600];
			deformData = new Color [100*100];
			//IsMouseVisible = true;
		}

		public override void LoadContent ()
		{
			if (content == null)
				content = new ContentManager(ScreenManager.Game.Services, "Content");

			gameFont = content.Load <SpriteFont> ("gamefont");
			// TODO: use this.Content to load your game content here eg.
			levelTexture = content.Load<Texture2D> ("level");
			levelTexture.GetData (levelColor);
			stickmanTexture = content.Load<Texture2D> ("stickman"); 
			deformTexture = content.Load<Texture2D> ("deform");
			deformTexture.GetData (deformData);

			Thread.Sleep (1000);

			ScreenManager.Game.ResetElapsedTime ();
		}

		public override void UnloadContent ()
		{
			content.Unload ();
		}

		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update (gameTime, otherScreenHasFocus, false);

			if (coveredByOtherScreen)
				pauseAlpha = Math.Min (pauseAlpha + 1f / 32, 1);
			else
				pauseAlpha = Math.Max (pauseAlpha - 1f / 32, 0);
		}

		public override void HandleInput(InputState input)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			
			// Look up inputs for the active player profile.
			int playerIndex = (int)ControllingPlayer.Value;
			
			KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
			GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];
			MouseState mouseState = input.CurrentMouseState;

			// The game pauses either if the user presses the pause button, or if
			// they unplug the active gamepad. This requires us to keep track of
			// whether a gamepad was ever plugged in, because we don't want to pause
			// on PC if they are playing with a keyboard and have no gamepad at all!
			bool gamePadDisconnected = !gamePadState.IsConnected &&
				input.GamePadWasConnected[playerIndex];
			
			if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
				ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
			else
			{
				deformPosition = new Vector2 (mouseState.X - 50, mouseState.Y - 50);
				
				if (levelColor [(int) (deformPosition.X + 50 + (deformPosition.Y + 50) * 800)] == Color.White)
					deformColor = Color.Black;
				else
					deformColor = Color.White;

				if (input.DidLeftMouseClick ())
					deformLevel ();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			// This game has a blue background. Why? Because!
			ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
			                                   Color.CornflowerBlue, 0, 0);
			
			// Our player and enemy are both actually just text strings.
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			
			spriteBatch.Begin();
			
			// draw the logo
			spriteBatch.Draw (levelTexture, new Vector2 (0, 0), Color.White);
			spriteBatch.Draw (stickmanTexture, new Vector2 (20, 50), Color.Black);
			spriteBatch.Draw (deformTexture, new Vector2 (deformPosition.X, deformPosition.Y), deformColor);

			spriteBatch.End();
			
			// If the game is transitioning on or off, fade it out to black.
			if (TransitionPosition > 0 || pauseAlpha > 0)
			{
				float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
				
				ScreenManager.FadeBackBufferToBlack(alpha);
			}
		}

		void deformLevel ()
		{
			levelTexture.GetData (levelColor);
			
			for (int x = 0; x < deformTexture.Width; x++) {
				for (int y = 0; y < deformTexture.Height; y++) {
					if (deformData [x + y*100].A != 0)
						levelColor [(int) (deformPosition.X + x + (deformPosition.Y + y) * 800)] = deformColor;
				}
			}
			
			levelTexture.SetData (levelColor);
		}
	}
}

