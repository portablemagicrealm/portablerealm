//
// MRFatigueCharacterEvent.cs
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

namespace PortableRealm
{
	
public class MRFatigueCharacterEvent : MRUpdateEvent
{
	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.FatigueCharacterEvent;
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// Updates this instance.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	public override bool Update ()
	{
		// check if we are done fatiguing a character
		if (mFatiguedCharacter != null)
		{
			if (mFatiguedCharacter.FatigueBalance != 0)
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Fatigue Asterisks (" + mFatiguedCharacter.FatigueBalance + ")");
			}
			else if (mFatiguedCharacter.WoundBalance > 0)
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Wound Chit");
			}
			else if (mFatiguedCharacter.HealBalance > 0)
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Heal Chit");
			}
			if (mFatiguedCharacter.FatigueBalance == 0 && mFatiguedCharacter.WoundBalance == 0 && mFatiguedCharacter.HealBalance == 0)
			{
				MRMainUI.TheUI.DisplayInstructionMessage(null);
				mFatiguedCharacter = null;
				MRGame.TheGame.PopView();
			}
			return false;
		}

		// check if a character needs to fatigue or wound a chit
		if (mFatiguedCharacter == null)
		{
			foreach (MRIControllable controllable in MRGame.TheGame.Controlables)
			{
				if (controllable is MRCharacter)
				{
					MRCharacter character = (MRCharacter)controllable;
					if (character.FatigueBalance != 0 || character.WoundBalance > 0 || character.HealBalance > 0)
					{
						mFatiguedCharacter = (MRCharacter)controllable;
						MRGame.TheGame.CharacterMat.Controllable = mFatiguedCharacter;
						MRGame.TheGame.PushView(MRGame.eViews.FatigueCharacter);
						return false;
					}
				}
			}
		}
		return true;
	}

	#endregion

	#region Members

	private MRCharacter mFatiguedCharacter;

	#endregion
}

}