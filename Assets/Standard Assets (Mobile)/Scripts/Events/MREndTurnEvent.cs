//
// MREndTurnEvent.cs
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
using System.Collections.Generic;

public class MREndTurnEvent : MRUpdateEvent
{
	#region Properties
	
	public override ePriority Priority 
	{ 
		get {
			return ePriority.EndTurnEvent;
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
		if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight)
		{
			MRIControllable activeControllable = MRGame.TheGame.ActiveControllable;
			MRILocation currentClearing = activeControllable.Location;
			MRTile currentTile = currentClearing.MyTileSide.Tile;

			// move any non-blocked prowling monsters in the same tile to the controllable's clearing
			IList<MRMonster> prowling = MRGame.TheGame.MonsterChart.ProwlingMonsters;
			foreach (MRMonster monster in prowling)
			{
				if (!monster.Blocked && monster.Location != null && monster.Location.MyTileSide.Tile == currentTile)
				{
					monster.Location = currentClearing;
					if (!activeControllable.Hidden)
					{
						// blocked
						if (!activeControllable.Blocked)
						{
							MRGame.TheGame.ShowInformationDialog("Blocked!");
							activeControllable.Blocked = true;
						}
						monster.Blocked = true;
					}
				}
			}

			// re-add the character to the clearing so he'll be on top
			//if (activeControllable is MRCharacter)
			//	activeControllable.Clearing = currentClearing;

			// flip and relocate tile chits
			currentTile.ActivateMapChits();

			// summon prowling denizens based on the chits in the tile
			MRTile.eTileType warningType = MRTile.eTileType.Valley;
			foreach (MRMapChit chit in currentTile.MapChits)
			{
				if (chit is MRWarningChit)
				{
					warningType = ((MRWarningChit)chit).TileType;
					break;
				}
			}

			ArrayList chits = new ArrayList();
			foreach (MRMapChit chit in currentTile.MapChits)
			{
				chits.Add(chit);
			}
			chits.Sort(new MRMapChit.MRMapChitSorter());
			foreach (MRMapChit chit in chits)
			{
				if (activeControllable.ActivatesChit(chit) && !chit.SummonedMonsters)
				{
					IList<MRDenizen> summoned = MRGame.TheGame.MonsterChart.GetSummonedDenizens(chit, warningType);
					if (summoned.Count > 0)
					{
						chit.SummonedMonsters = true;
						MRILocation summonClearing = null;
						switch (chit.ChitType)
						{
							case MRMapChit.eMapChitType.Warning:
								summonClearing = currentClearing;
								break;
							case MRMapChit.eMapChitType.Sound:
								summonClearing = currentTile.FrontSide.GetClearing(((MRSoundChit)chit).ClearingNumber);
								break;
							case MRMapChit.eMapChitType.Site:
								summonClearing = currentTile.FrontSide.GetClearing(((MRSiteChit)chit).ClearingNumber);
								break;
						}
						foreach (MRDenizen denizen in summoned)
						{
							Debug.Log("Chit " + chit.LongName + " summons " + denizen.Name);
							denizen.Location = summonClearing;
							if (denizen is MRMonster && currentClearing == summonClearing && !activeControllable.Hidden)
							{
								// blocked
								if (!activeControllable.Blocked)
								{
									MRGame.TheGame.ShowInformationDialog("Blocked!");
									activeControllable.Blocked = true;
								}
								denizen.Blocked = true;
							}
						}
					}
				}
			}
			// re-add map chits to tile so they'll be on top
			//currentTile.ActivateMapChits();
		}
		MRGame.TheGame.RemoveUpdateEvent(this);
/*
		if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Midnight)
		{
			++MRGame.DayOfMonth;
			MRGame.TimeOfDay = MRGame.eTimeOfDay.Birdsong;
		}
		else
			++MRGame.TimeOfDay;
		MRGame.TheGame.RemoveUpdateEvent(this);
		MRGame.TheGame.AddUpdateEvent(new MRInitGameTimeEvent());
*/
		return true;
	}

	#endregion
}

