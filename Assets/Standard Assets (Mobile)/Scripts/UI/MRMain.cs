//
// MRMain.cs
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
using System.IO;
using System.Text;
using AssemblyCSharp;

public class MRMain : MonoBehaviour, MRITouchable
{
	#region Constants

	public enum OptionsState
	{
		NoGame,
		NewGame,
		SelectStartingLocations,
		GameStarted,
	}

	#endregion

	#region Properties

	// for debugging
	//public TextMesh TapTest;

	public bool Visible
	{
		get{
			return mCamera.enabled;
		}
		
		set{
			mCamera.enabled = value;
			if (value)
			{
				MRGame.TheGame.InspectStack(mSelectedCharactersStack);
			}
			else if (MRGame.TheGame.InspectionStack == mSelectedCharactersStack)
			{
				MRGame.TheGame.InspectStack(null);
			}
		}
	}

	public int CurrentSaveGameSlot
	{
		get{
			return mCurrentSaveGameSlot;
		}

		set{
			mCurrentSaveGameSlot = value;
		}
	}

	public string CurrentSaveGameName
	{
		get{
			return mCurrentSaveGameName;
		}

		set{
			mCurrentSaveGameName = value;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		mState = OptionsState.NoGame;
		mCurrentSaveGameSlot = -1;
		mCurrentSaveGameName = "";

		mAvailableCharactersStack = MRGame.TheGame.NewGamePieceStack();
		mAvailableCharactersStack.Layer = LayerMask.NameToLayer("Dummy");

		mSelectedCharactersStack = MRGame.TheGame.NewGamePieceStack();
		mSelectedCharactersStack.gameObject.name = "SelectedCharactersStack";
		mSelectedCharactersStack.transform.parent = MRGame.TheGame.transform;

		// get the camera
		mCamera = gameObject.GetComponentInChildren<Camera>();

		// find the game objects in the view
		Transform[] transforms = GetComponentsInChildren<Transform>();
		foreach (Transform transform in transforms)
		{
			GameObject obj = transform.gameObject;
			switch (obj.name)
			{
				case "StartScreen":
					mStartScreen = obj;
					break;
				case "SelectCharacter":
					mSelectCharacter = obj;
					break;
				case "newGame":
					mNewGameButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "loadGame":
					mLoadGameButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "saveGame":
					mSaveGameButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "instructions":
					mInstructionsButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "credits":
					mCreditsButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "addCharacter":
					mAddCharacterButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "removeCharacter":
					mRemoveCharacterButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "startGame":
					mStartGameButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "back":
					mBackButton = obj.GetComponentInChildren<MRButton>();
					break;
				case "CharacterData":
					mCharacterDisplay = obj;
					break;
				case "nextCharacter":
					mCharacterRightArrow = obj.GetComponentInChildren<MRButton>();
					mEnabledArrow = mCharacterRightArrow.GetComponent<SpriteRenderer>().sprite;
					break;
				case "prevCharacter":
					mCharacterLeftArrow = obj.GetComponentInChildren<MRButton>();
					mDisabledArrow = mCharacterLeftArrow.GetComponent<SpriteRenderer>().sprite;
					break;
				case "Seed":
					mRandomSeed = obj.GetComponent<TextMesh>();
					break;
				case "Version":
					{
						TextMesh versionText = obj.GetComponent<TextMesh>();
						versionText.text = "V" + Application.version;
					}
					break;
				case "name":
					if (obj.transform.parent.name == "ownerName")
						mCharacterName = obj.GetComponent<TextMesh>();
					break;
				case "weight":
					mCharacterWeight = obj.GetComponent<TextMesh>();
					break;
				case "location":
					mCharacterStartLocation = obj.GetComponent<TextMesh>();
					break;
				case "special1":
					mCharacterAbilities[0] = obj.GetComponent<TextMesh>();
					break;
				case "special2":
					mCharacterAbilities[1] = obj.GetComponent<TextMesh>();
					break;
				case "weapon":
					mWeaponPosition = obj;
					break;
				case "armor":
					mArmorPosition = obj;
					break;
				case "helmet":
					mHelmetPosition = obj;
					break;
				case "shield":
					mShieldPosition = obj;
					break;
				default:
					if (obj.name.StartsWith("chit"))
						mChitPositions[int.Parse(obj.name.Substring("chit".Length)) - 1] = obj;
					break;
			}
		}

		((SpriteRenderer)(mCharacterLeftArrow.GetComponent<Renderer>())).sprite = mEnabledArrow;
		((SpriteRenderer)(mCharacterRightArrow.GetComponent<Renderer>())).sprite = mEnabledArrow;
	}

	// Update is called once per frame
	void Update ()
	{
		if (!Visible)
			return;

		if (mRandomSeed != null)
			mRandomSeed.text = "Seed: " + MRRandom.seed;

		switch (mState)
		{
			case OptionsState.NoGame:
				MRUtility.SetObjectVisibility(mStartScreen, true);
				MRUtility.SetObjectVisibility(mSelectCharacter, false);
				mSaveGameButton.Visible = false;
				break;
			case OptionsState.NewGame:
				MRUtility.SetObjectVisibility(mStartScreen, false);
				MRUtility.SetObjectVisibility(mSelectCharacter, true);
				mAddCharacterButton.Visible = (mAvailableCharacters.Count > 0);
				mRemoveCharacterButton.Visible = true;
				mStartGameButton.Visible = (mSelectedCharacters.Count > 0);
				mBackButton.Visible = (mSelectedCharacters.Count == 0);
				break;
			case OptionsState.SelectStartingLocations:
				MRUtility.SetObjectVisibility(mStartScreen, false);
				MRUtility.SetObjectVisibility(mSelectCharacter, false);
				SelectStartingLocations();
				break;
			case OptionsState.GameStarted:
				MRUtility.SetObjectVisibility(mStartScreen, true);
				MRUtility.SetObjectVisibility(mSelectCharacter, false);
				mSaveGameButton.Visible = true;
				break;
		}

		if (mDisplayedCharacterIndex >= 0 && mAvailableCharacters.Count > 0)
		{
			// display the character info
			MRCharacter character = mAvailableCharacters[mDisplayedCharacterIndex];
			mCharacterName.text = MRUtility.DisplayName(character.Name);
			mCharacterWeight.text = "Weight - " + character.BaseWeight.ToString();
			mCharacterStartLocation.text = "Start at - ";
			for (int i = 0; i < character.StartingLocations.Length; ++i)
			{
				if (i > 0)
					mCharacterStartLocation.text += ", ";
				mCharacterStartLocation.text += MRUtility.DisplayName(character.StartingLocations[i]);
			}
			for (int i = 0; i < mCharacterAbilities.Length; ++i)
				mCharacterAbilities[i].text = "";
			for (int i = 0; i < character.Abilities.Length; ++i)
			{
				if (i < mCharacterAbilities.Length)
					mCharacterAbilities[i].text += MRUtility.DisplayName(character.Abilities[i]);
			}

			// display the character's equipment
			IList<MRItem> activeItems = character.ActiveItems;
			foreach (MRItem item in activeItems)
			{
				if (item is MRWeapon)
				{
					SetItemPosition(item, mWeaponPosition);
				}
				else if (item is MRArmor)
				{
					MRArmor armor = (MRArmor)item;
					switch (armor.Type)
					{
						case MRArmor.eType.Breastplate:
						case MRArmor.eType.Full:
							// breastplate and full are shown in the same slot, since no character starts with both
							SetItemPosition(item, mArmorPosition);
							break;
						case MRArmor.eType.Helmet:
							SetItemPosition(item, mHelmetPosition);
							break;
						case MRArmor.eType.Shield:
							SetItemPosition(item, mShieldPosition);
							break;
						default:
							break;
					}
				}
			}

			// display the character's chits
			IList<MRActionChit> chits = character.Chits;
			for (int i = 0; i < chits.Count; ++i)
			{
				MRActionChit chit = chits[i];
				if (chit != null)
				{
					if (chit.Stack != null)
						chit.Stack.RemovePiece(chit);
					chit.Parent = mChitPositions[i].transform;
					chit.Layer = mChitPositions[i].layer;
					chit.Position = mChitPositions[i].transform.position;
					chit.LocalScale = new Vector3(1.3f, 1.3f, 1f);
				}
			}
		}
	}

	/// <summary>
	/// Creates a new game.
	/// </summary>
	private void CreateNewGame()
	{
		if (mState == OptionsState.NoGame)
		{
			mState = OptionsState.NewGame;
			mCurrentSaveGameSlot = -1;
			mCurrentSaveGameName = "";

			// create the map
			MRGame.TheGame.TheMap.CreateMap();

			// create all the characters
			foreach (MRGame.eCharacters characterId in Enum.GetValues(typeof(MRGame.eCharacters)))
			{
				MRCharacter character = MRGame.TheGame.CharacterManager.CreateCharacter(characterId);
				character.Parent = MRGame.TheGame.transform;
				mAvailableCharacters.Add(character);
				mAvailableCharactersStack.AddPieceToBottom(character);
			}

			mDisplayedCharacterIndex = 0;
		}
	}

	private void ChangeDisplayedCharacter(int newCharacterIndex)
	{
		if (newCharacterIndex < 0)
			newCharacterIndex = mAvailableCharacters.Count - 1;
		else if (newCharacterIndex >= mAvailableCharacters.Count)
			newCharacterIndex = 0;
		if (mAvailableCharacters.Count > 0 && newCharacterIndex >= 0 && newCharacterIndex < mAvailableCharacters.Count && newCharacterIndex != mDisplayedCharacterIndex)
		{
			// hide the current character display
			if (mDisplayedCharacterIndex >= 0 && mDisplayedCharacterIndex < mAvailableCharacters.Count)
			{
				MRCharacter character = mAvailableCharacters[mDisplayedCharacterIndex];

				// hide items
				IList<MRItem> activeItems = character.ActiveItems;
				foreach (MRItem item in activeItems)
				{
					if (item is MRWeapon)
					{
						ClearItemPosition(item);
					}
					else if (item is MRArmor)
					{
						MRArmor armor = (MRArmor)item;
						switch (armor.Type)
						{
							case MRArmor.eType.Breastplate:
							case MRArmor.eType.Full:
							case MRArmor.eType.Helmet:
							case MRArmor.eType.Shield:
								ClearItemPosition(item);
								break;
							default:
								break;
						}
					}
				}

				// hide chits
				IList<MRActionChit> chits = character.Chits;
				for (int i = 0; i < chits.Count; ++i)
				{
					MRActionChit chit = chits[i];
					if (chit != null)
					{
						chit.Layer = LayerMask.NameToLayer("Dummy");
					}
				}
			}
			mDisplayedCharacterIndex = newCharacterIndex;
		}
	}

	private void AddCharacter(int characterIndex)
	{
		if (mAvailableCharacters.Count > 0 && characterIndex >= 0 && characterIndex < mAvailableCharacters.Count)
		{
			MRCharacter character = mAvailableCharacters[characterIndex];
			if (!mSelectedCharacters.Contains(character))
			{
				mSelectedCharacters.Add(character);
				mSelectedCharactersStack.AddPieceToBottom(character);
			}
		}
	}

	private void RemoveCharacter(int characterIndex)
	{
		if (mAvailableCharacters.Count > 0 && characterIndex >= 0 && characterIndex < mAvailableCharacters.Count)
		{
			MRCharacter character = mAvailableCharacters[characterIndex];
			if (mSelectedCharacters.Contains(character))
			{
				mSelectedCharacters.Remove(character);
				mAvailableCharactersStack.AddPieceToBottom(character);
			}
		}
	}

	private void StartGame()
	{
		if (mSelectedCharacters.Count == 0 || mState != OptionsState.NewGame)
			return;

		mState = OptionsState.SelectStartingLocations;

		// delete any unselected characters
		foreach (MRCharacter character in mAvailableCharacters)
		{
			if (!mSelectedCharacters.Contains(character))
			{
				character.Destroy();
			}
		}
		mAvailableCharacters.Clear();
	}

	/// <summary>
	/// Exits the character selection screen and goes back to the main menu.
	/// </summary>
	private void ExitNewGame()
	{
		if (mSelectedCharacters.Count > 0)
			return;

		mState = OptionsState.NoGame;

		// delete any unselected characters
		foreach (MRCharacter character in mAvailableCharacters)
		{
			if (!mSelectedCharacters.Contains(character))
			{
				character.Destroy();
			}
		}
		mAvailableCharacters.Clear();
	}

	private void SelectStartingLocations()
	{
		if (MRGame.ShowingUI)
			return;

		// set the starting location for each character
		// characters that only have one start will be put there automatically, others will be asked where they want to start
		foreach (MRCharacter character in mSelectedCharacters)
		{
			if (character.StartingLocationIndex < 0)
			{
				if (character.StartingLocations.Length == 1)
				{
					character.StartingLocationIndex = 0;
				}
				else if (character.StartingLocations.Length > 1)
				{
					MRMainUI.TheUI.DisplaySelectionDialog("Starting Location", character.Name, character.StartingLocations, 
						delegate (int selected) {
							if (selected >= 0 && selected < character.StartingLocations.Length)
							{
								character.StartingLocationIndex = selected;
							}
					    });
				}
				return;
			}
			else if (!character.Score.VictoryPointsValid)
			{
				MRMainUI.TheUI.DisplayVictoryPointsSelectionDialog(character);
				return;
			}
		}

		// set up the map chits and place the characters
		MRGame.TheGame.TheMap.PlaceMapChits();
		foreach (MRCharacter character in mSelectedCharacters)
		{
			if (character.StartingLocation == null)
			{
				MRClearing clearing = null;
				if (character.StartingLocationIndex >= 0 && character.StartingLocationIndex < character.StartingLocations.Length)
					clearing = MRGame.TheGame.TheMap.ClearingForDwelling(character.StartingLocations[character.StartingLocationIndex].Dwelling());
				if (clearing != null)
				{
					character.StartingLocation = clearing;
				}
				else
				{
					Debug.LogError("No starting location for character " + character.Name);
					character.StartingLocation = MRGame.TheGame.TheMap.ClearingForDwelling(MRDwelling.eDwelling.Inn);
				}
			}
		}

		// all starting locations should be assigned, add characters to map
		foreach (MRCharacter character in mSelectedCharacters)
		{
			// there's a weird bug where the character piece isn't scaled right if you set the location once,
			// but setting it twice fixes it. The character orientation is also skewed.
			character.Location = character.StartingLocation;
			character.Location = character.StartingLocation;
			character.Hidden = true;
			MRGame.TheGame.AddCharacter(character);
		}
		mSelectedCharacters.Clear();

		// start the game proper 
		mState = OptionsState.GameStarted;
		MRGame.TheGame.StartGame();
	}

	private void SetItemPosition(MRItem item, GameObject position)
	{
		if (item.Stack != null)
			item.Stack.RemovePiece(item);
		item.Parent = position.transform;
		item.Layer = position.layer;
		item.Position = position.transform.position;
		item.LocalScale = new Vector3(1.3f, 1.3f, 1f);
	}

	private void ClearItemPosition(MRItem item)
	{
		item.LocalScale = new Vector3(1f, 1f, 1f);
		item.Parent = null;
		item.Layer = LayerMask.NameToLayer("Dummy");
		item.Position = Vector3.zero;
		item.LocalScale = Vector3.one;
	}

	public virtual bool OnTouched(GameObject touchedObject)
	{
		return true;
	}

	public bool OnReleased(GameObject touchedObject)
	{
		return true;
	}

	public virtual bool OnSingleTapped(GameObject touchedObject)
	{
		if (touchedObject == mNewGameButton.gameObject && mNewGameButton.Visible)
		{
			StartCoroutine(NewGame());
		}
		else if (touchedObject == mAddCharacterButton.gameObject && mAddCharacterButton.Visible)
		{
			AddCharacter(mDisplayedCharacterIndex);
		}
		else if (touchedObject == mRemoveCharacterButton.gameObject && mRemoveCharacterButton.Visible)
		{
			RemoveCharacter(mDisplayedCharacterIndex);
		}
		else if (touchedObject == mStartGameButton.gameObject && mStartGameButton.Visible)
		{
			StartGame();
		}
		else if (touchedObject == mBackButton.gameObject && mBackButton.Visible)
		{
			ExitNewGame();
		}
		else if (touchedObject == mLoadGameButton.gameObject && mLoadGameButton.Visible)
		{
			MRMainUI.TheUI.DisplayLoadGameSelectDialog();
		}
		else if (touchedObject == mSaveGameButton.gameObject && mSaveGameButton.Visible)
		{			
			SaveGame();
		}
		else if (touchedObject == mInstructionsButton.gameObject && mInstructionsButton.Visible)
		{
			ShowInstructions();
		}
		else if (touchedObject == mCreditsButton.gameObject && mCreditsButton.Visible)
		{
			ShowCredits();
		}
		else if (touchedObject == mCharacterLeftArrow.gameObject)
		{
			ChangeDisplayedCharacter(mDisplayedCharacterIndex - 1);
		}
		else if (touchedObject == mCharacterRightArrow.gameObject)
		{
			ChangeDisplayedCharacter(mDisplayedCharacterIndex + 1);
		}
		return true;
	}
	
	public virtual bool OnDoubleTapped(GameObject touchedObject)
	{
		return true;
	}

	public virtual bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	public bool OnPinchZoom(GameObject touchedObject, float pinchDelta)
	{
		return true;
	}

	public IEnumerator LoadGame()
	{
		if (mState == OptionsState.GameStarted)
		{
			yield return StartCoroutine(MRGame.TheGame.Reset());
			mState = OptionsState.NoGame;
		}

		try
		{
			string path = Application.persistentDataPath;
			String filename = Path.Combine(path, "game_" + CurrentSaveGameSlot + ".json");
			if (!File.Exists(filename))
			{
				// try the old save game name
				filename = Path.Combine(path, "game.json");
				mCurrentSaveGameSlot = -1;
				mCurrentSaveGameName = "";
			}
			if (File.Exists(filename))
			{
				StringBuilder dataBuffer = new StringBuilder(File.ReadAllText(filename));
				JSONObject gameData = new JSONObject(dataBuffer);
				MRGame.TheGame.Load(gameData);
			}
			else
				yield break;
		}
		catch (Exception err)
		{
			Debug.LogError("Error loading game: " + err);
			yield break;
		}

		// start the game proper 
		mState = OptionsState.GameStarted;
		MRGame.TheGame.StartGame();
	}

	public void SaveGame()
	{
		if (CurrentSaveGameSlot < 0)
		{
			// request a save game slot for the game
			MRMainUI.TheUI.DisplaySaveGameSelectDialog();
			return;
		}

		// todo: save game name/save slot
		JSONObject gameData = new JSONObject();
		MRGame.TheGame.Save(gameData);
		StringBuilder dataBuffer = new StringBuilder();
		gameData.Encode(dataBuffer);

		try
		{
			string path = Application.persistentDataPath;
			File.WriteAllText(Path.Combine(path, "game_" + CurrentSaveGameSlot + ".json"), dataBuffer.ToString());
			MRMainUI.TheUI.DisplayMessageDialog("Game Saved");
		}
		catch (Exception err)
		{
			Debug.LogError("Error saving game: " + err);
		}
	}

	/// <summary>
	/// Starts a new game. Run as a coroutine so we can clear out the current game before creating a new one.
	/// </summary>
	/// <returns>yield enumerator</returns>
	private IEnumerator NewGame()
	{
		if (mState == OptionsState.GameStarted)
		{
			yield return StartCoroutine(MRGame.TheGame.Reset());
			mState = OptionsState.NoGame;
		}
		CreateNewGame();
	}

	private void ShowInstructions()
	{
		MRMainUI.TheUI.DisplayInstructionsDialog();
	}

	private void ShowCredits()
	{
		MRMainUI.TheUI.DisplayCreditsDialog();
	}

	#endregion

	#region Members

	private Camera mCamera;
	private GameObject mStartScreen;
	private GameObject mSelectCharacter;
	private MRButton mNewGameButton;
	private MRButton mLoadGameButton;
	private MRButton mSaveGameButton;
	private MRButton mInstructionsButton;
	private MRButton mCreditsButton;
	private MRButton mAddCharacterButton;
	private MRButton mRemoveCharacterButton;
	private MRButton mStartGameButton;
	private MRButton mBackButton;
	private GameObject mCharacterDisplay;
	private GameObject mWeaponPosition;
	private GameObject mArmorPosition;
	private GameObject mHelmetPosition;
	private GameObject mShieldPosition;
	private GameObject[] mChitPositions = new GameObject[12];
	private TextMesh mRandomSeed;
	private TextMesh mCharacterName;
	private TextMesh mCharacterWeight;
	private TextMesh mCharacterStartLocation;
	private TextMesh[] mCharacterAbilities = new TextMesh[2];
	private Sprite mEnabledArrow;
	private Sprite mDisabledArrow;
	private MRButton mCharacterLeftArrow;
	private MRButton mCharacterRightArrow;
	private OptionsState mState;
	private int mDisplayedCharacterIndex;
	private IList<MRCharacter> mAvailableCharacters = new List<MRCharacter>();
	private IList<MRCharacter> mSelectedCharacters = new List<MRCharacter>();
	private MRGamePieceStack mAvailableCharactersStack;
	private MRGamePieceStack mSelectedCharactersStack;
	private int mCurrentSaveGameSlot;
	private string mCurrentSaveGameName;

	#endregion
}

