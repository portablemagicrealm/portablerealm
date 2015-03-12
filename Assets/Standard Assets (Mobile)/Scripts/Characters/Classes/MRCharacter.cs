//
// MRCharacter.cs
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
using System;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public abstract class MRCharacter : MRControllable, MRISerializable
{
	#region Properties

	public abstract MRGame.eCharacters Character { get; }
	public abstract string IconName { get; }

	public string[] StartingLocations
	{
		get{
			return mStartingLocations;
		}
	}

	public MRILocation StartingLocation
	{
		get{
			return mStartingLocation;
		}

		set{
			mStartingLocation = value;
		}
	}

	public override MRILocation Location
	{
		get{
			return base.Location;
		}

		set{
			base.Location = value;

			// if we are riding a horse in a cave, deactivate the horse
			if (mLocation != null && mLocation is MRClearing)
			{
				MRClearing clearing = (MRClearing)mLocation;
				if (clearing.type == MRClearing.eType.Cave)
				{
					MRHorse horse = HorseRidden();
					if (horse != null)
					{
						DeactivateItem(horse);
						MRGame.TheGame.ShowInformationDialog("Deactivated active horse.");
					}
				}
			}
		}
	}

	// how the controllable is moving
	public override MRGame.eMoveType MoveType 
	{
		get{
			return mMoveType;
		}

		set{
			mMoveType = value;
		}
	}

	/// <summary>
	/// Returns the current attack strength of the controllable.
	/// </summary>
	/// <value>The current strength.</value>
	public override MRGame.eStrength CurrentStrength 
	{ 
		get{
			MRGame.eStrength strength = MRGame.eStrength.Negligable;
			if (CombatSheet != null)
			{
				MRWeapon weapon = CombatSheet.CharacterData.weapon;
				if (weapon != null)
				{
					strength = weapon.CurrentStrength;
				}
				MRFightChit chit = CombatSheet.CharacterData.attackChit;
				if (chit != null && (weapon == null || !weapon.IsMissile))
				{
					// striking weapon, a chit strength > the weapon weight increases the strength
					MRGame.eStrength weight = MRGame.eStrength.Negligable;
					if (weapon != null)
						weight = weapon.BaseWeight;
					if (chit.CurrentStrength > weight)
						++strength;
				}
			}
			return strength;
		}
	}

	/// <summary>
	/// Returns the current sharpness of the controllable's weapon.
	/// </summary>
	/// <value>The current sharpness.</value>
	public override int CurrentSharpness 
	{ 
		get{
			int sharpness = 0;
			if (CombatSheet != null)
			{
				MRWeapon weapon = CombatSheet.CharacterData.weapon;
				if (weapon != null)
				{
					sharpness = weapon.CurrentSharpness;
				}
				else if (CombatSheet.CharacterData.attackChit != null)
				{
					// no weapon, assume a "dagger" with n strength and 1 sharpness
					sharpness = 1;
				}
			}
			return sharpness;
		}
	}

	/// <summary>
	/// Returns the current attack speed of the controllable.
	/// </summary>
	/// <value>The current attack speed.</value>
	public override int CurrentAttackSpeed 
	{ 
		get{
			int speed = int.MaxValue;
			if (CombatSheet != null)
			{
				MRFightChit chit = CombatSheet.CharacterData.attackChit;
				if (chit != null)
					speed = chit.CurrentTime;
			}
			return speed;
		}
	}
	
	/// <summary>
	/// Returns the current move speed of the controllable.
	/// </summary>
	/// <value>The current move speed.</value>
	public override int CurrentMoveSpeed 
	{ 
		get{
			int speed = int.MaxValue;
			if (CombatSheet != null)
			{
				MRMoveChit chit = CombatSheet.CharacterData.maneuverChit;
				if (chit != null)
					speed = chit.CurrentTime;
			}
			return speed;
		}
	}

	/// <summary>
	/// Returns the type of weapon used by the denizen.
	/// </summary>
	/// <value>The type of the weapon.</value>
	public override MRWeapon.eWeaponType WeaponType 
	{ 
		get{
			MRWeapon.eWeaponType weaponType = MRWeapon.eWeaponType.Striking;
			if (CombatSheet != null)
			{
				MRFightChit chit = CombatSheet.CharacterData.attackChit;
				if (chit != null)
				{
					MRWeapon weapon = CombatSheet.CharacterData.weapon;
					if (weapon != null)
						weaponType = weapon.IsMissile ? MRWeapon.eWeaponType.Missile : MRWeapon.eWeaponType.Striking;
				}
			}
			return weaponType;
		}
	}

	/// <summary>
	/// Gets the length of the controllable's active weapon.
	/// </summary>
	/// <value>The length of the weapon.</value>
	public override int WeaponLength
	{
		get{
			int length = int.MinValue;
			if (CombatSheet != null)
			{
				MRFightChit chit = CombatSheet.CharacterData.attackChit;
				if (chit != null)
				{
					MRWeapon weapon = CombatSheet.CharacterData.weapon;
					if (weapon != null)
						length = weapon.Length;
					else
					{
						// "dagger" with length 0
						length = 0;
					}
				}
			}
			return length;
		}
	}

	/// <summary>
	/// Returns the attack direction being used this round.
	/// </summary>
	/// <value>The attack type.</value>
	public override MRCombatManager.eAttackType AttackType 
	{ 
		get
		{
			MRCombatManager.eAttackType attack = MRCombatManager.eAttackType.None;
			if (CombatSheet != null && CombatTarget != null)
			{
				attack = CombatSheet.CharacterData.attackType;
			}
			return attack;
		}
	}
	
	/// <summary>
	/// Returns the maneuver direction being used this round.
	/// </summary>
	/// <value>The defense type.</value>
	public override MRCombatManager.eDefenseType DefenseType 
	{
		get
		{
			MRCombatManager.eDefenseType defense = MRCombatManager.eDefenseType.None;
			if (CombatSheet != null)
			{
				defense = CombatSheet.CharacterData.maneuverType;
			}
			return defense;
		}
	}

	/// <summary>
	/// Returns the number of asterisks healed during a rest.
	/// </summary>
	/// <value>The asterisk count.</value>
	public virtual int RestAsterisks
	{
		get{
			return 1;
		}
	}

	public virtual int CombatAsteriskLimit
	{
		get{
			return 2;
		}
	}

	public IList<MRActionChit> Chits
	{
		get{
			return mChits;
		}
	}

	public IList<MRActionChit> MoveChits
	{
		get{
			return mMoveChits;
		}
	}

	public IList<MRActionChit> FightChits
	{
		get{
			return mFightChits;
		}
	}

	public IList<MRActionChit> MagicChits
	{
		get{
			return mMagicChits;
		}
	}

	public IList<MRActionChit> ActiveChits
	{
		get{
			return mActiveChits;
		}
	}

	public IList<MRActionChit> FatiguedChits
	{
		get{
			return mFatiguedChits;
		}
	}

	public IList<MRActionChit> WoundedChits
	{
		get{
			return mWoundedChits;
		}
	}

	public int FatigueBalance
	{
		get{
			return mFatigueBalance;
		}
	}

	public int WoundBalance
	{
		get{
			return mWoundBalance;
		}
	}

	public int HealBalance
	{
		get{
			return mHealBalance;
		}
	}

	public IList<MRItem> ActiveItems
	{
		get{
			return mActiveItems;
		}
	}

	public IList<MRItem> InactiveItems
	{
		get{
			return mInactiveItems;
		}
	}

	public MRWeapon ActiveWeapon
	{
		get{
			foreach (MRItem piece in mActiveItems)
			{
				if (piece is MRWeapon)
					return (MRWeapon)piece;
			}
			return null;
		}
	}

	public virtual bool CanRearrangeItems
	{
		get{
			return MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight;
		}
	}

	public BitArray DiscoveredSites 
	{
		get{
			return mDiscoveredSites;
		}
	}

	public IList<uint> DiscoveredTreasures
	{
		get{
			return mDiscoveredTreasues;
		}
	}

	// Effective gold the controllable has
	public override int EffectiveGold 
	{ 
		get {
			if (HasCurse(MRGame.eCurses.Ashes))
				return -1;
			return base.EffectiveGold;
		}
	}

	public float BaseFame
	{
		get{
			return mFame;
		}

		set{
			mFame = value;
		}
	}

	public virtual float EffectiveFame
	{
		get{
			if (HasCurse(MRGame.eCurses.Disgust))
				return -1;
			return BaseFame;
		}
	}

	public float BaseNotoriety
	{
		get{
			return mNotoriety;
		}
		
		set{
			mNotoriety = value;
		}
	}

	public virtual float EffectiveNotoriety
	{
		get{
			return BaseNotoriety;
		}
	}

	public IList<MRDenizen> Hirelings
	{
		get {
			return mHirelings;
		}
	}

	/**********************/
	// MRIGamePiece properties

	//public override Vector3 OldScale 
	//{ 
	//	get	{
	//		return new Vector3(0.4f, 0.4f, 1.0f);
	//	}
	//	set {
	//	}
	//}

	#endregion

	#region Methods

	protected MRCharacter()
	{
	}
	
	protected MRCharacter(JSONObject jsonData, int index) :
		base(jsonData, index)
	{
		mMoveType = MRGame.eMoveType.Walk;
		mDiscoveredSites = new BitArray(Enum.GetValues(typeof(MRMapChit.eSiteChitType)).Length);
		// temp - discover all sites
		//foreach (MRMapChit.eSiteChitType site in Enum.GetValues(typeof(MRMapChit.eSiteChitType)))
		//{
		//	DiscoverSite(site);
		//}

		string weight = ((JSONString)jsonData["weight"]).Value;
		mWeight = weight.Strength();

		JSONArray startingLocations = (JSONArray)jsonData["start"];
		mStartingLocations = new string[startingLocations.Count];
		for (int i = 0; i < startingLocations.Count; ++i)
		{
			mStartingLocations[i] = ((JSONString)startingLocations[i]).Value;
		}

		// decode chits
		JSONArray chits = (JSONArray)jsonData["chits"];
		for (int i = 0; i < chits.Count; ++i)
		{
			JSONObject chitData = (JSONObject)chits[i];
			string action = ((JSONString)chitData["action"]).Value;
			MRActionChit chit = null;
			GameObject chitObject = new GameObject();
			//chitObject.transform.parent = gameObject.transform;
			switch (action)
			{
				case "mo":
				{
					chit = (MRActionChit)chitObject.AddComponent("MRMoveChit");
					string strength = ((JSONString)chitData["strength"]).Value;
					((MRMoveChit)chit).BaseStrength = strength.Strength();
					mMoveChits.Add(chit);
					break;
				}
				case "f":
				{
					chit = (MRActionChit)chitObject.AddComponent("MRFightChit");
					string strength = ((JSONString)chitData["strength"]).Value;
					((MRFightChit)chit).BaseStrength = strength.Strength();
					mFightChits.Add (chit);
					break;
				}
				case "ma":
				{
					chit = (MRActionChit)chitObject.AddComponent("MRMagicChit");
					string type = ((JSONString)chitData["strength"]).Value;
					((MRMagicChit)chit).BaseMagicType = Int32.Parse(type);
					mMagicChits.Add(chit);
					break;
				}
				case "b":
				{
					chit = (MRActionChit)chitObject.AddComponent("MRBerserkChit");
					string strength = ((JSONString)chitData["strength"]).Value;
					((MRBerserkChit)chit).BaseStrength = strength.Strength();
					mFightChits.Add (chit);
					break;
				}
				case "d":
				{
					chit = (MRActionChit)chitObject.AddComponent("MRDuckChit");
					string strength = ((JSONString)chitData["strength"]).Value;
					((MRDuckChit)chit).BaseStrength = strength.Strength();
					mMoveChits.Add(chit);
					break;
				}
				default:
					Debug.LogError("Invalid chit action " + action);
					GameObject.Destroy(chitObject);
					break;
			}
			if (chit != null)
			{
				int speed = ((JSONNumber)chitData["speed"]).IntValue;
				chit.BaseTime = speed;
				int effort = ((JSONNumber)chitData["effort"]).IntValue;
				chit.BaseAsterisks = effort;
				chit.State = MRActionChit.eState.active;
				
				mChits.Add(chit);
				mActiveChits.Add(chit);
			}
		}
		
		// decode equipment
		string weaponName = ((JSONString)jsonData["weapon"]).Value;
		bool hasShield = ((JSONBoolean)jsonData["shield"]).Value;
		bool hasHelmet = ((JSONBoolean)jsonData["helmet"]).Value;
		bool hasBreastplate = ((JSONBoolean)jsonData["breastplate"]).Value;
		bool hasArmor = ((JSONBoolean)jsonData["armor"]).Value;

		if (!String.IsNullOrEmpty(weaponName))
		{
			MRItem weapon = MRGame.TheGame.TreasureChart.GetWeaponFromNatives(weaponName);
			if (weapon != null)
			{
				AddInactiveItem(weapon);
				ActivateItem(weapon, true);
			}
			else
			{
				Debug.LogError("Unable to find weapon " + weaponName + " in treasures");
			}
		}		
		if (hasShield)
			InitArmor("shield");
		if (hasHelmet)
			InitArmor("helmet");
		if (hasBreastplate)
			InitArmor("breastplate");
		if (hasArmor)
			InitArmor("suit of armor");

		mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.characterCounterPrototype);
		mAttentionChit = (GameObject)MRGame.Instantiate(MRGame.TheGame.attentionChitPrototype);
		Sprite texture = (Sprite)Resources.Load(IconName, typeof(Sprite));
		SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "FrontSide")
			{
				sprite.color = MRGame.tan;
			}
			else if (sprite.gameObject.name == "BackSide")
			{
				sprite.color = MRGame.green;
			}
			else if (sprite.gameObject.name == "FrontSymbol")
			{
				sprite.sprite = texture;
			}
			else if (sprite.gameObject.name == "BackSymbol")
			{	
				sprite.sprite = texture;
			}
		}
		sprites = mAttentionChit.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "FrontSide")
			{
				sprite.color = MRGame.tan;
			}
			else if (sprite.gameObject.name == "FrontSymbol")
			{
				sprite.sprite = texture;
			}
		}
		PositionAttentionChit(null, Vector3.zero);

		// temp - add a treasure
		//MRTreasure treasure = MRItemManager.GetTreasure("golden icon");
		//if (treasure != null)
		//	AddInactiveItem(treasure);
	}

	/// <summary>
	/// Cleans up resources used by the instance prior to being removed.
	/// </summary>
	public override void Destroy()
	{
		base.Destroy();

		// remove the action chits
		foreach (MRActionChit chit in mChits)
		{
			if (chit.Stack != null)
				chit.Stack.RemovePiece(chit);
			GameObject.Destroy(chit.gameObject);
		}
		mChits.Clear();
		mActiveChits.Clear();
		mFatiguedChits.Clear();
		mWoundedChits.Clear();
		mMoveChits.Clear();
		mFightChits.Clear();
		mMagicChits.Clear();

		// remove the attention chit
		if (mAttentionChit != null)
		{
			GameObject.Destroy(mAttentionChit);
			mAttentionChit = null;
		}
	}

	// Does initialization associated with birdsong.
	public override void StartBirdsong()
	{
		mPonyMove = false;
		mKillCountForDay = 0;
		mNormalActivitiesAvailable = 2;
		mDaylightActivitesAvailable = 2;

		mExtraActivities.Clear();
		foreach (MRGame.eActivity activity in Enum.GetValues(typeof(MRGame.eActivity)))
		{
			mExtraActivities[activity] = 0;
		}

		mStartedDayRiding = false;
		MRHorse horse = HorseRidden();
		if (horse != null)
		{
			mStartedDayRiding = true;
			if (horse.Type == MRHorse.eType.Workhorse)
				mExtraActivities[MRGame.eActivity.Move] += 1;
		}
	}

	// Does initialization associated with sunrise.
	public override void StartSunrise()
	{
		// if we are in a cave or any move takes us to a cave, disable daylight activities
		if (Location is MRClearing && ((MRClearing)Location).type == MRClearing.eType.Cave)
			mDaylightActivitesAvailable = 0;
		else
		{
			IList<MRActivity> activities = ActivitiesForDay(MRGame.DayOfMonth).Activities;
			foreach (MRActivity activity in activities)
			{
				if (activity is MRMoveActivity && 
				    ((MRMoveActivity)activity).Clearing.type == MRClearing.eType.Cave)
				{
					mDaylightActivitesAvailable = 0;
					break;
				}
			}
		}
		CleanupActivities();
	}
	
	// Does initialization associated with daylight.
	public override void StartDaylight()
	{
		Hidden = false;
	}
	
	// Does initialization associated with sunset.
	public override void StartSunset()
	{
	}
	
	// Does initialization associated with evening.
	public override void StartEvening()
	{
	}
	
	// Does initialization associated with midnight.
	public override void StartMidnight()
	{
		base.StartMidnight();

		// if the character is in the clearing with the chapel, remove all curses
		foreach (MRIGamePiece piece in Location.Pieces.Pieces)
		{
			if (piece is MRDwelling && ((MRDwelling)piece).Type == MRDwelling.eDwelling.Chapel)
			{
				bool hadCurse = false;
				foreach (MRGame.eCurses curse in Enum.GetValues(typeof(MRGame.eCurses)))
				{
					if (HasCurse(curse))
					{
						hadCurse = true;
						RemoveCurse(curse);
					}
				}
				if (hadCurse)
					MRGame.TheGame.ShowInformationDialog("Curses removed");
				break;
			}
		}

		foreach (MRItem piece in mActiveItems)
		{
			if (piece is MRWeapon)
			{
				// weapon is unalerted
				((MRWeapon)piece).Alerted = false;
			}
			else if (piece is MRHorse)
			{
				// horse walks
				((MRHorse)piece).MoveType = MRHorse.eMoveType.Walk;
			}
		}
	}

	// Tests if the owner is allowed to do the activity.
	public override bool CanExecuteActivity(MRActivity activity)
	{
		// check for curses
		if (activity is MRSearchActivity && HasCurse(MRGame.eCurses.Eyemist))
			return false;
		if (activity is MRHideActivity && HasCurse(MRGame.eCurses.Squeak))
			return false;
		if (activity is MRRestActivity && HasCurse(MRGame.eCurses.IllHealth))
			return false;

		if (mNormalActivitiesAvailable + mDaylightActivitesAvailable > 0 ||
		    mExtraActivities[activity.Activity] > 0)
		{
			return true;
		}
		return false;
	}

	// Tells the controllable an activity was performed
	public override void ExecutedActivity(MRActivity activity)
	{
		if (mExtraActivities[activity.Activity] > 0)
			mExtraActivities[activity.Activity] = mExtraActivities[activity.Activity] - 1;
		else if (mNormalActivitiesAvailable > 0)
			--mNormalActivitiesAvailable;
		else if (mDaylightActivitesAvailable > 0)
			--mDaylightActivitesAvailable;

		// if we are riding a pony and moved, we get a free extra move
		if (activity is MRMoveActivity)
		{
			MRHorse horse = HorseRidden();
			if (mStartedDayRiding && !mPonyMove && horse != null && horse.Type == MRHorse.eType.Pony)
			{
				mExtraActivities[MRGame.eActivity.Move] += 1;
				mPonyMove = true;
			}
			else
				mPonyMove = false;
		}
	}

	public void OnClearingSelected(MRClearing clearing)
	{
		if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight)
		{
			// tell the current activity a clearing was selected
			MRActivityList activities = ActivitiesForDay(MRGame.DayOfMonth);
			foreach (MRActivity activity in activities.Activities)
			{
				if (activity.Active && !activity.Executed)
				{
					activity.OnClearingSelected(clearing);
					break;
				}
			}
		}
	}

	/// <summary>
	/// Returns if the character can fatigue a given number of asterisks of any type of chit.
	/// </summary>
	/// <returns><c>true</c> if this instance can fatigue chit the specified count; otherwise, <c>false</c>.</returns>
	/// <param name="count">number of asterisks that must be fatigued</param>
	public bool CanFatigueChit(int count)
	{
		return CanFatigueChit(count, MRActionChit.eType.any, MRGame.eStrength.Any);
	}

	/// <summary>
	/// Returns if the character can fatigue a given number of asterisks of a specific kind of chit.
	/// </summary>
	/// <returns><c>true</c> if this instance can fatigue chit the specified count type strength; otherwise, <c>false</c>.</returns>
	/// <param name="count">number of asterisks that must be fatigued</param>
	/// <param name="type">type of chit that needs to be fatigued</param>
	/// <param name="strength">strength of the chit that needs to be fatigued</param>
	public bool CanFatigueChit(int count, MRActionChit.eType type, MRGame.eStrength strength)
	{
		foreach (MRActionChit chit in mActiveChits)
		{
			if (chit.BaseAsterisks <= 0)
				continue;
			if (type != MRActionChit.eType.any && type != chit.Type)
				continue;
			if (strength != MRGame.eStrength.Any)
			{
				if (chit is MRFightChit && ((MRFightChit)chit).BaseStrength != strength)
					continue;
				if (chit is MRMoveChit && ((MRMoveChit)chit).BaseStrength != strength)
					continue;
			}
			count -= chit.BaseAsterisks;
		}	

		return count <= 0;
	}

	/// <summary>
	/// Sets the number of asterisks the character must fatigue. They can fatigue any type of chit.
	/// </summary>
	/// <param name="count">the asterisk count</param>
	public void SetFatigueBalance(int count)
	{
		SetFatigueBalance(count, MRActionChit.eType.any, MRGame.eStrength.Any);
	}

	/// <summary>
	/// Sets the number of asterisks the character must fatigue. They must fatigue the given type of chit.
	/// </summary>
	/// <param name="count">the asterisk count</param>
	/// <param name="type">type of chit to be fatigued</param>
	/// <param name="strength">strength of chit to be fatigued</param>
	public void SetFatigueBalance(int count, MRActionChit.eType type, MRGame.eStrength strength)
	{
		mFatigueBalance = count;
		mFatigueBalanceType = type;
		mFatigueBalanceStrength = strength;
	}

	/// <summary>
	/// Sets the number of chits the character must wound.
	/// </summary>
	/// <param name="count">Count.</param>
	public void SetWoundBalance(int count)
	{
		mWoundBalance = count;
	}

	/// <summary>
	/// Sets the number of chits the character can heal.
	/// </summary>
	/// <param name="count">Count.</param>
	public void SetHealBalance(int count)
	{
		mHealBalance = count;
	}

	/// <summary>
	/// Fatigues a chit and adjusts the fatigue balance. This function is also used to unfatigue a chit in order to "make change".
	/// </summary>
	/// <param name="chit">chit to fatigue</param>
	public void FatigueChit(MRActionChit chit)
	{
		// can't fatigue chits with no asterisks
		if (chit.BaseAsterisks == 0)
			return;

		// test for valid fatigue
		if (mFatigueBalance > 0 && chit.State == MRActionChit.eState.active &&
		    (mFatigueBalanceType == MRActionChit.eType.any || mFatigueBalanceType == chit.Type))
		{
			mActiveChits.Remove(chit);
			mFatiguedChits.Add(chit);
			chit.State = MRActionChit.eState.fatigued;
			mFatigueBalance -= chit.BaseAsterisks;
			if (mFatigueBalance < 0)
			{
				// if the character will need to "make change", the chit needs to be the same type
				mFatigueBalanceType = chit.Type;
			}
			return;
		}
		// test for "making change"
		if (mFatigueBalance < 0 && chit.State == MRActionChit.eState.fatigued &&
		    (mFatigueBalanceType == MRActionChit.eType.any || mFatigueBalanceType == chit.Type))
		{
			mFatiguedChits.Remove(chit);
			mActiveChits.Add(chit);
			chit.State = MRActionChit.eState.active;
			mFatigueBalance += chit.BaseAsterisks;
		}
	}

	/// <summary>
	/// Wounds a chit from combat or other effect.
	/// </summary>
	/// <param name="chit">the chit to wound</param>
	public void WoundChit(MRActionChit chit)
	{
		if (mWoundBalance <= 0)
			return;

		// active chits must be wounded first
		if (mActiveChits.Count > 0)
		{
			if (mActiveChits.Contains(chit))
			{
				if (chit.State == MRActionChit.eState.enchanted)
				{
					// enchanted chits can only be wounded if there are no non-enchanted active chits left
					foreach (MRActionChit testChit in mActiveChits)
					{
						if (testChit.State == MRActionChit.eState.active)
							return;
					}
				}
				mActiveChits.Remove(chit);
				mWoundedChits.Add(chit);
				chit.State = MRActionChit.eState.wounded;
				--mWoundBalance;
			}
			return;
		}

		// fatigued chits can be wounded if no active chits are available
		if (mFatiguedChits.Count > 0)
		{
			if (mFatiguedChits.Contains(chit))
			{
				mFatiguedChits.Remove(chit);
				mWoundedChits.Add(chit);
				chit.State = MRActionChit.eState.wounded;
				--mWoundBalance;
			}
			return;
		}
	}

	/// <summary>
	/// Heals a chit, from a Rest activity or other effect.
	/// </summary>
	/// <param name="chit">the chit to heal</param>
	public void HealChit(MRActionChit chit)
	{
		if (mHealBalance <= 0 || mActiveChits.Contains(chit))
			return;

		if (chit.State == MRActionChit.eState.fatigued && HasCurse(MRGame.eCurses.Wither))
			return;

		// 1-asterisk fatigued and no-asterisk wounded chits are made active
		if (chit.State == MRActionChit.eState.fatigued && chit.BaseAsterisks == 1)
		{
			mFatiguedChits.Remove(chit);
			mActiveChits.Add(chit);
			chit.State = MRActionChit.eState.active;
			--mHealBalance;
			return;
		}
		if (chit.State == MRActionChit.eState.wounded && chit.BaseAsterisks == 0)
		{
			mWoundedChits.Remove(chit);
			mActiveChits.Add(chit);
			chit.State = MRActionChit.eState.active;
			--mHealBalance;
			return;
		}
		// 1-asterisk wounded chits are fatigued
		if (chit.State == MRActionChit.eState.wounded && chit.BaseAsterisks == 1)
		{
			mWoundedChits.Remove(chit);
			mFatiguedChits.Add(chit);
			chit.State = MRActionChit.eState.fatigued;
			--mHealBalance;
			return;
		}
		// 2-asterisk fatigued chits can be made active if the character can fatigue a 1-asterisk chit
		if (chit.State == MRActionChit.eState.fatigued && chit.BaseAsterisks == 2 && CanFatigueChit(1))
		{
			mFatiguedChits.Remove(chit);
			mActiveChits.Add(chit);
			chit.State = MRActionChit.eState.active;
			--mHealBalance;
			mFatigueBalance = 1;
			mFatigueBalanceType = chit.Type;
			mFatigueBalanceStrength = MRGame.eStrength.Any;
			return;
		}
		// 2-asterisk wounded chits can be fatigued if the character can fatigue a 1-asterisk chit
		if (chit.State == MRActionChit.eState.wounded && chit.BaseAsterisks == 2 && CanFatigueChit(1))
		{
			mWoundedChits.Remove(chit);
			mFatiguedChits.Add(chit);
			chit.State = MRActionChit.eState.fatigued;
			--mHealBalance;
			mFatigueBalance = 1;
			mFatigueBalanceType = chit.Type;
			mFatigueBalanceStrength = MRGame.eStrength.Any;
			return;
		}
	}

	/// <summary>
	/// Adds an item to the character's inactive items. NOTE: to add an item to the active items, add it to the inactive items and then activate it.
	/// </summary>
	/// <param name="item">The item.</param>
	public void AddInactiveItem(MRItem item)
	{
		if (item.Stack != null)
			item.Stack.RemovePiece(item);
		if (item.Owner != null)
			item.Owner.RemoveItem(item);
		mInactiveItems.Add(item);
		item.Owner = this;
		item.Active = false;
	}

	/// <summary>
	/// Adds an item to the character's inactive items.
	/// </summary>
	/// <param name="item">The item.</param>
	public override void AddItem(MRItem item)
	{
		AddInactiveItem(item);
	}

	/// <summary>
	/// Removes an item from the character. The item can be active or inactive.
	/// </summary>
	/// <param name="item">Item to remove.</param>
	public void RemoveItem(MRItem item)
	{
		mActiveItems.Remove(item);
		mInactiveItems.Remove(item);
		item.Owner = null;
		item.Active = false;
	}

	/// <summary>
	/// Removes all the items from this character and puts them back in their starting locations.
	/// </summary>
	public void RemoveAllItems()
	{
		foreach (MRItem item in mActiveItems)
		{
			item.Owner = null;
			item.Active = false;
			if (item.StartStack != null)
				item.StartStack.AddPieceToBottom(item);
			else
			{
				Debug.LogError("Item " + item.Name + " has no start stack");
			}
		}
		foreach (MRItem item in mInactiveItems)
		{
			item.Owner = null;
			item.Active = false;
			if (item.StartStack != null)
				item.StartStack.AddPieceToBottom(item);
			else
			{
				Debug.LogError("Item " + item.Name + " has no start stack");
			}
		}
		mActiveItems.Clear();
		mInactiveItems.Clear();
	}

	/// <summary>
	/// Determines whether the character can activate the specified item.
	/// </summary>
	/// <returns><c>true</c> if the item can be activated, <c>false</c> if not</returns>
	/// <param name="item">The item.</param>
	/// <param name="replaceActiveItem">Flag that we want to replace a current active item</param>
	public virtual bool CanActivateItem(MRItem item, bool replaceActiveItem)
	{
		if (!replaceActiveItem)
		{
			// make sure there isn't an active item of the same type
			MRItem currentActiveItem = GetActiveItemOfSameType(item);
			if (currentActiveItem != null)
				return false;
		}

		if (item is MRHorse)
		{
			// a horse can only be active (ridden) if its strength is >= the weight if the character and their items
			if (((MRHorse)item).CurrentStrength < GetHeaviestWeight(false, true))
				return false;
			// a horse cannot be active in a cave
			if (Location != null && Location is MRClearing && ((MRClearing)Location).type == MRClearing.eType.Cave)
				return false;
		}

		return true;
	}

	/// <summary>
	/// Determines whether the character can deactivate the specified item.
	/// </summary>
	/// <returns><c>true</c> if the item can be deactivated, <c>false</c> if not</returns>
	/// <param name="item">Item.</param>
	public virtual bool CanDeactivateItem(MRItem item)
	{
		if (item is MRTreasure)
		{
			// an enchanted treasure cannot be deactivated
		}
		return true;
	}

	/// <summary>
	/// Activates the item in the character's inactive item list.
	/// </summary>
	/// <returns><c>true</c> if the item was activated, <c>false</c> if not</returns>
	/// <param name="item">The item. It must be in the inactive item list.</param>
	/// <param name="replaceActiveItem">Flag that we want to replace a current active item</param>
	public bool ActivateItem(MRItem item, bool replaceActiveItem)
	{
		if (mInactiveItems.Contains(item))
		{
			if (CanActivateItem(item, replaceActiveItem))
			{
				if (replaceActiveItem)
				{
					MRItem oldItem = GetActiveItemOfSameType(item);
					if (oldItem != null)
					{
						if (!DeactivateItem(oldItem))
							return false;
					}
				}
				mInactiveItems.Remove(item);
				mActiveItems.Add(item);
				item.Active = true;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Deactivates the item in the character's active item list.
	/// </summary>
	/// <returns><c>true</c> if the item was deactivated, <c>false</c> if not</returns>
	/// <param name="item">The item. It must be in the active item list.</param>
	public bool DeactivateItem(MRItem item)
	{
		if (mActiveItems.Contains(item))
		{
			if (CanDeactivateItem(item))
			{
				if (item is MRHorse)
					mStartedDayRiding = false;
				mActiveItems.Remove(item);
				mInactiveItems.Add(item);
				item.Active = false;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Returns if a given item is in the character's active list.
	/// </summary>
	/// <returns><c>true</c> if the item is active; otherwise, <c>false</c>.</returns>
	/// <param name="item">The item to test</param>
	public bool HasActiveItem(MRItem item)
	{
		if (item != null)
		{
			return mActiveItems.Contains(item);
		}
		return false;
	}

	/// <summary>
	/// Returns if a given item is in the character's inactive list.
	/// </summary>
	/// <returns><c>true</c> if the item is inactive; otherwise, <c>false</c>.</returns>
	/// <param name="item">The item to test</param>
	public bool HasInactiveItem(MRItem item)
	{
		if (item != null)
		{
			return mInactiveItems.Contains(item);
		}
		return false;
	}

	/// <summary>
	/// Returns if a given item is in the character's items.
	/// </summary>
	/// <returns><c>true</c> if the character owns the item; otherwise, <c>false</c>.</returns>
	/// <param name="item">The item to test</param>
	public bool HasItem(MRItem item)
	{
		return HasActiveItem(item) || HasInactiveItem(item);
	}

	/// <summary>
	/// Alerts the character's active weapon.
	/// </summary>
	public void AlertActiveWeapon()
	{
		foreach (MRIGamePiece piece in mActiveItems)
		{
			if (piece is MRWeapon)
			{
				((MRWeapon)piece).Alerted = true;
				break;
			}
		}
	}

	/// <summary>
	/// Returns the weight of the heaviest item owned by the character.
	/// </summary>
	/// <returns>The heaviest weight.</returns>
	/// <param name="includeHorse">Flag that horses should be included in the weight.</param>
	/// <param name="includeSelf">Flag that the character's own weight should be included in the weight.</param>
	public override MRGame.eStrength GetHeaviestWeight(bool includeHorse, bool includeSelf)
	{
		MRGame.eStrength result = GetHeaviestActiveWeight(includeHorse, includeSelf);

		foreach (MRIGamePiece item in mInactiveItems)
		{
			if (item is MRItem && (!(item is MRHorse) || includeHorse))
			{
				MRGame.eStrength itemWeight = ((MRItem)item).BaseWeight;
				if (itemWeight > result)
					result = itemWeight;
			}
		}

		return result;
	}

	/// <summary>
	/// Returns the weight of the heaviest active item owned by the character.
	/// </summary>
	/// <returns>The heaviest weight.</returns>
	/// <param name="includeHorse">Flag that horses should be included in the weight.</param>
	/// <param name="includeSelf">Flag that the character's own weight should be included in the weight.</param>
	public MRGame.eStrength GetHeaviestActiveWeight(bool includeHorse, bool includeSelf)
	{
		MRGame.eStrength result = MRGame.eStrength.Negligable;
		
		if (includeSelf)
			result = mWeight;
		
		foreach (MRItem item in mActiveItems)
		{
			if (!(item is MRHorse) || includeHorse)
			{
				MRGame.eStrength itemWeight = item.BaseWeight;
				if (itemWeight > result)
					result = itemWeight;
			}
		}
		
		return result;
	}

	/// <summary>
	/// Returns if the character can move with a given item.
	/// </summary>
	/// <returns><c>true</c> if the character can move with item the specified item; otherwise, <c>false</c>.</returns>
	/// <param name="item">Item.</param>
	public bool CanMoveWithItem(MRItem item)
	{
		if (item.BaseWeight == MRGame.eStrength.Negligable || item is MRHorse)
			return true;
		if (item.BaseWeight >= MRGame.eStrength.Immobile)
			return false;

		// test if we have a move chit of a strength >= the item's weight
		foreach (MRActionChit chit in mMoveChits)
		{
			if ((chit is MRMoveChit) && !(chit is MRDuckChit) && mActiveChits.Contains(chit))
			{
				if (((MRMoveChit)chit).BaseStrength >= item.BaseWeight)
					return true;
			}
		}

		// test if we have a horse that can carry the item; the active horse can carry active or inactive items,
		// inactive horses can only carry inactive items
		MRHorse horse = HorseRidden();
		if (horse != null && horse.CurrentStrength >= item.BaseWeight)
			return true;
		if (!item.Active)
		{
			foreach (MRIGamePiece piece in mInactiveItems)
			{
				if (piece is MRHorse && ((MRHorse)piece).CurrentStrength >= item.BaseWeight)
					return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Returns an active item of the same type as a given item for items where only one type can be active
	/// - weapon for weapon, shield for shield, etc. If more than one of the item type can be active, returns null.
	/// </summary>
	/// <returns>The active item of same type.</returns>
	/// <param name="testItem">Test item.</param>
	private MRItem GetActiveItemOfSameType(MRItem testItem)
	{
		if (testItem is MRWeapon)
		{
			foreach (MRItem piece in mActiveItems)
			{
				if (piece is MRWeapon)
					return (MRWeapon)piece;
			}
		}
		else if (testItem is MRArmor)
		{
			foreach (MRItem piece in mActiveItems)
			{
				if (piece is MRArmor && ((MRArmor)piece).Type == ((MRArmor)testItem).Type)
					return (MRArmor)piece;
			}
		}
		else if (testItem is MRHorse)
		{
			return HorseRidden();
		}

		return null;
	}

	/// <summary>
	/// Returns the horse being ridden by the character.
	/// </summary>
	/// <returns>The horse.</returns>
	public MRHorse HorseRidden()
	{
		foreach (MRItem piece in mActiveItems)
		{
			if (piece is MRHorse)
				return (MRHorse)piece;
		}
		return null;
	}

	// Returns the die pool for a given roll type
	public override MRDiePool DiePool(MRGame.eRollTypes roll)
	{
		MRDiePool pool = null;
		switch (roll)
		{
			case MRGame.eRollTypes.CombatRandomAssignment:
				pool = MRDiePool.DefaultPool;
				pool.ClampHigh = false;
				break;
			default:
				pool = base.DiePool(roll);
				break;
		}
		return pool;
	}

	// Returns if a site can be looted by the character
	public override bool CanLootSite(MRSiteChit site)
	{
		if (!base.CanLootSite(site))
			return false;

		if (site.SiteType == MRMapChit.eSiteChitType.Cairns || site.SiteType == MRMapChit.eSiteChitType.Pool)
			return CanFatigueChit(1);
		else if (site.SiteType == MRMapChit.eSiteChitType.Vault)
		{
			if (MRSiteChit.VaultOpened)
				return true;
			else if (HasLostKeysOrTStrengthTreasure())
			{
				return true;
			}
			return CanFatigueChit(1, MRActionChit.eType.any, MRGame.eStrength.Tremendous);
		}
		return true;
	}

	// Returns if a twit site can be looted by the character
	public override bool CanLootTwit(MRTreasure twit)
	{
		if (!base.CanLootTwit(twit))
			return false;

		if (twit.Id == MRUtility.IdForName("crypt of the knight"))
		{
			if (HasLostKeysOrTStrengthTreasure())
			{
				return true;
			}
			// temp - allow looting crypt
			//return CanFatigueChit(1, MRActionChit.eType.any, MRGame.eStrength.Tremendous);
		}

		return true;
	}

	/// <summary>
	/// Called when a site is looted (or attempted to be looted) by the controllable
	/// </summary>
	/// <param name="site">The site.</param>
	/// <param name="success">Flag that the site was looted successfully</param>
	public override void OnSiteLooted(MRSiteChit site, bool success)
	{
		// cache the vault state, since it may be changed by the base class
		bool vaultOpened = MRSiteChit.VaultOpened;

		base.OnSiteLooted(site, success);

		if (site.SiteType == MRMapChit.eSiteChitType.Cairns)
			SetFatigueBalance(1);
		if (site.SiteType == MRMapChit.eSiteChitType.Pool && success)
			SetFatigueBalance(1);
		else if (site.SiteType == MRMapChit.eSiteChitType.Vault && !vaultOpened)
		{
			if (!HasLostKeysOrTStrengthTreasure())
			{
				SetFatigueBalance(1, MRActionChit.eType.any, MRGame.eStrength.Tremendous);
			}
		}
	}
	
	/// <summary>
	/// Called when a twit site is looted (or attempted to be looted) by the controllable 
	/// </summary>
	/// <param name="twit">The twit site</param>
	/// <param name="success">Flag that the site was looted successfully</param>
	public override void OnTwitLooted(MRTreasure twit, bool success)
	{
		base.OnTwitLooted(twit, success);

		if (twit.Id == MRUtility.IdForName("crypt of the knight"))
		{
			if (!HasLostKeysOrTStrengthTreasure())
			{
				// temp - allow looting crypt
				//SetFatigueBalance(1, MRActionChit.eType.any, MRGame.eStrength.Tremendous);
			}
		}
	}

	/// <summary>
	/// Returns if the character has a curse.
	/// </summary>
	/// <returns><c>true</c>if the character has the specified curse; otherwise, <c>false</c>.</returns>
	/// <param name="curse">The curse.</param>
	public bool HasCurse(MRGame.eCurses curse)
	{
		return mCurses.Get((int)curse);
	}

	/// <summary>
	/// Applies a curse to the character.
	/// </summary>
	/// <param name="curse">The curse.</param>
	public void AddCurse(MRGame.eCurses curse)
	{
		mCurses.Set((int)curse, true);

		if (curse == MRGame.eCurses.Wither)
		{
			// fatigue all asterisks
			List<MRActionChit> toFatigue = new List<MRActionChit>();
			foreach (MRActionChit chit in mActiveChits)
			{
				if (chit.BaseAsterisks > 0)
					toFatigue.Add(chit);
			}
			foreach (MRActionChit chit in toFatigue)
			{
				chit.State = MRActionChit.eState.fatigued;
				mFatiguedChits.Add(chit);
				mActiveChits.Remove(chit);
			}
		}
	}

	/// <summary>
	/// Removes a curse from the character.
	/// </summary>
	/// <param name="curse">The curse.</param>
	public void RemoveCurse(MRGame.eCurses curse)
	{
		mCurses.Set((int)curse, false);
	}

	/// <summary>
	/// Returns if the characer has the lost keys or an active treasure that gives them T strength. For looting.
	/// </summary>
	/// <returns><c>true</c> if this instance has lost keys or T strength item; otherwise, <c>false</c>.</returns>
	private bool HasLostKeysOrTStrengthTreasure()
	{
		// check items
		if (HasActiveItem(MRItem.GetItem(MRUtility.IdForName("lost keys"))) ||
		    HasItem(MRItem.GetItem(MRUtility.IdForName("7-league boots"))) ||
		    HasItem(MRItem.GetItem(MRUtility.IdForName("gloves of strength"))))
		{
			return true;
		}

		// check horses
		foreach (MRItem piece in mActiveItems)
		{
			if (piece is MRHorse && ((MRHorse)piece).CurrentStrength == MRGame.eStrength.Tremendous)
				return true;
		}
		foreach (MRIGamePiece piece in mInactiveItems)
		{
			if (piece is MRHorse && ((MRHorse)piece).CurrentStrength == MRGame.eStrength.Tremendous)
				return true;
		}

		return false;
	}

	// Called when the controllable hits its target
	public override void HitTarget(MRIControllable attacker, bool targetDead)
	{
		// character's weapon becomes unalerted
		if (CombatSheet != null && CombatSheet.CharacterData.weapon != null)
			CombatSheet.CharacterData.weapon.Alerted = false;
	}
	
	// Called when the controllable misses its target
	public override void MissTarget(MRIControllable attacker)
	{
		// character's weapon becomes alerted
		if (CombatSheet != null && CombatSheet.CharacterData.weapon != null)
			CombatSheet.CharacterData.weapon.Alerted = true;
	}

	/// <summary>
	/// Awards the spoils of combat for killing a combatant.
	/// </summary>
	/// <param name="killed">Combatant that was killed.</param>
	/// <param name="awardFraction">Fraction of the spoils to award (due to shared kills).</param>
	public override void AwardSpoils(MRIControllable killed, float awardFraction)
	{
		++mKillCountForDay;
		if (killed is MRDenizen)
		{
			MRDenizen denizen = (MRDenizen)killed;
			BaseFame += (denizen.BountyFame * awardFraction) * mKillCountForDay;
			BaseNotoriety += (denizen.BountyNotoriety * awardFraction) * mKillCountForDay;
			BaseGold += (int)Math.Floor(denizen.BountyGold * awardFraction);
		}
		else
		{
			// todo: award spoils for killing other characters
		}
	}

	void OnGUI()
	{
		GUI.skin = MRGame.TheGame.skin;
		if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight)
		{
			// update the day's activities
			MRActivityList activities = ActivitiesForDay(MRGame.DayOfMonth);
			foreach (MRActivity activity in activities.Activities)
			{
				activity.OnGUI();
			}
		}
	}

	// Update is called once per frame
	public override void Update ()
	{
		if (mLocation == null)
		{
			// note; we need to re-add the character to their starting clearing to fix up a scale issue
			//Clearing = MRGame.TheGame.GetClearing("B1");
			//Clearing = MRGame.TheGame.GetClearing("B1");
		}

		Vector3 orientation = mCounter.transform.localEulerAngles;
		if (mHidden)
			orientation.y = 180f;
		else
			orientation.y = 0;
		mCounter.transform.localEulerAngles = orientation;

		if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight)
		{
			// update the day's activities
			MRActivityList activities = ActivitiesForDay(MRGame.DayOfMonth);
			foreach (MRActivity activity in activities.Activities)
			{
				activity.Update();
			}
		}
	}

	/// <summary>
	/// Sets the position of the character's attention chit on its comabt target.
	/// </summary>
	/// <param name="target">The controllable being targeted</param>
	/// <param name="position">The position relative to the target to place the chit.</param>
	public void PositionAttentionChit(GameObject target, Vector3 position)
	{
		if (mAttentionChit != null)
		{
			if (target != null)
			{
				mAttentionChit.transform.parent = target.transform;
				mAttentionChit.transform.localPosition = position;
				mAttentionChit.layer = target.layer;
			}
			else
			{
				mAttentionChit.transform.parent = null;
				mAttentionChit.layer = LayerMask.NameToLayer("Dummy");
			}
			foreach (SpriteRenderer sprite in mAttentionChit.GetComponentsInChildren<SpriteRenderer>())
			{
				sprite.gameObject.layer = mAttentionChit.layer;
			}
		}
	}

	private void InitArmor(string armorName)
	{
		MRItem armor = MRGame.TheGame.TreasureChart.GetArmorFromNatives(armorName);
		if (armor != null)
		{
			AddInactiveItem(armor);
			ActivateItem(armor, true);
		}
		else
		{
			Debug.LogError("Unable to find armor " + armorName + " in treasures");
		}
	}

	public override bool Load(JSONObject root)
	{
		if (!base.Load(root))
			return false;

		mFame = ((JSONNumber)root["fame"]).FloatValue;
		mNotoriety = ((JSONNumber)root["notoriety"]).FloatValue;
		mKillCountForDay = ((JSONNumber)root["kills"]).IntValue;
		mPonyMove = ((JSONBoolean)root["pony"]).Value;
		mStartedDayRiding = ((JSONBoolean)root["riding"]).Value;
		mNormalActivitiesAvailable = ((JSONNumber)root["normalactivities"]).IntValue;
		mDaylightActivitesAvailable = ((JSONNumber)root["dayactivities"]).IntValue;

		mActiveChits.Clear();
		mFatiguedChits.Clear();
		mWoundedChits.Clear();
		JSONArray chits = (JSONArray)root["activechits"];
		for (int i = 0; i < chits.Count; ++i)
		{
			mActiveChits.Add(mChits[((JSONNumber)chits[i]).IntValue]);
		}
		chits = (JSONArray)root["fatiguedchits"];
		for (int i = 0; i < chits.Count; ++i)
		{
			mFatiguedChits.Add(mChits[((JSONNumber)chits[i]).IntValue]);
		}
		chits = (JSONArray)root["woundedchits"];
		for (int i = 0; i < chits.Count; ++i)
		{
			mWoundedChits.Add(mChits[((JSONNumber)chits[i]).IntValue]);
		}
		chits = null;
		// todo: remove current items
		RemoveAllItems();
		JSONArray items = (JSONArray)root["activeitems"];
		for (int i = 0; i < items.Count; ++i)
		{
			JSONObject itemData = (JSONObject)items[i];
			uint itemId = ((JSONNumber)itemData["id"]).UintValue;
			MRItem item = MRItemManager.GetItem(itemId);
			if (item != null)
			{
				item.Load(itemData);
				AddItem(item);
				ActivateItem(item, true);
			}
		}
		items = (JSONArray)root["inactiveitems"];
		for (int i = 0; i < items.Count; ++i)
		{
			JSONObject itemData = (JSONObject)items[i];
			uint itemId = ((JSONNumber)itemData["id"]).UintValue;
			MRItem item = MRItemManager.GetItem(itemId);
			if (item != null)
			{
				item.Load(itemData);
				AddItem(item);
			}
		}
		items = null;
		int[] curses = new int[1];
		curses[0] = ((JSONNumber)root["curses"]).IntValue;
		mCurses = new BitArray(curses);

		return true;
	}
	
	public override void Save(JSONObject root)
	{
		base.Save(root);

		root["name"] = new JSONString(Name);
		root["fame"] = new JSONNumber(mFame);
		root["notoriety"] = new JSONNumber(mNotoriety);
		root["kills"] = new JSONNumber(mKillCountForDay);
		root["pony"] = new JSONBoolean(mPonyMove);
		root["riding"] = new JSONBoolean(mStartedDayRiding);
		root["normalactivities"] = new JSONNumber(mNormalActivitiesAvailable);
		root["dayactivities"] = new JSONNumber(mDaylightActivitesAvailable);
		JSONArray chits = new JSONArray(mActiveChits.Count);
		for (int i = 0; i < mActiveChits.Count; ++i)
		{
			chits[i] = new JSONNumber(mChits.IndexOf(mActiveChits[i]));
		}
		root["activechits"] = chits;
		chits = new JSONArray(mFatiguedChits.Count);
		for (int i = 0; i < mFatiguedChits.Count; ++i)
		{
			chits[i] = new JSONNumber(mChits.IndexOf(mFatiguedChits[i]));
		}
		root["fatiguedchits"] = chits;
		chits = new JSONArray(mWoundedChits.Count);
		for (int i = 0; i < mWoundedChits.Count; ++i)
		{
			chits[i] = new JSONNumber(mChits.IndexOf(mWoundedChits[i]));
		}
		root["woundedchits"] = chits;
		chits = null;
		JSONArray items = new JSONArray(mActiveItems.Count);
		for (int i = 0; i < mActiveItems.Count; ++i)
		{
			JSONObject itemData = new JSONObject();
			mActiveItems[i].Save(itemData);
			items[i] = itemData;
		}
		root["activeitems"] = items;
		items = new JSONArray(mInactiveItems.Count);
		for (int i = 0; i < mInactiveItems.Count; ++i)
		{
			JSONObject itemData = new JSONObject();
			mInactiveItems[i].Save(itemData);
			items[i] = itemData;
		}
		root["inactiveitems"] = items;
		items = null;
		int[] curses = new int[1];
		mCurses.CopyTo(curses, 0);
		root["curses"] = new JSONNumber(curses[0]);
	}

	#endregion

	#region Members
	
	protected MRGame.eMoveType mMoveType;
	protected bool mPonyMove;
	protected bool mStartedDayRiding;
	protected int mNormalActivitiesAvailable;
	protected int mDaylightActivitesAvailable;
	protected IDictionary<MRGame.eActivity, int> mExtraActivities = new Dictionary<MRGame.eActivity, int>();

	protected IList<MRActionChit> mChits = new List<MRActionChit>(12);
	protected IList<MRActionChit> mMoveChits = new List<MRActionChit>(12);
	protected IList<MRActionChit> mFightChits = new List<MRActionChit>(12);
	protected IList<MRActionChit> mMagicChits = new List<MRActionChit>(12);
	protected IList<MRActionChit> mActiveChits = new List<MRActionChit>(12);
	protected IList<MRActionChit> mFatiguedChits = new List<MRActionChit>(12);
	protected IList<MRActionChit> mWoundedChits = new List<MRActionChit>(12);

	protected int mFatigueBalance;
	protected int mWoundBalance;
	protected int mHealBalance;
	protected MRActionChit.eType mFatigueBalanceType;
	protected MRGame.eStrength mFatigueBalanceStrength;

	protected float mFame;
	protected float mNotoriety;
	protected int mKillCountForDay;

	protected IList<MRItem> mActiveItems = new List<MRItem>();
	protected IList<MRItem> mInactiveItems = new List<MRItem>();

	protected BitArray mCurses = new BitArray(Enum.GetValues(typeof(MRGame.eCurses)).Length);

	protected IList<MRDenizen> mHirelings = new List<MRDenizen>();

	protected GameObject mAttentionChit;

	protected string[] mStartingLocations;
	protected MRILocation mStartingLocation;

	#endregion
}
