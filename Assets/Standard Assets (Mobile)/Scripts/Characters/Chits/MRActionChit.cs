//
// MRActionChit.cs
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
	
public abstract class MRActionChit : MRChit
{
	#region Constants

	public enum eType
	{
		Move,
		Fight,
		Magic,
		Duck,
		Berserk,
		Any
	}

	public enum eState
	{
		Active,
		Fatigued,
		Wounded,
		Enchanted
	}

	public enum eAction
	{
		Attack,
		Thrust,
		Swing,
		Smash,
		Move,
		Charge,
		Dodge,
		Duck,
		RunAway,
		ActivateWeapon,
		Fatigue,
		FatigueFight,
		FatigueMove,
		FatigueMagic,
		FatigueChange,
		FatigueChangeFight,
		FatigueChangeMove,
		FatigueChangeMagic,
		Alert,
		CombatAlert,
		EnchantChit,
		EnchantTile,
		SupplyColor,
		CastSpell
	}

	#endregion

	#region Properties

	public abstract eType Type { get; }

	public int BaseTime
	{
		get{
			return mBaseTime;
		}
		
		set{
			mBaseTime = value;
			CurrentTime = value;
		}
	}
	
	public int CurrentTime
	{
		get{
			return mCurrentTime;
		}
		
		set{
			mCurrentTime = value;
		}
	}

	public int BaseAsterisks
	{
		get{
			return mBaseAsterisks;
		}
		
		set{
			mBaseAsterisks = value;
			CurrentAsterisks = value;
		}
	}
	
	public int CurrentAsterisks
	{
		get{
			return mCurrentAsterisks;
		}
		
		set{
			mCurrentAsterisks = value;
		}
	}

	public virtual eState State
	{
		get{
			return mState;
		}

		set{
			mState = value;
		}
	}

	public bool UsedThisRound
	{
		get{
			return mUsedThisRound;
		}

		set{
			mUsedThisRound = value;
		}
	}

	public bool Selectable
	{
		get{
			return mSelectable;
		}

		set{
			mSelectable = value;
		}
	}

	public MRCharacter Owner
	{
		get{
			return mOwner;
		}

		set{
			mOwner = value;
		}
	}

	#endregion

	#region Methods

	// Called when the script instance is being loaded
	void Awake()
	{
		mCounter = (GameObject)Instantiate(MRGame.TheGame.actionChitPrototype);
	}

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		Layer = LayerMask.NameToLayer("Dummy");
		Selectable = false;
		FrontColor = MRGame.tan;
		BackColor = MRGame.tan;

		SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "FrontOverlay")
				mFontSelectable = sprite.gameObject;
			else if (sprite.gameObject.name == "BackOverlay")
				mBackSelectable = sprite.gameObject;
		}
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();

		mFontSelectable.SetActive(Selectable);
		mBackSelectable.SetActive(Selectable);
	}

	public bool CanBeUsedFor(eAction action)
	{
		return CanBeUsedFor(action, MRGame.eStrength.Any);
	}

	public virtual bool CanBeUsedFor(eAction action, MRGame.eStrength strength)
	{
		return false;
	}

	public virtual void Alert(eAction action)
	{
	}

	#endregion

	#region Members

	protected int mBaseTime;
	protected int mCurrentTime;
	protected int mBaseAsterisks;
	protected int mCurrentAsterisks;
	protected eState mState;
	protected bool mUsedThisRound;
	protected bool mSelectable;
	protected GameObject mFontSelectable;
	protected GameObject mBackSelectable;
	protected MRCharacter mOwner;

	#endregion
}

}