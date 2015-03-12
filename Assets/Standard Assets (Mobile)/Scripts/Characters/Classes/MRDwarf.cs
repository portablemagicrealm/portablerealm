//
// MRDwarf.cs
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

public class MRDwarf : MRCharacter
{
	#region Properties

	public override MRGame.eCharacters Character 
	{ 
		get{
			return MRGame.eCharacters.Dwarf;
		} 
	}
	public override string Name 
	{ 
		get{
			return "Dwarf";
		} 
	}

	public override string IconName 
	{ 
		get{
			return "Textures/dwarf";
		}
	}

	/// <summary>
	/// Returns the number of asterisks healed during a rest.
	/// </summary>
	/// <value>The asterisk count.</value>
	public override int RestAsterisks
	{
		get{
			// Short Legs: can rest 2 asterisks
			return 2;
		}
	}

	#endregion

	#region Methods

	public MRDwarf()
	{
	}
	
	public MRDwarf(JSONObject jsonData, int index) :
		base(jsonData, index)
	{
	}

	// Does initialization associated with birdsong.
	public override void StartBirdsong()
	{
		base.StartBirdsong();

		// Short Legs: can't use sunlight activities
		mDaylightActivitesAvailable = 0;
	}

	// Returns the die pool for a given roll type
	public override MRDiePool DiePool(MRGame.eRollTypes roll)
	{
		// Cave Knowledge: roll one die on hide, meeting, and search tables if in cave
		if ((Location != null && Location is MRClearing && ((MRClearing)Location).type == MRClearing.eType.Cave) &&
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
		return base.DiePool(roll);
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	#endregion
}

