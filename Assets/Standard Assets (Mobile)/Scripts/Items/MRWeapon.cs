//
// MRWeapon.cs
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
using System.Text;
using AssemblyCSharp;

public class MRWeapon : MRItem
{
	#region Constants

	public enum eWeaponType
	{
		Striking,
		Missile
	}

	#endregion

	#region Properties

	public int Length
	{
		get{
			return mLength;
		}
	}

	public bool IsMissile
	{
		get{
			return mIsMissile;
		}
	}

	public MRGame.eStrength AlertedStrength
	{
		get {
			return mAlertedStrength;
		}
	}

	public MRGame.eStrength UnalertedStrength
	{
		get {
			return mUnalertedStrength;
		}
	}

	public MRGame.eStrength CurrentStrength
	{
		get {
			return Alerted ? mAlertedStrength : mUnalertedStrength;
		}
	}

	public int AlertedSharpness
	{
		get{
			return mAlertedSharpness;
		}
	}

	public int UnalertedSharpness
	{
		get{
			return mUnalertedSharpness;
		}
	}

	public int CurrentSharpness
	{
		get{
			return Alerted ? AlertedSharpness : UnalertedSharpness;
		}
	}

	public int AlertedSpeed
	{
		get{
			return mAlertedSpeed;
		}
	}
	
	public int UnalertedSpeed
	{
		get{
			return mUnalertedSpeed;
		}
	}

	public int CurrentSpeed
	{
		get{
			return Alerted ? AlertedSpeed : UnalertedSpeed;
		}
	}

	public bool Alerted
	{
		get{
			return mAlerted;
		}

		set{
			mAlerted = value;
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
			return (int)MRGame.eSortValue.Weapon;
		}
	}

	#endregion

	#region Methods

	public MRWeapon()
	{
	}

	public MRWeapon(JSONObject data, int index) : base((JSONObject)data["MRItem"], index)
	{
		mAlerted = false;

		mLength = ((JSONNumber)data["length"]).IntValue;

		string strength = ((JSONString)data["astrength"]).Value;
		if (strength.Length > 0)
			mAlertedStrength = strength.Strength();
		else
			mAlertedStrength = MRGame.eStrength.Chit;
		strength = ((JSONString)data["istrength"]).Value;
		if (strength.Length > 0)
			mUnalertedStrength = strength.Strength();
		else
			mUnalertedStrength = MRGame.eStrength.Chit;

		mAlertedSharpness = ((JSONNumber)data["asharp"]).IntValue;
		mUnalertedSharpness = ((JSONNumber)data["isharp"]).IntValue;
		mAlertedSpeed = ((JSONNumber)data["aspeed"]).IntValue;
		mUnalertedSpeed = ((JSONNumber)data["ispeed"]).IntValue;
		mIsMissile = ((JSONBoolean)data["missile"]).Value;
		bool isTreasure = ((JSONBoolean)data["treasure"]).Value;

		mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.mediumCounterPrototype);
		string imageName = ((JSONString)data["image"]).Value;
		Sprite texture = (Sprite)Resources.Load("Textures/" + imageName, typeof(Sprite));
		SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "FrontSide")
			{
				if (isTreasure)
					sprite.color = MRGame.yellow;
				else
					sprite.color = MRGame.offWhite;
			}
			else if (sprite.gameObject.name == "BackSide")
			{
				if (isTreasure)
					sprite.color = MRGame.gold;
				else
					sprite.color = MRGame.red;
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
		StringBuilder buffer = new StringBuilder();
		TextMesh[] texts = mCounter.GetComponentsInChildren<TextMesh>();
		foreach (TextMesh text in texts)
		{
			if (text.gameObject.name == "FrontText")
			{
				buffer.Length = 0;
				buffer.Append(mUnalertedStrength.ToChitString());
				if (mUnalertedSpeed > 0)
					buffer.Append(mUnalertedSpeed);
				for (int i = 0; i < mUnalertedSharpness; ++i)
					buffer.Append("*");
				text.text = buffer.ToString();
			}
			else if (text.gameObject.name == "BackText")
			{
				buffer.Length = 0;
				buffer.Append(mAlertedStrength.ToChitString());
				if (mAlertedSpeed > 0)
					buffer.Append(mAlertedSpeed);
				for (int i = 0; i < mAlertedSharpness; ++i)
					buffer.Append("*");
				text.text = buffer.ToString();
			}
		}
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();

		Vector3 orientation = mCounter.transform.localEulerAngles;
		if (mAlerted)
			orientation.y = 180f;
		else
			orientation.y = 0;
		mCounter.transform.localEulerAngles = orientation;
	}

	public override bool Load(JSONObject root)
	{
		if (!base.Load(root))
			return false;

		mAlerted = ((JSONBoolean)root["alert"]).Value;

		return true;
	}
	
	public override void Save(JSONObject root)
	{
		base.Save(root);

		root["alert"] = new JSONBoolean(mAlerted);
	}

	#endregion

	#region Properties

	private MRGame.eStrength mAlertedStrength;
	private MRGame.eStrength mUnalertedStrength;
	private int mAlertedSharpness;
	private int mUnalertedSharpness;
	private int mAlertedSpeed;
	private int mUnalertedSpeed;
	private int mLength;
	private bool mIsMissile;
	private bool mAlerted;
	private Nullable<MRNative.eGroup> mNativeOwner;

	#endregion
}

