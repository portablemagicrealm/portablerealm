//
// MRGamePieceStack.cs
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

/// <summary>
/// A stack of pieces. Responsible for setting the pieces positions and layer.
/// </summary>
public class MRGamePieceStack : MonoBehaviour, MRISerializable
{
	#region Properties

	public IList<MRIGamePiece> Pieces
	{
		get{
			return mPieces;
		}
	}

	public int Layer
	{
		get{
			return mLayer;
		}

		set{
			if (value >= 0 && value < 32)
			{
				mLayer = value;
				gameObject.layer = value;
				// set the layer of our pieces
				foreach (MRIGamePiece piece in mPieces)
					piece.Layer = mLayer;
			}
			else
			{
				Debug.LogError("Trying to set game piece stack to layer " + value);
			}
		}
	}

	public bool Inspecting
	{
		get{
			return mInspecting;
		}

		set{
			if (value != mInspecting)
			{
				mInspecting = value;
				mInspectionOffset = 0;
				foreach (MRIGamePiece piece in mPieces)
				{
					if (mInspecting)
					{
						SetPieceInspectionPos(piece);
					}
					else
					{
						SetPieceBasePos(piece);
					}
				}
			}
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// Called when the script is loaded.
	/// </summary>
	void Awake()
	{
		mInspecting = false;
	}

	/// <summary>
	/// Called when the script is enabled, before the first Update call.
	/// </summary>
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
		// adjust piece position
		if (mInspecting)
		{
			// draw the pieces in the inspection area
			if (MRGame.IsTouching)
			{
				// adjust the top offset for the user dragging the view
				Camera inspectionCamera = MRGame.TheGame.InspectionArea.InspectionCamera;
				Vector3 worldTouch = inspectionCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, inspectionCamera.nearClipPlane));
				if (MRGame.TheGame.InspectionArea.InspectionBoundsWorld.Contains(worldTouch))
				{
					if (MRGame.JustTouched)
						mLastTouchPos = MRGame.LastTouchPos;
					else
					{
						//Vector2 deltaTouch = MRGame.LastTouchPos - mLastTouchPos;
						mInspectionOffset += worldTouch.y - inspectionCamera.ScreenToWorldPoint(new Vector3(mLastTouchPos.x, mLastTouchPos.y, inspectionCamera.nearClipPlane)).y;
						mLastTouchPos = MRGame.LastTouchPos;
					}
				}
			}

			SetInspectionPositions();
		}
		else
		{
			// stack the pieces
			float zpos = transform.position.z - 0.1f;
			for (int i = mPieces.Count - 1; i >= 0; --i)
			{
				MRIGamePiece piece = mPieces[i];
				piece.Position = new Vector3(transform.position.x, transform.position.y, zpos);
				// if the piece is a MonoBehavior, it will be updated by the system
				if (!(piece is MonoBehaviour))
					piece.Update();
				zpos -= 0.2f;
			}
		}

		if (MRGame.IsDoubleTapped && (mInspecting || mPieces.Count == 1))
		{
			// see if a piece was selected
			Camera camera = null;
			if (mInspecting)
				camera = MRGame.TheGame.InspectionArea.InspectionCamera;
			else
			{
				foreach (Camera testCamera in Camera.allCameras)
				{
					if ((testCamera.cullingMask & (1 << gameObject.layer)) != 0)
					{
						camera = testCamera;
						break;
					}
				}
			}
			if (camera != null)
			{
				Vector3 worldTouch = camera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, camera.nearClipPlane));
				for (int i = 0; i < mPieces.Count; ++i)
				{
					MRIGamePiece piece = mPieces[i];
					Bounds bounds = piece.Bounds;
					if (worldTouch.x - piece.Position.x >= bounds.min.x * piece.LocalScale.x && 
					    worldTouch.x - piece.Position.x <= bounds.max.x * piece.LocalScale.x && 
					    worldTouch.y - piece.Position.y >= bounds.min.y * piece.LocalScale.y && 
					    worldTouch.y - piece.Position.y <= bounds.max.y * piece.LocalScale.y)
					{
						Debug.Log ("Select piece " + piece.Name);
						SendMessageUpwards("OnGamePieceSelectedGame", piece, SendMessageOptions.DontRequireReceiver);
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Adds a piece to the top of the stack.
	/// </summary>
	/// <param name="piece">the piece</param>
	public void AddPieceToTop(MRIGamePiece piece)
	{
		if (piece.Stack != null)
			piece.Stack.RemovePiece(piece);
		piece.Stack = this;
		mPieces.Insert(0, piece);
		piece.OldScale = piece.LocalScale;
		SetPieceBasePos(piece);
		if (Inspecting)
			SetPieceInspectionPos(piece);
	}

	/// <summary>
	/// Adds a piece to the bottom of the stack.
	/// </summary>
	/// <param name="piece">the piece</param>
	public void AddPieceToBottom(MRIGamePiece piece)
	{
		if (piece.Stack != null)
			piece.Stack.RemovePiece(piece);
		piece.Stack = this;
		mPieces.Insert(mPieces.Count, piece);
		piece.OldScale = piece.LocalScale;
		SetPieceBasePos(piece);
		if (Inspecting)
			SetPieceInspectionPos(piece);
	}

	/// <summary>
	/// Adds a piece at a given position in the stack.
	/// </summary>
	/// <param name="piece">the piece</param>
	/// <param name="index">position in the stack to add it</param>
	public void AddPieceAt(MRIGamePiece piece, int index)
	{
		if (piece.Stack != null)
			piece.Stack.RemovePiece(piece);
		piece.Stack = this;
		if (index < 0)
			index = 0;
		else if (index > mPieces.Count)
			index = mPieces.Count;
		mPieces.Insert(index, piece);
		piece.OldScale = piece.LocalScale;
		SetPieceBasePos(piece);
		if (Inspecting)
			SetPieceInspectionPos(piece);
	}

	/// <summary>
	/// Adds a piece before (over) a given piece. If the target piece is null or not in the stack, 
	/// the priece will be added at the top of the stack.
	/// </summary>
	/// <param name="piece">the piece</param>
	/// <param name="target">piece the piece will be placed under</param>
	public void AddPieceBefore(MRIGamePiece piece, MRIGamePiece target)
	{
		if (piece.Stack != null)
			piece.Stack.RemovePiece(piece);
		piece.Stack = this;
		int index = 0;
		if (target != null)
		{
			index = mPieces.IndexOf(target);
			if (index < 0)
				index = 0;
		}
		mPieces.Insert(index, piece);
		piece.OldScale = piece.LocalScale;
		SetPieceBasePos(piece);
		if (Inspecting)
			SetPieceInspectionPos(piece);
	}

	/// <summary>
	/// Adds a piece after (under) a given piece. If the target piece is null or not in the stack, 
	/// the priece will be added at the top of the stack.
	/// </summary>
	/// <param name="piece">the piece</param>
	/// <param name="target">piece the piece will be placed under</param>
	public void AddPieceAfter(MRIGamePiece piece, MRIGamePiece target)
	{
		if (piece.Stack != null)
			piece.Stack.RemovePiece(piece);
		piece.Stack = this;
		int index = 0;
		if (target != null)
		{
			index = mPieces.IndexOf(target) + 1;
			if (index < 1)
				index = 0;
		}
		mPieces.Insert(index, piece);
		piece.OldScale = piece.LocalScale;
		SetPieceBasePos(piece);
		if (Inspecting)
			SetPieceInspectionPos(piece);
	}

	/// <summary>
	/// Removes a piece from the stack.
	/// </summary>
	/// <param name="piece">the piece to remove</param>
	public void RemovePiece(MRIGamePiece piece)
	{
//		SetPieceBasePos(piece);
		piece.OldScale = Vector3.one;
		piece.LocalScale = Vector3.one;
		mPieces.Remove(piece);
		piece.Layer = LayerMask.NameToLayer("Dummy");
		piece.Stack = null;
	}

	/// <summary>
	/// Removes all game pieces from this stack.
	/// </summary>
	public void Clear()
	{
		foreach (MRIGamePiece piece in mPieces)
		{
			SetPieceBasePos(piece);
			piece.Stack = null;
		}
		mPieces.Clear();
	}

	private void SetPieceBasePos(MRIGamePiece piece)
	{
		piece.Layer = mLayer;
		piece.Parent = transform;
		piece.LocalScale = piece.OldScale;
		piece.Rotation = Quaternion.identity;
	}

	private void SetPieceInspectionPos(MRIGamePiece piece)
	{
		piece.Layer = LayerMask.NameToLayer("InspectionList");
		piece.OldScale = new Vector3(piece.LocalScale.x, piece.LocalScale.y, 1f);
		float inspectionScale = 2.0f / piece.Parent.localScale.x;
		piece.LocalScale = new Vector3(inspectionScale, inspectionScale, 1f);
	}

	private void SetInspectionPositions()
	{
		if (mInspectionOffset < 0)
			mInspectionOffset = 0;
		
		// display the pieces in the inspection area
		Camera inspectionCamera = MRGame.TheGame.InspectionArea.InspectionCamera;
		float pieceTopPos = MRGame.TheGame.InspectionArea.InspectionBoundsWorld.yMax;
		float spacing = pieceTopPos - inspectionCamera.ScreenToWorldPoint(new Vector3(0, MRGame.TheGame.InspectionArea.InspectionBoundsPixels.height - 15, 0)).y;
		for (int i = 0; i < mPieces.Count; ++i)
		{
			MRIGamePiece piece = mPieces[i];
			Bounds bounds = piece.Bounds;
			piece.Position = new Vector3(0, mInspectionOffset + pieceTopPos - bounds.extents.y * piece.LossyScale.y, 0.05f);
			pieceTopPos -= bounds.size.y * piece.LocalScale.y + spacing;
		}

		// if we're scrolling, make sure the bottom piece isn't too high
		pieceTopPos += mInspectionOffset;
		if (mInspectionOffset > 0 && pieceTopPos > MRGame.TheGame.InspectionArea.InspectionBoundsWorld.yMin)
		{
			mInspectionOffset -= (pieceTopPos - MRGame.TheGame.InspectionArea.InspectionBoundsWorld.yMin);
			SetInspectionPositions();
		}
	}

	public bool Load(JSONObject root)
	{
		JSONArray pieces = (JSONArray)root["pieces"];
		for (int i = 0; i < pieces.Count; ++i)
		{
			uint id = ((JSONNumber)pieces[i]).UintValue;
			MRIGamePiece piece = MRGame.TheGame.GetGamePiece(id);
			if (piece != null)
			{
				AddPieceToBottom(piece);
			}
			else
			{
				Debug.LogError("Game piece stack no piece for id " + id);
				return false;
			}
		}

		return true;
	}
	
	public void Save(JSONObject root)
	{
		JSONArray pieces = new JSONArray(mPieces.Count);
		for (int i = 0; i < mPieces.Count; ++i)
		{
			if (mPieces[i].Id != 0)
				pieces[i] = new JSONNumber(mPieces[i].Id);
			else
			{
				Debug.LogError("Trying to save clearing piece with id 0");
			}
		}
		root["pieces"] = pieces;
	}

	#endregion

	#region Members

	// The list of pieces, in order of topmost to bottommost
	private IList<MRIGamePiece> mPieces = new List<MRIGamePiece>();
	private int mLayer;
	private bool mInspecting;
	private float mInspectionOffset;
	private Vector2 mLastTouchPos;

	#endregion
}

