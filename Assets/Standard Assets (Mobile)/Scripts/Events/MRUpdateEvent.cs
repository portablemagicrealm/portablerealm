//
// MRUpdateEvent.cs
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
using System;
using System.Collections;

public abstract class MRUpdateEvent : IComparable
{
	#region Constants

	// Events will be handled in priority order (low->high).
	public enum ePriority
	{
		CreateMapEvent,
		CreateCharacterEvent,
		RollForCurseEvent,
		UpdateViewEvent,
		FatigueCharacterEvent,
		SelectChitEvent,
		SelectClearingEvent,
		CombatEvent,
		MonsterRollEvent,
		EndPhaseEvent,
		EndTurnEvent,
		InitGameTimeEvent,
		UpdateActivityListEvent,
	}

	#endregion

	#region Properties

	public abstract ePriority Priority { get; }

	#endregion

	#region Methods

	/// <summary>
	/// Updates this instance.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	public abstract bool Update ();

	public int CompareTo(object obj)
	{
		if (obj is MRUpdateEvent)
		{
			return (int)Priority - (int)(((MRUpdateEvent)obj).Priority);
		}
		throw new ArgumentException();
	}

	public virtual void OnClearingSelected(MRClearing clearing)
	{
	}

	#endregion
}

