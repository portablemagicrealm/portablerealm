//
// MRButton.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2016 Steve Jakab
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
using System;
using UnityEngine;

namespace PortableRealm
{
	
public class MRButton : MonoBehaviour, MRITouchable
{
	#region Constants

	public static readonly Color COLOR_NORMAL = new Color(1f, 1f, 1f, 1f);
	public static readonly Color COLOR_PRESSED = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	#endregion

	#region Properties

	public bool Visible
	{
		get{
			return mVisible;
		}
		
		set{
			mVisible = value;
			MRUtility.SetObjectVisibility(gameObject, mVisible);
		}
	}

	#endregion

	#region Methods

	public virtual void Start()
	{
		mTouched = false;
		mVisible = true;

		SpriteRenderer backgroundRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
		if (backgroundRenderer != null)
			mBackground = backgroundRenderer.gameObject;
	}

	public virtual void Update()
	{
		if (mBackground != null)
		{
			if (!mTouched)
				mBackground.GetComponent<SpriteRenderer>().color = COLOR_NORMAL;
			else
				mBackground.GetComponent<SpriteRenderer>().color = COLOR_PRESSED;
		}
	}

	public bool OnTouched(GameObject touchedObject)
	{
		if (Visible)
		{
			mTouched = true;
		}
		return Visible;
	}

	public bool OnReleased(GameObject touchedObject)
	{
		if (Visible)
		{
			bool wasTouched = mTouched;
			mTouched = false;
			if (wasTouched)
			{
				return OnButtonActivate(gameObject);
			}
			else
			{
				MRITouchable parentHandler = GetParentHandler();
				if (parentHandler != null)
				{
					return parentHandler.OnReleased(gameObject);
				}
			}
		}
		return false;
	}

	public bool OnSingleTapped(GameObject touchedObject)
	{
		if (Visible)
		{
			mTouched = false;
			return OnButtonActivate(gameObject);
		}
		return false;
	}

	public bool OnDoubleTapped(GameObject touchedObject)
	{
		if (Visible)
		{
			mTouched = false;
		}
		return Visible;

	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return Visible;
	}

	public bool OnTouchMove(GameObject touchedObject, float delta_x, float delta_y)
	{
		return Visible;
	}

	public virtual bool OnButtonActivate(GameObject touchedObject)
	{
		MRITouchable parentHandler = GetParentHandler();
		if (parentHandler != null)
		{
			return parentHandler.OnButtonActivate(gameObject);
		}
		return false;
	}

	public bool OnPinchZoom(float pinchDelta)
	{
		return true;
	}

	private MRITouchable GetParentHandler()
	{
		MRITouchable touchable = gameObject.transform.parent.GetComponentInParent<MRITouchable>();
		return touchable;
	}

	#endregion

	#region Members

	protected bool mTouched;
	protected bool mVisible;
	protected GameObject mBackground;

	#endregion
}

}