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

public class MRClock : MonoBehaviour, MRITouchable
{
	#region Constants

	private const float ROTATION_SPEED_DEG_PER_SEC = 120.0f;

	#endregion

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
		mRotatingClock = false;
		mCamera = gameObject.GetComponentsInChildren<Camera> ()[0];

		// position the clock image in he upper-right of the screen
		Rect cameraRect = new Rect(mCamera.rect);
		cameraRect.width *= 0.25f;
		cameraRect.height *= 0.25f;
		cameraRect.x = 1.0f - cameraRect.width;
		cameraRect.y = 1.0f - cameraRect.height;
		mCamera.rect = cameraRect;

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
		Vector3 colliderPosWorld = transform.TransformPoint(new Vector3(0 - collider.size.x / 2.0f, 0 - collider.size.y / 2.0f, 0));
		transform.Translate(cameraPosMap.x - colliderPosWorld.x, cameraPosMap.y - colliderPosWorld.y, 0);

		Quaternion desiredRotation = Quaternion.AngleAxis(30.0f + 60.0f * (int)MRGame.TimeOfDay, Vector3.forward);
		Quaternion currentRotation = mClockImage.transform.localRotation;
		if (!mRotatingClock && Mathf.Abs(Quaternion.Angle(desiredRotation, currentRotation)) > 0.01f)
		{
			StartCoroutine(RotateClock(1.0f/30.0f));
		}
	}

	/// <summary>
	/// Handles rotating the clock when the time of day changes.
	/// </summary>
	/// <returns>yield WaitForSeconds to continue, yield null when done</returns>
	/// <param name="waitTime">amount of time to wait between animation updates</param>
	private IEnumerator RotateClock(float waitTime)
	{
		mRotatingClock = true;
		float totalTime = 0;
		Quaternion startRotation = mClockImage.transform.localRotation;
		Quaternion desiredRotation = Quaternion.AngleAxis(30.0f + 60.0f * (int)MRGame.TimeOfDay, Vector3.forward);
		Quaternion currentRotation = startRotation;
		while (Mathf.Abs(Quaternion.Angle(desiredRotation, currentRotation)) > 0.01f)
		{
			totalTime += waitTime;
			mClockImage.transform.localRotation = Quaternion.Lerp(startRotation, desiredRotation, (ROTATION_SPEED_DEG_PER_SEC * totalTime) / 60.0f);
			currentRotation = mClockImage.transform.localRotation;
			yield return new WaitForSeconds(waitTime);
		}
		mRotatingClock = false;
		mClockImage.transform.localRotation = desiredRotation;
		mDateText.text = MRGame.DayOfMonth.ToString();
		yield return null;
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
		if (MRGame.TheGame.GameState == MRGame.eGameState.Active && touchedObject == gameObject)
		{
			SendMessageUpwards("NextGameTime");
		}
		return true;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	#endregion

	#region Members

	private Camera mCamera;
	private GameObject mClockImage;
	private TextMesh mDateText;
	private bool mRotatingClock;

	#endregion
}

