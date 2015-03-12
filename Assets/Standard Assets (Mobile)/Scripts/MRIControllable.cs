//
// MRIControlable.cs
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

// Interface for anything that has the potential to take activities (player characters, natives, monsters)
public interface MRIControllable : MRIGamePiece
{
	#region Properties

	// Where the controllable is.
	MRILocation Location {get; set;}

	// How the controllable is moving.
	MRGame.eMoveType MoveType {get; set;}

	// Returns a list of hidden roads/paths the controllable has discovered.
	IList<MRRoad> DiscoveredRoads {get;}

	// The hidden state of the controllable
	bool Hidden {get; set;}

	// base weight of the controllable
	MRGame.eStrength BaseWeight { get; }

	// Actual gold the controllable has
	int BaseGold { get; set; }

	// Effective gold the controllable has
	int EffectiveGold { get; }

	/// <summary>
	/// Returns the current attack strength of the controllable.
	/// </summary>
	/// <value>The current strength.</value>
	MRGame.eStrength CurrentStrength { get; }

	/// <summary>
	/// Returns the current attack speed of the controllable.
	/// </summary>
	/// <value>The current attack speed.</value>
	int CurrentAttackSpeed { get; }

	/// <summary>
	/// Returns the current move speed of the controllable.
	/// </summary>
	/// <value>The current move speed.</value>
	int CurrentMoveSpeed { get; }

	/// <summary>
	/// Returns the current sharpness of the controllable's weapon.
	/// </summary>
	/// <value>The current sharpness.</value>
	int CurrentSharpness { get; }

	/// <summary>
	/// Returns the type of weapon used by the denizen.
	/// </summary>
	/// <value>The type of the weapon.</value>
	MRWeapon.eWeaponType WeaponType { get; }

	/// <summary>
	/// Gets the length of the controllable's weapon.
	/// </summary>
	/// <value>The length of the weapon.</value>
	int WeaponLength { get; }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="MRIControlable"/> is blocked.
	/// </summary>
	/// <value><c>true</c> if blocked; otherwise, <c>false</c>.</value>
	bool Blocked { get; set; }

	/// <summary>
	/// Gets or sets the combat target.
	/// </summary>
	/// <value>The combat target.</value>
	MRIControllable CombatTarget { get; set; }

	/// <summary>
	/// Returns the list of things attacking this target.
	/// </summary>
	/// <value>The attackers.</value>
	IList<MRIControllable> Attackers { get; }

	/// <summary>
	/// Returns the list of attackers who killed this controllable.
	/// </summary>
	/// <value>The killers.</value>
	IList<MRIControllable> Killers { get; }

	/// <summary>
	/// Returns the attack direction being used this round.
	/// </summary>
	/// <value>The attack type.</value>
	MRCombatManager.eAttackType AttackType { get; }

	/// <summary>
	/// Returns the maneuver direction being used this round.
	/// </summary>
	/// <value>The defense type.</value>
	MRCombatManager.eDefenseType DefenseType { get; }

	// The combat sheet the controllable is on.
	MRCombatSheetData CombatSheet { get; set; }

	// The controllable this controllable is luring in combat.
	MRIControllable Luring { get; set; }

	// The controllable luring this controllable in combat.
	MRIControllable Lurer { get; set; }

	/// <summary>
	/// Gets a value indicating whether the comtrollable is dead.
	/// </summary>
	/// <value><c>true</c> if the controllable is dead; otherwise, <c>false</c>.</value>
	bool IsDead { get; }

	/// <summary>
	/// Returns how many combatants killed this controllable (due to simultanious attacks)
	/// </summary>
	/// <value>The killer count.</value>
	int KillerCount { get; }

	/// <summary>
	/// Returns if this is a red-side up tremendous monster.
	/// </summary>
	/// <value><c>true</c> if this instance is red side monster; otherwise, <c>false</c>.</value>
	bool IsRedSideMonster { get; }

	#endregion

	#region Methods

	// Does initialization associated with birdsong.
	void StartBirdsong();

	// Does initialization associated with sunrise.
	void StartSunrise();

	// Does initialization associated with daylight.
	void StartDaylight();

	// Does initialization associated with sunset.
	void StartSunset();

	// Does initialization associated with evening.
	void StartEvening();

	// Does initialization associated with midnight.
	void StartMidnight();

	// Returns the activity list for a given day. If the activity list doesn't exist, it will be created.
	MRActivityList ActivitiesForDay(int day);

	// Removes all blank activities from the end of the activities lists.
	void CleanupActivities();

	// Tests if the controllable is allowed to do the activity.
	bool CanExecuteActivity(MRActivity activity);

	// Tells the controllable an activity was performed
	void ExecutedActivity(MRActivity activity);

	// Tests if this controllable activates a map chit to summon monsters
	bool ActivatesChit(MRMapChit chit);

	// Returns the die pool for a given roll type
	MRDiePool DiePool(MRGame.eRollTypes roll);

	// Flags a site as being discovered
	void DiscoverSite(MRMapChit.eSiteChitType site);

	// Returns if a site has been discovered
	bool DiscoveredSite(MRMapChit.eSiteChitType site);

	// Flags a treasure as being discovered
	void DiscoverTreasure(uint treasureId);

	// Returns if a treasure has been discovered
	bool DiscoveredTreasure(uint treasureId);

	// Returns if a site can be looted by the controllable
	bool CanLootSite(MRSiteChit site);

	// Returns if a twit site can be looted by the controllable
	bool CanLootTwit(MRTreasure twit);

	// Called when a site is looted by the controllable
	void OnSiteLooted(MRSiteChit site, bool success);

	// Called when a twit site is looted by the controllable
	void OnTwitLooted(MRTreasure twit, bool success);

	// Adds an item to the controllable's items.
	void AddItem(MRItem item);

	// Returns the weight of the heaviest item owned by the controllable
	MRGame.eStrength GetHeaviestWeight(bool includeHorse, bool includeSelf);

	// Adds a controllable to the list of things attacking this target
	void AddAttacker(MRIControllable attacker);

	// Removes a controllable from the list of things attacking this target
	void RemoveAttacker(MRIControllable attacker);

	// Called when the controllable hits its target
	void HitTarget(MRIControllable attacker, bool targetDead);

	// Called when the controllable misses its target
	void MissTarget(MRIControllable attacker);

	/// <summary>
	/// Awards the spoils of combat for killing a combatant.
	/// </summary>
	/// <param name="killed">Combatant that was killed.</param>
	/// <param name="awardFraction">Fraction of the spoils to award (due to shared kills).</param>
	void AwardSpoils(MRIControllable killed, float awardFraction);

	#endregion
}

