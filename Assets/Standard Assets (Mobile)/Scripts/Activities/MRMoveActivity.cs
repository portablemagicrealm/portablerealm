//
// MRMoveActivity.cs
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
using AssemblyCSharp;

public class MRMoveActivity : MRActivity
{
	#region Properties

	public MRClearing Clearing
	{
		get{
			return mClearing;
		}

		set{
			mClearing = value;
		}
	}

	#endregion

	#region Methods

	public MRMoveActivity() : base(MRGame.eActivity.Move)
	{
	}

	protected override void InternalUpdate()
	{
		if (mClearing == null)
		{
			Debug.LogError("Executing move with no destination");
			Executed = true;
			return;
		}

		// make sure the clearings connect
		bool validForMoveType = false;
		MRILocation currentClearing = Owner.Location;
		switch (Owner.MoveType)
		{
			case MRGame.eMoveType.Walk:
			{
				MRRoad road = currentClearing.RoadTo(Clearing);
				if (road != null)
				{
					// if we're walking along a hidden road, make sure we've discovered it
					if (road.type == MRRoad.eRoadType.HiddenPath || 
				    	road.type == MRRoad.eRoadType.SecretPassage)
					{
						if (Owner.DiscoveredRoads.IndexOf(road) >= 0)
							validForMoveType = true;
						else
							Debug.LogWarning("Secret path not discovered");
					}
					else
						validForMoveType = true;
				}
				else
					Debug.LogWarning("No road");
				break;
			}
			case MRGame.eMoveType.WalkThroughWoods:
				if (currentClearing.RoadTo(Clearing) != null ||
			    	currentClearing.MyTileSide == Clearing.MyTileSide)
				{
					validForMoveType = true;
				}
				break;
			case MRGame.eMoveType.Fly:
				break;
		}
		if (validForMoveType)
		{
			switch (mClearing.type)
			{
				case MRClearing.eType.Woods:
				case MRClearing.eType.Cave:
					TestForDroppedItems();
					Owner.Location = Clearing;
					break;
				case MRClearing.eType.Mountain:
				{
					// only move if the previous activity was a move to the same clearing
					int myIndex = Parent.Activities.IndexOf(this);
					if (myIndex	> 0)
					{
						MRActivity prevActivity = Parent.Activities[myIndex - 1];
						if (prevActivity is MRMoveActivity && ((MRMoveActivity)prevActivity).Clearing == Clearing)
						{
							TestForDroppedItems();
							Owner.Location = Clearing;
							// temp - test fatigue
/*
							if (Owner is MRCharacter)
							{
								MRCharacter character = (MRCharacter)Owner;
								if (character.CanFatigueChit(1))
								{
									character.SetFatigueBalance(1);
								}
							}
*/
						}
					}
					break;
				}
			}
		}
		Executed = true;
	}

	private void TestForDroppedItems()
	{
		if (Owner is MRCharacter)
		{
			MRCharacter character = (MRCharacter)Owner;
			IList<MRItem> toDrop = new List<MRItem>();
			foreach (MRItem item in character.ActiveItems)
			{
				if (!character.CanMoveWithItem(item))
					toDrop.Add(item);
			}
			foreach (MRItem item in character.InactiveItems)
			{
				if (!character.CanMoveWithItem(item))
					toDrop.Add(item);
			}
			if (toDrop.Count > 0)
			{
				foreach (MRItem item in toDrop)
				{
					if (item is MRTreasure)
						((MRTreasure)item).Hidden = true;
					character.RemoveItem(item);
					character.Location.AbandonedItems.AddPieceToBottom(item);
				}
				MRGame.TheGame.ShowInformationDialog("Abandoned some items that were too heavy");
			}
		}
	}

	public override bool Load(JSONObject root)
	{
		bool result = base.Load(root);
		if (result)
		{
			JSONValue clearingTest = root["clearing"];
			if (clearingTest != null)
			{
				string clearingName = ((JSONString)clearingTest).Value;
				mClearing = MRGame.TheGame.GetClearing(clearingName);
				if (mClearing == null)
				{
					Debug.LogError("Move activity null clearing for " + clearingName);
				}
			}
		}
		return result;
	}
	
	public override void Save(JSONObject root)
	{
		base.Save(root);
		if (mClearing != null)
		{
			root["clearing"] = new JSONString(mClearing.Name);
		}
	}

	#endregion

	#region Members

	private MRClearing mClearing;

	#endregion
}

