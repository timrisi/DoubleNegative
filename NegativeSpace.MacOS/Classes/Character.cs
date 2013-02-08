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

		enum Direction {
			Left,
			Right
		}

		Vector2 gravity = new Vector2 (0, 2);
		Vector2 ground = new Vector2 (0, -1);
		Vector2 moveSpeed = new Vector2 (.75f, 0);
		float startingHeight;
		State state;
		Direction direction = Direction.Right;
		bool inverted = false;
		Texture2D target; 
		Vector2 targetPosition;

		// The current position of the sprite
		public Vector2 Position;
		public Color color;
		public bool IsActive = false;

		double power = 0;

		double angle = 0;

		Color groundColor = Color.Black;
		Color invertedGround = Color.White;

		// The texture object used when drawing the sprite
		Texture2D spriteTexture;

		//The size of the Sprite
		public Rectangle Size;

		public Character (Color color, bool inverted)
		{
			this.color = color;
			this.inverted = inverted;
			if (inverted) {
				gravity *= -1;
				ground *= -1;
			}

			state = State.Falling;
		}

		//Load the texture for the sprite using the Content Pipeline
		public void LoadContent (ContentManager contentManager)
		{
			spriteTexture = contentManager.Load<Texture2D> ("stickman");
			Size = new Rectangle (0, 0, (int)(spriteTexture.Width), (int)(spriteTexture.Height));
			target = contentManager.Load<Texture2D> ("target");
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

			if (IsActive) {
				spriteBatch.Draw (target, targetPosition, color);

				if (power > 0) {
					Vector2 powerPos = Position + targetLocation (power);
					Rectangle rect = new Rectangle (0, 0, 10, 10);

					spriteBatch.Draw (target, powerPos, Color.Green);
				}
			}
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
					    levelData [leftBottom.X + leftBottom.Y * 800] != invertedGround &&
						input.IsKeyPressed (Keys.Left, null, out playerIndex)) {
						Position -= moveSpeed;
						direction = Direction.Left;
					}
					if (Position.X < 0)
						Position.X = 0;
					
					if (/*state == State.Walking && */
					    levelData [rightBottom.X + rightBottom.Y * 800] != invertedGround &&
						input.IsKeyPressed (Keys.Right, null, out playerIndex)) {
						Position += moveSpeed;
						direction = Direction.Right;
					}
					if (Position.X + Size.Width > 800)
						Position.X = 800 - Size.Width;
				} else {
					if (/*state == State.Walking && */
					levelData [leftTop.X + leftTop.Y * 800] != groundColor &&
						input.IsKeyPressed (Keys.Left, null, out playerIndex)) {
						Position -= moveSpeed;
						direction = Direction.Left;
					}
					if (Position.X < 0)
						Position.X = 0;

					if (/*state == State.Walking && */
					levelData [rightTop.X + rightTop.Y * 800] != groundColor &&
						input.IsKeyPressed (Keys.Right, null, out playerIndex)) {
						Position += moveSpeed;
						direction = Direction.Right;
					}
					if (Position.X + Size.Width > 800)
						Position.X = 800 - Size.Width;
				}

				if (state == State.Walking &&
					input.IsNewKeyPress (Keys.Enter, null, out playerIndex))
					jump ();

				if (input.IsKeyPressed (Keys.Up, null, out playerIndex)) {
					angle -= Math.PI / 128;
					if (angle < -Math.PI / 2)
						angle = -Math.PI / 2;
				}

				if (input.IsKeyPressed (Keys.Down, null, out playerIndex)) {
					angle += Math.PI / 128;
					if (angle > Math.PI / 2)
						angle = Math.PI / 2;
				}

				if (input.IsKeyPressed (Keys.Space, null, out playerIndex) && power < 80) {
					power += 1;
					if (power == 80) {
						fire ();
						power = 0;
					}
				}

				if (input.KeyWasReleased (Keys.Space, null, out playerIndex)) {
					fire ();
					power = 0;
				}

				targetPosition = Position + targetLocation(80);
			}

			if (state != State.Jumping) {
				Position += gravity;
				state = State.Falling;
			}

			if (inverted) {
				Point topMiddle = new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + 1));
				while (topMiddle.Y < 0 ||
				       levelData [topMiddle.X + topMiddle.Y * 800] == invertedGround) {
					Position+= ground;
					topMiddle.Y += (int)ground.Y;
					state = State.Walking;
				}
			} else {
				Point bottomMiddle = new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + Size.Height - 1));

				while (bottomMiddle.Y  >= 600 || 
			       levelData [bottomMiddle.X + bottomMiddle.Y * 800] == groundColor) {
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

		Vector2 targetLocation (double power)
		{
			double x = power * Math.Cos (angle);
			double y = power * Math.Sin (angle);

			if (direction == Direction.Left)
				x = -x;

			return new Vector2 ((float) x, (float) y);
		}

		void fire ()
		{

		}
	}
}
