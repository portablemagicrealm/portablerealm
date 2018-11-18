//
// MRMonsterChart.cs
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
	
public class MRMonsterChart : MonoBehaviour, MRISerializable
{
	#region Internal Classes

	private class SummonTrigger
	{
		public SummonTrigger()
		{
			Type = MRMapChit.eMapChitType.Site;
			Sound = MRMapChit.eSoundChitType.Flutter;
			Warning = MRMapChit.eWarningChitType.Bones;
			Site = MRMapChit.eSiteChitType.Altar;
			Tile = MRTile.eTileType.Cave;
			Dwelling = MRDwelling.eDwelling.Chapel;
			UseClearing = false;
		}

		public SummonTrigger(string type, string subtype, string location)
		{
			switch (type)
			{
				case "sound":
					Type = MRMapChit.eMapChitType.Sound;
					switch (subtype)
					{
						case "flutter":
							Sound = MRMapChit.eSoundChitType.Flutter;
							break;
						case "howl":
							Sound = MRMapChit.eSoundChitType.Howl;
							break;
						case "patter":
							Sound = MRMapChit.eSoundChitType.Patter;
							break;
						case "roar":
							Sound = MRMapChit.eSoundChitType.Roar;
							break;
						case "slither":
							Sound = MRMapChit.eSoundChitType.Slither;
							break;
						default:
							Debug.LogError("Monster chart unknown sound type " + subtype);
							break;
					}
					break;
				case "warning":
					Type = MRMapChit.eMapChitType.Warning;
					switch (subtype)
					{
						case "bones":
							Warning = MRMapChit.eWarningChitType.Bones;
							break;
						case "dank":
							Warning = MRMapChit.eWarningChitType.Dank;
							break;
						case "ruins":
							Warning = MRMapChit.eWarningChitType.Ruins;
							break;
						case "smoke":
							Warning = MRMapChit.eWarningChitType.Smoke;
							break;
						case "stink":
							Warning = MRMapChit.eWarningChitType.Stink;
							break;
						default:
							Debug.LogError("Monster chart unknown warning type " + subtype);
							break;
					}
					break;
				case "site":
					Type = MRMapChit.eMapChitType.Site;
					switch (subtype)
					{
						case "altar":
							Site = MRMapChit.eSiteChitType.Altar;
							break;
						case "cairns":
							Site = MRMapChit.eSiteChitType.Cairns;
							break;
						case "hoard":
							Site = MRMapChit.eSiteChitType.Hoard;
							break;
						case "lair":
							Site = MRMapChit.eSiteChitType.Lair;
							break;
						case "pool":
							Site = MRMapChit.eSiteChitType.Pool;
							break;
						case "shrine":
							Site = MRMapChit.eSiteChitType.Shrine;
							break;
						case "statue":
							Site = MRMapChit.eSiteChitType.Statue;
							break;
						case "vault":
							Site = MRMapChit.eSiteChitType.Vault;
							break;
						default:
							Debug.LogError("Monster chart unknown site type " + subtype);
							break;
					}
					break;
				case "dwelling":
					Type = MRMapChit.eMapChitType.Dwelling;
					Dwelling = subtype.Dwelling();
					break;
				default:
					Debug.LogError("Monster chart unknown type " + type);
					break;
			}
			UseClearing = false;
			if (location != null)
			{
				switch (location)
				{
					case "w":
						Tile = MRTile.eTileType.Woods;
						UseClearing = true;
						break;
					case "m":
						Tile = MRTile.eTileType.Mountain;
						UseClearing = true;
						break;
					case "c":
						Tile = MRTile.eTileType.Cave;
						UseClearing = true;
						break;
					case "v":
						Tile = MRTile.eTileType.Valley;
						UseClearing = true;
						break;
					default:
						Debug.LogError("Monster chart unknown location " + location);
						break;
				}
			}
		}

		/// <summary>
		/// Returns if a given chit/tile matches this summon trigger.
		/// </summary>
		/// <param name="chit">Chit.</param>
		/// <param name="tileType">Tile type.</param>
		public bool Match(MRChit chit, MRTile.eTileType tileType)
		{
			if (chit is MRMapChit && ((MRMapChit)chit).ChitType == Type && (!UseClearing || tileType == Tile))
			{
				switch (Type)
				{
					case MRMapChit.eMapChitType.Site:
						if (((MRSiteChit)chit).SiteType == Site)
							return true;
						break;
					case MRMapChit.eMapChitType.Sound:
						if (((MRSoundChit)chit).SoundType == Sound)
							return true;
						break;
					case MRMapChit.eMapChitType.Warning:
						if (((MRWarningChit)chit).WarningType == Warning)
							return true;
						break;
					default:
						break;
				}
			}
			else if (chit is MRDwelling && ((MRDwelling)chit).Type == Dwelling)
			{
				return true;
			}
			return false;
		}

		public MRMapChit.eMapChitType Type;
		public MRMapChit.eSoundChitType Sound;
		public MRMapChit.eWarningChitType Warning;
		public MRMapChit.eSiteChitType Site;
		public MRTile.eTileType Tile;
		public MRDwelling.eDwelling Dwelling;
		public bool UseClearing;
	}

	private class SummonData
	{
		public IList<MRMonsterChartLocation> Boxes;
		public IList<SummonTrigger> Triggers;
		public IList<MRDenizen> Denizens;

		public SummonData()
		{
			Boxes = new List<MRMonsterChartLocation>();
			Triggers = new List<SummonTrigger>();
			Denizens = new List<MRDenizen>();
		}

		/// <summary>
		/// Returns if a given chit/tile matches a trigger for the summon data.
		/// </summary>
		/// <param name="chit">Chit.</param>
		/// <param name="tileType">Tile type.</param>
		public bool Match(MRChit chit, MRTile.eTileType tileType)
		{
			foreach (SummonTrigger trigger in Triggers)
			{
				if (trigger.Match(chit, tileType))
					return true;
			}
			return false;
		}
	}

	private class OnBoardData
	{
		public IList<MRDenizen> Denizens;
		public SummonTrigger Trigger;
		public MRILocation StartLocation;
		public int RegenNumber;

		public OnBoardData()
		{
			Denizens = new List<MRDenizen>();
		}

		/// <summary>
		/// Returns if a given chit/tile matches a trigger for the summon data.
		/// </summary>
		/// <param name="chit">Chit.</param>
		/// <param name="tileType">Tile type.</param>
		public bool Match(MRMapChit chit, MRTile.eTileType tileType)
		{
			return Trigger.Match(chit, tileType);
		}
	}

	#endregion

	#region Properties

	public MRGamePieceStack holdingArea;

	public bool Visible
	{
		get{
			return mCamera.enabled;
		}
		
		set{
			mCamera.enabled = value;
		}
	}

	public int MonsterRoll
	{
		get{
			return mMonsterRoll;
		}

		set{
			mMonsterRoll = value;
			if (MRGame.DayOfMonth % 7 == 0)
			{
				// regenerate monsters
				RegenerateDenizens();
			}
		}
	}

	public IList<MRMonster> ProwlingMonsters
	{
		get{
			IList<MRMonster> prowlers = new List<MRMonster>();
			if (mMonsterRoll >= 1 && mMonsterRoll <= 6)
			{
				IList<SummonData> row = mSummonsData[mMonsterRoll - 1];
				foreach (SummonData data in row)
				{
					foreach (MRDenizen denizen in data.Denizens)
					{
						if (denizen is MRMonster)
							prowlers.Add((MRMonster)denizen);
					}
				}
			}
			// ghosts always prowl
			for (int i = 0; i < 2; ++i)
			{
				MRMonster ghost = MRDenizenManager.GetMonster("ghost", i);
				if (ghost != null)
					prowlers.Add(ghost);
			}
			return prowlers;
		}
	}
/*
	public IList<MRDenizen> ProwlingDenizens
	{
		get{
			IList<MRDenizen> prowlers = new List<MRDenizen>();
			if (mMonsterRoll >= 1 && mMonsterRoll <= 6)
			{
				IList<SummonData> row = mSummonsData[mMonsterRoll - 1];
				foreach (SummonData data in row)
				{
					foreach (MRDenizen denizen in data.Denizens)
						prowlers.Add(denizen);
				}
			}
			// ghosts always prowl
			for (int i = 0; i < 2; ++i)
			{
				MRMonster ghost = MRDenizenManager.GetMonster("ghost", i);
				if (ghost != null)
					prowlers.Add(ghost);
			}
			return prowlers;
		}
	}
*/
	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		foreach (Camera camera in Camera.allCameras)
		{
			if (camera.name == "Monster Camera")
			{
				mCamera = camera;
				break;
			}
		}

		// get the monster roll markers
		mMonsterRollMarkers = new GameObject[6];
		SpriteRenderer[] sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "roll")
			{
				GameObject rowObject = sprite.gameObject.transform.parent.gameObject;
				if (rowObject.name.StartsWith("row_"))
				{
					int row = Int32.Parse(rowObject.name.Substring("row_".Length));
					if (row >= 1 && row <= 6)
						mMonsterRollMarkers[row - 1] = sprite.gameObject;
				}
			}
		}

		mOnBoardData = new List<OnBoardData>();
		mSummonsData = new List<SummonData>[6];
		for (int i = 0; i < mSummonsData.Length; ++i)
			mSummonsData[i] = new List<SummonData>();

		// get the monster chart locations
		mLocations = new List<MRMonsterChartLocation>[6];
		MRMonsterChartLocation[] locations = gameObject.GetComponentsInChildren<MRMonsterChartLocation>();
		foreach (MRMonsterChartLocation location in locations)
		{
			if (location.row >= 1 && location.row <= 6)
			{
				int row = location.row - 1;
				if (mLocations[row] == null)
					mLocations[row] = new List<MRMonsterChartLocation>();
				mLocations[row].Add(location);
			}
		}

		holdingArea.Layer = LayerMask.NameToLayer("Dummy");

		ResetMonsters();
	}

	/// <summary>
	/// Resets the monsters to their start locations.
	/// </summary>
	public void ResetMonsters()
	{
		Debug.LogWarning("reset monster chart");
		// clear out old data
		mDeadPool.Clear();
		mOnBoardData.Clear();
		for (int i = 0; i < mSummonsData.Length; ++i)
			mSummonsData[i].Clear();

		// assign the monsters to their start locations
		IDictionary<string, int> monsterIndexes = new Dictionary<string, int>();
		TextAsset monstersList = (TextAsset)Resources.Load("monsterchart");
		StringBuilder jsonText = new StringBuilder(monstersList.text);
		JSONObject jsonData = (JSONObject)JSONDecoder.CreateJSONValue(jsonText);
		JSONArray summonsData = (JSONArray)jsonData["summons"];
		for (int i = 0; i < summonsData.Count; ++i)
		{
			JSONObject summonData = (JSONObject)summonsData[i];

			int row = ((JSONNumber)summonData["row"]).IntValue;
			if (row >= 1 && row <= 6)
			{
				SummonData data = new SummonData();
				mSummonsData[row-1].Add(data);

				// init the boxes for the summon area
				JSONArray boxesData = (JSONArray)summonData["boxes"];
				for (int j = 0; j < boxesData.Count; ++j)
				{
					JSONObject monsterData = (JSONObject)boxesData[j];
					if (monsterData["monster"] != null)
					{
						LoadMonster(monsterData, row, data, monsterIndexes);
					}
					else if (monsterData["natives"] != null)
					{
						LoadNative(monsterData, row, data);
					}
				}

				// init the summon data for the area
				JSONArray chitsData = (JSONArray)summonData["chits"];
				for (int j = 0; j < chitsData.Count; ++j)
				{
					JSONObject chitData = (JSONObject)chitsData[j];
					string typeName = ((JSONString)chitData["type"]).Value;
					string subtypeName = ((JSONString)chitData["subtype"]).Value;
					string locationName = null;
					if (chitData["location"] != null)
						locationName = ((JSONString)chitData["location"]).Value;
					SummonTrigger trigger = new SummonTrigger(typeName, subtypeName, locationName);
					data.Triggers.Add(trigger);
				}
			}
		}

		// get the monsters that start on-board and put them in a holding area
		JSONArray onBoardData = (JSONArray)jsonData["startonboard"];
		for (int i = 0; i < onBoardData.Count; ++i)
		{
			OnBoardData data = new OnBoardData();
			JSONObject denizenData = (JSONObject)onBoardData[i];
			int amount = ((JSONNumber)denizenData["amount"]).IntValue;
			if (denizenData["monster"] != null)
			{
				for (int j = 0; j < amount; ++j)
				{
					string monsterName = ((JSONString)denizenData["monster"]).Value;
					MRMonster monster = MRDenizenManager.GetMonster(monsterName, j);
					if (monster != null)
					{
						data.Denizens.Add(monster);
						holdingArea.AddPieceToBottom(monster);
					}
				}
			}
			else if (denizenData["native"] != null)
			{
				for (int j = 0; j < amount; ++j)
				{
					string nativeName = ((JSONString)denizenData["native"]).Value;
					MRNative native = MRDenizenManager.GetNative(nativeName, j);
					if (native != null)
					{
						data.Denizens.Add(native);
						holdingArea.AddPieceToBottom(native);
					}
				}
			}
			data.RegenNumber = ((JSONNumber)denizenData["regen"]).IntValue;
			JSONObject summonChitData = (JSONObject)denizenData["chit"];
			data.Trigger = new SummonTrigger(
				((JSONString)summonChitData["type"]).Value,
				((JSONString)summonChitData["subtype"]).Value,
				((JSONString)summonChitData["location"]).Value
			);
			mOnBoardData.Add(data);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (!Visible)
			return;

		for (int i = 0; i < mMonsterRollMarkers.Length; ++i)
		{
			if (mMonsterRollMarkers[i] != null)
			{
				mMonsterRollMarkers[i].GetComponent<Renderer>().enabled = (mMonsterRoll == i + 1);
			}
		}
	}

	/// <summary>
	/// Returns a list of prowling monsters summoned by a given chit/tile.
	/// </summary>
	/// <returns>The summoned monsters.</returns>
	/// <param name="chit">Chit summoning the monsters.</param>
	/// <param name="tileType">Tile type.</param>
	public IList<MRMonster> GetSummonedMonsters(MRMapChit chit, MRTile.eTileType tileType)
	{
		IList<MRMonster> summoned = new List<MRMonster>();

		if (mMonsterRoll >= 1 && mMonsterRoll <= 6)
		{
			IList<SummonData> row = mSummonsData[mMonsterRoll - 1];
			foreach (SummonData data in row)
			{
				if (data.Match(chit, tileType))
				{
					// get the denizens from the first occupied box in the summon data
					foreach (MRMonsterChartLocation box in data.Boxes)
					{
						if (box.Occupants.Count > 0)
						{
							foreach (MRIGamePiece piece in box.Occupants.Pieces)
							{
								if (piece is MRMonster)
									summoned.Add((MRMonster)piece);
							}
							break;
						}
					}
					break;
				}
			}
		}
		return summoned;
	}

	/// <summary>
	/// Returns a list of prowling natives summoned by a given dwelling.
	/// </summary>
	/// <returns>The summoned natives.</returns>
	/// <param name="dwelling">Dwelling summoning the natives.</param>
	public IList<MRNative> GetSummonedNatives(MRDwelling dwelling)
	{
		IList<MRNative> summoned = new List<MRNative>();

		if (mMonsterRoll >= 1 && mMonsterRoll <= 6)
		{
			IList<SummonData> row = mSummonsData[mMonsterRoll - 1];
			foreach (SummonData data in row)
			{
				if (data.Match(dwelling, MRTile.eTileType.Valley))
				{
					foreach (MRMonsterChartLocation box in data.Boxes)
					{
						if (box.Occupants.Count > 0)
						{
							foreach (MRIGamePiece piece in box.Occupants.Pieces)
							{
								if (piece is MRNative)
									summoned.Add((MRNative)piece);
							}
							break;
						}
					}
				}
			}
		}

		return summoned;
	}

	/// <summary>
	/// Returns a list of denizens that start on-board from a given chit/tile.
	/// </summary>
	/// <returns>The summoned denizens.</returns>
	/// <param name="chit">Chit summoning the denizens.</param>
	/// <param name="tileType">Tile type.</param>
	/// <param name="start">Location the denizens will start on.</param>
	public IList<MRDenizen> GetOnBoardDenizens(MRMapChit chit, MRTile.eTileType tileType, MRILocation start)
	{
		IList<MRDenizen> summoned = new List<MRDenizen>();

		foreach (OnBoardData data in mOnBoardData)
		{
			if (data.Match(chit, tileType))
			{
				data.StartLocation = start;
				foreach (MRDenizen denizen in data.Denizens)
				{
					summoned.Add(denizen);
				}
			}
		}

		return summoned;
	}

	/// <summary>
	/// Adds a dead denizen to the dead pool.
	/// </summary>
	/// <param name="denizen">Dead denizen.</param>
	public void AddDeadDenizen(MRDenizen denizen)
	{
		mDeadPool.Add(denizen);
		if (denizen.Stack != null)
			denizen.Stack.RemovePiece(denizen);
		denizen.Layer = LayerMask.NameToLayer("Dummy");
		denizen.Location = null;
	}

	/// <summary>
	/// Regenerates dead denizens, based on the current monster roll.
	/// </summary>
	public void RegenerateDenizens()
	{
		// regenerate normal monsters
		IList<MRDenizen> toRemove = new List<MRDenizen>();
		if (mMonsterRoll >= 1 && mMonsterRoll <= 6)
		{
			IList<SummonData> row = mSummonsData[mMonsterRoll - 1];
			foreach (SummonData data in row)
			{
				foreach (MRIGamePiece piece in mDeadPool)
				{
					MRDenizen denizen = (MRDenizen)piece;
					if (data.Denizens.Contains(denizen))
					{
						if (denizen.MonsterBox != null)
						{
							toRemove.Add(denizen);
							denizen.MonsterBox.Occupants.AddPieceToTop(denizen);
						}
					}
				}
			}
		}

		// regenerate the on-board denizens
		foreach (OnBoardData data in mOnBoardData)
		{
			if (data.RegenNumber == 0 || data.RegenNumber == mMonsterRoll)
			{
				foreach (MRDenizen denizen in data.Denizens)
				{
					toRemove.Add(denizen);
					denizen.Location = data.StartLocation;
				}
			}
		}

		foreach (MRDenizen denizen in toRemove)
		{
			mDeadPool.Remove(denizen);
		}
	}

	/// <summary>
	/// Reads in info about where a monster should be placed on the chart, add adds the appropriate monster chits to it.
	/// </summary>
	/// <param name="monsterData">Monster data to read.</param>
	/// <param name="row">Which summon row the monster is on</param>
	/// <param name="data">To be filled in with info about what summons the monster</param>
	/// <param name="monsterIndexes">Which monsters have already been added to the chart.</param>
	private void LoadMonster(JSONObject monsterData, int row, SummonData data, IDictionary<string, int> monsterIndexes)
	{
		string locationName = ((JSONString)monsterData["name"]).Value;
		string monsterName = ((JSONString)monsterData["monster"]).Value;
		int amount = ((JSONNumber)monsterData["amount"]).IntValue;
		foreach (MRMonsterChartLocation location in mLocations[row - 1])
		{
			if (location.stackName == locationName)
			{
				data.Boxes.Add(location);
				if (location.Occupants == null)
				{
					MRGamePieceStack stack = MRGame.TheGame.NewGamePieceStack();
					location.Occupants = stack;
				}
				// add the monsters to the boxes
				int monsterIndex = 0;
				monsterIndexes.TryGetValue(monsterName, out monsterIndex);
				for (int i = 0; i < amount; ++i)
				{
					MRMonster monster = MRDenizenManager.GetMonster(monsterName, monsterIndex);
					if (monster != null)
					{
						++monsterIndex;
						if (monster.Owns != null)
						{
							monster.Owns.MonsterBox = location;
							location.Occupants.AddPieceToTop(monster.Owns);
							data.Denizens.Add(monster.Owns);
						}
						monster.MonsterBox = location;
						location.Occupants.AddPieceToTop(monster);
						data.Denizens.Add(monster);
					}
				}
				monsterIndexes[monsterName] = monsterIndex;
				break;
			}
		}
	}

	/// <summary>
	/// Reads in info about where a native should be placed on the chart, add adds the appropriate native chits to it.
	/// </summary>
	/// <param name="nativeData">Native data to read.</param>
	/// <param name="row">Which summon row the native is on.</param>
	/// <param name="data">To be filled in with info about what summons the native</param>
	private void LoadNative(JSONObject nativeData, int row, SummonData data)
	{
		string locationName = ((JSONString)nativeData["name"]).Value;
		string nativeName = ((JSONString)nativeData["natives"]).Value;
		int amount = ((JSONNumber)nativeData["amount"]).IntValue;
		foreach (MRMonsterChartLocation location in mLocations[row - 1])
		{
			if (location.stackName == locationName)
			{
				data.Boxes.Add(location);
				if (location.Occupants == null)
				{
					MRGamePieceStack stack = MRGame.TheGame.NewGamePieceStack();
					location.Occupants = stack;
				}
				// add the natives to the box
				for (int i = 0; i < amount; ++i)
				{
					MRNative native = MRDenizenManager.GetNative(nativeName, i);
					if (native != null)
					{
						native.MonsterBox = location;
						location.Occupants.AddPieceToTop(native);
						data.Denizens.Add(native);
					}
				}
			}
		}
	}

	public bool Load(JSONObject root) 
	{
		JSONObject monsterData = root["monsterChart"] as JSONObject;
		if (monsterData == null)
			return true;

		if (monsterData["deadPool"] != null)
		{
			JSONArray deadPool = (JSONArray)monsterData["deadPool"];
			for (int i = 0; i < deadPool.Count; ++i)
			{
				MRIGamePiece piece = MRGame.TheGame.GetGamePiece(deadPool[i]);
				if (piece != null && piece is MRDenizen)
				{
					AddDeadDenizen((MRDenizen)piece);
				}
			}
		}
		if (monsterData["onBoard"] != null)
		{
			JSONArray onBoard = (JSONArray)monsterData["onBoard"];
			for (int i = 0; i < onBoard.Count; ++i)
			{
				JSONObject creatureData = (JSONObject)onBoard[i];
                MRILocation startLocation = null;
                if (creatureData["startLocation"] != null)
                {
                    startLocation = MRGame.TheGame.GetClearing(creatureData["startLocation"]);
					if (startLocation == null)
					{
						Debug.LogError("OnBoard data bad location");
					}
                }
				if (startLocation != null)
				{
					MRMapChit.eMapChitType type = (MRMapChit.eMapChitType)((JSONNumber)creatureData["type"]).IntValue;
					switch (type)
					{
						case MRMapChit.eMapChitType.Dwelling:
							{
								MRDwelling.eDwelling subtype = (MRDwelling.eDwelling)((JSONNumber)creatureData["subtype"]).IntValue;
								for (int j = 0; j < mOnBoardData.Count; ++j)
								{
									if (mOnBoardData[j].Trigger.Type == type && mOnBoardData[j].Trigger.Dwelling == subtype)
									{
										mOnBoardData[j].StartLocation = startLocation;
									}
								}
							}
							break;
						case MRMapChit.eMapChitType.Site:
							{
								MRMapChit.eSiteChitType subtype = (MRMapChit.eSiteChitType)((JSONNumber)creatureData["subtype"]).IntValue;
								for (int j = 0; j < mOnBoardData.Count; ++j)
								{
									if (mOnBoardData[j].Trigger.Type == type && mOnBoardData[j].Trigger.Site == subtype)
									{
										mOnBoardData[j].StartLocation = startLocation;
									}
								}
							}
							break;
						case MRMapChit.eMapChitType.Sound:
							{
								MRMapChit.eSoundChitType subtype = (MRMapChit.eSoundChitType)((JSONNumber)creatureData["subtype"]).IntValue;
								for (int j = 0; j < mOnBoardData.Count; ++j)
								{
									if (mOnBoardData[j].Trigger.Type == type && mOnBoardData[j].Trigger.Sound == subtype)
									{
										mOnBoardData[j].StartLocation = startLocation;
									}
								}
							}
							break;
						case MRMapChit.eMapChitType.Warning:
							{
								MRMapChit.eWarningChitType subtype = (MRMapChit.eWarningChitType)((JSONNumber)creatureData["subtype"]).IntValue;
								for (int j = 0; j < mOnBoardData.Count; ++j)
								{
									if (mOnBoardData[j].Trigger.Type == type && mOnBoardData[j].Trigger.Warning == subtype)
									{
										mOnBoardData[j].StartLocation = startLocation;
										sTestLocation = mOnBoardData[j].StartLocation;
									}
								}
							}
							break;
					}
				}
			}
		}

		return true;
	}

	public void Save(JSONObject root) 
	{
		JSONObject monsterData = new JSONObject();

		if (mDeadPool.Count > 0)
		{
			JSONArray deadPool = new JSONArray(mDeadPool.Count);
			for (int i = 0; i < mDeadPool.Count; ++i)
			{
				deadPool[i] = new JSONNumber(mDeadPool[i].Id);
			}
			monsterData["deadPool"] = deadPool;
		}
		if (mOnBoardData.Count > 0)
		{
			JSONArray onBoard = new JSONArray(mOnBoardData.Count);
			for (int i = 0; i < mOnBoardData.Count; ++i)
			{
				JSONObject creatureData = new JSONObject();
                if (mOnBoardData[i].StartLocation != null)
                {
                    creatureData["startLocation"] = new JSONNumber(mOnBoardData[i].StartLocation.Id);
                }
				creatureData["type"] = new JSONNumber((int)mOnBoardData[i].Trigger.Type);
				switch (mOnBoardData[i].Trigger.Type)
				{
					case MRMapChit.eMapChitType.Dwelling:
						creatureData["subtype"] = new JSONNumber((int)mOnBoardData[i].Trigger.Dwelling);
						break;
					case MRMapChit.eMapChitType.Site:
						creatureData["subtype"] = new JSONNumber((int)mOnBoardData[i].Trigger.Site);
						break;
					case MRMapChit.eMapChitType.Sound:
						creatureData["subtype"] = new JSONNumber((int)mOnBoardData[i].Trigger.Sound);
						break;
					case MRMapChit.eMapChitType.Warning:
						creatureData["subtype"] = new JSONNumber((int)mOnBoardData[i].Trigger.Warning);
						break;
					default:
						break;
				}
				onBoard[i] = creatureData;
			}
			monsterData["onBoard"] = onBoard;
		}

		root["monsterChart"] = monsterData;
	}

	#endregion

	#region Members

	private Camera mCamera;
	private int mMonsterRoll;
	private GameObject[] mMonsterRollMarkers;

	// monster/native locations for each monster roll
	private IList<MRMonsterChartLocation>[] mLocations;

	// summon data for each monster roll
	private IList<SummonData>[] mSummonsData;

	// denizens that start on-board
	private IList<OnBoardData> mOnBoardData;

	private IList<MRDenizen> mDeadPool = new List<MRDenizen>();

	#endregion

	static MRILocation sTestLocation = null;
}

}