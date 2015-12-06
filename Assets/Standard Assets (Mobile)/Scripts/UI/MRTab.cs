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

public class MRTab : MonoBehaviour, MRITouchable
{
	#region Constants
	
	public static Color selectedColor = new Color(255f / 255f, 203f / 255f, 15f / 255f);
	public static Color unselectedColor = new Color(190f / 255f, 152f / 255f, 11f / 255f);
	
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
	void Start ()
	{
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
	void Update ()
	{
		if (Selected)
			Image.GetComponent<SpriteRenderer>().color = selectedColor;
		else
			Image.GetComponent<SpriteRenderer>().color = unselectedColor;
	}

	public bool OnSingleTapped(GameObject touchedObject)
	{
		if (touchedObject == gameObject)
		{
			Debug.Log("Tab selected: " + gameObject.name);
			SendMessageUpwards("OnTabSelected", this, SendMessageOptions.DontRequireReceiver);
		}
		return true;
	}

	public bool OnDoubleTapped(GameObject touchedObject)
	{
		return true;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	#endregion

	#region Members
	
	private Camera mCamera;
	[SerializeField]
	private bool mSelected;
	
	#endregion
}

