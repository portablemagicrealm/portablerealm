//
// MRMonsterChartLocation.cs
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

public class MRMonsterChartLocation : MonoBehaviour
{
	#region Properties

	public int row;
	public string stackName;
	public MRGame.eViews view;

	public MRGamePieceStack Occupants
	{
		get{
			return mOccupants;
		}

		set{
			mOccupants = value;
			mOccupants.Layer = gameObject.layer;
			if (mLocationMarker != null)
			{
				mOccupants.gameObject.transform.parent = mLocationMarker.transform;
				mOccupants.gameObject.transform.position = mLocationMarker.transform.position;
				mOccupants.gameObject.transform.localScale = Vector3.one;
			}	
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		foreach (Camera camera in Camera.allCameras)
		{
			if ((camera.cullingMask & (1 << gameObject.layer)) != 0)
			{
				mCamera = camera;
				break;
			}
		}
		if (mCamera == null)
		{
			Debug.LogError("No camera found for monster stack " + stackName);
		}
		mInspectionToggle = false;
		mLocationMarker = gameObject.GetComponentInChildren<SpriteRenderer>().gameObject;
		mCollider = mLocationMarker.gameObject.GetComponent<Collider2D>();
		TextMesh text = gameObject.GetComponentInChildren<TextMesh>();
		if (text != null)
			mName = text.text;
		else
			mName = "monsters";
		if (mOccupants != null)
		{
			mOccupants.gameObject.transform.parent = mLocationMarker.transform;
			mOccupants.gameObject.transform.position = mLocationMarker.transform.position;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (MRGame.TheGame.CurrentView != view || mOccupants == null)
			return;

		//something weird going on with scaling, this hack fixes it for now
		if (mOccupants != null)
		{
			if (mOriginalScale == Vector3.zero)
				mOriginalScale = mOccupants.gameObject.transform.localScale;
			if (!mOccupants.Inspecting)
				mOccupants.gameObject.transform.localScale = Vector3.one;
			else
				mOccupants.gameObject.transform.localScale = mOriginalScale;
		}

		// see if we are being selected for inspection
		if (MRGame.IsTouchHeld && !mInspectionToggle && mOccupants.Pieces.Count > 0)
		{
			Vector3 worldTouch = mCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane));
			RaycastHit2D[] hits = Physics2D.RaycastAll(worldTouch, Vector2.zero);
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider == mCollider)
				{
					Debug.Log("Occupants inspected: " + mName);
					mInspectionToggle = true;
					if (!mOccupants.Inspecting)
						MRGame.TheGame.InspectStack(mOccupants);
					else
						MRGame.TheGame.InspectStack(null);
					break;
				}
			}
		}
		else if (!MRGame.IsTouchHeld && mInspectionToggle)
		{
			mInspectionToggle = false;
		}
	}

	#endregion

	#region Members

	private MRGamePieceStack mOccupants;
	private GameObject mLocationMarker;
	private Collider2D mCollider;
	private Camera mCamera;
	private string mName;
	private bool mInspectionToggle;
	private Vector3 mOriginalScale = Vector3.zero;

	#endregion
}

