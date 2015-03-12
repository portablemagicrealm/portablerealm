//
// MRActivityList.cs
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

using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class MRActivityList : MRISerializable
{
	#region Properties

	public IList<MRActivity> Activities
	{
		get{
			return mActivities;
		}
	}

	public MRIControllable Owner
	{
		get{
			return mOwner;
		}
	}

	#endregion

	#region Methods

	public MRActivityList(MRIControllable owner)
	{
		mOwner = owner;
		mActivities = new List<MRActivity>();
	}

	public void AddActivity(MRActivity activity)
	{
		activity.Owner = mOwner;
		activity.Parent = this;
		mActivities.Add(activity);
	}

	public void SetActivity(MRActivity activity, int index)
	{
		activity.Owner = mOwner;
		activity.Parent = this;
		mActivities[index] = activity;
	}

	public void RemoveActivity(MRActivity activity)
	{
		mActivities.Remove(activity);
	}

	public void RemoveLastActivity()
	{
		if (mActivities.Count > 0)
			mActivities.RemoveAt(mActivities.Count - 1);
	}

	public virtual bool Load(JSONObject root)
	{
		JSONArray activities = (JSONArray)root["activities"];
		for (int i = 0; i < activities.Count; ++i)
		{
			JSONObject activityData = (JSONObject)activities[i];
			int activityId = ((JSONNumber)activityData["id"]).IntValue;
			MRActivity activity = MRActivity.CreateActivity((MRGame.eActivity)activityId);
			activity.Load(activityData);
			activity.Parent = this;
			activity.Owner = mOwner;
			mActivities.Add(activity);
		}
		return true;
	}
	
	public virtual void Save(JSONObject root)
	{
		JSONArray activities = new JSONArray(mActivities.Count);
		for (int i = 0; i < mActivities.Count; ++i)
		{
			JSONObject activity = new JSONObject();
			mActivities[i].Save(activity);
			activities[i] = activity;
		}
		root["activities"] = activities;
	}

	#endregion

	#region Members

	private IList<MRActivity> mActivities;
	private MRIControllable mOwner;

	#endregion
}
