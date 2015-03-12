//
// MRCombatEvent.cs
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

public class MRCombatEvent : MRUpdateEvent
{
	#region Constants

	enum ePhase
	{
		NextClearing,
		RunCombat,
		AllCombatDone,
	}

	#endregion

	#region Properties
	
	public override ePriority Priority 
	{ 
		get {
			return ePriority.CombatEvent;
		}
	}
	
	#endregion
	
	#region Methods

	public MRCombatEvent()
	{
		mPhase = ePhase.NextClearing;
		mCurrentClearing = null;
	}

	/// <summary>
	/// Updates this instance.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	public override bool Update ()
	{
		switch (mPhase)
		{
			case ePhase.NextClearing:
				MRGame.TheGame.InCombat = true;
				return SelectNextClearing();
			case ePhase.RunCombat:
				// run the combat until it is over
				if (!MRGame.TheGame.CombatManager.Update())
				{
					mPhase = ePhase.NextClearing;
				}
				return false;
			case ePhase.AllCombatDone:
				MRGame.TheGame.InCombat = false;
				MRGame.TheGame.CombatManager.Clearing = null;
				MRGame.TheGame.SetView(MRGame.eViews.Map);
				MRGame.TheGame.RemoveUpdateEvent(this);
				break;
			default:
				break;
		}
		return true;
	}

	/// <summary>
	/// Determines the next clearing combat takes place in. Once all combat is done, returns to the map.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	private bool SelectNextClearing()
	{
		bool ready = (mCurrentClearing == null);
		foreach (uint clearingId in MRGame.TheGame.Clearings)
		{
			// find the first occupied clearing after our current one
			MRClearing clearing = MRGame.TheGame.GetClearing(clearingId);
			if (clearing == mCurrentClearing)
				ready = true;
			if (ready && clearing != mCurrentClearing)
			{
				// if the clearing contains a character or hired leader, combat takes place
				foreach (MRIGamePiece piece in clearing.Pieces.Pieces)
				{
					if (piece is MRCharacter)
					{
						mCurrentClearing = clearing;
						MRGame.TheGame.CombatManager.Clearing = clearing;
						MRGame.TheGame.SetView(MRGame.eViews.Combat);
						mPhase = ePhase.RunCombat;
						return false;
					}
				}
			}
		}
		// no more clearings to fight in, end combat
		mPhase = ePhase.AllCombatDone;
		return false;
	}

	#endregion

	#region Members

	private ePhase mPhase;
	private MRClearing mCurrentClearing;


	#endregion
}

