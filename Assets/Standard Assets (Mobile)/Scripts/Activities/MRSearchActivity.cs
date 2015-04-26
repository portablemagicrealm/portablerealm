//
// MRSearchActivity.cs
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MRSearchActivity : MRActivity
{
	#region Constants

	public enum eSearchTable
	{
		None,
		Peer,
		Locate,
		Loot,
		ReadRunes,
		MagicSight,
		ToadstoolCircle,
		CryptOfKnight,
		EnchantedMeadow,
	}

	public enum eState
	{
		SelectTable,
		Roll,
		SelectPeerResult,
		SelectLocateResult,
		SelectMagicSightResult,
		SelectClearing,
		SelectedPeerRoll,
		SelectedLocateRoll,
		SelectCharacterLootDestination,
		SelectDenizenLootDestination,
		ShowTwitDiscovered,
		Done
	}

	#endregion

	#region Properties

	public override bool Active
	{
		set{
			if (Active != value && !Executed)
			{
				// set up table options (need to do this before any of the gui functions are run)
				if (value && Owner.Location != null && mLootSites[0] == null)
				{
					int siteIndex = 0;
					int twitIndex = 0;
					foreach (MRIGamePiece piece in Owner.Location.Pieces.Pieces)
					{
						if (piece is MRSiteChit && Owner.CanLootSite((MRSiteChit)piece))
						{
							if (mLootSites[siteIndex] == null)
							{
								mLootSites[siteIndex] = (MRSiteChit)piece;
								mLootSiteNames[siteIndex] = mLootSites[siteIndex].LongName.Substring(0, mLootSites[siteIndex].LongName.IndexOf('\n')).ToLower();
								Debug.Log("set loot site " + siteIndex + " name to " + mLootSiteNames[siteIndex]);

								// see if there is a twit site at the site that can be looted
								MRGamePieceStack stack = MRGame.TheGame.TreasureChart.GetSiteTreasures(mLootSites[siteIndex].SiteType);
								foreach (MRIGamePiece item in stack.Pieces)
								{
									if (item is MRTreasure && ((MRTreasure)item).IsTwiT)
									{
										MRTreasure twit = (MRTreasure)item;
										if (Owner.CanLootTwit(twit))
										{
											mTwitSites[twitIndex] = twit;
											mTwitSiteNames[twitIndex] = twit.Name;
											++twitIndex;
										}
									}
								}
								++siteIndex;
							}
						}
					}
					ShowSelectTable();
				}
			}
			base.Active = value;
		}
	}

	#endregion

	#region Methods

	public MRSearchActivity() : base(MRGame.eActivity.Search)
	{
		mTableSelected = eSearchTable.None;
		for (int i = 0; i < mLootSites.Length; ++i)
		{
			mLootSites[i] = null;
			mLootSiteNames[i] = null;
		}
		for (int i = 0; i < mTwitSites.Length; ++i)
		{
			mTwitSites[i] = null;
			mTwitSiteNames[i] = null;
		}
		mPieceLooted = null;
		mLootSiteSelected = null;
		mLootTwitSelected = null;
	}

	//public override void OnGUI()
	//{
	//	if (!Active)
	//		return;
	//	
	//	switch (mState)
	//	{
	//		case eState.SelectClearing:
	//		{
	//			GUILayout.BeginArea(new Rect(0, 0, Screen.width, 56));
	//			GUILayout.BeginHorizontal();
	//			GUILayout.FlexibleSpace();
	//			GUILayout.Label("Select Clearing", "BigLabel", GUILayout.ExpandHeight(true));
	//			GUILayout.FlexibleSpace();
	//			GUILayout.EndHorizontal();
	//			GUILayout.EndArea();
	//			break;
	//		}
	//	}
	//}

	private void ShowSelectTable()
	{
		mState = eState.SelectTable;

		List<string> buttons = new List<string>();
		buttons.Add("Peer");
		buttons.Add("Locate");
		// if the clearing contains a map chit that has been discovered, add the loot option
		for (int i = 0; i < mLootSites.Length; ++i)
		{
			if (mLootSites[i] != null)
			{
				buttons.Add("Loot " + mLootSiteNames[i]);
			}
		}
		// if the clearing contains a twit treasure site that has been discovered, add the loot option
		for (int i = 0; i < mTwitSites.Length; ++i)
		{
			if (mTwitSites[i] != null)
			{
				buttons.Add("Loot " + mTwitSiteNames[i]);
			}
		}
		// if there are abandoned items in the clearing, they can be looted
		if (Owner.Location.AbandonedItems.Pieces.Count > 0)
		{
			buttons.Add("Loot Abandoned Items");
		}
		MRMainUI.TheUI.DisplaySelectionDialog("Select Table", null, buttons.ToArray(), OnTableSelected);
	}

	private void OnTableSelected(int button)
	{
		if (!(Owner.Location is MRClearing))
		{
			return;
		}

		MRClearing ownerClearing = (MRClearing)Owner.Location;

		if (button == 0)
		{
			Debug.Log("Peer pressed");
			mTableSelected = eSearchTable.Peer;
			if (Owner.Location is MRClearing)
			{
				// if the player is in a mountain clearing, they can peer at any non-cave clearing that
				// is in their hex or an adjacent hex
				if (ownerClearing.type == MRClearing.eType.Mountain)
				{
					mState = eState.SelectClearing;
					MRGame.TheGame.AddUpdateEvent(new MRSelectClearingEvent(null, OnClearingSelected));
				}
				else
				{
					mClearing = ownerClearing;
					mState = eState.Roll;
				}
			}
			return;
		}
		else if (button == 1)
		{
			Debug.Log("Locate pressed");
			MRGame.ShowingUI = false;
			
			mState = eState.Roll;
			mClearing = ownerClearing;
			mTableSelected = eSearchTable.Locate;
			return;
		}
		int i = 0;
		int buttonOffset = 2;
		for (i = 0; i < mLootSites.Length; ++i)
		{
			if (mLootSites[i] == null)
				break;
			if (button == buttonOffset + i)
			{
				mState = eState.Roll;
				mClearing = ownerClearing;
				mLootSiteSelected = mLootSites[i];
				mLootStackSelected = MRGame.TheGame.TreasureChart.GetSiteTreasures(mLootSites[i].SiteType);
				mTableSelected = eSearchTable.Loot;
				return;
			}
		}
		buttonOffset += i;
		for (i = 0; i < mTwitSites.Length; ++i)
		{
			if (mTwitSites[i] == null)
				break;
			if (button == buttonOffset + i)
			{
				mState = eState.Roll;
				mClearing = ownerClearing;
				mLootTwitSelected = mTwitSites[i];
				mLootStackSelected = MRGame.TheGame.TreasureChart.GetTwitTreasures(mTwitSites[i].Id);
				if (mTwitSites[i].Id == MRUtility.IdForName("crypt of the knight"))
					mTableSelected = eSearchTable.CryptOfKnight;
				else if (mTwitSites[i].Id == MRUtility.IdForName("enchanted meadow"))
					mTableSelected = eSearchTable.EnchantedMeadow;
				else if (mTwitSites[i].Id == MRUtility.IdForName("toadstool circle"))
					mTableSelected = eSearchTable.ToadstoolCircle;
				else
				{
					Debug.LogError("Unknown twit treasure site looted: " + mTwitSiteNames[i]);
				}
				return;
			}
		}
		buttonOffset += i;
		if (Owner.Location.AbandonedItems.Pieces.Count > 0 && button == buttonOffset)
		{
			mState = eState.Roll;
			mClearing = ownerClearing;
			mLootSiteSelected = null;
			mLootStackSelected = Owner.Location.AbandonedItems;
			mTableSelected = eSearchTable.Loot;
			return;
		}
		Debug.LogError("Bad select table button " + button);
	}

	private void ShowSelectPeerResult()
	{
		mState = eState.SelectPeerResult;
		MRMainUI.TheUI.DisplaySelectionDialog("Select Result", null, new string[] {
			"Clues and Paths",
			"Hidden enemies and Paths",
			"Hidden enemies",
			"Clues"
		}, OnPeerResultSelected);
	}

	private void OnPeerResultSelected(int button)
	{
		switch (button)
		{
			case 0:
				mSelectedRoll = 2;
				break;
			case 1:
				mSelectedRoll = 3;
				break;
			case 2:
				mSelectedRoll = 4;
				break;
			case 3:
				mSelectedRoll = 5;
				break;
			default:
				Debug.LogError("Bad select peer result button " + button);
				return;
		}
		mState = eState.SelectedPeerRoll;
	}

	private void ShowSelectLocateResult()
	{
		mState = eState.SelectLocateResult;
		MRMainUI.TheUI.DisplaySelectionDialog("Select Result", null, new string[] {
			"Passages and Clues",
			"Passages",
			"Discover Chits"
		}, OnLocateResultSelected);
	}
	
	private void OnLocateResultSelected(int button)
	{
		switch (button)
		{
			case 0:
				mSelectedRoll = 2;
				break;
			case 1:
				mSelectedRoll = 3;
				break;
			case 2:
				mSelectedRoll = 4;
				break;
			default:
				Debug.LogError("Bad select locate result button " + button);
				return;
		}
		mState = eState.SelectedLocateRoll;
	}

	private void SelectLootDestination()
	{
		if (Owner is MRCharacter && ((MRCharacter)Owner).CanActivateItem(mPieceLooted, false))
		{
			mState = eState.SelectCharacterLootDestination;
			MRMainUI.TheUI.DisplaySelectionDialog("You found something!", mPieceLooted.Name, new string[] {
				"Set Active",
				"Set Inactive",
				"Abandon"
			}, OnCharacterLootDestinationSelected);
		}
		else
		{
			mState = eState.SelectDenizenLootDestination;
			MRMainUI.TheUI.DisplaySelectionDialog("You found something!", mPieceLooted.Name, new string[] {
				"Keep",
				"Abandon"
			}, OnDenizenLootDestinationSelected);

		}
	}

	private void OnCharacterLootDestinationSelected(int button)
	{
		switch (button)
		{
			case 0:
				((MRCharacter)Owner).AddInactiveItem(mPieceLooted);
				((MRCharacter)Owner).ActivateItem(mPieceLooted, false);
				break;
			case 1:
				((MRCharacter)Owner).AddInactiveItem(mPieceLooted);
				break;
			case 2:
				mClearing.AbandonedItems.AddPieceToTop(mPieceLooted);
				break;
			default:
				return;
		}
		mState = eState.Done;
		Executed = true;
		if (mLootSiteSelected != null)
			Owner.OnSiteLooted(mLootSiteSelected, true);
		else if (mLootTwitSelected != null)
			Owner.OnTwitLooted(mLootTwitSelected, true);
	}

	private void OnDenizenLootDestinationSelected(int button)
	{
		switch (button)
		{
			case 0:
				Owner.AddItem(mPieceLooted);
				break;
			case 1:
				mClearing.AbandonedItems.AddPieceToTop(mPieceLooted);
				break;
			default:
				return;
		}
		mState = eState.Done;
		Executed = true;
		if (mLootSiteSelected != null)
			Owner.OnSiteLooted(mLootSiteSelected, true);
		else if (mLootTwitSelected != null)
			Owner.OnTwitLooted(mLootTwitSelected, true);
	}

	private void ShowTwitDiscovered()
	{
		mState = eState.ShowTwitDiscovered;
		MRMainUI.TheUI.DisplaySelectionDialog("You found something!", mPieceLooted.Name, new string[] {
			"OK"
		}, OnTwitDiscovered);
	}

	private void OnTwitDiscovered(int button)
	{
		if (mPieceLooted.Id == MRUtility.IdForName("mouldy skeleton"))
		{
			// put the skeleton's treasues on its source stack
			MRGamePieceStack subTreasues = MRGame.TheGame.TreasureChart.GetTwitTreasures(mPieceLooted.Id);
			while (subTreasues.Pieces.Count > 0)
			{
				mPieceLooted.Stack.AddPieceToTop(subTreasues.Pieces[subTreasues.Pieces.Count - 1]);
			}
			// remove the skeleton from the game
			mPieceLooted.Stack.RemovePiece(mPieceLooted);
			mPieceLooted.Layer = LayerMask.NameToLayer("Dummy");
			MRGame.TheGame.RollForCurse(Owner, Owner);
		}
		else if (mPieceLooted.Id == MRUtility.IdForName("remains of thief"))
		{
			// give the sub-treasures to the owner
			MRGamePieceStack subTreasues = MRGame.TheGame.TreasureChart.GetTwitTreasures(mPieceLooted.Id);
			while (subTreasues.Pieces.Count > 0)
			{
				Owner.AddItem((MRItem)subTreasues.Pieces[subTreasues.Pieces.Count - 1]);
			}
			Owner.BaseGold += 20;
			// remove the remains from the game
			mPieceLooted.Stack.RemovePiece(mPieceLooted);
			mPieceLooted.Layer = LayerMask.NameToLayer("Dummy");
			MRGame.TheGame.RollForCurse(Owner, Owner);
		}
		else
		{
			// put the site at the bottom of the stack (unhidden)
			((MRTreasure)mPieceLooted).Hidden = false;
			mPieceLooted.Stack.AddPieceToBottom(mPieceLooted);
			// add it to the character's discovered sites
			Owner.DiscoverTreasure(mPieceLooted.Id);
		}
		mState = eState.Done;
		Executed = true;
	}

	protected override void InternalUpdate()
	{
		if (mState == eState.Roll)
		{
			switch (mTableSelected)
			{
				case eSearchTable.Locate:
					RollForLocateTable();
					break;
				case eSearchTable.Loot:
					RollForLootTable();
					break;
				case eSearchTable.MagicSight:
					RollForMagicSightTable();
					break;
				case eSearchTable.Peer:
					RollForPeerTable();
					break;
				case eSearchTable.ReadRunes:
					RollForRunesTable();
					break;
				case eSearchTable.ToadstoolCircle:
					RollForLootToadstoolCircle();
					break;
				case eSearchTable.CryptOfKnight:
					RollForLootCryptOfKnight();
					break;
				case eSearchTable.EnchantedMeadow:
					RollForLootEnchantedMeadow();
					break;

			}
		}
		else if (mState == eState.SelectedLocateRoll)
			RollForLocateTable();
		else if (mState == eState.SelectedPeerRoll)
			RollForPeerTable();
	}

	private void RollForLocateTable()
	{
		int roll = -1;
		if (mState == eState.SelectedLocateRoll)
			roll = mSelectedRoll;
		else
		{
			if (mDiePool == null)
			{
				mDiePool = Owner.DiePool(MRGame.eRollTypes.SearchLocate);
				mDiePool.RollDice();
			}
			if (mDiePool.RollReady)
			{
				roll = mDiePool.Roll;
				MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
				mDiePool = null;
			}
		}
		if (roll > 0)
		{
			Debug.Log("Search Locate Table roll = " + roll);
			switch (roll)
			{
				case 1:	// choice
					//mState = eState.SelectLocateResult;
					ShowSelectLocateResult();
					break;
				case 2:	// passages and clues
					FindPassages();
					FindClues();
					Executed = true;
					break;
				case 3:	// passages
					FindPassages();
					Executed = true;
					break;
				case 4:	// discover chits
					FindChits();
					Executed = true;
					break;
				case 5:	// nothing
				case 6:	// nothing
					Executed = true;
					break;
			}
		}
	}

	private void RollForLootTable()
	{
		if (mLootStackSelected != null)
		{
			if (mDiePool == null)
			{
				mDiePool = Owner.DiePool(MRGame.eRollTypes.SearchLoot);
				mDiePool.RollDice();
			}
			if (mDiePool.RollReady)
			{
				int roll = mDiePool.Roll;
				MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
				mDiePool = null;
				// temp - set roll 
				//roll = 3;
				Debug.Log("Search Loot Table roll = " + roll);

				if (roll <= mLootStackSelected.Pieces.Count)
				{
					mPieceLooted = (MRItem)(mLootStackSelected.Pieces[roll - 1]);
					if (mPieceLooted.BaseWeight != MRGame.eStrength.Immobile)
						//mState = eState.SelectLootDestination;
						SelectLootDestination();
					else
						//mState = eState.ShowTwitDiscovered;
						ShowTwitDiscovered();
				}
				else
				{
					Debug.Log("No loot");
					if (mLootSiteSelected != null)
						Owner.OnSiteLooted(mLootSiteSelected, false);
					else if (mLootTwitSelected != null)
						Owner.OnTwitLooted(mLootTwitSelected, false);
					Executed = true;
				}
			}
		}
		else
		{
			Debug.LogError("RollForLootTable with no loot stack");
			Executed = true;
		}
	}

	private void RollForLootToadstoolCircle()
	{
		if (mLootStackSelected != null)
		{
			if (mDiePool == null)
			{
				mDiePool = Owner.DiePool(MRGame.eRollTypes.LootToadstoolCircle);
				mDiePool.RollDice();
			}
			if (mDiePool.RollReady)
			{
				int roll = mDiePool.Roll;
				MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
				mDiePool = null;
				Debug.Log("Loot Toadstool Circle Table roll = " + roll);
				Executed = true;
				bool foundNothing = true;
				switch (roll)
				{
					case 1:
					{
						// devil sword
						MRItem sword = MRItem.GetItem(MRUtility.IdForName("devil sword"));
						if (sword != null && sword.Stack == mLootStackSelected)
						{
							mPieceLooted = sword;
							//mState = eState.SelectLootDestination;
							SelectLootDestination();
							Executed = false;
							foundNothing = false;
						}
						break;
					}
					case 2:
						// treasure
						foreach (MRIGamePiece item in mLootStackSelected.Pieces)
						{
							if (item is MRTreasure)
							{
								mPieceLooted = (MRItem)item;
								//mState = eState.SelectLootDestination;
								SelectLootDestination();
								Executed = false;
								foundNothing = false;
								break;
							}
						}
						break;
					case 3:
						// todo: teleport to any cave clearing
						foundNothing = false;
						break;
					case 4:
						// todo: flag: can peer in any clearing for the rest of the day
						foundNothing = false;
						break;
					case 5:
						// todo: power of the pit
						foundNothing = false;
						break;
					case 6:
						// cursed
						MRGame.TheGame.RollForCurse(Owner, Owner);
						foundNothing = false;
						break;
				}
				if (foundNothing)
					MRGame.TheGame.ShowInformationDialog("Nothing Found");
			}
		}
		else
		{
			Debug.LogError("RollForLootToadstoolCircle with no loot stack");
			Executed = true;
		}
	}

	private void RollForLootCryptOfKnight()
	{
		if (mLootStackSelected != null)
		{
			if (mDiePool == null)
			{
				mDiePool = Owner.DiePool(MRGame.eRollTypes.LootCryptOfKnight);
				mDiePool.RollDice();
			}
			if (mDiePool.RollReady)
			{
				int roll = mDiePool.Roll;
				MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
				mDiePool = null;
				// temp - fix roll
				//roll = 1;
				Debug.Log("Loot Crypt of the Knight Table roll = " + roll);
				Executed = true;
				bool foundNothing = true;
				switch (roll)
				{
					case 1:
						// warhorse
						foreach (MRIGamePiece item in mLootStackSelected.Pieces)
						{
							if (item is MRHorse)
							{
								mPieceLooted = (MRItem)item;
								SelectLootDestination();
								Executed = false;
								foundNothing = false;
								break;
							}
						}
						break;
					case 2:
					{
						// T armor
						MRItem armor = MRItem.GetItem(MRUtility.IdForName("tremendous armor"));
						if (armor != null && armor.Stack == mLootStackSelected)
						{
							mPieceLooted = armor;
							SelectLootDestination();
							Executed = false;
							foundNothing = false;
						}
						break;
					}
					case 3:
					{
						// bane sword
						MRItem sword = MRItem.GetItem(MRUtility.IdForName("bane sword"));
						if (sword != null && sword.Stack == mLootStackSelected)
						{
							mPieceLooted = sword;
							SelectLootDestination();
							Executed = false;
							foundNothing = false;
						}
						break;
					}
					case 4:
						// treasure
						foreach (MRIGamePiece item in mLootStackSelected.Pieces)
						{
							if (item is MRTreasure)
							{
								mPieceLooted = (MRItem)item;
								SelectLootDestination();
								Executed = false;
								foundNothing = false;
								break;
							}
						}
						break;
					case 5:
						// gold
						Owner.BaseGold += 1;
						MRGame.TheGame.ShowInformationDialog("Found a gold piece");
						foundNothing = false;
						break;
					case 6:
						MRGame.TheGame.RollForCurse(Owner, Owner);
						foundNothing = false;
						break;
				}
				if (foundNothing)
					MRGame.TheGame.ShowInformationDialog("Nothing Found");
			}
		}
		else
		{
			Debug.LogError("RollForLootCryptOfKnight with no loot stack");
			Executed = true;
		}
	}

	private void RollForLootEnchantedMeadow()
	{
		if (mLootStackSelected != null)
		{
			if (mDiePool == null)
			{
				mDiePool = Owner.DiePool(MRGame.eRollTypes.LootEnchantedMeadow);
				mDiePool.RollDice();
			}
			if (mDiePool.RollReady)
			{
				int roll = mDiePool.Roll;
				MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
				mDiePool = null;
				Debug.Log("Loot Enchanted Meadow Table roll = " + roll);
				Executed = true;
				bool foundNothing = true;
				switch (roll)
				{
					case 1:
						// pony
						foreach (MRIGamePiece item in mLootStackSelected.Pieces)
						{
							if (item is MRHorse)
							{
								mPieceLooted = (MRItem)item;
								//mState = eState.SelectLootDestination;
								SelectLootDestination();
								Executed = false;
								foundNothing = false;
								break;
							}
						}
						break;
					case 2:
					{
						// truesteel sword
						MRItem sword = MRItem.GetItem(MRUtility.IdForName("truesteel sword"));
						if (sword != null && sword.Stack == mLootStackSelected)
						{
							mPieceLooted = sword;
							//mState = eState.SelectLootDestination;
							SelectLootDestination();
							Executed = false;
							foundNothing = false;
						}
						break;
					}
					case 3:
						// wish
						foundNothing = false;
						break;
					case 4:
						// all chits return to active, wither broken
						foundNothing = false;
						break;
					case 5:
						// curse
						MRGame.TheGame.RollForCurse(Owner, Owner);
						foundNothing = false;
						break;
					case 6:
						// nothing
						break;
				}
				if (foundNothing)
					MRGame.TheGame.ShowInformationDialog("Nothing Found");
			}
		}
		else
		{
			Debug.LogError("RollForLootEnchantedMeadow with no loot stack");
			Executed = true;
		}
	}

	private void RollForMagicSightTable()
	{
		if (mDiePool == null)
		{
			mDiePool = Owner.DiePool(MRGame.eRollTypes.SearchMagicSight);
			mDiePool.RollDice();
		}
		if (mDiePool.RollReady)
		{
			int roll = mDiePool.Roll;
			MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
			mDiePool = null;
			Debug.Log("Search Magic Sight Table roll = " + roll);
			Executed = true;
		}
	}

	private void RollForPeerTable()
	{
		int roll = -1;
		if (mState == eState.SelectedPeerRoll)
			roll = mSelectedRoll;
		else
		{
			if (mDiePool == null)
			{
				mDiePool = Owner.DiePool(MRGame.eRollTypes.SearchPeer);
				mDiePool.RollDice();
			}
			if (mDiePool.RollReady)
			{
				roll = mDiePool.Roll;
				MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
				mDiePool = null;
			}
		}
		if (roll > 0)
		{
			Debug.Log("Search Peer Table roll = " + roll);
			switch (roll)
			{
				case 1:	// choice
					ShowSelectPeerResult();
					//mState = eState.SelectPeerResult;
					break;
				case 2:	// clues and paths
					FindClues();
					FindPaths();
					Executed = true;
					break;
				case 3:	// hidden enemies and paths
					FindHiddenEnemies();
					FindPaths();
					Executed = true;
					break;
				case 4:	// hidden enemies
					FindHiddenEnemies();
					Executed = true;
					break;
				case 5:	// clues
					FindClues();
					Executed = true;
					break;
				case 6:	// nothing
					Executed = true;
					break;
			}
		}
	}

	private void RollForRunesTable()
	{
		if (mDiePool == null)
		{
			mDiePool = Owner.DiePool(MRGame.eRollTypes.SearchRunes);
			mDiePool.RollDice();
		}
		if (mDiePool.RollReady)
		{
			int roll = mDiePool.Roll;
			MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
			mDiePool = null;
			Debug.Log("Search Runes Table roll = " + roll);
			Executed = true;
		}
	}

	private void FindChits()
	{
		if (mClearing != null)
		{
			IList<MRMapChit> chits = mClearing.MyTileSide.Tile.MapChits;
			foreach (MRMapChit chit in chits)
			{
				if (chit is MRSiteChit)
				{
					Owner.DiscoverSite(((MRSiteChit)chit).SiteType);
					Debug.Log("Discover site chit " + ((MRSiteChit)chit).SiteType);
				}
			}
		}
		else
			Debug.LogError("Find chits with no clearing set");
	}

	private void FindClues()
	{
		if (mClearing != null)
		{
			
		}
		else
			Debug.LogError("Find chits with no clearing set");
	}

	private void FindHiddenEnemies()
	{
	}

	private void FindPassages()
	{
		if (mClearing != null)
		{
			foreach (MRRoad road in mClearing.Roads)
			{
				if (road != null && road.type == MRRoad.eRoadType.SecretPassage)
				{
					if (!Owner.DiscoveredRoads.Contains(road))
					{
						Debug.Log("Discover hidden passage " + road.Name);
						Owner.DiscoveredRoads.Add(road);
					}
				}
			}
		}
		else
			Debug.LogError("Find passages with no clearing set");
	}

	private void FindPaths()
	{
		if (mClearing != null)
		{
			foreach (MRRoad road in mClearing.Roads)
			{
				if (road != null && road.type == MRRoad.eRoadType.HiddenPath)
				{
					if (!Owner.DiscoveredRoads.Contains(road))
					{
						Debug.Log("Discover secret path " + road.Name);
						Owner.DiscoveredRoads.Add(road);
					}
				}
			}
		}
		else
			Debug.LogError("Find paths with no clearing set");
	}

	// Called when the player selects a clearing
	private void OnClearingSelected(MRClearing clearing)
	{
		if (mState == eState.SelectClearing)
		{
			// the clearing needs to be a non-cave clearing that is in or adjacent to the current hex
			bool clearingValid = false;
			if (clearing.type != MRClearing.eType.Cave)
			{
				MRTile currentTile = Owner.Location.MyTileSide.Tile;
				MRTile clearingTile = clearing.MyTileSide.Tile;
				clearingValid = (currentTile == clearingTile);
				for (int i = 0; i < 6 && !clearingValid; ++i)
				{
					if (currentTile.GetAdjacentTile(i) == clearingTile)
						clearingValid = true;
				}
			}
			if (clearingValid)
			{
				Debug.Log("Mountain Peer valid clearing selected " + clearing.Name);
				mClearing = clearing;
				mState = eState.Roll;
			}
			else
			{
				// try again
				MRGame.TheGame.AddUpdateEvent(new MRSelectClearingEvent(null, OnClearingSelected));
			}
		}
		if (mState != eState.Roll)
			Debug.Log("Mountain Peer invalid clearing selected");
	}

	#endregion

	#region Members

	private eState mState;
	private Rect mSelectionWindow;
	private eSearchTable mTableSelected;
	private MRClearing mClearing;
	private int mSelectedRoll;
	private MRDiePool mDiePool;
	private MRSiteChit mLootSiteSelected;
	private MRTreasure mLootTwitSelected;
	private MRGamePieceStack mLootStackSelected;
	private MRItem mPieceLooted;
	private MRSiteChit[] mLootSites = new MRSiteChit[2];
	private string[] mLootSiteNames = new string[2];
	private MRTreasure[] mTwitSites = new MRTreasure[3];
	private string[] mTwitSiteNames = new string[3];

	#endregion
}

