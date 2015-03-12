//
// MRClearing.cs
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

public class MRClearing : MonoBehaviour, MRILocation
{

	public enum eType
	{
		Woods,
		Cave,
		Mountain,
	}

	#region Properties

	public int clearingNumber;
	public eType type;

	public GameObject Owner
	{
		get{
			return gameObject;
		}
	}

	public uint Id
	{
		get{
			return MRUtility.IdForName(Name);
		}
	}

	public string Name
	{
		get	{
			return MyTileSide.ShortName + (clearingNumber + 1);
		}
	}

	public ICollection<MRRoad> Roads
	{
		get	{
			return mRoads;
		}
	}

	public MRTileSide MyTileSide
	{
		get	{
			return mMyTileSide;
		}
		set	{
			mMyTileSide = value;
		}
	}

	public MRGamePieceStack Pieces
	{
		get {
			return mPieces;
		}
	}

	public MRGamePieceStack AbandonedItems
	{
		get {
			return mAbandonedItems;
		}
	}

	#endregion

	#region Methods

	// Called when the script instance is being loaded
	void Awake()
	{
		mPieces = gameObject.AddComponent<MRGamePieceStack>();
		mAbandonedItems = gameObject.AddComponent<MRGamePieceStack>();

		MRGame.TheGame.AddClearing(this);
	}

	// Use this for initialization
	void Start () 
	{
		renderer.enabled = false;

		mPieces.Layer = LayerMask.NameToLayer("Map");
		mPieces.transform.parent = transform;
		mPieces.transform.position = transform.position;

		mAbandonedItems.Layer = LayerMask.NameToLayer("Map");
		mAbandonedItems.transform.parent = transform;
		mAbandonedItems.transform.position = transform.position;

		mMapCamera = MRGame.TheGame.TheMap.MapCamera;
	}

	void OnDestroy()
	{
		Debug.LogWarning("Destroy clearing " + Name);
		MRGame.TheGame.RemoveClearing(this);
	}

	// Update is called once per frame
	void Update () 
	{
		if (MRGame.TheGame.CurrentView != MRGame.eViews.Map)
			return;

		if (mMyTileSide.Tile.Front == mMyTileSide.type && MRGame.IsDoubleTapped)
		{
			Vector3 worldTouch = mMapCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mMapCamera.nearClipPlane));
			RaycastHit2D hit = Physics2D.Raycast(worldTouch, Vector2.zero);
			if (hit.collider == collider2D)
			{
				Debug.Log("Clearing selected: " + Name);
				SendMessageUpwards("OnClearingSelectedGame", this, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	// Returns the road connecting this clearing to another clearing, or null if the clearings aren't connected.
	public MRRoad RoadTo(MRClearing clearing)
	{
		foreach (MRRoad road in mRoads)
		{
			if ((road.clearingConnection0 == this && road.clearingConnection1 == clearing) ||
			    (road.clearingConnection1 == this && road.clearingConnection0 == clearing))
			{
				return road;
			}
		}
		return null;
	}

	public void AddRoad(MRRoad road)
	{
		mRoads.Add(road);
	}

	public bool Load(JSONObject root)
	{
		if (Id != ((JSONNumber)root["id"]).UintValue)
			return false;
		if (!mPieces.Load((JSONObject)root["pieces"]))
			return false;
		if (!mAbandonedItems.Load((JSONObject)root["items"]))
			return false;
		return true;
	}

	public void Save(JSONObject root)
	{
		root["id"] = new JSONNumber(Id);
		root["set"] = new JSONNumber(1);		// this is in anticipation of multi-set boards
		JSONObject pieces = new JSONObject();
		mPieces.Save(pieces);
		root["pieces"] = pieces;
		JSONObject items = new JSONObject();
		mAbandonedItems.Save(items);
		root["items"] = items;
	}

	#endregion

	#region Members

	private ICollection<MRRoad> mRoads = new List<MRRoad>();
	private MRTileSide mMyTileSide;
	private Camera mMapCamera;
	private MRGamePieceStack mPieces;
	private MRGamePieceStack mAbandonedItems;

	#endregion
}
