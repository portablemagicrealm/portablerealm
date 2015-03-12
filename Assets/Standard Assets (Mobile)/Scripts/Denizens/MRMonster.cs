//
// MRMonster.cs
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

public class MRMonster : MRDenizen
{
	#region Properties

	/// <summary>
	/// Returns if this is a red-side up tremendous monster.
	/// </summary>
	/// <value><c>true</c> if this instance is red side monster; otherwise, <c>false</c>.</value>
	public override bool IsRedSideMonster 
	{ 
		get {
			return BaseWeight == MRGame.eStrength.Tremendous && Side == MRDenizen.eSide.Dark;
		}
	}

	#endregion

	#region Methods

	public MRMonster()
	{
	}
	
	public MRMonster(JSONObject jsonData, int index) :
		base((JSONObject)jsonData["MRDenizen"], index)
	{
	}

	protected override void CreateCounter()
	{
		switch (mWeight)
		{
			case MRGame.eStrength.Medium:
				mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.mediumMonsterCounterPrototype);
				break;
			case MRGame.eStrength.Heavy:
				mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.heavyMonsterCounterPrototype);
				break;
			case MRGame.eStrength.Tremendous:
				mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.tremendousMonsterCounterPrototype);
				break;
			default:
				Debug.LogError("Unexpected denizen weight " + mWeight);
				break;
		}
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	// Called when the controllable hits its target
	public override void HitTarget(MRIControllable attacker, bool targetDead)
	{
		if (BaseWeight == MRGame.eStrength.Tremendous)
		{
			// tremendous monsters that hit turn to their red side until they kill their target
			if (targetDead)
				Side = MRDenizen.eSide.Light;
			else
				Side = MRDenizen.eSide.Dark;
		}
	}

	#endregion
}

