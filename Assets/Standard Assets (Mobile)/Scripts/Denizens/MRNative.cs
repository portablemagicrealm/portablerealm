//
// MRNative.cs
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
using System.Text;
using AssemblyCSharp;

namespace PortableRealm
{
	
public class MRNative : MRDenizen
{
	#region Constants

	#endregion

	#region Properties

	public override int SortValue
	{
		get{
			return (int)MRGame.eSortValue.Denizen;
		}
	}

	public int MemberNumber
	{
		get{
			return mMemberNumber;
		}
	}

	public MRGame.eNatives Group 
	{
		get{
			return mGroup;
		}
	}

	public bool IsHired
	{
		get{
			return false;
		}
	}

	#endregion

	#region Methods

	public MRNative()
	{
	}
	
	public MRNative(JSONObject jsonData, int index) :
		base((JSONObject)jsonData["MRDenizen"], ((JSONNumber)jsonData["id"]).IntValue)
	{
		string groupName = ((JSONString)jsonData["group"]).Value;
		mGroup = groupName.Native();
		mMemberNumber = ((JSONNumber)jsonData["id"]).IntValue;
		mWage = ((JSONNumber)jsonData["wage"]).IntValue;

		string groupLetter = groupName.Substring(0, 1).ToUpper();
		TextMesh[] texts = mCounter.GetComponentsInChildren<TextMesh>();
		foreach (TextMesh text in texts)
		{
			if (text.gameObject.name == "FrontIdText" || text.gameObject.name == "BackIdText")
			{
				if (mMemberNumber == 0)
					text.text = groupLetter + "HQ";
				else
					text.text = groupLetter + mMemberNumber.ToString();
			}
		}
	}

	protected override void CreateCounter()
	{
		mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.nativeCounterPrototype);
	}

	/// <summary>
	/// Tests if the native is allowed to do the activity.
	/// </summary>
	/// <returns><c>true</c> if this instance can execute the specified activity; otherwise, <c>false</c>.</returns>
	/// <param name="activity">Activity.</param>
	public override bool CanExecuteActivity(MRActivity activity)
	{
		// hired leaders can trade like characters
		if (activity is MRTradeActivity && mMemberNumber == 0 & IsHired)
			return true;

		return base.CanExecuteActivity(activity);
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	public override bool IsValidTarget(MRSpell spell)
	{
		return (spell.Targets.Contains(MRGame.eSpellTarget.Native) ||
			spell.Targets.Contains(MRGame.eSpellTarget.Natives) ||
			spell.Targets.Contains(MRGame.eSpellTarget.NativeGroup));
	}

	#endregion

	#region Members

	//private MRNativeGroup mGroup;
	private MRGame.eNatives mGroup;
	private int mWage;
	private int mMemberNumber;	// 0 = leader

	#endregion
}

}