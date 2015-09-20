//
// MRBerserker.cs
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

public class MRBerserker : MRCharacter
{
	#region Properties

	public override MRGame.eCharacters Character 
	{ 
		get{
			return MRGame.eCharacters.Berserker;
		} 
	}
	public override string Name 
	{ 
		get{
			return "Berserker";
		} 
	}

	public override string IconName 
	{ 
		get{
			return "Textures/berserker";
		}
	}

	public override int StartingGoldValue 
	{ 
		get {
			return 23;
		}
	}

	#endregion

	#region Methods

	public MRBerserker()
	{
	}
	
	public MRBerserker(JSONObject jsonData, int index) :
		base(jsonData, index)
	{
	}

	// Does initialization associated with birdsong.
	public override void StartBirdsong()
	{
		base.StartBirdsong();

		// Robust: can do an extra rest phase
		int bonus = mExtraActivities[MRGame.eActivity.Rest];
		mExtraActivities[MRGame.eActivity.Rest] = bonus + 1;
	}

	// Returns the die pool for a given roll type
	public override MRDiePool DiePool(MRGame.eRollTypes roll)
	{
		return base.DiePool(roll);
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	#endregion
}

