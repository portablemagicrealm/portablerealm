//
// MRTreasure.cs
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
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Text;
using AssemblyCSharp;

namespace PortableRealm
{
	
public class MRTreasure : MRItem
{
	#region Properties

	public bool IsGreatTreasure
	{
		get{
			return mIsGreatTreasure;
		}
	}

	public bool IsLargeTreasure
	{
		get{
			return mIsLargeTreasure;
		}
	}

	public bool IsTwiT
	{
		get{
			return mIsTwiT;
		}
	}

	public int SellFame 
	{
		get{
			return mSellFame;
		}
	}

	public MRGame.eNatives SellFameGroup
	{
		get{
			return mSellFameGroup;
		}
	}

	public bool Hidden
	{
		get{
			return mHidden;
		}

		set{
			mHidden = value;
		}
	}

	public override int SortValue
	{
		get{
			return (int)MRGame.eSortValue.Treasure;
		}
	}

	#endregion

	#region Methods

	public MRTreasure ()
	{
	}

	public MRTreasure(JSONObject data, int index) : base((JSONObject)data["MRItem"], index)
	{
		mHidden = true;

		mIsGreatTreasure = ((JSONBoolean)data["isgreat"]).Value;
		mIsLargeTreasure = ((JSONBoolean)data["islarge"]).Value;
		mIsTwiT = ((JSONBoolean)data["istwit"]).Value;
		mSellFame = ((JSONNumber)data["sellfame"]).IntValue;
		if (mSellFame > 0)
		{
			mSellFameGroup = ((JSONString)data["sellfamegroup"]).Value.Native();
		}

		mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.treasureCardPrototype);
		TextMesh text = mCounter.GetComponentInChildren<TextMesh>();
		if (text.name == "FrontText")
		{
			StringBuilder buffer = new StringBuilder(Name.ToUpper());
			buffer.Replace(' ', '\n');
			text.text = buffer.ToString();
		}
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();

		Vector3 orientation = mCounter.transform.localEulerAngles;
		if ((Hidden && Math.Abs(orientation.y - 180f) > 0.1f) ||
			(!Hidden && Math.Abs(orientation.y) > 0.1f))
		{
			mCounter.transform.Rotate(new Vector3(0, 180f, 0));
		}
	}

	#endregion

	#region Members

	private bool mIsGreatTreasure;
	private bool mIsLargeTreasure;
	private bool mIsTwiT;
	private bool mHidden;
	private int mSellFame;
	private MRGame.eNatives mSellFameGroup;

	#endregion
}

}