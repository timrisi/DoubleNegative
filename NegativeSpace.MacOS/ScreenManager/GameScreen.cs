using System;
using Microsoft.Xna.Framework;

namespace NegativeSpace
{
	public enum ScreenState
	{
		TransitionOn,
		Active,
		TransitionOff,
		Hidden
	}

	public abstract class GameScreen 
	{
		public bool IsPopup;
		public TimeSpan TransitionOnTime = TimeSpan.Zero;
		public TimeSpan TransitionOffTime = TimeSpan.Zero;
		public float TransitionPosition = 1f;

		public float TransitionAlpha
		{
			get { return 1f - TransitionPosition; }
		}

		public ScreenState ScreenState = ScreenState.TransitionOn;
		public bool IsExiting = false;

		public bool IsActive {
			get {
				return !otherScreenHasFocus &&
					(ScreenState == ScreenState.Active ||
				        ScreenState == ScreenState.TransitionOn);
			}
		}

		bool otherScreenHasFocus;
		public ScreenManager ScreenManager;
		public PlayerIndex? ControllingPlayer;

		public GameScreen ()
		{
		}

		public virtual void LoadContent ()
		{
		}

		public virtual void UnloadContent ()
		{
		}

		public virtual void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			this.otherScreenHasFocus = otherScreenHasFocus;

			if (IsExiting) {
				ScreenState = ScreenState.TransitionOff;

				if (UpdateTransition (gameTime, TransitionOffTime, 1))
					ScreenManager.RemoveScreen (this);
			} else if (coveredByOtherScreen) {
				if (UpdateTransition (gameTime, TransitionOffTime, 1))
					ScreenState = ScreenState.TransitionOff;
				else
					ScreenState = ScreenState.Hidden;
			} else {
				if (UpdateTransition (gameTime, TransitionOnTime, -1))
					ScreenState = ScreenState.TransitionOn;
				else
					ScreenState = ScreenState.Active;
			}

		}

		bool UpdateTransition (GameTime gameTime, TimeSpan time, int direction)
		{
			float transitionDelta;

			if (time == TimeSpan.Zero)
				transitionDelta = 1;
			else
				transitionDelta = (float)gameTime.ElapsedGameTime.Milliseconds / time.Milliseconds;

			TransitionPosition += transitionDelta * direction;

			if ((direction < 0 && TransitionPosition <= 0) ||
			    (direction > 0 && TransitionPosition >= 1)) {
				TransitionPosition = MathHelper.Clamp (TransitionPosition, 0, 1);
				return false;
			}

			return true;

		}

		public virtual void HandleInput (InputState input)
		{
		}

		public virtual void Draw (GameTime gameTime)
		{
		}

		public void ExitScreen ()
		{
			if (TransitionOffTime == TimeSpan.Zero)
				ScreenManager.RemoveScreen (this);
			else
				IsExiting = true;
		}
	}
}

