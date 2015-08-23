//
// MRMapChit.cs
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
using System.Reflection;
using AssemblyCSharp;

public class MRMapChit : MRChit
{
	#region Sort Class

	public class MRMapChitSorter : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			return ((MRMapChit)x).SummonSortIndex - ((MRMapChit)y).SummonSortIndex;
		}
	}

	#endregion
	
	#region Constants

	public enum eMapChitType
	{
		Sound,
		Warning,
		Site,
		SuperSite
	}

	public enum eSoundChitType
	{
		Flutter,
		Howl,
		Patter,
		Roar,
		Slither
	}

	public enum eWarningChitType
	{
		Bones,
		Dank,
		Ruins,
		Smoke,
		Stink
	}

	public enum eSiteChitType
	{
		Altar,
		Cairns,
		Hoard,
		Lair,
		Pool,
		Shrine,
		Statue,
		Vault
	}

	public enum eSuperSiteChitType
	{
		LostCastle,
		LostCity,
	}

	private const int BIG_FONT_SIZE = 180;
	private const int SMALL_FONT_SIZE = 70;

	#endregion

	#region Properties

	public static IDictionary<eMapChitType, Type> MapChitTypes
	{
		get {
			return msMapChitTypes;
		}
	}

	public eMapChitType ChitType
	{
		get{
			return mChitType;
		}
	}

	public MRTile Tile
	{
		get{
			return mTile;
		}

		set{
			mTile = value;
		}
	}

	public GameObject Counter
	{
		get{
			return mCounter;
		}
	}

	public string LongName
	{
		get{
			return mLongName;
		}

		protected set{
			mLongName = value;
			Id = MRUtility.IdForName(mLongName);
		}
	}

	public string ShortName
	{
		get{
			return mShortName;
		}

		protected set{
			mShortName = value;
		}
	}

	public override string Name
	{
		get{
			return mLongName;
		}
	}

	public bool SummonedMonsters
	{
		get{
			return mSummonedMonsters;
		}

		set{
			mSummonedMonsters = false;
		}
	}

	public virtual int SummonSortIndex
	{
		get{
			return 100;
		}
	}

	public override Transform Parent
	{
		get{
			return base.Parent;
		}
		
		set {
			base.Parent = value;
			transform.localScale = Vector3.one;
		}
	}

	#endregion

	#region Methods

	static MRMapChit()
	{
		msMapChitTypes = new Dictionary<eMapChitType, Type>();
	}

	// Called when the script instance is being loaded
	void Awake()
	{
		mCounter = (GameObject)Instantiate(MRGame.TheGame.smallChitPrototype);
		mCounter.transform.parent = transform;

		SideUp = eSide.Back;

		mLongName = "XX";
		mShortName = "X";
	}

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "FrontSide")
			{
				switch (mChitType)
				{
					case eMapChitType.Site:
						sprite.color = MRGame.gold;
						break;
					case eMapChitType.SuperSite:
					case eMapChitType.Sound:
						sprite.color = MRGame.red;
						break;
					case eMapChitType.Warning:
						sprite.color = MRGame.yellow;
						break;
				}
			}
		}
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();

		TextMesh[] components = mCounter.GetComponentsInChildren<TextMesh>();
		foreach (TextMesh text in components)
		{
			if (text.gameObject.name == "FrontText")
			{
				if (MRGame.TheGame.TheMap.MapZoomed || (Stack != null && Stack.Inspecting))
				{
					text.fontSize = SMALL_FONT_SIZE;
					text.text = mLongName;
				}
				else
				{
					text.fontSize = BIG_FONT_SIZE;
					text.text = mShortName;
				}
				break;
			}
		}
	}

	public static MRMapChit Create(eMapChitType chitType)
	{
		GameObject root = new GameObject();
		MRMapChit chit = (MRMapChit)root.AddComponent(chitType.ClazzType());

		return chit;
	}

	public static MRMapChit Create(JSONObject root)
	{
		eMapChitType chitType = (eMapChitType)((JSONNumber)root["type"]).IntValue;
		MRMapChit chit = Create(chitType);
		chit.Load(root);

		return chit;
	}

	public override bool Load(JSONObject root)
	{
		if (!base.Load(root))
			return false;

		mChitType = (eMapChitType)((JSONNumber)root["type"]).IntValue;
		return true;
	}
	
	public override void Save(JSONObject root)
	{
		base.Save(root);
		root["type"] = new JSONNumber((int)mChitType);
		root["set"] = new JSONNumber(1);		// this is in anticipation of multi-set boards
	}

	#endregion

	#region Members
	
	protected eMapChitType mChitType;
	protected MRTile mTile;
	protected bool mSummonedMonsters;
	private string mLongName;
	private string mShortName;

	private static IDictionary<eMapChitType, Type> msMapChitTypes;

	#endregion
}

