//
// MRCombatSheetData.cs
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

public class MRCombatSheetData
{
	#region Subclasses

	public class MRCharacterCombatData
	{
		public MRWeapon weapon;
		public MRArmor shield;
		public MRArmor breastplate;
		public MRArmor helmet;
		public MRArmor fullArmor;
		public MRFightChit attackChit;
		public MRMoveChit maneuverChit;
		public MRCombatManager.eAttackType attackType = MRCombatManager.eAttackType.None;
		public MRCombatManager.eDefenseType maneuverType = MRCombatManager.eDefenseType.None;
		public MRCombatManager.eAttackType shieldType = MRCombatManager.eAttackType.None;
		public int pendingWounds;
	}

	public class AttackerData
	{
		public MRDenizen attacker;
		public MRCombatManager.eAttackType attackType;

		public AttackerData(MRDenizen _attacker, MRCombatManager.eAttackType _attackType)
		{
			attacker = _attacker;
			attackType = _attackType;
		}
	}

	public class DefenderData
	{
		public MRDenizen defender;
		public MRCombatManager.eDefenseType defenseType;

		public DefenderData(MRDenizen _defender, MRCombatManager.eDefenseType _defenseType)
		{
			defender = _defender;
			defenseType = _defenseType;
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// Adds an attacker to an attack square.
	/// </summary>
	/// <param name="attacker">Attacker.</param>
	/// <param name="slot">Slot to put the attacker on.</param>
	public void AddAttacker(MRDenizen attacker, MRCombatManager.eAttackType slot)
	{
		if (attacker.CombatSheet != null)
			attacker.CombatSheet.RemoveCombatant(attacker);

		attacker.CombatSheet = this;
		Attackers.Insert(0, new AttackerData(attacker, slot));
	}

	/// <summary>
	/// Adds a defender to a defense square. Will add it to a random one, but try to keep all squares evenly filled.
	/// </summary>
	/// <param name="defender">Defender.</param>
	public void AddDefender(MRDenizen defender)
	{
		// get the number of defenders in each slot
		IDictionary<MRCombatManager.eDefenseType, int> slots = new Dictionary<MRCombatManager.eDefenseType, int>();
		slots[MRCombatManager.eDefenseType.Charge] = 0;
		slots[MRCombatManager.eDefenseType.Dodge] = 0;
		slots[MRCombatManager.eDefenseType.Duck] = 0;
		foreach (DefenderData data in Defenders)
		{
			++slots[data.defenseType];
		}
		int lowestCount = int.MaxValue;
		foreach (int value in slots.Values)
		{
			if (value < lowestCount)
				lowestCount = value;
		}
		MRCombatManager.eDefenseType pick = MRCombatManager.eDefenseType.None;
		do
		{
			int rnd = UnityEngine.Random.Range(0, Enum.GetValues(typeof(MRCombatManager.eDefenseType)).Length);
			pick = (MRCombatManager.eDefenseType)Enum.GetValues(typeof(MRCombatManager.eDefenseType)).GetValue(rnd);
		} while (pick == MRCombatManager.eDefenseType.None || slots[pick] != lowestCount);

		AddDefender(defender, pick);
	}

	/// <summary>
	/// Adds a defender to a defense square.
	/// </summary>
	/// <param name="defender">Defender.</param>
	/// <param name="slot">Slot to put the defender on.</param>
	public void AddDefender(MRDenizen defender, MRCombatManager.eDefenseType slot)
	{
		if (defender.CombatSheet != null)
			defender.CombatSheet.RemoveCombatant(defender);

		defender.CombatSheet = this;
		Defenders.Insert(0, new DefenderData(defender, slot));
	}

	public void AddDefenderTarget(MRDenizen target, MRCombatManager.eDefenseType slot)
	{
		if (DefenderTarget != null)
		{
			Debug.LogError("Tried to add defender target to defender with existing target");
			return;
		}

		if (target.CombatSheet != null)
			target.CombatSheet.RemoveCombatant(target);

		target.CombatSheet = this;
		DefenderTarget = new DefenderData(target, slot);
	}

	public AttackerData FindAttacker(MRDenizen attacker)
	{
		foreach (AttackerData data in Attackers)
		{
			if (data.attacker == attacker)
				return data;
		}
		return null;
	}

	public DefenderData FindDefender(MRDenizen target)
	{
		foreach (DefenderData data in Defenders)
		{
			if (data.defender == target)
				return data;
		}
		if (DefenderTarget != null && DefenderTarget.defender == target)
			return DefenderTarget;
		return null;
	}

	/// <summary>
	/// Removes a combatant from the sheet. 
	/// </summary>
	/// <param name="combatant">The combatant to remove.</param>
	public void RemoveCombatant(MRIControllable combatant)
	{
		if (combatant.CombatSheet != this)
			return;

		combatant.CombatSheet = null;

		foreach (DefenderData data in Defenders)
		{
			if (data.defender == combatant)
			{
				Defenders.Remove(data);
				return;
			}
		}
		foreach (AttackerData data in Attackers)
		{
			if (data.attacker == combatant)
			{
				Attackers.Remove(data);
				return;
			}
		}
		if (DefenderTarget.defender == combatant)
			DefenderTarget = null;
	}

	/// <summary>
	/// Removes all combatants from this sheet.
	/// </summary>
	public void RemoveCombatants()
	{
		foreach (DefenderData data in Defenders)
		{
			data.defender.CombatSheet = null;
		}
		foreach (AttackerData data in Attackers)
		{
			data.attacker.CombatSheet = null;
		}
		if (DefenderTarget != null)
			DefenderTarget.defender.CombatSheet = null;
		Defenders.Clear();
		Attackers.Clear();
		DefenderTarget = null;
	}

	/// <summary>
	/// Returns he character's armor hit by a given attack direction.
	/// </summary>
	/// <returns>The armor for attack.</returns>
	/// <param name="attack">Attack.</param>
	public MRArmor CharacterArmorForAttack(MRCombatManager.eAttackType attack)
	{
		if (CharacterData == null)
			return null;

		if (CharacterData.shield != null && CharacterData.shieldType == attack)
			return CharacterData.shield;

		if (CharacterData.breastplate != null && 
		    (attack == MRCombatManager.eAttackType.Thrust || attack == MRCombatManager.eAttackType.Swing))
		{
			return CharacterData.breastplate;
		}

		if (CharacterData.helmet != null && attack == MRCombatManager.eAttackType.Smash)
			return CharacterData.helmet;

		if (CharacterData.fullArmor != null)
			return CharacterData.fullArmor;

		return null;
	}

	#endregion

	#region Members

	public MRControllable SheetOwner;
	public MRCharacterCombatData CharacterData;
	public IList<AttackerData> Attackers = new List<AttackerData>();
	public IList<DefenderData> Defenders = new List<DefenderData>();
	// the target of a defending denizen on their own sheet
	public DefenderData DefenderTarget;

	#endregion
}

