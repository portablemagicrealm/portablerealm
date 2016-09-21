//
// MRTile.cs
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
using System.Collections.Generic;
using AssemblyCSharp;

public class MRTile : MonoBehaviour, MRISerializable, MRITouchable
{
	#region Constants

	public enum eTileType
	{
		Valley,
		Woods,
		Cave,
		Mountain,
	}

	#endregion

	#region Properties

	public eTileType type;
	public String tileName;
	public String shortName;

	public bool Initialized
	{
		get
		{
			foreach (MRTileSide side in mSides.Values)
			{
				if (!side.Initialized)
					return false;
			}
			return mInitialized;
		}
	}
	
	public MRMap.eTileNames Id
	{
		get
		{
			return mId;
		}
		
		set
		{
			mId = value;
		}
	}

	public MRUtility.IntVector2 Coordinate
	{
		get{
			return mCoordinate;
		}

		set{
			mCoordinate = value;
			//Debug.Log("Coordinate for tile " + tileName + " set to " + mCoordinate);
		}
	}

	public MRTileSide.eType Front
	{
		get 
		{
			return mFront;
		}
		
		set
		{
			if (value != mFront)
			{
				mFront = value;
				gameObject.transform.Rotate(0, 180.0f, 0);
			}
		}
	}
	
	public MRTileSide FrontSide
	{
		get
		{
			MRTileSide side;
			mSides.TryGetValue(Front, out side);
			return side;
		}
	}
	
	// Which side is up (top of screen)
	//
	//       0
	//     5   1
	//     4   2
	//       3
	//
	public int Facing
	{
		get
		{
			return mFacing;
		}
		
		set
		{
			value = MREdge.Normalize(value);
			//Debug.Log("Set tile " + Id + " facing to " + value);
			if (value != mFacing)
			{
				int delta = value - mFacing;
//				if (delta < 0)
//					delta = -(delta + 6);
				mFacing = value;
				gameObject.transform.Rotate(0, 0, delta * 60.0f);
			}
		}
	}
	
	//
	// Returns a boolean array. If an array element is true, that edge has a road.
	//
	public bool[] RoadEdges
	{
		get
		{
			MRTileSide side;
			if (mSides.TryGetValue(MRTileSide.eType.Normal, out side))
				return side.RoadEdges;
			return null;
		}
	}

	public IList<MRMapChit> MapChits
	{
		get{
			return mMapChits;
		}
	}

	#endregion

	#region Methods

	public MRTile()
	{
		mFront = MRTileSide.eType.Normal;
		mFacing = 0;
	}

	void Awake()
	{
		MRTileSide[] sides = gameObject.GetComponentsInChildren<MRTileSide> ();
		foreach (MRTileSide side in sides)
		{
			side.Tile = this;
			mSides.Add(side.type, side);
		}

		// determine the hex size based on the collider height
		float miny = 0, maxy = 0;
		Vector2[] points = ((PolygonCollider2D)(gameObject.GetComponent<Collider2D>())).points;
		foreach (Vector2 pt in points)
		{
			if (pt.y > maxy)
				maxy = pt.y;
			if (pt.y < miny)
				miny = pt.y;
		}
		mSize = maxy - miny;

		mInitialized = true;
	}

	// Use this for initialization
	void Start () 
	{
		mMapCamera = MRGame.TheGame.TheMap.MapCamera;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (MRGame.TheGame.CurrentView != MRGame.eViews.Map)
			return;
	}

	public bool OnTouched(GameObject touchedObject)
	{
		return true;
	}

	public bool OnReleased(GameObject touchedObject)
	{
		return true;
	}

	public bool OnSingleTapped(GameObject touchedObject)
	{
		return true;
	}

	public bool OnDoubleTapped(GameObject touchedObject)
	{
		if (touchedObject == gameObject)
		{
			Debug.Log("Tile selected: " + tileName);
			SendMessageUpwards("OnTileSelectedGame", this, SendMessageOptions.DontRequireReceiver);
		}
		return true;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	public virtual bool OnButtonActivate(GameObject touchedObject)
	{
		return true;
	}

	public bool OnPinchZoom(float pinchDelta)
	{
		MRGame.TheGame.TheMap.OnPinchZoom(pinchDelta);
		return true;
	}

	/// <summary>
	/// Return the tile next to a given side.
	/// </summary>
	/// <returns>The adjacent tile.</returns>
	/// <param name="side">Side.</param>
	public MRTile GetAdjacentTile(int side)
	{
		return mAdjacentTile[MREdge.Normalize(side)];
	}

	public MRClearing GetClearingForEdge(int edge)
	{
		edge = MREdge.Normalize(edge);
		return FrontSide.EdgeClearings[edge];
	}

	/// <summary>
	/// Return which side a given tile is adjecent to. Returns -1 if not adjecent.
	/// </summary>
	/// <returns>The adjacent tile side.</returns>
	/// <param name="tile">Tile.</param>
	public int GetAdjacentTileSide(MRTile tile)
	{
		for (int side = 0; side < mAdjacentTile.Length; ++side)
		{
			if (mAdjacentTile[side] == tile)
				return side;
		}
		return -1;
	}

	/// <summary>
	/// Sets a tile to be adjacent to this tile. If there is already a tile on that side, it will be replaced.
	/// </summary>
	/// <param name="tile">The tile being placed.</param>
	/// <param name="theirSide">Which side of the tile is being placed next to us.</param>
	/// <param name="mySide">Which side the tile is placed next to.</param>
	/// <param name="backLink">Flag that a tile we are linking to is linking to us.</param>
	public void SetAdjacentTile(MRTile tile, int theirSide, int mySide, bool backLink)
	{
		mySide = MREdge.Normalize(mySide);
		theirSide = MREdge.Normalize(theirSide);

		MRTile oldTile = mAdjacentTile[mySide];
		if (oldTile != tile)
		{
			mAdjacentTile[mySide] = tile;
			// if my edge has a road, connect its clearing to the other tile's clearing
			MRClearing myClearing = GetClearingForEdge(mySide);
			if (myClearing != null)
			{
				MRClearing theirClearing = null;
				if (tile != null)
				{
					theirClearing = tile.GetClearingForEdge(theirSide);
					if (theirClearing == null)
					{
						Debug.LogError("Connecting tile side to invalid side");
						Application.Quit();
					}
				}
				foreach (MRRoad road in myClearing.Roads)
				{
					if (road.edgeConnection != null && road.edgeConnection.side == mySide)
					{
						//Debug.Log("Connect clearing " + road.clearingConnection0.Name + " to " + theirClearing.Name);
						road.clearingConnection1 = theirClearing;
						break;
					}
				}
			}
			// update old and new tiles
			if (oldTile != null)
			{
				// remove link to old tile
				int theirBacklinkSide = oldTile.GetAdjacentTileSide(this);
				if (theirBacklinkSide >= 0)
					oldTile.SetAdjacentTile(null, mySide, theirBacklinkSide, true);
			}
			if (tile != null)
			{
				// add link to new tile
				tile.SetAdjacentTile(this, mySide, theirSide, true);
				if (!backLink)
				{
					if (tile.transform.position == Vector3.zero)
					{
						// change new tile's facing and position
						tile.Facing = Facing - mySide + theirSide - 3;
						Vector3 offset = new Vector3(0, mSize, 0);
						tile.transform.position = transform.position + offset;
						tile.transform.RotateAround(transform.position, Vector3.forward, 60.0f * (Facing - mySide));
						tile.transform.Rotate(new Vector3(0, 0, 60.0f * (mySide - Facing)));
					}
				}
			}
		}
	}

	/// <summary>
	/// Disconnects the tile from all its neighbors
	/// </summary>
	public void RemoveFromMap()
	{
		for (int i = 0; i < 6; ++i)
		{
			// note the theirSide param value doesn't matter
			SetAdjacentTile(null, 0, i, false);
		}
		Facing = 0;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
	}

	/// <summary>
	/// Returns a randomized list of edges that have roads.
	/// </summary>
	/// <returns>The randomized road edges.</returns>
	public IList<int> GetRandomizedRoadEdges()
	{
		IList<int> roadEdges = new List<int>();
		for (int i = 0; i < 6; ++i)
		{
			if (RoadEdges[i])
				roadEdges.Add(i);
		}
		roadEdges.Shuffle();
		return roadEdges;
	}

	/// <summary>
	/// Returns a randomized list of edges that meets the following criteria:
	///    1) has a road
	///    2) is not next to a tile
	///    3) has an adjacent edge that is next to a tile
	/// </summary>
	/// <returns>The random available edges.</returns>
	public IList<int> GetRandomAvailableEdges()
	{
		IList<int> goodEdges = new List<int>();
		IList<int> roadEdges = GetRandomizedRoadEdges();
		foreach (int testEdge in roadEdges)
		{
			// test to see if the space next to a road is empty
			if (GetAdjacentTile(testEdge) == null)
			{
				// check the edges of either side of the test road 
				if (GetAdjacentTile(testEdge + 1) != null ||
					GetAdjacentTile(testEdge - 1) != null)
				{
					goodEdges.Add(testEdge);
				}
			}
		}

		return goodEdges;
	}

	/// <summary>
	/// Returns the coordinate offset for a given side of the tile.
	/// </summary>
	/// <returns>The offset for the side</returns>
	/// <param name="side">Side to get the coordinate of.</param>
	public MRUtility.IntVector2 OffsetForSide(int side)
	{
		side = MREdge.Normalize(6 - Facing + side);
		if (mCoordinate.x % 2 == 0)
			return MRMap.EvenTileOffsets[side];
		else
			return MRMap.OddTileOffsets[side];
	}

	/// <summary>
	/// Returns the coordinate for a given side of the tile.
	/// </summary>
	/// <returns>The coordinate for the side</returns>
	/// <param name="side">Side to get the coordinate of.</param>
	public MRUtility.IntVector2 CoordinateForSide(int side)
	{
		return Coordinate + OffsetForSide(side);
	}

	/// <summary>
	/// Returns the side number for a given adjacent coordinate.
	/// </summary>
	/// <returns>The side number, or -1 if the coordinate if not adjacent</returns>
	/// <param name="adjacent">Adjacent coordinate.</param>
	public int SideForCoordinate(MRUtility.IntVector2 adjacent)
	{
		MRUtility.IntVector2 offset = adjacent - Coordinate;
		MRUtility.IntVector2[] offsets;
		if (mCoordinate.x % 2 == 0)
			offsets = MRMap.EvenTileOffsets;
		else
			offsets = MRMap.OddTileOffsets;
		for (int i = 0; i < offsets.Length; ++i)
		{
			if (offsets[i] == offset)
				return MREdge.Normalize(Facing + i);
		}
		return -1;
	}

	public MRUtility.IntVector2[] AdjacentCoordinates()
	{
		MRUtility.IntVector2[] adjacent = new MRUtility.IntVector2[6];
		for (int i = 0; i < 6; ++i)
		{
			adjacent[i] = CoordinateForSide(i);
		}
		return adjacent;
	}

	/// <summary>
	/// Tests if a tile can connect to this tile, and an adjacent tile.
	/// </summary>
	/// <returns><c>true</c>, if valid connection was tested, <c>false</c> otherwise.</returns>
	/// <param name="testTile">Test tile.</param>
	/// <param name="tileEdge">Tile edge.</param>
	/// <param name="myEdge">My edge.</param>
	/// <param name="clockwise">If set to <c>true</c> clockwise.</param>
	public bool TestValidConnection(int myEdge, MRTile testTile, int testEdge)
	{
		testEdge = MREdge.Normalize(testEdge);
		myEdge = MREdge.Normalize(myEdge);

		return testTile.RoadEdges[testEdge] == this.RoadEdges[myEdge];
	}

	public void AddMapChit(MRMapChit chit)
	{
		MapChits.Add(chit);
		chit.Parent = FrontSide.transform;
		chit.Layer = LayerMask.NameToLayer("Map");
		GameObject positionObject = null;
		string positionObjectName = "chit" + (MapChits.Count - 1);
		Component[] coms = FrontSide.GetComponentsInChildren<SpriteRenderer>();
		foreach (Component com in coms)
		{
			if (com.gameObject.name == positionObjectName)
			{
				positionObject = com.gameObject;
				break;
			}
		}
		if (positionObject != null)
		{
			Vector3 targetPos = positionObject.transform.position;
			chit.Position = new Vector3(targetPos.x, targetPos.y, -1.0f);
		}
		else
		{
			Debug.LogError("Missing chit position marker");
		}
	}

	/// <summary>
	/// Flips map chits face up and puts clearing chits in their clearing.
	/// </summary>
	public void ActivateMapChits()
	{
		//foreach (MRChit chit in mMapChits)
		for (int i = 0; i < mMapChits.Count; ++i)
		{
			MRChit chit = mMapChits[i];
			chit.SideUp = MRChit.eSide.Front;
			if (chit is MRSuperSiteChit)
			{
				// add the contained chits to the tile
				MRSuperSiteChit superSite = (MRSuperSiteChit)chit;
				foreach (MRMapChit subChit in superSite.ContainedChits)
				{
					mMapChits.Add(subChit);
					subChit.Parent = FrontSide.transform;
				}
				superSite.ContainedChits.Clear();
			}
			else if (chit is MRWarningChit && ((MRWarningChit)chit).Substitute != MRDwelling.eDwelling.None)
			{
				// find a clearing that connects to the borderlands
				MRTile borderland;
				MRGame.TheGame.TheMap.MapTiles.TryGetValue(MRMap.eTileNames.borderland, out borderland);
				MRClearing borderlandClearing = borderland.FrontSide.GetClearing(1);
				MRClearing testClearing = FrontSide.GetClearing(5);
				if (MRMap.Path(borderlandClearing, testClearing) == null)
				{
					testClearing = FrontSide.GetClearing(4);
					if (MRMap.Path(borderlandClearing, testClearing) == null)
						testClearing = FrontSide.GetClearing(2);
				}
				// replace the chit with its substitute
				MRDwelling dwelling = MRDwelling.Create();
				dwelling.Parent = testClearing.gameObject.transform;
				dwelling.Type = ((MRWarningChit)chit).Substitute;
				testClearing.AddPieceToBottom(dwelling);
				mMapChits.Remove((MRMapChit)chit);
				DestroyObject(chit);
			}
			else
			{
				int clearingNumber = -1;
				if (chit is MRSiteChit)
				{
					clearingNumber = ((MRSiteChit)chit).ClearingNumber;
				}
				else if (chit is MRSoundChit)
				{
					clearingNumber = ((MRSoundChit)chit).ClearingNumber;
				}
				if (clearingNumber >= 1)
				{
					MRClearing clearing = FrontSide.GetClearing(clearingNumber);
					if (clearing != null)
						clearing.AddPieceToTop(chit);
				}
			}
		}
	}

	/// <summary>
	/// Connects the roads on this tile to the ajacent tiles.
	/// </summary>
	public void ConnectRoads()
	{
		for (int edge = 0; edge < 6; ++edge)
		{
			MRClearing clearing = GetClearingForEdge(edge);
			if (clearing != null)
			{
				foreach (MRRoad road in clearing.Roads)
				{
					if (road.edgeConnection != null && road.clearingConnection1 == null)
					{
						MRTile adjacent = mAdjacentTile[edge];
						if (adjacent != null)
						{
							for (int otherEdge = 0; otherEdge < 6; ++otherEdge)
							{
								if (adjacent.GetAdjacentTile(otherEdge) == this)
								{
									road.clearingConnection1 = adjacent.GetClearingForEdge(otherEdge);
									if (road.clearingConnection1 == null)
									{
										Debug.LogError("No connection from tile " + adjacent.name + " to " + name);
									}
									break;
								}
							}
						}
						break;
					}
				}
			}
		}
	}

	public bool Load(JSONObject root)
	{
		if (root == null)
			return false;

		int id = ((JSONNumber)root["id"]).IntValue;
		if (id != (int)mId)
			return false;

		// read the position and orientation of the tile
		Front = (MRTileSide.eType)((JSONNumber)root["front"]).IntValue;
		Facing = ((JSONNumber)root["facing"]).IntValue;
		JSONArray adjacent = (JSONArray)root["adjacent"];
		for (int i = 0; i < 6; ++i)
		{
			if (adjacent[i] is JSONNumber)
			{
				int adjacentId = ((JSONNumber)adjacent[i]).IntValue;
				MRTile tile = MRGame.TheGame.TheMap[(MRMap.eTileNames)adjacentId];
				if (tile != null)
					mAdjacentTile[i] = tile;
				else
				{
					Debug.LogError("Load tile " + mId + ": no adjacent tile " + adjacentId);
					return false;
				}
			}
		}

		JSONArray pos = (JSONArray)root["pos"];
		transform.position = new Vector3(((JSONNumber)pos[0]).FloatValue,
		                                 ((JSONNumber)pos[1]).FloatValue,
		                                 ((JSONNumber)pos[2]).FloatValue);
		transform.localRotation = Quaternion.identity;
		transform.Rotate(new Vector3(0, 0, 60.0f * mFacing));

		// get the map chits
		JSONArray chits = (JSONArray)root["chits"];
		for (int i = 0; i < chits.Count; ++i)
		{
			JSONObject chitData = (JSONObject)chits[i];
			if (chitData == null)
				return false;
			MRMapChit chit = MRMapChit.Create(chitData);
			if (chit == null)
				return false;
			AddMapChit(chit);
		}

		return true;
	}

	public void Save(JSONObject root)
	{
		// game data
		root["id"] = new JSONNumber((int)mId);
		root["set"] = new JSONNumber(1);		// this is in anticipation of multi-set boards
		root["front"] = new JSONNumber((int)mFront);
		root["facing"] = new JSONNumber(mFacing);
		JSONArray adjacent = new JSONArray(6);
		for (int i = 0; i < 6; ++i)
		{
			if (mAdjacentTile[i] != null)
				adjacent[i] = new JSONNumber((int)mAdjacentTile[i].Id);
		}
		root["adjacent"] = adjacent;

		// transform data
		JSONArray pos = new JSONArray(3);
		pos[0] = new JSONNumber(transform.position.x);
		pos[1] = new JSONNumber(transform.position.y);
		pos[2] = new JSONNumber(transform.position.z);
		root["pos"] = pos;

		// chits
		JSONArray chits = new JSONArray(mMapChits.Count);
		for (int i = 0; i < mMapChits.Count; ++i)
		{
			JSONObject chitData = new JSONObject();
			mMapChits[i].Save(chitData);
			chits[i] = chitData;
		}
		root["chits"] = chits;

	}

	#endregion

	#region Members

	private MRMap.eTileNames mId;
	private MRTileSide.eType mFront;
	private int mFacing;
	private float mSize;
	private Dictionary<MRTileSide.eType, MRTileSide> mSides = new Dictionary<MRTileSide.eType, MRTileSide>();
	private MRTile[] mAdjacentTile = new MRTile[6];
	private MRUtility.IntVector2 mCoordinate;
	private IList<MRMapChit> mMapChits = new List<MRMapChit>();
	private bool mInitialized;
	private Camera mMapCamera;

	#endregion

}
