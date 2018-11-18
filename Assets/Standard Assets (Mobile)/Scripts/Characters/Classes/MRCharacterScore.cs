//
// MRCharacterScore.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2015 Steve Jakab
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
using AssemblyCSharp;

namespace PortableRealm
{
	
public class MRCharacterScore : MRISerializable
{
	#region Constants

	public enum eVictoryType
	{
		GreatTreasure,
		Fame,
		Notoriety,
		Gold,
		Spell
	}

	#endregion

	#region Properties

	public MRCharacter Owner
	{
		get{
			if (mOwner != null && mOwner.IsAlive)
				return (MRCharacter)mOwner.Target;
			return null;
		}

		set{
			mOwner = new WeakReference(value);
		}
	}

	public int VictoryPointsGreatTreasure
	{
		get{
			return mVictoryPoints[eVictoryType.GreatTreasure];
		}

		set{
			mVictoryPoints[eVictoryType.GreatTreasure] = value;
		}
	}

	public int VictoryPointsFame
	{
		get{
			return mVictoryPoints[eVictoryType.Fame];
		}
		
		set{
			mVictoryPoints[eVictoryType.Fame] = value;
		}
	}

	public int VictoryPointsNotoriety
	{
		get{
			return mVictoryPoints[eVictoryType.Notoriety];
		}
		
		set{
			mVictoryPoints[eVictoryType.Notoriety] = value;
		}
	}

	public int VictoryPointsGold
	{
		get{
			return mVictoryPoints[eVictoryType.Gold];
		}
		
		set{
			mVictoryPoints[eVictoryType.Gold] = value;
		}
	}

	public int VictoryPointsSpell
	{
		get{
			return mVictoryPoints[eVictoryType.Spell];
		}
		
		set{
			mVictoryPoints[eVictoryType.Spell] = value;
		}
	}

	public bool VictoryPointsValid
	{
		get{
			return VictoryPointsGreatTreasure +
				VictoryPointsFame +
				VictoryPointsNotoriety +
				VictoryPointsGold +
				VictoryPointsSpell == 5;
		}
	}

	public int VictoryRequirementGreatTreasure
	{
		get{
			return VictoryPointsGreatTreasure;
		}
	}
	
	public int VictoryRequirementFame
	{
		get{
			return VictoryPointsFame * 10;
		}
	}
	
	public int VictoryRequirementNotoriety
	{
		get{
			return VictoryPointsNotoriety * 20;
		}
	}
	
	public int VictoryRequirementGold
	{
		get{
			return VictoryPointsGold * 30;
		}
	}
	
	public int VictoryRequirementSpell
	{
		get{
			return VictoryPointsSpell * 2;
		}
	}

	public int VictoryScoreGreatTreasure
	{
		get{
			if (mOwner.IsAlive)
			{
				MRCharacter character = (MRCharacter)mOwner.Target;
				int treasures = 0;
				IList<MRItem> items = character.Items;
				foreach (MRItem item in items)
				{
					if (item is MRTreasure && ((MRTreasure)item).IsGreatTreasure)
						++treasures;
				}
				treasures -= VictoryRequirementGreatTreasure;
				if (treasures < 0)
					treasures *= 3;
				return treasures;
			}
			return 0;
		}
	}
	
	public int VictoryScoreFame
	{
		get{
			if (mOwner.IsAlive)
			{
				MRCharacter character = (MRCharacter)mOwner.Target;
				float fame = character.EffectiveFame;
				IList<MRItem> items = character.Items;
				foreach (MRItem item in items)
				{
					fame += item.BaseFame;
				}
				fame -= VictoryRequirementFame;
				if (fame < 0)
					fame *= 3;
				return (int)fame;
			}
			return 0;
		}
	}
	
	public int VictoryScoreNotoriety
	{
		get{
			if (mOwner.IsAlive)
			{
				MRCharacter character = (MRCharacter)mOwner.Target;
				float notoriety = character.EffectiveNotoriety;
				IList<MRItem> items = character.Items;
				foreach (MRItem item in items)
				{
					notoriety += item.BaseNotoriety;
				}
				notoriety -= VictoryRequirementNotoriety;
				if (notoriety < 0)
					notoriety *= 3;
				return (int)notoriety;
			}
			return 0;
		}
	}
	
	public int VictoryScoreGold
	{
		get{
			if (mOwner.IsAlive)
			{
				MRCharacter character = (MRCharacter)mOwner.Target;
				int gold = character.EffectiveGold - character.StartingGoldValue;
				gold -= VictoryRequirementGold;
				if (gold < 0)
					gold *= 3;
				return gold;
			}
			return 0;
		}
	}
	
	public int VictoryScoreSpell
	{
		get{
			if (mOwner.IsAlive)
			{
				MRCharacter character = (MRCharacter)mOwner.Target;
				
			}
			return 0;
			//return VictoryRequirementSpell * 2;
		}
	}

	public int VictoryBasicScoreGreatTreasure
	{
		get{
			return VictoryScoreGreatTreasure;
		}
	}
	
	public int VictoryBasicScoreFame
	{
		get{
			return (int)Math.Floor(VictoryScoreFame / 10.0);
		}
	}
	
	public int VictoryBasicScoreNotoriety
	{
		get{
			return (int)Math.Floor(VictoryScoreNotoriety / 20.0);
		}
	}
	
	public int VictoryBasicScoreGold
	{
		get{
			return (int)Math.Floor(VictoryScoreGold / 30.0);
		}
	}
	
	public int VictoryBasicScoreSpell
	{
		get{
			return (int)Math.Floor(VictoryScoreSpell / 2.0);
		}
	}

	public int VictoryBonusScoreGreatTreasure
	{
		get{
			return VictoryBasicScoreGreatTreasure * VictoryPointsGreatTreasure;
		}
	}
	
	public int VictoryBonusScoreFame
	{
		get{
			return VictoryBasicScoreFame * VictoryPointsFame;
		}
	}
	
	public int VictoryBonusScoreNotoriety
	{
		get{
			return VictoryBasicScoreNotoriety * VictoryPointsNotoriety;
		}
	}
	
	public int VictoryBonusScoreGold
	{
		get{
			return VictoryBasicScoreGold * VictoryPointsGold;
		}
	}
	
	public int VictoryBonusScoreSpell
	{
		get{
			return VictoryBasicScoreSpell * VictoryPointsSpell;
		}
	}

	public int VictoryBasicScore
	{
		get{
			return VictoryBasicScoreGreatTreasure +
				VictoryBasicScoreFame +
				VictoryBasicScoreNotoriety +
				VictoryBasicScoreGold +
				VictoryBasicScoreSpell;
		}
	}

	public int VictoryBonusScore
	{
		get{
			return VictoryBonusScoreGreatTreasure +
				VictoryBonusScoreFame +
				VictoryBonusScoreNotoriety +
				VictoryBonusScoreGold +
				VictoryBonusScoreSpell;
		}
	}

	public int VictoryScore
	{
		get{
			if (mOwner.IsAlive)
			{
				MRCharacter character = (MRCharacter)mOwner.Target;
				if (!character.IsDead)
					return VictoryBasicScore + VictoryBonusScore;
				return -100;
			}
			return 0;
		}
	}

	#endregion

	#region Methods

	public MRCharacterScore()
	{
		mOwner = null;
		mVictoryPoints = new Dictionary<eVictoryType, int>();
		mVictoryPoints[eVictoryType.GreatTreasure] = 0;
		mVictoryPoints[eVictoryType.Fame] = 0;
		mVictoryPoints[eVictoryType.Notoriety] = 0;
		mVictoryPoints[eVictoryType.Gold] = 0;
		mVictoryPoints[eVictoryType.Spell] = 0;
	}

	public MRCharacterScore(MRCharacter owner) : this()
	{
		Owner = owner;
	}

	public virtual bool Load(JSONObject root)
	{
		mVictoryPoints.Clear();
		if (root["victorypoints"] != null)
		{
			JSONObject vp = (JSONObject)root["victorypoints"];
			mVictoryPoints[eVictoryType.GreatTreasure] = ((JSONNumber)vp["gt"]).IntValue;
			mVictoryPoints[eVictoryType.Fame] = ((JSONNumber)vp["f"]).IntValue;
			mVictoryPoints[eVictoryType.Notoriety] = ((JSONNumber)vp["n"]).IntValue;
			mVictoryPoints[eVictoryType.Gold] = ((JSONNumber)vp["g"]).IntValue;
			mVictoryPoints[eVictoryType.Spell] = ((JSONNumber)vp["s"]).IntValue;
		}
		else
		{
			// old save, set dummy requirements
			mVictoryPoints[eVictoryType.GreatTreasure] = 1;
			mVictoryPoints[eVictoryType.Fame] = 1;
			mVictoryPoints[eVictoryType.Notoriety] = 1;
			mVictoryPoints[eVictoryType.Gold] = 2;
			mVictoryPoints[eVictoryType.Spell] = 0;
		}
		return true;
	}

	public virtual void Save(JSONObject root)
	{
		JSONObject vp = new JSONObject();
		vp["gt"] = new JSONNumber(mVictoryPoints[eVictoryType.GreatTreasure]);
		vp["f"] = new JSONNumber(mVictoryPoints[eVictoryType.Fame]);
		vp["n"] = new JSONNumber(mVictoryPoints[eVictoryType.Notoriety]);
		vp["g"] = new JSONNumber(mVictoryPoints[eVictoryType.Gold]);
		vp["s"] = new JSONNumber(mVictoryPoints[eVictoryType.Spell]);
		root["victorypoints"] = vp;
	}

	#endregion

	#region Members

	private WeakReference mOwner;
	private Dictionary<eVictoryType, int> mVictoryPoints;

	#endregion
}

}