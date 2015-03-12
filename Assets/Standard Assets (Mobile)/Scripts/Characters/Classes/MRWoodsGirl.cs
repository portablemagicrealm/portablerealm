//
// MRWoodsGirl.cs
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
using System.Collections;
using AssemblyCSharp;

public class MRWoodsGirl : MRCharacter
{
	#region Properties

	public override MRGame.eCharacters Character 
	{ 
		get{
			return MRGame.eCharacters.WoodsGirl;
		} 
	}
	public override string Name 
	{ 
		get{
			return "Woods Girl";
		} 
	}

	public override string IconName 
	{ 
		get{
			return "Textures/woodsgirl";
		}
	}

	#endregion

	#region Methods

	public MRWoodsGirl()
	{
	}
	
	public MRWoodsGirl(JSONObject jsonData, int index) :
		base(jsonData, index)
	{
	}

	// Does initialization associated with birdsong.
	public override void StartBirdsong()
	{
		base.StartBirdsong();
	}

	// Returns the die pool for a given roll type
	public override MRDiePool DiePool(MRGame.eRollTypes roll)
	{
		// Tracking Skills: roll one die on hide, meeting, and search tables if on a woods tile
		if ((Location != null && Location.MyTileSide.Tile.type == MRTile.eTileType.Woods) &&
		    (roll == MRGame.eRollTypes.Hide || 
			roll == MRGame.eRollTypes.SearchLocate ||
			roll == MRGame.eRollTypes.SearchLoot ||
			roll == MRGame.eRollTypes.SearchMagicSight ||
			roll == MRGame.eRollTypes.SearchPeer ||
			roll == MRGame.eRollTypes.SearchRunes ||
			roll == MRGame.eRollTypes.MeetingEncounter ||
			roll == MRGame.eRollTypes.MeetingHire ||
			roll == MRGame.eRollTypes.MeetingTrade))
		{
			MRDiePool pool = MRDiePool.NewDicePool;
			pool.NumDice = 1;
			return pool;
		}
		// Archer: roll 1 die on missile rolls
		else if (roll == MRGame.eRollTypes.Missile)
		{
			MRDiePool pool = MRDiePool.NewDicePool;
			pool.NumDice = 1;
			return pool;
		}
		return base.DiePool(roll);
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	#endregion
}

