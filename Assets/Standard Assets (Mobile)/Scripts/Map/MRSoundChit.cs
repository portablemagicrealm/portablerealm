//
// MRSoundChit.cs
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

public static class SoundChitExtensions
{
	public static int[] ClearingNumbers(this MRMapChit.eSoundChitType type)
	{
		switch (type)
		{
			case MRMapChit.eSoundChitType.Flutter:
				return new int[] {1, 2};
			case MRMapChit.eSoundChitType.Howl:
				return new int[] {4, 5};
			case MRMapChit.eSoundChitType.Patter:
				return new int[] {2, 5};
			case MRMapChit.eSoundChitType.Roar:
				return new int[] {4, 6};
			case MRMapChit.eSoundChitType.Slither:
				return new int[] {3, 6};
		}
		return new int[] {1};
	}
}

public class MRSoundChit : MRMapChit
{
	#region Properties
	
	public MRMapChit.eSoundChitType SoundType
	{
		get{
			return mSoundType;
		}
		
		set{
			mSoundType = value;
			switch (mSoundType)
			{
				case MRMapChit.eSoundChitType.Flutter:
					LongName = "FLUTTER\n" + mClearingNumber;
					ShortName = "FL" + mClearingNumber;
					break;
				case MRMapChit.eSoundChitType.Howl:
					LongName = "HOWL\n" + mClearingNumber;
					ShortName = "HO" + mClearingNumber;
					break;
				case MRMapChit.eSoundChitType.Patter:
					LongName = "PATTER\n" + mClearingNumber;
					ShortName = "PA" + mClearingNumber;
					break;
				case MRMapChit.eSoundChitType.Roar:
					LongName = "ROAR\n" + mClearingNumber;
					ShortName = "RO" + mClearingNumber;
					break;
				case MRMapChit.eSoundChitType.Slither:
					LongName = "SLITHER\n" + mClearingNumber;
					ShortName = "SL" + mClearingNumber;
					break;
			}
		}
	}
	
	public int ClearingNumber
	{
		get{
			return mClearingNumber;
		}
		
		set{
			mClearingNumber = value;
		}
	}

	public override int SummonSortIndex
	{
		get{
			return 20 + ClearingNumber;
		}
	}

	#endregion
	
	#region Methods

	static MRSoundChit()
	{
		MapChitTypes[eMapChitType.Sound] = typeof(MRSoundChit);
	}

	// dummy function to reference the class
	static public void Init() {}

	// Use this for initialization
	public override void Start ()
	{
		mChitType = eMapChitType.Sound;
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	public override bool Load(JSONObject root)
	{
		base.Load(root);
		ClearingNumber = ((JSONNumber)root["clearing"]).IntValue;
		SoundType = (MRMapChit.eSoundChitType)((JSONNumber)root["sound"]).IntValue;
		
		return true;
	}
	
	public override void Save(JSONObject root)
	{
		base.Save(root);
		root["clearing"] = new JSONNumber(ClearingNumber);
		root["sound"] = new JSONNumber((int)SoundType);
	}

	#endregion
	
	#region Members
	
	private MRMapChit.eSoundChitType mSoundType;
	private int mClearingNumber;
	
	#endregion
}

