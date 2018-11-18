//
// MRCharacterMat.cs
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
using System.Collections.Generic;

namespace PortableRealm
{
	
public class MRCharacterMat : MonoBehaviour
{
	#region Properties

	public GUISkin skin;

	public MRTabGroup tabs;
	public MRCharacterItemsDisplay itemsDisplay;
	public MRCharacterSpellsDisplay spellsDisplay;
	public MRCharacterScoreDisplay scoreDisplay;

	public Camera CharacterMatCamera
	{
		get{
			return mCamera;
		}
	}

	public bool Visible
	{
		get{
			return mCamera.enabled;
		}
		
		set{
			mCamera.enabled = value;
		}
	}

	public MRIControllable Controllable
	{
		get{
			return mControllable;
		}

		set{
			mControllable = value;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		// get the camera
		foreach (Camera camera in Camera.allCameras)
		{
			if (camera.name == "Character Camera")
			{
				mCamera = camera;
				break;
			}
		}

		itemsDisplay.Parent = this;
		scoreDisplay.Parent = this;
		spellsDisplay.Parent = this;
	}

	// Update is called once per frame
	void Update ()
	{
		if (!Visible || mControllable == null)
		{
			return;
		}

		if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectAttack ||
		    MRGame.TheGame.CurrentView == MRGame.eViews.SelectManeuver ||
		    MRGame.TheGame.CurrentView == MRGame.eViews.SelectChit ||
		    MRGame.TheGame.CurrentView == MRGame.eViews.Alert)
		{
			// force the selection of the items tab
			tabs.Enabled = true;
			tabs.SelectedTab = 0;
			tabs.Enabled = false;
		}
		else if (MRGame.TheGame.CurrentView == MRGame.eViews.SelectSpell)
		{
			// force the selection of the spells tab
			tabs.Enabled = true;
			tabs.SelectedTab = 1;
			tabs.Enabled = false;
		}
		else
		{
			tabs.Enabled = true;
		}
	}

	#endregion

	#region Members

	private Camera mCamera;
	private MRIControllable mControllable;

	#endregion
}

}