//
// MRDenizenManager.cs
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

public class MRDenizenManager
{
	#region Properties
	
	public static IDictionary<uint, MRMonster> Monsters
	{
		get{
			return msMonsters;
		}
	}

	#endregion

	#region Methods
	
	public MRDenizenManager ()
	{
		try
		{
			TextAsset denizenList = (TextAsset)Resources.Load("denizens");
			StringBuilder jsonText = new StringBuilder(denizenList.text);
			JSONObject jsonData = (JSONObject)JSONDecoder.CreateJSONValue(jsonText);

			// parse the monsters data
			JSONArray monstersData = (JSONArray)jsonData["monsters"];
			int count = monstersData.Count;
			for (int i = 0; i < count; ++i)
			{
				object[] monsters = JSONDecoder.DecodeObjects((JSONObject)monstersData[i]);
				if (monsters != null)
				{
					foreach (object monster in monsters)
					{
						((MRMonster)monster).Layer = LayerMask.NameToLayer("Dummy");
						msMonsters.Add(((MRMonster)monster).Id, (MRMonster)monster);
					}
				}
			}
			foreach (MRMonster monster in msMonsters.Values)
			{
				monster.SetOwnership();
			}

			// parse the natives data
			JSONArray nativesData = (JSONArray)jsonData["natives"];
			count = nativesData.Count;
			for (int i = 0; i < count; ++i)
			{
				object[] natives = JSONDecoder.DecodeObjects((JSONObject)nativesData[i]);
				if (natives != null)
				{
					foreach (object native in natives)
					{
						((MRNative)native).Layer = LayerMask.NameToLayer("Dummy");
						msNatives.Add(((MRNative)native).Id, (MRNative)native);
					}
				}
			}
			//foreach (MRNative native in msNatives.Values)
			//{
			//	native.SetOwnership();
			//}
		}
		catch (Exception err)
		{
			Debug.LogError("Error parsing denizens: " + err);
		}
	}

	/// <summary>
	/// Removes all denizens from their current stacks.
	/// </summary>
	public static void ResetDenizens()
	{
		foreach(MRMonster monster in msMonsters.Values)
		{
			if (monster.Stack != null)
			{
				monster.Stack.RemovePiece(monster);
			}
		}
		foreach(MRNative native in msNatives.Values)
		{
			if (native.Stack != null)
			{
				native.Stack.RemovePiece(native);
			}
		}
	}

	public static MRMonster GetMonster(string name, int index)
	{
		return GetMonster(MRUtility.IdForName(name, index));
	}

	public static MRMonster GetMonster(uint id)
	{
		MRMonster monster = null;
		msMonsters.TryGetValue(id, out monster);
		return monster;
	}

	public static MRNative GetNative(string name, int index)
	{
		return GetNative(MRUtility.IdForName(name, index));
	}

	public static MRNative GetNative(uint id)
	{
		MRNative native = null;
		msNatives.TryGetValue(id, out native);
		return native;
	}

	public static void StartMidnight()
	{
		// reset monster conditions at end of day
		foreach (MRMonster monster in msMonsters.Values)
		{
			monster.StartMidnight();
		}
	}

	#endregion

	#region Members

	private static IDictionary<uint, MRMonster> msMonsters = new Dictionary<uint, MRMonster>();
	private static IDictionary<uint, MRNative> msNatives = new Dictionary<uint, MRNative>();

	#endregion
}

}