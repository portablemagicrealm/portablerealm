//
// MRInspectionArea.cs
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

public class MRInspectionArea : MonoBehaviour
{
	#region Properties

	/// <summary>
	/// Returns the area for use in inspection, in world coordinates.
	/// </summary>
	/// <value>The inspection bounds.</value>
	public Rect InspectionBoundsWorld
	{
		get{
			return mInspectionBoundsWorld;
		}
	}

	/// <summary>
	/// Returns the area for use in inspection, in pixels.
	/// </summary>
	/// <value>The inspection bounds.</value>
	public Rect InspectionBoundsPixels
	{
		get{
			return mInspectionBoundsPixels;
		}
	}

	public Camera InspectionCamera
	{
		get{
			return mCamera;
		}
	}

	private string HeaderText
	{
		get{
			return mHeaderText.text;
		}

		set{
			mHeaderText.text = value;
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

		// get the header
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform in transforms)
		{
			if (transform.gameObject.name == "Header")
			{
				mHeader = transform.gameObject;
				mHeaderBackground = mHeader.GetComponentInChildren<SpriteRenderer>().gameObject;
				mHeaderText = mHeader.GetComponentInChildren<TextMesh>();
			}
		}

		// get the base bounds of the inspection area
		Vector3 lowerLeft = mCamera.ViewportToWorldPoint(Vector3.zero);
		Vector3 upperRight = mCamera.ViewportToWorldPoint(Vector3.one);
		mInspectionBoundsWorld.xMin = lowerLeft.x;
		mInspectionBoundsWorld.xMax = upperRight.x;
		mInspectionBoundsWorld.yMin = lowerLeft.y;
		mInspectionBoundsWorld.yMax = upperRight.y;
		lowerLeft = mCamera.ViewportToScreenPoint(Vector3.zero);
		upperRight = mCamera.ViewportToScreenPoint(Vector3.one);
		mInspectionBoundsPixels.xMin = lowerLeft.x;
		mInspectionBoundsPixels.xMax = upperRight.x;
		mInspectionBoundsPixels.yMin = lowerLeft.y;
		mInspectionBoundsPixels.yMax = upperRight.y;

		// adjust the header
		Bounds headerBounds = mHeaderBackground.GetComponentInChildren<SpriteRenderer>().bounds;
		Transform headerPosition = mHeader.transform;
		Transform headerScale = mHeaderBackground.transform;
		mHeader.transform.position = new Vector3(headerPosition.position.x,
		                                         mInspectionBoundsWorld.min.y + mInspectionBoundsWorld.height - headerBounds.extents.y,
		                                         headerPosition.position.z);
		mHeaderBackground.transform.localScale = new Vector3((mInspectionBoundsWorld.width / 2.0f) / (headerBounds.extents.x / headerScale.localScale.x),
		                                                     headerScale.localScale.y,
		                                                     headerScale.localScale.z);
		headerBounds = mHeaderBackground.GetComponentInChildren<SpriteRenderer>().bounds;
		mInspectionBoundsWorld.yMax -= headerBounds.size.y;

		// adjust the inspection area for the header
		mInspectionBoundsPixels.yMax = mCamera.WorldToScreenPoint(new Vector3(0, mInspectionBoundsWorld.yMax, 0)).y;

		// adjust for the view buttons
		MRViewButton button = gameObject.GetComponentInChildren<MRViewButton>();
		Bounds buttonBounds = button.gameObject.GetComponentInChildren<SpriteRenderer>().bounds;
		Transform buttonTransfom = button.gameObject.GetComponentInChildren<SpriteRenderer>().transform;
		mInspectionBoundsPixels.xMin = mCamera.WorldToScreenPoint(new Vector3(buttonBounds.max.x, 0, 0)).x -
			mCamera.WorldToScreenPoint(new Vector3(buttonBounds.min.x, 0, 0)).x + 2;
		mInspectionBoundsWorld.xMin = mCamera.ScreenToWorldPoint(new Vector3(mInspectionBoundsPixels.xMin, 0, 0)).x;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// highlight the view button for the current view
		foreach (MRViewButton button in gameObject.GetComponentsInChildren<MRViewButton>())
		{
			// note combat highlights the map tab
			if (button.id == MRGame.TheGame.CurrentView || 
			    (button.id == MRGame.eViews.Map && MRGame.TheGame.CurrentView == MRGame.eViews.Combat))
			{
				if (button != mCurrentViewButton)
				{
					if (mCurrentViewButton != null)
						mCurrentViewButton.Selected = false;
					mCurrentViewButton = button;
					mCurrentViewButton.Selected = true;
				}
				break;
			}
		}

		// display the inspection area text
		HeaderText = "";
		switch (MRGame.TheGame.CurrentView)
		{
			case MRGame.eViews.Map:
				if (MRGame.TheGame.InspectionStack != null)
				{
					HeaderText = MRGame.TheGame.InspectionStack.Name;
				}
				else if (MRGame.TheGame.ActiveControllable != null)
				{
					HeaderText = MRGame.TheGame.ActiveControllable.Name;
				}
				break;
			case MRGame.eViews.Characters:
				break;
			case MRGame.eViews.Monsters:
				break;
			case MRGame.eViews.Treasure:
				break;
			case MRGame.eViews.Main:
				break;
			case MRGame.eViews.FatigueCharacter:
				break;
			case MRGame.eViews.Combat:
				break;
			case MRGame.eViews.SelectAttack:
				break;
			case MRGame.eViews.SelectManeuver:
				break;
			case MRGame.eViews.SelectChit:
				break;
			case MRGame.eViews.SelectClearing:
				break;
		}
	}

	#endregion

	#region Members

	private Camera mCamera;
	private Rect mInspectionBoundsWorld;
	private Rect mInspectionBoundsPixels;
	private MRViewButton mCurrentViewButton;
	private GameObject mHeader;
	private GameObject mHeaderBackground;
	private TextMesh mHeaderText;

	#endregion
}

