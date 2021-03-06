//
// MRVictoryPointSelectionDialog.cs
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
using UnityEngine.UI;
using System;
using System.Collections;

namespace PortableRealm
{
	
public class MRGameTypeSelectionDialog : MonoBehaviour
{
	private const int MaxBoLChapter = 1;

	#region Properties

	public Button DefaultSelect;
	public Button BoLMinus;
	public Button BoLPlus;
	public Text BoLChapter;
	public Button BoLSelect;

	public int BookOfLearningChapter
	{
		get{
			return mBoLChapter;
		}
	}

	public MRGame.eGameType GameType
	{
		get{
			return mGameType;
		}
	}

	public MRMainUI.OnButtonPressed Callback
	{
		get{
			return mCallback;
		}

		set{
			mCallback = value;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		mBoLChapter = 1;
		mGameType = MRGame.eGameType.Default;
		DefaultSelect.onClick.AddListener(OnDefaultSelectClicked);
		BoLSelect.onClick.AddListener(OnBoLSelectClicked);
		BoLMinus.onClick.AddListener(OnBoLMinusClicked);
		BoLPlus.onClick.AddListener(OnBoLPlusClicked);
	}

	// Update is called once per frame
	void Update ()
	{
		BoLChapter.text = mBoLChapter.ToString();
	}

	private void OnDefaultSelectClicked()
	{
		mGameType = MRGame.eGameType.Default;
		if (mCallback != null)
		{
			mCallback(0);
		}
	}

	private void OnBoLSelectClicked()
	{
		mGameType = MRGame.eGameType.BookOfLearning;
		if (mCallback != null)
		{
			mCallback(1);
		}
	}

	private void OnBoLMinusClicked()
	{
		if (mBoLChapter > 1)
		--mBoLChapter;
	}
	
	private void OnBoLPlusClicked()
	{
		if (mBoLChapter < MaxBoLChapter)
			++mBoLChapter;
	}

	#endregion

	#region Members

	private int mBoLChapter;
	private MRGame.eGameType mGameType;
	private MRMainUI.OnButtonPressed mCallback = null;
	
	#endregion
}

}