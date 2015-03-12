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

public class MRViewButton : MonoBehaviour
{
	#region Constants

	public static Color selectedColor = new Color(255f / 255f, 203f / 255f, 15f / 255f);
	public static Color unselectedColor = new Color(190f / 255f, 152f / 255f, 11f / 255f);

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
	void Start ()
	{
		foreach (Camera camera in Camera.allCameras)
		{
			if (camera.name == "Inspection Camera")
			{
				mCamera = camera;
				break;
			}
		}

		SpriteRenderer backgroundRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
		mBackground = backgroundRenderer.gameObject;
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
		                                            0);

		mSelected = false;
	}

	// Update is called once per frame
	void Update ()
	{
		if (mSelected)
			mBackground.GetComponent<SpriteRenderer>().color = selectedColor;
		else
			mBackground.GetComponent<SpriteRenderer>().color = unselectedColor;

		if (MRGame.IsSingleTapped)
		{
			Vector3 screenPos = new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane);
			Vector3 viewportTouch = mCamera.ScreenToViewportPoint(screenPos);
			if (viewportTouch.x > 0 && viewportTouch.x < 1 && viewportTouch.y > 0 && viewportTouch.y < 1)
			{
				Vector3 worldTouch = mCamera.ScreenToWorldPoint(screenPos);
				RaycastHit2D[] hits = Physics2D.RaycastAll(worldTouch, Vector2.zero);
				foreach (RaycastHit2D hit in hits)
				{
					if (hit.collider == mBackground.collider2D)
					{
						Debug.Log("Tab selected: " + id);
						SendMessageUpwards("OnViewButtonSelectedGame", this, SendMessageOptions.DontRequireReceiver);
						break;
					}
				}
			}
		}
	}

	#endregion

	#region Members

	private bool mSelected;
	private GameObject mBackground;
	private Camera mCamera;

	#endregion
}
