//
// MRWhiteKnight.cs
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
using AssemblyCSharp;

public class MRWhiteKnight : MRCharacter
{
	#region Properties
	
	public override MRGame.eCharacters Character 
	{ 
		get{
			return MRGame.eCharacters.WhiteKnight;
		} 
	}
	public override string Name 
	{ 
		get{
			return "White Knight";
		} 
	}
	
	public override string IconName 
	{ 
		get{
			return "Textures/white_knight";
		}
	}
	
	#endregion
	
	#region Methods
	
	public MRWhiteKnight()
	{
	}
	
	public MRWhiteKnight(JSONObject jsonData, int index) :
		base(jsonData, index)
	{
	}
	
	// Does initialization associated with birdsong.
	public override void StartBirdsong()
	{
		base.StartBirdsong();
		
		// Health: can do an extra rest phase
		int restBonus = mExtraActivities[MRGame.eActivity.Rest];
		mExtraActivities[MRGame.eActivity.Rest] = restBonus + 1;
	}
	
	// Returns the die pool for a given roll type
	public override MRDiePool DiePool(MRGame.eRollTypes roll)
	{
		// Honor: -1 on meeting table
		if (roll == MRGame.eRollTypes.MeetingEncounter ||
		    roll == MRGame.eRollTypes.MeetingHire ||
		    roll == MRGame.eRollTypes.MeetingTrade)
		{
			MRDiePool pool = MRDiePool.NewDicePool;
			pool.DieMod = -1;
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
