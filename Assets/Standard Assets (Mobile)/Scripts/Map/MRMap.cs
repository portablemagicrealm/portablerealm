//
// MRMap.cs
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
using System.Text;
using AssemblyCSharp;

public class MRMap : MonoBehaviour, MRISerializable
{
	#region Constants

	public enum eTileNames
	{
		awfulvalley,
		badvalley,
		borderland,
		cavern,
		caves,
		cliff,
		crag,
		curstvalley,
		darkvalley,
		deepwoods,
		evilvalley,
		highpass,
		ledges,
		lindenwoods,
		maplewoods,
		mountain,
		nutwoods,
		oakwoods,
		pinewoods,
		ruins,
	}

	#endregion

	#region Properties

	public MRTile AwfulValley;
	public MRTile BadValley;
	public MRTile Borderland;
	public MRTile Cavern;
	public MRTile Caves;
	public MRTile Cliff;
	public MRTile Crag;
	public MRTile CurstValley;
	public MRTile DarkValley;
	public MRTile DeepWoods;
	public MRTile EvilValley;
	public MRTile HighPass;
	public MRTile Ledges;
	public MRTile LindenWoods;
	public MRTile MapleWoods;
	public MRTile Mountain;
	public MRTile NutWoods;
	public MRTile OakWoods;
	public MRTile PineWoods;
	public MRTile Ruins;

	public bool MapCreated
	{
		get{
			return mMapCreated;
		}
	}

	public Camera MapCamera
	{
		get{
			return mMapCamera;
		}
	}

	public bool MapZoomed
	{
		get{
			return mMapZoomed;
		}
	}

	public IDictionary<eTileNames, MRTile> MapTiles
	{
		get{
			return mMapTiles;
		}
	}

	public MRTile this[eTileNames id]
	{
		get{
			MRTile tile;
			if (mMapTiles.TryGetValue(id, out tile))
				return tile;
			return null;
		}
	}

	public bool Visible
	{
		get{
			return mMapCamera.enabled;
		}
		
		set{
			mMapCamera.enabled = value;
			MRGame.TheGame.Clock.Visible = value;
		}
	}

	public IDictionary<string, MRRoad> Roads
	{
		get
		{
			return mRoads;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start () 
	{
		foreach (Camera camera in Camera.allCameras)
		{
			if (camera.name == "Map Camera")
			{
				mMapCamera = camera;
				break;
			}
		}
		mMapZoomed = false;
		mMapCreated = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!MapCreated)
			return;

		if (!Visible)
			return;

		// test user interaction
		if (MRGame.IsTouching)
		{
			// change screen space to world space
			Vector3 worldTouch = mMapCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mMapCamera.nearClipPlane));
			Vector3 viewportTouch = mMapCamera.ScreenToViewportPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mMapCamera.nearClipPlane));
			if (viewportTouch.x < 0)
				return;
			Vector2 pointTouch = new Vector2(worldTouch.x, worldTouch.y);
			Collider2D[] hits = Physics2D.OverlapPointAll(pointTouch);

			// see if our last position was over a tile
			MRTile touchedTile = null;
			foreach (KeyValuePair<eTileNames, MRTile> pair in mMapTiles)
			{
				Collider2D collider = pair.Value.collider2D;
				if (collider.OverlapPoint(pointTouch))
				{
					touchedTile = pair.Value;
					break;
				}
			}
			if (touchedTile != null)
			{
				if (mMoveStarted)
				{
					worldTouch = mMapCamera.ScreenToWorldPoint(new Vector3(MRGame.TouchPos.x, MRGame.TouchPos.y, 0));
					if (worldTouch.x - pointTouch.x != 0 || 
					    worldTouch.y - pointTouch.y != 0)
					{
						mMapCamera.transform.Translate(new Vector3(pointTouch.x - worldTouch.x, pointTouch.y - worldTouch.y, 0));
					}
				}
				else if (MRGame.JustTouched)
					mMoveStarted = true;
			}
		}
		else
			mMoveStarted = false;
	}

	/// <summary>
	/// Creates a new random map.
	/// </summary>
	public void CreateMap()
	{
		if (mMapCreated)
			return;

		// set up the tile pool
		CreateTilePool ();

		if (mMapTiles.Count == 0)
		{
			while (!BuildMap()) {}
			PlaceMapChits();
		}

		mMapCreated = true;
	}

	private void CreateTilePool()
	{
		mTilePool.Add(eTileNames.awfulvalley, AwfulValley);
		mTilePool.Add(eTileNames.badvalley, BadValley);
		mTilePool.Add(eTileNames.borderland, Borderland);
		mTilePool.Add(eTileNames.cavern, Cavern);
		mTilePool.Add(eTileNames.caves, Caves);
		mTilePool.Add(eTileNames.cliff, Cliff);
		mTilePool.Add(eTileNames.crag, Crag);
		mTilePool.Add(eTileNames.curstvalley, CurstValley);
		mTilePool.Add(eTileNames.darkvalley, DarkValley);
		mTilePool.Add(eTileNames.deepwoods, DeepWoods);
		mTilePool.Add(eTileNames.evilvalley, EvilValley);
		mTilePool.Add(eTileNames.highpass, HighPass);
		mTilePool.Add(eTileNames.ledges, Ledges);
		mTilePool.Add(eTileNames.lindenwoods, LindenWoods);
		mTilePool.Add(eTileNames.maplewoods, MapleWoods);
		mTilePool.Add(eTileNames.mountain, Mountain);
		mTilePool.Add(eTileNames.nutwoods, NutWoods);
		mTilePool.Add(eTileNames.oakwoods, OakWoods);
		mTilePool.Add(eTileNames.pinewoods, PineWoods);
		mTilePool.Add(eTileNames.ruins, Ruins);
	}

	/// <summary>
	/// Creates a random map from the tile set.
	/// </summary>
	/// <returns><c>true</c>, if map was built, <c>false</c> otherwise.</returns>
	private bool BuildMap()
	{
		foreach (MRTile tile in mMapTiles.Values)
		{
			Destroy(tile.gameObject);
		}
		mMapTiles.Clear();

		// create the borderland
		MRTile tilePrototype;
		mTilePool.TryGetValue(eTileNames.borderland, out tilePrototype);
		MRTile borderland = (MRTile)Instantiate(tilePrototype);
		borderland.transform.parent = transform;
		borderland.Id = eTileNames.borderland;
		mMapTiles.Add(eTileNames.borderland, borderland);

		// add the 2nd tile
		IList<eTileNames> tilesLeft = new List<eTileNames>();
		foreach (eTileNames type in Enum.GetValues(typeof(eTileNames)))
		{
			// ledges and highpass can't be the 2nd tile
			if (type != eTileNames.borderland && type != eTileNames.ledges && type != eTileNames.highpass)
				tilesLeft.Add(type);
		}
		tilesLeft.Shuffle();

		eTileNames tileId = tilesLeft[tilesLeft.Count - 1];
		tilesLeft.RemoveAt(tilesLeft.Count - 1);
		if (!mTilePool.TryGetValue(tileId, out tilePrototype))
		{
			Debug.LogError("invalid tile " + tileId);
			Application.Quit();
		}
		MRTile newTile = (MRTile)Instantiate(tilePrototype);
		newTile.transform.parent = transform;
		newTile.Id = tileId;
		if (!newTile.Initialized)
		{
			Debug.LogError("tile " + tileId + " not initialized");
			Application.Quit();
		}
		mMapTiles.Add(tileId, newTile);
		int fromSide, toSide;
		int count = 20;
		do
		{
			--count;
			fromSide = MRRandom.Range(0, 5);
		} while (!newTile.Edges[fromSide] && count > 0);
		if (count == 0)
		{
			Debug.LogError("no edges for tile " + tileId);
			Application.Quit();
		}
		toSide = MRRandom.Range(0, 5);
		Debug.Log("attach tile " + eTileNames.borderland + " side " + toSide + " to tile " + tileId + " side " + fromSide);
		borderland.SetAdjacentTile(newTile, fromSide, toSide, false);

		// add ledges and highpass back in
		tilesLeft.Add(eTileNames.ledges);
		tilesLeft.Add(eTileNames.highpass);
		tilesLeft.Shuffle();

		// add the rest of the tiles
		while (tilesLeft.Count > 0)
		{
			// pick a random tile
			tileId = tilesLeft[tilesLeft.Count - 1];
			tilesLeft.RemoveAt(tilesLeft.Count - 1);
			if (!mTilePool.TryGetValue(tileId, out tilePrototype))
			{
				Debug.LogError("invalid tile " + tileId);
				Application.Quit();
			}
			newTile = (MRTile)Instantiate(tilePrototype);
			newTile.transform.parent = transform;
			newTile.Id = tileId;
			if (!newTile.Initialized)
			{
				Debug.LogError("tile " + tileId + " not initialized");
				Application.Quit();
			}
			// pick a random tile on the map
			bool connected = false;
			int[] targetIndexes = new int[mMapTiles.Count];
			for (int i = 0; i < targetIndexes.Length; ++i)
				targetIndexes[i] = i;
			targetIndexes.Shuffle();
			foreach (int targetIndex in targetIndexes)
			{
				eTileNames targetName = eTileNames.borderland;
				MRTile targetTile = null;
				int i = targetIndex;
				foreach (KeyValuePair<eTileNames, MRTile> iter in mMapTiles)
				{
					if (i-- == 0)
					{
						targetName = iter.Key;
						targetTile = iter.Value;
						break;
					}
				}
				IList<int> targetEdges = targetTile.GetRandomAvailableEdges();
				foreach (int targetEdge in targetEdges)
				{
					// pick a random edge of our tile to attach
					IList<int> roadEdges = newTile.GetRandomizedRoadEdges();
					foreach (int testEdge in roadEdges)
					{
						if (targetTile.TestValidConnection(newTile, testEdge, targetEdge, true) &&
						    targetTile.TestValidConnection(newTile, testEdge, targetEdge, false))
						{
							// connect the tiles
							connected = true;
							Debug.Log("attach tile " + targetName + " side " + targetEdge + " to tile " + tileId + " side " + testEdge);

							targetTile.SetAdjacentTile(newTile, testEdge, targetEdge, false);
							// verify that a tile clearing connects to the borderland
							bool isConnectedToBorderland = false;
							for (i = 0; i < 6; ++i)
							{
								MRClearing testClearing = newTile.FrontSide.Clearings[i];
								if (testClearing == null)
									continue;
								IList<MRClearing> path = Path(borderland.FrontSide.Clearings[0], testClearing);
								if (path != null)
								{
									StringBuilder s = new StringBuilder();
									foreach(MRClearing clearing in path)
									{
										s.Append(clearing.Name);
										s.Append(" ");
									}
									Debug.Log(s);
									// ledges and highpass must have all their clearings connect
									if (i == 5 || !(tileId == eTileNames.ledges || tileId == eTileNames.highpass))
									{
										isConnectedToBorderland = true;
										break;
									}
								}
								else if (tileId == eTileNames.ledges || tileId == eTileNames.highpass)
								{
									// ledges and highpass must have all their clearings connect
									break;
								}
							}
							if (!isConnectedToBorderland)
							{
								// remove the tile for another round
								//Debug.LogWarning("Can't connect tile " + tileId + " to borderland");
								newTile.RemoveFromMap();
								connected = false;
							}
							else
							{
								mMapTiles.Add(tileId, newTile);
								break;
							}
						}
					}
					if (connected)
						break;
				}
				if (connected)
					break;
			}
			if (!connected)
			{
				// put the tile back in the tilesLeft container
				if (tilesLeft.Count == 0)
				{
					Debug.LogError("Unable to complete map");
					return false;
				}
				Debug.LogWarning("Failed to add tile " + tileId);
				Destroy(newTile.gameObject);
				tilesLeft.Add(tileId);
				tilesLeft.Shuffle();
			}
		}
		return true;
	}

	/// <summary>
	/// Returns a list of a path from one clearing to another, or null if there is no path between them.
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	public static IList<MRClearing> Path(MRClearing from, MRClearing to)
	{
		if (from == null || to == null)
		{
			Debug.LogError("Path to nowhere");
			return null;
		}

		//SortedList<float, MRClearing> f_score = new SortedList<float, MRClearing>();
		IDictionary<MRClearing, float> g_score = new Dictionary<MRClearing, float>();
		IDictionary<MRClearing, float> openSet = new Dictionary<MRClearing, float>();
		HashSet<MRClearing> closedSet = new HashSet<MRClearing>();
		IDictionary<MRClearing, MRClearing> cameFrom = new Dictionary<MRClearing, MRClearing>();

		float distance = Vector2.Distance(from.gameObject.transform.position, to.gameObject.transform.position);
		openSet.Add(from, distance);
		g_score.Add(from, 0);
		while (openSet.Count > 0)
		{
			// slow, but we aren't dealing with a big data set
			MRClearing current = null;
			float minDist = float.MaxValue;
			foreach (KeyValuePair<MRClearing, float> item in openSet)
			{
				if (item.Value < minDist)
				{
					minDist = item.Value;
					current = item.Key;
				}
			}
			if (current == to)
			{
				// done
				return UnrollPath(cameFrom, to);
			}
			openSet.Remove(current);
			closedSet.Add(current);
			foreach (MRRoad road in current.Roads)
			{
				MRClearing neighbor = road.clearingConnection0;
				if (neighbor == current)
					neighbor = road.clearingConnection1;
				if (neighbor != null)
				{
					if (!closedSet.Contains(neighbor))
					{
						float gTemp = 0;
						g_score.TryGetValue(current, out gTemp);
						float gNeighbor = 0;
						g_score.TryGetValue(neighbor, out gNeighbor);
						gTemp += Math.Abs(current.gameObject.transform.position.x - neighbor.gameObject.transform.position.x) + 
							Math.Abs(current.gameObject.transform.position.y - neighbor.gameObject.transform.position.y);
						if (!openSet.ContainsKey(neighbor) || gTemp < gNeighbor)
						{
							cameFrom[neighbor] = current;
							g_score[neighbor] = gTemp;
							float fNeighbor = gTemp + Math.Abs(to.gameObject.transform.position.x - neighbor.gameObject.transform.position.x) + 
								Math.Abs(to.gameObject.transform.position.y - neighbor.gameObject.transform.position.y);
							openSet[neighbor] = fNeighbor;
						}
					}
				}
			}
		}

		//Debug.Log("No Path");
		return null;
	}

	/// <summary>
	/// A map path is created in reverse. This recursive function creates a list of clearings with the actual path.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="cameFrom">Came from.</param>
	/// <param name="currentNode">Current node.</param>
	private static IList<MRClearing> UnrollPath(IDictionary<MRClearing, MRClearing> cameFrom, MRClearing currentNode)
	{
		IList<MRClearing> path;
		if (cameFrom.ContainsKey(currentNode))
		{
			MRClearing prevNode;
			cameFrom.TryGetValue(currentNode, out prevNode);
			path = UnrollPath(cameFrom, prevNode);
		}
		else
		{
			path = new List<MRClearing>();
		}
		path.Add(currentNode);
		return path;
	}

	/// <summary>
	/// Create the map chits. They will be assigned to tiles when the map is created.
	/// </summary>
	private void CreateMapChits()
	{
		foreach (MRMapChit.eMapChitType chitType in Enum.GetValues(typeof(MRMapChit.eMapChitType)))
		{
			IList<MRMapChit> chitsForType = new List<MRMapChit>();
			mMapChitsByType.Add(chitType, chitsForType);
			switch (chitType)
			{
				case MRMapChit.eMapChitType.Site:
				{
					foreach (MRMapChit.eSiteChitType siteType in Enum.GetValues(typeof(MRMapChit.eSiteChitType)))
					{
						MRSiteChit chit = (MRSiteChit)MRMapChit.Create(MRMapChit.eMapChitType.Site);
						chit.Parent = transform;
						chit.SiteType = siteType;
						chitsForType.Add(chit);
						mMapChits.Add(chit);
					}
					break;
				}
				case MRMapChit.eMapChitType.Sound:
				{
					foreach (MRMapChit.eSoundChitType soundType in Enum.GetValues(typeof(MRMapChit.eSoundChitType)))
					{
						foreach (int clearingNum in soundType.ClearingNumbers())
						{
							MRSoundChit chit = (MRSoundChit)MRMapChit.Create(MRMapChit.eMapChitType.Sound);
							chit.Parent = transform;
							chit.ClearingNumber = clearingNum;
							chit.SoundType = soundType;
							chitsForType.Add(chit);
							mMapChits.Add(chit);
						}
					}
					break;
				}
				case MRMapChit.eMapChitType.SuperSite:
				{
					foreach (MRMapChit.eSuperSiteChitType siteType in Enum.GetValues(typeof(MRMapChit.eSuperSiteChitType)))
					{
						MRSuperSiteChit chit = (MRSuperSiteChit)MRMapChit.Create(MRMapChit.eMapChitType.SuperSite);
						chit.Parent = transform;
						chit.ClearingNumber = siteType.ClearingNumber();
						chit.SiteType = siteType;
						chitsForType.Add(chit);
						mMapChits.Add(chit);
					}
					break;
				}
				case MRMapChit.eMapChitType.Warning:
				{
					foreach (MRMapChit.eWarningChitType warningType in Enum.GetValues(typeof(MRMapChit.eWarningChitType)))
					{
						foreach (MRTile.eTileType tileType in Enum.GetValues(typeof(MRTile.eTileType)))
						{
							MRWarningChit chit = (MRWarningChit)MRMapChit.Create(MRMapChit.eMapChitType.Warning);
							chit.Parent = transform;
							chit.WarningType = warningType;
							chit.TileType = tileType;
							chitsForType.Add(chit);
							mMapChits.Add(chit);
						}
					}
					break;
				}
			}
		}
	}

	private IList<MRMapChit> GetMapChitsForType(MRMapChit.eMapChitType type)
	{
		IList<MRMapChit> chits;
		mMapChitsByType.TryGetValue(type, out chits);
		return chits;
	}

	/// <summary>
	/// Assigns the map chits to their initial map locations.
	/// </summary>
	private void PlaceMapChits()
	{
		CreateMapChits();

		// create a group of site and sound chits
		IList<MRMapChit> chitList = new List<MRMapChit>();
		foreach (MRMapChit chit in GetMapChitsForType(MRMapChit.eMapChitType.Site))
		{
//			chit.SideUp = MRChit.eSide.Back;
			chitList.Add(chit);
		}
		foreach (MRMapChit chit in GetMapChitsForType(MRMapChit.eMapChitType.Sound))
		{
//			chit.SideUp = MRChit.eSide.Back;
			chitList.Add(chit);
		}
		chitList.Shuffle();
		chitList.Shuffle();

		// put 5 of the mixed chits into each of the lost castle and lost city
		IList<MRMapChit> lostCityList = new List<MRMapChit>();
		IList<MRMapChit> lostCastleList = new List<MRMapChit>();
		foreach (MRMapChit superChit in GetMapChitsForType(MRMapChit.eMapChitType.SuperSite))
		{
//			superChit.SideUp = MRChit.eSide.Back;
			if (((MRSuperSiteChit)superChit).SiteType == MRMapChit.eSuperSiteChitType.LostCastle)
				lostCastleList.Add(superChit);
			else
				lostCityList.Add(superChit);
			for (int i = 0; i < 5; ++i)
			{
				((MRSuperSiteChit)superChit).ContainedChits.Add(chitList[0]);
				chitList[0].Layer = LayerMask.NameToLayer("Dummy");
				chitList.RemoveAt(0);
			}
		}

		// split the remaining sound and site chits into the lost castle and lost city lists
		while (chitList.Count > 0)
		{
			lostCastleList.Add(chitList[0]);
			lostCityList.Add(chitList[1]);
			chitList.RemoveAt(0);
			chitList.RemoveAt(0);
		}
		chitList = null;
		lostCastleList.Shuffle();
		lostCastleList.Shuffle();
		lostCityList.Shuffle();
		lostCityList.Shuffle();

		// assign the lost castle chits to the mountain tiles, and the lost city chits to the cave tiles
		ICollection<MRTile> tiles = mMapTiles.Values;
		foreach (MRTile tile in tiles)
		{
			if (tile.type == MRTile.eTileType.Cave)
			{
				tile.AddMapChit(lostCityList[0]);
				lostCityList.RemoveAt(0);
			}
			else if (tile.type == MRTile.eTileType.Mountain)
			{
				tile.AddMapChit(lostCastleList[0]);
				lostCastleList.RemoveAt(0);
			}
		}
		if (lostCityList.Count > 0)
		{
			Debug.LogError("Not all lost city chits assigned");
		}
		if (lostCastleList.Count > 0)
		{
			Debug.LogError("Not all lost castle chits assigned");
		}
		lostCityList = null;
		lostCastleList = null;

		// assign the warning chits to their tiles
		IDictionary<MRTile.eTileType, IList<MRMapChit>> warningChits = new Dictionary<MRTile.eTileType, IList<MRMapChit>>();
		warningChits.Add(MRTile.eTileType.Cave, new List<MRMapChit>());
		warningChits.Add(MRTile.eTileType.Mountain, new List<MRMapChit>());
		warningChits.Add(MRTile.eTileType.Valley, new List<MRMapChit>());
		warningChits.Add(MRTile.eTileType.Woods, new List<MRMapChit>());
		foreach (MRMapChit chit in GetMapChitsForType(MRMapChit.eMapChitType.Warning))
		{
//			chit.SideUp = MRChit.eSide.Back;
			warningChits[((MRWarningChit)chit).TileType].Add(chit);
		}
		foreach (IList<MRMapChit> chits in warningChits.Values)
		{
			chits.Shuffle();
			chits.Shuffle();
		}

		MRTile borderland;
		mMapTiles.TryGetValue(eTileNames.borderland, out borderland);
		MRClearing borderlandClearing = borderland.FrontSide.GetClearing(1);
		foreach (MRTile tile in tiles)
		{
			MRWarningChit chit = (MRWarningChit)warningChits[tile.type][0];
			warningChits[tile.type].RemoveAt(0);
			if (chit.Substitute == MRDwelling.eDwelling.None ||
			    chit.Substitute == MRDwelling.eDwelling.LargeFire ||
			    chit.Substitute == MRDwelling.eDwelling.SmallFire)
			{
				tile.AddMapChit(chit);
			}
			else
			{
				// replace the warning with its dwelling in clearing 5 or 4
				MRClearing clearing = tile.FrontSide.GetClearing(5);
				// if clearing 5 doesn't connect to the borderland, use clearing 4
				if (Path(clearing, borderlandClearing) == null)
					clearing = tile.FrontSide.GetClearing(4);
				if (clearing != null)
				{
					if (chit.Substitute != MRDwelling.eDwelling.Ghosts)
					{
						//MRDwelling dwelling = clearing.gameObject.AddComponent<MRDwelling>();
						MRDwelling dwelling = MRDwelling.Create();
						dwelling.Parent = clearing.gameObject.transform;
						dwelling.Type = chit.Substitute;
						clearing.Pieces.AddPieceToBottom(dwelling);
					}
					mDwellings[chit.Substitute] = clearing;
				}
				DestroyObject(chit);
			}
		}

		mMapChits.Clear();
		mMapChitsByType.Clear();
	}

	/// <summary>
	/// Returns the clearing that contains a given dwelling.
	/// </summary>
	/// <returns>The clearing for dwelling.</returns>
	/// <param name="dwellingId">Dwelling identifier.</param>
	public MRClearing ClearingForDwelling(MRDwelling.eDwelling dwellingId)
	{
		MRClearing clearing;
		if (!mDwellings.TryGetValue(dwellingId, out clearing))
		{
			Debug.LogError("No clearing for dwelling " + dwellingId + ", returning inn clearing");
			clearing = mDwellings[MRDwelling.eDwelling.Inn];
		}
		return clearing;
	}

	/// <summary>
	/// Called when a tile is selected. Toggles the camera to zoom into the selected tile.
	/// </summary>
	/// <param name="tile">Tile.</param>
	public void OnTileSelected(MRTile tile)
	{
		if (Math.Abs(mMapCamera.orthographicSize - MRGame.MAP_CAMERA_FAR_SIZE) < 0.1f)
		{
			mMapCamera.transform.position = new Vector3(tile.transform.position.x, 
			                                            tile.transform.position.y, 
			                                            mMapCamera.transform.position.z);
			mMapCamera.orthographicSize = MRGame.MAP_CAMERA_NEAR_SIZE;
			mMapZoomed = true;
			Debug.Log("Zoom in");
		}
		else
		{
			mMapCamera.orthographicSize = MRGame.MAP_CAMERA_FAR_SIZE;
			mMapZoomed = false;
			Debug.Log("Zoom out");
		}
	}

	/// <summary>
	/// Does map maintenance during the midnight phase.
	/// </summary>
	public void StartMidnight()
	{
		// reset tile chits summon state
		foreach (MRTile tile in mMapTiles.Values)
		{
			foreach (MRMapChit chit in tile.MapChits)
			{
				chit.SummonedMonsters = false;
			}
		}
	}

	public bool Load(JSONObject root)
	{
		// todo: clean up old map so we can create the new one
		if (mMapCreated)
			return false;

		if (root == null)
			return false;

		// create the base tiles
		CreateTilePool();
		foreach (KeyValuePair<eTileNames, MRTile> pair in mTilePool)
		{
			MRTile tilePrototype = pair.Value;
			MRTile tile = (MRTile)Instantiate(tilePrototype);
			tile.transform.parent = transform;
			tile.Id = pair.Key;
			mMapTiles.Add(tile.Id, tile);
		}
		mTilePool.Clear();

		// initialize the tiles from the json data
		JSONArray tiles = (JSONArray)root["tiles"];
		for (int i = 0; i < tiles.Count; ++i)
		{
			JSONObject tileData = (JSONObject)tiles[i];
			int tileId = ((JSONNumber)tileData["id"]).IntValue;
			MRTile tile = this[(eTileNames)tileId];
			if (tile != null)
			{
				if (!tile.Load(tileData))
					return false;
			}
			else
			{
				Debug.LogError("MRMap.Load: no tile for id " + tileId);
			}
		}
		// connect all the tile roads
		foreach (MRTile tile in mMapTiles.Values)
		{
			tile.ConnectRoads();
		}

		// create the dwelling chits
		foreach (MRDwelling.eDwelling type in Enum.GetValues(typeof(MRDwelling.eDwelling)))
		{
			MRDwelling dwelling = MRDwelling.Create();
			dwelling.Parent = gameObject.transform;
			dwelling.Layer = LayerMask.NameToLayer("Dummy");
			dwelling.Type = type;
		}

		mMapCreated = true;
		return true;
	}

	public void Save(JSONObject root)
	{
		JSONArray tiles = new JSONArray(mMapTiles.Count);
		int i = 0;
		foreach (MRTile tile in mMapTiles.Values)
		{
			JSONObject tileData = new JSONObject();
			tile.Save(tileData);
			tiles[i++] = tileData;
		}
		root["tiles"] = tiles;
	}

	#endregion

	#region Members

	private bool mMapCreated;
	private Camera mMapCamera;
	private bool mMapZoomed;
	private bool mMoveStarted;
	private IDictionary<eTileNames, MRTile> mTilePool = new Dictionary<eTileNames, MRTile>();
	private IDictionary<eTileNames, MRTile> mMapTiles = new Dictionary<eTileNames, MRTile>();
	private IDictionary<string, MRRoad> mRoads = new Dictionary<string, MRRoad>();
	private IDictionary<MRDwelling.eDwelling, MRClearing> mDwellings = new Dictionary<MRDwelling.eDwelling, MRClearing>();

	private IList<MRMapChit> mMapChits = new List<MRMapChit>();
	private IDictionary<MRMapChit.eMapChitType, IList<MRMapChit>> mMapChitsByType = new Dictionary<MRMapChit.eMapChitType, IList<MRMapChit>>();

	#endregion
}
