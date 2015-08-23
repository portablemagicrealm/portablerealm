//
// MRUpdateViewEvent.cs
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

public class MRUpdateViewEvent : MRUpdateEvent
{
	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.UpdateViewEvent;
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
		// set the main view
		switch (MRGame.TheGame.CurrentView)
		{
			case MRGame.eViews.Map:
			case MRGame.eViews.SelectClearing:
				MRGame.TheGame.TheMap.Visible = true;
				MRGame.TheGame.MonsterChart.Visible = false;
				MRGame.TheGame.TreasureChart.Visible = false;
				MRGame.TheGame.CharacterMat.Visible = false;
				MRGame.TheGame.CombatSheet.Visible = false;
				MRGame.TheGame.Main.Visible = false;
				break;
			case MRGame.eViews.Characters:
				MRGame.TheGame.CharacterMat.Visible = true;
				MRGame.TheGame.TheMap.Visible = false;
				MRGame.TheGame.MonsterChart.Visible = false;
				MRGame.TheGame.TreasureChart.Visible = false;
				MRGame.TheGame.CharacterMat.Controllable = MRGame.TheGame.ActiveControllable;
				MRGame.TheGame.CombatSheet.Visible = false;
				MRGame.TheGame.Main.Visible = false;
				break;
			case MRGame.eViews.Monsters:
				MRGame.TheGame.MonsterChart.Visible = true;
				MRGame.TheGame.TreasureChart.Visible = false;
				MRGame.TheGame.TheMap.Visible = false;
				MRGame.TheGame.CharacterMat.Visible = false;
				MRGame.TheGame.CombatSheet.Visible = false;
				MRGame.TheGame.Main.Visible = false;
				break;
			case MRGame.eViews.Treasure:
				MRGame.TheGame.TreasureChart.Visible = true;
				MRGame.TheGame.MonsterChart.Visible = false;
				MRGame.TheGame.TheMap.Visible = false;
				MRGame.TheGame.CharacterMat.Visible = false;
				MRGame.TheGame.CombatSheet.Visible = false;
				MRGame.TheGame.Main.Visible = false;
				break;
			case MRGame.eViews.Main:
				MRGame.TheGame.Main.Visible = true;
				MRGame.TheGame.TreasureChart.Visible = false;
				MRGame.TheGame.MonsterChart.Visible = false;
				MRGame.TheGame.TheMap.Visible = false;
				MRGame.TheGame.CharacterMat.Visible = false;
				MRGame.TheGame.CombatSheet.Visible = false;
				break;
			case MRGame.eViews.FatigueCharacter:
			case MRGame.eViews.SelectAttack:
			case MRGame.eViews.SelectManeuver:
			case MRGame.eViews.SelectChit:
				MRGame.TheGame.CharacterMat.Visible = true;
				MRGame.TheGame.TheMap.Visible = false;
				MRGame.TheGame.MonsterChart.Visible = false;
				MRGame.TheGame.TreasureChart.Visible = false;
				MRGame.TheGame.CombatSheet.Visible = false;
				MRGame.TheGame.Main.Visible = false;
				break;
			case MRGame.eViews.Combat:
				MRGame.TheGame.CombatSheet.Visible = true;
				MRGame.TheGame.TreasureChart.Visible = false;
				MRGame.TheGame.MonsterChart.Visible = false;
				MRGame.TheGame.TheMap.Visible = false;
				MRGame.TheGame.CharacterMat.Visible = false;
				MRGame.TheGame.Main.Visible = false;
				break;
			default:
				break;
		}
		MRGame.TheGame.RemoveUpdateEvent(this);

		return true;
	}

	#endregion
}

