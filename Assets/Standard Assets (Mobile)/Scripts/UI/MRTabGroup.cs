//
// MRTabGroup.cs
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

public class MRTabGroup : MonoBehaviour
{
	#region Properties

	public bool Enabled
	{
		get{
			return mEnabled;
		}

		set{
			mEnabled = value;
		}
	}

	public int SelectedTab
	{
		get{
			for (int i = 0; i < mTabs.Length; ++i)
			{
				if (mTabs[i].Selected)
					return mTabs[i].Index;
			}
			return 0;
		}

		set{
			for (int i = 0; i < mTabs.Length; ++i)
			{
				if (mTabs[i].Index == value)
				{
					OnTabSelected(mTabs[i]);
					break;
				}
			}
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		mTabs = gameObject.GetComponentsInChildren<MRTab>();
		mEnabled = true;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	/// <summary>
	/// Called when a tab in the tab group has been selected.
	/// </summary>
	/// <param name="tab">the tab that was pressed</param>
	public void OnTabSelected(MRTab tab)
	{
		if (Enabled)
		{
			if (mTabs != null)
			{
				for (int i = 0; i < mTabs.Length; ++i)
				{
					if (mTabs[i] != tab)
						mTabs[i].Selected = false;
				}
			}
			tab.Selected = true;
		}
	}

	#endregion

	#region Members

	protected MRTab[] mTabs;
	protected bool mEnabled;

	#endregion
}

