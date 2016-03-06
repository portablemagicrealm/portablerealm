//
// MRViewButton.cs
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

public class MRViewButton : MRButton
{
	#region Constants

	public static readonly Color SELECTED_COLOR = new Color(255f / 255f, 203f / 255f, 15f / 255f);
	public static readonly Color UNSELECTED_COLOR = new Color(190f / 255f, 152f / 255f, 11f / 255f);

	#endregion

	#region Properties

	public MRGame.eViews id;

	public bool Selected
	{
		get{
			return mSelected;
		}

		set{
			mSelected = value;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		foreach (Camera camera in Camera.allCameras)
		{
			if (camera.name == "Inspection Camera")
			{
				mCamera = camera;
				break;
			}
		}

		SpriteRenderer backgroundRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
		Bounds bounds = backgroundRenderer.bounds;

		float screenHeight = MRGame.TheGame.InspectionArea.InspectionBoundsPixels.height;
		float wantedButtonHeight = screenHeight / MRGame.ViewTabCount;
		float boundsHeight = mCamera.WorldToScreenPoint(new Vector3(0, bounds.max.y, 0)).y -
			mCamera.WorldToScreenPoint(new Vector3(0, bounds.min.y, 0)).y;
		float yScale = wantedButtonHeight / boundsHeight;
		mBackground.transform.localScale = new Vector3(1f, yScale, 1f);

		Vector3 desiredWorldPos = mCamera.ScreenToWorldPoint(new Vector3(0, wantedButtonHeight * (int)id, 0));
		gameObject.transform.position = new Vector3(desiredWorldPos.x + bounds.extents.x, 
		                                            desiredWorldPos.y + bounds.extents.y * yScale, 
		                                            gameObject.transform.position.z);

		mSelected = false;
	}

	// Update is called once per frame
	public override void Update ()
	{
		if (mBackground != null)
		{
			if (!mTouched)
			{
				if (mSelected)
					mBackground.GetComponent<SpriteRenderer>().color = SELECTED_COLOR;
				else
					mBackground.GetComponent<SpriteRenderer>().color = UNSELECTED_COLOR;
			}
			else
				mBackground.GetComponent<SpriteRenderer>().color = COLOR_PRESSED;
		}
	}

	public override bool OnTouched(GameObject touchedObject)
	{
		base.OnTouched(touchedObject);
		return true;
	}

	public override bool OnReleased(GameObject touchedObject)
	{
		base.OnReleased(touchedObject);
		return true;
	}

	public override bool OnSingleTapped(GameObject touchedObject)
	{
		base.OnSingleTapped(touchedObject);
		if (touchedObject == mBackground)
		{
			Debug.Log("Tab selected: " + id);
			SendMessageUpwards("OnViewButtonSelectedGame", this, SendMessageOptions.DontRequireReceiver);
		}
		return true;
	}

	public override bool OnDoubleTapped(GameObject touchedObject)
	{
		base.OnDoubleTapped(touchedObject);
		return true;
	}

	public override bool OnTouchHeld(GameObject touchedObject)
	{
		base.OnTouchHeld(touchedObject);
		return true;
	}

	#endregion

	#region Members

	private bool mSelected;
	private Camera mCamera;

	#endregion
}

