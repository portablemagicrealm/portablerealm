//
// MRTreasureChartLocation.cs
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

public class MRTreasureChartLocation : MonoBehaviour, MRITouchable
{
	#region Properties

	public string stackName;
	public MRGame.eViews view;

	public MRGamePieceStack Treasures
	{
		get{
			return mTreasures;
		}

		set{
			mTreasures = value;
			if (mLocationMarker != null)
			{
				mTreasures.gameObject.transform.parent = mLocationMarker.transform;
				mTreasures.gameObject.transform.position = mLocationMarker.transform.position;
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
			Debug.LogError("No camera found for treasue stack " + stackName);
		}

		mLocationMarker = gameObject.GetComponentInChildren<SpriteRenderer>().gameObject;
		mCollider = mLocationMarker.gameObject.GetComponent<Collider2D>();
		TextMesh text = gameObject.GetComponentInChildren<TextMesh>();
		if (text != null)
			mName = text.text;
		else
			mName = "treasures";
		if (mTreasures != null)
		{
			mTreasures.gameObject.transform.parent = mLocationMarker.transform;
			mTreasures.gameObject.transform.position = mLocationMarker.transform.position;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (MRGame.TheGame.CurrentView != view || mTreasures == null)
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
		mTreasures.OnDoubleTapped(touchedObject);
		return true;
	}
	
	public bool OnTouchHeld(GameObject touchedObject)
	{
		if (mTreasures.Count > 0)
		{
			Debug.Log("Treasures inspected: " + mName);
			if (!mTreasures.Inspecting)
				MRGame.TheGame.InspectStack(mTreasures);
			else
				MRGame.TheGame.InspectStack(null);
		}
		return true;
	}

	#endregion

	#region Members

	private MRGamePieceStack mTreasures;
	private GameObject mLocationMarker;
	private Collider2D mCollider;
	private Camera mCamera;
	private string mName;

	#endregion
}

