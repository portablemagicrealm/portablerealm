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

	private const float TIMED_MESSAGE_BOX_DISPLAY_TIME = 2.0f;

	#endregion

	#region Properties

	public GameObject SelectionDialogPrototype;
	public GameObject MessageDialogPrototype;
	public GameObject TimedMessagePrototype;
	public GameObject InstructionMessagePrototype;
	public GameObject AttackManeuverDialogPrototype;

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

	public void DisplayAttackManeuverDialog()
	{
		if (mAttackManeuverDialog == null)
		{
			mAttackManeuverDialog = (GameObject)Instantiate(AttackManeuverDialogPrototype);
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

	#endregion

	#region Members

	private static MRMainUI msTheUI;
	private GameObject mSelectionDialog;
	private GameObject mMessageDialog;
	private GameObject mTimedMessageBox;
	private GameObject mInstructionMessage;
	private GameObject mAttackManeuverDialog;
	private Button[] mDialogButtons;
	private ButtonClickedAction[] mDialogCallbacks;
	private OnButtonPressed mOkCallback;
	private float mTimedMessageBoxStartTime;

	#endregion
}
