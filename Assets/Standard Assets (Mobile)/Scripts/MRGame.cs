//
// MRGame.cs
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

public class MRGame : MonoBehaviour, MRISerializable
{
	#region Constants

	public enum eTouchType
	{
		None,
		Touched,
		Single,
		Double,
		Held,
		Move,
		PinchZoom
	}

	public enum eGameType
	{
		Default,
		BookOfLearning
	}

	public enum eGameState
	{
		NoGame,
		Active,
		GameOver
	}

	public enum eActivity
	{
		// note the order here needs to match the order in the activity list image
		Enchant,
		Follow,
		Hire,
		Alert,
		Rest,
		Trade,
		Search,
		Move,
		Hide,
		None
	}

	public enum eTimeOfDay
	{
		Birdsong,
		Sunrise,
		Daylight,
		Sunset,
		Evening,
		Midnight
	}

	public enum eStrength
	{
		Negligable,
		Light,
		Medium,
		Heavy,
		Tremendous,
		Immobile,	// for treasures within treaures (except the chest)
		Chit,		// strength determined by the chit played
		Any
	}

	public enum eMoveType
	{
		Walk,
		WalkThroughWoods,
		Fly
	}

	public enum eCharacters
	{
		Amazon,
		Berserker,
		BlackKnight,
		Captain,
		Druid,
		Dwarf,
		Elf,
		Magician,
		Pilgrim,
		Sorceror,
		Swordsman,
		WhiteKnight,
		Witch,
		WitchKing,
		Wizard,
		WoodsGirl
	}

	public enum eRollTypes
	{
		Hide,
		Missile,
		MeetingTrade,
		MeetingHire,
		MeetingEncounter,
		SearchPeer,
		SearchLocate,
		SearchLoot,
		SearchRunes,
		SearchMagicSight,
		PowerOfPit,
		Wish,
		Curse,
		SpellTransform,
		Lost,
		LootToadstoolCircle,
		LootCryptOfKnight,
		LootEnchantedMeadow,
		CombatRandomAssignment,
	}

	public enum eNatives
	{
		Bashkars,
		Company,
		Guard,
		Lancers,
		Order,
		Patrol,
		Rogues,
		Soldiers,
		Woodfolk
	}

	public enum eRelationship
	{
		Enemy,
		Unfriendly,
		Neutral,
		Friendly,
		Ally
	}

	public enum eCurses
	{
		Eyemist,
		Squeak,
		Wither,
		IllHealth,
		Ashes,
		Disgust
	}

	public enum eDialogId
	{
		Message = 1,
		Search,
	}

	public enum eViews
	{
		Map,
		Characters,
		Monsters,
		Treasure,
		Main,
		FatigueCharacter,	// not accessable through view tabs
		Combat,				// not accessable through view tabs
		SelectAttack,		// not accessable through view tabs
		SelectManeuver,		// not accessable through view tabs
		SelectChit,			// not accessable through view tabs
		SelectSpell,		// not accessable through view tabs
		SelectClearing,		// not accessable through view tabs
		Alert,				// not accessable through view tabs
		Trade,				// not accessable through view tabs
	}
	public const int ViewTabCount = 5;

	public enum eSortValue
	{
		Chit,
		Character,
		Denizen,
		Monster,
		MonsterHead,
		Weapon,
		SmallArmor,
		LargeArmor,
		Spell,
		Item,
		Dwelling,
		Treasure
	}

	public enum eMagicColor
	{
		White,
		Grey,
		Gold,
		Purple,
		Black,
		Any,
		None
	}

	public enum eSpellDuration
	{
		Attack,
		Combat,
		Day,
		Fly,
		Instant,
		Permanent,
		Phase,
		None
	}

	public enum eSpellTarget
	{
		Artifact,
		Bats,
		CaveClearing,
		Character,
		Characters,
		Clearing,
		ControlledMonster,
		Curse,
		Demon,
		Giants,
		Goblin,
		Goblins,
		Hex,
		HiredLeader,
		LightCharacter,
		Monster,
		Monsters,
		Native,
		Natives,
		NativeGroup,
		Octopus,
		Ogre,
		Ogres,
		SoundChit,
		Spell,
		SpellBook,
		SpellChitAny,
		SpellChit1,
		SpellChit2,
		SpellChit3,
		SpellChit4,
		SpellChit5,
		SpellChit6,
		SpellChit7,
		SpellChit8,
		Spider,
		Weapon,
		WingedDemon,
		None
	}

	public static readonly Color tan = new Color(248f / 255f, 207f / 255f, 127f / 255f);
	public static readonly Color green = new Color(157f / 255f, 203f / 255f, 0);
	public static readonly Color yellow = new Color(255f / 255f, 255f / 255f, 0);
	public static readonly Color red = new Color(255f / 255f, 81f / 255f, 81f / 255f);
	public static readonly Color gold = new Color(255f / 255f, 204f / 255f, 0);
	public static readonly Color lightGrey = new Color(192f / 255f, 192f / 255f, 192f / 255f);
	public static readonly Color offWhite = new Color(230f / 255f, 230f / 255f, 230f / 255f);
	public static readonly Color yellowGreen = new Color(208f / 255f, 184f / 255f, 0 / 255f);
	public static readonly Color lightBlue = new Color(203f / 255f, 223f / 255f, 227f / 255f);
	public static readonly Color darkBlue = new Color(131f / 255f, 196f / 255f, 214f / 255f);
	public static readonly Color lightPurple = new Color(198f / 255f, 168f / 255f, 195f / 255f);
	public static readonly Color darkPurple = new Color(189f / 255f, 55f / 255f, 181f / 255f);
	public static readonly Color lightGrey2 = new Color(194f / 255f, 193f / 255f, 192f / 255f);
	public static readonly Color darkGrey = new Color(164f / 255f, 159f / 255f, 167f / 255f);
	public static readonly Color pink = new Color(228f / 255f, 177f / 255f, 195f / 255f);
	public static readonly Color lightGreen = new Color(206f / 255f, 210f / 255f, 114f / 255f);
	public static readonly Color darkGreen = new Color(146f / 255f, 193f / 255f, 4f / 255f);
	public static readonly Color white = new Color(255f / 255f, 255f / 255f, 255f / 255f);
	public static readonly Color black = new Color(0f / 255f, 0f / 255f, 0f / 255f);
	public static readonly Color almostBlack = new Color(60f / 255f, 60f / 255f, 60f / 255f);

	public static Dictionary<eMagicColor, Color> MagicColorMap = new Dictionary<eMagicColor, Color>()
	{
		{eMagicColor.White, white},
		{eMagicColor.Grey, lightGrey},
		{eMagicColor.Gold, gold},
		{eMagicColor.Purple, darkPurple},
		{eMagicColor.Black, almostBlack},
		{eMagicColor.None, pink},
	};

	public static Dictionary<MRGame.eRelationship, String> NativeRelationshipIconMap = new Dictionary<MRGame.eRelationship, String>()
	{
		{eRelationship.Enemy, "Textures/enemy"},
		{eRelationship.Unfriendly, "Textures/unfriendly"},
		{eRelationship.Neutral, "Textures/neutral"},
		{eRelationship.Friendly, "Textures/friendly"},
		{eRelationship.Ally, "Textures/ally"}
	};

	public const int MAX_LEARNABLE_SPELLS = 14;

	public const float MAP_CAMERA_FAR_SIZE = 5.0f;
	public const float MAP_CAMERA_NEAR_SIZE = 2.2f;
	public const float MAP_CAMERA_PINCH_ZOOM_SPEED = 5.0f;

	private const float DOUBLE_CLICK_TIME = 0.3f;
	private const float TOUCH_HELD_TIME = 0.75f;

	#endregion

	#region Properties

	public GameObject inspectionAreaPrototype;
	public MRMap mapPrototype;
	public MRTreasureChart treasureChartPrototype;
	public MRMonsterChart monsterChartPrototype;
	public MRCharacterMat characterMatPrototype;
	public MRActivityListWidget activityListPrototype;
	public MRCombatSheet combatSheetPrototype;
	public MRTradeWindow tradeWindowPrototype;
	public MRMain mainPrototype;
	public GameObject characterCounterPrototype;
	public GameObject mediumMonsterCounterPrototype;
	public GameObject heavyMonsterCounterPrototype;
	public GameObject tremendousMonsterCounterPrototype;
	public GameObject nativeCounterPrototype;
	public GameObject smallChitPrototype;
	public GameObject mediumChitPrototype;
	public GameObject mediumLargeChitPrototype;
	public GameObject largeChitPrototype;
	public GameObject actionChitPrototype;
	public GameObject attentionChitPrototype;
	public GameObject smallCounterPrototype;
	public GameObject mediumCounterPrototype;
	public GameObject largeCounterPrototype;
	public GameObject treasureCardPrototype;
	public GameObject spellCardPrototype;
	public GameObject gamePieceStackPrototype;
	public MRClock gameClock;
	public GUISkin skin;

	public static MRGame TheGame
	{
		get{
			return msTheGame;
		}
	}

	public static eTimeOfDay TimeOfDay
	{
		get{
			return msGameTime;
		}

		set{
			msGameTime = value;
		}
	}

	public static int DayOfMonth
	{
		get{
			return msGameDay;
		}

		set{
			msGameDay = value;
		}
	}

	public static Vector2 TouchPos
	{
		get{
			return msTouchPos;
		}
	}

	public static Vector2 LastTouchPos
	{
		get{
			return msLastTouchPos;
		}
	}

	public static bool JustTouched
	{
		get{
			if (!msShowingUI)
				return msJustTouched;
			return false;
		}
	}

	public static bool IsTouching
	{
		get{
			if (!msShowingUI)
				return msIsTouching;
			return false;
		}
	}

	public static bool IsTouchHeld
	{
		get{
			if (!msShowingUI)
				return msTouchHeld;
			return false;
		}
	}

	public static RaycastHit2D[] TouchedObjects
	{
		get{
			return msTouchedObjects;
		}
	}

	public static bool ShowingUI
	{
		get{
			return msShowingUI;
		}
		set{
			msShowingUI = value;
		}
	}

	public eGameState GameState
	{
		get{
			return mGameState;
		}
	}

	public bool InCombat
	{
		get{
			return mInCombat;
		}

		set{
			mInCombat = value;
		}
	}

	public MRTables Tables
	{
		get{
			return mTables;
		}
	}

	public eViews CurrentView
	{
		get{
			if (mViews.Count > 0)
				return mViews.Peek();
			else
				return eViews.Map;
		}
	}

	public MRCharacterManager CharacterManager
	{
		get{
			return mCharacterManager;
		}
	}

	public MRMap TheMap
	{
		get{
			return mTheMap;
		}
	}

	public MRClock Clock
	{
		get{
			return mClock;
		}
	}

	public MRTreasureChart TreasureChart
	{
		get{
			return mTreasureChart;
		}
	}

	public MRMonsterChart MonsterChart
	{
		get{
			return mMonsterChart;
		}
	}

	public MRCharacterMat CharacterMat
	{
		get{
			return mCharacterMat;
		}
	}

	public MRCombatSheet CombatSheet
	{
		get {
			return mCombatSheet;
		}
	}

	public MRCombatManager CombatManager
	{
		get{
			return mCombatManager;
		}
	}

	public MRMain Main
	{
		get{
			return mOptions;
		}
	}

	public MRActivityListWidget ActivityList
	{
		get{
			return mActivityList;
		}
	}

	public IList<MRIControllable> Controlables
	{
		get{
			return mControllables;
		}
	}

	public MRIControllable ActiveControllable
	{
		get{
			if (mActiveControllableIndex >= 0 && mActiveControllableIndex < mControllables.Count)
				return mControllables[mActiveControllableIndex];
			return null;
		}
	}

	public MRInspectionArea InspectionArea
	{
		get{
			return mInspectionArea;
		}
	}

	public MRGamePieceStack InspectionStack
	{
		get{
			return mInspectionStack;
		}
	}

	public ICollection<uint> Clearings
	{
		get {
			return mClearings.Keys;
		}
	}

	public IList<eMagicColor> WorldMagic
	{
		get{
			return mWorldMagic.AsReadOnly();
		}
	}

	#endregion

	#region Methods

	void Awake()
	{
#if DEBUG
		// set random seed for debugging here
		//MRRandom.seed = 15675905594576123712;
		MRRandom.seed = 1463927239610160118;
#endif
	}

	// Use this for initialization
	void Start ()
	{
		Debug.Log("Random seed = " + MRRandom.seed);

		msTheGame = this;

		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.AutoRotation;

#if UNITY_IPHONE
		Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.Gray);
#elif UNITY_ANDROID
		Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
#endif

		msGameTime = eTimeOfDay.Birdsong;
		msGameDay = 1;
		msShowingUI = false;
		mActiveControllableIndex = 0;
		mInCombat = false;
		mGameState = eGameState.NoGame;

		mTables = new MRTables();

		// static class initialization
		MRSiteChit.Init();
		MRSoundChit.Init();
		MRSuperSiteChit.Init();
		MRWarningChit.Init();

		// find the map button so we can make it the initial main view
		GameObject inspectionArea = (GameObject)Instantiate(inspectionAreaPrototype);
		inspectionArea.transform.parent = transform;
		mInspectionArea = inspectionArea.GetComponent<MRInspectionArea>();
		SetView(eViews.Main);

		// create the options screen
		mOptions = (MRMain)Instantiate(mainPrototype);
		mOptions.transform.parent = transform;

		// create the spells
		mSpellManager = new MRSpellManager();

		// create all the game objects
		mItemManager = new MRItemManager();
		mTreasureChart = (MRTreasureChart)Instantiate(treasureChartPrototype);
		mTreasureChart.transform.parent = transform;

		// create the denizens
		mDenizenManager = new MRDenizenManager();
		mMonsterChart = (MRMonsterChart)Instantiate(monsterChartPrototype);
		mMonsterChart.transform.parent = transform;

		// create the combat sheet
		mCombatSheet = (MRCombatSheet)Instantiate(combatSheetPrototype);
		mCombatSheet.transform.parent = transform;
		mCombatManager = new MRCombatManager();

		// create the game map
		mTheMap = (MRMap)Instantiate(mapPrototype);
		mTheMap.transform.parent = transform;
		mActivityList = (MRActivityListWidget)Instantiate(activityListPrototype);
		mActivityList.transform.parent = transform;
		mClock = (MRClock)Instantiate(gameClock);
		mClock.transform.parent = transform;

		mCharacterManager = new MRCharacterManager();
		mCharacterMat = (MRCharacterMat)Instantiate(characterMatPrototype);
		mCharacterMat.transform.parent = transform;

		AddUpdateEvent(new MRUpdateViewEvent());
	}

	// Update is called once per frame
	void Update ()
	{
		mClearingSelectedThisFrame = false;
		mTileSelectedThisFrame = false;

		TestForTouch();

		// adjust the event list before processing
		foreach (MRUpdateEvent evt in mEventsToRemove)
		{
			if (mUpdateEvents.ContainsKey(evt))
				mUpdateEvents.Remove(evt);
		}
		mEventsToRemove.Clear();
		
		foreach (MRUpdateEvent evt in mEventsToAdd)
		{
			if (!mUpdateEvents.ContainsKey(evt))
				mUpdateEvents.Add(evt, evt);
		}
		mEventsToAdd.Clear();

		// process the event list
		foreach (MRUpdateEvent evt in mUpdateEvents.Keys)
		{
			if (!evt.Update())
				break;
		}

		// update the controllables
		foreach (MRIControllable controllable in mControllables)
		{
			controllable.Update();
		}

		// show/hide the activity list
		if (mActivityList != null)
		{
			mActivityList.Visible = false;
			if ((CurrentView == eViews.Map || CurrentView == eViews.SelectClearing) && mInspectionStack == null && mControllables.Count > 0)
				mActivityList.Visible = true;
			else
				mActivityList.Visible = false;
		}
	}

	public void StartGame()
	{
		mGameState = eGameState.Active;
		SetView(MRGame.eViews.Map);
		AddUpdateEvent(new MRFatigueCharacterEvent());
		AddUpdateEvent(new MRInitGameTimeEvent());
	}

	public void AddUpdateEvent(MRUpdateEvent evt)
	{
		mEventsToAdd.Add(evt);
	}

	public void RemoveUpdateEvent(MRUpdateEvent evt)
	{
		mEventsToRemove.Add(evt);
	}

	public MRUpdateEvent GetUpdateEvent(System.Type eventClass)
	{
		foreach (MRUpdateEvent evt in mUpdateEvents.Keys)
		{
			if (evt.GetType() == eventClass)
				return evt;
		}
		return null;
	}

	public void AddCharacter(MRCharacter character)
	{
		mControllables.Add(character);
	}

	public void RemoveCharacter(MRCharacter character)
	{
		mControllables.Remove(character);
		if (mControllables.Count == 0)
			mGameState = eGameState.GameOver;
	}

	//
	// Set the default gui skin.
	//
	public void OnGUI()
	{
//		GUI.skin = skin;
	}

	public MRGamePieceStack NewGamePieceStack()
	{
		return ((GameObject)Instantiate(gamePieceStackPrototype)).GetComponentInChildren<MRGamePieceStack>();
	}

	/// <summary>
	/// Gets a piece id from json data.
	/// </summary>
	/// <returns>The piece id.</returns>
	/// <param name="data">Data.</param>
	public uint GetPieceId(JSONValue data)
	{
		uint id = 0;
		if (data is JSONNumber)
		{
			id = ((JSONNumber)data).UintValue;
		}
		else if (data is JSONString)
		{
			string name = ((JSONString)data).Value;
			id = GetPieceId (name);
		}
		else
		{
			Debug.LogError("GetPieceId unknown json type");
		}
		return id;
	}

	/// <summary>
	/// Gets a piece id from json data.
	/// </summary>
	/// <returns>The piece id.</returns>
	/// <param name="name">Name.</param>
	public uint GetPieceId(string name)
	{
		uint id = 0;
		string[] splitData = name.Split(new char[]{','});
		if (splitData.Length <= 2)
		{
			name = splitData[0];
			int index = 0;
			if (splitData.Length == 2)
			{
				index = int.Parse(splitData[1]);
			}
			id = MRUtility.IdForName(name, index);
		}
		return id;
	}

	/// <summary>
	/// Gets a game piece from json data.
	/// </summary>
	/// <returns>The game piece.</returns>
	/// <param name="data">Data.</param>
	public MRIGamePiece GetGamePiece(JSONValue data)
	{
		MRIGamePiece piece = null;
		uint id = GetPieceId(data);
		piece = GetGamePiece(id);
		if (piece == null)
		{
			Debug.LogError("GetGamePiece unknown piece id " + id);
		}
		return piece;
	}

	/// <summary>
	/// Gets a game piece for a give id.
	/// </summary>
	/// <returns>The game piece.</returns>
	/// <param name="id">Identifier.</param>
	public MRIGamePiece GetGamePiece(uint id)
	{
		MRIGamePiece piece = null;
		mGamePieces.TryGetValue(id, out piece);
		return piece;
	}

	/// <summary>
	/// Gets a game piece from a name.
	/// </summary>
	/// <returns>The game piece.</returns>
	/// <param name="name">Name.</param>
	public MRIGamePiece GetGamePiece(string name)
	{
		uint id = GetPieceId(name);
		return GetGamePiece(id);
	}

	/// <summary>
	/// Gets a game piece from a name and index.
	/// </summary>
	/// <returns>The game piece.</returns>
	/// <param name="name">Name.</param>
	/// <param name="index">Index.</param>
	public MRIGamePiece GetGamePiece(string name, int index)
	{
		uint id = MRUtility.IdForName(name, index);
		return GetGamePiece(id);
	}

	public void AddGamePiece(MRIGamePiece piece)
	{
		if (GetGamePiece(piece.Id) != null)
		{
			Debug.LogError("Duplicate game piece id for " + piece.Name);
		}
		mGamePieces[piece.Id] = piece;
	}

	public void RemoveGamePiece(MRIGamePiece piece)
	{
		mGamePieces.Remove(piece.Id);
	}

	/// <summary>
	/// Gets a clearing from json data.
	/// </summary>
	/// <returns>The game piece.</returns>
	/// <param name="data">Data.</param>
	public MRClearing GetClearing(JSONValue data)
	{
		MRClearing clearing = null;
		uint id = 0;
		if (data is JSONNumber)
		{
			id = ((JSONNumber)data).UintValue;
		}
		else if (data is JSONString)
		{
			string name = ((JSONString)data).Value;
			id = MRUtility.IdForName(name);
		}
		else
		{
			Debug.LogError("GetClearing unknown json type");
			return null;
		}
		clearing = GetClearing(id);
		if (clearing == null)
		{
			Debug.LogError("GetClearing unknown id " + id);
		}
		return clearing;
	}

	/// <summary>
	/// Returns the clearing with a given name.
	/// </summary>
	/// <returns>The clearing.</returns>
	/// <param name="name">Clearing name.</param>
	public MRClearing GetClearing(string name)
	{
		return GetClearing(MRUtility.IdForName(name));
	}

	/// <summary>
	/// Returns the clearing with a given id.
	/// </summary>
	/// <returns>The clearing.</returns>
	/// <param name="id">Clearing id.</param>
	public MRClearing GetClearing(uint id)
	{
		MRClearing clearing = null;
		mClearings.TryGetValue(id, out clearing);
		return clearing;
	}

	/// <summary>
	/// Adds a clearing to the clearing map. If the map contains a clearing with the same name, the new clearing will be used.
	/// </summary>
	/// <param name="clearing">Clearing.</param>
	public void AddClearing(MRClearing clearing)
	{
		uint id = clearing.Id;
		//Debug.Log("Clearing " + clearing.Name + " id = " + id);
		if (!mClearings.ContainsKey(id))
			mClearings.Add(id, clearing);
		else
		{
			Debug.LogWarning("Duplicate clearing " + clearing.Name + " using new one");
			mClearings[id] = clearing;
		}
	}

	public void RemoveClearing(MRClearing clearing)
	{
		MRClearing test;
		if (mClearings.TryGetValue(clearing.Id, out test))
		{
			if (test == clearing)
				mClearings.Remove(clearing.Id);
		}
	}

	/// <summary>
	/// Returns the clearing with a given name.
	/// </summary>
	/// <returns>The clearing.</returns>
	/// <param name="name">Clearing name.</param>
	public MRRoad GetRoad(string name)
	{
		return GetRoad(MRUtility.IdForName(name));
	}
	
	/// <summary>
	/// Returns the clearing with a given id.
	/// </summary>
	/// <returns>The clearing.</returns>
	/// <param name="id">Clearing id.</param>
	public MRRoad GetRoad(uint id)
	{
		MRRoad road = null;
		mRoads.TryGetValue(id, out road);
		return road;
	}
	
	/// <summary>
	/// Adds a road to the road map. If the map contains a road with the same name, the new road will be used.
	/// </summary>
	/// <param name="clearing">Clearing.</param>
	public void AddRoad(MRRoad road)
	{
		uint id = road.Id;
		if (!mRoads.ContainsKey(id))
			mRoads.Add(id, road);
		else
		{
			Debug.LogWarning("Duplicate road " + road.Name + " using new one");
			mRoads[id] = road;
		}
	}

	/// <summary>
	/// Removes a road from the road map.
	/// </summary>
	/// <param name="road">Road.</param>
	public void RemoveRoad(MRRoad road)
	{
		MRRoad test;
		if (mRoads.TryGetValue(road.Id, out test))
		{
			if (test == road)
				mRoads.Remove(road.Id);
		}
	}

	/// <summary>
	/// Returns the location (clearing or road) with a given id.
	/// </summary>
	/// <returns>The location.</returns>
	/// <param name="id">Location id.</param>
	public MRILocation GetLocation(uint id)
	{
		MRClearing clearing = null;
		if (mClearings.TryGetValue(id, out clearing))
			return clearing;
		MRRoad road = null;
		if (mRoads.TryGetValue(id, out road))
			return road;
		return null;
	}

	/// <summary>
	/// Sets the active controllable to the first controllable in the controllables list.
	/// </summary>
	public void ResetActiveControllable()
	{
		mActiveControllableIndex = 0;
	}

	/// <summary>
	/// Randomizes the order of the controllables in the controllables list.
	/// </summary>
	public void RandomizeControllables()
	{
		mControllables.Shuffle();
	}

	/// <summary>
	/// Changes the game time to the next day phase.
	/// </summary>
	public void NextGameTime()
	{
		if (Controlables.Count == 0)
			return;

		if (TimeOfDay == eTimeOfDay.Birdsong)
		{
			// don't advance if player moving but hasn't selected a clearing
			IList<MRActivity> activities = ActiveControllable.ActivitiesForDay(MRGame.DayOfMonth).Activities;
			foreach (MRActivity activity in activities)
			{
				if (activity is MRMoveActivity && ((MRMoveActivity)activity).Clearing == null)
				{
					return;
				}
			}
			// go to the next controllable before going to the next game time
			if (mActiveControllableIndex < mControllables.Count - 1)
			{
				++mActiveControllableIndex;
				return;
			}
		}
		if (TimeOfDay == MRGame.eTimeOfDay.Daylight)
		{
			// don't advance unless the active controllable has executed all their activities
			if (ActiveControllable != null)
			{
				foreach (MRActivity activity in ActiveControllable.ActivitiesForDay(DayOfMonth).Activities)
				{
					if (!activity.Executed)
					{
						return;
					}
				}
			}
			// go to the next controllable before going to the next game time
			if (mActiveControllableIndex < mControllables.Count - 1)
			{
				++mActiveControllableIndex;
				return;
			}
		}
		if (TimeOfDay == eTimeOfDay.Midnight)
		{
			++DayOfMonth;
			if (DayOfMonth <= 28)
			{
				TimeOfDay = eTimeOfDay.Birdsong;
				mWorldMagic.Clear();
				List<eMagicColor> worldMagic;
				if (mWorldMagicSupply.TryGetValue(DayOfMonth, out worldMagic))
					mWorldMagic = worldMagic;
			}
			else
			{
				// for now we only allow 4-week games
				--DayOfMonth;
				mGameState = eGameState.GameOver;
				// force a map refresh to show the game over message
				TheMap.Visible = true;
				return;
			}
		}
		else
			++TimeOfDay;
		AddUpdateEvent(new MRInitGameTimeEvent());
	}

	/// <summary>
	/// Called by a clearing when it has been double-clicked. Passes the message back down to the game's children.
	/// </summary>
	/// <param name="clearing">The clearing selected.</param>
	public void OnClearingSelectedGame(MRClearing clearing)
	{
		// send the message back down
		if (!mClearingSelectedThisFrame)
		{
			mClearingSelectedThisFrame = true;
			// clearing selection is handled by MRSelectClearingEvents, so pass the clearing to it
			foreach (MRUpdateEvent evt in mUpdateEvents.Keys)
			{
				evt.OnClearingSelected(clearing);
			}
			//BroadcastMessage("OnClearingSelected", clearing, SendMessageOptions.DontRequireReceiver);
		}
	}

	/// <summary>
	/// Sets the current view to a given view.
	/// </summary>
	/// <param name="view">View to show.</param>
	public void SetView(eViews view)
	{
		if (InCombat && view == eViews.Map)
			view = eViews.Combat;
		PopView();
		PushView(view);
		// we've got one more update view event on the event list than we need, so remove it
		mEventsToAdd.RemoveAt(mEventsToAdd.Count - 1);
	}

	/// <summary>
	/// Sets the current view to a given view, keeping track of the previous view. 
	/// The previous view can be restored by calling PopView().
	/// </summary>
	/// <param name="view">View.</param>
	public void PushView(eViews view)
	{
		mViews.Push(view);
		AddUpdateEvent(new MRUpdateViewEvent());
	}

	/// <summary>
	/// Restores the previous view of a call to PushView().
	/// </summary>
	public void PopView()
	{
		if (mViews.Count > 0)
			mViews.Pop();
		AddUpdateEvent(new MRUpdateViewEvent());
	}

	/// <summary>
	/// Called by a view button when it has been selected
	/// </summary>
	/// <param name="button">the button that was pressed</param>
	public void OnViewButtonSelectedGame(MRViewButton button)
	{
		// don't change the view if we're forcing it
		if (mViews.Count > 1)
			return;

		SetView(button.id);
		InspectStack(null);
	}

	//
	// Called by a tile when it has been double-clicked. Passes the message back down to the game's children.
	//
	public void OnTileSelectedGame(MRTile tile)
	{
		// send the message back down
		if (!mTileSelectedThisFrame)
		{
			mTileSelectedThisFrame = true;
			BroadcastMessage("OnTileSelected", tile, SendMessageOptions.DontRequireReceiver);
		}
	}

	/// <summary>
	/// Called when a game piece has been selected.
	/// </summary>
	/// <param name="piece">The selected piece.</param>
	public void OnGamePieceSelectedGame(MRIGamePiece piece)
	{
		BroadcastMessage("OnGamePieceSelected", piece, SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Called when the player selects "none" or "cancel" when selecting an attack or maneuver chit.
	/// </summary>
	/// <param name="option">The option selected</param>
	public void OnAttackManeuverSelectedGame(MRCombatManager.eAttackManeuverOption option)
	{
		BroadcastMessage("OnAttackManeuverSelected", option, SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Inspects a game piece stack.
	/// </summary>
	/// <param name="stack">the stack</param>
	public void InspectStack(MRGamePieceStack stack)
	{
		// show/hide the inspection stack
		if (mInspectionStack != null)
			mInspectionStack.Inspecting = false;
		mInspectionStack = stack;
		if (mInspectionStack != null)
			mInspectionStack.Inspecting = true;
	}

	/// <summary>
	/// Rolls for a curse.
	/// </summary>
	/// <param name="roller">Controllable rolling the curse (may be null)</param>
	/// <param name="target">Target of the curse</param>
	public void RollForCurse(MRIControllable roller, MRIControllable target)
	{
		if (target is MRCharacter)
		{
			AddUpdateEvent(new MRRollForCurseEvent(roller, (MRCharacter)target));
		}
	}

	/// <summary>
	/// Displays a simple message dialog box with an "ok" button.
	/// </summary>
	/// <param name="message">the message to display in the dislog</param>
	public void ShowInformationDialog(string message)
	{
		MRMainUI.TheUI.DisplayMessageDialog(message);
	}

	/// <summary>
	/// Displays a simple message dialog box with an "ok" button.
	/// </summary>
	/// <param name="message">the message to display in the dialog</param>
	/// <param name="title">the dialog title</param>
	public void ShowInformationDialog(string message, string title)
	{
		MRMainUI.TheUI.DisplayMessageDialog(message, title);
	}

	//
	// Updates the raw mouse/touch state of the game.
	//
	private void TestForTouch()
	{
		if (ShowingUI)
			return;

		msJustTouched = false;
		msTouchedObjects = null;

		if (Application.platform == RuntimePlatform.Android ||
		    Application.platform == RuntimePlatform.IPhonePlayer)
		{
			// see if the user pressed the "back" button
			if (Input.GetKey(KeyCode.Escape))
			{
				if (mInspectionStack != null)
					InspectStack(null);
				else
				{
					// todo: save game?
					Application.Quit();
				}
				return;
			}

			if (Input.touchCount == 1)
			{
				msTouchDuration += Time.deltaTime;
				msTouch = Input.GetTouch(0);
				switch (msTouch.phase)
				{
					case TouchPhase.Began:
//						Debug.Log("MRGame TestForTouch touch began");
						msIsTouching = true;
						msJustTouched = true;
						msStartTouchPos = msTouch.position;
						msTouchPos = msStartTouchPos;
						msLastTouchPos = msStartTouchPos;
						HandleTouch(eTouchType.Touched);
						break;
					case TouchPhase.Stationary:
//						Debug.Log("MRGame TestForTouch touch stationary");
						if (msTouchDuration >= TOUCH_HELD_TIME)
						{
							if (!msTouchHeld)
							{
								msTouchHeld = true;
								HandleTouch(eTouchType.Held);
							}
						}
						break;
					case TouchPhase.Moved:
//						Debug.Log("MRGame TestForTouch touch moved");
						msIsTouching = true;
						msLastTouchPos = msTouchPos;
						msTouchPos = msTouch.position;
						HandleTouch(eTouchType.Move, msTouchPos.x - msLastTouchPos.x, msTouchPos.y - msLastTouchPos.y);
						break;
					case TouchPhase.Ended:
//						HandleRelease();
						if (msIsTouching && msTouchDuration < 0.2f) //making sure it only check the touch once && it was a short touch/tap and not a dragging.
						{
							if (msTapCoroutine == null)
							{
//								Debug.Log("MRGame TestForTouch touch ended, calling testsingleordouble");
								msTapCoroutine = StartCoroutine(TestSingleOrDoubleTap());
								msIsTouching = false;
							}
						}
						else
						{
							HandleRelease();
						}
						break;
					default:
						break;
				}
			}
			else if (Input.touchCount == 2 && TheMap != null)
			{
				// test for pinch zoom
				Touch touch0 = Input.GetTouch(0);
				Touch touch1 = Input.GetTouch(1);
				Vector2 touch0Pos = TheMap.MapCamera.ScreenToViewportPoint(touch0.position); 
				Vector2 touch1Pos = TheMap.MapCamera.ScreenToViewportPoint(touch1.position); 
				Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
				Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
				touch0PrevPos = TheMap.MapCamera.ScreenToViewportPoint(touch0PrevPos); 
				touch1PrevPos = TheMap.MapCamera.ScreenToViewportPoint(touch1PrevPos);
				float prevTouchDelta = (touch0PrevPos - touch1PrevPos).magnitude;
				float touchDelta = (touch0Pos - touch1Pos).magnitude;
				float pinchDelta = prevTouchDelta - touchDelta;
				HandleTouch(eTouchType.PinchZoom, pinchDelta);
			}
			else
			{
				msTouchDuration = 0;
				msIsTouching = false;
				msJustTouched = false;
				msTouchHeld = false;
			}
		}
		else
		{
			// PC - mouse handling
			bool justReleased = false;

			if (msIsTouching)
				msLastTouchPos = msTouchPos;
			else
				msLastTouchPos = Input.mousePosition;
			msJustTouched = Input.GetMouseButtonDown(0);
			justReleased = Input.GetMouseButtonUp(0);
			msIsTouching = Input.GetMouseButton(0);
			msTouchPos = Input.mousePosition;

			if (msJustTouched)
			{
				msStartTouchPos = Input.mousePosition;
				msTouchDuration = 0;
				msLastTouchTime = Time.time;
				HandleTouch(eTouchType.Touched);
			}
			else if (msIsTouching)
			{
				msTouchDuration += Time.deltaTime;
				// see if the touch is being held
				if (Time.time - msLastTouchTime >= TOUCH_HELD_TIME)
				{
					if (!msTouchHeld && 
					    Math.Abs(msLastTouchPos.x - msTouchPos.x) < 0.1f &&
					    Math.Abs(msLastTouchPos.y - msTouchPos.y) < 0.1f)
					{
					    msTouchHeld = true;
						HandleTouch(eTouchType.Held);
					}
				}
				HandleTouch(eTouchType.Move, msTouchPos.x - msLastTouchPos.x, msTouchPos.y - msLastTouchPos.y);
			}
			else if (justReleased)
			{
				if (msTouchDuration > 0 && msTouchDuration < 0.2f) //making sure it only check the touch once && it was a short touch/tap and not a dragging.
				{
					++msClickCount;
					if (msTapCoroutine == null)
						msTapCoroutine = StartCoroutine(TestSingleOrDoubleClick());
				}
				else
				{
					HandleRelease();
				}
				msLastReleaseTime = Time.time;
				msTouchHeld = false;
			}
		}
	}

	/// <summary>
	/// Tests if the user single or double tapped on touch devices. Call this as a coroutine.
	/// </summary>
	/// <returns>The coroutine yield.</returns>
	private IEnumerator TestSingleOrDoubleTap()
	{
//		Debug.Log("MRGame TestSingleOrDoubleTap enter");
		yield return new WaitForSeconds(0.25f);
//		Debug.Log("MRGame TestSingleOrDoubleTap after yield");
		msLastTouchPos = msTouch.position;
		eTouchType touchType = eTouchType.None;
		if (msTouch.tapCount == 1)
		{
			touchType = eTouchType.Single;
		}
		else if (msTouch.tapCount == 2)
		{
			// this coroutine has been called twice. We should stop the next one here otherwise we get two double tap
			touchType = eTouchType.Double;
		}

		StopCoroutine(msTapCoroutine);
		msTapCoroutine = null;
		if (touchType != eTouchType.None)
		{
			HandleTouch(touchType);
		}
//		Debug.Log("MRGame TestSingleOrDoubleTap exit");
	}

	/// <summary>
	/// Tests if the user single or double clicked with a mouse. Call this as a coroutine.
	/// </summary>
	/// <returns>The coroutine yield.</returns>
	private IEnumerator TestSingleOrDoubleClick()
	{
		System.DateTime then = System.DateTime.Now;
//		Debug.Log("MRGame TestSingleOrDoubleClick enter");
		yield return new WaitForSeconds(0.25f);
		System.DateTime now = System.DateTime.Now;
//		Debug.Log("MRGame TestSingleOrDoubleClick after yield, dt = " + ((now - then).Ticks / 10000f));
//		Debug.Log("TestSingleOrDoubleTap, msClickCount="+msClickCount);
//		msLastTouchPos = msTouch.position;
		eTouchType touchType = eTouchType.None;
		if (msClickCount == 1)
		{
			touchType = eTouchType.Single;
		}
		else if (msClickCount > 1)
		{
			// this coroutine has been called twice. We should stop the next one here otherwise we get two double tap
			touchType = eTouchType.Double;
		}

		StopCoroutine(msTapCoroutine);
		msTapCoroutine = null;
		msClickCount = 0;
		msTouchDuration = 0;
		if (touchType != eTouchType.None)
		{
			HandleTouch(touchType);
		}
	}

	// class to hold data for OnTouched()
	private class TouchedData : IComparable<TouchedData>
	{
		public TouchedData(MRITouchable _touched, GameObject _hitObject)
		{
			touched = _touched;
			hitObject = _hitObject;
		}
		public MRITouchable touched;
		public GameObject hitObject;

		public int CompareTo(TouchedData other)
		{
			float myZ = hitObject.transform.position.z;
			float otherZ = other.hitObject.transform.position.z;
			return (myZ < otherZ) ? -1 : 1;
		}
	}

	/// <summary>
	/// Determines which widget(s) were touched by a tap/click, and tells them to activate.
	/// </summary>
	/// <param name="touchType">Touch type.</param>
	private void HandleTouch(eTouchType touchType)
	{
		HandleTouch(touchType, 0);
	}

	/// <summary>
	/// Determines which widget(s) were touched by a tap/click, and tells them to activate.
	/// </summary>
	/// <param name="touchType">Touch type.</param>
	/// <param name="delta">Position delta for move or zoom</param>
	private void HandleTouch(eTouchType touchType, float delta)
	{
		HandleTouch(touchType, delta, 0);
	}

	/// <summary>
	/// Determines which widget(s) were touched by a tap/click, and tells them to activate.
	/// </summary>
	/// <param name="touchType">Touch type.</param>
	/// <param name="delta1">Position delta 1 for move or zoom</param>
	/// <param name="delta1">Position delta 2 for move or zoom</param>
	private void HandleTouch(eTouchType touchType, float delta1,  float delta2)
	{
//		Debug.Log("MRGame OnTouched enter");
		// find the object touched
		List<TouchedData> touched = new List<TouchedData>();
		Camera[] cameras = Camera.allCameras;
		for (int i = 0; i < cameras.Length; ++i)
		{
			Camera camera = cameras[i];
			Vector3 worldTouch = camera.ScreenToWorldPoint(new Vector3(msStartTouchPos.x, msStartTouchPos.y, camera.nearClipPlane));
			RaycastHit2D[] hits = Physics2D.RaycastAll(worldTouch, Vector2.zero, Mathf.Infinity, camera.cullingMask);
			for (int j = 0; j < hits.Length; ++j)
			{
				if (hits[j].collider != null) 
				{
					GameObject hitObject = hits[j].transform.gameObject;
					MonoBehaviour[] touchables = hitObject.GetComponentsInParent<MonoBehaviour>(false);
					for (int k = 0; k < touchables.Length; ++k)
					{
						MonoBehaviour touchable = touchables[k];
						if (touchable != null && touchable is MRITouchable && touchable.gameObject)
						{
							touched.Add(new TouchedData((MRITouchable)touchable, hitObject));
							// break out of 2 inner loops
							k = touchables.Length;
							//j = hits.Length;
						}
					}
				}
			}
		}
		if (touched.Count > 0)
		{
			touched.Sort ();
			bool handled = false;
			for (int i = 0; i < touched.Count && !handled; ++i)
			{
				if (touchType == eTouchType.Touched)
				{
//					HandleRelease();
					mLastTouched = touched[i].touched;
					handled = touched[i].touched.OnTouched(touched[i].hitObject);
				}
				if (touchType == eTouchType.Single)
				{
					handled = touched[i].touched.OnSingleTapped(touched[i].hitObject);
					HandleRelease();
				}
				else if (touchType == eTouchType.Double)
				{
					handled = touched[i].touched.OnDoubleTapped(touched[i].hitObject);
					HandleRelease();
				}
				else if (touchType == eTouchType.Held)
					handled = touched[i].touched.OnTouchHeld(touched[i].hitObject);
				else if (touchType == eTouchType.Move)
					handled = touched[i].touched.OnTouchMove(touched[i].hitObject, delta1, delta2);
				else if (touchType == eTouchType.PinchZoom)
					handled = touched[i].touched.OnPinchZoom(delta1);
			}
		}
	}

	/// <summary>
	/// Send a message to the last button that was touched that it has been released.
	/// </summary>
	private void HandleRelease()
	{
		if (mLastTouched != null)
			mLastTouched.OnReleased(null);
		mLastTouched = null;
	}

	/// <summary>
	/// Loads the world data.
	/// @todo eventually this will be configurable for a given setup/scenario
	/// </summary>
	public void LoadWorldData(string worldName)
	{
		try
		{
			TextAsset worldData = (TextAsset)Resources.Load(worldName);
			StringBuilder jsonText = new StringBuilder(worldData.text);
			JSONObject root = (JSONObject)JSONDecoder.CreateJSONValue(jsonText);

			// read the global magic supplied for various days
			mWorldMagicSupply.Clear();
			if (root["magic_supply"] != null)
			{
				JSONArray magicSupply = (JSONArray)root["magic_supply"];
				for (int i = 0; i < magicSupply.Count; ++i)
				{
					JSONObject magicData = (JSONObject)magicSupply[i];
					int day = ((JSONNumber)magicData["day"]).IntValue;
					JSONArray colorData = (JSONArray)magicData["colors"];
					List<eMagicColor> colors = new List<eMagicColor>();
					for (int j = 0; j < colorData.Count; ++j)
					{
						colors.Add(((JSONString)colorData[j]).Value.ToColor());
					}
					mWorldMagicSupply[day] = colors;
				}
			}
			if (root["map"] != null)
			{
				TheMap.Load((JSONObject)root["map"]);
			}
			if (root["characters"] != null)
			{
				JSONArray characters = (JSONArray)root["characters"];
				for (int i = 0; i < characters.Count; ++i)
				{
					JSONObject characterData = (JSONObject)characters[i];
					JSONString characterName = (JSONString)characterData["name"];
					MRCharacter character = CharacterManager.CreateCharacter(characterName.Value);
					character.Load(characterData);
					AddCharacter(character);
				}
			}
			if (root["clearings"] != null)
			{
				JSONArray clearings = (JSONArray)root["clearings"];
				for (int i = 0; i < clearings.Count; ++i)
				{
					JSONObject clearingData = (JSONObject)clearings[i];
					MRMap.eTileNames tileId;
					if (!MRMap.TileNameMap.TryGetValue(((JSONString)clearingData["tile"]).Value, out tileId))
					{
						return;
					}
					MRTile tile = TheMap.MapTiles[tileId];
					int clearingNumber = ((JSONNumber)clearingData["number"]).IntValue;
					uint clearingId = MRUtility.IdForName(tile.FrontSide.ShortName + clearingNumber);
					MRClearing clearing;
					if (mClearings.TryGetValue(clearingId, out clearing))
					{
						clearing.Load(clearingData);
					}
				}
			}
			if (root["monsterChart"] != null)
			{
				mMonsterChart.Load(root);
			}
			if (root["treasures"] != null)
			{
				JSONObject treasures = (JSONObject)root["treasures"];
				mTreasureChart.Load(treasures);
			}
			if (root["random"] != null)
			{
				JSONObject randomData = (JSONObject)root["random"];
				MRRandom.Load(randomData);
			}
		}
		catch (Exception err)
		{
			Debug.LogError("Error parsing world data:" + err);
		}
	}

	/// <summary>
	/// Resets the game to its initial state for a new game.
	/// </summary>
	public IEnumerator Reset()
	{
		// force end of combat
		CombatManager.CombatPhase = MRCombatManager.eCombatPhase.CombatDone;
		CombatManager.Clearing = null;
		mInCombat = false;

		// remove the characters
		foreach (MRIControllable controllable in mControllables)
		{
			if (controllable is MRCharacter)
			{
				((MRCharacter)controllable).Destroy();
			}
		}
		mControllables.Clear();

		// reset the denizens
		MRDenizenManager.ResetDenizens();
		mMonsterChart.ResetMonsters();

		// reset the treasures
		MRItemManager.ResetItems();
		mTreasureChart.CreateTreasures();

		// clear the map
		mTheMap.ClearMap();
		yield return new WaitForSeconds(0.1f);

		// reset the game state
		msGameTime = eTimeOfDay.Birdsong;
		msGameDay = 1;
		msShowingUI = false;
		mActiveControllableIndex = 0;
		mGameState = eGameState.NoGame;
	}

	/// <summary>
	/// Load the game from json data.
	/// </summary>
	/// <param name="root">The root json structure</param>
	public bool Load(JSONObject root)
	{
		Debug.Log("Load game start");
#if UNITY_ANDROID || UNITY_IOS
		Handheld.StartActivityIndicator();
#endif
		// load global data
		msGameTime = (eTimeOfDay)((JSONNumber)root["time"]).IntValue;
		msGameDay = ((JSONNumber)root["day"]).IntValue;

		// load the game name
		if (root["gameName"] != null)
		{
			JSONString gameName = (JSONString)root["gameName"];
			MRGame.TheGame.Main.CurrentSaveGameName = gameName.Value;
		}

		// load the random number generator
		if (root["random"] != null)
		{
			JSONObject randomData = (JSONObject)root["random"];
			if (!MRRandom.Load(randomData))
			{
#if UNITY_ANDROID || UNITY_IOS
				Handheld.StopActivityIndicator();
#endif
				Debug.LogError("Load random number generator error");
				return false;
			}
		}

		// load the map
		JSONObject mapData = (JSONObject)root["map"];
		if (!mTheMap.Load(mapData))
		{
#if UNITY_ANDROID || UNITY_IOS
			Handheld.StopActivityIndicator();
#endif
			Debug.LogError("Load game map error");
			return false;
		}
		MRSiteChit.VaultOpened = ((JSONBoolean)root["vault"]).Value;

		// load characters
		mControllables.Clear();
		JSONArray characters = (JSONArray)root["characters"];
		for (int i = 0; i < characters.Count; ++i)
		{
			JSONObject characterData = (JSONObject)characters[i];
			JSONString characterName = (JSONString)characterData["name"];
			MRCharacter character = CharacterManager.CreateCharacter(characterName.Value);
			if (character == null)
			{
#if UNITY_ANDROID || UNITY_IOS
				Handheld.StopActivityIndicator();
#endif
				Debug.LogError("Load game character create error");
				return false;
			}
			if (!character.Load(characterData))
			{
#if UNITY_ANDROID || UNITY_IOS
				Handheld.StopActivityIndicator();
#endif
				Debug.LogError("Load game character error");
				return false;
			}
			AddCharacter(character);
		}

		// load other controllables

		// load clearings
		JSONArray clearings = (JSONArray)root["clearings"];
		for (int i = 0; i < clearings.Count; ++i)
		{
			JSONObject clearingData = (JSONObject)clearings[i];
			uint clearingId = ((JSONNumber)clearingData["id"]).UintValue;
			MRClearing clearing;
			if (mClearings.TryGetValue(clearingId, out clearing))
			{
				clearing.Load(clearingData);
			}
		}

		// load roads
		if (root["roads"] != null)
		{
			JSONArray roads = (JSONArray)root["roads"];
			for (int i = 0; i < roads.Count; ++i)
			{
				JSONObject roadData = (JSONObject)roads[i];
				string roadName = ((JSONString)roadData["name"]).Value;
				MRRoad road = GetRoad(roadName);
				if (road != null)
				{
					road.Load(roadData);
				}
				else
				{
					Debug.LogError("Tried to load data for unknown road " + roadName);
				}
			}
		}

		if (root["monsterChart"] != null)
		{
			mMonsterChart.Load(root);
		}

		// load the treasure chart
		if (root["treasures"] != null)
		{
			JSONObject treasures = (JSONObject)root["treasures"];
			mTreasureChart.Load(treasures);
		}

		if (root["active"] != null)
			mActiveControllableIndex = ((JSONNumber)root["active"]).IntValue;
		MRActivityList currentActivityList = ActiveControllable.ActivitiesForDay(MRGame.DayOfMonth);
		MRGame.TheGame.ActivityList.ActivityList = currentActivityList;

		if (root["monsterRoll"] != null)
			mMonsterChart.MonsterRoll = ((JSONNumber)root["monsterRoll"]).IntValue;

		mWorldMagic.Clear();
		List<eMagicColor> worldMagic;
		if (mWorldMagicSupply.TryGetValue(DayOfMonth, out worldMagic))
			mWorldMagic = worldMagic;
		TheGame.Clock.UpdateDate();

#if UNITY_ANDROID || UNITY_IOS
		Handheld.StopActivityIndicator();
#endif
		Debug.Log("Load game end");
		return true;
	}

	public void Save(JSONObject root)
	{
		Debug.Log("Save game start");
#if UNITY_ANDROID || UNITY_IOS
		Handheld.StartActivityIndicator();
#endif
		// save global data
		root["gameName"] = new JSONString(MRGame.TheGame.Main.CurrentSaveGameName);
		root["time"] = new JSONNumber((int)msGameTime);
		root["day"] = new JSONNumber(msGameDay);
		root["vault"] = new JSONBoolean(MRSiteChit.VaultOpened);
		root["active"] = new JSONNumber(mActiveControllableIndex);
		root["monsterRoll"] = new JSONNumber(mMonsterChart.MonsterRoll);

		// save the random number generator
		JSONObject randomData = new JSONObject();
		MRRandom.Save(randomData);
		root["random"] = randomData;

		// save the map
		JSONObject mapData = new JSONObject();
		mTheMap.Save(mapData);
		root["map"] = mapData;

		// save the characters
		int characterCount = 0;
		foreach (MRIControllable controllable in mControllables)
		{
			if (controllable is MRCharacter)
				++characterCount;
		}
		JSONArray characters = new JSONArray(characterCount);
		int characterIndex = 0;
		foreach (MRIControllable controllable in mControllables)
		{
			if (controllable is MRCharacter)
			{
				JSONObject characterData = new JSONObject();
				((MRCharacter)controllable).Save(characterData);
				characters[characterIndex] = characterData;
			}
		}
		root["characters"] = characters;

		// save other controllables

		// save clearings
		JSONArray clearings = new JSONArray(mClearings.Count);
		int clearingIndex = 0;
		foreach (MRClearing clearing in mClearings.Values)
		{
			JSONObject clearingData = new JSONObject();
			clearing.Save(clearingData);
			clearings[clearingIndex++] = clearingData;
		}
		root["clearings"] = clearings;

		// save roads that have something on them
		int roadCount = 0;
		foreach (MRRoad road in TheMap.Roads.Values)
		{
			if (road.Pieces.Count > 0)
				++roadCount;
		}
		if (roadCount > 0)
		{
			JSONArray roads = new JSONArray(roadCount);
			int roadIndex = 0;
			foreach (MRRoad road in TheMap.Roads.Values)
			{
				if (road.Pieces.Count > 0)
				{
					JSONObject roadData = new JSONObject();
					road.Save(roadData);
					roads[roadIndex++] = roadData;
				}
			}
			root["roads"] = roads;
		}

		// save the monster data
		mMonsterChart.Save(root);

		// save the treasure chart
		JSONObject treasures = new JSONObject();
		mTreasureChart.Save(treasures);
		root["treasures"] = treasures;

#if UNITY_ANDROID || UNITY_IOS
		Handheld.StopActivityIndicator();
#endif
		Debug.Log("Save game end");
	}

	#endregion

	#region Members

	private SortedList<MRUpdateEvent, MRUpdateEvent> mUpdateEvents= new SortedList<MRUpdateEvent, MRUpdateEvent>();
	private List<MRUpdateEvent> mEventsToAdd = new List<MRUpdateEvent>();
	private List<MRUpdateEvent> mEventsToRemove = new List<MRUpdateEvent>();

	private eGameState mGameState;
	private bool mInCombat;
	private Stack<eViews> mViews = new Stack<eViews>();

	private MRInspectionArea mInspectionArea;

	private MRMap mTheMap;

	private MRActivityListWidget mActivityList;
	private MRClock mClock;
	private Dictionary<uint, MRClearing> mClearings = new Dictionary<uint, MRClearing>();
	private Dictionary<uint, MRRoad> mRoads = new Dictionary<uint, MRRoad>();
	private Dictionary<uint, MRIGamePiece> mGamePieces = new Dictionary<uint, MRIGamePiece>();
	private MRTables mTables;
	private MRCharacterManager mCharacterManager;
	private MRCharacterMat mCharacterMat;
	private MRItemManager mItemManager;
	private MRDenizenManager mDenizenManager;
	private MRSpellManager mSpellManager;
	private MRTreasureChart mTreasureChart;
	private MRMonsterChart mMonsterChart;
	private MRCombatSheet mCombatSheet;
	private MRMain mOptions;
	private MRGamePieceStack mInspectionStack;
	private MRCombatManager mCombatManager;

	private Dictionary<int, List<eMagicColor>> mWorldMagicSupply = new Dictionary<int, List<eMagicColor>>();
	private List<eMagicColor> mWorldMagic = new List<eMagicColor>();

	private List<MRIControllable> mControllables = new List<MRIControllable>();
	private int mActiveControllableIndex;
	private bool mClearingSelectedThisFrame;
	private bool mTileSelectedThisFrame;

	private static MRGame msTheGame;
	private static Touch msTouch;
	private static Coroutine msTapCoroutine;
	private static Vector2 msStartTouchPos;
	private static Vector2 msTouchPos;
	private static Vector2 msLastTouchPos;
	private static RaycastHit2D[] msTouchedObjects;
	private static float msLastTouchTime;
	private static float msLastReleaseTime;
	private static float msTouchDuration;
	private static int msClickCount;
	private static bool msIsTouching;
	private static bool msJustTouched;
	private static bool msTouchHeld;
	private static MRITouchable mLastTouched;
	private static bool msShowingUI;

	private static eTimeOfDay msGameTime;
	private static int msGameDay;

	#endregion
}

}