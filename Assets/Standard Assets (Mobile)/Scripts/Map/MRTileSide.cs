//
// MRTileSide.cs
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

namespace PortableRealm
{
	
public class MRTileSide : MonoBehaviour, MRIColorSource
{
	#region Constants

	public enum eType
	{
		Normal,
		Enchanted
	}
	const int MAX_CLEARINGS = 6;

	#endregion

	#region Properties

	public eType type;

	public bool Initialized
	{
		get
		{
			return mInitialized;
		}
	}

	public string ShortName
	{
		get{
			return (type == eType.Normal ? "n" : "e") + Tile.shortName;
		}
	}

	public MRTile Tile
	{
		get
		{
			return mTile;
		}
		set
		{
			mTile = value;
		}
	}
	
	/// <summary>
	/// Returns a boolean array. If an array element is true, that edge has a road.
	/// </summary>
	public bool[] RoadEdges
	{
		get
		{
			return mRoadEdges;
		}
	}

	public MRClearing[] EdgeClearings
	{
		get
		{
			return mEdgeClearings;
		}
	}

	/// <summary>
	/// Returns the clearings that are on this tile side.
	/// </summary>
	public MRClearing[] Clearings
	{
		get
		{
			return mClearings;
		}
	}

	public GameObject[] ChitLocations
	{
		get{
			return mChitLocations;
		}
	}

	/// <summary>
	/// Returns a list of the color magic supplied by this object.
	/// </summary>
	/// <value>The magic supplied.</value>
	public virtual IList<MRGame.eMagicColor> MagicSupplied 
	{ 
		get	{
			// @todo make this data-driven
			List<MRGame.eMagicColor> magicSupplied = new List<MRGame.eMagicColor>();
			if (type == eType.Enchanted)
			{
				switch (mTile.type)
				{
					case MRTile.eTileType.Cave:
						if (mTile.Id == MRMap.eTileNames.ruins)
							magicSupplied.Add(MRGame.eMagicColor.Grey);
						else if (mTile.Id == MRMap.eTileNames.highpass || 
							mTile.Id == MRMap.eTileNames.caves ||
							mTile.Id == MRMap.eTileNames.cavern)
						{
							magicSupplied.Add(MRGame.eMagicColor.Purple);
						}
						break;
					case MRTile.eTileType.Mountain:
						if (mTile.Id == MRMap.eTileNames.deepwoods)
							magicSupplied.Add(MRGame.eMagicColor.Gold);
						else if (mTile.Id == MRMap.eTileNames.mountain || 
							mTile.Id == MRMap.eTileNames.ledges ||
							mTile.Id == MRMap.eTileNames.cliff)
						{
							magicSupplied.Add(MRGame.eMagicColor.Purple);
						}
						else if (mTile.Id == MRMap.eTileNames.crag)
						{
							magicSupplied.Add(MRGame.eMagicColor.Grey);
							magicSupplied.Add(MRGame.eMagicColor.Gold);
							magicSupplied.Add(MRGame.eMagicColor.Purple);
						}
						break;
					case MRTile.eTileType.Valley:
						magicSupplied.Add(MRGame.eMagicColor.Grey);
						break;
					case MRTile.eTileType.Woods:
						magicSupplied.Add(MRGame.eMagicColor.Gold);
						break;
				}
			}
			return magicSupplied;
		}
	}

	#endregion

	#region Methods

	void Awake()
	{
		// set up clearings
		MRClearing[] clearings = gameObject.GetComponentsInChildren<MRClearing> ();
		mClearings = new MRClearing[MAX_CLEARINGS];
		foreach (MRClearing clearing in clearings) 
		{
			clearing.MyTileSide = this;
			mClearings[clearing.clearingNumber] = clearing;
		}
		
		// set up edges
		bool hasRoadEdge = false;
		MREdge[] roadEdges = gameObject.GetComponentsInChildren<MREdge> ();
		foreach (MREdge roadEdge in roadEdges) 
		{
			if (roadEdge.side >= 0 && roadEdge.side < 6)
			{
				mRoadEdges[roadEdge.side] = true;
				hasRoadEdge = true;
			}
		}
		if (type == eType.Normal && !hasRoadEdge)
		{
			Debug.LogError("no edges for tile");
			Application.Quit();
		}

		// get other tile locations
		Component[] coms = GetComponentsInChildren<SpriteRenderer>();
		foreach (var com in coms)
		{
			if (com.gameObject.name.StartsWith("chit"))
			{
				int chitNumber = int.Parse(com.gameObject.name.Substring("chit".Length));
				mChitLocations[chitNumber] = com.gameObject;
			}
		}

		// set up roads
		MRRoad[] roads = gameObject.GetComponentsInChildren<MRRoad>();
		foreach (MRRoad road in roads)
		{
			road.MyTileSide = this;
			road.clearingConnection0.AddRoad(road);
			if (road.clearingConnection1 != null)
				road.clearingConnection1.AddRoad(road);
			else if (road.edgeConnection != null)
				mEdgeClearings[road.edgeConnection.side] = road.clearingConnection0;
			else
			{
				Debug.LogError("Road with no 2nd clearing or edge");
				Application.Quit();
			}
		}

		mInitialized = true;
	}

	// Use this for initialization
	void Start () 
	{
		MREdge[] roadEdges = gameObject.GetComponentsInChildren<MREdge> ();
		foreach (MREdge roadEdge in roadEdges) 
		{
			roadEdge.Tile = mTile;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	/// <summary>
	/// Returns the clearing for a given clearing number.
	/// </summary>
	/// <returns>The clearing, or null of there is no clearing for the number.</returns>
	/// <param name="clearingNumber">the clearing number. Note this is 1-based.</param>
	public MRClearing GetClearing(int clearingNumber)
	{
		if (clearingNumber >= 1 && clearingNumber <= 6)
			return mClearings[clearingNumber - 1];
		return null;
	}

	#endregion

	#region Members

	private MRTile mTile;
	private MRClearing[] mClearings;
	private GameObject[] mChitLocations = new GameObject[2];
	private bool[] mRoadEdges = new bool[6];
	private MRClearing[] mEdgeClearings = new MRClearing[6];
	private bool mInitialized;

	#endregion
}

}