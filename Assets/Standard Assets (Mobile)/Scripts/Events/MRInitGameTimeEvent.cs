//
// MRInitGameTimeEvent.cs
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

public class MRInitGameTimeEvent : MRUpdateEvent
{
	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.InitGameTimeEvent;
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
		// process events for the current time of day
		switch (MRGame.TimeOfDay)
		{
			case MRGame.eTimeOfDay.Birdsong:
				// set up activities for the day
				foreach (MRIControllable controlable in MRGame.TheGame.Controlables)
					controlable.StartBirdsong();
				MRGame.TheGame.ResetActiveControllable();
				MRGame.TheGame.AddUpdateEvent(new MRUpdateActivityListEvent());
				MRGame.TheGame.RemoveUpdateEvent(this);
				break;
			case MRGame.eTimeOfDay.Sunrise:
				MRGame.TheGame.ActivityList.ActivityList = null;
				foreach (MRIControllable controlable in MRGame.TheGame.Controlables)
					controlable.StartSunrise();
				// make monster roll
				MRGame.TheGame.AddUpdateEvent(new MRMonsterRollEvent());
				MRGame.TheGame.RemoveUpdateEvent(this);
				break;
			case MRGame.eTimeOfDay.Daylight:
				MRGame.TheGame.AddUpdateEvent(new MRUpdateActivityListEvent());
				MRGame.TheGame.RandomizeControllables();
				MRGame.TheGame.ResetActiveControllable();
				MRGame.TheGame.RemoveUpdateEvent(this);
				break;
			case MRGame.eTimeOfDay.Sunset:
				foreach (MRIControllable controlable in MRGame.TheGame.Controlables)
					controlable.StartSunset();
				MRGame.TheGame.ActivityList.ActivityList = null;
				MRGame.TheGame.RemoveUpdateEvent(this);
				break;
			case MRGame.eTimeOfDay.Evening:
				foreach (MRIControllable controlable in MRGame.TheGame.Controlables)
					controlable.StartEvening();
				MRGame.TheGame.AddUpdateEvent(new MRCombatEvent());
				MRGame.TheGame.RemoveUpdateEvent(this);
				break;
			case MRGame.eTimeOfDay.Midnight:
				foreach (MRIControllable controlable in MRGame.TheGame.Controlables)
					controlable.StartMidnight();
				MRGame.TheGame.TheMap.StartMidnight();
				MRDenizenManager.StartMidnight();
				MRGame.TheGame.MonsterChart.MonsterRoll = 0;
				MRGame.TheGame.RemoveUpdateEvent(this);
				break;
			default:
				break;
		}
		return true;
	}

	#endregion
}

