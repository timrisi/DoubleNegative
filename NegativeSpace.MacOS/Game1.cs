#region File Description
//-----------------------------------------------------------------------------
// ScreenManagerGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

#endregion

namespace NegativeSpace
{
	/// <summary>
	/// Default Project Template
	/// </summary>
	public class Game1 : Game
	{

		#region Fields
		GraphicsDeviceManager graphics;
		ScreenManager screenManager;

		int bufferWidth = 800;
		int bufferHeight = 600;
		#endregion

		#region Initialization

		public Game1 ()
		{
			Content.RootDirectory = "Content";

			graphics = new GraphicsDeviceManager (this);
			graphics.PreferredBackBufferWidth = bufferWidth;
			graphics.PreferredBackBufferHeight = bufferHeight;
			graphics.IsFullScreen = false;

			screenManager = new ScreenManager (this);

			Components.Add (screenManager);

			screenManager.AddScreen (new BackgroundScreen (), null);
			screenManager.AddScreen (new MainMenuScreen (), null);
		}

		/// <summary>
		/// This is called when the game should draw itself. 
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			// Clear the backbuffer
			graphics.GraphicsDevice.Clear (Color.Black);

			base.Draw (gameTime);
		}
		#endregion
	}
}
