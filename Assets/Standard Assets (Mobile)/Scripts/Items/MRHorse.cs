//
// MRHorse.cs
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
using System.Text;
using AssemblyCSharp;

public class MRHorse : MRItem
{
	#region Constants

	public enum eType
	{
		Pony,
		Workhorse,
		Warhorse
	}

	public enum eMoveType
	{
		Walk,
		Gallop
	}

	#endregion

	#region Properties

	public eType Type
	{
		get{
			return mType;
		}
	}

	public eMoveType MoveType
	{
		get{
			return mMoveType;
		}
		set{
			mMoveType = value;
		}
	}
	
	public MRGame.eStrength WalkStrength
	{
		get{
			return mWalkStrength;
		}
	}

	public MRGame.eStrength GallopStrength
	{
		get{
			return mGallopStrength;
		}
	}

	public MRGame.eStrength CurrentStrength
	{
		get{
			if (MoveType == eMoveType.Walk)
				return WalkStrength;
			else
				return GallopStrength;
		}
	}

	public int WalkSpeed
	{
		get{
			return mWalkSpeed;
		}
	}

	public int GallopSpeed
	{
		get{
			return mGallopSpeed;
		}
	}

	public int CurrentSpeed
	{
		get{
			if (MoveType == eMoveType.Walk)
				return WalkSpeed;
			else
				return GallopSpeed;
		}
	}

	public bool LikesIt
	{
		get{
			return false;
		}
	}

	#endregion

	#region Methods

	public MRHorse()
	{
	}

	public MRHorse(JSONObject data, int index) : base((JSONObject)data["MRItem"], index)
	{
		if (Name.Equals("pony", System.StringComparison.OrdinalIgnoreCase))
			mType = eType.Pony;
		else if (Name.Equals("workhorse", System.StringComparison.OrdinalIgnoreCase))
			mType = eType.Workhorse;
		else if (Name.Equals("warhorse", System.StringComparison.OrdinalIgnoreCase))
			mType = eType.Warhorse;
		else
		{
			Debug.LogError("Unknown horse name" + Name);
		}

		mMoveType = eMoveType.Walk;

		string strength = ((JSONString)data["walk_strenth"]).Value;
		mWalkStrength = strength.Strength();
		strength = ((JSONString)data["gallop_strenth"]).Value;
		mGallopStrength = strength.Strength();

		mWalkSpeed = ((JSONNumber)data["walk_speed"]).IntValue;
		mGallopSpeed = ((JSONNumber)data["gallop_speed"]).IntValue;

		mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.largeCounterPrototype);
		string imageName = ((JSONString)data["image"]).Value;
		Sprite texture = (Sprite)Resources.Load("Textures/" + imageName, typeof(Sprite));
		SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "FrontSide")
			{
				sprite.color = MRGame.yellowGreen;
			}
			else if (sprite.gameObject.name == "BackSide")
			{
				sprite.color = MRGame.yellowGreen;
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
				buffer.Append(WalkStrength.ToChitString());
				buffer.Append(WalkSpeed);
				text.text = buffer.ToString();
			}
			else if (text.gameObject.name == "BackText")
			{
				buffer.Length = 0;
				buffer.Append(GallopStrength.ToChitString());
				buffer.Append(GallopSpeed);
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
		if (mMoveType == eMoveType.Gallop)
			orientation.y = 180f;
		else
			orientation.y = 0;
		mCounter.transform.localEulerAngles = orientation;
	}

	#endregion

	#region Members

	private eType mType;
	private eMoveType mMoveType;
	private MRGame.eStrength mWalkStrength;
	private int mWalkSpeed;
	private MRGame.eStrength mGallopStrength;
	private int mGallopSpeed;

	#endregion
}

