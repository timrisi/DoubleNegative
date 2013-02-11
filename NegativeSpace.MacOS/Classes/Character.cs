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
		enum State
		{
			Walking,
			Jumping,
			Falling
		}

		Vector2 lastDirection = new Vector2 (1, 0);
		Vector2 direction = new Vector2 (0, 0);
		float gravity = 20f;
		float ground = -20f;
		float moveSpeed = 60;
		float startingHeight;
		State state;

		bool inverted = false;
		Texture2D target; 
		Vector2 targetPosition;

		// The current position of the sprite
		public Vector2 Position;
		public Vector2 Velocity = new Vector2 (0, 0);
		public Color color;
		public bool IsActive = false;

		double power = 0;

		double angle = 0;

		Color groundColor = Color.Black;

		// The texture object used when drawing the sprite
		Texture2D spriteTexture;

		//The size of the Sprite
		public Rectangle Size;

	
		Point bottomMiddle {
			get {
				return inverted ? new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y)) :
					new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + Size.Height));
			}
		}

		Point belowMiddle {
			get {
				return inverted ? new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + 1)) :
					new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + Size.Height - 1));
			}
		}

		Point aboveMiddle {
			get {
				return inverted ? new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + Size.Height + 1)) : 
					new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y - 1));
					
			}
		}

		Point leftCorner {
			get {
				return inverted ? new Point ((int)Position.X, (int)Position.Y + Size.Height) : 
					new Point ((int)Position.X, (int)Position.Y);
			}
		}

		Point rightCorner {
			get {
				return inverted ? new Point ((int)Position.X + Size.Width, (int)Position.Y + Size.Height) :
					new Point ((int)(Position.X + Size.Width), (int)Position.Y);
			}
		}


		public Character (Color color, bool inverted)
		{
			this.color = color;
			this.inverted = inverted;
			if (inverted) {
				gravity *= -1;
				ground *= -1;
				groundColor = Color.White;
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

			if (direction.X != 0)
				lastDirection = direction;
			direction.X = 0;

			if (IsActive) {
				if (state == State.Walking && getColor (leftCorner, levelData) != groundColor &&
					input.IsKeyPressed (Keys.Left, null, out playerIndex))
					direction.X = -1;
				else if (state == State.Walking && getColor (rightCorner, levelData) != groundColor &&
					input.IsKeyPressed (Keys.Right, null, out playerIndex))
					direction.X = 1;

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
			}
		}

		public void Update (GameTime gameTime, Color[] levelData)
		{
			if (state == State.Jumping)
				updateJump (gameTime, levelData);

			if (IsActive) {
				if (state == State.Walking)
					Velocity.X = moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * direction.X;
				targetPosition = Position + targetLocation (80);
			}

			if (getColor (belowMiddle, levelData) == groundColor)
				Velocity.Y += ground * (float)gameTime.ElapsedGameTime.TotalSeconds;

			Velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
			Position += Velocity;

			if (getColor (belowMiddle, levelData) == groundColor) {
				Velocity.Y = 0;
				state = State.Walking;
			}

			if (Velocity.Y != 0)
				state = State.Falling;

			while (getColor (belowMiddle, levelData) == groundColor) {
				Position.Y += ground * (float)gameTime.ElapsedGameTime.TotalSeconds;
				state = State.Walking;
			}

			/*if (inverted) {
				Point topMiddle = new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + 1));

				if (levelData [topMiddle.X + topMiddle.Y * 800] == groundColor)
					Velocity.Y += ground * (float)gameTime.ElapsedGameTime.TotalSeconds;

				Velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
				Position += Velocity;
				state = State.Falling;

				if (levelData [topMiddle.X + topMiddle.Y * 800] == groundColor) {
					Velocity.Y = 0;
					state = State.Walking;
				}

				while (topMiddle.Y < 0 ||
				       levelData [topMiddle.X + topMiddle.Y * 800] == groundColor) {
					Position.Y += ground * (float)gameTime.ElapsedGameTime.TotalSeconds;
					topMiddle = new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + 1));
					state = State.Walking;
				}
			} else {
				Point bottomMiddle = new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + Size.Height - 1));

				if (levelData [bottomMiddle.X + bottomMiddle.Y * 800] == groundColor)
					Velocity.Y += ground * (float)gameTime.ElapsedGameTime.TotalSeconds;

				Velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
				Position += Velocity;

				if (levelData [bottomMiddle.X + bottomMiddle.Y * 800] == groundColor) {
					Velocity.Y = 0;
					state = State.Walking;
				}

				while (bottomMiddle.Y  >= 600 || 
				       levelData [bottomMiddle.X + bottomMiddle.Y * 800] == groundColor) {
					Position.Y += ground * (float)gameTime.ElapsedGameTime.TotalSeconds;
					bottomMiddle = new Point ((int)(Position.X + Size.Width / 2), (int)(Position.Y + Size.Height - 1));
					state = State.Walking;
				}
			}*/
		}

		void jump ()
		{
			state = State.Jumping;

			Velocity.Y = inverted ? 6f : -6f;
		}

		void updateJump (GameTime gameTime, Color[] levelData)
		{
			if (direction.X != 0)
				Velocity.X = moveSpeed * 1.5f * (float)gameTime.ElapsedGameTime.TotalSeconds * direction.X;
			else
				Velocity.X = moveSpeed * 1.5f * (float)gameTime.ElapsedGameTime.TotalSeconds * lastDirection.X;

			if (levelData [aboveMiddle.X + aboveMiddle.Y * 800] == groundColor) {
				Velocity.Y = 0;
			}
		}

		Color getColor (Point point, Color[] levelData)
		{
			return levelData [point.X + point.Y * 800];
		}

		Vector2 targetLocation (double power)
		{
			double x = power * Math.Cos (angle);
			double y = power * Math.Sin (angle);

			if (direction.X < 0 || (direction.X == 0 && lastDirection.X < 0))
				x = -x;

			return new Vector2 ((float)x, (float)y);
		}

		void fire ()
		{

		}
	}
}
