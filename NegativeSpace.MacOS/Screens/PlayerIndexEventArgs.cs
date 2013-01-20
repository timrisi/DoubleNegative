using System;
using Microsoft.Xna.Framework;

namespace NegativeSpace
{
	public class PlayerIndexEventArgs : EventArgs
	{	
		public PlayerIndex PlayerIndex;

		public PlayerIndexEventArgs (PlayerIndex playerIndex)
		{
			PlayerIndex = playerIndex;
		}
	}
}

