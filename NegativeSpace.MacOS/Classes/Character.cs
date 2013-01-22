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
			Jumping,
			Falling
		}

		Vector2 gravity = new Vector2 (0, 2);
		Vector2 ground = new Vector2 (0, -1);
		Vector2 moveSpeed = new Vector2 (.75f, 0);
		float startingHeight;
		State state;
		bool inverted = false;

		// The current position of the sprite
		public Vector2 Position;
		public Color color;
		public bool IsActive = false;

		public Color opposite {
			get { return color == Color.White ? Color.Black : Color.White; }
		}

		// The texture object used when drawing the sprite
		Texture2D spriteTexture;

		//The size of the Sprite
		public Rectangle Size;

		public Character (Color color)
		{
			this.color = color;
			if (color == Color.White) {
				gravity *= -1;
				ground *= -1;
				inverted = true;
			}

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
			if (!inverted)
				spriteBatch.Draw (spriteTexture, Position, color);
			else
				spriteBatch.Draw (spriteTexture, 
				                  Position, 
				                  null, 
				                  color, 
				                  0f, //(float)Math.PI, 
				                  new Vector2 (0, 0), //new Vector2 (0, -spriteTexture.Height), //new Vector2 (spriteTexture.Width, spriteTexture.Height), 
				                  new Vector2 (1, 1), 
				                  SpriteEffects.FlipVertically, 
				                  0);
		}

		//Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
		public void Update (GameTime gameTime, InputState input, Color[] levelData)
		{

		}

		public void HandleInput (InputState input, Color[] levelData)
		{
			PlayerIndex playerIndex;

			if (state == State.Jumping)
				updateJump ();

			//Point leftMiddle = new Point ((int)Position.X, (int) (Position.Y + Size.Height / 2));
			//Point rightMiddle = new Point ((int) (Position.X + Size.Width), (int) (Position.Y + Size.Height / 2));
			Point leftTop = new Point ((int)Position.X, (int)Position.Y);
			Point rightTop = new Point ((int)(Position.X + Size.Width), (int)Position.Y);
			Point leftBottom = new Point ((int)Position.X, (int)Position.Y + Size.Height);
			Point rightBottom = new Point ((int)Position.X + Size.Width, (int)Position.Y + Size.Height);

			if (IsActive) {
				if (inverted) {
					if (/*state == State.Walking && */
					    levelData [leftBottom.X + leftBottom.Y * 800] != color &&
						input.IsKeyPressed (Keys.A, null, out playerIndex)) {
						Position -= moveSpeed;
					}
					if (Position.X < 0)
						Position.X = 0;
					
					if (/*state == State.Walking && */
					    levelData [rightBottom.X + rightBottom.Y * 800] != color &&
						input.IsKeyPressed (Keys.D, null, out playerIndex)) {
						Position += moveSpeed;
					}
					if (Position.X + Size.Width > 800)
						Position.X = 800 - Size.Width;
				} else {
					if (/*state == State.Walking && */
					levelData [leftTop.X + leftTop.Y * 800] != color &&
						input.IsKeyPressed (Keys.A, null, out playerIndex)) {
						Position -= moveSpeed;
					}
					if (Position.X < 0)
						Position.X = 0;

					if (/*state == State.Walking && */
					levelData [rightTop.X + rightTop.Y * 800] != color &&
						input.IsKeyPressed (Keys.D, null, out playerIndex)) {
						Position += moveSpeed;
					}
					if (Position.X + Size.Width > 800)
						Position.X = 800 - Size.Width;
				}

				if (state == State.Walking &&
					input.IsNewKeyPress (Keys.Space, null, out playerIndex))
					jump ();
			}

			if (state != State.Jumping) {
				Position += gravity;
				state = State.Falling;
			}

			if (inverted) {
				Point topMiddle = new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + 1));
				while (topMiddle.X - Size.Width / 2 >= 0 &&
				       topMiddle.X + Size.Width / 2 <= 800 &&
				       topMiddle.Y - Size.Height / 2 >= 0 &&
				       topMiddle.Y + Size.Height / 2 <= 600 &&
				       levelData [topMiddle.X + topMiddle.Y * 800] == color) {
					Position+= ground;
					topMiddle.Y += (int)ground.Y;
					state = State.Walking;
				}
			} else {
				Point bottomMiddle = new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + Size.Height - 1));
				while (bottomMiddle.X - Size.Height / 2 >= 0 &&
			       bottomMiddle.X + Size.Width / 2 <= 800 &&
			       bottomMiddle.Y - Size.Height / 2 >= 0 &&
			       bottomMiddle.Y + Size.Height / 2 <= 600 &&
			       levelData [bottomMiddle.X + bottomMiddle.Y * 800] == color) {
					Position += ground;
					bottomMiddle.Y += (int)ground.Y;
					state = State.Walking;
				}
			}
		}

		void jump ()
		{
			state = State.Jumping;
			startingHeight = Position.Y;
			Position -= gravity;
		}

		void updateJump ()
		{
			Position -= gravity;

			if (!inverted && Position.Y < startingHeight - 60)
				state = State.Falling;
			else if (inverted && Position.Y > startingHeight + 60)
				state = State.Falling;
		}
	}
}
