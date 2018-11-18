//
// MRSelectSpellEvent.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2018 Steve Jakab
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
using System;
using System.Collections;
using System.Collections.Generic;

namespace PortableRealm
{
public class MRSelectSpellEvent : MRUpdateEvent
{
	#region Callback class for dialog buttons

	public delegate void OnSpellSelected(MRSpell spell);

	#endregion

	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.SelectSpellEvent;
		}
	}

	public MRSpell SelectedSpell
	{
		get{
			return mSelectedSpell;
		}

		set{
			if (value != null && mSpells.Contains(value))
			{
				mSelectedSpell = value;
//				MRMainUI.TheUI.HideAttackManeuverDialog();
//				MRMainUI.TheUI.DisplayInstructionMessage(null);
				MRGame.TheGame.PopView();
			}
		}
	}

	public int SpellLimitTime 
	{
		get {
			return mFastestTime;
		}
	}

	#endregion

	#region Methods

	public MRSelectSpellEvent(MRCharacter character, int fastestTime, List<MRSpell> spells, OnSpellSelected callback)
	{
		mInitialized = false;
		mSelectedSpell = null;
		mCharacter = character;
		mFastestTime = fastestTime;
		mSpells.AddRange(spells);
		mCallback = callback;
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
			mCharacter.SelectSpellData = this;
			MRGame.TheGame.CharacterMat.Controllable = mCharacter;
			MRGame.TheGame.PushView(MRGame.eViews.SelectSpell);
			return false;
		}

		// check if we are done
		if (mInitialized)
		{
			if (MRGame.TheGame.CurrentView != MRGame.eViews.SelectSpell)
			{
				MRGame.TheGame.RemoveUpdateEvent(this);
				if (mCallback != null)
					mCallback(mSelectedSpell);
				return false;
			}
			return false;
		}
		return true;
	}

	#endregion

	#region Members

	private bool mInitialized;
	private MRCharacter mCharacter;
	private int mFastestTime;
	private List<MRSpell> mSpells = new List<MRSpell>();
	private MRSpell mSelectedSpell;
	private OnSpellSelected mCallback;

	#endregion
}
}

