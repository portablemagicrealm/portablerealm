//
// MRSelectChitEvent.cs
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

public class MRSelectChitEvent : MRUpdateEvent
{
	#region Callback class for dialog buttons

	public delegate void OnChitSelected(MRActionChit chit);

	#endregion

	#region Constants

	public enum eCompare
	{
		LessThan,
		LessThanEqualTo,
		EqualTo,
		GreaterThanEqualTo,
		GreaterThan
	}

	#endregion

	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.SelectChitEvent;
		}
	}

	public MRActionChit.eType ChitType
	{
		get{
			return mChitType;
		}
	}

	public MRGame.eStrength Strength
	{
		get{
			return mStrength;
		}
	}

	public eCompare StrengthDirection
	{
		get{
			return mStrengthDirection;
		}
	}

	public int Speed
	{
		get{
			return mSpeed;
		}
	}

	public eCompare SpeedDirection
	{
		get{
			return mSpeedDirection;
		}
	}

	public MRActionChit SelectedChit
	{
		get{
			return mChit;
		}

		set{
			if (value != null)
			{
				// test chit against desired result

				mChit = value;
				MRMainUI.TheUI.HideAttackManeuverDialog();
				MRMainUI.TheUI.DisplayInstructionMessage(null);
				MRGame.TheGame.PopView();
			}
		}
	}

	#endregion

	#region Methods

	public MRSelectChitEvent(MRCharacter character, MRActionChit.eType type, MRGame.eStrength strength,
	                         eCompare strengthDirection, int speed, eCompare speedDirection,
	                         OnChitSelected callback)
	{
		mCharacter = character;
		mChitType = type;
		mStrength = strength;
		mStrengthDirection = strengthDirection;
		mSpeed = speed;
		mSpeedDirection = speedDirection;
		mCallback = callback;
		mInitialized = false;
	}

	/// <summary>
	/// Updates this instance.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	public override bool Update ()
	{
		if (!mInitialized)
		{
			mInitialized = true;
			mCharacter.SelectChitData = this;
			MRGame.TheGame.CharacterMat.Controllable = mCharacter;
			MRGame.TheGame.PushView(MRGame.eViews.SelectChit);
			return false;
		}

		// check if we are done fatiguing a character
		if (mInitialized)
		{
			if (MRGame.TheGame.CurrentView != MRGame.eViews.SelectChit)
			{
				MRGame.TheGame.RemoveUpdateEvent(this);
				if (mCallback != null)
					mCallback(mChit);
				return false;
			}
/*
			if (mCharacter.FatigueBalance != 0)
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Fatigue Asterisks (" + mCharacter.FatigueBalance + ")");
			}
			else if (mCharacter.WoundBalance > 0)
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Wound Chit");
			}
			else if (mCharacter.HealBalance > 0)
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Heal Chit");
			}
			if (mCharacter.FatigueBalance == 0 && mCharacter.WoundBalance == 0 && mCharacter.HealBalance == 0)
			{
				MRMainUI.TheUI.DisplayInstructionMessage(null);
				mCharacter = null;
				MRGame.TheGame.PopView();
			}
*/
			return false;
		}
		return true;
	}

	#endregion

	#region Members

	private bool mInitialized;
	private MRCharacter mCharacter;
	private MRActionChit.eType mChitType;
	private MRGame.eStrength mStrength;
	private eCompare mStrengthDirection;
	private int mSpeed;
	private eCompare mSpeedDirection;
	private MRActionChit mChit;
	private OnChitSelected mCallback;

	#endregion
}

