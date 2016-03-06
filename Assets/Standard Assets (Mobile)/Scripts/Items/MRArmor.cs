//
// MRArmor.cs
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
using AssemblyCSharp;

public class MRArmor : MRItem
{
	public enum eType
	{
		Shield,
		Helmet,
		Breastplate,
		Full
	}

	public enum eState
	{
		Undamaged,
		Damaged,
		Destroyed
	}

	#region Properties

	public eType Type
	{
		get{
			return mType;
		}
	}

	public eState State
	{
		get{
			return mState;
		}

		set{
			mState = value;
		}
	}

	public int DamagedPrice
	{
		get{
			return mDamagedPrice;
		}
	}

	public int DestroyedPrice
	{
		get{
			return mDestroyedPrice;
		}
	}

	public int CurrentPrice
	{
		get{
			int price = BasePrice;
			switch (mState)
			{
				case eState.Damaged:
					price = DamagedPrice;
					break;
				case eState.Destroyed:
					price = DestroyedPrice;
					break;
				default:
					break;
			}
			return price;
		}
	}

	public Nullable<MRNative.eGroup> NativeOwner
	{
		get{
			return mNativeOwner;
		}
		
		set{
			mNativeOwner = value;
		}
	}

	public override int SortValue
	{
		get{
			return mType == eType.Full ? (int)MRGame.eSortValue.LargeArmor : (int)MRGame.eSortValue.SmallArmor;
		}
	}

	#endregion

	#region Methods

	public MRArmor()
	{
	}

	public MRArmor(JSONObject data, int index) : 
		base((JSONObject)data["MRItem"], index)
	{
		mState = eState.Undamaged;

		string type = ((JSONString)data["type"]).Value;
		mType = type.Armor();

		mDamagedPrice = ((JSONNumber)data["damagedprice"]).IntValue;
		mDestroyedPrice = ((JSONNumber)data["destroyedprice"]).IntValue;
		bool isTreasure = ((JSONBoolean)data["treasure"]).Value;

		if (mType != eType.Full)
			mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.mediumCounterPrototype);
		else
			mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.largeCounterPrototype);
		string imageName = ((JSONString)data["image"]).Value;
		Sprite texture = (Sprite)Resources.Load("Textures/" + imageName, typeof(Sprite));
		SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "FrontSide")
			{
				if (isTreasure)
					sprite.color = MRGame.gold;
				else
					sprite.color = MRGame.lightGrey;
			}
			else if (sprite.gameObject.name == "BackSide")
			{
				if (isTreasure)
					sprite.color = MRGame.yellow;
				else
					sprite.color = MRGame.offWhite;
			}
			else if (sprite.gameObject.name == "FrontSymbol")
			{
				sprite.sprite = texture;
			}
			else if (sprite.gameObject.name == "BackSymbol")
			{	
				sprite.sprite = texture;
			}
		}
		TextMesh[] texts = mCounter.GetComponentsInChildren<TextMesh>();
		foreach (TextMesh text in texts)
		{
			if (text.gameObject.name == "FrontText")
			{
				text.text = BaseWeight.ToChitString();
			}
			else if (text.gameObject.name == "BackText")
			{
				text.text = BaseWeight.ToChitString();
			}
		}
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();

		Vector3 orientation = mCounter.transform.localEulerAngles;
		if (State == eState.Damaged || State == eState.Destroyed)
			orientation.y = 180f;
		else
			orientation.y = 0;
		mCounter.transform.localEulerAngles = orientation;
	}

	#endregion

	#region Members

	private eType mType;
	private eState mState;
	private int mDamagedPrice;
	private int mDestroyedPrice;
	private Nullable<MRNative.eGroup> mNativeOwner;

	#endregion
}

