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

public class MRMonsterChart : MonoBehaviour
{
	#region Internal Classes

	private class SummonTrigger
	{
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
		public bool Match(MRMapChit chit, MRTile.eTileType tileType)
		{
			if (chit.ChitType == Type && (!UseClearing || tileType == Tile))
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
				}
			}
			return false;
		}

		public MRMapChit.eMapChitType Type;
		public MRMapChit.eSoundChitType Sound;
		public MRMapChit.eWarningChitType Warning;
		public MRMapChit.eSiteChitType Site;
		public MRTile.eTileType Tile;
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
		public bool Match(MRMapChit chit, MRTile.eTileType tileType)
		{
			foreach (SummonTrigger trigger in Triggers)
			{
				if (trigger.Match(chit, tileType))
					return true;
			}
			return false;
		}
	}

	#endregion

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
			return prowlers;
		}
	}

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
			return prowlers;
		}
	}

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

		// add the monsters to the locations
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
							for (int k = 0; k < amount; ++k)
							{
								MRMonster monster = MRDenizenManager.GetMonster(monsterName, monsterIndex);
								if (monster != null)
								{
									++monsterIndex;
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
				mMonsterRollMarkers[i].renderer.enabled = (mMonsterRoll == i + 1);
			}
		}
	}

	/// <summary>
	/// Returns a list of prowling denizens summoned by a given chit/tile.
	/// </summary>
	/// <returns>The summoned denizens.</returns>
	/// <param name="chit">Chit summoning the denizens.</param>
	/// <param name="tileType">Tile type.</param>
	public IList<MRDenizen> GetSummonedDenizens(MRMapChit chit, MRTile.eTileType tileType)
	{
		IList<MRDenizen> summoned = new List<MRDenizen>();

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
								if (piece is MRDenizen)
									summoned.Add((MRDenizen)piece);
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

		foreach (MRDenizen denizen in toRemove)
		{
			mDeadPool.Remove(denizen);
		}
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

	private IList<MRDenizen> mDeadPool = new List<MRDenizen>();

	#endregion
}

