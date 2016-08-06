//
// MRSuperSiteChit.cs
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
using System.Collections.Generic;
using AssemblyCSharp;

public static class SuperSiteChitExtensions
{
	public static int ClearingNumber(this MRMapChit.eSuperSiteChitType type)
	{
		switch (type)
		{
			case MRMapChit.eSuperSiteChitType.LostCastle:
				return 1;
			case MRMapChit.eSuperSiteChitType.LostCity:
				return 3;
		}
		return 1;
	}
}

public class MRSuperSiteChit : MRMapChit
{
	#region Properties

	public MRMapChit.eSuperSiteChitType SiteType
	{
		get{
			return mSiteType;
		}

		set{
			mSiteType = value;
			switch (mSiteType)
			{
				case MRMapChit.eSuperSiteChitType.LostCastle:
					LongName = "LOST\nCASTLE\n" + mClearingNumber;
					ShortName = "CA" + mClearingNumber;
					break;
				case MRMapChit.eSuperSiteChitType.LostCity:
					LongName = "LOST\nCITY\n" + mClearingNumber;
					ShortName = "CI" + mClearingNumber;
					break;
			}
		}
	}

	public int ClearingNumber
	{
		get{
			return mClearingNumber;
		}
		
		set{
			mClearingNumber = value;
		}
	}

	public IList<MRMapChit> ContainedChits
	{
		get{
			return mContainedChits;
		}
	}

	#endregion

	#region Methods

	static MRSuperSiteChit()
	{
		MapChitTypes[eMapChitType.SuperSite] = typeof(MRSuperSiteChit);
	}

	// dummy function to reference the class
	static public void Init() {}

	// Use this for initialization
	public override void Start ()
	{
		mChitType = eMapChitType.SuperSite;
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	public override void OnDestroy()
	{
		base.OnDestroy();

		foreach (MRMapChit chit in mContainedChits)
		{
			DestroyObject(chit.gameObject);
		}
	}

	public override bool Load(JSONObject root)
	{
		if (!base.Load(root))
			return false;

		SiteType = (MRMapChit.eSuperSiteChitType)((JSONNumber)root["site"]).IntValue;

		mContainedChits.Clear();
		JSONArray contents = (JSONArray)root["contents"];
		for (int i = 0; i < contents.Count; ++i)
		{
			JSONObject chitData = (JSONObject)contents[i];
			if (chitData == null)
				return false;
			MRMapChit chit = MRMapChit.Create(chitData);
			if (chit == null)
				return false;
			chit.Layer = LayerMask.NameToLayer("Dummy");
			mContainedChits.Add(chit);
		}
		return true;
	}
	
	public override void Save(JSONObject root)
	{
		base.Save(root);
		root["site"] = new JSONNumber((int)SiteType);
		JSONArray contents = new JSONArray(mContainedChits.Count);
		for (int i = 0; i < mContainedChits.Count; ++i)
		{
			JSONObject chitData = new JSONObject();
			mContainedChits[i].Save(chitData);
			contents[i] = chitData;
		}
		root["contents"] = contents;
	}

	#endregion

	#region Members

	private MRMapChit.eSuperSiteChitType mSiteType;
	private int mClearingNumber;
	private IList<MRMapChit> mContainedChits = new List<MRMapChit>();

	#endregion
}

