//
// MRActivity.cs
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
using AssemblyCSharp;

public abstract class MRActivity : MRISerializable
{
	#region Properties

	public MRGame.eActivity Activity
	{
		get{
			return mActivity;
		}
	}

	public MRActivityList Parent
	{
		get{
			return mParent;
		}
		
		set{
			mParent = value;
		}
	}

	public MRIControllable Owner
	{
		get{
			return mOwner;
		}

		set{
			mOwner = value;
		}
	}

	public virtual bool Active
	{
		get{
			return mActive;
		}

		set{
			if (mActive != value && !Executed)
			{
				if (value == true)
				{
					// make sure we can execute the activity
					if (!Owner.CanExecuteActivity(this))
					{
						Executed = true;
						value = false;
					}
				}
				mActive = value;
			}
		}
	}

	public bool Canceled
	{
		get{
			return mCanceled;
		}

		set{
			mCanceled = value;
			if (mCanceled)
				Executed = true;
		}
	}

	public bool Executed
	{
		get{
			return mExecuted;
		}
		
		set{
			mExecuted = value;
			if (mExecuted)
			{
				Active = false;
				if (!Canceled)
					MRGame.TheGame.AddUpdateEvent(new MREndPhaseEvent());
			}
		}
	}

	#endregion

	#region Methods

	// this class can only be created via derived classes
	protected MRActivity(MRGame.eActivity activity)
	{
		mActivity = activity;
		mActive = false;
		mCanceled = false;
		mExecuted = false;
#if DEBUG
		m_id = ++ms_id;
#endif
	}

	public void Update()
	{
		if (!mExecuted && mActive)
		{
			InternalUpdate();
			if (mExecuted)
				Owner.ExecutedActivity(this);
		}
	}

	protected abstract void InternalUpdate();

	public virtual bool Load(JSONObject root)
	{
		mActive = ((JSONBoolean)root["activity"]).Value;
		mCanceled = ((JSONBoolean)root["canceled"]).Value;
		mExecuted = ((JSONBoolean)root["executed"]).Value;
		return true;
	}
	
	public virtual void Save(JSONObject root)
	{
		root["id"] = new JSONNumber((int)mActivity);
		root["activity"] = new JSONBoolean(mActive);
		root["canceled"] = new JSONBoolean(mCanceled);
		root["executed"] = new JSONBoolean(mExecuted);
	}

	#endregion

	#region Factory
	
	public static MRActivity CreateActivity(MRGame.eActivity activityId)
	{
		MRActivity activity = null;
		switch (activityId)
		{
			case MRGame.eActivity.Enchant:
				activity = new MREnchantActivity();
				break;
			case MRGame.eActivity.Follow:
				activity = new MRFollowActivity();
				break;
			case MRGame.eActivity.Hire:
				activity = new MRHireActivity();
				break;
			case MRGame.eActivity.Alert:
				activity = new MRAlertActivity();
				break;
			case MRGame.eActivity.Rest:
				activity = new MRRestActivity();
				break;
			case MRGame.eActivity.Trade:
				activity = new MRTradeActivity();
				break;
			case MRGame.eActivity.Search:
				activity = new MRSearchActivity();
				break;
			case MRGame.eActivity.Move:
				activity = new MRMoveActivity();
				break;
			case MRGame.eActivity.Hide:
				activity = new MRHideActivity();
				break;
			case MRGame.eActivity.None:
				activity = new MRNoActivity();
				break;
			default:
				Debug.LogError("Trying to create unknown activity type " + activityId);
				break;
		}
		return activity;
	}
	
	#endregion
	
	#region Members

	private MRGame.eActivity mActivity;
	private bool mActive;
	private bool mCanceled;
	private bool mExecuted;
	private MRIControllable mOwner;
	private MRActivityList mParent;
#if DEBUG
	private static int ms_id;
	public int m_id;
#endif

	#endregion
}

