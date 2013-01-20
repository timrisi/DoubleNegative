using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NegativeSpace
{
	public class MenuScreen : GameScreen
	{
		public List<MenuEntry> MenuEntries = new List<MenuEntry> ();
		int selectedEntry = 0;
		string menuTitle;

		public MenuScreen (string menuTitle)
		{
			this.menuTitle = menuTitle;

			TransitionOffTime = TimeSpan.FromSeconds (0.5);
			TransitionOnTime = TimeSpan.FromSeconds (0.5);
		}

		public override void HandleInput (InputState input)
		{
			if (input.IsMenuUp (ControllingPlayer)) {
				selectedEntry--;

				if (selectedEntry < 0)
					selectedEntry = MenuEntries.Count - 1;
			}

			if (input.IsMenuDown (ControllingPlayer)) {
				selectedEntry++;

				if (selectedEntry >= MenuEntries.Count)
					selectedEntry = 0;
			}

			PlayerIndex playerIndex;

			if (input.IsMenuSelect (ControllingPlayer, out playerIndex))
				OnSelectEntry (selectedEntry, playerIndex);
			else if (input.IsMenuCancel (ControllingPlayer, out playerIndex))
				OnCancel (playerIndex);
		}

		protected virtual void OnSelectEntry (int entryIndex, PlayerIndex playerIndex)
		{
			MenuEntries [selectedEntry].OnSelectEntry (playerIndex);
		}

		protected virtual void OnCancel (PlayerIndex playerIndex)
		{
			ExitScreen ();
		}

		protected void OnCancel(object sender, PlayerIndexEventArgs e)
		{
			OnCancel(e.PlayerIndex);
		}

		protected virtual void UpdateMenuEntryLocations ()
		{
			float transitionOffset = (float)Math.Pow (TransitionPosition, 2);

			Vector2 position = new Vector2 (0, 175f);

			for (int i = 0; i < MenuEntries.Count; i++) {
				MenuEntry menuEntry = MenuEntries [i];

				position.X = (ScreenManager.GraphicsDevice.Viewport.Width - menuEntry.GetWidth (this)) / 2;

				if (ScreenState == ScreenState.TransitionOn)
					position.X -= transitionOffset * 256;
				else
					position.X += transitionOffset * 512;

				menuEntry.Position = position;

				position.Y += menuEntry.GetHeight (this);
			}
		}

		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);

			for (int i = 0; i < MenuEntries.Count; i++) {
				bool isSelected = IsActive && i == selectedEntry;

				MenuEntries [i].Update (this, isSelected, gameTime);
			}
		}

		public override void Draw (GameTime gameTime)
		{
			UpdateMenuEntryLocations ();

			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			SpriteFont font = ScreenManager.Font;

			spriteBatch.Begin ();

			for (int i = 0; i < MenuEntries.Count; i++)
			{
				MenuEntry menuEntry = MenuEntries [i];
				
				bool isSelected = IsActive && (i == selectedEntry);
				
				menuEntry.Draw (this, isSelected, gameTime);
			}

			float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
			
			// Draw the menu title centered on the screen
			Vector2 titlePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 80);
			Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
			Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
			float titleScale = 1.25f;
			
			titlePosition.Y -= transitionOffset * 100;
			
			spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
			                       titleOrigin, titleScale, SpriteEffects.None, 0);
			
			spriteBatch.End();
		}
	}
}

