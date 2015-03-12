//
// MRRollForCurseEvent.cs
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

public class MRRollForCurseEvent : MRUpdateEvent
{
	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.RollForCurseEvent;
		}
	}

	#endregion

	#region Methods

	public MRRollForCurseEvent(MRIControllable roller, MRCharacter target)
	{
		mRoller = roller;
		mTarget = target;
		mDiePool = roller.DiePool(MRGame.eRollTypes.Curse);
		mDiePool.RollDice();
	}

	/// <summary>
	/// Updates this instance.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	public override bool Update ()
	{
		if (mDiePool.RollReady)
		{
			int roll = mDiePool.Roll;
			MRMainUI.TheUI.DisplayDieRollResult(mDiePool);
			Debug.Log("Curse roll = " + roll);
			MRGame.eCurses curse = (MRGame.eCurses)(roll - 1);
			mTarget.AddCurse(curse);

			MRGame.TheGame.ShowInformationDialog("Cursed with " + MRUtility.StringFromCurse(curse), "Cursed!");

			MRGame.TheGame.RemoveUpdateEvent(this);
		}

		return false;
	}

	#endregion

	#region Members

	private MRIControllable mRoller;
	private MRCharacter mTarget;
	private MRDiePool mDiePool;

	#endregion
}

