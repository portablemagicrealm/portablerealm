//
// MRControllable.cs
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

public abstract class MRControllable : MRIControllable, MRISerializable
{
	#region Properties
	
	// How the controllable is moving.
	public abstract MRGame.eMoveType MoveType {get; set;}

	/// <summary>
	/// Returns the current attack strength of the controllable.
	/// </summary>
	/// <value>The current strength.</value>
	public abstract MRGame.eStrength CurrentStrength { get; }
	
	/// <summary>
	/// Returns the current attack speed of the controllable.
	/// </summary>
	/// <value>The current attack speed.</value>
	public abstract int CurrentAttackSpeed { get; }
	
	/// <summary>
	/// Returns the current move speed of the controllable.
	/// </summary>
	/// <value>The current move speed.</value>
	public abstract int CurrentMoveSpeed { get; }
	
	/// <summary>
	/// Returns the current sharpness of the controllable's active weapon.
	/// </summary>
	/// <value>The current sharpness.</value>
	public abstract int CurrentSharpness { get; }

	/// <summary>
	/// Returns the type of weapon used by the denizen.
	/// </summary>
	/// <value>The type of the weapon.</value>
	public abstract MRWeapon.eWeaponType WeaponType { get; }

	/// <summary>
	/// Gets the length of the controllable's active weapon.
	/// </summary>
	/// <value>The length of the weapon.</value>
	public abstract int WeaponLength { get; }

	/// <summary>
	/// Returns the attack direction being used this round.
	/// </summary>
	/// <value>The attack type.</value>
	public abstract MRCombatManager.eAttackType AttackType { get; }
	
	/// <summary>
	/// Returns the maneuver direction being used this round.
	/// </summary>
	/// <value>The defense type.</value>
	public abstract MRCombatManager.eDefenseType DefenseType { get; }

	public uint Id
	{
		get{
			return mId;
		}

		protected set{
			mId = value;
			MRGame.TheGame.AddGamePiece(this);
		}
	}

	public GameObject Counter
	{
		get{
			return mCounter;
		}
	}

	// Where the controllable is.
	public virtual MRILocation Location
	{
		get{
			return mLocation;
		}
		
		set{
			MRILocation oldLocation = mLocation;
			mLocation = value;
			if (oldLocation != null)
				oldLocation.RemovePiece(this);
			if (mLocation != null)
				mLocation.AddPieceToTop(this);
		}
	}

	// Returns a list of hidden roads/paths the controllable has discovered.
	public virtual IList<MRRoad> DiscoveredRoads 
	{
		get{
			return mDiscoveredRoads;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="MRControllable"/> is hidden.
	/// </summary>
	/// <value><c>true</c> if hidden; otherwise, <c>false</c>.</value>
	public virtual bool Hidden
	{
		get{
			return mHidden;
		}
		
		set{
			mHidden = value;
		}
	}

	// base weight of the controllable
	public virtual MRGame.eStrength BaseWeight
	{
		get{
			return mWeight;
		}
	}

	// Actual gold the controllable has
	public virtual int BaseGold 
	{ 
		get {
			return mGold;
		}

		set {
			mGold = value;
			if (mGold < 0)
				mGold = 0;
		}
	}
	
	// Effective gold the controllable has
	public virtual int EffectiveGold 
	{ 
		get {
			return BaseGold;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="MRIControlable"/> is blocked.
	/// </summary>
	/// <value><c>true</c> if blocked; otherwise, <c>false</c>.</value>
	public virtual bool Blocked 
	{ 
		get{
			return mBlocked;
		}

		set{
			mBlocked = value;
			if (mBlocked)
			{
				Hidden = false;
				// cancel any remaining activities
				MRActivityList activities = ActivitiesForDay(MRGame.DayOfMonth);
				if (activities != null)
				{
					foreach (MRActivity activity in activities.Activities)
					{
						if (!activity.Executed)
							activity.Canceled = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets the combat target.
	/// </summary>
	/// <value>The combat target.</value>
	public virtual MRIControllable CombatTarget 
	{ 
		get {
			return mCombatTarget;
		}

		set {
			if (mCombatTarget != null)
				mCombatTarget.RemoveAttacker(this);
			mCombatTarget = value;
			if (value != null)
			{
				Hidden = false;
				mCombatTarget.AddAttacker(this);
			}
		}
	}

	/// <summary>
	/// Returns the list of things attacking this target.
	/// </summary>
	/// <value>The attackers.</value>
	public virtual IList<MRIControllable> Attackers
	{ 
		get {
			return mAttackers;
		}
	}

	/// <summary>
	/// Returns the list of attackers who killed this controllable.
	/// </summary>
	/// <value>The killers.</value>
	public virtual IList<MRIControllable> Killers
	{
		get{
			return mKillers;
		}
	}

	// The combat sheet the controllable is on.
	public virtual MRCombatSheetData CombatSheet 
	{ 
		get {
			return mCombatSheet;
		}

		set {
			mCombatSheet = value;
		}
	}
	
	// The controllable this controllable is luring in combat.
	public virtual MRIControllable Luring 
	{ 
		get {
			return mLuring;
		}

		set {
			mLuring = value;
		}
	}
	
	// The controllable luring this controllable in combat.
	public virtual MRIControllable Lurer 
	{ 
		get {
			return mLurer;
		}

		set {
			mLurer = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether the comtrollable is dead.
	/// </summary>
	/// <value><c>true</c> if the controllable is dead; otherwise, <c>false</c>.</value>
	public bool IsDead 
	{ 
		get {
			return KillerCount > 0;
		}
	}

	/// <summary>
	/// Returns how many combatants killed this controllable (due to simultanious attacks)
	/// </summary>
	/// <value>The killer count.</value>
	public int KillerCount
	{
		get{
			return mKillers.Count;
		}
	}

	/// <summary>
	/// Returns if this is a red-side up tremendous monster.
	/// </summary>
	/// <value><c>true</c> if this instance is red side monster; otherwise, <c>false</c>.</value>
	public virtual bool IsRedSideMonster 
	{ 
		get {
			return false;
		}
	}

	/**********************/
	// MRIGamePiece properties

	public virtual string Name 
	{ 
		get{
			return mName;
		}
	}

	public Bounds Bounds
	{
		get{
			if (mCounter != null)
				return mCounter.GetComponentInChildren<SpriteRenderer>().sprite.bounds;
			else
				return new Bounds();
		}
	}

	public virtual int Layer
	{
		get{
			return mCounter.layer;
		}
		
		set{
			if (value >= 0 && value < 32)
			{
				mCounter.layer = value;
				// todo: figure out why we need both of these to get the counter displayed correctly
				for (int i = 0; i < mCounter.transform.childCount; ++i)
				{
					Transform childTransform = mCounter.transform.GetChild(i);
					childTransform.gameObject.layer = value;
				}
				foreach (Transform transform in mCounter.GetComponentsInChildren<Transform>())
				{
					transform.gameObject.layer = value;
				}
			}
			else
			{
				Debug.LogError("Trying to set controllable " + Name + " to layer " + value);
			}
		}
	}
	
	public virtual Vector3 Position
	{
		get{
			return mCounter.transform.position;
		}

		set{
			mCounter.transform.position = value;
			UpdateAttackerPositions();
		}

	}

	public virtual Vector3 LocalScale
	{
		get{
			return mCounter.transform.localScale;
		}
		
		set{
			if (mLocation != null && mLocation.Pieces == Stack)
			{
				Counter.transform.localScale = new Vector3(
					1.0f / mLocation.Owner.transform.localScale.x,
					1.0f / mLocation.Owner.transform.localScale.y,
					1.0f);
			}
			else
			{
				mCounter.transform.localScale = value;
			}
			UpdateAttackerPositions();
		}
	}

	public virtual Vector3 LossyScale 
	{ 
		get {
			return mCounter.transform.lossyScale;
		}
	}

	public virtual Quaternion Rotation 
	{ 
		get {
			return mCounter.transform.rotation;
		}
		
		set {
			mCounter.transform.rotation = value;
		}
	}

	public virtual Transform Parent
	{
		get{
			return mCounter.transform.parent;
		}

		set {
			mCounter.transform.parent = value;
		}
	}

	public virtual Vector3 OldScale 
	{ 
		get	{
			return mOldScale;
		}

		set {
			mOldScale = value;
		}
	}

	public virtual MRGamePieceStack Stack 
	{ 
		get{
			return mStack;
		}

		set{
			mStack = value;
		}
	}

	#endregion

	#region Methods

	// Does initialization associated with birdsong.
	public abstract void StartBirdsong();
	
	// Does initialization associated with sunrise.
	public abstract void StartSunrise();
	
	// Does initialization associated with daylight.
	public abstract void StartDaylight();
	
	// Does initialization associated with sunset.
	public abstract void StartSunset();
	
	// Does initialization associated with evening.
	public abstract void StartEvening();
	
	// Does initialization associated with midnight.
	public virtual void StartMidnight()
	{
		Blocked = false;
	}
	
	// Tests if the controllable is allowed to do the activity.
	public abstract bool CanExecuteActivity(MRActivity activity);
	
	// Tells the controllable an activity was performed
	public abstract void ExecutedActivity(MRActivity activity);

	protected MRControllable()
	{
	}
	
	protected MRControllable(JSONObject jsonData, int index)
	{
		mName = ((JSONString)jsonData["name"]).Value;

		string weight = ((JSONString)jsonData["weight"]).Value;
		mWeight = weight.Strength();

		Id = MRUtility.IdForName(mName, index);
	}

	/// <summary>
	/// Cleans up resources used by the instance prior to being removed.
	/// </summary>
	public virtual void Destroy()
	{
		// remove ourself from the board
		if (Stack != null)
			Stack.RemovePiece(this);

		if (mCounter != null)
		{
			GameObject.DestroyObject(mCounter);
			mCounter = null;
		}
	}

	void OnDestroy()
	{
		Destroy ();
	}

	// Tests if this controllable activates a map chit to summon monsters
	public virtual bool ActivatesChit(MRMapChit chit)
	{
		return true;
	}

	/// <summary>
	/// Returns the die pool for a given roll type
	/// </summary>
	/// <returns>The pool.</returns>
	/// <param name="roll">Roll type.</param>
	public virtual MRDiePool DiePool(MRGame.eRollTypes roll)
	{
		return MRDiePool.DefaultPool;
	}

	// Returns the activity list for a given day. If the activity list doesn't exist, it will be created.
	public virtual MRActivityList ActivitiesForDay(int day)
	{
		// note day is base-1
		while (mActivities.Count < day)
		{
			MRActivityList activityList = new MRActivityList(this);
			activityList.AddActivity(MRActivity.CreateActivity(MRGame.eActivity.None));
//			activityList.AddActivity(MRActivity.CreateActivity(MRGame.eActivity.None));
			mActivities.Add(activityList);
		}
		return mActivities[day - 1];
	}

	// Removes all blank activities from the end of the activities lists
	public virtual void CleanupActivities()
	{
		foreach (MRActivityList activities in mActivities)
		{
			while (activities.Activities.Count > 0)
			{
				MRActivity activity = activities.Activities[activities.Activities.Count - 1];
				if (activity.Activity == MRGame.eActivity.None)
					activities.Activities.Remove(activity);
				else
					break;
			}
		}
	}

	// Flags a site as being discovered
	public virtual void DiscoverSite(MRMapChit.eSiteChitType site)
	{
		mDiscoveredSites.Set((int)site, true);
	}
	
	// Returns if a site has been discovered
	public virtual bool DiscoveredSite(MRMapChit.eSiteChitType site)
	{
		return mDiscoveredSites.Get((int)site);
	}
	
	// Flags a treasure as being discovered
	public virtual void DiscoverTreasure(uint treasureId)
	{
		if (!mDiscoveredTreasues.Contains(treasureId))
			mDiscoveredTreasues.Add(treasureId);
	}
	
	// Returns if a treasure has been discovered
	public virtual bool DiscoveredTreasure(uint treasureId)
	{
		return mDiscoveredTreasues.Contains(treasureId);
	}

	// Returns if a site can be looted by the controllable
	public virtual bool CanLootSite(MRSiteChit site)
	{
		if (!DiscoveredSite(site.SiteType))
			return false;
		return true;
	}

	// Returns if a twit site can be looted by the controllable
	public virtual bool CanLootTwit(MRTreasure twit)
	{
		if (!DiscoveredTreasure(twit.Id))
			return false;
		return true;
	}

	// Called when a site is looted by the controllable
	public virtual void OnSiteLooted(MRSiteChit site, bool success)
	{
		if (site.SiteType == MRMapChit.eSiteChitType.Vault)
			MRSiteChit.VaultOpened = true;
	}
	
	// Called when a twit site is looted by the controllable
	public virtual void OnTwitLooted(MRTreasure twit, bool success)
	{
	}

	// Adds a controllable to the list of things attacking this target
	public virtual void AddAttacker(MRIControllable attacker)
	{
		if (!mAttackers.Contains(attacker))
		{
			mAttackers.Insert(0, attacker);
			UpdateAttackerPositions();
		}
	}
	
	// Removes a controllable from the list of things attacking this target
	public virtual void RemoveAttacker(MRIControllable attacker)
	{
		if (attacker != null)
		{
			if (attacker is MRCharacter)
				((MRCharacter)attacker).PositionAttentionChit(null, Vector3.zero);
			mAttackers.Remove(attacker);
			UpdateAttackerPositions();
		}
	}

	// Called when the controllable hits its target
	public virtual void HitTarget(MRIControllable attacker, bool targetDead)
	{
		// do nothing
	}
	
	// Called when the controllable misses its target
	public virtual void MissTarget(MRIControllable attacker)
	{
		// do nothing
	}

	/// <summary>
	/// Awards the spoils of combat for killing a combatant.
	/// </summary>
	/// <param name="killed">Combatant that was killed.</param>
	/// <param name="awardFraction">Fraction of the spoils to award (due to shared kills).</param>
	public abstract void AwardSpoils(MRIControllable killed, float awardFraction);

	/// <summary>
	/// If this controllable is being attacked, shows the attacker chits over this chit.
	/// </summary>
	protected virtual void UpdateAttackerPositions()
	{
		if (mAttackers.Count > 0)
		{
			Vector3 cornerPosition = new Vector3();
			foreach (SpriteRenderer sprite in mCounter.GetComponentsInChildren<SpriteRenderer>())
			{
				if (cornerPosition.x == 0)
				{
					Bounds bounds = mCounter.GetComponentInChildren<SpriteRenderer>().bounds;
					Vector3 scale1 = mCounter.transform.localScale;
					Vector3 scale2 = mCounter.transform.lossyScale;
					cornerPosition.x = bounds.extents.x / scale1.x;
					cornerPosition.y = bounds.extents.y / scale1.y;
				}
				if (sprite.gameObject.transform.localPosition.z < cornerPosition.z)
					cornerPosition.z = sprite.gameObject.transform.localPosition.z;
			}

			foreach (MRIControllable attacker in mAttackers)
			{
				if (attacker is MRCharacter)
				{
					MRCharacter character = (MRCharacter)attacker;
					character.PositionAttentionChit(mCounter, cornerPosition);
				}
			}
		}
	}

	public virtual bool Load(JSONObject root)
	{
		if (root == null)
			return false;

		if (((JSONNumber)root["id"]).UintValue != mId)
			return false;

		mGold = ((JSONNumber)root["gold"]).IntValue;
		mHidden = ((JSONBoolean)root["hidden"]).Value;
		mBlocked = ((JSONBoolean)root["blocked"]).Value;
		mDiscoveredRoads.Clear();
		JSONArray roads = (JSONArray)root["roads"];
		for (int i = 0; i < roads.Count; ++i)
		{
			string roadName = ((JSONString)roads[i]).Value;
			MRRoad road;
			if (MRGame.TheGame.TheMap.Roads.TryGetValue(roadName, out road))
				mDiscoveredRoads.Add(road);
			else
			{
				Debug.LogError("Controllable " + mId + " load unknown road " + roadName);
			}
		}
		mDiscoveredTreasues.Clear();
		JSONArray treasures = (JSONArray)root["treasures"];
		for (int i = 0; i < treasures.Count; ++i)
		{
			mDiscoveredTreasues.Add((uint)((JSONNumber)treasures[i]).IntValue);
		}
		int[] sites = new int[1];
		sites[0] = ((JSONNumber)root["sites"]).IntValue;
		mDiscoveredSites = new BitArray(sites);
		mActivities.Clear();
		JSONArray activities = (JSONArray)root["activities"];
		for (int i = 0; i < activities.Count; ++i)
		{
			MRActivityList activityList = new MRActivityList(this);
			activityList.Load((JSONObject)activities[i]);
			mActivities.Add(activityList);
		}
		if (root["location"] != null)
		{
			uint locationId = ((JSONNumber)root["location"]).UintValue;
			MRILocation location = MRGame.TheGame.GetLocation(locationId);
			if (location != null)
			{
				Location = location;
			}
			else
			{
				Debug.LogError("Invalid location " + locationId + " for creature " + Name);
				return false;
			}
		}

		return true;
	}
	
	public virtual void Save(JSONObject root)
	{
		root["id"] = new JSONNumber(mId);
		root["gold"] = new JSONNumber(mGold);
		root["hidden"] = new JSONBoolean(mHidden);
		root["blocked"] = new JSONBoolean(mBlocked);
		if (Location != null)
		{
			root["location"] = new JSONNumber(Location.Id);
		}
		JSONArray roads = new JSONArray(mDiscoveredRoads.Count);
		for (int i = 0; i < mDiscoveredRoads.Count; ++i)
		{
			roads[i] = new JSONString(mDiscoveredRoads[i].Name);
		}
		root["roads"] = roads;
		JSONArray treasures = new JSONArray(mDiscoveredTreasues.Count);
		for (int i = 0; i < mDiscoveredTreasues.Count; ++i)
		{
			treasures[i] = new JSONNumber(mDiscoveredTreasues[i]);
		}
		root["treasures"] = treasures;
		int[] sites = new int[1];
		mDiscoveredSites.CopyTo(sites, 0);
		root["sites"] = new JSONNumber(sites[0]);
		JSONArray activities = new JSONArray(mActivities.Count);
		for (int i = 0; i < mActivities.Count; ++i)
		{
			JSONObject activityList = new JSONObject();
			mActivities[i].Save(activityList);
			activities[i] = activityList;
		}
		root["activities"] = activities;
	}

	// Add an item to the controllable's items.
	public abstract void AddItem(MRItem item);

	// Returns the weight of the heaviest item owned by the controllable
	public abstract MRGame.eStrength GetHeaviestWeight(bool includeHorse, bool includeSelf);


	/**********************/
	// MRIGamePiece methods

	public abstract void Update();

	#endregion

	#region Members
	
	protected string mName;
	protected GameObject mCounter;
	protected Vector3 mOldScale;
	protected MRILocation mLocation;
	protected IList<MRRoad> mDiscoveredRoads = new List<MRRoad>();
	protected IList<MRActivityList> mActivities = new List<MRActivityList>();
	protected BitArray mDiscoveredSites = new BitArray(Enum.GetValues(typeof(MRMapChit.eSiteChitType)).Length);
	protected IList<uint> mDiscoveredTreasues = new List<uint>();
	protected MRGame.eStrength mWeight;
	protected int mGold;
	protected bool mHidden;
	protected bool mBlocked;
	protected MRIControllable mCombatTarget;
	protected IList<MRIControllable> mAttackers = new List<MRIControllable>();
	protected IList<MRIControllable> mKillers = new List<MRIControllable>();
	protected MRGamePieceStack mStack;
	protected MRCombatSheetData mCombatSheet;
	protected MRIControllable mLuring;
	protected MRIControllable mLurer;
	private uint mId;

	#endregion
}
