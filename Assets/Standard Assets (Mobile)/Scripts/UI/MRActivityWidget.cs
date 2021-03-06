//
// MRActivityWidget.cs
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

namespace PortableRealm
{
	
public class MRActivityWidget : MonoBehaviour, MRITouchable
{
	#region Properties

	public MRClearingSelector clearingPrototype;

	public MRActivity Activity
	{
		get{
			return mActivity;
		}

		set{
			if (mActivity != value)
			{
				mActivity = value;
				if (mCamera != null)
					UpdateWidgetForActivity();
				if (mActivity.Activity == MRGame.eActivity.Move && mClearing == null)
				{
					// create a clearing selector for the move
					mClearing = (MRClearingSelector)Instantiate(clearingPrototype);
					mClearing.transform.parent = transform;
					mClearing.Activity = this;
					mClearing.Clearing = ((MRMoveActivity)mActivity).Clearing;
				}
				else if (mClearing != null)
				{
					// remove the clearing selector
					Destroy(mClearing.gameObject);
					mClearing = null;
				}
			}
		}
	}

	public int ListPosition
	{
		get{
			return mListPosition;
		}

		set{
			if (mListPosition != value)
			{
				mListPosition = value;
				if (mCamera != null)
					UpdateWidgetForActivity();
			}
		}
	}

	public bool Interactive
	{
		get{
			return mIsInteractive;
		}

		set{
			mIsInteractive = value;
			//Debug.Log("Activity " + mListPosition + " interactive set to " + mIsInteractive);
		}
	}

	public bool Visible
	{
		get{
			return mVisible;
		}
		
		set{
			mVisible = value;
			if (mClearing != null)
				mClearing.Visible = value;
		}
	}

	public bool Enabled
	{
		get{
			return mIsEnabled;
		}

		set{
			if (mIsEnabled != value)
			{
				mIsEnabled = value;
				if (mCamera != null)
					UpdateWidgetForActivity();
			}
		}
	}

	public bool Editing
	{
		get{
			return mEditingActivity;
		}

		set{
			mEditingActivity = value;
		}
	}

	public float ActivityPixelSize
	{
		get{
			return MRGame.TheGame.InspectionArea.ActivityWidthPixels;
		}
	}

	public Camera ActivityCamera
	{
		get{
			return mCamera;
		}
	}

	public MRClearingSelector Clearing
	{
		get{
			return mClearing;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		mVisible = true;

		mBorder = null;
		mCanceledBorder = null;
		foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
		{
			if (t.gameObject.name == "Border")
			{
				mBorder = t.gameObject;
			}
			else if (t.gameObject.name == "Canceled Border")
			{
				mCanceledBorder = t.gameObject;
				break;
			}
		}
		if (mBorder == null)
		{
			Debug.LogError("No border for activity widget");
			Application.Quit();
		}
		if (mCanceledBorder == null)
		{
			Debug.LogError("No canceled border for activity widget");
			Application.Quit();
		}
		mBorder.GetComponent<Renderer>().enabled = true;
		mCanceledBorder.GetComponent<Renderer>().enabled = false;
		Sprite sprite = ((SpriteRenderer)mBorder.GetComponent<Renderer>()).sprite;
		mBorderSize = sprite.bounds.extents.y;

		// adjust the camera so it just shows the border area
		mCamera = gameObject.GetComponentsInChildren<Camera> ()[0];
		mCamera.orthographicSize = mBorderSize;
		mCamera.aspect = 1;

		foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
		{
			if (t.gameObject.name == "Activities")
			{
				mActivityStrip = t.gameObject;
				break;
			}
		}
		if (mActivityStrip == null)
		{
			Debug.LogError("No actions for activity widget");
			Application.Quit();
		}

		mActivityStripWidth = ((SpriteRenderer)mActivityStrip.GetComponent<Renderer>()).sprite.bounds.extents.x * 2.0f;
		int numActivities = Enum.GetValues(typeof(MRGame.eActivity)).Length;
		mActivityWidth = mActivityStripWidth / numActivities;
		mMaxActivityListPos = (mActivityStripWidth / 2) - (0.5f * mActivityWidth);
		mMinActivityListPos = (mActivityStripWidth / 2) - ((numActivities - 0.5f) * mActivityWidth);

		UpdateWidgetForActivity();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!Visible)
		{
			mCamera.enabled = false;
			return;
		}
		mCamera.enabled = true;

		// show the appropriate border, for the canceled state of the activity
		mBorder.GetComponent<Renderer>().enabled = (mActivity == null || !mActivity.Canceled);
		mCanceledBorder.GetComponent<Renderer>().enabled = !mBorder.GetComponent<Renderer>().enabled;

		if (!mIsInteractive)
		{
			// set the action based on the widget's current position
			mChangingActivity = false;
			UpdateActivityForWidget();
			return;
		}

		if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Birdsong && MRGame.JustTouched && MRGame.TheGame.CurrentView == MRGame.eViews.Map)
		{
			// if the user starts a touch in our area, let them change the action
			if (!mChangingActivity)
			{
				Vector3 viewportTouch = mCamera.ScreenToViewportPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane));
				if (viewportTouch.x < 0 || viewportTouch.y < 0 || viewportTouch.x > 1 || viewportTouch.y > 1)
					return;
			}
			mChangingActivity = true;
		}
		if (mChangingActivity)
		{
			if (MRGame.IsTouching)
			{
				// change screen space to world space
				Vector3 lastTouch = mCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane));
				Vector3 thisTouch = mCamera.ScreenToWorldPoint(new Vector3(MRGame.TouchPos.x, MRGame.TouchPos.y, mCamera.nearClipPlane));
				mActivityStrip.transform.Translate(new Vector3(thisTouch.x - lastTouch.x, 0, 0));
				if (mActivityStrip.transform.position.x < mMinActivityListPos)
				{
					Vector3 oldPos = mActivityStrip.transform.position;
					mActivityStrip.transform.position = new Vector3(mMinActivityListPos, oldPos.y, oldPos.z);
				}
				else if (mActivityStrip.transform.position.x > mMaxActivityListPos)
				{
					Vector3 oldPos = mActivityStrip.transform.position;
					mActivityStrip.transform.position = new Vector3(mMaxActivityListPos, oldPos.y, oldPos.z);
				}
			}
			else
			{
				mChangingActivity = false;
				// set the action based on the widget's current position
				UpdateActivityForWidget();
			}
		}
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
		if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight)
		{
			if (!mActivity.Executed)
			{
				mActivity.Active = true;
			}
		}
		return true;
	}

	public bool OnTouchMove(GameObject touchedObject, float delta_x, float delta_y)
	{
		return true;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	public bool OnButtonActivate(GameObject touchedObject)
	{
		return true;
	}

	public bool OnPinchZoom(float pinchDelta)
	{
		return true;
	}

	//
	// Change our action for whichever icon is showing the most on screen.
	//
	private void UpdateActivityForWidget()
	{
		float activity = ((mActivityStripWidth / 2) - mActivityStrip.transform.position.x) / mActivityWidth;
		MRGame.eActivity activityType = (MRGame.eActivity)activity;
		if (Activity == null || Activity.Activity != activityType)
		{
			Activity = MRActivity.CreateActivity(activityType);
		}
		UpdateWidgetForActivity();
	}

	//
	// Show the correct icon for our action
	//
	private void UpdateWidgetForActivity()
	{
		Vector3 oldPos = mActivityStrip.transform.position;
		mActivityStrip.transform.position = new Vector3((mActivityStripWidth / 2) - (((int)mActivity.Activity + 0.5f) * mActivityWidth), oldPos.y, oldPos.z);

		// adjust our position so we don't overlap another activity
		gameObject.transform.position = new Vector3(0, mListPosition * mBorderSize * 2.0f, 0);

		// adjust the camera for the list position
		Rect newPos = new Rect();
		float parentOffset = 0;
		newPos.x = (parentOffset + MRGame.TheGame.InspectionArea.TabWidthPixels) / Screen.width;
		newPos.y = (MRGame.TheGame.InspectionArea.InspectionBoundsPixels.yMax - (MRGame.TheGame.InspectionArea.ActivityWidthPixels * mListPosition) - MRGame.TheGame.InspectionArea.ActivityWidthPixels) / Screen.height;
		newPos.width = MRGame.TheGame.InspectionArea.ActivityWidthPixels / Screen.width;
		newPos.height = MRGame.TheGame.InspectionArea.ActivityWidthPixels / Screen.height;
		mCamera.rect = newPos;

		// if we're not enabled, don't show the activity strip
		mActivityStrip.GetComponent<Renderer>().enabled = mIsEnabled;
	}

	#endregion

	#region Members

	private Camera mCamera;
	private GameObject mActivityStrip = null;
	private GameObject mBorder = null;
	private GameObject mCanceledBorder = null;
	private bool mVisible;
	private float mBorderSize;
	private float mActivityStripWidth;
	private float mActivityWidth;
	private float mMinActivityListPos;
	private float mMaxActivityListPos;
	private bool mChangingActivity = false;
	private bool mEditingActivity = false;
	private MRActivity mActivity;
	private MRClearingSelector mClearing;
	private int mListPosition;
	private bool mIsInteractive;
	private bool mIsEnabled;

	#endregion
}

}