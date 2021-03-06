using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace NegativeSpace
{
	public class InputState
	{
		public const int MaxInputs = 4;
		public readonly KeyboardState[] CurrentKeyboardStates;
		public readonly GamePadState[] CurrentGamePadStates; 
		public MouseState CurrentMouseState;
		
		public readonly KeyboardState[] LastKeyboardStates;             
		public readonly GamePadState[] LastGamePadStates;
		public MouseState LastMouseState;

		public readonly bool [] GamePadWasConnected;

		public InputState ()
		{
			CurrentKeyboardStates = new KeyboardState [MaxInputs];
			CurrentGamePadStates = new GamePadState [MaxInputs];
			
			LastKeyboardStates = new KeyboardState [MaxInputs];
			LastGamePadStates = new GamePadState [MaxInputs];

			GamePadWasConnected = new bool[MaxInputs];
		}

		public void Update ()
		{
			LastMouseState = CurrentMouseState;
			CurrentMouseState = Mouse.GetState ();

			for (int i = 0; i < MaxInputs; i++) {
				LastKeyboardStates [i] = CurrentKeyboardStates [i];
				LastGamePadStates [i] = CurrentGamePadStates [i];

				CurrentKeyboardStates [i] = Keyboard.GetState ((PlayerIndex) i);
				CurrentGamePadStates [i] = GamePad.GetState ((PlayerIndex) i);

				if (CurrentGamePadStates [i].IsConnected)
					GamePadWasConnected [i] = true;
			}
		}

		public bool IsNewKeyPress (Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			if (controllingPlayer.HasValue) {
				playerIndex = controllingPlayer.Value;
				int i = (int)playerIndex;

				return (CurrentKeyboardStates [i].IsKeyDown (key) &&
					LastKeyboardStates [i].IsKeyUp (key));
			} else {
				return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
				        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
				        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
				        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
			}
		}

		public bool IsKeyPressed (Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			if (controllingPlayer.HasValue) {
				playerIndex = controllingPlayer.Value;
				int i = (int)playerIndex;
				
				return (CurrentKeyboardStates [i].IsKeyDown (key));
			} else {
				return (IsKeyPressed(key, PlayerIndex.One, out playerIndex) ||
				        IsKeyPressed(key, PlayerIndex.Two, out playerIndex) ||
				        IsKeyPressed(key, PlayerIndex.Three, out playerIndex) ||
				        IsKeyPressed(key, PlayerIndex.Four, out playerIndex));
			}
		}

		public bool KeyWasReleased (Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			if (controllingPlayer.HasValue) {
				playerIndex = controllingPlayer.Value;
				int i = (int)playerIndex;
				
				return (CurrentKeyboardStates [i].IsKeyUp (key) &&
				        LastKeyboardStates [i].IsKeyDown (key));
			} else {
				return (KeyWasReleased(key, PlayerIndex.One, out playerIndex) ||
				        KeyWasReleased(key, PlayerIndex.Two, out playerIndex) ||
				        KeyWasReleased(key, PlayerIndex.Three, out playerIndex) ||
				        KeyWasReleased(key, PlayerIndex.Four, out playerIndex));
			}
		}

		public bool DidLeftMouseClick ()
		{
			return (LastMouseState.LeftButton == ButtonState.Pressed &&
			        CurrentMouseState.LeftButton == ButtonState.Released);
		}

		public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
		                             out PlayerIndex playerIndex)
		{
			if (controllingPlayer.HasValue)
			{
				// Read input from the specified player.
				playerIndex = controllingPlayer.Value;
				
				int i = (int)playerIndex;
				
				return (CurrentGamePadStates[i].IsButtonDown(button) &&
				        LastGamePadStates[i].IsButtonUp(button));
			}
			else
			{
				// Accept input from any player.
				return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
				        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
				        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
				        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
			}
		}

		public bool IsMenuSelect(PlayerIndex? controllingPlayer,
		                         out PlayerIndex playerIndex)
		{
			return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
			       IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
		}

		public bool IsMenuCancel(PlayerIndex? controllingPlayer,
		                         out PlayerIndex playerIndex)
		{
			return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
		}

		public bool IsMenuUp(PlayerIndex? controllingPlayer)
		{
			PlayerIndex playerIndex;
			
			return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
		}

		public bool IsMenuDown(PlayerIndex? controllingPlayer)
		{
			PlayerIndex playerIndex;
			
			return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
		}

		public bool IsPauseGame(PlayerIndex? controllingPlayer)
		{
			PlayerIndex playerIndex;
			
			return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
			       IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
		}
	}
}

