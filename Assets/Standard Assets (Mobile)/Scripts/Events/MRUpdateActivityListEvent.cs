//
// MRUpdateActivityListEvent.cs
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
	
public class MRUpdateActivityListEvent : MRUpdateEvent
{
	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.UpdateActivityListEvent;
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
		if (MRGame.TheGame.ActiveControllable != null)
		{
			MRActivityList currentActivityList = MRGame.TheGame.ActiveControllable.ActivitiesForDay(MRGame.DayOfMonth);

			// process events for the current time of day
			switch (MRGame.TimeOfDay)
			{
				case MRGame.eTimeOfDay.Birdsong:
					// set up the activity list for each controllable
					if (MRGame.TheGame.ActivityList.ActivityList != currentActivityList)
					{
						MRGame.TheGame.ActivityList.ActivityList = currentActivityList;
					}
					break;
				case MRGame.eTimeOfDay.Daylight:
					// execute the day's activities for each controllable
					if (MRGame.TheGame.ActivityList.ActivityList != currentActivityList)
					{
						MRGame.TheGame.ActiveControllable.StartDaylight();
						MRGame.TheGame.ActivityList.ActivityList = currentActivityList;
						if (currentActivityList.Activities.Count == 0)
							MRGame.TheGame.AddUpdateEvent(new MREndPhaseEvent());
					}
					break;
				default:
					MRGame.TheGame.RemoveUpdateEvent(this);
					break;
			}
		}
		return true;
	}

	#endregion
}

}