//
// MRTreasureChart.cs
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

public class MRTreasureChart : MonoBehaviour
{
	#region Properties

	public bool Visible
	{
		get{
			return mCamera.enabled;
		}

		set{
			mCamera.enabled = value;
		}
	}

	public MRGamePieceStack DestroyedItems
	{
		get{
			return mDestroyedItems;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		foreach (Camera camera in Camera.allCameras)
		{
			if (camera.name == "Treasure Camera")
			{
				mCamera = camera;
				break;
			}
		}

		// initialize the stacks
		MRGamePieceStack stack;
		foreach (MRMapChit.eSiteChitType site in Enum.GetValues(typeof(MRMapChit.eSiteChitType)))
		{
			stack = MRGame.TheGame.NewGamePieceStack();
			stack.Layer = gameObject.layer;
			stack.transform.parent = transform;
			mSiteTreasures.Add(site, stack);
		}
		foreach (MRNative.eGroup native in Enum.GetValues(typeof(MRNative.eGroup)))
		{
			stack = MRGame.TheGame.NewGamePieceStack();
			stack.Layer = gameObject.layer;
			stack.transform.parent = transform;
			mNativeTreasures.Add(native, stack);
		}
		IList<MRItem> twits = new List<MRItem>();
		foreach (MRTreasure treasure in MRItemManager.Treasure.Values)
		{
			if (treasure.IsTwiT)
				twits.Add(treasure);
		}
		foreach (MRTreasure twit in twits)
		{
			stack = MRGame.TheGame.NewGamePieceStack();
			stack.Layer = gameObject.layer;
			stack.transform.parent = transform;
			mTwitTreasures.Add(twit.Id, stack);
		}
		twits = null;
		mDestroyedItems = MRGame.TheGame.NewGamePieceStack();
		mDestroyedItems.Layer = LayerMask.NameToLayer("Dummy");

		CreateTreasures();
	}

	/// <summary>
	/// Randomizes the treasures and assigns them to their initial locations.
	/// </summary>
	public void CreateTreasures()
	{
		// separate the treasures into large, small, and treasures within treasues
		IList<MRItem> twits = new List<MRItem>();
		IList<MRItem> largeTreasures = new List<MRItem>();
		IList<MRItem> smallTreasures = new List<MRItem>();
		IDictionary<string, int> armorTracker = new Dictionary<string, int>();
		IDictionary<string, int> weaponTracker = new Dictionary<string, int>();

		ICollection<MRTreasure> treasures = MRItemManager.Treasure.Values;
		foreach (MRTreasure treasure in treasures)
		{
			if (treasure.IsLargeTreasure)
				largeTreasures.Add(treasure);
			else if (treasure.IsTwiT)
				twits.Add(treasure);
			else
				smallTreasures.Add(treasure);
		}
		MRUtility.Shuffle(smallTreasures);
		MRUtility.Shuffle(smallTreasures);
		MRUtility.Shuffle(largeTreasures);
		MRUtility.Shuffle(largeTreasures);

		// get the treasure chart json data
		TextAsset itemsList = (TextAsset)Resources.Load("treasurechart");
		StringBuilder jsonText = new StringBuilder(itemsList.text);
		JSONObject jsonData = (JSONObject)JSONDecoder.CreateJSONValue(jsonText);

		// add treasures to the twit stacks
		JSONArray treasuresData = (JSONArray)jsonData["twits"];
		for (int i = 0; i < treasuresData.Count; ++i)
		{
			JSONObject twitTreasure = (JSONObject)treasuresData[i];
			foreach (MRTreasure twit in twits)
			{
				if (twit.Name.StartsWith(((JSONString)twitTreasure["name"]).Value))
				{
					AddTreasuresToStack(twitTreasure, mTwitTreasures[twit.Id], 
					                    smallTreasures, largeTreasures,
					                    armorTracker, weaponTracker,
					                    null);
					break;
				}
			}
		}

		// merge the twits and the rest of the large treasures
		foreach (MRTreasure twit in twits)
		{
			largeTreasures.Add(twit);
		}
		twits = null;
		MRUtility.Shuffle(largeTreasures);
		MRUtility.Shuffle(largeTreasures);

		// add treasures to the sites
		treasuresData = (JSONArray)jsonData["sites"];
		for (int i = 0; i < treasuresData.Count; ++i)
		{
			JSONObject siteTreasure = (JSONObject)treasuresData[i];
			AddTreasuresToStack(siteTreasure, 
			                    mSiteTreasures[(MRMapChit.eSiteChitType)(((JSONNumber)siteTreasure["id"]).IntValue)],
			                    smallTreasures, largeTreasures,
			                    armorTracker, weaponTracker,
			                    null);
		}

		// add treasures to the natives
		treasuresData = (JSONArray)jsonData["natives"];
		for (int i = 0; i < treasuresData.Count; ++i)
		{
			JSONObject nativeTreasure = (JSONObject)treasuresData[i];
			MRNative.eGroup nativeGroup = (MRNative.eGroup)(((JSONNumber)nativeTreasure["id"]).IntValue);
			AddTreasuresToStack(nativeTreasure, 
			                    mNativeTreasures[nativeGroup],
			                    smallTreasures, largeTreasures,
			                    armorTracker, weaponTracker,
			                    nativeGroup);
		}

		// temp - hide any leftover treasures
		foreach (MRItem treasure in smallTreasures)
		{
			treasure.Layer = LayerMask.NameToLayer("Dummy");
		}
		foreach (MRItem treasure in largeTreasures)
		{
			treasure.Layer = LayerMask.NameToLayer("Dummy");
		}
	}

	private void AddTreasuresToStack(JSONObject treasureData, 
	                                 MRGamePieceStack stack, 
	                                 IList<MRItem> smallTreasures, 
	                                 IList<MRItem> largeTreasures,
	                                 IDictionary<string, int> armorTracker,
	                                 IDictionary<string, int> weaponTracker,
	                                 Nullable<MRNative.eGroup> nativeOwner)
	{
		// place the stack at its location on the treasure chart
		MRTreasureChartLocation stackLocation = null;
		String stackName = ((JSONString)treasureData["name"]).Value;
		MRTreasureChartLocation[] locations = gameObject.GetComponentsInChildren<MRTreasureChartLocation>();
		foreach (MRTreasureChartLocation location in locations)
		{
			if (stackName == location.stackName)
			{
				stackLocation = location;
				location.Treasures = stack;
				break;
			}
		}
		if (stackLocation == null)
		{
			Debug.LogError("No stack location for " + stackName);
		}

		// add small treasures
		int smallCount = ((JSONNumber)treasureData["st"]).IntValue;
		for (int i = 0; i < smallCount; ++i)
		{
			if (smallTreasures.Count > 0)
			{
				MRItem treasure = smallTreasures[smallTreasures.Count - 1];
				smallTreasures.RemoveAt(smallTreasures.Count - 1);
				stack.AddPieceToTop(treasure);
			}
		}

		// add large treasures
		int largeCount = ((JSONNumber)treasureData["lt"]).IntValue;
		for (int i = 0; i < largeCount; ++i)
		{
			if (largeTreasures.Count > 0)
			{
				MRItem treasure = largeTreasures[largeTreasures.Count - 1];
				largeTreasures.RemoveAt(largeTreasures.Count - 1);
				stack.AddPieceToTop(treasure);
			}
		}

		// add horses
		JSONArray horsesList = (JSONArray)treasureData["horses"];
		for (int i = 0; i < horsesList.Count; ++i)
		{
			JSONObject horseData = (JSONObject)horsesList[i];
			string name = ((JSONString)horseData["name"]).Value;
			int amount = ((JSONNumber)horseData["count"]).IntValue;
			int speed = -1;
			if (horseData["speed"] != null)
				speed = ((JSONNumber)horseData["speed"]).IntValue;
			MRGame.eStrength strength = MRGame.eStrength.Any;
			if (horseData["strength"] != null)
				strength = ((JSONString)horseData["strength"]).Value.Strength();
			foreach (MRHorse horse in MRItemManager.Horses.Values)
			{
				if (name.Equals(horse.Name, StringComparison.OrdinalIgnoreCase))
				{
					if (horse.Stack == null && (
						strength == MRGame.eStrength.Any ||
					    (strength == horse.GallopStrength && speed == horse.GallopSpeed)))
					{
						stack.AddPieceToTop(horse);
						if (--amount == 0)
							break;
					}
				}
			}
		}

		// add armor
		JSONArray armorList = (JSONArray)treasureData["armor"];
		for (int i = 0; i < armorList.Count; ++i)
		{
			JSONObject armorData = (JSONObject)armorList[i];
			string name = ((JSONString)armorData["name"]).Value;
			int amount = ((JSONNumber)armorData["count"]).IntValue;
			int baseIndex = 0;
			armorTracker.TryGetValue(name, out baseIndex);
			for (int j = 0; j < amount; ++j)
			{
				MRArmor armor = MRItemManager.GetArmor(name, baseIndex + j);
				if (armor != null)
				{
					armor.NativeOwner = nativeOwner;
					stack.AddPieceToTop(armor);
				}
			}
			armorTracker[name] = baseIndex + amount;
		}

		// add weapons
		JSONArray weaponList = (JSONArray)treasureData["weapons"];
		for (int i = 0; i < weaponList.Count; ++i)
		{
			JSONObject weaponData = (JSONObject)weaponList[i];
			string name = ((JSONString)weaponData["name"]).Value;
			int amount = ((JSONNumber)weaponData["count"]).IntValue;
			int baseIndex = 0;
			weaponTracker.TryGetValue(name, out baseIndex);
			for (int j = 0; j < amount; ++j)
			{
				MRWeapon weapon = MRItemManager.GetWeapon(name, baseIndex + j);
				if (weapon != null)
				{
					weapon.NativeOwner = nativeOwner;
					stack.AddPieceToTop(weapon);
				}
			}
			weaponTracker[name] = baseIndex + amount;
		}
	}

	public MRGamePieceStack GetSiteTreasures(MRMapChit.eSiteChitType site)
	{
		return mSiteTreasures[site];
	}

	public MRGamePieceStack GetNativeTreasures(MRNative.eGroup group)
	{
		return mNativeTreasures[group];
	}

	public MRGamePieceStack GetTwitTreasures(uint twitId)
	{
		MRGamePieceStack stack = null;
		mTwitTreasures.TryGetValue(twitId, out stack);
		if (stack == null)
		{
			Debug.LogError("Tried to get invalid twit treasures for id " + twitId);
		}
		return stack;
	}

	/// <summary>
	/// Returns a weapon of a given name from the natives that have that weapon. Does not remove the weapon from the native's stack.
	/// </summary>
	/// <returns>The weapon.</returns>
	/// <param name="weaponName">Weapon name.</param>
	public MRWeapon GetWeaponFromNatives(string weaponName)
	{
		foreach (MRGamePieceStack stack in mNativeTreasures.Values)
		{
			foreach (MRIGamePiece piece in stack.Pieces)
			{
				if (piece is MRWeapon && ((MRWeapon)piece).Name == weaponName)
				{
					return (MRWeapon)piece;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Returns an armor of a given name from a random group of natives. Does not remove the armor from the native's stack.
	/// </summary>
	/// <returns>The armor.</returns>
	/// <param name="armorName">Armor name.</param>
	public MRArmor GetArmorFromNatives(string armorName)
	{
		// for each group of natives, find an armor or the desired type
		List<MRArmor> armors = new List<MRArmor>();
		foreach (MRGamePieceStack stack in mNativeTreasures.Values)
		{
			foreach (MRIGamePiece piece in stack.Pieces)
			{
				if (piece is MRArmor && ((MRArmor)piece).Name == armorName)
				{
					armors.Add((MRArmor)piece);
					break;
				}
			}
		}
		// pick a random armor from the ones found
		if (armors.Count > 0)
		{
			int index = MRRandom.Range(0, armors.Count);
			return armors[index];
		}

		return null;
	}

	/// <summary>
	/// Returns an armor of a given name from a specific group of natives. Does not remove the armor from the native's stack.
	/// </summary>
	/// <returns>The armor from natives.</returns>
	/// <param name="armorName">Armor name.</param>
	/// <param name="group">Native group.</param>
	public MRArmor GetArmorFromNatives(string armorName, MRNative.eGroup group)
	{
		MRGamePieceStack stack = mNativeTreasures[group];
		foreach (MRIGamePiece piece in stack.Pieces)
		{
			if (piece is MRArmor && ((MRArmor)piece).Name == armorName)
			{
				return (MRArmor)piece;
			}
		}
		return null;
	}

	/// <summary>
	/// Puts a weapon back with the natives who originally owned it.
	/// </summary>
	/// <param name="weapon">Weapon.</param>
	public void ReturnWeaponToNatives(MRWeapon weapon)
	{
		if (weapon != null && weapon.NativeOwner != null)
		{
			mNativeTreasures[weapon.NativeOwner.Value].AddPieceToBottom(weapon);
		}
	}

	/// <summary>
	/// Puts an armor back with the natives who originally owned it.
	/// </summary>
	/// <param name="armor">Armor.</param>
	public void ReturnArmorToNatives(MRArmor armor)
	{
		if (armor != null && armor.NativeOwner != null)
		{
			mNativeTreasures[armor.NativeOwner.Value].AddPieceToBottom(armor);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (MRGame.TheGame.CurrentView != MRGame.eViews.Treasure)
			return;
	}

	#endregion

	#region Members

	private Camera mCamera;
	private IDictionary<MRMapChit.eSiteChitType, MRGamePieceStack> mSiteTreasures = new Dictionary<MRMapChit.eSiteChitType, MRGamePieceStack>();
	private IDictionary<MRNative.eGroup, MRGamePieceStack> mNativeTreasures = new Dictionary<MRNative.eGroup, MRGamePieceStack>();
	private IDictionary<uint, MRGamePieceStack> mTwitTreasures = new Dictionary<uint, MRGamePieceStack>();
	private MRGamePieceStack mDestroyedItems;

	#endregion
}

