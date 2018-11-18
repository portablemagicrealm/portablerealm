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

namespace PortableRealm
{
	
public class MRClearing : MonoBehaviour, MRILocation, MRITouchable
{
	#region Constants

	public enum eType
	{
		Woods,
		Cave,
		Mountain,
	}

	#endregion

	#region Properties

	public int clearingNumber;
	public eType type;

	// MRILocation properties

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

	public virtual IList<MRGame.eMagicColor> MagicSupplied
	{
		get{
			HashSet<MRGame.eMagicColor> magicSupplied = new HashSet<MRGame.eMagicColor>();
			foreach (var color in MRGame.TheGame.WorldMagic)
			{
				magicSupplied.Add(color);
			}
			foreach (var color in MyTileSide.MagicSupplied)
			{
				magicSupplied.Add(color);
			}
			// borderlands supplies magic on a per-clearing basis
			// @todo make this data driven
			if (MyTileSide.Tile.Id == MRMap.eTileNames.borderland && MyTileSide.type == MRTileSide.eType.Enchanted)
			{
				switch (clearingNumber)
				{
					case 1:
						magicSupplied.Add(MRGame.eMagicColor.Grey);
						break;
					case 2:
					case 3:
						magicSupplied.Add(MRGame.eMagicColor.Gold);
						break;
					case 4:
					case 5:
						magicSupplied.Add(MRGame.eMagicColor.Purple);
						break;
					case 6:
						magicSupplied.Add(MRGame.eMagicColor.Grey);
						magicSupplied.Add(MRGame.eMagicColor.Gold);
						magicSupplied.Add(MRGame.eMagicColor.Purple);
						break;
					default:
						Debug.LogError("MagicSupplied invalid Borderlands clearing");
						break;
				}
			}
			foreach (var item in mPieces.Pieces)
			{
				if (item is MRIColorSource)
				{
					foreach (var color in ((MRIColorSource)item).MagicSupplied)
					{
						magicSupplied.Add(color);
					}
				}
			}
			return new List<MRGame.eMagicColor>(magicSupplied);
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
		GetComponent<Renderer>().enabled = false;

		mPieces.Layer = LayerMask.NameToLayer("Map");
		mPieces.transform.parent = transform;
		mPieces.transform.position = transform.position;
		mPieces.StackScale = 1.0f / transform.localScale.x;
		mPieces.Name = Name.Substring(1);

		mAbandonedItems.Layer = LayerMask.NameToLayer("Map");
		mAbandonedItems.transform.parent = transform;
		mAbandonedItems.transform.position = transform.position;
		mPieces.Name = Name.Substring(1);

		mMapCamera = MRGame.TheGame.TheMap.MapCamera;
	}

	void OnDestroy()
	{
		//Debug.LogWarning("Destroy clearing " + Name);
		MRGame.TheGame.RemoveClearing(this);
	}

	// Update is called once per frame
	void Update () 
	{
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
			Debug.Log("Clearing selected: " + Name);
			SendMessageUpwards("OnClearingSelectedGame", this, SendMessageOptions.DontRequireReceiver);
		}
		return true;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		if (mMyTileSide.Tile.Front == mMyTileSide.type && mPieces.Count > 0)
		{
			if (!mPieces.Inspecting)
				MRGame.TheGame.InspectStack(mPieces);
			else
				MRGame.TheGame.InspectStack(null);
		}
		return true;
	}

	public bool OnTouchMove(GameObject touchedObject, float delta_x, float delta_y)
	{
		return true;
	}

	public bool OnButtonActivate(GameObject touchedObject)
	{
		return true;
	}

	public bool OnPinchZoom(float pinchDelta)
	{
		MRGame.TheGame.TheMap.OnPinchZoom(pinchDelta);
		return true;
	}

	/// <summary>
	/// Returns the road connecting this location to another location, or null if the locations aren't connected.
	/// </summary>
	/// <returns>The road.</returns>
	/// <param name="clearing">Clearing.</param>
	public MRRoad RoadTo(MRILocation target)
	{
		foreach (MRRoad road in mRoads)
		{
			if ((road.clearingConnection0 == this && road.clearingConnection1 != null && road.clearingConnection1.Id == target.Id) ||
			    (road.clearingConnection1 == this && road.clearingConnection0 != null && road.clearingConnection0.Id == target.Id))
			{
				return road;
			}
		}
		return null;
	}

	/// <summary>
	/// Adds a piece to the top of the location.
	/// </summary>
	/// <param name="piece">the piece</param>
	public void AddPieceToTop(MRIGamePiece piece)
	{
		Pieces.AddPieceToTop(piece);
	}

	/// <summary>
	/// Adds a piece to the bottom of the location.
	/// </summary>
	/// <param name="piece">the piece</param>
	public void AddPieceToBottom(MRIGamePiece piece)
	{
		Pieces.AddPieceToBottom(piece);
	}

	/// <summary>
	/// Removes a piece from the location.
	/// </summary>
	/// <param name="piece">the piece to remove</param>
	public void RemovePiece(MRIGamePiece piece)
	{
		Pieces.RemovePiece(piece);
	}

	public void AddRoad(MRRoad road)
	{
		mRoads.Add(road);
	}

	public bool Load(JSONObject root)
	{
		if (root["id"] != null)
		{
			if (Id != ((JSONNumber)root["id"]).UintValue)
				return false;
		}
		else
		{
			// todo: check for tile and clearing number
		}
		if (root["pieces"] != null)
		{
			if (!mPieces.Load((JSONObject)root["pieces"]))
				return false;
		}
		if (root["items"] != null)
		{
			if (!mAbandonedItems.Load((JSONObject)root["items"]))
				return false;
		}
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

}