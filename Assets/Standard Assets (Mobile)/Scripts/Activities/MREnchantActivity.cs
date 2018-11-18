//
// MREnchantActivity.cs
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

using System.Collections;
using System.Collections.Generic;

namespace PortableRealm
{
	
public class MREnchantActivity : MRActivity
{
	#region Constants

	private enum eState
	{
		Start,
		SelectEnchantType,
		SelectChit,
	}

	#endregion

	#region Methods

	public MREnchantActivity() : base(MRGame.eActivity.Enchant)
	{
		mState = eState.Start;
	}

	protected override void InternalUpdate()
	{
		if (mState != eState.Start)
			return;
		if (Owner.CanExecuteActivity(this) && Owner is MRCharacter)
		{
			MRCharacter character = (MRCharacter)Owner;
			if (!character.HasMeditated)
			{
				character.HasMeditated = true;
			}
			else
			{
				// determine if the character can enchant a chit or their tile
				bool canEnchantChit = false;
				bool canEnchantTile = false;

				IList<MRActionChit> magicChits = character.MagicChits;
				foreach (var chit in magicChits)
				{
					MRMagicChit magicChit = (MRMagicChit)chit;
					if (magicChit.CanBeUsedFor(MRActionChit.eAction.EnchantChit))
					{
						mValidEnchantChitValues.Add(magicChit.MagicColor);
						canEnchantChit = true;
					}
					if (magicChit.CanBeUsedFor(MRActionChit.eAction.EnchantTile))
					{
						// there needs to be a color supply that matches the chit type - it can come from the clearing 
						// or an enchanted chit the character has
						if (character.Location is MRClearing)
						{
							MRClearing clearing = (MRClearing)character.Location;
							if (clearing.MagicSupplied.Contains(magicChit.MagicColor))
							{
								mValidEnchantTileValues.Add(magicChit.MagicColor);
								canEnchantTile = true;
							}
							else
							{
								foreach (var chit2 in magicChits)
								{
									MRMagicChit magicChit2 = (MRMagicChit)chit2;
									if (chit != chit2 && magicChit2.IsEnchanted && magicChit2.MagicColor == magicChit.MagicColor)
									{
										mValidEnchantTileValues.Add(magicChit.MagicColor);
										canEnchantTile = true;
									}
								}
							}
						}
					}
				}
				if (canEnchantChit || canEnchantTile)
				{
					Executed = false;
					if (canEnchantChit && canEnchantTile)
					{
						mState = eState.SelectEnchantType;
						MRMainUI.TheUI.DisplaySelectionDialog("Enchant", null, new string[] {"Enchant Chit", "Enchant Tile"}, EnchantSelectionCallback);
					}
					else if (canEnchantChit)
						EnchantChit();
					else
						EnchantTile();
					return;
				}
			}
		}
		Executed = true;
	}

	/// <summary>
	/// Callback for the player selecting whether they want to enchant a chit or tile.
	/// </summary>
	/// <param name="button">Button.</param>
	private void EnchantSelectionCallback(int button)
	{
		if (button == 0)
			EnchantChit();
		else if (button == 1)
			EnchantTile();
	}

	/// <summary>
	/// Starts the sequence to enchant a magic chit.
	/// </summary>
	private void EnchantChit()
	{
		MRCharacter character = (MRCharacter)Owner;
		mState = eState.SelectChit;
		MRGame.eMagicColor[] validColors = new MRGame.eMagicColor[mValidEnchantChitValues.Count];
		mValidEnchantChitValues.CopyTo(validColors);
		MRGame.TheGame.AddUpdateEvent(new MRSelectChitEvent(character, MRActionChit.eAction.EnchantChit, validColors, false, OnEnchantChitSelected));
	}

	/// <summary>
	/// Starts the sequence to enchant a tile.
	/// </summary>
	private void EnchantTile()
	{
		MRCharacter character = (MRCharacter)Owner;
		mState = eState.SelectChit;
		MRGame.eMagicColor[] validColors = new MRGame.eMagicColor[mValidEnchantTileValues.Count];
		mValidEnchantTileValues.CopyTo(validColors);
		MRGame.TheGame.AddUpdateEvent(new MRSelectChitEvent(character, MRActionChit.eAction.EnchantChit, validColors, false, OnEnchantTileSelected));
	}

	/// <summary>
	/// Callback when a chit is selected for enchanting the chit.
	/// </summary>
	/// <param name="chit">Chit.</param>
	void OnEnchantChitSelected(MRActionChit chit)
	{
		if (chit is MRMagicChit)
		{
			((MRMagicChit)chit).IsEnchanted = true;
		}
		Executed = true;
	}

	/// <summary>
	/// Callback when a chit is selected for enchanting a tile.
	/// </summary>
	/// <param name="chit">Chit.</param>
	void OnEnchantTileSelected(MRActionChit chit)
	{
		if (chit is MRMagicChit)
		{
			MRMagicChit magicChit = (MRMagicChit)chit;
			// if the clearing is supplying the color magic, the character doesn't need to fatigue a chit
			bool fatigueChit = true;
			MRCharacter character = (MRCharacter)Owner;
			if (character.Location is MRClearing)
			{
				MRClearing clearing = (MRClearing)character.Location;
				if (clearing.MagicSupplied.Contains(magicChit.MagicColor))
				{
					fatigueChit = false;	
				}
			}
			if (fatigueChit)
			{
				mSeletcedMagicChit = magicChit;
				MRGame.TheGame.AddUpdateEvent(new MRSelectChitEvent(character, MRActionChit.eAction.SupplyColor, new MRGame.eMagicColor[] {magicChit.MagicColor}, true, OnColorChitSelected));
				return;
			}
			else
			{
				character.Location.MyTileSide.Tile.StartFlip();
			}
		}
		Executed = true;
	}

	/// <summary>
	/// Callback when a color chit is selected for enchanting a tile.
	/// </summary>
	/// <param name="chit">Chit.</param>
	void OnColorChitSelected(MRActionChit chit)
	{
		if (chit is MRMagicChit)
		{
			MRMagicChit colorChit = (MRMagicChit)chit;
			if (colorChit.IsEnchanted && colorChit.MagicColor == mSeletcedMagicChit.MagicColor)
			{
				MRCharacter character = (MRCharacter)Owner;
				character.ForceFatigueChit(colorChit);
				character.Location.MyTileSide.Tile.StartFlip();
			}
		}
		Executed = true;
	}

	#endregion

	#region Members

	private eState mState;
	private HashSet<MRGame.eMagicColor> mValidEnchantChitValues = new HashSet<MRGame.eMagicColor>();
	private HashSet<MRGame.eMagicColor> mValidEnchantTileValues = new HashSet<MRGame.eMagicColor>();
	private MRMagicChit mSeletcedMagicChit;

	#endregion
}

}