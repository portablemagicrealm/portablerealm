//
// MRCharacterItemsDisplay.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2015 Steve Jakab
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
using System;
using System.Collections;
using System.Collections.Generic;

public class MRCharacterItemsDisplay : MRTabItems, MRITouchable
{
	#region Properties

	public MRCharacterMat Parent
	{
		get{
			return mParent;
		}

		set{
			mParent = value;
		}
	}

	#endregion

	#region Methods

	// Called when the script instance is being loaded
	void Awake()
	{
		mActiveWeapon = MRGame.TheGame.NewGamePieceStack();
		mActiveArmor = MRGame.TheGame.NewGamePieceStack();
		mActiveBreastplate = MRGame.TheGame.NewGamePieceStack();
		mActiveHelmet = MRGame.TheGame.NewGamePieceStack();
		mActiveShield = MRGame.TheGame.NewGamePieceStack();
		mActiveHorse = MRGame.TheGame.NewGamePieceStack();
		mActiveBoots = MRGame.TheGame.NewGamePieceStack();
		mActiveGloves = MRGame.TheGame.NewGamePieceStack();
		mActiveItems = MRGame.TheGame.NewGamePieceStack();
		mInactiveWeapon = MRGame.TheGame.NewGamePieceStack();
		mInactiveArmor = MRGame.TheGame.NewGamePieceStack();
		mInactiveBreastplate = MRGame.TheGame.NewGamePieceStack();
		mInactiveHelmet = MRGame.TheGame.NewGamePieceStack();
		mInactiveShield = MRGame.TheGame.NewGamePieceStack();
		mInactiveHorse = MRGame.TheGame.NewGamePieceStack();
		mInactiveItems = MRGame.TheGame.NewGamePieceStack();
	}

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();

		// get the chit positions, stats, and curses
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform subTrans in transforms)
		{
			if (subTrans.gameObject.name == "ActiveChits")
				mActiveChitsArea = subTrans.gameObject;
			else if (subTrans.gameObject.name == "FatiguedChits")
				mFatiguedChitsArea = subTrans.gameObject;
			else if (subTrans.gameObject.name == "WoundedChits")
				mWoundedChitsArea = subTrans.gameObject;
			else if (subTrans.gameObject.name == "ActiveItems")
				mActiveItemsArea = subTrans.gameObject;
			else if (subTrans.gameObject.name == "InactiveItems")
				mInactiveItemsArea = subTrans.gameObject;
			else if (subTrans.gameObject.name == "Gold")
			{
				TextMesh[] texts = subTrans.gameObject.GetComponentsInChildren<TextMesh>();
				foreach (TextMesh text in texts)
				{
					if (text.gameObject.name == "Amount")
						mGold = text;
				}
			}
			else if (subTrans.gameObject.name == "Fame")
			{
				TextMesh[] texts = subTrans.gameObject.GetComponentsInChildren<TextMesh>();
				foreach (TextMesh text in texts)
				{
					if (text.gameObject.name == "Amount")
						mFame = text;
				}
			}
			else if (subTrans.gameObject.name == "Notoriety")
			{
				TextMesh[] texts = subTrans.gameObject.GetComponentsInChildren<TextMesh>();
				foreach (TextMesh text in texts)
				{
					if (text.gameObject.name == "Amount")
						mNotoriety = text;
				}
			}
			else if (subTrans.gameObject.name == "Weight")
			{
				TextMesh[] texts = subTrans.gameObject.GetComponentsInChildren<TextMesh>();
				foreach (TextMesh text in texts)
				{
					if (text.gameObject.name == "Value")
						mWeight = text;
				}
			}
			else if (subTrans.gameObject.name == "Vulnerability")
			{
				TextMesh[] texts = subTrans.gameObject.GetComponentsInChildren<TextMesh>();
				foreach (TextMesh text in texts)
				{
					if (text.gameObject.name == "Value")
						mVulnerability = text;
				}
			}
			else if (subTrans.gameObject.name == "Ashes")
			{
				mCurses.Add(MRGame.eCurses.Ashes, subTrans.gameObject);
			}
			else if (subTrans.gameObject.name == "Disgust")
			{
				mCurses.Add(MRGame.eCurses.Disgust, subTrans.gameObject);
			}
			else if (subTrans.gameObject.name == "Eyemist")
			{
				mCurses.Add(MRGame.eCurses.Eyemist, subTrans.gameObject);
			}
			else if (subTrans.gameObject.name == "Illhealth")
			{
				mCurses.Add(MRGame.eCurses.IllHealth, subTrans.gameObject);
			}
			else if (subTrans.gameObject.name == "Squeak")
			{
				mCurses.Add(MRGame.eCurses.Squeak, subTrans.gameObject);
			}
			else if (subTrans.gameObject.name == "Wither")
			{
				mCurses.Add(MRGame.eCurses.Wither, subTrans.gameObject);
			}
		}
		mActiveChitsBackground = null;
		transforms = mActiveChitsArea.GetComponentsInChildren<Transform>();
		foreach (Transform activeTrans in transforms)
		{
			if (activeTrans.gameObject.name.StartsWith("chit"))
			{
				mActiveChitsPositions[int.Parse(activeTrans.gameObject.name.Substring("chit".Length)) - 1] = activeTrans.gameObject;
			}
			else if (activeTrans.gameObject.name == "Background")
			{
				mActiveChitsBackground = activeTrans.gameObject;
			}
		}
		transforms = mFatiguedChitsArea.GetComponentsInChildren<Transform>();
		foreach (Transform fatigueTrans in transforms)
		{
			if (fatigueTrans.gameObject.name.StartsWith("chit"))
			{
				mFatiguedChitsPositions[int.Parse(fatigueTrans.gameObject.name.Substring("chit".Length)) - 1] = fatigueTrans.gameObject;
			}
		}
		GameObject woundedBackground = null;
		transforms = mWoundedChitsArea.GetComponentsInChildren<Transform>();
		foreach (Transform woundTrans in transforms)
		{
			if (woundTrans.gameObject.name.StartsWith("chit"))
			{
				mWoundedChitsPositions[int.Parse(woundTrans.gameObject.name.Substring("chit".Length)) - 1] = woundTrans.gameObject;
			}
			else if (woundTrans.gameObject.name == "Background")
			{
				woundedBackground = woundTrans.gameObject;
			}
		}
		
		// get the item positions
		transforms = mActiveItemsArea.GetComponentsInChildren<Transform>();
		foreach (Transform activeTrans in transforms)
		{
			if (activeTrans.gameObject.name == "weapon")
			{
				SetupTreasureStack(activeTrans.gameObject, mActiveWeapon);
			}
			else if (activeTrans.gameObject.name == "armor")
			{
				SetupTreasureStack(activeTrans.gameObject, mActiveArmor);
			}
			else if (activeTrans.gameObject.name == "shield")
			{
				SetupTreasureStack(activeTrans.gameObject, mActiveShield);
			}
			else if (activeTrans.gameObject.name == "helmet")
			{
				SetupTreasureStack(activeTrans.gameObject, mActiveHelmet);
			}
			else if (activeTrans.gameObject.name == "breastplate")
			{
				SetupTreasureStack(activeTrans.gameObject, mActiveBreastplate);
			}
			else if (activeTrans.gameObject.name == "horse")
			{
				SetupTreasureStack(activeTrans.gameObject, mActiveHorse);
			}
			else if (activeTrans.gameObject.name == "boots")
			{
				SetupTreasureStack(activeTrans.gameObject, mActiveBoots);
			}
			else if (activeTrans.gameObject.name == "gloves")
			{
				SetupTreasureStack(activeTrans.gameObject, mActiveGloves);
			}
			else if (activeTrans.gameObject.name == "treasures")
			{
				SetupTreasureStack(activeTrans.gameObject, mActiveItems);
			}
		}
		transforms = mInactiveItemsArea.GetComponentsInChildren<Transform>();
		foreach (Transform inactiveTrans in transforms)
		{
			if (inactiveTrans.gameObject.name == "weapon")
			{
				SetupTreasureStack(inactiveTrans.gameObject, mInactiveWeapon);
			}
			else if (inactiveTrans.gameObject.name == "armor")
			{
				SetupTreasureStack(inactiveTrans.gameObject, mInactiveArmor);
			}
			else if (inactiveTrans.gameObject.name == "shield")
			{
				SetupTreasureStack(inactiveTrans.gameObject, mInactiveShield);
			}
			else if (inactiveTrans.gameObject.name == "helmet")
			{
				SetupTreasureStack(inactiveTrans.gameObject, mInactiveHelmet);
			}
			else if (inactiveTrans.gameObject.name == "breastplate")
			{
				SetupTreasureStack(inactiveTrans.gameObject, mInactiveBreastplate);
			}
			else if (inactiveTrans.gameObject.name == "horse")
			{
				SetupTreasureStack(inactiveTrans.gameObject, mInactiveHorse);
			}
			else if (inactiveTrans.gameObject.name == "treasures")
			{
				SetupTreasureStack(inactiveTrans.gameObject, mInactiveItems);
			}
		}
		
		// adjust the active and wounded areas so they are on the far left and right of the view area
		Vector3 left = Parent.CharacterMatCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
		Vector3 activePos = mActiveChitsArea.transform.position;
		float activeLeft = mActiveChitsBackground.GetComponent<Renderer>().bounds.min.x;
		float delta = left.x - activeLeft;
		mActiveChitsArea.transform.position = new Vector3(activePos.x + delta, activePos.y, activePos.z);
		activePos = mActiveItemsArea.transform.position;
		mActiveItemsArea.transform.position = new Vector3(activePos.x + delta, activePos.y, activePos.z);
		Vector3 right = Parent.CharacterMatCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));
		Vector3 woundedPos = mWoundedChitsArea.transform.position;
		float woudnedRight = woundedBackground.GetComponent<Renderer>().bounds.max.x;
		delta = right.x - woudnedRight;
		mWoundedChitsArea.transform.position = new Vector3(woundedPos.x + delta, woundedPos.y, woundedPos.z);
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update();

		if (Parent.Visible && Parent.Controllable is MRCharacter)
		{
			MRCharacter character = (MRCharacter)Parent.Controllable;
			
			mFatiguedChitsArea.SetActive(true);
			mWoundedChitsArea.SetActive(true);
			
			if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectAttack ||
			    MRGame.TheGame.CurrentView == MRGame.eViews.SelectManeuver ||
			    MRGame.TheGame.CurrentView == MRGame.eViews.SelectChit)
			{
				// hide fatigued and wounded chits
				mFatiguedChitsArea.SetActive(false);
				mWoundedChitsArea.SetActive(false);
				Bounds b = mActiveChitsBackground.GetComponent<Renderer>().bounds;
				Vector3 bmax = mParent.CharacterMatCamera.WorldToViewportPoint(b.max);
				Vector3 bmin = mParent.CharacterMatCamera.WorldToViewportPoint(b.min);
				Vector3 activeUpRight = mParent.CharacterMatCamera.WorldToScreenPoint(mActiveChitsBackground.GetComponent<Renderer>().bounds.max);
				MRMainUI.TheUI.DisplayAttackManeuverDialog(activeUpRight.x / Screen.width, activeUpRight.y / (Screen.height * 2));
			}

			// display gold, fame, and notoriety
			mGold.text = character.EffectiveGold.ToString();
			mFame.text = ((int)(character.EffectiveFame)).ToString();
			mNotoriety.text = ((int)(character.EffectiveNotoriety)).ToString();

			// display weight and vulnerability
			mWeight.text = character.BaseWeight.ToChitString();
			mVulnerability.text = character.CurrentVulnerability.ToChitString();
			
			// display the character's chits in their proper locations
			IList<MRActionChit> chits = character.Chits;
			for (int i = 0; i < chits.Count; ++i)
			{
				MRActionChit chit = chits[i];
				if (chit != null)
				{
					if (chit.Stack != null)
						chit.Stack.RemovePiece(chit);
					if (character.CanSelectChit(chit))
						chit.FrontColor = MRGame.offWhite;
					else
						chit.FrontColor = MRGame.darkGrey;
					switch (chit.State)
					{
						case MRActionChit.eState.Active:
							chit.Parent = mActiveChitsPositions[i].transform;
							chit.Layer = mActiveChitsPositions[i].layer;
							chit.Position = mActiveChitsPositions[i].transform.position;
							chit.LocalScale = new Vector3(1.3f, 1.3f, 1f);
							break;
						case MRActionChit.eState.Fatigued:
							chit.Parent = mFatiguedChitsPositions[i].transform;
							chit.Layer = mFatiguedChitsPositions[i].layer;
							chit.Position = mFatiguedChitsPositions[i].transform.position;
							chit.LocalScale = new Vector3(1.3f, 1.3f, 1f);
							break;
						case MRActionChit.eState.Wounded:
							chit.Parent = mWoundedChitsPositions[i].transform;
							chit.Layer = mWoundedChitsPositions[i].layer;
							chit.Position = mWoundedChitsPositions[i].transform.position;
							chit.LocalScale = new Vector3(1.3f, 1.3f, 1f);
							break;
					}
				}
			}
			MRUtility.SetObjectVisibility(mActiveChitsArea, true);
			MRUtility.SetObjectVisibility(mFatiguedChitsArea, true);
			MRUtility.SetObjectVisibility(mWoundedChitsArea, true);
			
			// display active items in their proper locations
			mActiveWeapon.Clear();
			mActiveBreastplate.Clear();
			mActiveArmor.Clear();
			mActiveHelmet.Clear();
			mActiveShield.Clear();
			mActiveItems.Clear();
			mActiveHorse.Clear();
			IList<MRItem> activeItems = character.ActiveItems;
			foreach (MRItem item in activeItems)
			{
				if (item is MRWeapon)
				{
					mActiveWeapon.AddPieceToTop(item);
				}
				else if (item is MRArmor)
				{
					MRArmor armor = (MRArmor)item;
					switch (armor.Type)
					{
						case MRArmor.eType.Breastplate:
							mActiveBreastplate.AddPieceToTop(armor);
							break;
						case MRArmor.eType.Full:
							mActiveArmor.AddPieceToTop(armor);
							break;
						case MRArmor.eType.Helmet:
							mActiveHelmet.AddPieceToTop(armor);
							break;
						case MRArmor.eType.Shield:
							mActiveShield.AddPieceToTop(armor);
							break;
						default:
							break;
					}
				}
				else if (item is MRTreasure)
				{
					MRTreasure treasure = (MRTreasure)item;
					treasure.Hidden = false;
					mActiveItems.AddPieceToTop(treasure);
				}
				else if (item is MRHorse)
				{
					MRHorse horse = (MRHorse)item;
					mActiveHorse.AddPieceToTop(horse);
				}
			}
			
			// display inactive items in their proper locations
			mInactiveWeapon.Clear();
			mInactiveBreastplate.Clear();
			mInactiveArmor.Clear();
			mInactiveHelmet.Clear();
			mInactiveShield.Clear();
			mInactiveItems.Clear();
			mInactiveHorse.Clear();
			IList<MRItem> inactiveItems = character.InactiveItems;
			foreach (MRItem item in inactiveItems)
			{
				if (item is MRWeapon)
				{
					mInactiveWeapon.AddPieceToTop(item);
				}
				else if (item is MRArmor)
				{
					MRArmor armor = (MRArmor)item;
					switch (armor.Type)
					{
						case MRArmor.eType.Breastplate:
							mInactiveBreastplate.AddPieceToTop(armor);
							break;
						case MRArmor.eType.Full:
							mInactiveArmor.AddPieceToTop(armor);
							break;
						case MRArmor.eType.Helmet:
							mInactiveHelmet.AddPieceToTop(armor);
							break;
						case MRArmor.eType.Shield:
							mInactiveShield.AddPieceToTop(armor);
							break;
						default:
							break;
					}
				}
				else if (item is MRTreasure)
				{
					MRTreasure treasure = (MRTreasure)item;
					treasure.Hidden = false;
					mInactiveItems.AddPieceToTop(treasure);
				}
				else if (item is MRHorse)
				{
					MRHorse horse = (MRHorse)item;
					mInactiveHorse.AddPieceToTop(horse);
				}
			}
			
			// display curses
			foreach (MRGame.eCurses curse in Enum.GetValues(typeof(MRGame.eCurses)))
			{
				mCurses[curse].GetComponent<Renderer>().enabled = character.HasCurse(curse);
			}
			
			// display ui elements
			if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectAttack)
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Select Attack");
			}
			else if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectManeuver)
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Select Maneuver");
			}
			else if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectChit)
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Select Chit");
			}
		}
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
		if (Parent.Visible && Parent.Controllable is MRCharacter)
		{
			// see if an action chit has been selected
			MRActionChit selectedChit = touchedObject.GetComponentInChildren<MRActionChit>();
			if (selectedChit != null)
			{
				OnGamePieceSelected(selectedChit);
			}
		}
		return true;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	private void SetupTreasureStack(GameObject locationObj, MRGamePieceStack stack)
	{
		stack.Layer = LayerMask.NameToLayer("CharacterMat");
		MRTreasureChartLocation location = locationObj.GetComponentInChildren<MRTreasureChartLocation>();
		if (location != null)
			location.Treasures = stack;
	}

	/// <summary>
	/// Called when a game piece has been selected.
	/// </summary>
	/// <param name="piece">The game piece.</param>
	public void OnGamePieceSelected(MRIGamePiece piece)
	{
		MRCharacter character = Parent.Controllable as MRCharacter;
		MRActionChit chit = piece as MRActionChit;
		MRItem item = piece as MRItem;

		switch (MRGame.TheGame.CurrentView)
		{
			case MRGame.eViews.FatigueCharacter:
				if (chit != null)
				{
					// heal, wound, or fatigue the selected chit
					if (character.FatigueBalance != 0)
						character.FatigueChit(chit);
					else if (character.WoundBalance > 0)
						character.WoundChit(chit);
					else if (character.HealBalance > 0)
						character.HealChit(chit);
				}
				break;					
			case MRGame.eViews.SelectChit:
				if (chit != null)
				{
					if (character.SelectChitData != null)
					{
						character.SelectChitData.SelectedChit = chit;
					}
					else
					{
						MRMainUI.TheUI.HideAttackManeuverDialog();
						MRMainUI.TheUI.DisplayInstructionMessage(null);
						MRGame.TheGame.PopView();
					}
				}
				break;
			case MRGame.eViews.Alert:
				if (chit != null && character.SelectChitFilter != null && character.SelectChitFilter.IsValidSelectChit(chit))
				{
					chit.Alert(character.SelectChitFilter.Action);

					MRUpdateEvent evt = MRGame.TheGame.GetUpdateEvent(typeof(MRAlertEvent));
					if (evt != null)
					{
						evt.EndEvent();
					}
				}
				else if (item != null && item.Active && item is MRWeapon)
				{
					((MRWeapon)item).Alerted = !((MRWeapon)item).Alerted;
					MRUpdateEvent evt = MRGame.TheGame.GetUpdateEvent(typeof(MRAlertEvent));
					if (evt != null)
					{
						evt.EndEvent();
					}
				}
				break;
			case MRGame.eViews.SelectAttack:
				if (chit != null && character.SelectChitFilter != null && character.SelectChitFilter.IsValidSelectChit(chit))
				{
					if (MRGame.TheGame.CombatManager.SetAttack(character, (MRFightChit)chit, MRCombatManager.eAttackType.None))
					{
						MRMainUI.TheUI.HideAttackManeuverDialog();
						MRMainUI.TheUI.DisplayInstructionMessage(null);
						MRGame.TheGame.PopView();
					}
				}
				break;
			case MRGame.eViews.SelectManeuver:
				if (chit != null && character.SelectChitFilter != null && character.SelectChitFilter.IsValidSelectChit(chit))
				{
					if (MRGame.TheGame.CombatManager.SetManeuver(character, (MRMoveChit)chit, MRCombatManager.eDefenseType.None))
					{
						MRMainUI.TheUI.HideAttackManeuverDialog();
						MRMainUI.TheUI.DisplayInstructionMessage(null);
						MRGame.TheGame.PopView();
					}
				}
				break;
			case MRGame.eViews.Characters:
				if (item != null && character.CanRearrangeItems)
				{
					// by default, set the item active or inactive
					if (item.Active)
						character.DeactivateItem(item);
					else
						character.ActivateItem(item, true);
				}
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Called when the player selects "none" or "cancel" when selecting an attack or maneuver chit.
	/// </summary>
	/// <param name="option">The option selected.</param>
	public void OnAttackManeuverSelected(MRCombatManager.eAttackManeuverOption option)
	{
		if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectAttack ||
		    MRGame.TheGame.CurrentView == MRGame.eViews.SelectManeuver ||
		    MRGame.TheGame.CurrentView == MRGame.eViews.SelectChit)
		{
			if (option == MRCombatManager.eAttackManeuverOption.None)
			{
				if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectAttack)
					MRGame.TheGame.CombatManager.SetAttack((MRCharacter)Parent.Controllable, null, MRCombatManager.eAttackType.None);
				else if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectManeuver)
					MRGame.TheGame.CombatManager.SetManeuver((MRCharacter)Parent.Controllable, null, MRCombatManager.eDefenseType.None);
			}
			MRMainUI.TheUI.HideAttackManeuverDialog();
			MRMainUI.TheUI.DisplayInstructionMessage(null);
			MRGame.TheGame.PopView();
		}
		else if (MRGame.TheGame.CurrentView == MRGame.eViews.Alert)
		{

		}
	}

	#endregion

	#region Members

	private MRCharacterMat mParent;

	private GameObject mActiveChitsArea;
	private GameObject mFatiguedChitsArea;
	private GameObject mWoundedChitsArea;
	private GameObject mActiveChitsBackground;
	private GameObject[] mActiveChitsPositions = new GameObject[12];
	private GameObject[] mFatiguedChitsPositions = new GameObject[12];
	private GameObject[] mWoundedChitsPositions = new GameObject[12];
	
	private GameObject mActiveItemsArea;
	private GameObject mInactiveItemsArea;
	private MRGamePieceStack mActiveWeapon;
	private MRGamePieceStack mActiveArmor;
	private MRGamePieceStack mActiveBreastplate;
	private MRGamePieceStack mActiveHelmet;
	private MRGamePieceStack mActiveShield;
	private MRGamePieceStack mActiveHorse;
	private MRGamePieceStack mActiveBoots;
	private MRGamePieceStack mActiveGloves;
	private MRGamePieceStack mActiveItems;
	private MRGamePieceStack mInactiveWeapon;
	private MRGamePieceStack mInactiveArmor;
	private MRGamePieceStack mInactiveBreastplate;
	private MRGamePieceStack mInactiveHelmet;
	private MRGamePieceStack mInactiveShield;
	private MRGamePieceStack mInactiveHorse;
	private MRGamePieceStack mInactiveItems;

	private TextMesh mGold;
	private TextMesh mFame;
	private TextMesh mNotoriety;
	private TextMesh mWeight;
	private TextMesh mVulnerability;
	private IDictionary<MRGame.eCurses, GameObject> mCurses = new Dictionary<MRGame.eCurses, GameObject>();

	#endregion
}








