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

namespace PortableRealm
{

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

	/// <summary>
	/// If this "monster" is a head/club, returns the monster that owns it
	/// </summary>
	/// <value>The owning monster.</value>
	public MRMonster OwnedBy
	{
		get{
			return mOwnedBy;
		}
	}

	/// <summary>
	/// If this is a tremendous monster, returns its head/club.
	/// </summary>
	/// <value>The head/club "monster".</value>
	public MRMonster Owns
	{
		get {
			return mOwns;
		}
	}

	public override int SortValue
	{
		get{
			return mOwnedBy == null ? (int)MRGame.eSortValue.Monster : (int)MRGame.eSortValue.MonsterHead;
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
		mFlies = ((JSONBoolean)jsonData["flies"]).Value;
		mOwnedByName = ((JSONString)jsonData["owned_by"]).Value;
		mOwnedBy = null;
		mOwns = null;
	}

	public void SetOwnership()
	{
		if (mOwnedByName != null && mOwnedByName.Length > 0)
		{
			mOwnedBy = MRDenizenManager.GetMonster(mOwnedByName, mIndex);
			if (mOwnedBy != null)
			{
				mOwnedBy.mOwns = this;
			}
			else
			{
				Debug.LogError("Unable to find monster owner " + mOwnedByName);
			}
		}
		else
		{
			mOwnedBy = null;
		}
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

	/// <summary>
	/// Called when the monster hits its target 
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="targetDead">If set to <c>true</c> target dead.</param>
	public override void HitTarget(MRIControllable target, bool targetDead)
	{
		if (BaseWeight == MRGame.eStrength.Tremendous)
		{
			// tremendous monsters that hit turn to their red side until they kill their target
			if (targetDead)
				Side = MRDenizen.eSide.Light;
			else
				Side = MRDenizen.eSide.Dark;
		}
		else if (mOwnedBy != null)
		{
			// a hit by a head/club counts as a hit by its owner
			mOwnedBy.HitTarget(target, targetDead);
		}
	}

	public override bool IsValidTarget(MRSpell spell)
	{
		return (spell.Targets.Contains(MRGame.eSpellTarget.Monster) ||
			spell.Targets.Contains(MRGame.eSpellTarget.Monsters));
	}

	#endregion

	#region Members

	private bool mFlies;
	private string mOwnedByName;
	private MRMonster mOwnedBy;
	private MRMonster mOwns;

	#endregion
}

}