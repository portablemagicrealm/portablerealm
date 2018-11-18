//
// MRCharacterManager.cs
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
using System.Text;
using AssemblyCSharp;

namespace PortableRealm
{

public class MRCharacterManager
{
	#region constants

	private static readonly IDictionary<MRGame.eCharacters, string> CharacterMap = new Dictionary<MRGame.eCharacters, string>
	{
		{MRGame.eCharacters.Amazon, "amazon"},
		{MRGame.eCharacters.Berserker, "berserker"},
		{MRGame.eCharacters.BlackKnight, "black knight"},
		{MRGame.eCharacters.Captain, "captain"},
		{MRGame.eCharacters.Druid, "druid"},
		{MRGame.eCharacters.Dwarf, "dwarf"},
		{MRGame.eCharacters.Elf, "elf"},
		{MRGame.eCharacters.Magician, "magician"},
		{MRGame.eCharacters.Pilgrim, "pilgrim"},
		{MRGame.eCharacters.Sorceror, "sorceror"},
		{MRGame.eCharacters.Swordsman, "swordsman"},
		{MRGame.eCharacters.WhiteKnight, "white knight"},
		{MRGame.eCharacters.Witch, "witch"},
		{MRGame.eCharacters.WitchKing, "witch king"},
		{MRGame.eCharacters.Wizard, "wizard"},
		{MRGame.eCharacters.WoodsGirl, "woods girl"}
	};

	#endregion

	#region Methods

	public MRCharacterManager()
	{
		try
		{
			TextAsset charactersList = (TextAsset)Resources.Load("characters");
			StringBuilder jsonText = new StringBuilder(charactersList.text);
			JSONObject jsonData = (JSONObject)JSONDecoder.CreateJSONValue(jsonText);

			JSONArray charactersData = (JSONArray)jsonData["characters"];
			int count = charactersData.Count;
			for (int i = 0; i < count; ++i)
			{
				JSONObject characterData = (JSONObject)charactersData[i];
				String characterName = ((JSONString)characterData["name"]).Value;
				mCharactersData.Add(characterName.ToLower(), characterData);
			}
		}
		catch (Exception err)
		{
			Debug.LogError("Error parsing character data:" + err);
		}
	}

	public MRCharacter CreateCharacter(MRGame.eCharacters characterId)
	{
		return CreateCharacter(CharacterMap[characterId]);
	}

	public MRCharacter CreateCharacter(string name)
	{
		MRCharacter character = null;
		try
		{
			JSONObject characterData;
			if (!mCharactersData.TryGetValue(name.ToLower(), out characterData))
			{
				Debug.LogError("No character data for " + name);
				return null;
			}

			string className = ((JSONString)characterData["class"]).Value;
			className = "PortableRealm." + className;
			Type t = Type.GetType(className);
			if (t == null)
			{
				Debug.LogError("Unable to find character class " + className);
				return null;
			}
			ConstructorInfo cinfo = t.GetConstructor(new Type[] {typeof(JSONObject), typeof(int)});
			if (cinfo == null)
			{
				Debug.LogError("Unable to construct character class " + className);
				return null;
			}
			character = (MRCharacter)cinfo.Invoke(new object[] {characterData, 0});
		}
		catch (Exception err)
		{
			Debug.LogError(err.ToString());
		}
		return character;
	}

	#endregion

	#region Members

	private IDictionary<string, JSONObject> mCharactersData = new Dictionary<string, JSONObject>();

	#endregion
}

}