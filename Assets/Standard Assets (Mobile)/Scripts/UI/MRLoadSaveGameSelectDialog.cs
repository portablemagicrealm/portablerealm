//
// MRLoadSaveGameSelectDialog.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2016 Steve Jakab
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
using System.IO;
using System.Text;
using AssemblyCSharp;

public class MRLoadSaveGameSelectDialog : MonoBehaviour
{
	#region Constants

	public enum Mode
	{
		Load,
		Save
	}

	#endregion

	#region Properties

	public Text Title;
	public Toggle Slot1;
	public Toggle Slot2;
	public Toggle Slot3;
	public Toggle Slot4;
	public Toggle Slot5;
	public Text Slot1Text;
	public Text Slot2Text;
	public Text Slot3Text;
	public Text Slot4Text;
	public Text Slot5Text;
	public InputField GameNameInput;
	public Button Ok;
	public Button Cancel;

	public MRMainUI.OnButtonPressed Callback
	{
		get{
			return mCallback;
		}

		set{
			mCallback = value;
		}
	}

	public Mode DialogMode
	{
		get{
			return mMode;
		}

		set{
			mMode = value;
			if (mMode == Mode.Load)
			{
				Title.text = "Load Game";
				GameNameInput.gameObject.SetActive(false);
			}
			else if (mMode == Mode.Save)
			{
				Title.text = "Save Game";
				GameNameInput.gameObject.SetActive(true);
			}
		}
	}

	public int Selected
	{
		get{
			for (int i = 0; i < mSelections.Length; ++i)
			{
				if (mSelections[i].isOn)
					return i;
			}
			return -1;
		}

		set{
			foreach (Toggle selection in mSelections)
			{
				selection.isOn = false;
			}
			if (value >= 0 &&  value < mSelections.Length)
			{
				mSelections[value].isOn = true;
			}
		}
	}

	public String SelectedGameName
	{
		get{
			int selected = Selected;
			if (selected >= 0)
				return mSelectionNames[selected].text;
			return "";
		}
	}

	#endregion

	#region Methods

	public MRLoadSaveGameSelectDialog ()
	{
		
	}

	// Called when the script instance is being loaded
	void Awake()
	{
		mMode = Mode.Load;
		mSelections[0] = Slot1;
		mSelections[1] = Slot2;
		mSelections[2] = Slot3;
		mSelections[3] = Slot4;
		mSelections[4] = Slot5;
		mSelectionNames[0] = Slot1Text;
		mSelectionNames[1] = Slot2Text;
		mSelectionNames[2] = Slot3Text;
		mSelectionNames[3] = Slot4Text;
		mSelectionNames[4] = Slot5Text;
	}

	// Use this for initialization
	void Start ()
	{
		Ok.onClick.AddListener(OnOkClicked);
		Cancel.onClick.AddListener(OnCancelClicked);
		GameNameInput.onValidateInput = FilterGameName;
		Selected = 0;

		if (msGameNames == null)
		{
			// get the list of games
			msGameNames = new string[mSelectionNames.Length];
			string path = Application.persistentDataPath;
			for (int i = 0; i < mSelections.Length; ++i)
			{
				String filename = Path.Combine(path, "game_" + i + ".json");
				if (File.Exists(filename))
				{
					StringBuilder dataBuffer = new StringBuilder(File.ReadAllText(filename));
					JSONObject root = new JSONObject(dataBuffer);
					if (root["gameName"] != null)
					{
						JSONString gameName = (JSONString)root["gameName"];
						msGameNames[i] = gameName.Value;
					}
				}
				else
				{
					msGameNames[i] = "";
				}
			}
		}
		for (int i = 0; i < mSelections.Length; ++i)
		{
			mSelectionNames[i].text = msGameNames[i];
		}
	}

	// Update is called once per frame
	void Update ()
	{
	}

	/// <summary>
	/// Called when the selected game slot changes.
	/// </summary>
	public void OnSelectionChanged()
	{
		if (mMode == Mode.Load)
		{
		}
		else if (mMode == Mode.Save)
		{
			if (Selected >= 0 && Selected < 5)
			{
				// move the game name input field to the selected slot
				Vector3 currentPos = GameNameInput.transform.position;
				GameNameInput.transform.position = new Vector3(currentPos.x, mSelections[Selected].transform.position.y, currentPos.z);
			}
		}
	}

	/// <summary>
	/// Called when the user stops editing the game name.
	/// </summary>
	public void OnGameNameChanged()
	{
		int selected = Selected;
		if (selected >= 0 && !String.IsNullOrEmpty(GameNameInput.text))
		{
		}
	}

	private void OnOkClicked()
	{
		if (mCallback != null && Selected >= 0)
		{
			if (mMode == Mode.Load && !String.IsNullOrEmpty(SelectedGameName))
			{
				mCallback(0);
			}
			else if (mMode == Mode.Save && !String.IsNullOrEmpty(GameNameInput.text))
			{
				mSelectionNames[Selected].text = GameNameInput.text;
				msGameNames[Selected] = SelectedGameName;
				mCallback(0);
			}
		}
	}

	private void OnCancelClicked()
	{
		if (mCallback != null)
		{
			Selected = -1;
			mCallback(1);
		}
	}

	/// <summary>
	/// Prevents invalid characters from being used in a game name.
	/// </summary>
	/// <returns>The character to be used in the name</returns>
	/// <param name="text">Text.</param>
	/// <param name="charIndex">Char index.</param>
	/// <param name="addedChar">Added char.</param>
	private char FilterGameName(string text, int charIndex, char addedChar)
	{
		if (Char.IsLetterOrDigit(addedChar) || addedChar == ' ' || addedChar == '_')
			return addedChar;
		return '\0';
	}

	#endregion

	#region Members

	private MRMainUI.OnButtonPressed mCallback = null;
	private Toggle[] mSelections = new Toggle[5];
	private Text[] mSelectionNames = new Text[5];
	private Mode mMode;
	private static string[] msGameNames = null;

	#endregion
}
