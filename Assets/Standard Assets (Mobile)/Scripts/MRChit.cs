//
// MRChit.cs
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
using AssemblyCSharp;

public class MRChit : MonoBehaviour, MRIGamePiece, MRISerializable
{
	#region Constants

	public enum eSide
	{
		Front,
		Back
	}

	#endregion

	#region Properties

	public eSide SideUp
	{
		get{
			return mSideUp;
		}

		set{
			mSideUp = value;
		}
	}

	/**********************/
	// MRIGamePiece properties

	public uint Id
	{
		get
		{
			return mId;
		}
		
		protected set{
			mId = value;
			MRGame.TheGame.AddGamePiece(this);
		}
	}

	public int Layer
	{
		get{
			return gameObject.layer;
		}
		
		set{
			SetLayer(value);
		}
	}
	
	public virtual Vector3 Position
	{
		get{
			//return mCounter.transform.position;
			return gameObject.transform.position;
		}

		set{
			//mCounter.transform.position = value;
			gameObject.transform.position = value;
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
		get {
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

	public virtual Transform Parent
	{
		get{
			return gameObject.transform.parent;
		}

		set {
			gameObject.transform.parent = value;
		}
	}

	public Bounds Bounds
	{
		get{
			//return mBounds;
			if (mCounter != null)
				return mCounter.GetComponentInChildren<SpriteRenderer>().sprite.bounds;
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

	public virtual string Name
	{
		get{
			return mName;
		}

		protected set{
			mName = value;
			mId = MRUtility.IdForName(mName);
		}
	}

	public MRGamePieceStack Stack 
	{ 
		get{
			return mStack;
		}
		set{
			mStack = value;
		}
	}

	public virtual bool Visible
	{
		get{
			return mCounter.activeSelf;
		}
		
		set{
			mCounter.SetActive(value);
		}
	}

	public virtual Color FrontColor
	{
		get{
			return mFrontColor;
		}

		set{
			mFrontColor = value;
			if (mFrontSide != null)
				mFrontSide.color = mFrontColor;
		}
	}

	public virtual Color BackColor
	{
		get{
			return mBackColor;
		}

		set{
			mBackColor = value;
			if (mBackSide != null)
				mBackSide.color = mBackColor;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	public virtual void Start ()
	{
		try
		{
			mCounter.transform.parent = gameObject.transform;
			mCounter.transform.localPosition = Vector3.zero;
			SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer sprite in sprites)
			{
				if (sprite.gameObject.name == "FrontSide")
					mFrontSide = sprite;
				else if (sprite.gameObject.name == "BackSide")
					mBackSide = sprite;
			}
			FrontColor = MRGame.white;
			BackColor = MRGame.white;
		}
		catch (Exception err)
		{
			Debug.LogError(err.ToString());
		}
	}
	
	// Update is called once per frame
	public virtual void Update ()
	{
		if (mCounter != null)
		{
			if (mBounds == null)
			{
				mBounds = mCounter.GetComponentInChildren<SpriteRenderer>().sprite.bounds;
			}
			Vector3 orientation = mCounter.transform.localEulerAngles;
			if (mSideUp == eSide.Back)
				orientation.y = 180f;
			else
				orientation.y = 0;
			mCounter.transform.localEulerAngles = orientation;
		}
	}

	void OnDestroy()
	{
		if (mCounter != null)
		{
			DestroyObject(mCounter);
			mCounter = null;
		}
	}

	protected void SetLayer(int layer)
	{
		if (mCounter != null)
		{
			gameObject.layer = layer;
			mCounter.layer = layer;
		
			// we also need to set the layer of our children
			Component[] components = mCounter.GetComponentsInChildren<Component>();
			foreach (Component component in components)
			{
				component.gameObject.layer = layer;
			}
		}
	}

	public virtual bool Load(JSONObject root)
	{
		if (root == null)
			return false;

		mSideUp = (eSide)((JSONNumber)root["sideup"]).IntValue;
		return true;
	}
	
	public virtual void Save(JSONObject root)
	{
		root["sideup"] = new JSONNumber((int)mSideUp);
	}

	#endregion

	#region Members
	
	protected GameObject mCounter;
	protected Bounds mBounds;
	protected Vector3 mOldScale;
	protected eSide mSideUp;
	protected MRGamePieceStack mStack;
	private uint mId;
	private string mName;
	private Color mFrontColor;
	private Color mBackColor;
	private SpriteRenderer mFrontSide;
	private SpriteRenderer mBackSide;

	#endregion
}

