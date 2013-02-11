using System;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace NegativeSpace
{
	public class GameplayScreen : GameScreen
	{
		ContentManager content;
		SpriteFont gameFont;
		
		Texture2D levelTexture;

		List <Character> redTeam;
		List <Character> blueTeam;
		List <Character> currentTeam {
			get {
				return currentTurn == Color.Red ? redTeam : blueTeam;
			}
		}

		List <Character> opposingTeam {
			get {
				return opponent == Color.Blue ? blueTeam : redTeam;
			}
		}

		List <Character> characters {
			get {
				return redTeam.Union (blueTeam).ToList ();
			}
		}

		int activeIndex;
		int ActiveIndex {
			get { return activeIndex; }
			set {
				activeIndex = value;
				if (activeIndex >= currentTeam.Count)
					activeIndex = 0;
			}
		}
		Texture2D deformTexture;
		Color[] levelData;
		Color[] deformData;
		Vector2 deformPosition;
		MouseState currentMouseState;
		Color deformColor;
		Random random = new Random ();

		Color currentTurn = Color.Red;
		Color opponent {
			get {
				return currentTurn == Color.Red ? Color.Blue : Color.Red;
			}
		}

		float pauseAlpha;

		public GameplayScreen ()
		{
			TransitionOnTime = TimeSpan.FromSeconds (1.5);
			TransitionOffTime = TimeSpan.FromSeconds (0.5);

			levelData = new Color [800 * 600];
			deformData = new Color [100 * 100];

			redTeam = new List<Character> ();
			redTeam.Add (new Character (Color.Red, false) { Position = new Vector2 (0, 0) });
			redTeam [0].IsActive = true;
			ActiveIndex = 0;
			redTeam.Add (new Character (Color.Red, true) { Position = new Vector2 (100, 400) });

			blueTeam = new List<Character> ();
			blueTeam.Add (new Character (Color.Blue, false) { Position = new Vector2 (540, 0) });
			blueTeam.Add (new Character (Color.Blue, true) { Position = new Vector2 (500, 400) });
		}

		public override void LoadContent ()
		{
			if (content == null)
				content = new ContentManager (ScreenManager.Game.Services, "Content");

			gameFont = content.Load <SpriteFont> ("gamefont");
			// TODO: use this.Content to load your game content here eg.
			levelTexture = content.Load<Texture2D> ("level");
			levelTexture.GetData (levelData);

			foreach (var red in redTeam)
				red.LoadContent (content);
			foreach (var blue in blueTeam)
				blue.LoadContent (content);

			deformTexture = content.Load<Texture2D> ("deform");
			deformTexture.GetData (deformData);

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

			foreach (var character in characters)
				character.Update (gameTime, levelData);
		}

		public override void HandleInput (InputState input)
		{
			if (input == null)
				throw new ArgumentNullException ("input");
			
			// Look up inputs for the active player profile.
			int playerIndex = (int)ControllingPlayer.Value;
			
			KeyboardState keyboardState = input.CurrentKeyboardStates [playerIndex];
			GamePadState gamePadState = input.CurrentGamePadStates [playerIndex];
			MouseState mouseState = input.CurrentMouseState;

			// The game pauses either if the user presses the pause button, or if
			// they unplug the active gamepad. This requires us to keep track of
			// whether a gamepad was ever plugged in, because we don't want to pause
			// on PC if they are playing with a keyboard and have no gamepad at all!
			bool gamePadDisconnected = !gamePadState.IsConnected &&
				input.GamePadWasConnected [playerIndex];
			
			if (input.IsPauseGame (ControllingPlayer) || gamePadDisconnected)
				ScreenManager.AddScreen (new PauseMenuScreen (), ControllingPlayer);
			else {
				PlayerIndex index;
				if (input.IsNewKeyPress (Keys.Tab, null, out index)) {
					currentTeam [ActiveIndex].IsActive = false;
					ActiveIndex += 1;
					currentTeam [ActiveIndex].IsActive = true;
				}

				if (input.IsNewKeyPress (Keys.RightShift, null, out index)) {
					currentTeam [ActiveIndex].IsActive = false;
					currentTurn = opponent;
					ActiveIndex = 0;
					currentTeam [ActiveIndex].IsActive = true;
				}

				foreach (var character in characters)
					character.HandleInput (input, levelData);

				deformPosition = new Vector2 (mouseState.X - 50, mouseState.Y - 50);
				
				if (levelData [(int)(deformPosition.X + 50 + (deformPosition.Y + 50) * 800)] == Color.White)
					deformColor = Color.Black;
				else
					deformColor = Color.White;

				if (input.DidLeftMouseClick ())
					deformLevel ();
			}
		}

		public override void Draw (GameTime gameTime)
		{
			// This game has a blue background. Why? Because!
			ScreenManager.GraphicsDevice.Clear (ClearOptions.Target,
			                                   Color.CornflowerBlue, 0, 0);
			
			// Our player and enemy are both actually just text strings.
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			
			spriteBatch.Begin ();
			
			// draw the logo
			spriteBatch.Draw (levelTexture, new Vector2 (0, 0), Color.White);

			foreach (var character in characters)
				character.Draw (spriteBatch);

			spriteBatch.Draw (deformTexture, new Vector2 (deformPosition.X, deformPosition.Y), deformColor);

			spriteBatch.End ();
			
			// If the game is transitioning on or off, fade it out to black.
			if (TransitionPosition > 0 || pauseAlpha > 0) {
				float alpha = MathHelper.Lerp (1f - TransitionAlpha, 1f, pauseAlpha / 2);
				
				ScreenManager.FadeBackBufferToBlack (alpha);
			}
		}

		void deformLevel ()
		{
			levelTexture.GetData (levelData);
			
			for (int x = 0; x < deformTexture.Width; x++) {
				for (int y = 0; y < deformTexture.Height; y++) {
					if (deformData [x + y * 100].A != 0 && deformPosition.X + x < 800 && deformPosition.Y + y < 600)
						levelData [(int)(deformPosition.X + x + (deformPosition.Y + y) * 800)] = deformColor;
				}
			}
			
			levelTexture.SetData (levelData);
		}
	}
}

