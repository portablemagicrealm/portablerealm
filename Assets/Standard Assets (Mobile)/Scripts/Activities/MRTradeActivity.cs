//
// MRTradeActivity.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2014 Steve Jakab
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;

namespace PortableRealm
{
	
public class MRTradeActivity : MRActivity
{
	#region Constants

	enum eState
	{
		Start,
		SelectBuySell,
		SelectLeader
	}

	#endregion

	#region Methods

	public MRTradeActivity() : base(MRGame.eActivity.Trade)
	{
		mState = eState.Start;
	}

	protected override void InternalUpdate()
	{
		if (mState == eState.Start)
		{
			Executed = true;
			if (Owner is MRCharacter && Owner.CanExecuteActivity(this))
			{
				// there needs to be a non-hired native leader on the location
				MRILocation location = Owner.Location;
				foreach (MRIGamePiece piece in location.Pieces.Pieces)
				{
					if (piece is MRNative)
					{
						MRNative native = (MRNative)piece;
						if (native.MemberNumber == 0 && !native.IsHired)
						{
							mLeaders.Add(native);
						}
					}
				}
				if (mLeaders.Count > 0)
				{
					Executed = false;
					mState = eState.SelectBuySell;
					MRMainUI.TheUI.DisplaySelectionDialog("Trade", null, new string[] {"Buy", "Sell", "Cancel"}, BuySellCallback);
				}
			}
		}
	}

	/// <summary>
	/// Callback for the player selecting whether they want to buy or sell.
	/// </summary>
	/// <param name="button">Button.</param>
	private void BuySellCallback(int button)
	{
		if (button == 2)
		{
			// cancel
			Executed = true;
			return;
		}

		mBuying = (button == 0);
		if (mLeaders.Count == 1)
		{
			mTrader = mLeaders[0];
			ActivateTrade();
			return;
		}
		// player needs to choose whom to trade with
		string[] groups = new string[mLeaders.Count];
		for (int i = 0; i < mLeaders.Count; ++i)
		{
			groups[i] = mLeaders[i].Group.ToString();
		}
		mState = eState.SelectLeader;
		MRMainUI.TheUI.DisplaySelectionDialog("Select Group", null, groups, SelectLeaderCallback);
	}

	/// <summary>
	/// Callback for the player selecting which native group they want to trade with.
	/// </summary>
	/// <param name="button">Button.</param>
	private void SelectLeaderCallback(int button)
	{
		if (button >= 0 && button < mLeaders.Count)
		{
			mTrader = mLeaders[button];
			ActivateTrade();
		}
		else
		{
			Executed = true;
		}
	}

	/// <summary>
	/// Start the actual trading process.
	/// </summary>
	private void ActivateTrade()
	{
		if (mBuying)
		{
			MRGame.TheGame.AddUpdateEvent(new MRBuyEvent((MRCharacter)Owner, mTrader));
		}
		else
		{
			MRGame.TheGame.AddUpdateEvent(new MRSellEvent((MRCharacter)Owner, mTrader));
		}
		Executed = true;
	}

	#endregion

	#region Members

	private eState mState;
	private bool mBuying;
	private MRNative mTrader;
	private IList<MRNative> mLeaders = new List<MRNative>();

	#endregion
}

}