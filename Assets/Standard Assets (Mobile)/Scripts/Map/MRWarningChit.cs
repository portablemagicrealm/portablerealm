//
// MRWarningChit.cs
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

namespace PortableRealm
{
	
public class MRWarningChit : MRMapChit
{
	#region Properties
	
	public MRMapChit.eWarningChitType WarningType
	{
		get{
			return mWarningType;
		}
		
		set{
			mWarningType = value;
			mChitType = eMapChitType.Warning;
			mSubstitute = MRDwelling.eDwelling.None;
		}
	}
	
	public MRTile.eTileType TileType
	{
		get{
			return mTileType;
		}
		
		set{
			mTileType = value;

			string longName = "";
			switch (mWarningType)
			{
				case eWarningChitType.Bones:
					longName = "BONES";
					ShortName = "BO";
					break;
				case eWarningChitType.Dank:
					longName = "DANK";
					ShortName = "DA";
					break;
				case eWarningChitType.Ruins:
					longName = "RUINS";
					ShortName = "RU";
					break;
				case eWarningChitType.Smoke:
					longName = "SMOKE";
					ShortName = "SM";
					break;
				case eWarningChitType.Stink:
					longName = "STINK";
					ShortName = "ST";
					break;
			}
			switch (mTileType)
			{
				case MRTile.eTileType.Cave:
					LongName = longName + "\nC";
					break;
				case MRTile.eTileType.Mountain:
					LongName = longName + "\nM";
					break;
				case MRTile.eTileType.Valley:
					LongName = longName + "\nV";
					switch (mWarningType)
					{
						case eWarningChitType.Bones:
							mSubstitute = MRDwelling.eDwelling.Ghosts;
							break;
						case eWarningChitType.Dank:
							mSubstitute = MRDwelling.eDwelling.Chapel;
							break;
						case eWarningChitType.Ruins:
							mSubstitute = MRDwelling.eDwelling.GuardHouse;
							break;
						case eWarningChitType.Smoke:
							mSubstitute = MRDwelling.eDwelling.House;
							break;
						case eWarningChitType.Stink:
							mSubstitute = MRDwelling.eDwelling.Inn;
							break;
					}
					break;
				case MRTile.eTileType.Woods:
					LongName = longName + "\nW";
					switch (mWarningType)
					{
						case eWarningChitType.Smoke:
							mSubstitute = MRDwelling.eDwelling.SmallFire;
							break;
						case eWarningChitType.Stink:
							mSubstitute = MRDwelling.eDwelling.LargeFire;
							break;
					}
					break;
			}
		}
	}

	public MRDwelling.eDwelling Substitute
	{
		get{
			return mSubstitute;
		}
	}

	public override int SummonSortIndex
	{
		get{
			// order doesn't matter for warning chits, as they summon to the controllable's clearing
			return 10;
		}
	}

	#endregion
	
	#region Methods

	static MRWarningChit()
	{
		MapChitTypes[eMapChitType.Warning] = typeof(MRWarningChit);
	}

	// dummy function to reference the class
	static public void Init() {}

	// Use this for initialization
	public override void Start ()
	{
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	public override bool Load(JSONObject root)
	{
		base.Load(root);

		if (root["warning"] is JSONNumber)
		{
			WarningType = (MRMapChit.eWarningChitType)((JSONNumber)root["warning"]).IntValue;
		}
		else if (root["warning"] is JSONString)
		{
			if (ChitWarningMap.TryGetValue(((JSONString)root["warning"]).Value, out mWarningType))
			{
				WarningType = mWarningType;
			}
			else
			{
				return false;
			}
		}

		if (root["tile"] is JSONNumber)
		{
			TileType = (MRTile.eTileType)((JSONNumber)root["tile"]).IntValue;
		}
		else if (root["tile"] is JSONString)
		{
			if (MRTile.TileTypeMap.TryGetValue(((JSONString)root["tile"]).Value, out mTileType))
			{
				TileType = mTileType;
			}
			else
			{
				return false;
			}
		}
		
		return true;
	}
	
	public override void Save(JSONObject root)
	{
		base.Save(root);
		root["warning"] = new JSONNumber((int)WarningType);
		root["tile"] = new JSONNumber((int)TileType);
	}

	#endregion
	
	#region Members
	
	private MRMapChit.eWarningChitType mWarningType;
	private MRTile.eTileType mTileType;
	private MRDwelling.eDwelling mSubstitute;
	
	#endregion
}

}