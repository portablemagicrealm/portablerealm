//
// MRMainUI.cs
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
using UnityEngine.UI;
using System;
using System.Collections;

namespace PortableRealm
{
	
public class MRMainUI : MonoBehaviour
{
	#region Callback class for dialog buttons
	public delegate void OnButtonPressed(int buttonId);

	private class ButtonClickedAction
	{
		public ButtonClickedAction(OnButtonPressed callback, int buttonId)
		{
			mCallback = callback;
			mId = buttonId;
		}

		public void OnClicked()
		{
			MRGame.ShowingUI = false;
			Destroy (TheUI.mSelectionDialog);
			
			TheUI.mSelectionDialog = null;
			TheUI.mDialogButtons = null;
			TheUI.mDialogCallbacks = null;

			mCallback(mId);
		}

		private int mId;
		private OnButtonPressed mCallback;
	}

	#endregion

	#region Constants

	public enum eCombatActionButton
	{
		None,
		FlipWeapon,
		RunAway,
		CastSpell,
		ActivateItem,
		AbandonItems
	}

	private const float TIMED_MESSAGE_BOX_DISPLAY_TIME = 2.0f;

	#endregion

	#region Properties

	public GameObject SelectionDialogPrototype;
	public GameObject MessageDialogPrototype;
	public GameObject YesNoDialogPrototype;
	public GameObject TimedMessagePrototype;
	public GameObject InstructionMessagePrototype;
	public GameObject AttackManeuverDialogPrototype;
	public GameObject CombatActionDialogPrototype;
	public GameObject VictoryPointsSelectionDialogPrototype;
	public GameObject SaveLoadSelectDialogPrototype;
	public GameObject GameTypeSelectDialogPrototype;
	public GameObject InstructionsDialogPrototype;
	public GameObject CreditsDialogPrototype;

	public static MRMainUI TheUI
	{
		get{
			return msTheUI;
		}
	}

	#endregion
	
	#region Methods

	void Awake()
	{
		msTheUI = this;
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		// check if a timed message has timed out
		if (mTimedMessageBox != null && Time.time - mTimedMessageBoxStartTime >= TIMED_MESSAGE_BOX_DISPLAY_TIME)
		{
			Destroy (mTimedMessageBox);
			mTimedMessageBox = null;
		}
	}

	/// <summary>
	/// Displays a dialog getting the player choose an item from multiple options.
	/// </summary>
	/// <param name="title">Dialog title.</param>
	/// <param name="buttons">List of options.</param>
	/// <param name="callback">Class that will be called when a button is pressed.</param>
	public void DisplaySelectionDialog(string title, string subtitle, string[] buttons, OnButtonPressed callback)
	{
		mSelectionDialog = (GameObject)Instantiate(SelectionDialogPrototype);

		// set the title
		foreach (Text text in mSelectionDialog.GetComponentsInChildren<Text>())
		{
			if (text.gameObject.name == "Title")
			{
				text.text = title;
				break;
			}
		}

		// set the subtitle
		foreach (Text text in mSelectionDialog.GetComponentsInChildren<Text>())
		{
			if (text.gameObject.name == "Subtitle")
			{
				if (subtitle != null)
					text.text = subtitle;
				else
					text.enabled = false;
				break;
			}
		}

		// set up the buttons
		Button buttonProto = mSelectionDialog.GetComponentInChildren<Button>();
		mDialogButtons = new Button[buttons.Length];
		mDialogCallbacks = new ButtonClickedAction[buttons.Length];
		mDialogButtons[0] = buttonProto;
		mDialogCallbacks[0] = new ButtonClickedAction(callback, 0);
		buttonProto.onClick.AddListener(mDialogCallbacks[0].OnClicked);
		buttonProto.GetComponentInChildren<Text>().text = buttons[0];
		RectTransform prevButtonTransform = buttonProto.GetComponentInChildren<RectTransform>();
		float buttonHeight = prevButtonTransform.sizeDelta.y;
		for (int i = 1; i < buttons.Length; ++i)
		{
			mDialogButtons[i] = (Button)Instantiate(buttonProto);
			mDialogCallbacks[i] = new ButtonClickedAction(callback, i);
			mDialogButtons[i].onClick.AddListener(mDialogCallbacks[i].OnClicked);
			mDialogButtons[i].transform.SetParent(buttonProto.transform.parent, false);
			mDialogButtons[i].GetComponentInChildren<Text>().text = buttons[i];
			RectTransform buttonTransform = mDialogButtons[i].GetComponentInChildren<RectTransform>();
			buttonTransform.anchoredPosition = new Vector2(prevButtonTransform.anchoredPosition.x, prevButtonTransform.anchoredPosition.y - buttonHeight);
			prevButtonTransform = buttonTransform;
		}

		// adjust the dialog size for the number of buttons
		float minHeight = 197;
		float bottomOffset = 0;
		foreach (RectTransform background in mSelectionDialog.GetComponentsInChildren<RectTransform>())
		{
			if (background.gameObject.name == "Main Background")
			{
				bottomOffset = minHeight - buttons.Length * buttonHeight;
				if (bottomOffset < 0)
					bottomOffset = 0;
				background.offsetMin = new Vector2(0, bottomOffset);
				break;
			}
		}

		// center the dialog
		RectTransform dialogTransform = (RectTransform)mSelectionDialog.transform;
		dialogTransform.Translate(new Vector3(0, -bottomOffset, 0));

		mSelectionDialog.transform.SetParent(transform, false);

		MRGame.ShowingUI = true;
	}

	/// <summary>
	/// Displays a simple message dialog with no title.
	/// </summary>
	/// <param name="message">The message.</param>
	public void DisplayMessageDialog(string message)
	{
		DisplayMessageDialog(message, null);
	}

	/// <summary>
	/// Displays a simple message dialog.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="title">Title of the message dialog.</param>
	public void DisplayMessageDialog(string message, string title)
	{
		DisplayMessageDialog(message, title, null);
	}

	/// <summary>
	/// Displays a simple message dialog.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="title">Title of the message dialog.</param>
	/// <param name="callback">Class that will be called when ok is pressed.</param>
	public void DisplayMessageDialog(string message, string title, OnButtonPressed callback)
	{
		mMessageDialog = (GameObject)Instantiate(MessageDialogPrototype);

		// set the title and message
		if (title == null)
			title = "";
		foreach (Text text in mMessageDialog.GetComponentsInChildren<Text>())
		{
			if (text.gameObject.name == "Title")
			{
				text.text = title;
			}
			else if (text.gameObject.name == "Message")
			{
				text.text = message;
			}
		}
		
		// set the ok button callback
		Button ok = mMessageDialog.GetComponentInChildren<Button>();
		ok.onClick.AddListener(OnOkClicked);
		mOkCallback = callback;
		
		mMessageDialog.transform.SetParent(transform, false);
		MRGame.ShowingUI = true;
	}

	/// <summary>
	/// Displays a simple message dialog with an ok/cancel response.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="title">Title of the message dialog.</param>
	/// <param name="okCallback">Class that will be called when "ok" is pressed.</param>
	/// <param name="cancelCallback">Class that will be called when "cancel" is pressed.</param>
	public void DisplayOkCancelDialog(string message, string title, OnButtonPressed okCallback, OnButtonPressed cancelCallback)
	{
		DisplayYesNoDialog(message, title, "Ok", "Cancel", okCallback, cancelCallback);
	}

	/// <summary>
	/// Displays a simple message dialog with a yes/no response.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="title">Title of the message dialog.</param>
	/// <param name="yesCallback">Class that will be called when "yes" is pressed.</param>
	/// <param name="noCallback">Class that will be called when "yes" is pressed.</param>
	public void DisplayYesNoDialog(string message, string title, OnButtonPressed yesCallback, OnButtonPressed noCallback)
	{
		DisplayYesNoDialog(message, title, "Yes", "No", yesCallback, noCallback);
	}

	/// <summary>
	/// Displays a simple message dialog with a yes/no response.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="title">Title of the message dialog.</param>
	/// <param name="yesText">Text to display on the "yes" button.</param>
	/// <param name="noText">Text to display on the "no" button.</param>
	/// <param name="yesCallback">Class that will be called when "yes" is pressed.</param>
	/// <param name="noCallback">Class that will be called when "yes" is pressed.</param>
	public void DisplayYesNoDialog(string message, string title, string yesText, string noText, OnButtonPressed yesCallback, OnButtonPressed noCallback)
	{
		mYesNoDialog = (GameObject)Instantiate(YesNoDialogPrototype);

		// set the title and message
		if (title == null)
			title = "";
		foreach (Text text in mYesNoDialog.GetComponentsInChildren<Text>())
		{
			if (text.gameObject.name == "Title")
			{
				text.text = title;
			}
			else if (text.gameObject.name == "Message")
			{
				text.text = message;
			}
			else if (text.gameObject.name == "YesText")
			{
				text.text = yesText;
			}
			else if (text.gameObject.name == "NoText")
			{
				text.text = noText;
			}
		}
		
		// set the ok button callback
		foreach (Button button in mYesNoDialog.GetComponentsInChildren<Button>())
		{
			if (button.name == "yesButton")
			{
				button.onClick.AddListener(OnYesClicked);
				mYesCallback = yesCallback;
			}
			else if (button.name == "noButton")
			{
				button.onClick.AddListener(OnNoClicked);
				mNoCallback = noCallback;
			}
		}
		
		mYesNoDialog.transform.SetParent(transform, false);
		MRGame.ShowingUI = true;
	}

	public void DisplayAttackManeuverDialog(float leftPositionViewport, float topPositionViewport)
	{
		if (mAttackManeuverDialog == null)
		{
			mAttackManeuverDialog = (GameObject)Instantiate(AttackManeuverDialogPrototype);
			Vector2 anchor = ((RectTransform)mAttackManeuverDialog.transform).anchorMin;
			anchor.x = leftPositionViewport;
			anchor.y = topPositionViewport;
			((RectTransform)mAttackManeuverDialog.transform).anchorMin = anchor;
			((RectTransform)mAttackManeuverDialog.transform).anchorMax = anchor;
			//Vector3 position = mAttackManeuverDialog.transform.localPosition;
			// note world x 0 is left screen, but ui 0 is the center

			//position.x = leftPosition - (Screen.width / 2.0f) + (((RectTransform)mAttackManeuverDialog.transform).rect.width * scale.x / 2.0f);
			//mAttackManeuverDialog.transform.localPosition = position;
			mAttackManeuverDialog.transform.SetParent(transform, false);

			// set button callbacks
			Button[] buttons = mAttackManeuverDialog.GetComponentsInChildren<Button>();
			foreach (Button button in buttons)
			{
				if (button.gameObject.name == "None")
					button.onClick.AddListener(OnAttackManeuverNoneClicked);
				else if (button.gameObject.name == "Cancel")
					button.onClick.AddListener(OnAttackManeuverCancelClicked);
			}
		}
	}

	public void HideAttackManeuverDialog()
	{
		if (mAttackManeuverDialog != null)
		{
			DestroyObject(mAttackManeuverDialog);
			mAttackManeuverDialog = null;
		}
	}

	/// <summary>
	/// Displays the results of a die pool roll at the bottom of the screen.
	/// </summary>
	/// <param name="pool">The die pool to display</param>
	public void DisplayDieRollResult(MRDiePool pool)
	{
		if (mTimedMessageBox != null)
			DestroyObject(mTimedMessageBox);
		mTimedMessageBox = (GameObject)Instantiate(TimedMessagePrototype);
		Text text = mTimedMessageBox.GetComponentInChildren<Text>();
		string message;
		if (pool.DieRolls.Length == 1)
			message = "Roll " + pool.DieRolls[0];
		else
			message = "Roll " + pool.DieRolls[0] + ", " + pool.DieRolls[1];
		message += " = " + pool.Roll;
		text.text = message;
		mTimedMessageBox.transform.SetParent(transform, false);
		mTimedMessageBoxStartTime = Time.time;
	}

	/// <summary>
	/// Displays the results of a die pool roll with a message at the bottom of the screen.
	/// </summary>
	/// <param name="message">Message to display with the roll</param>
	/// <param name="pool">The die pool to display</param>
	public void DisplayDieRollResult(string message, MRDiePool pool)
	{
		if (mTimedMessageBox != null)
			DestroyObject(mTimedMessageBox);
		mTimedMessageBox = (GameObject)Instantiate(TimedMessagePrototype);
		Text text = mTimedMessageBox.GetComponentInChildren<Text>();
		if (pool.DieRolls.Length == 1)
			message += " = " + pool.Roll;
		else
			message = " = (" + pool.DieRolls[0] + ", " + pool.DieRolls[1] + ") = " + pool.Roll;
		text.text = message;
		mTimedMessageBox.transform.SetParent(transform, false);
		mTimedMessageBoxStartTime = Time.time;
	}

	/// <summary>
	/// Displays an instructional message at the top of the screen. To stop showing the message, 
	/// call this function with null as the message.
	/// </summary>
	/// <param name="message">Message to display.</param>
	public void DisplayInstructionMessage(string message)
	{
		if (mInstructionMessage != null)
		{
			// check if we're showing the same message
			Text text = mInstructionMessage.GetComponentInChildren<Text>();
			if (message != null && message.Equals(text.text, StringComparison.Ordinal))
				return;
			DestroyObject(mInstructionMessage);
		}
		if (message != null)
		{
			mInstructionMessage = (GameObject)Instantiate(InstructionMessagePrototype);
			Text text = mInstructionMessage.GetComponentInChildren<Text>();
			text.text = message;
			mInstructionMessage.transform.SetParent(transform, false);
		}
	}

	/// <summary>
	/// Displays the combat action selection dialog.
	/// </summary>
	/// <param name="canActivateWeapon">Set to <c>true</c> to allow alert/unalert weapon.</param>
	/// <param name="canRunAway">Set to <c>true</c> to allow running away.</param>
	/// <param name="canCastSpell">Set to <c>true</c> to allow casting a spell.</param>
	public void DisplayCombatActionDialog(bool canActivateWeapon, bool canRunAway, bool canCastSpell, OnButtonPressed callback)
	{
		if (mCombatActionDialog == null)
		{
			mCombatActionDialog = (GameObject)Instantiate(CombatActionDialogPrototype);
			mCombatActionDialog.transform.SetParent(transform, false);
			
			// set button callbacks
			Button[] buttons = mCombatActionDialog.GetComponentsInChildren<Button>();
			foreach (Button button in buttons)
			{
				if (button.gameObject.name == "Weapon")
				{
					button.interactable = canActivateWeapon;
					button.onClick.AddListener(OnCombatActionWeaponClicked);
				}
				else if (button.gameObject.name == "Run")
				{
					button.interactable = canRunAway;
					button.onClick.AddListener(OnCombatActionRunClicked);
				}
				else if (button.gameObject.name == "Spell")
				{
					button.interactable = canCastSpell;
					button.onClick.AddListener(OnCombatActionSpellClicked);
				}
				else if (button.gameObject.name == "None")
					button.onClick.AddListener(OnCombatActionNoneClicked);
			}

			mOkCallback = callback;
		}
	}

	/// <summary>
	/// Displays the victory points selection dialog.
	/// </summary>
	/// <param name="character">Character to choose victory points for.</param>
	public void DisplayVictoryPointsSelectionDialog(MRCharacter character)
	{
		if (mVictoryPointsSelectionDialog == null && character != null)
		{
			mVictoryPointsSelectionDialog = (GameObject)Instantiate(VictoryPointsSelectionDialogPrototype);
			mVictoryPointsSelectionDialog.transform.SetParent(transform, false);

			MonoBehaviour[] scripts = mVictoryPointsSelectionDialog.GetComponents<MonoBehaviour>();
			for (int i = 0; i < scripts.Length; ++i)
			{
				if (scripts[i] is MRVictoryPointSelectionDialog)
				{
					MRVictoryPointSelectionDialog dialogScript = (MRVictoryPointSelectionDialog)scripts[i];
					dialogScript.Character = character;
					dialogScript.Callback = OnVictoryPointsSelected;
					MRGame.ShowingUI = true;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Displays the save/new game select dialog.
	/// </summary>
	public void DisplaySaveGameSelectDialog()
	{
		if (mLoadSaveGameSelectDialog == null)
		{
			mLoadSaveGameSelectDialog = (GameObject)Instantiate(SaveLoadSelectDialogPrototype);
			mLoadSaveGameSelectDialog.transform.SetParent(transform, false);
			MonoBehaviour[] scripts = mLoadSaveGameSelectDialog.GetComponents<MonoBehaviour>();
			for (int i = 0; i < scripts.Length; ++i)
			{
				if (scripts[i] is MRLoadSaveGameSelectDialog)
				{
					MRLoadSaveGameSelectDialog dialogScript = (MRLoadSaveGameSelectDialog)scripts[i];
					dialogScript.DialogMode = MRLoadSaveGameSelectDialog.Mode.Save;
					dialogScript.Callback = OnSaveSlotSelected;
					MRGame.ShowingUI = true;
					break;
				}
			}

			MRGame.ShowingUI = true;
		}
	}

	/// <summary>
	/// Displays the load game select dialog.
	/// </summary>
	public void DisplayLoadGameSelectDialog()
	{
		if (mLoadSaveGameSelectDialog == null)
		{
			mLoadSaveGameSelectDialog = (GameObject)Instantiate(SaveLoadSelectDialogPrototype);
			mLoadSaveGameSelectDialog.transform.SetParent(transform, false);
			MonoBehaviour[] scripts = mLoadSaveGameSelectDialog.GetComponents<MonoBehaviour>();
			for (int i = 0; i < scripts.Length; ++i)
			{
				if (scripts[i] is MRLoadSaveGameSelectDialog)
				{
					MRLoadSaveGameSelectDialog dialogScript = (MRLoadSaveGameSelectDialog)scripts[i];
					dialogScript.DialogMode = MRLoadSaveGameSelectDialog.Mode.Load;
					dialogScript.Callback = OnLoadSlotSelected;
					MRGame.ShowingUI = true;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Displaies the game type selection dialog.
	/// </summary>
	public void DisplayGameTypeSelectDialog()
	{
		if (mGameTypeSelectDialog == null)
		{
			mGameTypeSelectDialog = (GameObject)Instantiate(GameTypeSelectDialogPrototype);
			mGameTypeSelectDialog.transform.SetParent(transform, false);
			MonoBehaviour[] scripts = mGameTypeSelectDialog.GetComponents<MonoBehaviour>();
			for (int i = 0; i < scripts.Length; ++i)
			{
				if (scripts[i] is MRGameTypeSelectionDialog)
				{
					MRGameTypeSelectionDialog dialogScript = (MRGameTypeSelectionDialog)scripts[i];
					dialogScript.Callback = OnGameTypeSelected;
					MRGame.ShowingUI = true;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Displays the instructions dialog.
	/// </summary>
	public void DisplayInstructionsDialog()
	{
		mInstructionsDialog = (GameObject)Instantiate(InstructionsDialogPrototype);
		mInstructionsDialog.transform.SetParent(transform, false);

		// set button callbacks
		Button[] buttons = mInstructionsDialog.GetComponentsInChildren<Button>();
		foreach (Button button in buttons)
		{
			if (button.gameObject.name == "Back")
			{
				button.onClick.AddListener(OnInstructionsBackClicked);
			}
		}

		MRGame.ShowingUI = true;
	}

	/// <summary>
	/// Displays the credits dialog.
	/// </summary>
	public void DisplayCreditsDialog()
	{
		mCreditsDialog = (GameObject)Instantiate(CreditsDialogPrototype);
		mCreditsDialog.transform.SetParent(transform, false);

		// set button callbacks
		Button[] buttons = mCreditsDialog.GetComponentsInChildren<Button>();
		foreach (Button button in buttons)
		{
			if (button.gameObject.name == "Back")
			{
				button.onClick.AddListener(OnCreditsBackClicked);
			}
		}

		MRGame.ShowingUI = true;
	}

	/// <summary>
	/// Hides the combat action selection dialog.
	/// </summary>
	private void HideCombatActionDialog()
	{
		if (mCombatActionDialog != null)
		{
			DestroyObject(mCombatActionDialog);
			mCombatActionDialog = null;
		}
	}

	/// <summary>
	/// Called when the player clicks the "ok" button of the message dialog.
	/// </summary>
	private void OnOkClicked()
	{
		MRGame.ShowingUI = false;

		Destroy (mMessageDialog);
		mMessageDialog = null;

		if (mOkCallback != null)
			mOkCallback(0);
	}

	/// <summary>
	/// Called when the player clicks the "yes" button of the yesno dialog.
	/// </summary>
	private void OnYesClicked()
	{
		MRGame.ShowingUI = false;

		Destroy (mYesNoDialog);
		mYesNoDialog = null;

		if (mYesCallback != null)
			mYesCallback(0);
	}

	/// <summary>
	/// Called when the player clicks the "no" button of the yesno dialog.
	/// </summary>
	private void OnNoClicked()
	{
		MRGame.ShowingUI = false;

		Destroy (mYesNoDialog);
		mYesNoDialog = null;

		if (mNoCallback != null)
			mNoCallback(1);
	}

	/// <summary>
	/// Called when the player clicks the "none" button of the attack/maneuver dialog.
	/// </summary>
	private void OnAttackManeuverNoneClicked()
	{
		MRGame.TheGame.OnAttackManeuverSelectedGame(MRCombatManager.eAttackManeuverOption.None);
	}

	/// <summary>
	/// Called when the player clicks the "cancel" button of the attack/maneuver dialog.
	/// </summary>
	private void OnAttackManeuverCancelClicked()
	{
		MRGame.TheGame.OnAttackManeuverSelectedGame(MRCombatManager.eAttackManeuverOption.Cancel);
	}

	private void OnCombatActionWeaponClicked()
	{
		HideCombatActionDialog();
		if (mOkCallback != null)
			mOkCallback((int)eCombatActionButton.FlipWeapon);
	}

	private void OnCombatActionRunClicked()
	{
		HideCombatActionDialog();
		if (mOkCallback != null)
			mOkCallback((int)eCombatActionButton.RunAway);
	}

	private void OnCombatActionSpellClicked()
	{
		HideCombatActionDialog();
		if (mOkCallback != null)
			mOkCallback((int)eCombatActionButton.CastSpell);
	}

	private void OnCombatActionNoneClicked()
	{
		HideCombatActionDialog();
		if (mOkCallback != null)
			mOkCallback((int)eCombatActionButton.None);
	}

	private void OnVictoryPointsSelected(int buttonId)
	{
		MRGame.ShowingUI = false;
		
		Destroy(mVictoryPointsSelectionDialog);
		mVictoryPointsSelectionDialog = null;
	}

	/// <summary>
	/// Called when the player selected the game type for a new game.
	/// </summary>
	/// <param name="buttonId">Button identifier, 0 = ok.</param>
	private void OnGameTypeSelected(int buttonId)
	{
		MRGame.eGameType type = MRGame.eGameType.Default;
		int bolChapter = 1;

		MonoBehaviour[] scripts = mGameTypeSelectDialog.GetComponents<MonoBehaviour>();
		for (int i = 0; i < scripts.Length; ++i)
		{
			if (scripts[i] is MRGameTypeSelectionDialog)
			{
				MRGameTypeSelectionDialog dialogScript = (MRGameTypeSelectionDialog)scripts[i];
				type = dialogScript.GameType;
				bolChapter = dialogScript.BookOfLearningChapter;
				break;
			}
		}

		MRGame.ShowingUI = false;
		Destroy(mGameTypeSelectDialog);
		mGameTypeSelectDialog = null;

		switch (type)
		{
			case MRGame.eGameType.BookOfLearning:
			{
				string bolChapterName = "bol_";
				if (bolChapter < 10)
					bolChapterName += "0";
				bolChapterName += bolChapter.ToString();
				bolChapterName += "_world";
				MRGame.TheGame.Main.StartPregeneratedGame(bolChapterName);
				break;
			}
			case MRGame.eGameType.Default:
			default:
				MRGame.TheGame.Main.StartNewGame();
				break;
		}
	}

	/// <summary>
	/// Called when the player selected which game to load.
	/// </summary>
	/// <param name="buttonId">Button identifier, 0 = ok.</param>
	private void OnLoadSlotSelected(int buttonId)
	{
		int selected = -1;
		String selectedName = "";
		if (buttonId == 0)
		{
			// ok selected
			MonoBehaviour[] scripts = mLoadSaveGameSelectDialog.GetComponents<MonoBehaviour>();
			for (int i = 0; i < scripts.Length; ++i)
			{
				if (scripts[i] is MRLoadSaveGameSelectDialog)
				{
					MRLoadSaveGameSelectDialog dialogScript = (MRLoadSaveGameSelectDialog)scripts[i];
					selected = dialogScript.Selected;
					selectedName = dialogScript.SelectedGameName;
					break;
				}
			}
		}

		MRGame.ShowingUI = false;
		Destroy(mLoadSaveGameSelectDialog);
		mLoadSaveGameSelectDialog = null;

		if (selected >= 0 && !String.IsNullOrEmpty(selectedName))
		{
			MRGame.TheGame.Main.CurrentSaveGameSlot = selected;
			MRGame.TheGame.Main.CurrentSaveGameName = selectedName;
			StartCoroutine(MRGame.TheGame.Main.LoadGame());
		}
	}

	/// <summary>
	/// Called when the player selected which game to save.
	/// </summary>
	/// <param name="buttonId">Button identifier, 0 = ok.</param>
	private void OnSaveSlotSelected(int buttonId)
	{
		int selected = -1;
		String selectedName = "";
		if (buttonId == 0)
		{
			// ok selected
			MonoBehaviour[] scripts = mLoadSaveGameSelectDialog.GetComponents<MonoBehaviour>();
			for (int i = 0; i < scripts.Length; ++i)
			{
				if (scripts[i] is MRLoadSaveGameSelectDialog)
				{
					MRLoadSaveGameSelectDialog dialogScript = (MRLoadSaveGameSelectDialog)scripts[i];
					selected = dialogScript.Selected;
					selectedName = dialogScript.SelectedGameName;
					break;
				}
			}
		}

		MRGame.ShowingUI = false;
		Destroy(mLoadSaveGameSelectDialog);
		mLoadSaveGameSelectDialog = null;

		if (selected >= 0 && !String.IsNullOrEmpty(selectedName))
		{
			MRGame.TheGame.Main.CurrentSaveGameSlot = selected;
			MRGame.TheGame.Main.CurrentSaveGameName = selectedName;
			MRGame.TheGame.Main.SaveGame();
		}
	}

	private void OnInstructionsBackClicked()
	{
		MRGame.ShowingUI = false;
		
		Destroy (mInstructionsDialog);
		mInstructionsDialog = null;
	}

	private void OnCreditsBackClicked()
	{
		MRGame.ShowingUI = false;
		
		Destroy (mCreditsDialog);
		mCreditsDialog = null;
	}

	#endregion

	#region Members

	private static MRMainUI msTheUI;
	private GameObject mSelectionDialog;
	private GameObject mMessageDialog;
	private GameObject mYesNoDialog;
	private GameObject mTimedMessageBox;
	private GameObject mInstructionMessage;
	private GameObject mAttackManeuverDialog;
	private GameObject mCombatActionDialog;
	private GameObject mVictoryPointsSelectionDialog;
	private GameObject mLoadSaveGameSelectDialog;
	private GameObject mGameTypeSelectDialog;
	private GameObject mInstructionsDialog;
	private GameObject mCreditsDialog;
	private Button[] mDialogButtons;
	private ButtonClickedAction[] mDialogCallbacks;
	private OnButtonPressed mOkCallback;
	private OnButtonPressed mYesCallback;
	private OnButtonPressed mNoCallback;
	private float mTimedMessageBoxStartTime;

	#endregion
}

}