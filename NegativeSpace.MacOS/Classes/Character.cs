using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NegativeSpace
{
	public class Character
	{
		enum State {
			Walking,
			Falling
		}

		Vector2 gravity = new Vector2 (0, 2);
		Vector2 ground = new Vector2 (0, -1);
		Vector2 moveSpeed = new Vector2 (.75f, 0);
		State state;

		// The current position of the sprite
		public Vector2 Position;
		public Color color;

		// The texture object used when drawing the sprite
		Texture2D spriteTexture;

		//The size of the Sprite
		public Rectangle Size;

		public Character (Color color)
		{
			this.color = color;
			state = State.Falling;
		}

		//Load the texture for the sprite using the Content Pipeline
		public void LoadContent (ContentManager contentManager)
		{
			spriteTexture = contentManager.Load<Texture2D> ("stickman");
			Size = new Rectangle (0, 0, (int)(spriteTexture.Width), (int)(spriteTexture.Height));
		}

		//Draw the sprite to the screen
		public virtual void Draw (SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (spriteTexture, Position, color);
		}

		//Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
		public void Update (GameTime gameTime, InputState input, Color[] levelData)
		{

		}

		public void HandleInput (InputState input, Color[] levelData)
		{
			PlayerIndex playerIndex;

			Point leftMiddle = new Point ((int)Position.X, (int) (Position.Y + Size.Height / 2));
			Point rightMiddle = new Point ((int) (Position.X + Size.Width), (int) (Position.Y + Size.Height / 2));
			Point leftTop = new Point ((int)Position.X, (int) Position.Y);
			Point rightTop = new Point ((int) (Position.X + Size.Width), (int) Position.Y);

			if (state == State.Walking && 
			    levelData [leftTop.X + leftTop.Y * 800] != Color.Black &&
			    input.IsKeyPressed (Keys.A, null, out playerIndex)) {
				Position -= moveSpeed;
			}
			if (Position.X < 0)
				Position.X = 0;

			if (state == State.Walking && 
			    levelData [rightTop.X + rightTop.Y * 800] != Color.Black &&
			    input.IsKeyPressed (Keys.D, null, out playerIndex)) {
				Position += moveSpeed;
			}
			if (Position.X + Size.Width > 800)
				Position.X = 800 - Size.Width;

			Position += gravity;
			state = State.Falling;

			Point bottomMiddle = new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + Size.Height - 1));
			while (levelData [bottomMiddle.X + bottomMiddle.Y * 800] == Color.Black) {
				Position += ground;
				bottomMiddle.Y += (int) ground.Y;
				state = State.Walking;
			}
			
		}
	}
}
