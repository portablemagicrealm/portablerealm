//
// MRCombatSheet.cs
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

public class MRCombatSheet : MonoBehaviour
{
	#region Constants

	private static readonly Dictionary<MRCombatManager.eCombatPhase, string> PhaseNames = new Dictionary<MRCombatManager.eCombatPhase, string>
	{
		{MRCombatManager.eCombatPhase.Lure, "Lure"},
		{MRCombatManager.eCombatPhase.RandomAssignment, "Random Assignment"},
		{MRCombatManager.eCombatPhase.Deployment, "Deployment"},
		{MRCombatManager.eCombatPhase.TakeAction, "Take Action"},
		{MRCombatManager.eCombatPhase.SelectTarget, "Select Target"},
		{MRCombatManager.eCombatPhase.ActivateSpells, "Activate Spells"},
		{MRCombatManager.eCombatPhase.SelectAttackAndManeuver, "Select Attack/Maneuver"},
		{MRCombatManager.eCombatPhase.RandomizeAttacks, "Randomize"},
		{MRCombatManager.eCombatPhase.ResolveAttacks, "Resolve Attacks"},
		{MRCombatManager.eCombatPhase.FatigueChits, "Fatigue Chits"},
		{MRCombatManager.eCombatPhase.Disengage, "Disengage"},
		{MRCombatManager.eCombatPhase.CombatDone, "Combat Done"}
	};

	#endregion

	#region Properties

	public Camera CombatCamera
	{
		get{
			return mCamera;
		}
	}
	
	public bool Visible
	{
		get{
			return mCamera.enabled;
		}
		
		set{
			mCamera.enabled = value;
		}
	}

	public MRCombatSheetData CombatData
	{
		get{
			return mCombatData;
		}

		set{
			mCombatData = value;
		}
	}

	public MRCombatManager Combat
	{
		get{
			return mCombat;
		}
		
		set{
			mCombat = value;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		mAttackerAttacks = new Dictionary<MRCombatManager.eAttackType, GameObject>();
		mAttackerShields = new Dictionary<MRCombatManager.eAttackType, GameObject>();
		mAttackerManeuvers = new Dictionary<MRCombatManager.eDefenseType, GameObject>();
		mDefenderDefense = new Dictionary<string, MRCombatManager.eDefenseType>();
		mShieldPositions = new Dictionary<MRCombatManager.eAttackType, MRGamePieceStack>();
		mWeaponPositions = new Dictionary<MRCombatManager.eAttackType, MRGamePieceStack>();
		mAttackPositions = new Dictionary<MRCombatManager.eAttackType, MRGamePieceStack>();
		mManeuverPositions = new Dictionary<MRCombatManager.eDefenseType, MRGamePieceStack>();
		mDefenderPositions = new Dictionary<MRCombatManager.eDefenseType, MRGamePieceStack>();

		// get the camera
		mCamera = gameObject.GetComponentInChildren<Camera>();

		// get the owner name control
		TextMesh[] textMeshes = gameObject.GetComponentsInChildren<TextMesh>();
		foreach (TextMesh mesh in textMeshes)
		{
			if (mesh.gameObject.name == "name" && mesh.gameObject.transform.parent.gameObject.name == "ownerName")
			{
				mOwnerName = mesh;
			}
			else if (mesh.gameObject.name == "name" && mesh.gameObject.transform.parent.gameObject.name == "phaseName")
			{
				mPhaseName = mesh;
			}
		}

		// get the colliders for the attacker and defender boxes
		Collider2D[] colliders = gameObject.GetComponentsInChildren<Collider2D>();
		foreach (Collider2D collider in colliders)
		{
			MRGamePieceStack stack = collider.gameObject.GetComponentInChildren<MRGamePieceStack>();
			if (stack != null)
				stack.Layer = gameObject.layer;
			switch (collider.gameObject.name)
			{
				case "AttackThrust":
				{
					mAttackerAttacks.Add(MRCombatManager.eAttackType.Thrust, collider.gameObject);
					MRGamePieceStack[] stacks = collider.gameObject.GetComponentsInChildren<MRGamePieceStack>();
					for (int i = 0; i < stacks.Length; ++i)
					{
						stacks[i].Layer = gameObject.layer;
						if (stacks[i].transform.parent.name == "weapon")
							mWeaponPositions[MRCombatManager.eAttackType.Thrust] = stacks[i];
						else
							mAttackPositions[MRCombatManager.eAttackType.Thrust] = stacks[i];
					}
					break;
				}
				case "AttackSwing":
				{
					mAttackerAttacks.Add(MRCombatManager.eAttackType.Swing, collider.gameObject);
					MRGamePieceStack[] stacks = collider.gameObject.GetComponentsInChildren<MRGamePieceStack>();
					for (int i = 0; i < stacks.Length; ++i)
					{
						stacks[i].Layer = gameObject.layer;
						if (stacks[i].transform.parent.name == "weapon")
							mWeaponPositions[MRCombatManager.eAttackType.Swing] = stacks[i];
						else
							mAttackPositions[MRCombatManager.eAttackType.Swing] = stacks[i];
					}
					break;
				}
				case "AttackSmash":
				{
					mAttackerAttacks.Add(MRCombatManager.eAttackType.Smash, collider.gameObject);
					MRGamePieceStack[] stacks = collider.gameObject.GetComponentsInChildren<MRGamePieceStack>();
					for (int i = 0; i < stacks.Length; ++i)
					{
						stacks[i].Layer = gameObject.layer;
						if (stacks[i].transform.parent.name == "weapon")
							mWeaponPositions[MRCombatManager.eAttackType.Smash] = stacks[i];
						else
							mAttackPositions[MRCombatManager.eAttackType.Smash] = stacks[i];
					}
					break;
				}
				case "ShieldThrust":
					mAttackerShields.Add(MRCombatManager.eAttackType.Thrust, collider.gameObject);
					mShieldPositions.Add(MRCombatManager.eAttackType.Thrust, stack);
					break;
				case "ShieldSwing":
					mAttackerShields.Add(MRCombatManager.eAttackType.Swing, collider.gameObject);
					mShieldPositions.Add(MRCombatManager.eAttackType.Swing, stack);
					break;
				case "ShieldSmash":
					mAttackerShields.Add(MRCombatManager.eAttackType.Smash, collider.gameObject);
					mShieldPositions.Add(MRCombatManager.eAttackType.Smash, stack);
					break;
				case "Breastplate":
					mBreastplatePosition = stack;
					break;
				case "Helmet":
					mHelmetPosition = stack;
					break;
				case "Armor":
					mArmorPosition = stack;
					break;
				case "ManeuverCharge":
					mAttackerManeuvers.Add(MRCombatManager.eDefenseType.Charge, collider.gameObject);
					mManeuverPositions.Add (MRCombatManager.eDefenseType.Charge, stack);
					break;
				case "ManeuverDodge":
					mAttackerManeuvers.Add(MRCombatManager.eDefenseType.Dodge, collider.gameObject);
					mManeuverPositions.Add (MRCombatManager.eDefenseType.Dodge, stack);
					break;
				case "ManeuverDuck":
					mAttackerManeuvers.Add(MRCombatManager.eDefenseType.Duck, collider.gameObject);
					mManeuverPositions.Add (MRCombatManager.eDefenseType.Duck, stack);
					break;
				case "ChargeAndThrust":
					mDefenderDefense.Add(collider.gameObject.name, MRCombatManager.eDefenseType.Charge);
					stack.StackScale = 2f;
					mDefenderPositions.Add(MRCombatManager.eDefenseType.Charge, stack);
					break;
				case "DodgeAndSwing":
					mDefenderDefense.Add(collider.gameObject.name, MRCombatManager.eDefenseType.Dodge);
					stack.StackScale = 2f;
					mDefenderPositions.Add(MRCombatManager.eDefenseType.Dodge, stack);
					break;
				case "DuckAndSmash":
					mDefenderDefense.Add(collider.gameObject.name, MRCombatManager.eDefenseType.Duck);
					stack.StackScale = 2f;
					mDefenderPositions.Add(MRCombatManager.eDefenseType.Duck, stack);
					break;
				case "nextSheet":
					mSheetRightArrow = collider.gameObject;
					mEnabledArrow = ((SpriteRenderer)(mSheetRightArrow.renderer)).sprite;
					break;
				case "prevSheet":
					mSheetLeftArrow = collider.gameObject;
					mDisabledArrow = ((SpriteRenderer)(mSheetLeftArrow.renderer)).sprite;
					break;
				case "endCombat":
				{
					SpriteRenderer[] renderers = collider.gameObject.GetComponentsInChildren<SpriteRenderer>();
					foreach (SpriteRenderer renderer in renderers)
					{
						if (renderer.gameObject.name == "background")
							mEndCombatEnabled = renderer;
					}
					break;
				}
				default:
					break;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		MRCombatManager combatManger = MRGame.TheGame.CombatManager;

		if (!Visible)
			return;

		if (mCombatData == null)
			return;

		// update the phase name
		if (mPhaseName != null && MRGame.TheGame.CombatManager.CombatPhase != MRCombatManager.eCombatPhase.StartRound)
		{
			mPhaseName.text = "";
			if (mCombat.CurrentCombatant != null)
				mPhaseName.text = MRUtility.DisplayName(mCombat.CurrentCombatant.Name) + ": ";
			mPhaseName.text += PhaseNames[MRGame.TheGame.CombatManager.CombatPhase];
		}

		// update the prev and next sheet arrows
		Sprite arrow = null;
		if (mCombat.CombatSheets.Count > 1)
			arrow = mEnabledArrow;
		else
			arrow = mDisabledArrow;
		((SpriteRenderer)(mSheetLeftArrow.renderer)).sprite = arrow;
		((SpriteRenderer)(mSheetRightArrow.renderer)).sprite = arrow;

		// update the end combat button
		mEndCombatEnabled.enabled = mCombat.AllowEndCombat;

		// remove current pieces from the sheet
		ClearSheet();

		// update piece positions on the combat sheet
		if (mCombatData.SheetOwner is MRCharacter)
		{
			if (mCombatData.CharacterData == null)
			{
				Debug.LogError("Comabt sheet data for " + mCombatData.SheetOwner.Name + " has no character data");
				return;
			}
			mOwnerName.text = "Sheet- " + MRUtility.DisplayName(mCombatData.SheetOwner.Name);

			// place the character's armor pieces
			if (mCombatData.CharacterData.shield != null)
			{
				mShieldPositions[mCombatData.CharacterData.shieldType].AddPieceToTop(mCombatData.CharacterData.shield);
			}
			if (mCombatData.CharacterData.breastplate != null)
			{
				mBreastplatePosition.AddPieceToTop(mCombatData.CharacterData.breastplate);
			}
			if (mCombatData.CharacterData.helmet != null)
			{
				mHelmetPosition.AddPieceToTop(mCombatData.CharacterData.helmet);
			}
			if (mCombatData.CharacterData.fullArmor != null)
			{
				mArmorPosition.AddPieceToTop(mCombatData.CharacterData.fullArmor);
			}

			// place the character's weapon and attack chit
			if (mCombatData.CharacterData.attackType != MRCombatManager.eAttackType.None)
			{
				if (mCombatData.CharacterData.weapon != null)
				{
					mWeaponPositions[mCombatData.CharacterData.attackType].AddPieceToTop(mCombatData.CharacterData.weapon);
				}
				if (mCombatData.CharacterData.attackChit != null)
				{
					mAttackPositions[mCombatData.CharacterData.attackType].AddPieceToTop(mCombatData.CharacterData.attackChit);
				}
			}

			// place the character's maneuver chit
			if (mCombatData.CharacterData.maneuverType != MRCombatManager.eDefenseType.None)
			{
				if (mCombatData.CharacterData.maneuverChit != null)
				{
					mManeuverPositions[mCombatData.CharacterData.maneuverType].AddPieceToTop(mCombatData.CharacterData.maneuverChit);
				}
			}
		}
		else
		{
			mOwnerName.text = "Sheet- " + MRUtility.DisplayName(mCombatData.SheetOwner.Name);
		}
		foreach (MRCombatSheetData.DefenderData data in mCombatData.Defenders)
		{
			mDefenderPositions[data.defenseType].AddPieceToBottom(data.defender);
		}
		if (mCombatData.DefenderTarget != null)
		{
			mManeuverPositions[mCombatData.DefenderTarget.defenseType].AddPieceToTop(mCombatData.DefenderTarget.defender);
		}

		// test user interaction
		if (MRGame.IsDoubleTapped)
		{
			Vector3 worldTouch = mCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane));
			RaycastHit2D[] hits = Physics2D.RaycastAll(worldTouch, Vector2.zero);
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.gameObject.name == "endCombat")
				{
					if (mCombat.AllowEndCombat)
						MRGame.TheGame.CombatManager.CombatPhase = MRCombatManager.eCombatPhase.CombatDone;
					return;
				}
				if (hit.collider.gameObject.name == "endPhase")
				{
					MRGame.TheGame.CombatManager.EndPhase();
					return;
				}
				if (mCombatData.SheetOwner is MRCharacter && mCombat.CombatPhase == MRCombatManager.eCombatPhase.SelectAttackAndManeuver)
				{
					// test shield interaction
					foreach (MRCombatManager.eAttackType attackType in Enum.GetValues(typeof(MRCombatManager.eAttackType)))
					{
						if (mAttackerShields.ContainsKey(attackType) && hit.collider.gameObject == mAttackerShields[attackType])
						{
							mCombatData.CharacterData.shieldType = attackType;
							break;
						}
					}
					// test weapon interaction
					foreach (MRCombatManager.eAttackType attackType in Enum.GetValues(typeof(MRCombatManager.eAttackType)))
					{
						if (mAttackerAttacks.ContainsKey(attackType) && hit.collider.gameObject == mAttackerAttacks[attackType])
						{
							if (mCombatData.CharacterData.attackChit == null || mCombatData.CharacterData.attackType == attackType)
							{
								// select fight chit
								MRGame.TheGame.CharacterMat.Controllable = mCombatData.SheetOwner;
								MRGame.TheGame.CombatManager.LastSelectedAttackType = attackType;
								MRGame.TheGame.PushView(MRGame.eViews.SelectAttack);
							}
							else
							{
								// change attack type
								mCombatData.CharacterData.attackType = attackType;
							}
							break;
						}
					}
					// test maneuver interaction
					foreach (MRCombatManager.eDefenseType defenseType in Enum.GetValues(typeof(MRCombatManager.eDefenseType)))
					{
						if (mAttackerManeuvers.ContainsKey(defenseType) && hit.collider.gameObject == mAttackerManeuvers[defenseType])
						{
							if (mCombatData.CharacterData.maneuverChit == null || mCombatData.CharacterData.maneuverType == defenseType)
							{
								// select maneuver chit
								MRGame.TheGame.CharacterMat.Controllable = mCombatData.SheetOwner;
								MRGame.TheGame.CombatManager.LastSelectedDefenseType = defenseType;
								MRGame.TheGame.PushView(MRGame.eViews.SelectManeuver);
							}
							else
							{
								// change attack type
								mCombatData.CharacterData.maneuverType = defenseType;
							}
							break;
						}
					}
				}
			}
		}
		else if (MRGame.IsSingleTapped)
		{
			Vector3 worldTouch = mCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane));
			RaycastHit2D[] hits = Physics2D.RaycastAll(worldTouch, Vector2.zero);
			foreach (RaycastHit2D hit in hits)
			{
				if (mCombat.CombatSheets.Count > 1)
				{
					if (hit.collider.gameObject.name == "nextSheet")
					{
						++MRGame.TheGame.CombatManager.CurrentCombatantSheetIndex;
					}
					else if (hit.collider.gameObject.name == "prevSheet")
					{
						--MRGame.TheGame.CombatManager.CurrentCombatantSheetIndex;
					}
				}
			}
		}
		else if (MRGame.IsTouchHeld && !mInspectionToggle)
		{
			Vector3 worldTouch = mCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane));
			RaycastHit2D[] hits = Physics2D.RaycastAll(worldTouch, Vector2.zero);
			foreach (RaycastHit2D hit in hits)
			{
				MRCombatManager.eDefenseType defenseType;
				if (mDefenderDefense.TryGetValue(hit.collider.gameObject.name, out defenseType))
				{
					mInspectionToggle = true;
					MRGamePieceStack defenseStack = mDefenderPositions[defenseType];
					if (!defenseStack.Inspecting)
					{
						MRGame.TheGame.InspectStack(defenseStack);
					}
					else
					{
						MRGame.TheGame.InspectStack(null);
					}
				}
			}
		}
		else if (!MRGame.IsTouchHeld && mInspectionToggle)
		{
			mInspectionToggle = false;
		}
	}

	/// <summary>
	/// Called when a game piece has been selected.
	/// </summary>
	/// <param name="piece">The selected piece.</param>
	public void OnGamePieceSelected(MRIGamePiece piece)
	{
		// pass the piece over to the combat manager
		if (Visible && piece is MRIControllable)
		{
			MRGame.TheGame.CombatManager.OnControllableSelected(mCombatData.SheetOwner, (MRIControllable)piece);
		}
	}

	private void ClearSheet()
	{
		mBreastplatePosition.Clear();
		mHelmetPosition.Clear();
		mArmorPosition.Clear();
		mShieldPositions[MRCombatManager.eAttackType.Smash].Clear();
		mShieldPositions[MRCombatManager.eAttackType.Swing].Clear();
		mShieldPositions[MRCombatManager.eAttackType.Thrust].Clear();
		mWeaponPositions[MRCombatManager.eAttackType.Smash].Clear();
		mWeaponPositions[MRCombatManager.eAttackType.Swing].Clear();
		mWeaponPositions[MRCombatManager.eAttackType.Thrust].Clear();
		mAttackPositions[MRCombatManager.eAttackType.Smash].Clear();
		mAttackPositions[MRCombatManager.eAttackType.Swing].Clear();
		mAttackPositions[MRCombatManager.eAttackType.Thrust].Clear();
		mManeuverPositions[MRCombatManager.eDefenseType.Charge].Clear();
		mManeuverPositions[MRCombatManager.eDefenseType.Dodge].Clear();
		mManeuverPositions[MRCombatManager.eDefenseType.Duck].Clear();
		mDefenderPositions[MRCombatManager.eDefenseType.Charge].Clear();
		mDefenderPositions[MRCombatManager.eDefenseType.Dodge].Clear();
		mDefenderPositions[MRCombatManager.eDefenseType.Duck].Clear();
		mManeuverPositions[MRCombatManager.eDefenseType.Charge].Clear();
		mManeuverPositions[MRCombatManager.eDefenseType.Dodge].Clear();
		mManeuverPositions[MRCombatManager.eDefenseType.Duck].Clear();
	}

	#endregion

	#region Members

	private Camera mCamera;
	private TextMesh mOwnerName;
	private TextMesh mPhaseName;
	private IDictionary<MRCombatManager.eAttackType, GameObject> mAttackerAttacks;
	private IDictionary<MRCombatManager.eAttackType, GameObject> mAttackerShields;
	private IDictionary<MRCombatManager.eDefenseType, GameObject> mAttackerManeuvers;
	private IDictionary<string, MRCombatManager.eDefenseType> mDefenderDefense;

	private IDictionary<MRCombatManager.eAttackType, MRGamePieceStack> mWeaponPositions;
	private IDictionary<MRCombatManager.eAttackType, MRGamePieceStack> mAttackPositions;
	private IDictionary<MRCombatManager.eAttackType, MRGamePieceStack> mShieldPositions;
	private IDictionary<MRCombatManager.eDefenseType, MRGamePieceStack> mManeuverPositions;
	private IDictionary<MRCombatManager.eDefenseType, MRGamePieceStack> mDefenderPositions;
	private MRGamePieceStack mBreastplatePosition;
	private MRGamePieceStack mHelmetPosition;
	private MRGamePieceStack mArmorPosition;
	private Sprite mEnabledArrow;
	private Sprite mDisabledArrow;
	private GameObject mSheetLeftArrow;
	private GameObject mSheetRightArrow;
	private SpriteRenderer mEndCombatEnabled;
	private bool mInspectionToggle = false;

	private MRCombatManager mCombat;
	private MRCombatSheetData mCombatData;

	#endregion
}

