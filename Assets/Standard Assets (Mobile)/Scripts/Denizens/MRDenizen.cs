//
// MRDenizen.cs
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

namespace PortableRealm
{
	
public abstract class MRDenizen : MRControllable
{
	#region Internal Classes

	protected class CombatData
	{
		public MRGame.eStrength mStrength;
		public int mAttackSpeed;
		public int mMoveSpeed;
		public int mSharpness;
		public Color32 mColor;

		public CombatData(JSONObject jsonData)
		{
			string strength = ((JSONString)jsonData["strength"]).Value;
			mStrength = strength.Strength();

			mAttackSpeed = ((JSONNumber)jsonData["attack_speed"]).IntValue;
			mMoveSpeed = ((JSONNumber)jsonData["move_speed"]).IntValue;
			mSharpness = ((JSONNumber)jsonData["sharpness"]).IntValue;

			string color = ((JSONString)jsonData["color"]).Value;
			int red = Int32.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			int green = Int32.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			int blue = Int32.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			mColor = new Color32((byte)red, (byte)green, (byte)blue, 255);
		}
	}

	#endregion
	
	#region Constants

	public enum eSide
	{
		Light,
		Dark
	}

	#endregion

	#region Properties

	public bool IsControlled
	{
		get{
			return mIsControlled;
		}

		set{
			mIsControlled = value;
		}
	}

	public MRMonsterChartLocation MonsterBox
	{
		get{
			return mMonsterBox;
		}

		set{
			mMonsterBox = value;
		}
	}

	public eSide Side
	{
		get{
			return mSide;
		}

		set{
			mSide = value;
		}
	}

	public bool Armored
	{
		get{
			return mArmored;
		}
	}

	public int BountyFame
	{
		get{
			return mBountyFame;
		}
	}

	public int BountyNotoriety
	{
		get{
			return mBountyNotoriety;
		}
	}

	public int BountyGold 
	{
		get{
			return mBountyGold;
		}
	}

	/// <summary>
	/// Returns the current attack strength of the controllable.
	/// </summary>
	/// <value>The current strength.</value>
	public override MRGame.eStrength CurrentStrength
	{
		get{
			if (Side == eSide.Light)
				return mLightSide.mStrength;
			else
				return mDarkSide.mStrength;
		}
	}

	/// <summary>
	/// Returns the current attack speed of the controllable.
	/// </summary>
	/// <value>The current attack speed.</value>
	public override int CurrentAttackSpeed
	{
		get{
			if (Side == eSide.Light)
				return mLightSide.mAttackSpeed;
			else
				return mDarkSide.mAttackSpeed;
		}
	}

	/// <summary>
	/// Returns the current move speed of the controllable.
	/// </summary>
	/// <value>The current move speed.</value>
	public override int CurrentMoveSpeed
	{
		get{
			if (Side == eSide.Light)
				return mLightSide.mMoveSpeed;
			else
				return mDarkSide.mMoveSpeed;
		}
	}

	/// <summary>
	/// Returns the current sharpness of the controllable's active weapon.
	/// </summary>
	/// <value>The current sharpness.</value>
	public override int CurrentSharpness
	{
		get{
			if (Side == eSide.Light)
				return mLightSide.mSharpness;
			else
				return mDarkSide.mSharpness;
		}
	}

	/// <summary>
	/// Returns the type of weapon used by the denizen.
	/// </summary>
	/// <value>The type of the weapon.</value>
	public override MRWeapon.eWeaponType WeaponType 
	{ 
		get{
			return MRWeapon.eWeaponType.Striking;
		}
	}

	/// <summary>
	/// Gets the length of the controllable's active weapon.
	/// </summary>
	/// <value>The length of the weapon.</value>
	public override int WeaponLength
	{
		get{
			return mLength;
		}
	}

	/// <summary>
	/// Returns the attack direction being used this round.
	/// </summary>
	/// <value>The attack type.</value>
	public override MRCombatManager.eAttackType AttackType 
	{ 
		get
		{
			MRCombatManager.eAttackType attack = MRCombatManager.eAttackType.None;
			if (CombatSheet != null && CombatTarget != null)
			{
				MRCombatSheetData.AttackerData attackData = CombatSheet.FindAttacker(this);
				if (attackData != null)
					attack = attackData.attackType;
				else
				{
					// try to infer the attack from the defense
					MRCombatSheetData.DefenderData defenseData = CombatSheet.FindDefender(this);
					if (defenseData != null)
					{
						switch (defenseData.defenseType)
						{
							case MRCombatManager.eDefenseType.Charge:
								attack = MRCombatManager.eAttackType.Thrust;
								break;
							case MRCombatManager.eDefenseType.Dodge:
								attack = MRCombatManager.eAttackType.Swing;
								break;
							case MRCombatManager.eDefenseType.Duck:
								attack = MRCombatManager.eAttackType.Smash;
								break;
							default:
								break;
						}
					}
				}
			}
			return attack;
		}
	}
	
	/// <summary>
	/// Returns the maneuver direction being used this round.
	/// </summary>
	/// <value>The defense type.</value>
	public override MRCombatManager.eDefenseType DefenseType 
	{
		get
		{
			MRCombatManager.eDefenseType defense = MRCombatManager.eDefenseType.None;
			if (CombatSheet != null)
			{
				MRCombatSheetData.DefenderData defenseData = CombatSheet.FindDefender(this);
				if (defenseData != null)
					defense = defenseData.defenseType;
				else
				{
					// try to infer the defense from the attack
					MRCombatSheetData.AttackerData attackData = CombatSheet.FindAttacker(this);
					if (attackData != null)
					{
						switch (attackData.attackType)
						{
							case MRCombatManager.eAttackType.Thrust:
								defense = MRCombatManager.eDefenseType.Charge;
								break;
							case MRCombatManager.eAttackType.Swing:
								defense = MRCombatManager.eDefenseType.Dodge;
								break;
							case MRCombatManager.eAttackType.Smash:
								defense = MRCombatManager.eDefenseType.Duck;
								break;
							default:
								break;
						}
					}
				}
			}
			return defense;
		}
	}

	// How the controllable is moving.
	public override MRGame.eMoveType MoveType 
	{
		get{
			return MRGame.eMoveType.Walk;
		}

		set{
		}
	}

	public override Vector3 OldScale 
	{ 
		get{
			return Vector3.one;
		} 

		set{
		} 
	}

	#endregion

	#region Methods

	protected MRDenizen()
	{
	}

	protected MRDenizen(JSONObject jsonData, int index) :
		base((JSONObject)jsonData["MRControllable"], index)
	{
		mImageName = ((JSONString)jsonData["image"]).Value;
		
		mBountyFame = ((JSONNumber)jsonData["fame"]).IntValue;
		mBountyNotoriety = ((JSONNumber)jsonData["notoriety"]).IntValue;
		mBountyGold = ((JSONNumber)jsonData["gold"]).IntValue;
		mLength = ((JSONNumber)jsonData["length"]).IntValue;
		mArmored = ((JSONBoolean)jsonData["armored"]).Value;
		
		mLightSide = new CombatData((JSONObject)jsonData["light_side"]);
		mDarkSide = new CombatData((JSONObject)jsonData["dark_side"]);

		CreateCounter();
		if (mCounter == null)
			return;

		Sprite texture = null;
		if (!String.IsNullOrEmpty(mImageName))
			texture = (Sprite)Resources.Load("Textures/" + mImageName, typeof(Sprite));
		SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "FrontSide")
			{
				sprite.color = mLightSide.mColor;
			}
			else if (sprite.gameObject.name == "BackSide")
			{
				sprite.color = mDarkSide.mColor;
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
				if (mLightSide.mStrength != MRGame.eStrength.Negligable)
					buffer.Append(mLightSide.mStrength.ToChitString());
				if (mLightSide.mAttackSpeed >= 0)
					buffer.Append(mLightSide.mAttackSpeed);
				if (mLightSide.mAttackSpeed >= 0 && mLightSide.mMoveSpeed >= 0)
					buffer.Append("-");
				if (mLightSide.mMoveSpeed >= 0)
					buffer.Append(mLightSide.mMoveSpeed);
				for (int i = 0; i < mLightSide.mSharpness; ++i)
					buffer.Append("*");
				text.text = buffer.ToString();
			}
			else if (text.gameObject.name == "BackText")
			{
				buffer.Length = 0;
				if (mDarkSide.mStrength != MRGame.eStrength.Negligable)
					buffer.Append(mDarkSide.mStrength.ToChitString());
				if (mDarkSide.mAttackSpeed >= 0)
					buffer.Append(mDarkSide.mAttackSpeed);
				if (mDarkSide.mAttackSpeed >= 0 && mDarkSide.mMoveSpeed >= 0)
					buffer.Append("-");
				if (mDarkSide.mMoveSpeed >= 0)
					buffer.Append(mDarkSide.mMoveSpeed);
				for (int i = 0; i < mDarkSide.mSharpness; ++i)
					buffer.Append("*");
				text.text = buffer.ToString();
			}
		}
	}

	/// <summary>
	/// Sets mCounter to the right counter to use for this denizen.
	/// </summary>
	protected abstract void CreateCounter();

	// Does initialization associated with birdsong.
	public override void StartBirdsong()
	{
	}
	
	// Does initialization associated with sunrise.
	public override void StartSunrise()
	{
	}
	
	// Does initialization associated with daylight.
	public override void StartDaylight()
	{
	}
	
	// Does initialization associated with sunset.
	public override void StartSunset()
	{
	}
	
	// Does initialization associated with evening.
	public override void StartEvening()
	{
	}
	
	// Does initialization associated with midnight.
	public override void StartMidnight()
	{
		base.StartMidnight();

		Side = eSide.Light;
	}
	
	/// <summary>
	/// Tests if the denizen is allowed to do the activity.
	/// </summary>
	/// <returns><c>true</c> if this instance can execute the specified activity; otherwise, <c>false</c>.</returns>
	/// <param name="activity">Activity.</param>
	public override bool CanExecuteActivity(MRActivity activity)
	{
		if (activity is MREnchantActivity)
			return false;

		return true;
	}
	
	// Tells the controllable an activity was performed
	public override void ExecutedActivity(MRActivity activity)
	{
	}
	
	// Add an item to the controllable's items.
	public override void AddItem(MRItem item)
	{
	}
	
	// Returns the weight of the heaviest item owned by the controllable
	public override MRGame.eStrength GetHeaviestWeight(bool includeHorse, bool includeSelf)
	{
		return MRGame.eStrength.Negligable;
	}

	/// <summary>
	/// Awards the spoils of combat for killing a combatant.
	/// </summary>
	/// <param name="killed">Combatant that was killed.</param>
	/// <param name="awardFraction">Fraction of the spoils to award (due to shared kills).</param>
	public override void AwardSpoils(MRIControllable killed, float awardFraction)
	{
		// todo: if this is a controlled creature, award to controller
	}

	/// <summary>
	/// Flips the denizen to its other side.
	/// </summary>
	public void Flip()
	{
		Side = (Side == eSide.Light ? eSide.Dark : eSide.Light);
	}

	public override void Update()
	{
		if (mCounter != null)
		{
			Vector3 orientation = EulerAngles;
			if (Side == eSide.Light)
				orientation.y = 0;
			else
				orientation.y = 180f;
			EulerAngles = orientation;
		}
	}

	#endregion

	#region Members

	protected bool mIsControlled;
	protected eSide mSide;

	protected string mImageName;
	protected int mBountyFame;
	protected int mBountyNotoriety;
	protected int mBountyGold;
	protected int mLength;
	protected bool mArmored;
	protected CombatData mLightSide;
	protected CombatData mDarkSide;

	protected MRMonsterChartLocation mMonsterBox;
	
	#endregion
}

}