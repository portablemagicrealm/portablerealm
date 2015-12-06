//
// MRAlertEvent.cs
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

public class MRAlertEvent : MRUpdateEvent
{
	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.AlertEvent;
		}
	}

	#endregion

	#region Methods

	public MRAlertEvent(MRCharacter character, MRActionChit.eAction alertType)
	{
		mCharacter = character;
		mAlertType = alertType;
	}

	/// <summary>
	/// Updates this instance.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	public override bool Update ()
	{
		if (mAlertType == MRActionChit.eAction.CombatAlert || mAlertType == MRActionChit.eAction.Alert)
		{
			MRMainUI.TheUI.DisplayInstructionMessage("Alert weapon or chit");
		}
		else
		{
			MRGame.TheGame.RemoveUpdateEvent(this);
			return false;
		}

		if (!mFirstPass)
		{
			mCharacter.SelectChitFilter = new MRSelectChitEvent.MRSelectChitFilter(mAlertType);
			MRGame.TheGame.CharacterMat.Controllable = mCharacter;
			MRGame.TheGame.PushView(MRGame.eViews.Alert);
			mFirstPass = true;
		}
		return false;
	}

	/// <summary>
	/// Called to end the event.
	/// </summary>
	public override void EndEvent()
	{
		base.EndEvent();

		MRMainUI.TheUI.DisplayInstructionMessage(null);
		mCharacter.SelectChitFilter = null;
		MRGame.TheGame.CharacterMat.Controllable = null;
		MRGame.TheGame.PopView();
	}

	#endregion

	#region Members

	private bool mFirstPass;
	private MRCharacter mCharacter;
	private MRActionChit.eAction mAlertType;

	#endregion
}

