//
// MREndPhaseEvent.cs
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

public class MREndPhaseEvent : MRUpdateEvent
{
	#region Properties
	
	public override ePriority Priority 
	{ 
		get {
			return ePriority.EndPhaseEvent;
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
		MRGame.TheGame.RemoveUpdateEvent(this);

		// for daylight, make sure all controllables' phases have been run 
		if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight)
		{
			if (MRGame.TheGame.ActiveControllable != null)
			{
				// check if blocked
				if (!MRGame.TheGame.ActiveControllable.Hidden)
				{
					MRILocation clearing = MRGame.TheGame.ActiveControllable.Location;
					if (clearing != null)
					{
						foreach (MRIGamePiece piece in clearing.Pieces.Pieces)
						{
							if (piece is MRMonster)
							{
								if (!MRGame.TheGame.ActiveControllable.Blocked)
								{
									MRGame.TheGame.ShowInformationDialog("Blocked!");
									MRGame.TheGame.ActiveControllable.Blocked = true;
								}
								((MRMonster)piece).Blocked = true;
							}
						}
					}
				}
				// check if all activities have been executed
				foreach (MRActivity activity in MRGame.TheGame.ActiveControllable.ActivitiesForDay(MRGame.DayOfMonth).Activities)
				{
					if (!activity.Executed)
					{
						return false;
					}
				}
			}
		}
		MRGame.TheGame.AddUpdateEvent(new MREndTurnEvent());
		return true;
	}
	
	#endregion
}

