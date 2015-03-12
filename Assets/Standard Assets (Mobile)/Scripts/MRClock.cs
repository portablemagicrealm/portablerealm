//
// MRClock.cs
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

public class MRClock : MonoBehaviour
{
	#region Properties

	public bool Visible
	{
		get{
			return mCamera.enabled;
		}
		
		set{
			mCamera.enabled = value;
			
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		mCamera = gameObject.GetComponentsInChildren<Camera> ()[0];

		// position the clock image in he upper-right of the screen
		Rect cameraRect = new Rect(mCamera.rect);
		cameraRect.width *= 0.25f;
		cameraRect.height *= 0.25f;
		cameraRect.x = 1.0f - cameraRect.width;
		cameraRect.y = 1.0f - cameraRect.height;
		mCamera.rect = cameraRect;

		// make sure our collider matches the camera view area
		Camera mapCamera = MRGame.TheGame.TheMap.MapCamera;
		BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
		Vector3 cameraLLPosScreen = mCamera.ViewportToScreenPoint(Vector3.zero);
		Vector3 cameraURPosScreen = mCamera.ViewportToScreenPoint(Vector3.one);
		Vector3 cameraLLPosMap = mapCamera.ScreenToWorldPoint(cameraLLPosScreen);
		Vector3 cameraURPosMap = mapCamera.ScreenToWorldPoint(cameraURPosScreen);
		collider.size = new Vector2(cameraURPosMap.x - cameraLLPosMap.x, cameraURPosMap.y - cameraLLPosMap.y);

		foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
		{
			if (t.gameObject.name == "TimeOfDay")
			{
				mClockImage = t.gameObject;
				break;
			}
		}
		mDateText = gameObject.GetComponentsInChildren<TextMesh>()[0];
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!MRGame.TheGame.TheMap.Visible)
			return;

		// adjust the position so that the collider is in the same place the camera is showing
		Camera mapCamera = MRGame.TheGame.TheMap.MapCamera;
		BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
		Vector3 cameraPosScreen = mCamera.ViewportToScreenPoint(Vector3.zero);
		Vector3 cameraPosMap = mapCamera.ScreenToWorldPoint(cameraPosScreen);
		Vector3 colliderPosWorld = transform.TransformPoint(new Vector3(collider.center.x - collider.size.x / 2.0f, collider.center.y - collider.size.y / 2.0f, 0));
		transform.Translate(cameraPosMap.x - colliderPosWorld.x, cameraPosMap.y - colliderPosWorld.y, 0);
		colliderPosWorld = transform.TransformPoint(new Vector3(collider.center.x - collider.size.x / 2.0f, collider.center.y - collider.size.y / 2.0f, 0));

		if (MRGame.IsDoubleTapped)
		{
			Vector3 worldTouch = mapCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane));
			RaycastHit2D hit = Physics2D.Raycast(worldTouch, Vector2.zero);
			if (hit.collider == collider2D)
			{
				SendMessageUpwards("NextGameTime");
			}
		}

		mClockImage.transform.localRotation = Quaternion.AngleAxis(30.0f + 60.0f * (int)MRGame.TimeOfDay, Vector3.forward);
		mDateText.text = MRGame.DayOfMonth.ToString();
	}

	#endregion

	#region Members

	private Camera mCamera;
	private GameObject mClockImage;
	private TextMesh mDateText;

	#endregion
}

