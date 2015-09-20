//
// MRTabItems.cs
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

public class MRTabItems : MonoBehaviour
{
	#region Properties

	public bool Active
	{
		get{
			if (TabParent != null)
				return TabParent.Selected;
			return false;
		}

		set{
			if (value)
			{
				// show and enable all our contents
				Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < renderers.Length; ++i)
					renderers[i].enabled = true;
				MonoBehaviour[] scripts = gameObject.GetComponentsInChildren<MonoBehaviour>();
				for (int i = 0; i < scripts.Length; ++i)
					scripts[i].enabled = true;
			}
			else
			{
				// hide and disable all our contents
				Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < renderers.Length; ++i)
					renderers[i].enabled = false;
				MonoBehaviour[] scripts = gameObject.GetComponentsInChildren<MonoBehaviour>();
				for (int i = 0; i < scripts.Length; ++i)
					scripts[i].enabled = false;
			}
		}
	}

	public MRTab TabParent
	{
		get{
			return mTabParent;
		}

		set{
			mTabParent = value;
		}
	}

	#endregion

	#region Methods
		
	// Use this for initialization
	protected virtual void Start ()
	{
	}

	// Update is called once per frame
	protected virtual void Update ()
	{

	}

	#endregion

	#region Members

	private MRTab mTabParent;

	#endregion
}

