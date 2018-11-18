//
// MRMagicChit.cs
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
using System.Text;

namespace PortableRealm
{

public class MRMagicChit : MRActionChit, MRIColorSource
{
	#region Constants

	public static Dictionary<int, MRGame.eMagicColor> MagicMap = new Dictionary<int, MRGame.eMagicColor>()
	{
		{1, MRGame.eMagicColor.White},
		{2, MRGame.eMagicColor.Grey},
		{3, MRGame.eMagicColor.Gold},
		{4, MRGame.eMagicColor.Purple},
		{5, MRGame.eMagicColor.Black},
		{6, MRGame.eMagicColor.None},
		{7, MRGame.eMagicColor.None},
		{8, MRGame.eMagicColor.None},
	};

	#endregion

	#region Properties

	public override eType Type
	{
		get{
			return eType.Magic;
		}
	}

	public int BaseMagicType
	{
		get{
			return mBaseMagicType;
		}
		
		set{
			mBaseMagicType = value;
			CurrentMagicType = value;
		}
	}
	
	public int CurrentMagicType
	{
		get{
			return mCurrentMagicType;
		}
		
		set{
			mCurrentMagicType = value;
		}
	}

	public bool IsEnchanted
	{
		get{
			return mEnchanted;
		}
		set{
			mEnchanted = value;
			if (mEnchanted)
				mState = eState.Enchanted;
			SideUp = mEnchanted ? eSide.Back : eSide.Front;
		}
	}

	public MRGame.eMagicColor MagicColor
	{
		get{
			return MagicMap[CurrentMagicType];
		}
	}

	public IList<MRGame.eMagicColor> MagicSupplied 
	{ 
		get {
			IList<MRGame.eMagicColor> color = new List<MRGame.eMagicColor>();
			color.Add(MagicColor);
			return color;
		}
	}

	public override eState State
	{
		set{
			base.State = value;
			// magic chits can only be enchanted if active
			if (State != eState.Active)
				IsEnchanted = false;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		mChitText = mCounter.GetComponentInChildren<TextMesh>();

		if (MagicColor != MRGame.eMagicColor.None)
			BackColor = MRGame.MagicColorMap[MagicColor];

		SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "BackImage")
			{
				Sprite texture = (Sprite)Resources.Load(mOwner.IconName, typeof(Sprite));
				sprite.sprite = texture;
				break;
			}
		}
	}

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();

		if (!mEnchanted)
		{
			StringBuilder buffer = new StringBuilder("MAGIC\n");
			buffer.Append(CurrentMagicType.ToRomanNumeral());
			buffer.Append(" ");
			buffer.Append(CurrentTime);
			buffer.Append("\n");
			for (int i = 0; i < CurrentAsterisks; ++i)
			{
				buffer.Append("*");
				if (i < CurrentAsterisks - 1)
					buffer.Append(" ");
			}
			mChitText.text = buffer.ToString();
		}
	}

	public override bool CanBeUsedFor(eAction action, MRGame.eStrength strength)
	{
		bool canBeUsed = false;
		if (strength == MRGame.eStrength.Any)
		{
			switch (action)
			{
				case eAction.Alert:
				case eAction.CastSpell:
					canBeUsed = (!mEnchanted && State == MRActionChit.eState.Active);
					break;
				case eAction.EnchantChit:
				case eAction.EnchantTile:
					canBeUsed = (!mEnchanted && State == MRActionChit.eState.Active && CurrentMagicType <= 5);
					break;
				case eAction.Fatigue:
				case eAction.FatigueMagic:
					canBeUsed = (!mEnchanted && State == MRActionChit.eState.Active && BaseAsterisks > 0);
					break;
				case eAction.FatigueChange:
				case eAction.FatigueChangeMagic:
					canBeUsed = (!mEnchanted && State == MRActionChit.eState.Fatigued && BaseAsterisks == 1);
					break;
				case eAction.SupplyColor:
					canBeUsed = (mEnchanted && State == MRActionChit.eState.Enchanted && CurrentMagicType <= 5);
					break;
			}
		}
		return canBeUsed;
	}
	
	public override void Alert(eAction action)
	{
		if (action == eAction.Alert)
		{
			CurrentTime = 0;
		}
	}

	#endregion

	#region Members

	private int mBaseMagicType;
	private int mCurrentMagicType;
	private bool mEnchanted;
	private TextMesh mChitText;

	#endregion
}

}