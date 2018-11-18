//
// MRBuyEvent.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2016 Steve Jakab
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
using System;
using System.Collections;
using System.Collections.Generic;

namespace PortableRealm
{
	
public class MRBuyEvent : MRUpdateEvent
{
	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.TradeEvent;
		}
	}

	#endregion

	#region Methods

	public MRBuyEvent (MRCharacter character, MRNative leader)
	{
		mCharacter = character;
		mLeader = leader;

		mTradeWindow = (MRTradeWindow)UnityEngine.Object.Instantiate(MRGame.TheGame.tradeWindowPrototype);
		mTradeWindow.transform.parent = MRGame.TheGame.transform;

		mTradeWindow.Seller = leader;
		mTradeWindow.Buyer = character;
		mTradeWindow.PriceMod = 1;

		MRGamePieceStack treasureStack = MRGame.TheGame.TreasureChart.GetNativeTreasures(mLeader.Group);
		List<MRItem> items = new List<MRItem>();
		foreach (MRIGamePiece piece in treasureStack.Pieces)
		{
			if (piece is MRItem)
			{
				items.Add((MRItem)piece);
			}
		}
		mTradeWindow.ItemsAvailable = items;

		MRGame.TheGame.PushView(MRGame.eViews.Trade);
	}

	/// <summary>
	/// Updates this instance.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	public override bool Update ()
	{
		if (mTradeWindow.TradeDone)
		{
			MRGame.TheGame.PopView();
			EndEvent();
		}
		return false;
	}

	public override void EndEvent()
	{
		UnityEngine.Object.Destroy(mTradeWindow.gameObject);
		mTradeWindow = null;
		base.EndEvent();
	}

	#endregion

	#region Members

	private MRCharacter mCharacter;
	private MRNative mLeader;
	private MRTradeWindow mTradeWindow;

	#endregion
}

}