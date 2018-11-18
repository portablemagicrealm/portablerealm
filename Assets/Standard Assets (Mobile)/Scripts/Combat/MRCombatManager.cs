//
// MRCombat.cs
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

namespace PortableRealm
{
	
public class MRCombatManager
{
	#region Constants

	public enum eCombatPhase
	{
		PreStartRound,
		StartRound,
		Lure,
		RandomAssignment,
		Deployment,
		TakeAction,
		SelectTarget,
		ActivateSpells,
		SelectAttackAndManeuver,
		RandomizeAttacks,
		ResolveAttacks,
		FatigueChits,
		Disengage,
		CombatDone
	}

	public enum eAttackType
	{
		None,
		Thrust,
		Swing,
		Smash
	}

	public enum eDefenseType
	{
		None,
		Charge,
		Dodge,
		Duck
	}

	public enum eAttackManeuverOption
	{
		None,
		Cancel
	}

	public enum eHitResult
	{
		Miss,
		Undercut,
		Intercept
	}

	private static readonly IDictionary<int, IDictionary<eDefenseType, eDefenseType>> DefenderReposition = new Dictionary<int, IDictionary<eDefenseType, eDefenseType>>
	{
		{1, new Dictionary<eDefenseType, eDefenseType>
			{
				{eDefenseType.Charge, eDefenseType.Charge},
				{eDefenseType.Dodge, eDefenseType.Duck},
				{eDefenseType.Duck, eDefenseType.Dodge}
			}},
		{2, new Dictionary<eDefenseType, eDefenseType>
			{
				{eDefenseType.Charge, eDefenseType.Duck},
				{eDefenseType.Dodge, eDefenseType.Dodge},
				{eDefenseType.Duck, eDefenseType.Charge}
			}},
		{3, new Dictionary<eDefenseType, eDefenseType>
			{
				{eDefenseType.Charge, eDefenseType.Dodge},
				{eDefenseType.Dodge, eDefenseType.Charge},
				{eDefenseType.Duck, eDefenseType.Duck}
			}},
		{4, new Dictionary<eDefenseType, eDefenseType>
			{
				{eDefenseType.Charge, eDefenseType.Charge},
				{eDefenseType.Dodge, eDefenseType.Dodge},
				{eDefenseType.Duck, eDefenseType.Duck}
			}},
		{5, new Dictionary<eDefenseType, eDefenseType>
			{
				{eDefenseType.Charge, eDefenseType.Dodge},
				{eDefenseType.Dodge, eDefenseType.Duck},
				{eDefenseType.Duck, eDefenseType.Charge}
			}},
		{6, new Dictionary<eDefenseType, eDefenseType>
			{
				{eDefenseType.Charge, eDefenseType.Duck},
				{eDefenseType.Dodge, eDefenseType.Charge},
				{eDefenseType.Duck, eDefenseType.Dodge}
			}}
	};

	private static readonly IDictionary<int, IDictionary<eAttackType, eAttackType>> AttackerReposition = new Dictionary<int, IDictionary<eAttackType, eAttackType>>
	{
		{1, new Dictionary<eAttackType, eAttackType>
			{
				{eAttackType.Thrust, eAttackType.Thrust},
				{eAttackType.Swing, eAttackType.Smash},
				{eAttackType.Smash, eAttackType.Swing}
			}},
		{2, new Dictionary<eAttackType, eAttackType>
			{
				{eAttackType.Thrust, eAttackType.Smash},
				{eAttackType.Swing, eAttackType.Swing},
				{eAttackType.Smash, eAttackType.Thrust}
			}},
		{3, new Dictionary<eAttackType, eAttackType>
			{
				{eAttackType.Thrust, eAttackType.Swing},
				{eAttackType.Swing, eAttackType.Thrust},
				{eAttackType.Smash, eAttackType.Smash}
			}},
		{4, new Dictionary<eAttackType, eAttackType>
			{
				{eAttackType.Thrust, eAttackType.Thrust},
				{eAttackType.Swing, eAttackType.Swing},
				{eAttackType.Smash, eAttackType.Smash}
			}},
		{5, new Dictionary<eAttackType, eAttackType>
			{
				{eAttackType.Thrust, eAttackType.Swing},
				{eAttackType.Swing, eAttackType.Smash},
				{eAttackType.Smash, eAttackType.Thrust}
			}},
		{6, new Dictionary<eAttackType, eAttackType>
			{
				{eAttackType.Thrust, eAttackType.Smash},
				{eAttackType.Swing, eAttackType.Thrust},
				{eAttackType.Smash, eAttackType.Swing}
			}}
	};

	/// <summary>
	/// The missile table. Maps a die roll to the bonus/penalty damage for a missile attack.
	/// </summary>
	private static readonly IDictionary<int, int> MissileTable = new Dictionary<int, int>
	{
		{1, 2},
		{2, 1},
		{3, 0},
		{4, -1},
		{5, -2},
		{6, -3}
	};

	#endregion

	#region Properties

	public eCombatPhase CombatPhase
	{
		get{
			return mCombatPhase;
		}

		set{
			mCombatPhase = value;
			if (mCombatPhase == eCombatPhase.CombatDone)
				EndCombat();
		}
	}

	public bool AllowEndCombat
	{
		get{
			return mAllowEndCombat;
		}
	}

	public eAttackType LastSelectedAttackType
	{
		get {
			return mLastSelectedAttackType;
		}

		set{
			mLastSelectedAttackType = value;
		}
	}

	public eDefenseType LastSelectedDefenseType
	{
		get {
			return mLastSelectedDefenseType;
		}
		
		set{
			mLastSelectedDefenseType = value;
		}
	}

	public MRClearing Clearing
	{
		get{
			return mClearing;
		}

		set{
			EndCombat();
			mClearing = value;
			if (mClearing != null)
				StartCombat();
		}
	}

	public MRGamePieceStack CombatStack
	{
		get{
			return mCombatStack;
		}
	}

	public bool Active
	{
		get{
			return (mClearing != null);
		}
	}

	public IList<MRControllable> Combatants
	{
		get{
			return mCombatants;
		}
	}

	public MRControllable CurrentCombatant
	{
		get
		{
			if (mCurrentCombatantIndex >= 0 && mCurrentCombatantIndex < mCombatants.Count)
				return mCombatants[mCurrentCombatantIndex];
			return null;
		}
	}

	public IList<MRCombatSheetData> CombatSheets
	{
		get{
			return mCombatSheets;
		}
	}

	public int CurrentCombatantSheetIndex
	{
		get
		{
			return mCurrentSheetIndex;
		}

		set
		{
			if (value < 0)
				value = mCombatSheets.Count - 1;
			else if (value >= mCombatSheets.Count)
				value = 0;
			mCurrentSheetIndex = value;
		}
	}

	public MRCombatSheetData CurrentCombatantSheet
	{
		get
		{
			if (mCurrentSheetIndex >= 0 && mCurrentSheetIndex < mCombatSheets.Count)
				return mCombatSheets[mCurrentSheetIndex];
			return null;
		}
	}

	public IList<MRDenizen> Enemies
	{
		get{
			return mEnemies;
		}
	}

	public MRSpell TargetingSpell
	{
		get{
			return mTargetingSpell;
		}
	}

	#endregion
	
	#region Methods

	public MRCombatManager()
	{
		mCombatPhase = eCombatPhase.CombatDone;
		mLastSelectedAttackType = eAttackType.Thrust;
		mLastSelectedDefenseType = eDefenseType.Charge;
		mCombatStack = MRGame.TheGame.NewGamePieceStack();
		mCombatStack.gameObject.name = "CombatStack";
		mCombatStack.Inspecting = true;
	}

	/// <summary>
	/// Update the current combat.
	/// </summary>
	/// <returns>true if the combat is still going, false if it is over</returns>
	public bool Update()
	{
		if (mClearing == null || mCombatPhase == eCombatPhase.CombatDone)
			return false;

		if (mCurrentSheetIndex >= 0 && mCurrentSheetIndex < mCombatSheets.Count)
		{
			MRGame.TheGame.CombatSheet.CombatData = mCombatSheets[mCurrentSheetIndex];
		}

		if (MRGame.TheGame.InspectionStack != null && MRGame.TheGame.InspectionStack != mCombatStack)
		{
			mCombatStack.Visible = false;
		}
		else
		{
			mCombatStack.Visible = true;
		}

		if (mCombatPhase == eCombatPhase.FatigueChits && mCurrentCombatantIndex >= 0)
		{
			// this will happen after the character has fatigued their chits (fatigue event takes precidence over combat),
			// so go to the next character/phase
			RunPhase();
		}

		return true;
	}

	/// <summary>
	/// Executes the current combat phase.
	/// </summary>
	public void RunPhase()
	{
		Debug.Log("Combat RunPhase " + mCombatPhase);

		// run combat phases that need to be executed by each character/denizen at a time
		switch (mCombatPhase)
		{
			case eCombatPhase.Lure:
				// characters and controlled denizens can do these
				++mCurrentCombatantIndex;
				while (mCurrentCombatantIndex < mCombatants.Count)
				{
					MRControllable combatant = CurrentCombatant;
					if (combatant is MRCharacter)
						return;
					else if (((MRDenizen)combatant).IsControlled)
						return;
					++mCurrentCombatantIndex;
				}
				break;
			case eCombatPhase.SelectTarget:
				// characters and controlled denizens can do these
				mTargetingSpell = null;
				++mCurrentCombatantIndex;
				if (mCurrentCombatantIndex == 0)
				{
					FlipNativeHorses();
					AssignUncontrolledTargets();
				}
				while (mCurrentCombatantIndex < mCombatants.Count)
				{
					MRControllable combatant = CurrentCombatant;
					if (combatant is MRCharacter)
					{
						// if the character is casting a spell, targeting works differently
						if (((MRCharacter)combatant).CastingSpell)
							AssignSpellTargets();
						return;
					}
					else if (((MRDenizen)combatant).IsControlled)
						return;
					++mCurrentCombatantIndex;
				}
				break;
			case eCombatPhase.TakeAction:
				// characters can do these
				++mCurrentCombatantIndex;
				while (mCurrentCombatantIndex < mCombatants.Count)
				{
					MRControllable combatant = CurrentCombatant;
					if (combatant is MRCharacter)
					{
						ShowSelectCombatAction();
						return;
					}
					++mCurrentCombatantIndex;
				}
				break;
			case eCombatPhase.ActivateSpells:
				ActivateNextSpell();
				return;
			case eCombatPhase.SelectAttackAndManeuver:
				// characters can do these
				++mCurrentCombatantIndex;
				while (mCurrentCombatantIndex < mCombatants.Count)
				{
					MRControllable combatant = CurrentCombatant;
					if (combatant is MRCharacter)
					{
						// if the chararacter cast a spell, skip them
						if (!((MRCharacter)combatant).CastingSpell)
							return;
					}
					++mCurrentCombatantIndex;
				}
				break;
			case eCombatPhase.FatigueChits:
				// characters can do these
				++mCurrentCombatantIndex;
				while (mCurrentCombatantIndex < mCombatants.Count)
				{
					MRControllable combatant = CurrentCombatant;
					if (combatant is MRCharacter)
					{
						int fatigueCount = ((MRCharacter)combatant).GetAsterisksUsed();
						MRActionChit.eType fatigueType = ((MRCharacter)combatant).GetAsterisksTypeForFatigue();
						--fatigueCount;
						int woundCount = combatant.CombatSheet.CharacterData.pendingWounds;
						combatant.CombatSheet.CharacterData.pendingWounds = 0;
						if (fatigueCount > 0 || woundCount > 0)
						{
							mRoundsWithNothing = -1;
							if (fatigueCount > 0)
								((MRCharacter)combatant).SetFatigueBalance(fatigueCount, fatigueType, MRGame.eStrength.Any);
							if (woundCount > 0)
								((MRCharacter)combatant).SetWoundBalance(woundCount);
							return;
						}
					}
					++mCurrentCombatantIndex;
				}
				break;
			case eCombatPhase.ResolveAttacks:
				if (++mCurrentCombatantIndex < mCombatants.Count)
				{
					ResolveNextAttack();
					return;
				}
				break;
			case eCombatPhase.Disengage:
				if (++mRoundsWithNothing < 2)
					mCombatPhase = eCombatPhase.PreStartRound;
				break;
			default:
				break;
		}

		EndPhase();
	}

	/// <summary>
	/// End the current combat phase.
	/// </summary>
	public void EndPhase()
	{
		Debug.Log("Combat EndPhase " + mCombatPhase);

		// update to the next phase
		mCurrentCombatantIndex = -1;
		mCombatPhase++;
		switch (mCombatPhase)
		{
			case eCombatPhase.StartRound:
				StartRound();
				break;
			case eCombatPhase.Lure:
				RunPhase();
				break;
			case eCombatPhase.RandomAssignment:
				AssignEnemies();
				break;
			case eCombatPhase.Deployment:
				break;
			case eCombatPhase.TakeAction:
				RunPhase();
				break;
			case eCombatPhase.SelectTarget:
				RunPhase();
				break;
			case eCombatPhase.ActivateSpells:
				SortSpells();
				RunPhase();
				break;
			case eCombatPhase.SelectAttackAndManeuver:
				AssignUnassignedTargets();
				RunPhase();
				break;
			case eCombatPhase.RandomizeAttacks:
				RandomizeEnemies();
				break;
			case eCombatPhase.ResolveAttacks:
				StartAttackResolution();
				break;
			case eCombatPhase.FatigueChits:
				// loop back to the pre update phase
				RunPhase();
				break;
			case eCombatPhase.Disengage:
				Disengage();
				break;
			case eCombatPhase.CombatDone:
				EndCombat();
				break;
		}
	}

	/// <summary>
	/// Does inititalization for the start of combat.
	/// </summary>
	private void StartCombat()
	{
		mRound = 0;
		mRoundsWithNothing = 0;
		mAllowEndCombat = true;
		MRGame.TheGame.CombatSheet.Combat = this;
		mCombatPhase = eCombatPhase.StartRound;
		mCurrentCombatantIndex = 0;
		List<MRIGamePiece> toRemove = new List<MRIGamePiece>();
		foreach (MRIGamePiece piece in mClearing.Pieces.Pieces)
		{
			if (piece is MRControllable)
			{
				MRControllable controllable = (MRControllable)piece;
				if (controllable.IsDead)
					continue;
				toRemove.Add(controllable);
				mCombatants.Add(controllable);
				if (controllable is MRCharacter)
				{
					mFriends.Add(controllable);
					// need a combat sheet per character (minimum)
					MRCharacter character = (MRCharacter)controllable;
					MRCombatSheetData sheetData = new MRCombatSheetData();
					sheetData.SheetOwner = character;
					sheetData.CharacterData = new MRCombatSheetData.MRCharacterCombatData();
					character.CombatSheet = sheetData;
					foreach (MRIGamePiece item in character.ActiveItems)
					{
						if (item is MRWeapon)
						{
							sheetData.CharacterData.weapon = (MRWeapon)item;
						}
						else if (item is MRArmor)
						{
							MRArmor armor = (MRArmor)item;
							switch (armor.Type)
							{
								case MRArmor.eType.Breastplate:
									sheetData.CharacterData.breastplate = armor;
									break;
								case MRArmor.eType.Full:
									sheetData.CharacterData.fullArmor = armor;
									break;
								case MRArmor.eType.Helmet:
									sheetData.CharacterData.helmet = armor;
									break;
								case MRArmor.eType.Shield:
									sheetData.CharacterData.shield = armor;
									sheetData.CharacterData.shieldType = eAttackType.Thrust;
									break;
								default:
									break;
							}
						}
						else if (item is MRHorse)
						{
						}
					}
					mCombatSheets.Add(sheetData);
				}
				else if (controllable is MRNative)
				{
					// todo: check meeting table
					// for now, natives are always non-hostile
				}
				else if (controllable is MRDenizen)
				{
					// if the denizen is hostile, put it in the enemies list
					// todo: determine if denizen is hostile
					mEnemies.Add((MRDenizen)controllable);
				}
			}
		}
		foreach (MRIGamePiece piece in toRemove)
		{
			mCombatStack.AddPieceToBottom(piece);
		}
		StartRound();
	}

	/// <summary>
	/// Cleans up at the end of combat.
	/// </summary>
	private void EndCombat()
	{
//		MRGame.TheGame.CombatSheet.HideCombatStack();
		MRGame.TheGame.CombatSheet.Combat = null;
		MRGame.TheGame.CombatSheet.CombatData = null;
		mCurrentSheetIndex = 0;
		if (mClearing != null)
		{
			// put the combatants back in their clearing
			foreach (MRControllable combatant in mCombatants)
			{
				combatant.CombatSheet = null;
				combatant.CombatTarget = null;
				combatant.Lurer = null;
				combatant.Luring = null;
				if (combatant is MRDenizen)
				{
					((MRDenizen)combatant).Side = MRDenizen.eSide.Light;
				}
				else
				{
					((MRCharacter)combatant).CastingSpell = false;
					((MRCharacter)combatant).PositionAttentionChit(null, Vector3.zero);
					((MRCharacter)combatant).ClearCombatChits();
				}
				mClearing.AddPieceToBottom(combatant);
			}
			mClearing.Pieces.SortBySize();
		}
		mCasters.Clear();
		mCombatants.Clear();
		mCombatSheets.Clear();
		mFriends.Clear();
		mEnemies.Clear();
	}

	/// <summary>
	/// Removes a character from combat.
	/// </summary>
	/// <param name="characrter">The character.</param>
	private void RemoveCharacterFromCombat(MRCharacter character)
	{
		character.CombatTarget = null;
		character.Lurer = null;
		character.Luring = null;
		List<MRIControllable> attackers = new List<MRIControllable>(character.Attackers);
		foreach (MRIControllable attacker in attackers)
		{
			attacker.CombatTarget = null;
		}
		if (character.CombatSheet != null)
			mCombatSheets.Remove(character.CombatSheet);
		character.CombatSheet = null;
		character.ClearCombatChits();
		mCombatants.Remove(character);
		mFriends.Remove(character);
		character.PositionAttentionChit(null, Vector3.zero);
		if (mCombatSheets.Count == 0)
			CombatPhase = eCombatPhase.CombatDone;
	}

	/// <summary>
	/// Does initialization for the beginning of a round of combat.
	/// </summary>
	private void StartRound()
	{
		++mRound;

		// clear targets
		mCasters.Clear();
		foreach (MRControllable combatant in mCombatants)
		{
			// clear all targets except for red-side tremendous monsters and uncontrolled denizens on a character sheet
			if (combatant is MRDenizen)
			{
				if (combatant.CombatTarget is MRCharacter && !((MRDenizen)combatant).IsControlled)
					continue;
				if (combatant.IsRedSideMonster)
				{
					mRoundsWithNothing = -1;
					continue;
				}
			}
			else
			{
				combatant.CombatSheet.CharacterData.pendingWounds = 0;
				((MRCharacter)combatant).CastingSpell = false;
				combatant.CombatSheet.CharacterData.spell = null;
				combatant.CombatSheet.CharacterData.spellColor = null;
				combatant.CombatSheet.CharacterData.spellMagic = null;
				combatant.CombatSheet.CharacterData.spellTargets.Clear();
			}
			combatant.CombatTarget = null;
			combatant.Lurer = null;
			combatant.Luring = null;
		}
		// remove combat sheets not belonging to players
		for (int i = mCombatSheets.Count - 1; i >= 0; --i)
		{
			if (!(mCombatSheets[i].SheetOwner is MRCharacter))
			{
				foreach (MRControllable combatant in mCombatants)
				{
					if (combatant is MRDenizen && combatant.CombatSheet == mCombatSheets[i])
						mCombatStack.AddPieceToBottom(combatant);
				}
				mCombatSheets[i].RemoveCombatants();
				mCombatSheets.RemoveAt(i);
			}
			else
			{
				// remove all defenders with no targets
				for (int j = mCombatSheets[i].Defenders.Count - 1; j >= 0; --j)
				{
					MRIControllable defender = mCombatSheets[i].Defenders[j].defender;
					if (defender.CombatTarget == null)
					{
						mCombatStack.AddPieceToBottom(defender);
						mCombatSheets[i].RemoveCombatant(defender);
					}
				}
			}
		}
		// create new combat sheets for red-side tremendous monsters attacking denizens
		foreach (MRControllable combatant in mCombatants)
		{
			if (combatant.IsRedSideMonster && combatant.CombatTarget is MRDenizen)
			{
				CreateCombatSheet(combatant, (MRControllable)combatant.CombatTarget, combatant);
				// the denizen being attacked must target the red-side monster
				combatant.CombatTarget.CombatTarget = combatant;
			}
		}

		if (mEnemies.Count == 0)
			mAllowEndCombat = true;
		else
			mAllowEndCombat = false;

		RunPhase();
	}

	/// <summary>
	/// Lure the specified attacker and target.
	/// </summary>
	/// <param name="attacker">Attacker doing the luring.</param>
	/// <param name="target">Target being lured.</param>
	private void Lure(MRControllable attacker, MRControllable target)
	{
		// can't lure self
		if (attacker == target)
			return;

		// can't lure characters
		if (target is MRCharacter)
			return;

		// can't lure red-side tremendous monsters
		if (target.IsRedSideMonster)
			return;

		// can't lure controlled denizens
		if (((MRDenizen)target).IsControlled)
			return;

		if (attacker is MRDenizen)
		{
			// uncontrolled denizens can't lure
			if (!((MRDenizen)attacker).IsControlled)
				return;

			// controlled denizens can only lure once
			if (attacker.Luring != null)
				return;
		}

		// can't lure denizens that have been lured by a hireling
		if (target.Lurer != null && target.Lurer is MRDenizen && ((MRDenizen)target.Lurer).IsControlled)
			return;

		attacker.Luring = target;
		target.Lurer = attacker;
		if (attacker is MRCharacter)
		{
			CreateCombatSheet(attacker, attacker, target);
			attacker.Hidden = false;
		}
		else
		{
			CreateCombatSheet(attacker, target, attacker);
			attacker.Hidden = false;
			attacker.CombatTarget = target;
			target.CombatTarget = attacker;
		}
	}

	/// <summary>
	/// Randomly assigns unassigned enemies to their targets.
	/// </summary>
	private void AssignEnemies()
	{
		foreach (MRDenizen enemy in mEnemies)
		{
			// skip head/club
			if (enemy is MRMonster && ((MRMonster)enemy).OwnedBy != null)
				continue;
			if (enemy.CombatTarget == null)
			{
				// create a list of players that are involved in this combat; the player may not actually be in the clearing
				IList<MRCharacter> players = new List<MRCharacter>();
				foreach (MRControllable friend in mFriends)
				{
					if (friend is MRCharacter)
					{
						MRCharacter character = (MRCharacter)friend;
						if (!character.Hidden)
						{
							players.Add(character);
						}
						else
						{
							// todo: check if the player has an unhidden hireling in the combat
						}
					}
					else if (!friend.Hidden)
					{
						// todo: add controlling character to the players list
					}
				}
				if (players.Count == 0)
				{
					// no eligible targets
					return;
				}
				// players do die pool rolloffs until there is one left
				while (players.Count > 1)
				{
					int highRoll = 0;
					IList<int> rolls = new List<int>();
					foreach (MRCharacter character in players)
					{
						MRDiePool diePool = character.DiePool(MRGame.eRollTypes.CombatRandomAssignment);
						diePool.RollDiceNow();
						int roll = diePool.Roll;
						rolls.Add(roll);
						highRoll = roll > highRoll ? roll : highRoll;
					}
					// remove any players who rolled low
					for (int i = rolls.Count - 1; i >= 0; --i)
					{
						if (rolls[i] < highRoll)
							players.RemoveAt(i);
					}
				}
				MRCharacter player = players[0];
				MRControllable target = null;
				if (!player.Hidden)
					target = player;
				else
				{
					// todo: choose a player's hireling
				}
				if (target != null)
				{
					enemy.CombatTarget = target;
					if (target is MRCharacter)
					{
						// enemy is defender on character sheet
						CreateCombatSheet(target, target, enemy);
					}
					else
					{
						// enemy is attacker on denizen sheet
						CreateCombatSheet(target, enemy, target);
					}
				}
			}
		}
	}

	/// <summary>
	/// Creates a combat sheet for a denizen, and places an attacker and defender on the sheet.
	/// </summary>
	/// <param name="owner">The sheet owner.</param>
	/// <param name="attacker">The attacker on the sheet.</param>
	/// <param name="defender">The defender on the sheet.</param>
	private void CreateCombatSheet(MRControllable owner, MRControllable attacker, MRControllable defender)
	{
		MRCombatSheetData sheet = null;
		foreach (MRCombatSheetData testSheet in mCombatSheets)
		{
			if (testSheet.SheetOwner == owner)
			{
				sheet = testSheet;
				break;
			}
		}
		if (sheet == null)
		{
			if (owner is MRCharacter)
			{
				// this shouldn't happen
				Debug.LogError("Combat sheet creation found player without sheet");
				return;
			}
			sheet = new MRCombatSheetData();
			sheet.SheetOwner = owner;
			mCombatSheets.Add(sheet);
		}
		if (attacker != null && attacker is MRDenizen)
		{
			sheet.AddAttacker((MRDenizen)attacker, eAttackType.Thrust);
			mCombatStack.RemovePiece(attacker);
		}
		if (defender != null && defender is MRDenizen)
		{
			if (defender == owner)
			{
				// defending on own sheet always goes to "charge and thrust" box
				sheet.AddDefender((MRDenizen)defender, eDefenseType.Charge);
				if (defender is MRMonster && ((MRMonster)defender).Owns != null)
					sheet.AddDefender(((MRMonster)defender).Owns, eDefenseType.Dodge);
			}
			else
				sheet.AddDefender((MRDenizen)defender);
			mCombatStack.RemovePiece(defender);
		}
	}

	private void FlipNativeHorses()
	{
		// todo: native horses ridden by unhired natives turn dark side up
		// todo: native horses ridden by hired natives flip
	}

	/// <summary>
	/// Assigns targets to uncontrolled denizens defending on their own sheet.
	/// </summary>
	private void AssignUncontrolledTargets()
	{
		// if the uncontrolled denizen is a defender on its own sheet, it attacks the most recent attacker on its sheet
		foreach (MRIControllable combatant in mCombatants)
		{
			if (combatant is MRDenizen)
			{
				MRDenizen denizen = (MRDenizen)combatant;
				if (denizen.CombatSheet != null && denizen.CombatSheet.SheetOwner == denizen && denizen.CombatSheet.Attackers.Count > 0)
				{
					if (denizen is MRMonster || !denizen.IsControlled)
					{
						MRIControllable target = denizen.CombatSheet.Attackers[0].attacker;
						denizen.CombatSheet.AddDefenderTarget((MRDenizen)target, eDefenseType.Charge);
					}
				}
			}
		}
	}

	/// <summary>
	/// Assign the target(s) of any spells being cast.
	/// </summary>
	private void AssignSpellTargets() 
	{
		MRCharacter character = (MRCharacter)CurrentCombatant;
		foreach (MRCombatSheetData combatData in mCombatSheets)
		{
			MRSpell spell = combatData.CharacterData.spell;	
			if (combatData.SheetOwner == character && spell != null && spell.Targets.Count > 0)
			{
				if (spell.Targets.Count > 1)
				{
					// caster need to choose a target or targets
					mTargetingSpell = spell;
				}
				else
				{
					switch (spell.Targets[0])
					{
						// caster need to choose a target or targets
						case MRGame.eSpellTarget.Artifact:
						case MRGame.eSpellTarget.CaveClearing:
						case MRGame.eSpellTarget.Character:
						case MRGame.eSpellTarget.Characters:
						case MRGame.eSpellTarget.ControlledMonster:
						case MRGame.eSpellTarget.Curse:
						case MRGame.eSpellTarget.Demon:
						case MRGame.eSpellTarget.Goblin:
						case MRGame.eSpellTarget.HiredLeader:
						case MRGame.eSpellTarget.LightCharacter:
						case MRGame.eSpellTarget.Monster:
						case MRGame.eSpellTarget.Monsters:
						case MRGame.eSpellTarget.Native:
						case MRGame.eSpellTarget.Natives:
						case MRGame.eSpellTarget.NativeGroup:
						case MRGame.eSpellTarget.Octopus:
						case MRGame.eSpellTarget.Ogre:
						case MRGame.eSpellTarget.SoundChit:
						case MRGame.eSpellTarget.Spell:
						case MRGame.eSpellTarget.SpellBook:
						case MRGame.eSpellTarget.SpellChitAny:
						case MRGame.eSpellTarget.SpellChit1:
						case MRGame.eSpellTarget.SpellChit2:
						case MRGame.eSpellTarget.SpellChit3:
						case MRGame.eSpellTarget.SpellChit4:
						case MRGame.eSpellTarget.SpellChit5:
						case MRGame.eSpellTarget.SpellChit6:
						case MRGame.eSpellTarget.SpellChit7:
						case MRGame.eSpellTarget.SpellChit8:
						case MRGame.eSpellTarget.Spider:
						case MRGame.eSpellTarget.Weapon:
						case MRGame.eSpellTarget.WingedDemon:
							mTargetingSpell = spell;
							break;
						// auto targets, no choice needed
						case MRGame.eSpellTarget.Bats:
						case MRGame.eSpellTarget.Clearing:
						case MRGame.eSpellTarget.Giants:
						case MRGame.eSpellTarget.Goblins:
						case MRGame.eSpellTarget.Hex:
						case MRGame.eSpellTarget.Ogres:
							RunPhase();
							break;
						case MRGame.eSpellTarget.None:
						default:
							Debug.LogError("Unhandled spell target type " + spell.Targets[0] + " for spell " + spell.Name);
							RunPhase();
							break;
					}
				}
				return;
			}
		}
		Debug.LogError("AssignSpellTargets called with no valid spell");
		RunPhase();
	}

	/// <summary>
	/// Puts unassigned denizens that are being attacked on their own combat sheet.
	/// </summary>
	private void AssignUnassignedTargets()
	{
		foreach (MRIControllable combatant in mCombatants)
		{
			if (combatant is MRDenizen)
			{
				MRDenizen denizen = (MRDenizen)combatant;
				if (denizen.CombatSheet == null && denizen.Attackers.Count > 0)
				{
					CreateCombatSheet(denizen, null, denizen);
				}
			}
		}
	}

	/// <summary>
	/// Determines what combat action(s) the current charcter can take and shows the select action dialog.
	/// If the character can't take any valid actions, advances to the next character.
	/// </summary>
	private void ShowSelectCombatAction()
	{
		bool canActivateWeapon = false;
		bool canRunAway = false;
		bool canCastSpell = false;

		// test what actions the character can do
		if (CurrentCombatant != null && CurrentCombatant is MRCharacter)
		{
			MRCharacter character = (MRCharacter)CurrentCombatant;

			// get the fastest opponent on the sheet
			int fastestTime = FastestOpponentTime(character);

			// test against the character's fight chits
			if (character.ActiveWeapon != null)
			{
				MRSelectChitEvent.MRSelectChitFilter filter = new MRSelectChitEvent.MRSelectChitFilter(
					MRActionChit.eAction.ActivateWeapon, 
					character.ActiveWeapon.BaseWeight, MRSelectChitEvent.eCompare.GreaterThanEqualTo,
					fastestTime, MRSelectChitEvent.eCompare.LessThan);
				foreach (MRActionChit chit in character.ActiveChits)
				{
					if (filter.IsValidSelectChit(chit))
					{
						canActivateWeapon = true;
						break;
					}
				}
				// todo: test gloves
			}

			// todo: test that the character can move to a valid clearing
			// test against the character's move chits
			{
				MRSelectChitEvent.MRSelectChitFilter filter = new MRSelectChitEvent.MRSelectChitFilter(
					MRActionChit.eAction.RunAway, 
					character.GetHeaviestWeight(false, false), MRSelectChitEvent.eCompare.GreaterThanEqualTo,
					fastestTime, MRSelectChitEvent.eCompare.LessThan);
				foreach (MRActionChit chit in character.ActiveChits)
				{
					if (filter.IsValidSelectChit(chit))
					{
						canRunAway = true;
						break;
					}
				}
				// todo: test horse and items
			}

			// see if the character can cast any spells
			mSelectableSpells.Clear();
			foreach (var spell in character.LearnedSpells)
			{
				if (character.CanCastSpell(spell, fastestTime))
				{
					canCastSpell = true;
					mSelectableSpells.Add(spell);
				}
			}
		}

		if (canActivateWeapon || canRunAway || canCastSpell)
		{
			MRMainUI.TheUI.DisplayCombatActionDialog(canActivateWeapon, canRunAway, canCastSpell, OnCombatActionSelected);
		}
		else
		{
			RunPhase();
		}
	}

	/// <summary>
	/// Called when the current combatant tries to run away.
	/// </summary>
	private void RunAway()
	{
		MRCharacter character = (MRCharacter)CurrentCombatant;

		mRunAwayChit = null;
		int fastestTime = FastestOpponentTime(character);
		MRGame.eStrength heaviest = character.GetHeaviestWeight(false, false);

		// todo: test horse, boots, and magic carpet - if they can be used then no chit needs to be selected

		// select what chit to use for running
		MRGame.TheGame.AddUpdateEvent(new MRSelectChitEvent(character, MRActionChit.eAction.RunAway,
		                                                    heaviest, MRSelectChitEvent.eCompare.GreaterThanEqualTo,
		                                                    fastestTime, MRSelectChitEvent.eCompare.LessThan,
		                                                    OnRunChitSeleted));
	}

	/// <summary>
	/// Called when the current combatant tries to activate or unactivate their weapon.
	/// </summary>
	private void FlipWeapon()
	{
		MRCharacter character = (MRCharacter)CurrentCombatant;
		
		int fastestTime = FastestOpponentTime(character);
		MRGame.eStrength heaviest = character.ActiveWeapon.BaseWeight;
		
		// todo: test gloves - if they can be used then no chit needs to be selected
		
		// select what chit to use for activating the weapon
		MRGame.TheGame.AddUpdateEvent(new MRSelectChitEvent(character, MRActionChit.eAction.ActivateWeapon,
		                                                    heaviest, MRSelectChitEvent.eCompare.GreaterThanEqualTo,
		                                                    fastestTime, MRSelectChitEvent.eCompare.LessThan,
		                                                    OnFlipWeaponChitSeleted));
	}

	/// <summary>
	/// Called when the current combatant wants to cast a spell.
	/// </summary>
	private void SelectSpell()
	{
		if (mSelectableSpells.Count == 0)
		{
			RunPhase();
			return;
		}
		MRCharacter character = (MRCharacter)CurrentCombatant;
		int fastestTime = FastestOpponentTime(character);
		MRGame.TheGame.AddUpdateEvent(new MRSelectSpellEvent(character, fastestTime, mSelectableSpells, OnSpellSelected));
	}

	/// <summary>
	/// Makes the current combatant leave combat and run away to a given road segment.
	/// </summary>
	/// <param name="road">Road.</param>
	private void RunAway(MRRoad road)
	{
		MRCharacter character = (MRCharacter)CurrentCombatant;
		if (mRunAwayChit != null && mRunAwayChit.BaseAsterisks > 1)
		{
			character.SetFatigueBalance(mRunAwayChit.BaseAsterisks - 1, MRActionChit.eType.Move, MRGame.eStrength.Any);
		}

		// remove the character from combat and put them on the road segment
		RemoveCharacterFromCombat(character);
		character.Location = road;
	}

	/// <summary>
	/// Called when a chit is selected to run away.
	/// </summary>
	/// <param name="chit">Chit.</param>
	private void OnRunChitSeleted(MRActionChit chit)
	{
		MRCharacter character = (MRCharacter)CurrentCombatant;

		// double check the chit is valid
		if (character.IsValidSelectChit(chit))
		{
			character.SelectChitData = null;
			chit.UsedThisRound = true;
			mRunAwayChit = chit;
			MRRoad lastRoad = null;
			MRClearing lastClearing = character.LastClearingEntered;
			if (lastClearing != null)
				lastRoad = mClearing.RoadTo(lastClearing);
			if (lastRoad == null)
			{
				// need to select a clearing to move to
				MRGame.TheGame.AddUpdateEvent(new MRSelectClearingEvent(mClearing, OnRunClearingSelected));
			}
			else
			{
				// todo: verify can move to road
				RunAway(lastRoad);
			}
		}
		else
		{
			// go back and ask for an encounter selection
			character.SelectChitData = null;
			--mCurrentCombatantIndex;
			RunPhase();
		}
	}

	/// <summary>
	/// Called when a chit is selected to flip a weapon.
	/// </summary>
	/// <param name="chit">Chit.</param>
	private void OnFlipWeaponChitSeleted(MRActionChit chit)
	{
		MRCharacter character = (MRCharacter)CurrentCombatant;
		
		// double check the chit is valid
		if (character.IsValidSelectChit(chit) && character.ActiveWeapon != null)
		{
			chit.UsedThisRound = true;
			character.ActiveWeapon.Alerted = !character.ActiveWeapon.Alerted;
		}
		else
		{
			// go back and ask for an encounter selection
			--mCurrentCombatantIndex;
		}
		character.SelectChitData = null;
		RunPhase();
	}

	/// <summary>
	/// Called when a clearing is selected to run away to.
	/// </summary>
	/// <param name="clearing">Clearing.</param>
	private void OnRunClearingSelected(MRClearing clearing)
	{
		MRRoad road = mClearing.RoadTo(clearing);
		if (road != null)
			RunAway(road);
		else
		{
			// go back and ask for an encounter selection
			--mCurrentCombatantIndex;
			RunPhase();
		}
	}

	/// <summary>
	/// Called when the user has selected what spell they want to cast.
	/// </summary>
	/// <param name="spell">Spell.</param>
	private void OnSpellSelected(MRSpell spell)
	{
		if (spell == null || !mSelectableSpells.Contains(spell))
		{
			RunPhase();
			return;
		}
		Debug.Log("Selected spell " + spell.Name);
		MRCharacter character = (MRCharacter)CurrentCombatant;
		foreach (MRCombatSheetData combatData in mCombatSheets)
		{
			if (combatData.SheetOwner == character)
			{
				combatData.CharacterData.spell = spell;

				// select what chit to use for casting the spell
				// todo: test if spell came from artifact supplying magic
				int fastestTime = FastestOpponentTime(character);
				MRGame.TheGame.AddUpdateEvent(new MRSelectChitEvent(character, MRActionChit.eAction.CastSpell,
					new int[] {spell.CurrentMagicType},
					fastestTime, MRSelectChitEvent.eCompare.LessThanEqualTo,
					false,
					OnMagicChitSelected));
				break;
			}
		}
	}

	/// <summary>
	/// Called when the user has selected what chit they want to use to cast a spell.
	/// </summary>
	/// <param name="chit">Chit selected.</param>
	private void OnMagicChitSelected(MRActionChit chit)
	{
		Debug.Log("Selected magic chit " + chit.Name);

		MRCharacter character = (MRCharacter)CurrentCombatant;
		HashSet<MRGame.eMagicColor> availableColors = new HashSet<MRGame.eMagicColor>(mClearing.MagicSupplied);

		foreach (MRCombatSheetData combatData in mCombatSheets)
		{
			if (combatData.SheetOwner == character && combatData.CharacterData.spell != null)
			{
				chit.UsedThisRound = true;
				combatData.CharacterData.spellMagic = chit as MRMagicChit;
				character.CastingSpell = true;

				if (!availableColors.Contains(combatData.CharacterData.spell.Color))
				{
					MRGame.TheGame.AddUpdateEvent(new MRSelectChitEvent(character, MRActionChit.eAction.SupplyColor, 
						new MRGame.eMagicColor[] { combatData.CharacterData.spell.Color }, true, OnMagicColorSelected));
					return;	
				}
				break;
			}
		}

		RunPhase();
	}

	/// <summary>
	/// Called when the user has selected what chit they want to use to supply the color for a spell.
	/// </summary>
	/// <param name="chit">Chit selected.</param>
	private void OnMagicColorSelected(MRActionChit chit)
	{
		Debug.Log("Selected color chit " + chit.Name);

		MRCharacter character = (MRCharacter)CurrentCombatant;
		foreach (MRCombatSheetData combatData in mCombatSheets)
		{
			if (combatData.SheetOwner == character && combatData.CharacterData.spell != null)
			{
				combatData.CharacterData.spellColor = chit as MRMagicChit;
				// the color chit disenchants and fatigues as soon as it is selected
				// note the magic chit used to power the spell fatigues after the spell activates
				character.ForceFatigueChit(chit);
			}
		}

		RunPhase();
	}

	/// <summary>
	/// Sorts all spells being cast into their casting order.
	/// </summary>
	private void SortSpells()
	{
		mCasters.Clear();
		foreach (MRCombatSheetData combatData in mCombatSheets)
		{
			if (combatData.CharacterData.spell != null)
			{
				mCasters.Add(combatData);
			}
		}
		if (mCasters.Count > 1)
		{
			mCasters.Sort(delegate(MRCombatSheetData x, MRCombatSheetData y) {
				return x.CharacterData.spellMagic.CurrentTime - y.CharacterData.spellMagic.CurrentTime;
			});
		}
	}

	/// <summary>
	/// Activates the next spell to be cast.
	/// </summary>
	private void ActivateNextSpell()
	{
		if (mCasters.Count == 0)
		{
			EndPhase();
			return;
		}
		MRCombatSheetData casterData = mCasters[0];
		mCasters.RemoveAt(0);

		MRCharacter caster = (MRCharacter)casterData.SheetOwner;
		if (casterData.CharacterData.spell != null)
		{
			MRMainUI.TheUI.DisplayMessageDialog(caster.Name.DisplayName() + " casts " + 
				casterData.CharacterData.spell.Name.DisplayName(), "", delegate (int butonId)
				{
					// if the target is another caster, this spell will cancel the target's spell if it is slower
					foreach (var target in casterData.CharacterData.spellTargets)
					{
						if (target is MRCharacter && target != caster)
						{
							foreach (var otherCasterData in mCasters)
							{
								MRCharacter otherCaster = (MRCharacter)otherCasterData.SheetOwner;
								if (otherCaster == target)
								{
									if (otherCasterData.CharacterData.spellMagic.CurrentTime > casterData.CharacterData.spellMagic.CurrentTime)
									{
										otherCaster.CastingSpell = false;
										otherCasterData.CharacterData.spell = null;
										otherCaster.ForceFatigueChit(otherCasterData.CharacterData.spellMagic);
									}
									break;
								}
							}
						}
					}

					casterData.CharacterData.spellMagic.UsedThisRound = false;
					casterData.CharacterData.spell.Activate(caster, 
						casterData.CharacterData.spellMagic,
						casterData.CharacterData.spellColor,
						casterData.CharacterData.spellTargets);

					if (casterData.CharacterData.spell.Duration == MRGame.eSpellDuration.Instant)
					{
						caster.ForceFatigueChit(casterData.CharacterData.spellMagic);
					}

					RunPhase();
				});
		}
	}

	/// <summary>
	/// Sets the attack chit for a given attacker.
	/// </summary>
	/// <returns><c>true</c>, if attack was set, <c>false</c> otherwise.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="chit">Chit.</param>
	/// <param name="attackType">Attack type.</param>
	public bool SetAttack(MRCharacter attacker, MRFightChit chit, eAttackType attackType)
	{
		if (chit != null && (chit.UsedThisRound || !attacker.ActiveChits.Contains(chit)))
			return false;

		bool success = false;
		foreach (MRCombatSheetData combatData in mCombatSheets)
		{
			if (combatData.SheetOwner == attacker)
			{
				if (chit != null)
				{
					if (attackType == eAttackType.None)
						attackType = mLastSelectedAttackType != eAttackType.None ? mLastSelectedAttackType : eAttackType.Thrust;

					// make sure the selected chit is valid
					if (attacker.IsValidAttack(chit))
					{
						if (combatData.CharacterData.attackChit != null)
							combatData.CharacterData.attackChit.UsedThisRound = false;
						chit.UsedThisRound = true;
						combatData.CharacterData.weapon = attacker.ActiveWeapon;
						combatData.CharacterData.attackChit = chit;
						combatData.CharacterData.attackType = attackType;
						chit.Selectable = false;
						success = true;
					}
				}
				else
				{
					if (combatData.CharacterData.attackChit != null)
						combatData.CharacterData.attackChit.UsedThisRound = false;
					combatData.CharacterData.weapon = null;
					combatData.CharacterData.attackChit = null;
					combatData.CharacterData.attackType = eAttackType.None;
					success = true;
				}
				break;
			}
		}
		return success;
	}

	/// <summary>
	/// Sets the maneuver chit for a given attacker.
	/// </summary>
	/// <returns><c>true</c>, if maneuver was set, <c>false</c> otherwise.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="chit">Chit.</param>
	/// <param name="maneuverType">Maneuver type.</param>
	public bool SetManeuver(MRCharacter attacker, MRMoveChit chit, eDefenseType maneuverType)
	{
		if (chit != null && (chit.UsedThisRound || !attacker.ActiveChits.Contains(chit)))
			return false;
		
		bool success = false;
		foreach (MRCombatSheetData combatData in mCombatSheets)
		{
			if (combatData.SheetOwner == attacker)
			{
				if (chit != null)
				{
					if (maneuverType == eDefenseType.None)
						maneuverType = mLastSelectedDefenseType != eDefenseType.None ? mLastSelectedDefenseType : eDefenseType.Charge;
					// make sure the maneuver matches or exceeds the weight limit
					if (attacker.IsValidManeuver(chit))
					{
						if (combatData.CharacterData.maneuverChit != null)
							combatData.CharacterData.maneuverChit.UsedThisRound = false;
						chit.UsedThisRound = true;
						combatData.CharacterData.maneuverChit = chit;
						combatData.CharacterData.maneuverType = maneuverType;
						chit.Selectable = false;
						success = true;
					}
				}
				else
				{
					if (combatData.CharacterData.maneuverChit != null)
						combatData.CharacterData.maneuverChit.UsedThisRound = false;
					combatData.CharacterData.maneuverChit = null;
					combatData.CharacterData.maneuverType = eDefenseType.None;
					success = true;
				}
				break;
			}
		}
		return success;
	}

	/// <summary>
	/// Flips a denizen during repositioning, unless it is a tremendous monster or horse.
	/// </summary>
	/// <param name="denizen">Denizen.</param>
	private void FlipDenizen(MRDenizen denizen)
	{
		// todo: filter native horses
		if (!(denizen is MRMonster) || denizen.BaseWeight < MRGame.eStrength.Tremendous)
		{
			denizen.Flip();
		}
	}

	/// <summary>
	/// Randomizes uncontrolled denizens .
	/// </summary>
	private void RandomizeEnemies()
	{
		foreach (MRCombatSheetData combatSheet in mCombatSheets)
		{
			// change the attacker/defense direction of the combatants
			if (combatSheet.SheetOwner is MRCharacter)
			{
				if (combatSheet.Defenders.Count > 0)
				{
					MRDiePool repositionRoll = MRDiePool.NewDiePool;
					Debug.Log("Reposition defenders for character sheet");
					repositionRoll.RollDiceNow();
					IDictionary<eDefenseType, eDefenseType> repositionTable = DefenderReposition[repositionRoll.Roll];
					// randomize the defenders
					foreach (MRCombatSheetData.DefenderData defender in combatSheet.Defenders)
					{
						defender.defenseType = repositionTable[defender.defenseType];
					}
				}
			}
			else
			{
				if (combatSheet.DefenderTarget != null)
				{
					// randomize the denizen target
					MRDiePool repositionRoll = MRDiePool.NewDiePool;
					Debug.Log("Reposition defenders for denizen sheet");
					repositionRoll.RollDiceNow();
					IDictionary<eDefenseType, eDefenseType> repositionTable = DefenderReposition[repositionRoll.Roll];
					combatSheet.DefenderTarget.defenseType = repositionTable[combatSheet.DefenderTarget.defenseType];
				}
				if (combatSheet.Attackers.Count > 0)
				{
					// randomize the denizen's attackers
					MRDiePool repositionRoll = MRDiePool.NewDiePool;
					Debug.Log("Reposition attackers for denizen sheet");
					repositionRoll.RollDiceNow();
					IDictionary<eAttackType, eAttackType> repositionTable = AttackerReposition[repositionRoll.Roll];
					foreach (MRCombatSheetData.AttackerData attacker in combatSheet.Attackers)
					{
						attacker.attackType = repositionTable[attacker.attackType];
					}
				}
				if (!((MRDenizen)combatSheet.SheetOwner).IsControlled)
				{
					// randomize the defender
					MRDiePool repositionRoll = MRDiePool.NewDiePool;
					Debug.Log("Reposition defenders for uncontrolled denizen sheet");
					repositionRoll.RollDiceNow();
					IDictionary<eDefenseType, eDefenseType> repositionTable = DefenderReposition[repositionRoll.Roll];
					// randomize the defender (there should be only 1, unless the other is a head/club)
					if (combatSheet.Defenders.Count > 1)
					{
						if (combatSheet.Defenders.Count > 2 || 
							(combatSheet.Defenders[0].defender is MRMonster && combatSheet.Defenders[1].defender is MRMonster &&
							(((MRMonster)combatSheet.Defenders[0].defender).Owns == (MRMonster)combatSheet.Defenders[1].defender ||
							((MRMonster)combatSheet.Defenders[1].defender).Owns == (MRMonster)combatSheet.Defenders[0].defender)))
						{
							Debug.LogError("Combat sheet with own defender has more than 1 defender");
						}
					}
					foreach (MRCombatSheetData.DefenderData defender in combatSheet.Defenders)
					{
						defender.defenseType = repositionTable[defender.defenseType];
					}
				}
			}
			// See if denizens change tactics. Tremendous monsters (and their head/club), native horses, and hirelings on their own sheet never change tactics.
			foreach (eAttackType attack in Enum.GetValues(typeof(eAttackType)))
			{
				if (attack == eAttackType.None || combatSheet.Attackers.Count == 0)
					continue;
				bool doRoll = false;
				foreach (MRCombatSheetData.AttackerData attacker in combatSheet.Attackers)
				{
					if (attacker.attackType == attack)
					{
						doRoll = true;
						break;
					}
				}
				if (doRoll)
				{
					Debug.Log("Change tactics roll for attacker");
					MRDiePool changeTacticsRoll = MRDiePool.NewDicePool;
					changeTacticsRoll.RollDiceNow();
					if (changeTacticsRoll.Roll == 6)
					{
						foreach (MRCombatSheetData.AttackerData attacker in combatSheet.Attackers)
						{
							if (attacker.attackType == attack)
							{
								FlipDenizen(attacker.attacker);
							}
						}
					}
				}
			}
			foreach (eDefenseType defense in Enum.GetValues(typeof(eDefenseType)))
			{
				if (defense == eDefenseType.None || combatSheet.Defenders.Count == 0)
					continue;
				bool doRoll = false;
				foreach (MRCombatSheetData.DefenderData defender in combatSheet.Defenders)
				{
					if (defender.defenseType == defense)
					{
						doRoll = true;
						break;
					}
				}
				if (doRoll)
				{
					MRDiePool changeTacticsRoll = MRDiePool.NewDicePool;
					Debug.Log("Change tactics roll for defender");
					changeTacticsRoll.RollDiceNow();
					if (changeTacticsRoll.Roll == 6)
					{
						foreach (MRCombatSheetData.DefenderData defender in combatSheet.Defenders)
						{
							if (defender.defenseType == defense)
							{
								FlipDenizen(defender.defender);
							}
						}
					}
				}
			}
			if (combatSheet.DefenderTarget != null)
			{
				MRDenizen denizen = combatSheet.DefenderTarget.defender;
				MRDiePool changeTacticsRoll = MRDiePool.NewDicePool;
                Debug.Log("Change tactics roll for defender target");
				changeTacticsRoll.RollDiceNow();
				if (changeTacticsRoll.Roll == 6)
				{
					FlipDenizen(denizen);
				}
			}
		}
	}

	/// <summary>
	/// Initializes the data for resolving attacks, setting up the attack order.
	/// </summary>
	private void StartAttackResolution()
	{
		mCombatants.Sort(SortCombatants);
		mCurrentCombatantIndex = -1;
		RunPhase();
	}

	/// <summary>
	/// Resolves the next attack. Will end the ResolveAttacks phase once all 
	/// </summary>
	private void ResolveNextAttack()
	{
		MRIControllable combatant = CurrentCombatant;
		MRIControllable target = combatant.CombatTarget;

		if (target == null || 
		    (combatant is MRDenizen && ((MRDenizen)combatant).CurrentStrength == MRGame.eStrength.Negligable) ||
		    (combatant is MRCharacter && combatant.CombatSheet.CharacterData.attackChit == null))
		{
			RunPhase();
			return;
		}

		// determine if the target was hit
		eHitResult hitResult = HitResult((MRControllable)combatant, (MRControllable)target);

		if (hitResult == eHitResult.Miss)
		{
			// miss
			combatant.MissTarget(target);
			MRMainUI.TheUI.DisplayMessageDialog(combatant.Name.DisplayName() + " attacks " + 
			                                    target.Name.DisplayName() + ", misses", "", OnAttackMessageClicked);
			return;
		}

		// successful hit
		String hitName = " undercuts ";
		if (hitResult == eHitResult.Intercept)
			hitName = " intercepts ";
		if (combatant.IsRedSideMonster)
			hitName = " picks up ";
		bool targetDead = false;
		MRArmor characterArmor = null;
		MRGame.eStrength damage = FinalDamage((MRControllable)combatant, (MRControllable)target, out characterArmor, out targetDead);

		if (target is MRCharacter)
		{
			if (characterArmor != null)
			{
				if (damage >= characterArmor.BaseWeight)
				{
					// damage the armor
					mRoundsWithNothing = -1;
					if (characterArmor.State == MRArmor.eState.Undamaged && damage == characterArmor.BaseWeight)
						characterArmor.State = MRArmor.eState.Damaged;
					else if (characterArmor.State == MRArmor.eState.Damaged || damage > characterArmor.BaseWeight)
						characterArmor.State = MRArmor.eState.Destroyed;
				}
				if (damage >= MRGame.eStrength.Medium)
				{
					mRoundsWithNothing = -1;
					++target.CombatSheet.CharacterData.pendingWounds;
				}
			}
			else
			{
				if (damage >= target.CurrentVulnerability)
				{
					mRoundsWithNothing = -1;
					targetDead = true;
				}
				else if (damage > MRGame.eStrength.Negligable)
				{
					mRoundsWithNothing = -1;
					++target.CombatSheet.CharacterData.pendingWounds;
				}
			}
		}
		else
		{
			if (damage >= target.BaseWeight)
			{
				mRoundsWithNothing = -1;
				targetDead = true;
			}
		}
		if (targetDead)
		{
			target.Killers.Add(combatant);
			if (target is MRMonster)
			{
				// killing a monster also kills its head/club
				if (((MRMonster)target).Owns != null)
					((MRMonster)target).Owns.Killers.Add(combatant);
			}
		}

		combatant.HitTarget(target, targetDead);

		String message = combatant.Name.DisplayName() + hitName + target.Name.DisplayName();
		if (targetDead)
		{
			message += " killing them";
		}
		else
		{
			message += " for " + damage + " damage";
		}
		MRMainUI.TheUI.DisplayMessageDialog(message, "", OnAttackMessageClicked);

		// if we're not in a simultanious attack situation, apply the damage
		if (mCurrentCombatantIndex + 1 == mCombatants.Count || 
		    SortCombatants((MRControllable)combatant, mCombatants[mCurrentCombatantIndex + 1]) != 0)
		{
			ApplyDamageEffects();
		}
	}

	/// <summary>
	/// Determines if an attacker has hit their target.
	/// </summary>
	/// <returns>The attack result.</returns>
	/// <param name="attacker">Attacker.</param>
	/// <param name="defender">Defender.</param>
	private eHitResult HitResult(MRControllable attacker, MRControllable defender)
	{
		eHitResult result = eHitResult.Miss;

		// test for undercut
		if (attacker.CurrentAttackSpeed < defender.CurrentMoveSpeed)
			result = eHitResult.Undercut;
		else
		{
			// test for intercept
			eAttackType attack = attacker.AttackType;
			eDefenseType defense = defender.DefenseType;
			if ((attack == eAttackType.Thrust && defense == eDefenseType.Charge) ||
			    (attack == eAttackType.Swing && defense == eDefenseType.Dodge) ||
			    (attack == eAttackType.Smash && defense == eDefenseType.Duck) ||
			    (attack != eAttackType.None && defense == eDefenseType.None))
			{
				result = eHitResult.Intercept;
			}
		}

		return result;
	}

	/// <summary>
	/// Determines the final damage on a successful attack.
	/// </summary>
	/// <returns>The damage</returns>
	/// <param name="attacker">Attacker</param>
	/// <param name="defender">Defender</param>
	/// <param name="characterArmor">The character's armor that was hit</param>
	/// <param name="instantKill">Flag that the attack resulted in an instant kill.</param>
	private MRGame.eStrength FinalDamage(MRControllable attacker, MRControllable defender, out MRArmor characterArmor, out bool instantKill)
	{
		characterArmor = null;
		instantKill = false;

		// get the base damage and sharpness
		MRGame.eStrength attackStrength = attacker.CurrentStrength;
		int sharpness = attacker.CurrentSharpness;
		
		// if the attacker has a missile weapon, roll on the missile table
		int bonusDamage = 0;
		if (attacker.WeaponType == MRWeapon.eWeaponType.Missile)
		{
			MRDiePool missileRoll = attacker.DiePool(MRGame.eRollTypes.Missile);
			missileRoll.RollDiceNow();
			MRMainUI.TheUI.DisplayDieRollResult(missileRoll);
			int roll = missileRoll.Roll;
			bonusDamage = MissileTable[roll];
			
			// missile damage > tremendous = instant kill
			if ((int)attackStrength + sharpness + bonusDamage > (int)MRGame.eStrength.Tremendous)
			{
				instantKill = true;
			}
		}

		// a red-side up tremendous monster = instant kill
		if (attacker.IsRedSideMonster)
			instantKill = true;

		if (!instantKill)
		{
			if (defender is MRCharacter)
			{
				characterArmor = defender.CombatSheet.CharacterArmorForAttack(attacker.AttackType);
			}
			if (sharpness > 0)
			{
				// reduce sharpness for armor
				if (characterArmor != null || (defender is MRDenizen && ((MRDenizen)defender).Armored))
				{
					--sharpness;
				}
			}
		}
		
		// determine final damage
		int damageValue = (int)attackStrength + sharpness + bonusDamage;
		MRGame.eStrength damage;
		if (damageValue < (int)MRGame.eStrength.Negligable)
			damage = MRGame.eStrength.Negligable;
		else if (damageValue > (int)MRGame.eStrength.Tremendous)
			damage = MRGame.eStrength.Tremendous;
		else
			damage = (MRGame.eStrength)damageValue;

		return damage;
	}

	/// <summary>
	/// Applies pending damage effects to combatants. Does not include wounds (that happend in the FatigueChits phase).
	/// </summary>
	private void ApplyDamageEffects()
	{
		List<MRControllable> toRemove = new List<MRControllable>();
		foreach (MRControllable combatant in mCombatants)
		{
			if (combatant.IsDead)
			{
				if (combatant.KillerCount > 0)
				{
					// no spoils for club/head
					if (!(combatant is MRMonster) || ((MRMonster)combatant).OwnedBy == null)
					{
						foreach (MRIControllable killer in combatant.Killers)
						{
							killer.AwardSpoils(combatant, 1.0f / combatant.KillerCount);
						}
					}
				}
				combatant.Killers.Clear();
				toRemove.Add(combatant);
			}
			if (combatant is MRCharacter)
			{
				MRCharacter character = (MRCharacter)combatant;

				// remove any destroyed armor
				MRArmor destroyedArmor = null;
				if (combatant.CombatSheet.CharacterData.shield != null && 
				    combatant.CombatSheet.CharacterData.shield.State == MRArmor.eState.Destroyed)
				{
					destroyedArmor = combatant.CombatSheet.CharacterData.shield;
					combatant.CombatSheet.CharacterData.shield = null;
				}
				else if (combatant.CombatSheet.CharacterData.breastplate != null && 
				         combatant.CombatSheet.CharacterData.breastplate.State == MRArmor.eState.Destroyed)
				{
					destroyedArmor = combatant.CombatSheet.CharacterData.breastplate;
					combatant.CombatSheet.CharacterData.breastplate = null;
				}
				else if (combatant.CombatSheet.CharacterData.helmet != null && 
				         combatant.CombatSheet.CharacterData.helmet.State == MRArmor.eState.Destroyed)
				{
					destroyedArmor = combatant.CombatSheet.CharacterData.helmet;
					combatant.CombatSheet.CharacterData.helmet = null;
				}
				else if (combatant.CombatSheet.CharacterData.fullArmor != null && 
				         combatant.CombatSheet.CharacterData.fullArmor.State == MRArmor.eState.Destroyed)
				{
					destroyedArmor = combatant.CombatSheet.CharacterData.fullArmor;
					combatant.CombatSheet.CharacterData.fullArmor = null;
				}
				if (destroyedArmor != null)
				{
					character.BaseGold += destroyedArmor.CurrentPrice;
					character.RemoveItem(destroyedArmor);
					if (destroyedArmor.CurrentPrice > 0)
					{
						// treasure-type armor, remove from game
						MRGame.TheGame.TreasureChart.DestroyedItems.AddPieceToBottom(destroyedArmor);
					}
					else
					{
						// normal armor, repair and give back to native group
						destroyedArmor.State = MRArmor.eState.Undamaged;
						MRGame.TheGame.TreasureChart.ReturnArmorToNatives(destroyedArmor);
					}
				}
			}
		}

		// remove dead combatants
		foreach (MRControllable combatant in toRemove)
		{
			mCombatants.Remove(combatant);
			mFriends.Remove(combatant);
			if (combatant.CombatTarget != null)
				combatant.CombatTarget.RemoveAttacker(combatant);
			combatant.Lurer = null;
			combatant.Luring = null;
			if (combatant is MRDenizen)
			{
				MRDenizen denizen = (MRDenizen)combatant;
				denizen.Side = MRDenizen.eSide.Light;
				mEnemies.Remove(denizen);
				MRGame.TheGame.MonsterChart.AddDeadDenizen(denizen);
			}
			else
			{
				MRCharacter character = (MRCharacter)combatant;
				MRGame.TheGame.RemoveCharacter(character);
				character.PositionAttentionChit(null, Vector3.zero);
				mClearing.AddPieceToBottom(character);
			}

			if (combatant.CombatSheet != null)
			{
				// clean up combat sheets
				if (combatant is MRDenizen)
				{
					combatant.CombatSheet.RemoveCombatant(combatant);
				}
			}
			// untarget any combatant targeting the dead combatant
			foreach (MRControllable otherCombatant in mCombatants)
			{
				if (otherCombatant.CombatTarget == combatant)
				{
					otherCombatant.CombatTarget = null;
				}
			}
		}
	}

	private void Disengage()
	{
		// double check for character deaths due to wounds
		ApplyDamageEffects();

		// clean up character mats
		foreach (MRCombatSheetData combatSheet in mCombatSheets)
		{
			if (combatSheet.CharacterData != null)
			{
				if (combatSheet.CharacterData.attackChit != null)
				{
					if (combatSheet.CharacterData.attackChit.Stack != null)
						combatSheet.CharacterData.attackChit.Stack.RemovePiece(combatSheet.CharacterData.attackChit);
					combatSheet.CharacterData.attackChit = null;
				}
				if (combatSheet.CharacterData.maneuverChit != null)
				{
					if (combatSheet.CharacterData.maneuverChit.Stack != null)
						combatSheet.CharacterData.maneuverChit.Stack.RemovePiece(combatSheet.CharacterData.maneuverChit);
					combatSheet.CharacterData.maneuverChit = null;
				}
				if (combatSheet.CharacterData.weapon != null)
				{
					if (combatSheet.CharacterData.weapon.Stack != null)
						combatSheet.CharacterData.weapon.Stack.RemovePiece(combatSheet.CharacterData.weapon);
					combatSheet.CharacterData.weapon = null;
				}
				combatSheet.CharacterData.attackType = eAttackType.None;
				combatSheet.CharacterData.maneuverType = eDefenseType.None;
			}
		}
		foreach (MRControllable combatant in mCombatants)
		{
			if (combatant is MRCharacter)
				((MRCharacter)combatant).ClearCombatChits();
		}
	}

	private void OnCombatActionSelected(int buttonId)
	{
		switch (buttonId)
		{
			case (int)MRMainUI.eCombatActionButton.FlipWeapon:
				FlipWeapon();
				break;
			case (int)MRMainUI.eCombatActionButton.RunAway:
				RunAway();
				break;
			case (int)MRMainUI.eCombatActionButton.CastSpell:
				SelectSpell();
				break;
			default:
				RunPhase();
				break;
		}
	}

	private void OnAttackMessageClicked(int butonId)
	{
		RunPhase();
	}

	/// <summary>
	/// Returns the fastest opponent time for a character.
	/// </summary>
	/// <returns>The opponent time.</returns>
	/// <param name="character">Character.</param>
	private int FastestOpponentTime(MRCharacter character)
	{
		// get the fastest denizen on the sheet
		// todo: also check any move chits from charging players
		int fastestTime = int.MaxValue;
		foreach (MRCombatSheetData.AttackerData attacker in character.CombatSheet.Attackers)
		{
			MRDenizen denizen = attacker.attacker;
			if (denizen.CurrentMoveSpeed < fastestTime)
				fastestTime = denizen.CurrentMoveSpeed;
		}
		foreach (MRCombatSheetData.DefenderData defender in character.CombatSheet.Defenders)
		{
			MRDenizen denizen = defender.defender;
			if (denizen.CurrentMoveSpeed < fastestTime)
				fastestTime = denizen.CurrentMoveSpeed;
		}
		return fastestTime;
	}

	/// <summary>
	/// Sorts two combatants, with weapon length taking precidence in the 1st round, and speed in later rounds.
	/// </summary>
	/// <returns> <0 if a is before b, >0 if b is before a, and 0 if they are tied </returns>
	/// <param name="a">One combatant</param>
	/// <param name="b">The other combatant</param>
	private int SortCombatants(MRControllable a, MRControllable b)
	{
		if (mRound == 1)
			return SortByWeaponLength(a, b);
		else
			return SortBySpeed(a, b);
	}

	/// <summary>
	/// Sorts two combatants, with weapon length taking precidence over speed.
	/// </summary>
	/// <returns> <0 if a is before b, >0 if b is before a, and 0 if they are tied </returns>
	/// <param name="a">One combatant</param>
	/// <param name="b">The other combatant</param>
	private int SortByWeaponLength(MRControllable a, MRControllable b)
	{
		int lengthCompare = CompareWeaponLengths(a, b);
		if (lengthCompare != 0)
			return lengthCompare;
		return CompareWeaponSpeeds(a, b);
	}

	/// <summary>
	/// Sorts two combatants, with speed taking precidence over weapon length.
	/// </summary>
	/// <returns> <0 if a is before b, >0 if b is before a, and 0 if they are tied </returns>
	/// <param name="a">One combatant</param>
	/// <param name="b">The other combatant</param>
	private int SortBySpeed(MRControllable a, MRControllable b)
	{
		int speedCompare = CompareWeaponSpeeds(a, b);
		if (speedCompare != 0)
			return speedCompare;
		return CompareWeaponLengths(a, b);
	}

	/// <summary>
	/// Compares the weapon lengths of two combatants, where the longer weapon goes first.
	/// </summary>
	/// <returns> < 0 if combatant a is before b, > 0 if combatant b is before a, and 0 if tied </returns>
	/// <param name="a">The 1st combatant to test.</param>
	/// <param name="b">The 2nd combatant to test.</param>
	private int CompareWeaponLengths(MRControllable a, MRControllable b)
	{
		int length_a = a.WeaponLength;
		int length_b = b.WeaponLength;
		return length_b - length_a;
	}

	/// <summary>
	/// Compares the weapon speeds of two combatants, where the faster weapon goes first.
	/// </summary>
	/// <returns> < 0 if combatant a is before b, > 0 if combatant b is before a, and 0 if tied </returns>
	/// <param name="a">The 1st combatant to test.</param>
	/// <param name="b">The 2nd combatant to test.</param>
	private int CompareWeaponSpeeds(MRControllable a, MRControllable b)
	{
		int speed_a = a.CurrentAttackSpeed;
		int speed_b = b.CurrentAttackSpeed;
		return speed_a - speed_b;
	}

	/// <summary>
	/// Called when a controllable piece has been selected.
	/// </summary>
	/// <param name="piece">The selected piece.</param>
	public void OnControllableSelected(MRIControllable selector, MRIControllable piece)
	{
		switch (mCombatPhase)
		{
			case eCombatPhase.Lure:
				Lure((MRControllable)selector, (MRControllable)piece);
				break;
			case eCombatPhase.SelectTarget:
				if (mTargetingSpell != null)
				{
					Debug.Log("OnControllableSelected testing spell target");
					if (piece is MRISpellTarget && ((MRISpellTarget)piece).IsValidTarget(mTargetingSpell))
					{
						Debug.Log("OnControllableSelected valid target");
						selector.CombatTarget = piece;
						foreach (MRCombatSheetData combatData in mCombatSheets)
						{
							if (combatData.SheetOwner == selector)
							{
								// @todo handle select multiple targets
								combatData.CharacterData.spellTargets.Clear();
								combatData.CharacterData.spellTargets.Add((MRISpellTarget)piece);
								break;
							}
						}
					}
					else
					{
						Debug.Log("OnControllableSelected invalid target");
					}
				}
				else
				{
					if (selector == piece)
						break;
					if (selector is MRCharacter)
					{
						// don't allow selecting head/club
						if (piece is MRMonster && ((MRMonster)piece).OwnedBy != null)
							break;
						if (selector.CombatTarget != piece)
							selector.CombatTarget = piece;
						else
							selector.CombatTarget = null;
					}
				}
				break;
			default:
				break;
		}
	}

	#endregion
	
	#region Members

	private int mRound;
	private int mRoundsWithNothing;
	private bool mAllowEndCombat;
	private eCombatPhase mCombatPhase;
	private MRClearing mClearing;
	private MRGamePieceStack mCombatStack;
	private int mCurrentCombatantIndex;
	private int mCurrentSheetIndex;
	private eAttackType mLastSelectedAttackType;
	private eDefenseType mLastSelectedDefenseType;
	private MRActionChit mRunAwayChit;
	private List<MRCombatSheetData> mCombatSheets = new List<MRCombatSheetData>();
	private List<MRControllable> mCombatants = new List<MRControllable>();
	private List<MRControllable> mFriends = new List<MRControllable>();
	private List<MRDenizen> mEnemies = new List<MRDenizen>();
	private List<MRSpell> mSelectableSpells = new List<MRSpell>();
	private List<MRCombatSheetData> mCasters = new List<MRCombatSheetData>();
	private MRSpell mTargetingSpell;

	#endregion
}

}