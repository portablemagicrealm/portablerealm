//
// MRActivityListWidget.cs
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

public class MRActivityListWidget : MonoBehaviour, MRITouchable
{
	#region Properties

	public MRActivityWidget prototypeActivity;

	public MRActivityList ActivityList
	{
		get{
			return mActivityList;
		}

		set{
			if (mActivityList != value)
			{
				foreach (MRActivityWidget widget in mActivityWidgets)
				{
					Destroy(widget.gameObject);
				}
				mActivityWidgets.Clear();
				mCurrentActivity = null;
			}
			mActivityList = value;
			mInitializeActivityList = true;
		}
	}

	public bool Visible
	{
		get{
			return mVisible;
		}

		set{
			mVisible = value;
			foreach (MRActivityWidget widget in mActivityWidgets)
			{
				widget.Visible = value;
			}
		}
	}

	public float BorderPixelSize
	{
		get{
			return mBorderPixelSize;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		GameObject border = null;
		foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
		{
			if (t.gameObject.name == "Current Activity")
			{
				border = t.gameObject;
				break;
			}
		}
		if (border == null)
		{
			Debug.LogError("No border for current activity");
			Application.Quit();
		}
		float borderSize = ((SpriteRenderer)border.GetComponent<Renderer>()).sprite.bounds.extents.y;
		mBorderPixelSize = ((SpriteRenderer)border.GetComponent<Renderer>()).sprite.rect.height;
		mCamera = gameObject.GetComponentsInChildren<Camera> ()[0];
		mCamera.orthographicSize = borderSize;
		mCamera.aspect = 1;
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

		if (mActivityList != null)
		{
			if (mCurrentActivity != null && MRGame.TimeOfDay == MRGame.eTimeOfDay.Birdsong && MRGame.TheGame.CurrentView == MRGame.eViews.Map)
			{
				// adjust the activity list so that our current activity is always the next to last one
				for (int i = 0; i < mActivityWidgets.Count; ++i)
				{
					if (mCurrentActivity == mActivityWidgets[i])
					{
						if (mActivityList.Activities.Count > i + 2)
						{
							while (mActivityList.Activities.Count > i + 2)
								mActivityList.RemoveLastActivity();
						}
						while (mActivityList.Activities.Count < i + 2)
							mActivityList.AddActivity(MRActivity.CreateActivity(MRGame.eActivity.None));
						break;
					}
				}
				mActivityList.RemoveLastActivity();
				mActivityList.AddActivity(MRActivity.CreateActivity(MRGame.eActivity.None));
			}
			else if (mCurrentActivity != null && MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight)
			{
				// see if we should go to the next activity
				if (mCurrentActivity.Activity.Executed)
				{
					int currentIndex = mActivityWidgets.IndexOf(mCurrentActivity);
					if (currentIndex < mActivityWidgets.Count - 1)
					{
						mCurrentActivity = mActivityWidgets[currentIndex + 1];
					}
					else
					{
						mCurrentActivity = null;
					}
				}
			}

			// make sure our widget list matches our activity list
			for (int i = 0; i < mActivityList.Activities.Count; ++i)
			{
				MRActivity activity = mActivityList.Activities[i];
				if (mActivityWidgets.Count > i)
				{
					mActivityWidgets[i].ListPosition = i;
					if (mCurrentActivity != mActivityWidgets[i])
					{
						// update the widget for the activity
						mActivityWidgets[i].Activity = activity;
						mActivityWidgets[i].Enabled = true;
						mActivityWidgets[i].Interactive = false;
					}
					else if (mCurrentActivity)
					{
						// update the activity for the widget
						if (mActivityWidgets[i].Activity != activity)
							mActivityList.SetActivity(mActivityWidgets[i].Activity, i);
					}
				}
				else if (mActivityWidgets.Count == i)
				{
					MRActivityWidget widget = (MRActivityWidget)Instantiate(prototypeActivity);
					widget.transform.parent = transform;
					if (mCurrentActivity == null)
					{
						if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Birdsong)
						{
							// if we're in birdsong, make the last activity current
							if (!mInitializeActivityList || i == mActivityList.Activities.Count - 1)
							{
								mCurrentActivity = widget;
								mCurrentActivity.Editing = true;
								mInitializeActivityList = false;
							}
						}
						else
						{
							// if we're in daylight, make the 1st unexecuted activity current
							if (!mActivityList.Activities[i].Executed)
							{
								mCurrentActivity = widget;
								mInitializeActivityList = false;
							}
						}
					}
					widget.ListPosition = i;
					widget.ParentSize = mBorderPixelSize;
					widget.Activity = activity;
					mActivityWidgets.Add(widget);
				}
				if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Birdsong)
				{
					// the last activity shown is always a blank one
					if (i == mActivityList.Activities.Count - 1)
					{
						mActivityWidgets[i].Enabled = false;
						mActivityWidgets[i].Interactive = false;
					}
				}
				else if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight)
				{
					mActivityWidgets[i].Interactive = false;
				}
			}
			while (mActivityWidgets.Count > mActivityList.Activities.Count)
			{
				Destroy(mActivityWidgets[mActivityWidgets.Count - 1].gameObject);
				mActivityWidgets.RemoveAt(mActivityWidgets.Count - 1);
			}
			if (mCurrentActivity != null)
			{
				mCurrentActivity.Enabled = true;
				mCurrentActivity.Interactive = true;
			}
		}
		UpdateCamera();
	}

	private void UpdateCamera()
	{
		if (mActivityList != null)
		{
			if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Birdsong)
			{
				// toggle if we're changing the current activity based on the touch state
				if (!mChangingCurrentActivity)
				{
					if (MRGame.JustTouched && MRGame.TheGame.CurrentView == MRGame.eViews.Map)
					{
						// if the user starts a touch in our area, let them change the action
						Vector3 viewportTouch = mCamera.ScreenToViewportPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane));
						if (viewportTouch.x >= 0 && viewportTouch.y >= 0 && viewportTouch.x <= 1 && viewportTouch.y <= 1)
						{
							mChangingCurrentActivity = true;
						}
					}
				}
				else
				{
					if (!MRGame.IsTouching)
						mChangingCurrentActivity = false;
				}
			}

			int currentActivityIndex = mActivityWidgets.IndexOf(mCurrentActivity);
			if (currentActivityIndex == -1)
			{
				mCamera.enabled = false;
				return;
			}

			float activitySize = mActivityWidgets[0].ActivityPixelSize;
			if (activitySize == 0)
				activitySize = mBorderPixelSize;

			if (mChangingCurrentActivity)
			{
				float dy = MRGame.LastTouchPos.y - MRGame.TouchPos.y;
				mCurrentWidgetOffset += dy;
				if (currentActivityIndex == 0 && mCurrentWidgetOffset < 0)
				{
					// don't go to previous activity if we're at the 1st activity
					mCurrentWidgetOffset = 0;
				}
				else if (mCurrentActivity.Activity.Activity == MRGame.eActivity.None && mCurrentWidgetOffset > 0)
				{
					// don't go to the next activity if we haven't selected an activity for the current phase
					mCurrentWidgetOffset = 0;
				}
				else if (mCurrentActivity.Activity.Activity == MRGame.eActivity.Move && 
				         (mCurrentActivity.Clearing == null || mCurrentActivity.Clearing.Clearing == null))
				{
					// don't go to the next activity if we're moving and haven't selected a clearing
					mCurrentWidgetOffset = 0;
				}
				if (mCurrentWidgetOffset > activitySize / 2.0f)
				{
					// go to next activity
					if (currentActivityIndex < mActivityWidgets.Count - 1)
					{
						++currentActivityIndex;
						if (mCurrentActivity)
							mCurrentActivity.Editing = false;
						mCurrentActivity = mActivityWidgets[currentActivityIndex];
						mCurrentActivity.Editing = true;
						mCurrentWidgetOffset = mCurrentWidgetOffset - activitySize;
					}
				}
				else if (mCurrentWidgetOffset < -activitySize / 2.0f)
				{
					// go to previous activity
					if (currentActivityIndex > 0)
					{
						--currentActivityIndex;
						if (mCurrentActivity)
							mCurrentActivity.Editing = false;
						mCurrentActivity = mActivityWidgets[currentActivityIndex];
						mCurrentActivity.Editing = true;
						mCurrentWidgetOffset = mCurrentWidgetOffset + activitySize;
					}
				}

				Rect newPos = new Rect();
				newPos.x = MRGame.TheGame.InspectionArea.InspectionBoundsPixels.xMin / Screen.width;
				newPos.y = (MRGame.TheGame.InspectionArea.InspectionBoundsPixels.yMax / Screen.height) - (((activitySize * (currentActivityIndex + 1))+ (mBorderPixelSize - activitySize) + mCurrentWidgetOffset) / Screen.height) * MRGame.DpiScale;
				newPos.width = (mBorderPixelSize / Screen.width) * MRGame.DpiScale;
				newPos.height = (mBorderPixelSize / Screen.height) * MRGame.DpiScale;
				mCamera.rect = newPos;
			}
			else
			{
				mCurrentWidgetOffset = 0;

				// adjust the camera for the list position
				Rect newPos = new Rect();
				newPos.x = MRGame.TheGame.InspectionArea.InspectionBoundsPixels.xMin / Screen.width;
				newPos.y = (MRGame.TheGame.InspectionArea.InspectionBoundsPixels.yMax / Screen.height) - (((activitySize * (currentActivityIndex + 1)) + (mBorderPixelSize - activitySize)) / Screen.height) * MRGame.DpiScale;
				newPos.width = (mBorderPixelSize / Screen.width) * MRGame.DpiScale;
				newPos.height = (mBorderPixelSize / Screen.height) * MRGame.DpiScale;
				mCamera.rect = newPos;
			}
			mCamera.enabled = true;
		}
		else
		{
			// turn off the camera
			mCamera.enabled = false;
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
		if (MRGame.TimeOfDay == MRGame.eTimeOfDay.Daylight && mCurrentActivity != null)
		{
			mCurrentActivity.OnDoubleTapped(touchedObject);
		}
		return true;
	}
	
	public bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	#endregion

	#region Members

	private MRActivityList mActivityList;
	private IList<MRActivityWidget> mActivityWidgets = new List<MRActivityWidget>();
	private MRActivityWidget mCurrentActivity;
	private Camera mCamera;
	private bool mVisible;
	private bool mInitializeActivityList;
	private float mBorderPixelSize;
	private bool mChangingCurrentActivity;
	private float mCurrentWidgetOffset;

	#endregion
}
