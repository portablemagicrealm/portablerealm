//
// MRTradeWindow.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2017 Steve Jakab
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
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PortableRealm
{

public class MRTradeWindow : MonoBehaviour, MRITouchable
{
	#region Constants

	private enum eState
	{
		Normal,
		RollTrade,
		Blocked,
		ExecuteBuy,
		TradeBack
	}

	#endregion

	#region Properties

	public MRTradeItem itemPrefab;
	public RectTransform fromContent;
	public RectTransform toContent;
	public TextMesh fromName;
	public TextMesh toName;
	public TextMesh characterGold;
	public SpriteRenderer relationshipIcon;
	public Camera itemCamera;
	public MRGamePieceStack itemSnapshot;

	public bool TradeDone 
	{
		get{
			return mTradeDone;
		}
	}

	public MRControllable Seller
	{
		get{
			return mSeller;
		}

		set{
			mSeller = value;
			fromName.text = mSeller.Name.DisplayName();
			if (mSeller is MRCharacter)
			{
				mGold = ((MRCharacter)mSeller).EffectiveGold;
				characterGold.text = mGold.ToString();
			}
		}
	}

	public MRControllable Buyer
	{
		get{
			return mBuyer;
		}

		set{
			mBuyer = value;
			toName.text = mBuyer.Name.DisplayName();
			if (mBuyer is MRCharacter)
			{
				Gold = ((MRCharacter)mBuyer).EffectiveGold;
			}
		}
	}

	public IList<MRItem> ItemsAvailable
	{
		set{
			mItemsAvailable = value;

			// clear out old items
			foreach (Transform child in fromContent) 
			{
				GameObject.Destroy(child.gameObject);
			}

			// add the new ones
			foreach (MRItem item in mItemsAvailable)
			{
				MRTradeItem newItem = (MRTradeItem)Instantiate(itemPrefab);
				newItem.transform.SetParent(fromContent, false);
				newItem.Item = item;
				newItem.Price = item.CurrentPrice * mPriceMod;
				newItem.ItemCamera = itemCamera;
				newItem.ItemSnapshotStack = itemSnapshot;
			}
		}
	}

	public IList<MRItem> ItemsBought
	{
		set{
			mItemsBought = value;

			// clear out old items
			foreach (Transform child in toContent) 
			{
				GameObject.Destroy(child.gameObject);
			}

			// add the new ones
			foreach (MRItem item in mItemsBought)
			{
				MRTradeItem newItem = (MRTradeItem)Instantiate(itemPrefab);
				newItem.transform.SetParent(toContent, false);
				newItem.Item = item;
				newItem.ItemCamera = itemCamera;
				newItem.ItemSnapshotStack = itemSnapshot;
				if (item == mItemBought) 
				{
					newItem.Price = item.CurrentPrice * mPriceMod;
				}
				else
				{
					newItem.Price = item.CurrentPrice;
				}
			}
		}
	}

	public int Gold
	{
		get{
			return mGold;
		}

		set{
			mGold = value;
			if (mState == eState.TradeBack && mGold > 0)
			{
				// reflect the fact that when trading back the seller does not give change
				characterGold.text = "0";
			}
			else
			{
				characterGold.text = mGold.ToString();
			}
		}
	}

	public int PriceMod
	{
		set{
			mPriceMod = value;
		}
	}

	#endregion

	#region Methods

	public MRTradeWindow ()
	{
		mState = eState.Normal;
		mTradeDone = false;
		mItemsAvailable = new List<MRItem>();
		mItemsBought = new List<MRItem>();
		mPriceMod = 1;
	}

	void Start()
	{
		Transform[] transforms = GetComponentsInChildren<Transform>();
		foreach (Transform transform in transforms)
		{
			GameObject obj = transform.gameObject;
			switch (obj.name)
			{
				case "accept":
					mAcceptButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "cancel":
					mCancelButton = obj.GetComponentInChildren<MRButton>();
					break;
				default:
					break;
			}
		}

		MRCharacter character = mBuyer is MRCharacter ? (MRCharacter)mBuyer : (MRCharacter)mSeller;
		MRNative native = mBuyer is MRNative ? (MRNative)mBuyer : (MRNative)mSeller;
		mTradeRelationship = character.Relationships[native.Group];
		relationshipIcon.sprite = (Sprite)Resources.Load(MRGame.NativeRelationshipIconMap[mTradeRelationship], typeof(Sprite));
	}

	void Update()
	{
		if (MRGame.ShowingUI != true)
		{
			switch (mState)
			{
				case eState.RollTrade:
					RollMeetingTable();
					break;
				case eState.Blocked:
					MRGame.TheGame.ShowInformationDialog("Blocked!", "Trade Result");
					mBuyer.Blocked = true;
					mState = eState.Normal;
					mTradeDone = true;
					break;
				case eState.ExecuteBuy:
					ExecuteBuy();
					break;
				default:
					break;
			}
		}
	}

	void OnItemSelected(MRTradeItem tradeItem)
	{
		Debug.Log("Trade item " + tradeItem.Item.Name + " selected");
		if (tradeItem.transform.parent == fromContent)
			TransferToBuyer(tradeItem);
		else if (tradeItem.transform.parent == toContent)
			TransferToSeller(tradeItem);
	}

	public bool OnTouched(GameObject touchedObject)
	{
		return true;
	}

	public bool OnReleased(GameObject touchedObject)
	{
		return true;
	}

	public bool OnSingleTapped(GameObject touchedObject)
	{
		return true;
	}
	
	public bool OnDoubleTapped(GameObject touchedObject)
	{
		return true;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	public bool OnTouchMove(GameObject touchedObject, float delta_x, float delta_y)
	{
		return true;
	}

	public bool OnButtonActivate(GameObject touchedObject)
	{
		if (touchedObject == mAcceptButton.gameObject)
		{
			if (mGold >= 0)
			{
				if (mState == eState.Normal)
				{
					if (mItemsBought.Count > 0)
					{
						// TODO: assuming that the trade is between one character and one native; when hired natives
						// are implemented this won't be the case
						if (mSeller is MRCharacter)
						{
							MRCharacter character = (MRCharacter)mSeller;
							character.BaseGold = mGold;
							TransferToNatives(mItemsBought);
							mTradeDone = true;
						}
						else
						{
							mItemBought = mItemsBought[0];
							// need to roll on the reaction table to get the final price
							if (mSeller is MRNative && mTradeRelationship < MRGame.eRelationship.Ally)
							{
								// buy drinks first
								mDrinkCost = 0;
								foreach (MRIGamePiece piece in mBuyer.Location.Pieces.Pieces)
								{
									if (piece is MRNative &&
										((MRNative)piece).Group == ((MRNative)mSeller).Group)
									{
										++mDrinkCost;
									}
								}
								MRCharacter character = (MRCharacter)mBuyer;
								if (character.BaseGold >= mDrinkCost)
								{
									MRMainUI.TheUI.DisplayYesNoDialog("Buy drinks (" + mDrinkCost + ")?", "Trade", BoughtDrinks, BoughtDrinks);
								}
								else
								{
									RollMeetingTable();
								}
							}
							else
							{
								RollMeetingTable();
							}
						}
					}
				}
				else if (mState == eState.TradeBack)
				{
					if (Gold >= 0)
					{
						// set the payer gold to 0 because no change is given for tradebacks
						Gold = 0;
						TransferToNatives(mItemsAvailable);
						AcceptTradeCallback(0); 
					}
					mTradeDone = true;
				}
			}
		}
		else if (touchedObject == mCancelButton.gameObject)
		{
			mTradeDone = true;
		}
		return true;
	}

	/// <summary>
	/// Callback for the player choosing to buy drinks before rolling on the trade table.
	/// </summary>
	/// <param name="button">Button.</param>
	private void BoughtDrinks(int button)
	{
		if (button == 0)
		{
			MRCharacter character = (MRCharacter)mBuyer;
			character.BaseGold -= mDrinkCost;
			if (mTradeRelationship < MRGame.eRelationship.Ally)
			{
				++mTradeRelationship;
			}
		}
		RollMeetingTable();
	}

	private void RollMeetingTable()
	{
		if (mDiePool == null)
		{
			mState = eState.RollTrade;
			mDiePool = mBuyer.DiePool(MRGame.eRollTypes.MeetingTrade);
			mDiePool.RollDice();
		}
		else if (mDiePool.RollReady)
		{
			mState = eState.Normal;
			int roll = mDiePool.Roll;
			MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
			mDiePool = null;

			MRTable meetingTable = MRGame.TheGame.Tables["meeting"];
			string result = meetingTable.GetValue(roll - 1, (int)mTradeRelationship); 
			switch (result) 
			{
				case "i":	// insult
					if (mBuyer is MRCharacter)
					{
						MRMainUI.TheUI.DisplayYesNoDialog("Insult! Lose Notoriety?", "Trade Result", "Yes", "No", 
							InsultCallback, InsultCallback);
					}
					else
					{
						mState = eState.Blocked;
					}
					return;
				case "c":	// challenge
					if (mBuyer is MRCharacter && !((MRCharacter)mBuyer).HasCurse(MRGame.eCurses.Disgust))
					{
						MRMainUI.TheUI.DisplayYesNoDialog("Challenge!  Lose Fame?", "Trade Result", "Yes", "No", 
							ChallengeCallback, ChallengeCallback);
					}
					else
					{
						mState = eState.Blocked;
					}
					return;
				case "b":	// block/battle
					mState = eState.Blocked;
					return;
				case "x":	// no deal
					MRGame.TheGame.ShowInformationDialog("No Deal!", "Trade Result");
					break;
				case "o":	// opportunity
					MRGame.TheGame.ShowInformationDialog("Opportunity!", "Trade Result");
					++mTradeRelationship;
					mState = eState.RollTrade;
					return;
				case "t":	// trouble
					MRGame.TheGame.ShowInformationDialog("Trouble!", "Trade Result");
					--mTradeRelationship;
					mState = eState.RollTrade;
					return;
				case "p0":	// boon
					MRMainUI.TheUI.DisplayYesNoDialog("Boon! Accept the boon?", "Trade Result", "Yes", "No", 
							BoonCallback, BoonCallback);
					return;
				default:
					if (result[0] == 'p') 
					{
						mPriceMod = int.Parse(result.Substring(1));
						MRGame.TheGame.ShowInformationDialog("Price x" + mPriceMod, "Trade Result");
						mState = eState.ExecuteBuy;
						return;
					}
					else
					{
						Debug.LogError("Unknown trade result " + result);
					}
					break;
			}
			mTradeDone = true;
		}
	}

	/// <summary>
	/// Executes a buy action with the current item and price modifier for a character. If the character can't 
	/// afford the item, they will be allowed to sell their equipment. 
	/// </summary>
	private void ExecuteBuy()
	{
		if (mBuyer is MRCharacter)
		{
			MRCharacter character = (MRCharacter)mBuyer;
			Gold -= mItemBought.CurrentPrice * mPriceMod;
			if (Gold >= 0)
			{
				MRMainUI.TheUI.DisplayYesNoDialog("Accept Trade?", "Trade Result", "Yes", "No", 
					AcceptTradeCallback, AcceptTradeCallback);
				mState = eState.Normal;
			}
			else
			{
				List<MRItem> characterItems = new List<MRItem>(character.Items);
				characterItems.Add(mItemBought);
				ItemsBought = characterItems;
				ItemsAvailable = new List<MRItem>();
				mState = eState.TradeBack;
			}
		}
	}

	/// <summary>
	/// Callback for an insult result response.
	/// </summary>
	/// <param name="buttonId">Button identifier.</param>
	private void InsultCallback(int buttonId) 
	{
		if (buttonId == 0)
		{
			// player chose to lose notoriety
			MRGame.TheGame.ShowInformationDialog("Lost 5 Notoriety.", "Trade Result");
			MRCharacter character = (MRCharacter)mBuyer;
			character.BaseNotoriety -= 5;
			mTradeDone = true;
		}
		else
		{
			// player chose to be blocked
			mState = eState.Blocked;
		}
	}

	/// <summary>
	/// Callback for a challenge result response.
	/// </summary>
	/// <param name="buttonId">Button identifier.</param>
	private void ChallengeCallback(int buttonId) 
	{
		if (buttonId == 0)
		{
			// player chose to lose fame
			MRGame.TheGame.ShowInformationDialog("Lost 5 Fame.", "Trade Result");
			MRCharacter character = (MRCharacter)mBuyer;
			character.BaseFame -= 5;
			mTradeDone = true;
		}
		else
		{
			// player chose to be blocked
			mState = eState.Blocked;
		}
	}

	/// <summary>
	/// Callback for a boon result response.
	/// </summary>
	/// <param name="buttonId">Button identifier.</param>
	private void BoonCallback(int buttonId) 
	{
		if (buttonId == 0)
		{
			// player accepted the boon, gets item for free at the cost of trading relationship
			MRCharacter character = (MRCharacter)mBuyer;
			MRNative native = (MRNative)mSeller;
			mTradeRelationship = character.Relationships[native.Group];
			if (mTradeRelationship > MRGame.eRelationship.Enemy)
				--mTradeRelationship;
			character.Relationships[native.Group] = mTradeRelationship;
			AcceptTradeCallback(0);
		}
		else
		{
			// payer rejects the boon, cost = x1
			mPriceMod = 1;
			mState = eState.ExecuteBuy;
		}
	}

	/// <summary>
	/// Callback for an accept trade result response.
	/// </summary>
	/// <param name="buttonId">Button identifier.</param>
	private void AcceptTradeCallback(int buttonId) 
	{
		if (buttonId == 0)
		{
			// player accepted the trade
			MRCharacter character = (MRCharacter)mBuyer;
			MRNative native = (MRNative)mSeller;
			character.BaseGold = Gold;
			character.AddInactiveItem(mItemBought);
			if (mItemBought is MRTreasure)
			{
				// some treasures have fame when bought
				MRTreasure treasure = (MRTreasure)mItemBought;
				if (treasure.SellFame > 0 && treasure.SellFameGroup == native.Group)
				{
					character.BaseFame -= treasure.SellFame;
				}
			}
		}
		mTradeDone = true;
	}

	public bool OnPinchZoom(float pinchDelta)
	{
		return true;
	}

	private void TransferToBuyer(MRTradeItem tradeItem)
	{
		// TODO: assuming that the trade is between one character and one native; when hired natives
		// are implemented this won't be the case
		if (mBuyer is MRCharacter)
		{
			if (mState == eState.Normal)
			{
				// characters can only buy one item per action from natives
				if (mItemsBought.Count > 0)
					return;
			}
			else if (mState == eState.TradeBack)
			{
				Gold -= tradeItem.Price;
				mItemsAvailable.Remove(tradeItem.Item);
			}
		}
		else
		{
			Gold += tradeItem.Price;
		}
		mItemsBought.Add(tradeItem.Item);
		tradeItem.transform.SetParent(toContent, false);
	}

	private void TransferToSeller(MRTradeItem tradeItem)
	{
		// TODO: assuming that the trade is between one character and one native; when hired natives
		// are implemented this won't be the case
		if (mBuyer is MRCharacter)
		{
			if (mState == eState.TradeBack)
			{
				// cannot trade back the item being bought
				if (tradeItem.Item == mItemBought)
					return;
				Gold += tradeItem.Price;
				mItemsAvailable.Add(tradeItem.Item);
			}
		}
		else
		{
			Gold -= tradeItem.Price;
		}
		mItemsBought.Remove(tradeItem.Item);
		tradeItem.transform.SetParent(fromContent, false);
	}

	/// <summary>
	/// Transfers items in an item list to the native group in the trade.
	/// </summary>
	/// <param name="itemList">Item list.</param>
	private void TransferToNatives(IList<MRItem> itemList)
	{
		MRCharacter character;
		MRNative native;
		if (mSeller is MRCharacter)
		{
			character = (MRCharacter)mSeller;
			native = (MRNative)mBuyer;
		}
		else
		{
			character = (MRCharacter)mBuyer;
			native = (MRNative)mSeller;
		}
		MRGamePieceStack treasureStack = MRGame.TheGame.TreasureChart.GetNativeTreasures(native.Group);

		foreach (MRItem item in itemList)
		{
			character.RemoveItem(item);
			treasureStack.AddPieceToBottom(item);
			if (item is MRArmor)
			{
				// natives auto-repair armor
				((MRArmor)item).State = MRArmor.eState.Undamaged;
			}
			if (item is MRTreasure)
			{
				// some treasures have fame when sold
				MRTreasure treasure = (MRTreasure)item;
				if (treasure.SellFame > 0 && treasure.SellFameGroup == native.Group)
				{
					character.BaseFame += treasure.SellFame;
				}
			}
		}
		treasureStack.SortBySize();
	}

	#endregion

	#region Members

	private eState mState;
	private MRButton mAcceptButton;
	private MRButton mCancelButton;
	private bool mTradeDone;
	private MRControllable mSeller;
	private MRControllable mBuyer;
	private IList<MRItem> mItemsAvailable;
	private IList<MRItem> mItemsBought;
	private MRItem mItemBought;
	private int mPriceMod;
	private int mGold;
	private int mDrinkCost;
	private MRGame.eRelationship mTradeRelationship;
	MRDiePool mDiePool;


	#endregion
}

}