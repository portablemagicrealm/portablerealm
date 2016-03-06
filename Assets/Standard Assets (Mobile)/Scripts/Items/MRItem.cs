//
// MRItem.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AssemblyCSharp;

public class MRItem : MRIGamePiece, MRISerializable
{
	#region Properties

	public bool Active
	{
		get{
			return mIsActive;
		}

		set{
			mIsActive = value;
		}
	}

	public int Index
	{
		get{
			return mIndex;
		}
	}

	public MRGame.eStrength BaseWeight
	{
		get{
			return mBaseWeight;
		}
	}

	public int BaseFame
	{
		get{
			return mBaseFame;
		}
	}

	public int BaseNotoriety
	{
		get{
			return mBaseNotoriety;
		}
	}

	public int BasePrice
	{
		get{
			return mBasePrice;
		}
	}

	public MRCharacter Owner
	{
		get{
			return mOwner;
		}

		set{
			mOwner = value;
		}
	}

	public MRGamePieceStack StartStack 
	{ 
		get{
			return mStartStack;
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
			mCounter.transform.parent = value;
		}
	}

	public Bounds Bounds
	{
		get{
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

	public string Name
	{
		get{
			return mName;
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
			return (int)MRGame.eSortValue.Item;
		}
	}

	#endregion
	
	#region Methods

	protected MRItem()
	{
	}

	protected MRItem(JSONObject data, int index)
	{
		mName = ((JSONString)data["name"]).Value;
		mIndex = index;

		mBaseFame = ((JSONNumber)data["fame"]).IntValue;
		mBaseNotoriety = ((JSONNumber)data["notoriety"]).IntValue;
		mBasePrice = ((JSONNumber)data["gold"]).IntValue;

		string weight = ((JSONString)data["weight"]).Value;
		mBaseWeight = weight.Strength();

		// compute the id by using the item name plus the item index
		uint id = MRUtility.IdForName(mName, index);
		while (msItems.ContainsKey(id))
			id = MRUtility.IdForName(mName, ++index);
		Id = id;
		msItems.Add(id, this);
	}

	public static MRItem GetItem(uint id)
	{
		MRItem item = null;
		msItems.TryGetValue(id, out item);
		return item;
	}

	// Update is called once per frame
	public virtual void Update ()
	{
	}

	public virtual bool Load(JSONObject root)
	{
		if (mId != ((JSONNumber)root["id"]).UintValue)
			return false;

		return true;
	}
	
	public virtual void Save(JSONObject root)
	{
		root["id"] = new JSONNumber(mId);
	}

	#endregion

	#region Members

	private uint mId;
	private string mName;
	private int mIndex;
	private MRGame.eStrength mBaseWeight;
	private int mBaseFame;
	private int mBaseNotoriety;
	private int mBasePrice;
	private bool mIsActive;
	private MRCharacter mOwner;

	protected GameObject mCounter;
	protected Vector3 mOldScale;
	protected MRGamePieceStack mStartStack;
	protected MRGamePieceStack mStack;

	private static IDictionary<uint, MRItem> msItems = new Dictionary<uint, MRItem>();

	#endregion
}

