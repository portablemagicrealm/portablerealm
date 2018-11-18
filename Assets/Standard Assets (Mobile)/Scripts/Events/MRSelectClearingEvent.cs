//
// MRSelectClearingEvent.cs
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

namespace PortableRealm
{
	
public class MRSelectClearingEvent : MRUpdateEvent
{
	#region Callback delegate

	public delegate void ClearingSelectedCallback(MRClearing clearing);

	#endregion

	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.SelectClearingEvent;
		}
	}

	public MRClearing Connection
	{
		get{
			return mConnection;
		}
	}

	public MRClearing Selected
	{
		get{
			return mSelected;
		}
	}

	#endregion

	#region Methods

	public MRSelectClearingEvent(MRClearing connection, ClearingSelectedCallback callback)
	{
		mInitialized = false;
		mConnection = connection;
		mCallback = callback;
	}

	/// <summary>
	/// Updates this instance.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	public override bool Update ()
	{
		if (!mInitialized)
		{
			mInitialized = true;
			MRGame.TheGame.PushView(MRGame.eViews.SelectClearing);
		}
		else
		{
			// check if we are done
			if (mSelected != null)
			{
				// make sure the selected clearing connects to the connection clearing
				if (mConnection == null || mConnection.RoadTo(mSelected) != null)
				{
					MRMainUI.TheUI.DisplayInstructionMessage(null);
					MRGame.TheGame.PopView();
					MRGame.TheGame.RemoveUpdateEvent(this);
					if (mCallback != null)
						mCallback(mSelected);
				}
			}
			else
			{
				MRMainUI.TheUI.DisplayInstructionMessage("Select Clearing");
			}
		}
		return false;
	}

	public override void OnClearingSelected(MRClearing clearing)
	{
		mSelected = clearing;
	}

	#endregion

	#region Members

	private bool mInitialized;
	private MRClearing mConnection;
	private MRClearing mSelected;
	private ClearingSelectedCallback mCallback;

	#endregion
}

}