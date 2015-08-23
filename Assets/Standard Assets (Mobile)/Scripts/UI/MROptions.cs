//
// MROptions.cs
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

public class MROptions : MonoBehaviour
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
	
	public Camera CombatCamera
	{
		get{
			return mCamera;
		}
	}
	
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

	#endregion

	#region Methods

	// Use this for initialization
	void Start ()
	{
		mState = OptionsState.NoGame;

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
					mNewGameButton = obj;
					break;
				case "loadGame":
					mLoadGameButton = obj;
					break;
				case "saveGame":
					mSaveGameButton = obj;
					break;
				case "credits":
					mCreditsButton = obj;
					break;
				case "addCharacter":
					mAddCharacterButton = obj;
					break;
				case "removeCharacter":
					mRemoveCharacterButton = obj;
					break;
				case "startGame":
					mStartGameButton = obj;
					break;
				case "CharacterData":
					mCharacterDisplay = obj;
					break;
				case "nextCharacter":
					mCharacterRightArrow = obj;
					mEnabledArrow = ((SpriteRenderer)(mCharacterRightArrow.renderer)).sprite;
					break;
				case "prevCharacter":
					mCharacterLeftArrow = obj;
					mDisabledArrow = ((SpriteRenderer)(mCharacterLeftArrow.renderer)).sprite;
					break;
				case "Seed":
					mRandomSeed = obj.GetComponent<TextMesh>();;
					break;
				case "name":
					if (obj.transform.parent.name == "ownerName")
						mCharacterName = obj.GetComponent<TextMesh>();
					break;
				default:
					if (obj.name.StartsWith("chit"))
						mChitPositions[int.Parse(obj.name.Substring("chit".Length)) - 1] = obj;
					break;
			}
		}
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
				MRUtility.SetObjectVisibility(mSaveGameButton, false);
				break;
			case OptionsState.NewGame:
				MRUtility.SetObjectVisibility(mStartScreen, false);
				MRUtility.SetObjectVisibility(mSelectCharacter, true);
				MRUtility.SetObjectVisibility(mAddCharacterButton, mAvailableCharacters.Count > 0);
				MRUtility.SetObjectVisibility(mRemoveCharacterButton, true);
				MRUtility.SetObjectVisibility(mStartGameButton, mSelectedCharacters.Count > 0);
				break;
			case OptionsState.SelectStartingLocations:
				MRUtility.SetObjectVisibility(mStartScreen, false);
				MRUtility.SetObjectVisibility(mSelectCharacter, false);
				SelectStartingLocations();
				break;
			case OptionsState.GameStarted:
				MRUtility.SetObjectVisibility(mStartScreen, true);
				MRUtility.SetObjectVisibility(mSelectCharacter, false);
				MRUtility.SetObjectVisibility(mNewGameButton, false);
				MRUtility.SetObjectVisibility(mLoadGameButton, false);
				break;
		}

		if (mDisplayedCharacterIndex >= 0 && mAvailableCharacters.Count > 0)
		{
			// display the next/prev character arrows
			if (mDisplayedCharacterIndex == 0)
			{
				((SpriteRenderer)(mCharacterLeftArrow.renderer)).sprite = mDisabledArrow;
				((SpriteRenderer)(mCharacterRightArrow.renderer)).sprite = mEnabledArrow;
			}
			else if (mDisplayedCharacterIndex == mAvailableCharacters.Count - 1)
			{
				((SpriteRenderer)(mCharacterLeftArrow.renderer)).sprite = mEnabledArrow;
				((SpriteRenderer)(mCharacterRightArrow.renderer)).sprite = mDisabledArrow;
			}
			else
			{
				((SpriteRenderer)(mCharacterLeftArrow.renderer)).sprite = mEnabledArrow;
				((SpriteRenderer)(mCharacterRightArrow.renderer)).sprite = mEnabledArrow;
			}

			// display the character info
			MRCharacter character = mAvailableCharacters[mDisplayedCharacterIndex];
			mCharacterName.text = MRUtility.DisplayName(character.Name);

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

		// test user interaction
		if (MRGame.IsSingleTapped)
		{
			Vector3 worldTouch = mCamera.ScreenToWorldPoint(new Vector3(MRGame.LastTouchPos.x, MRGame.LastTouchPos.y, mCamera.nearClipPlane));
			RaycastHit2D[] hits = Physics2D.RaycastAll(worldTouch, Vector2.zero);
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.gameObject == mNewGameButton && mNewGameButton.renderer.enabled)
				{
					CreateNewGame();
					return;
				}
				else if (hit.collider.gameObject == mAddCharacterButton && mAddCharacterButton.renderer.enabled)
				{
					AddCharacter(mDisplayedCharacterIndex);
					return;
				}
				else if (hit.collider.gameObject == mRemoveCharacterButton && mRemoveCharacterButton.renderer.enabled)
				{
					RemoveCharacter(mDisplayedCharacterIndex);
					return;
				}
				else if (hit.collider.gameObject == mStartGameButton && mStartGameButton.renderer.enabled)
				{
					StartGame();
					return;
				}
				else if (hit.collider.gameObject == mLoadGameButton && mLoadGameButton.renderer.enabled)
				{
					LoadGame();
					return;
				}
				else if (hit.collider.gameObject == mSaveGameButton && mSaveGameButton.renderer.enabled)
				{
					SaveGame();
					return;
				}
				else if (hit.collider.gameObject == mCharacterLeftArrow && mCharacterLeftArrow.renderer.enabled && mDisplayedCharacterIndex > 0)
				{
					ChangeDisplayedCharacter(mDisplayedCharacterIndex - 1);
					return;
				}
				else if (hit.collider.gameObject == mCharacterRightArrow && mCharacterRightArrow.renderer.enabled && mDisplayedCharacterIndex < mAvailableCharacters.Count - 1)
				{
					ChangeDisplayedCharacter(mDisplayedCharacterIndex + 1);
					return;
				}
			}
		}
	}

	private void CreateNewGame()
	{
		if (mState == OptionsState.NoGame)
		{
			mState = OptionsState.NewGame;

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
		if (mAvailableCharacters.Count > 0 && newCharacterIndex >= 0 && newCharacterIndex < mAvailableCharacters.Count && newCharacterIndex != mDisplayedCharacterIndex)
		{
			// hide the current character display
			if (mDisplayedCharacterIndex >= 0 && mDisplayedCharacterIndex < mAvailableCharacters.Count)
			{
				MRCharacter character = mAvailableCharacters[mDisplayedCharacterIndex];
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

	private void SelectStartingLocations()
	{
		if (MRGame.ShowingUI)
			return;

		// set the starting location for each character
		// characters that only have one start will be put there automatically, others will be asked where they want to start
		foreach (MRCharacter character in mSelectedCharacters)
		{
			if (character.StartingLocation == null)
			{
				MRClearing clearing = null;
				if (character.StartingLocations.Length == 1)
				{
					clearing = MRGame.TheGame.TheMap.ClearingForDwelling(character.StartingLocations[0].Dwelling());
				}
				else if (character.StartingLocations.Length > 1)
				{
					MRMainUI.TheUI.DisplaySelectionDialog("Starting Location", character.Name, character.StartingLocations, 
						delegate (int selected) {
							if (selected >= 0 && selected < character.StartingLocations.Length)
							{
								clearing = MRGame.TheGame.TheMap.ClearingForDwelling(character.StartingLocations[selected].Dwelling());
								if (clearing != null)
								{
									character.StartingLocation = clearing;
								}
							}
					    });
					return;
				}
				else
				{
					Debug.LogError("No starting location for character " + character.Name);
					clearing = MRGame.TheGame.TheMap.ClearingForDwelling(MRDwelling.eDwelling.Inn);
				}
				if (clearing != null)
				{
					character.StartingLocation = clearing;
					return;
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
		MRGame.TheGame.SetView(MRGame.eViews.Map);
		MRGame.TheGame.AddUpdateEvent(new MRFatigueCharacterEvent());
		MRGame.TheGame.AddUpdateEvent(new MRInitGameTimeEvent());
		mState = OptionsState.GameStarted;
	}

	private void LoadGame()
	{
		// todo: save game name/save slot

		try
		{
			string path = Application.persistentDataPath;
			if (File.Exists(Path.Combine(path, "game.json")))
			{
				StringBuilder dataBuffer = new StringBuilder(File.ReadAllText(Path.Combine(path, "game.json")));
				JSONObject gameData = new JSONObject(dataBuffer);
				MRGame.TheGame.Load(gameData);
			}
			else
				return;
		}
		catch (Exception err)
		{
			Debug.LogError("Error loading game: " + err);
			return;
		}

		// start the game proper 
		MRGame.TheGame.SetView(MRGame.eViews.Map);
		MRGame.TheGame.AddUpdateEvent(new MRFatigueCharacterEvent());
		MRGame.TheGame.AddUpdateEvent(new MRInitGameTimeEvent());
		mState = OptionsState.GameStarted;
	}

	private void SaveGame()
	{
		// todo: save game name/save slot

		JSONObject gameData = new JSONObject();
		MRGame.TheGame.Save(gameData);
		StringBuilder dataBuffer = new StringBuilder();
		gameData.Encode(dataBuffer);

		try
		{
			string path = Application.persistentDataPath;
			File.WriteAllText(Path.Combine(path, "game.json"), dataBuffer.ToString());
			MRMainUI.TheUI.DisplayMessageDialog("Game Saved");
		}
		catch (Exception err)
		{
			Debug.LogError("Error saving game: " + err);
		}
	}

	#endregion

	#region Members

	private Camera mCamera;
	private GameObject mStartScreen;
	private GameObject mSelectCharacter;
	private GameObject mNewGameButton;
	private GameObject mLoadGameButton;
	private GameObject mSaveGameButton;
	private GameObject mCreditsButton;
	private GameObject mAddCharacterButton;
	private GameObject mRemoveCharacterButton;
	private GameObject mStartGameButton;
	private GameObject mCharacterDisplay;
	private GameObject[] mChitPositions = new GameObject[12];
	private TextMesh mRandomSeed;
	private TextMesh mCharacterName;
	private Sprite mEnabledArrow;
	private Sprite mDisabledArrow;
	private GameObject mCharacterLeftArrow;
	private GameObject mCharacterRightArrow;
	private OptionsState mState;
	private int mDisplayedCharacterIndex;
	private IList<MRCharacter> mAvailableCharacters = new List<MRCharacter>();
	private IList<MRCharacter> mSelectedCharacters = new List<MRCharacter>();
	private MRGamePieceStack mAvailableCharactersStack;
	private MRGamePieceStack mSelectedCharactersStack;

	#endregion
}

