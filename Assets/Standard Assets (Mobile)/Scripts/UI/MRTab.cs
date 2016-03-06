//
// MRTab.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2015 Steve Jakab
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

public class MRTab : MRButton
{
	#region Constants
	
	public static readonly Color SELECTED_COLOR = new Color(255f / 255f, 203f / 255f, 15f / 255f);
	public static readonly Color UNSELECTED_COLOR = new Color(190f / 255f, 152f / 255f, 11f / 255f);
	
	#endregion

	#region Properties

	public TextMesh Text;
	public GameObject Image;
	public MRTabItems Items;
	public int Index;

	public bool Selected
	{
		get{
			return mSelected;
		}

		set{
			mSelected = value;
			if (Items != null)
			{
				Items.Active = mSelected;
			}
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		if (Items != null)
		{
			Items.TabParent = this;
			Items.Active = Selected;
		}

		foreach (Camera camera in Camera.allCameras)
		{
			if ((camera.cullingMask & (1 << gameObject.layer)) != 0)
			{
				mCamera = camera;
				break;
			}
		}
	}

	// Update is called once per frame
	public override void Update ()
	{
		if (mBackground != null)
		{
			if (!mTouched)
			{
				if (Selected)
					Image.GetComponent<SpriteRenderer>().color = SELECTED_COLOR;
				else
					Image.GetComponent<SpriteRenderer>().color = UNSELECTED_COLOR;
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
		if (touchedObject == gameObject)
		{
			Debug.Log("Tab selected: " + gameObject.name);
			SendMessageUpwards("OnTabSelected", this, SendMessageOptions.DontRequireReceiver);
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
	
	private Camera mCamera;
	[SerializeField]
	private bool mSelected;
	
	#endregion
}

