//
// MRCharacterScoreDisplay.cs
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
using System.Collections;

namespace PortableRealm
{
	
public class MRCharacterScoreDisplay : MRTabItems
{
	#region Properties

	public TextMesh GreateTreasureBaseValue;
	public TextMesh GreateTreasureScore;
	public TextMesh GreateTreasureBasicScore;
	public TextMesh GreateTreasureBonusScore;
	public TextMesh SpellsBaseValue;
	public TextMesh SpellsScore;
	public TextMesh SpellsBasicScore;
	public TextMesh SpellsBonusScore;
	public TextMesh FameBaseValue;
	public TextMesh FameScore;
	public TextMesh FameBasicScore;
	public TextMesh FameBonusScore;
	public TextMesh NotorietyBaseValue;
	public TextMesh NotorietyScore;
	public TextMesh NotorietyBasicScore;
	public TextMesh NotorietyBonusScore;
	public TextMesh GoldBaseValue;
	public TextMesh GoldScore;
	public TextMesh GoldBasicScore;
	public TextMesh GoldBonusScore;
	public TextMesh TotalBasicScore;
	public TextMesh TotalBonusScore;
	public TextMesh TotalScore;

	public MRCharacterMat Parent
	{
		get{
			return mParent;
		}
		
		set{
			mParent = value;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update();

		if (Parent.Controllable != null && Parent.Controllable is MRCharacter)
		{
			MRCharacter character = (MRCharacter)Parent.Controllable;

			GreateTreasureBaseValue.text = "";
			GreateTreasureScore.text = character.Score.VictoryScoreGreatTreasure.ToString();
			GreateTreasureBasicScore.text = character.Score.VictoryBasicScoreGreatTreasure.ToString();
			GreateTreasureBonusScore.text = character.Score.VictoryBonusScoreGreatTreasure.ToString();
			SpellsBaseValue.text = "";
			SpellsScore.text = character.Score.VictoryScoreSpell.ToString();
			SpellsBasicScore.text = character.Score.VictoryBasicScoreSpell.ToString();
			SpellsBonusScore.text = character.Score.VictoryBonusScoreSpell.ToString();
			FameBaseValue.text = "";
			FameScore.text = character.Score.VictoryScoreFame.ToString();
			FameBasicScore.text = character.Score.VictoryBasicScoreFame.ToString();
			FameBonusScore.text = character.Score.VictoryBonusScoreFame.ToString();
			NotorietyBaseValue.text = "";
			NotorietyScore.text = character.Score.VictoryScoreNotoriety.ToString();
			NotorietyBasicScore.text = character.Score.VictoryBasicScoreNotoriety.ToString();
			NotorietyBonusScore.text = character.Score.VictoryBonusScoreNotoriety.ToString();
			GoldBaseValue.text = "";
			GoldScore.text = character.Score.VictoryScoreGold.ToString();
			GoldBasicScore.text = character.Score.VictoryBasicScoreGold.ToString();
			GoldBonusScore.text = character.Score.VictoryBonusScoreGold.ToString();
			TotalBasicScore.text = character.Score.VictoryBasicScore.ToString();
			TotalBonusScore.text = character.Score.VictoryBonusScore.ToString();
			TotalScore.text = character.Score.VictoryScore.ToString();
		}
		else
		{
			GreateTreasureBaseValue.text = "";
			GreateTreasureScore.text = "";
			GreateTreasureBasicScore.text = "";
			GreateTreasureBonusScore.text = "";
			SpellsBaseValue.text = "";
			SpellsScore.text = "";
			SpellsBasicScore.text = "";
			SpellsBonusScore.text = "";
			FameBaseValue.text = "";
			FameScore.text = "";
			FameBasicScore.text = "";
			FameBonusScore.text = "";
			NotorietyBaseValue.text = "";
			NotorietyScore.text = "";
			NotorietyBasicScore.text = "";
			NotorietyBonusScore.text = "";
			GoldBaseValue.text = "";
			GoldScore.text = "";
			GoldBasicScore.text = "";
			GoldBonusScore.text = "";
			TotalBasicScore.text = "";
			TotalBonusScore.text = "";
			TotalScore.text = "";
		}
	}

	#endregion

	#region Members

	private MRCharacterMat mParent;

	#endregion
}

}