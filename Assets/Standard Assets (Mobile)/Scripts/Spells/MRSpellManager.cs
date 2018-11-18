//
// MRSpellManager.cs
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
using System.Text;
using AssemblyCSharp;

namespace PortableRealm
{

public class MRSpellManager
{
	#region Properties

	public static IDictionary<int, IList<MRSpell>> SpellsByType
	{
		get{
			return msSpellsByType;
		}
	}

	#endregion

	#region Methods

	public MRSpellManager()
	{
		for (int type = 1; type <= 8; ++type)
		{
			msSpellsByType[type] = new List<MRSpell>();
		}

		TextAsset itemsList = (TextAsset)Resources.Load("Spells");
		StringBuilder jsonText = new StringBuilder(itemsList.text);
		JSONObject jsonData = (JSONObject)JSONDecoder.CreateJSONValue(jsonText);

		JSONArray spellsData = (JSONArray)jsonData["spells"];
		int count = spellsData.Count;
		for (int i = 0; i < count; ++i)
		{
			object[] spells = JSONDecoder.DecodeObjects((JSONObject)spellsData[i]);
			if (spells != null)
			{
				foreach (object obj in spells)
				{
					MRSpell spell = (MRSpell)obj;
					msSpells[spell.Id] = spell;
					msSpellsByType[spell.CurrentMagicType].Add(spell);
				}
			}
		}
	}

	public static MRSpell GetSpell(uint id)
	{
		MRSpell spell;
		if (msSpells.TryGetValue(id, out spell))
			return spell;
		Debug.LogError("Request for unknown spell id " + id);
		return null;
	}

	#endregion

	#region Members

	private static IDictionary<uint, MRSpell> msSpells = new Dictionary<uint, MRSpell>();
	private static IDictionary<int, IList<MRSpell>> msSpellsByType = new Dictionary<int, IList<MRSpell>>();

	#endregion
}

}

