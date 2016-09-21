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

public class MRInspectionArea : MonoBehaviour, MRITouchable
{
	#region Constants

	// proportion of the inspection area taken up by items
	public const float TAB_RATIO = 0.15f;
	public const float ACTIVITY_RATIO = 0.45f;
	public const float CLEARING_RATIO = 0.40f;

	#endregion

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

	public float TabWidthPixels
	{
		get{
			return mTabWidth;
		}
	}

	public float ActivityWidthPixels
	{
		get{
			return mActivityWidth;
		}
	}

	public float ClearingWidthPixels
	{
		get{
			return mClearingWidth;
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
				break;
			}
		}
		transforms = mHeader.GetComponentsInChildren<Transform>();
		foreach (Transform transform in transforms)
		{
			switch (transform.gameObject.name)
			{
				case "text":
					mHeaderText = transform.gameObject.GetComponentInChildren<TextMesh>();
					break;
				case "background":
					mHeaderBackground = transform.gameObject;
					break;
				case "back":
					mHeaderBack = transform.gameObject;
					break;
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

		// comput the item widths
		mTabWidth = mInspectionBoundsPixels.width * TAB_RATIO;
		mActivityWidth = mInspectionBoundsPixels.width * ACTIVITY_RATIO;
		mClearingWidth = mInspectionBoundsPixels.width * CLEARING_RATIO;

		// adjust the header
		Bounds headerBounds = mHeaderBackground.GetComponentInChildren<SpriteRenderer>().bounds;
		Transform headerPosition = mHeader.transform;
		Transform headerScale = mHeaderBackground.transform;
		mHeader.transform.position = new Vector3(headerPosition.position.x,
		                                         mInspectionBoundsWorld.min.y + mInspectionBoundsWorld.height - headerBounds.extents.y,
		                                         headerPosition.position.z);
		float headerStretch = (mInspectionBoundsWorld.width / 2.0f) / (headerBounds.extents.x / headerScale.localScale.x);
		mHeaderBackground.transform.localScale = new Vector3(headerStretch,
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

		// display the inspection area back image
		MRUtility.SetObjectVisibility(mHeaderBack, (MRGame.TheGame.InspectionStack != null && MRGame.TheGame.CurrentView != MRGame.eViews.Main));

		// display the inspection area text
		HeaderText = "";
		switch (MRGame.TheGame.CurrentView)
		{
			case MRGame.eViews.Map:
			case MRGame.eViews.SelectClearing:
				if (MRGame.TheGame.InspectionStack != null)
				{
					HeaderText = MRGame.TheGame.InspectionStack.Name;
				}
				else if (MRGame.TheGame.ActiveControllable != null)
				{
					HeaderText = MRGame.TheGame.ActiveControllable.Name;
				}
				break;
			default:
				break;
		}
	}

	public bool OnTouched(GameObject touchedObject)
	{
		return false;
	}

	public bool OnReleased(GameObject touchedObject)
	{
		return false;
	}

	public bool OnSingleTapped(GameObject touchedObject)
	{
		return false;
	}

	public bool OnDoubleTapped(GameObject touchedObject)
	{
		if (touchedObject == mHeader)
		{
			return HeaderTapped();
		}
		else if (touchedObject == this.gameObject)
		{
			if (MRGame.TheGame.InspectionStack != null)
				return MRGame.TheGame.InspectionStack.OnDoubleTapped(MRGame.TheGame.InspectionStack.gameObject);
			else if (MRGame.TheGame.CurrentView == MRGame.eViews.Combat)
				return MRGame.TheGame.CombatManager.CombatStack.OnDoubleTapped(MRGame.TheGame.CombatManager.CombatStack.gameObject);
		}
		return false;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return false;
	}

	public virtual bool OnButtonActivate(GameObject touchedObject)
	{
		if (touchedObject == mHeader)
		{
			return HeaderTapped();
		}
		return false;
	}

	public bool OnPinchZoom(float pinchDelta)
	{
		return false;
	}

	/// <summary>
	/// Handles when the inspection area header image is tapped (or double-tapped).
	/// </summary>
	/// <returns><c>true</c>, if the tap was handled, <c>false</c> otherwise.</returns>
	private bool HeaderTapped()
	{
		// if we are inspecting a stack, return the stack to its regular position
		if (MRGame.TheGame.InspectionStack != null)
		{
			MRGame.TheGame.InspectStack(null);
			return true;
		}
		// if we are on the map, center it on the current character
		else if (MRGame.TheGame.CurrentView == MRGame.eViews.Map)
		{
			if (MRGame.TheGame.TheMap != null &&
			    MRGame.TheGame.ActiveControllable != null &&
			    MRGame.TheGame.ActiveControllable.Location != null)
			{
				MRGame.TheGame.TheMap.CenterMapOnTile(MRGame.TheGame.ActiveControllable.Location.MyTileSide.Tile);
				return true;
			}
		}
		return false;
	}

	#endregion

	#region Members

	private Camera mCamera;
	private Rect mInspectionBoundsWorld;
	private Rect mInspectionBoundsPixels;
	private MRViewButton mCurrentViewButton;
	private GameObject mHeader;
	private GameObject mHeaderBackground;
	private GameObject mHeaderBack;
	private TextMesh mHeaderText;
	private float mTabWidth;
	private float mActivityWidth;
	private float mClearingWidth;

	#endregion
}

