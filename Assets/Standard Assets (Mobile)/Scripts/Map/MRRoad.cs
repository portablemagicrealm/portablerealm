//
// MRRoad.cs
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

public class MRRoad : MonoBehaviour, MRILocation
{
	#region Constants

	public enum eRoadType
	{
		Road,
		Tunnel,
		HiddenPath,
		SecretPassage,
	}

	#endregion

	#region Properties

	public MRClearing clearingConnection0;
	public MRClearing clearingConnection1;
	public MREdge edgeConnection;
	public eRoadType type;

	public string Name
	{
		get
		{
			if (clearingConnection0 != null && clearingConnection1 != null)
			{
				return clearingConnection0.Name + "-" + clearingConnection1.Name;
			}
			else if (clearingConnection0 != null && edgeConnection != null)
			{
				return clearingConnection0.Name + "-Edge" + edgeConnection.side;
			}
			else
			{
				Debug.LogError("Road with no connections");
				return "unknown";
			}
		}
	}

	// MRILocation properties

	public GameObject Owner 
	{ 
		get {
			return gameObject;
		}
	}
	
	public uint Id 
	{ 
		get {
			return MRUtility.IdForName(Name);
		}
	}
	
	public ICollection<MRRoad> Roads 
	{ 
		get	{
			return new List<MRRoad>{this};
		}
	}
	
	public MRTileSide MyTileSide 
	{ 
		get {
			return mMyTileSide;
		}
		set {
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
		get	{
			return null;
		}
	}

	#endregion

	#region Methods

	// Called when the script instance is being loaded
	void Awake()
	{
		gameObject.SetActive(false);
		enabled = false;

		mPieces = MRGame.TheGame.NewGamePieceStack();
		mPieces.Layer = LayerMask.NameToLayer("Map");
		mPieces.transform.parent = transform;
		mPieces.transform.position = transform.position;

		MRGame.TheGame.AddRoad(this);
	}

	// Use this for initialization
	void Start () 
	{
		try
		{
			MRGame.TheGame.TheMap.Roads[Name] = this;
		}
		catch (Exception err)
		{
			Debug.LogError("Road " + Name + " : " + err.ToString());
		}
	}

	void OnDestroy()
	{
		try
		{
			// remove ourself from the road map
			MRGame.TheGame.RemoveRoad(this);
			MRRoad test;
			if (MRGame.TheGame.TheMap.Roads.TryGetValue(Name, out test))
			{
				if (test == this)
					MRGame.TheGame.TheMap.Roads.Remove(Name);
			}
		}
		catch (Exception err)
		{
			Debug.LogError("Road " + Name + " : " + err.ToString());
		}
	}

	/// <summary>
	/// Returns the road connecting this location to another location, or null if the locations aren't connected.
	/// </summary>
	/// <returns>The road.</returns>
	/// <param name="clearing">Clearing.</param>
	public MRRoad RoadTo(MRILocation target)
	{
		ICollection<MRRoad> targetRoads = target.Roads;
		if (targetRoads.Contains(this))
			return this;
		return null;
	}

	/// <summary>
	/// Adds a piece to the top of the location.
	/// </summary>
	/// <param name="piece">the piece</param>
	public void AddPieceToTop(MRIGamePiece piece)
	{
		Pieces.AddPieceToTop(piece);
		gameObject.SetActive(true);
	}
	
	/// <summary>
	/// Removes a piece from the location.
	/// </summary>
	/// <param name="piece">the piece to remove</param>
	public void RemovePiece(MRIGamePiece piece)
	{
		Pieces.RemovePiece(piece);
		if (Pieces.Count == 0)
			gameObject.SetActive(false);
	}

	public bool Load(JSONObject root)
	{
		if (root["pieces"] != null)
		{
			if (!mPieces.Load((JSONObject)root["pieces"]))
				return false;

			// make sure controllable locations are set
			List<MRControllable> controllables = new List<MRControllable>();
			foreach (MRIGamePiece piece in mPieces.Pieces)
			{
				if (piece is MRControllable)
					controllables.Add((MRControllable)piece);
			}
			foreach (MRControllable controllable in controllables)
			{
				if (controllable.Location == null || controllable.Location.Id != Id)
					controllable.Location = this;
			}

			gameObject.SetActive(true);
		}
		return true;
	}
	
	public void Save(JSONObject root)
	{
		if (mPieces.Count > 0)
		{
			root["name"] = new JSONString(Name);
			JSONObject pieces = new JSONObject();
			mPieces.Save(pieces);
			root["pieces"] = pieces;
		}
	}

	#endregion

	#region Members

	private MRTileSide mMyTileSide;
	private MRGamePieceStack mPieces;

	#endregion
}
