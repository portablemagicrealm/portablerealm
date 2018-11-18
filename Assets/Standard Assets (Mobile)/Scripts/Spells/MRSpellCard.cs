//
// MRSpellCard.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2017 Steve Jakab
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
using System.Text;

namespace PortableRealm
{
public class MRSpellCard : MRIGamePiece
{
	#region Properties

	public MRSpell Spell
	{
		get {
			return mSpell;
		}
	}

	public bool Selectable
	{
		get{
			return mSelectable;
		}

		set{
			mSelectable = value;
		}
	}

	/**********************/
	// MRIGamePiece properties

	public uint Id
	{
		get{
			return mId;
		}

		protected set{
			mId = value;
			MRGame.TheGame.AddGamePiece(this);
		}
	}

	public int Layer 
	{ 
		get {
			return mCounter.layer;
		}

		set {
			if (value >= 0 && value < 32)
			{
				mCounter.layer = value;
				foreach (Transform transform in mCounter.GetComponentsInChildren<Transform>())
				{
					transform.gameObject.layer = value;
				}
			}
			else
			{
				Debug.LogError("Trying to set item " + Name + " to layer " + value);
			}
		}
	}

	public virtual Vector3 Position
	{
		get{
			return mCounter.transform.position;
		}

		set{
			mCounter.transform.position = value;
		}
	}

	public virtual Vector3 LocalScale
	{
		get{
			return mCounter.transform.localScale;
		}

		set{
			mCounter.transform.localScale = value;
		}
	}

	public virtual Vector3 LossyScale
	{
		get{
			return mCounter.transform.lossyScale;
		}
	}

	public virtual Quaternion Rotation 
	{ 
		get {
			return mCounter.transform.rotation;
		}

		set {
			mCounter.transform.rotation = value;
		}
	}

	public Transform Parent
	{
		get {
			return mCounter.transform.parent;
		}

		set {
			mCounter.transform.SetParent(value);
		}
	}

	public Bounds Bounds
	{
		get{
			if (mCounter != null)
			{
				var component = mCounter.GetComponentInChildren<SpriteRenderer>();
				var sprite = component.sprite;
				Bounds bounds = sprite.bounds;
				return bounds;
//				return mCounter.GetComponentInChildren<SpriteRenderer>().sprite.bounds;
			}
			else
				return new Bounds();
		}
	}

	public Vector3 OldScale 
	{ 
		get	{
			return mOldScale;
		}
		set {
			mOldScale = value;
		}
	}

	public string Name
	{
		get{
			return mSpell.Name;
		}
	}

	public MRGamePieceStack Stack 
	{ 
		get{
			return mStack;
		}
		set{
			if (mStartStack == null)
				mStartStack = value;
			mStack = value;
		}
	}

	public virtual bool Visible
	{
		get{
			return mCounter.activeSelf;
		}

		set{
			MRUtility.SetObjectVisibility(mCounter, value);
		}
	}

	public virtual int SortValue
	{
		get{
			return (int)MRGame.eSortValue.Spell;
		}
	}

	#endregion

	#region Methods

	public MRSpellCard(MRSpell spell)
	{
		mSpell = spell;
		Id = mSpell.Id;

		mCounter = (GameObject)MRGame.Instantiate(MRGame.TheGame.spellCardPrototype);
		TextMesh text = mCounter.GetComponentInChildren<TextMesh>();
		if (text.name == "FrontText")
		{
			StringBuilder buffer = new StringBuilder(mSpell.Name.ToUpper());
			buffer.Replace(' ', '\n');
			buffer.AppendLine();
			buffer.Append(mSpell.CurrentMagicType.ToRomanNumeral());
			text.text = buffer.ToString();
		}

		SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in sprites)
		{
			if (sprite.gameObject.name == "FrontOverlay")
				mFontSelectable = sprite.gameObject;
//			else if (sprite.gameObject.name == "BackOverlay")
//				mBackSelectable = sprite.gameObject;
		}
	}

	// Update is called once per frame
	public virtual void Update ()
	{
		Vector3 orientation = mCounter.transform.localEulerAngles;
		if ((mSpell.Hidden && Math.Abs(orientation.y - 180f) > 0.1f) ||
			(!mSpell.Hidden && Math.Abs(orientation.y) > 0.1f))
		{
			mCounter.transform.Rotate(new Vector3(0, 180f, 0));
		}

		mFontSelectable.SetActive(Selectable);
//		mBackSelectable.SetActive(Selectable);
	}

	#endregion

	#region Members

	private uint mId;
	private bool mSelectable;
	private MRSpell mSpell;
	private GameObject mCounter;
	private GameObject mFontSelectable;
	private GameObject mBackSelectable;
	private Vector3 mOldScale;
	private MRGamePieceStack mStartStack;
	private MRGamePieceStack mStack;

	#endregion
}
}

