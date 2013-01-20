#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace NegativeSpace
{
	public class ScreenManager : DrawableGameComponent
	{
		#region Fields
		List <GameScreen> screens = new List <GameScreen> ();
		List <GameScreen> screensToUpdate = new List <GameScreen> ();
		InputState input = new InputState ();
		public SpriteBatch SpriteBatch;
		public SpriteFont Font;
		Texture2D blankTexture;
		bool isInitialized;
		bool traceEnabled;
		#endregion

		public ScreenManager (Game game) : base (game)
		{
		}

		public override void Initialize ()
		{
			base.Initialize ();

			Game.IsMouseVisible = true;

			isInitialized = true;
		}

		protected override void LoadContent ()
		{
			ContentManager content = Game.Content;

			SpriteBatch = new SpriteBatch (GraphicsDevice);
			Font = content.Load <SpriteFont> ("menufont");
			blankTexture = content.Load <Texture2D> ("blank");

			foreach (var screen in screens)
				screen.LoadContent ();
		}

		protected override void UnloadContent ()
		{
			foreach (var screen in screens)
				screen.UnloadContent ();
		}

		public override void Update (GameTime gameTime)
		{
			input.Update ();

			screensToUpdate.Clear ();

			foreach (var screen in screens)
				screensToUpdate.Add (screen);

			bool otherScreenHasFocus = !Game.IsActive;
			bool coveredByOtherScreen = false;

			while (screensToUpdate.Count > 0) {
				GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

				screensToUpdate.RemoveAt (screensToUpdate.Count - 1);

				screen.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);

				if (screen.ScreenState == ScreenState.TransitionOn ||
				    screen.ScreenState == ScreenState.Active) {
					if (!otherScreenHasFocus) {
						screen.HandleInput (input);
						otherScreenHasFocus = true;
					}

					if (!screen.IsPopup)
						coveredByOtherScreen = true;
				}
			}

			if (traceEnabled)
				traceScreens ();
		}

		void traceScreens ()
		{
			List<string> screenNames = new List<string> ();

			foreach (var screen in screens)
				screenNames.Add (screen.GetType ().Name);

			Console.WriteLine (string.Join (", ", screenNames.ToArray ()));
		}

		public override void Draw (GameTime gameTime)
		{
			foreach (var screen in screens) {
				if (screen.ScreenState == ScreenState.Hidden)
					continue;

				screen.Draw (gameTime);
			}
		}

		public void AddScreen (GameScreen screen, PlayerIndex? controllingPlayer)
		{
			screen.ControllingPlayer = controllingPlayer;
			screen.ScreenManager = this;
			screen.IsExiting = false;

			if (isInitialized)
				screen.LoadContent ();

			screens.Add (screen);
		}

		public void RemoveScreen (GameScreen screen)
		{
			if (isInitialized)
				screen.UnloadContent ();

			screens.Remove (screen);
			screensToUpdate.Remove (screen);
		}

		public GameScreen [] GetScreens ()
		{
			return screens.ToArray ();
		}

		public void FadeBackBufferToBlack (float alpha)
		{
			Viewport viewport = GraphicsDevice.Viewport;

			SpriteBatch.Begin ();

			SpriteBatch.Draw (blankTexture,
			                  new Rectangle (0, 0, viewport.Width, viewport.Height),
			                  Color.Black * alpha);

			SpriteBatch.End ();
		}
	}
}