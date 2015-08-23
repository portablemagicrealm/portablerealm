//
// MRClearingSelector.cs
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

public class MRClearingSelector : MonoBehaviour
{
	#region Properties

	public MRActivityWidget Activity
	{
		get{
			return mActivity;
		}

		set{
			mActivity = value;
		}
	}

	public MRClearing Clearing
	{
		get{
			return mClearing;
		}

		set{
			mClearing = value;
		}
	}

	public bool Visible
	{
		get{
			return mVisible;
		}
		
		set{
			mVisible = value;
		}
	}

	#endregion

	#region Methods

	public MRClearingSelector()
	{
	}

	// Use this for initialization
	void Start ()
	{
		mVisible = true;

		mWorldSize = ((SpriteRenderer)renderer).sprite.bounds.extents.y;
		mPixelSize = ((SpriteRenderer)renderer).sprite.rect.height;
		
		// adjust the camera so it just shows the border area
		mCamera = gameObject.GetComponentsInChildren<Camera> ()[0];
		mCamera.orthographicSize = mWorldSize;
		mCamera.aspect = 1;
		mCamera.enabled = false;

		// get the cleaning name component
		mTextWidget = gameObject.GetComponentsInChildren<TextMesh>()[0];

		mRequestClearing = false;
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

		if (Activity.Editing && mClearing == null && !mRequestClearing)
		{
			mRequestClearing = true;
			MRGame.TheGame.AddUpdateEvent(new MRSelectClearingEvent(null, OnClearingSelected));
		}

		if (mActivity != null && mActivity.Activity.Activity == MRGame.eActivity.Move)
		{
			Rect parentPos = mActivity.ActivityCamera.rect;
			Rect myPos = new Rect();
			myPos.x = parentPos.x + parentPos.width;
			myPos.y = parentPos.y;
			myPos.width = (mPixelSize / Screen.width) * MRGame.DpiScale;
			myPos.height = (mPixelSize / Screen.height) * MRGame.DpiScale;
			mCamera.rect = myPos;

			gameObject.transform.position = mActivity.gameObject.transform.position;

			if (mClearing != null)
			{
				// we don't display the 'n' or 'e' tag on the clearing name
				mTextWidget.text = mClearing.Name.Substring(1);
			}
			else
				mTextWidget.text = "?";

			mCamera.enabled = true;
		}
		else
			mCamera.enabled = true;
	}

	public void OnClearingSelected(MRClearing clearing)
	{
		if (mActivity.Editing && mClearing == null)
		{
			mClearing = clearing;
			((MRMoveActivity)mActivity.Activity).Clearing = clearing;
		}
	}

	#endregion

	#region Members

	private MRActivityWidget mActivity;
	private MRClearing mClearing;
	private float mWorldSize;
	private float mPixelSize;
	private Camera mCamera;
	private bool mVisible;
	private TextMesh mTextWidget;
	private bool mRequestClearing;

	#endregion
}

