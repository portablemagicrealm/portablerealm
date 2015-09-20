//
// MRVictoryPointSelectionDialog.cs
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
using UnityEngine.UI;
using System;
using System.Collections;

public class MRVictoryPointSelectionDialog : MonoBehaviour
{
	#region Properties

	public Text Title;
	public Button GreatTreasureMinus;
	public Button GreatTreasurePlus;
	public Text GreatTreasureValue;
	public Button FameMinus;
	public Button FamePlus;
	public Text FameValue;
	public Button NotorietyMinus;
	public Button NotorietyPlus;
	public Text NotorietyValue;
	public Button GoldMinus;
	public Button GoldPlus;
	public Text GoldValue;
	public Button SpellsMinus;
	public Button SpellsPlus;
	public Text SpellsValue;
	public Button Ok;

	public MRCharacter Character
	{
		get {
			if (mCharacter != null && mCharacter.IsAlive)
				return (MRCharacter)mCharacter.Target;
			return null;
		}

		set {
			mCharacter = new WeakReference(value);
		}
	}

	public MRMainUI.OnButtonPressed Callback
	{
		get{
			return mCallback;
		}

		set{
			mCallback = value;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		GreatTreasurePlus.onClick.AddListener(OnGreatTreasurePlusClicked);
		GreatTreasureMinus.onClick.AddListener(OnGreatTreasureMinusClicked);
		FamePlus.onClick.AddListener(OnFamePlusClicked);
		FameMinus.onClick.AddListener(OnFameMinusClicked);
		NotorietyPlus.onClick.AddListener(OnNotorietyPlusClicked);
		NotorietyMinus.onClick.AddListener(OnNotorietyMinusClicked);
		GoldPlus.onClick.AddListener(OnGoldPlusClicked);
		GoldMinus.onClick.AddListener(OnGoldMinusClicked);
		SpellsPlus.onClick.AddListener(OnSpellsPlusClicked);
		SpellsMinus.onClick.AddListener(OnSpellsMinusClicked);
		Ok.onClick.AddListener(OnOkClicked);
	}

	// Update is called once per frame
	void Update ()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			Title.text = character.Name + " Victory Points";
			Ok.interactable = character.Score.VictoryPointsValid;
			GreatTreasureValue.text = character.Score.VictoryPointsGreatTreasure.ToString();
			FameValue.text = character.Score.VictoryPointsFame.ToString();
			NotorietyValue.text = character.Score.VictoryPointsNotoriety.ToString();
			GoldValue.text = character.Score.VictoryPointsGold.ToString();
			SpellsValue.text = character.Score.VictoryPointsSpell.ToString();
		}
	}

	private void OnGreatTreasurePlusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsGreatTreasure < 5)
				++character.Score.VictoryPointsGreatTreasure;
		}
	}

	private void OnGreatTreasureMinusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsGreatTreasure > 0)
				--character.Score.VictoryPointsGreatTreasure;
		}
	}

	private void OnFamePlusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsFame < 5)
				++character.Score.VictoryPointsFame;
		}
	}
	
	private void OnFameMinusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsFame > 0)
				--character.Score.VictoryPointsFame;
		}
	}

	private void OnNotorietyPlusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsNotoriety < 5)
				++character.Score.VictoryPointsNotoriety;
		}
	}
	
	private void OnNotorietyMinusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsNotoriety > 0)
				--character.Score.VictoryPointsNotoriety;
		}
	}

	private void OnGoldPlusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsGold < 5)
				++character.Score.VictoryPointsGold;
		}
	}
	
	private void OnGoldMinusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsGold > 0)
				--character.Score.VictoryPointsGold;
		}
	}

	private void OnSpellsPlusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsSpell < 5)
				++character.Score.VictoryPointsSpell;
		}
	}
	
	private void OnSpellsMinusClicked()
	{
		MRCharacter character = Character;
		if (character != null)
		{
			if (character.Score.VictoryPointsSpell > 0)
				--character.Score.VictoryPointsSpell;
		}
	}

	private void OnOkClicked()
	{
		if (mCallback != null)
		{
			mCallback(0);
		}
	}

	#endregion

	#region Members

	private WeakReference mCharacter = null;
	private MRMainUI.OnButtonPressed mCallback = null;

	#endregion
}

