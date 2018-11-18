//
// MRCharacterSpellsDisplay.cs
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
using System;
using System.Collections;
using System.Collections.Generic;

namespace PortableRealm
{
	
public class MRCharacterSpellsDisplay : MRTabItems, MRITouchable
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
	}

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();

		if (mLearnedSpells == null)
		{
			int learnedSpellsCount = 0;
			int itemSpellsCount = 0;
			MRTreasureChartLocation[] locations = gameObject.GetComponentsInChildren<MRTreasureChartLocation>();
			foreach (var location in locations)
			{
				if (location.stackName.StartsWith("knownSpell"))
				{
					++learnedSpellsCount;
				}
				else if (location.stackName.StartsWith("itemSpell"))
				{
					++itemSpellsCount;
				}
			}
			mLearnedSpells = new MRGamePieceStack[learnedSpellsCount];
			mItemSpells = new MRGamePieceStack[itemSpellsCount];
			mLearnedSpellsLocations = new GameObject[learnedSpellsCount];
			mItemSpellsLocations = new GameObject[itemSpellsCount];
			foreach (var location in locations)
			{
				if (location.stackName.StartsWith("knownSpell"))
				{
					int index = int.Parse(location.stackName.Substring("knownSpell".Length));
					mLearnedSpells[index] = MRGame.TheGame.NewGamePieceStack();;
					mLearnedSpellsLocations[index] = location.gameObject;
					SetupTreasureStack(mLearnedSpellsLocations[index], mLearnedSpells[index]);
				}
				else if (location.stackName.StartsWith("itemSpell"))
				{
					int index = int.Parse(location.stackName.Substring("itemSpell".Length));
					mItemSpells[index] = MRGame.TheGame.NewGamePieceStack();;
					mItemSpellsLocations[index] = location.gameObject;
					SetupTreasureStack(mItemSpellsLocations[index], mItemSpells[index]);
				}
			}
		}
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update();

		if (Parent.Visible && Parent.Controllable is MRCharacter)
		{
			MRCharacter character = (MRCharacter)Parent.Controllable;
			int spellLimitTime = int.MaxValue;
			if (character.SelectSpellData != null)
			{
				spellLimitTime = character.SelectSpellData.SpellLimitTime;
			}
			foreach (var stack in mLearnedSpells)
			{
				foreach (var item in stack.Pieces)
				{
					MRSpellCard card = item as MRSpellCard;
					if (card != null)
					{
						card.Spell.Known = false;
					}
				}
				stack.Clear();
			}
			foreach (var stack in mItemSpells)
			{
				stack.Clear();
			}
			int spellIndex = 0;
			IList<MRSpell> spells = character.LearnedSpells;
			foreach (var spell in spells)
			{
				MRIGamePiece spellCard = MRGame.TheGame.GetGamePiece(spell.Id);
				if (spellCard != null)
				{
					MRSpellCard card = spellCard as MRSpellCard;
					if (card != null)
					{
						card.Spell.Known = true;
						card.LocalScale = new Vector3(1.5f, 1.5f, 1f);
						if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectSpell &&
							character.CanCastSpell(spell, spellLimitTime))
						{
							card.Selectable = true;
						}
						mLearnedSpells[spellIndex++].AddPieceToTop(spellCard);
					}
				}
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
			// see if a spell has been selected
			MRSpellCard selectedSpell = touchedObject.GetComponentInChildren<MRSpellCard>();
			if (selectedSpell != null)
			{
				OnGamePieceSelected(selectedSpell);
			}
		}
		return true;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	public virtual bool OnTouchMove(GameObject touchedObject, float delta_x, float delta_y)
	{
		return true;
	}

	public virtual bool OnButtonActivate(GameObject touchedObject)
	{
		return true;
	}

	public bool OnPinchZoom(float pinchDelta)
	{
		return true;
	}

	/// <summary>
	/// Called when a game piece has been selected.
	/// </summary>
	/// <param name="piece">The game piece.</param>
	public void OnGamePieceSelected(MRIGamePiece piece)
	{
		MRCharacter character = Parent.Controllable as MRCharacter;
		MRSpellCard spellCard = piece as MRSpellCard;

		switch (MRGame.TheGame.CurrentView)
		{
			case MRGame.eViews.SelectSpell:
				if (spellCard != null)
				{
					if (character.SelectSpellData != null)
					{
						character.SelectSpellData.SelectedSpell = spellCard.Spell;
					}
					else
					{
						MRGame.TheGame.PopView();
					}
				}
				break;
			default:
				break;
		}
	}

	private void SetupTreasureStack(GameObject locationObj, MRGamePieceStack stack)
	{
		stack.Layer = LayerMask.NameToLayer("CharacterMat");
		MRTreasureChartLocation location = locationObj.GetComponentInChildren<MRTreasureChartLocation>();
		if (location != null)
			location.Treasures = stack;
	}

	#endregion

	#region Members

	private MRCharacterMat mParent;
	private MRGamePieceStack[] mLearnedSpells;
	private MRGamePieceStack[] mItemSpells;
	private GameObject[] mLearnedSpellsLocations;
	private GameObject[] mItemSpellsLocations;

	#endregion
}

}