//
// MRDwelling.cs
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

namespace PortableRealm
{
	
public class MRDwelling : MRChit, MRIColorSource
{
	#region Constants

	public enum eDwelling
	{
		None,
		Chapel,
		GuardHouse,
		House,
		Inn,
		LargeFire,
		SmallFire,
		Ghosts
	}

	#endregion

	#region Properties

	public eDwelling Type
	{
		get{
			return mType;
		}

		set{
			mType = value;
			Id = MRUtility.IdForName(value.ToString());
		}
	}

	public override Transform Parent
	{
		get{
			return base.Parent;
		}

		set {
			base.Parent = value;
			transform.localScale = Vector3.one;
		}
	}

	public override int SortValue
	{
		get{
			return (int)MRGame.eSortValue.Dwelling;
		}
	}

	/// <summary>
	/// Returns a list of the color magic supplied by this object.
	/// </summary>
	/// <value>The magic supplied.</value>
	public virtual IList<MRGame.eMagicColor> MagicSupplied 
	{ 
		get	{
			List<MRGame.eMagicColor> magic = new List<MRGame.eMagicColor>();
			if (Type == eDwelling.Chapel)
				magic.Add(MRGame.eMagicColor.White);
			return magic;
		}
	}

	#endregion

	#region Methods

	// Called when the script instance is being loaded
	void Awake()
	{
		mCounter = (GameObject)Instantiate(MRGame.TheGame.largeChitPrototype);
		mCounter.transform.parent = transform;
	}

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		if (mType != eDwelling.None)
		{
			string iconName = null;
			switch (mType)
			{
				case eDwelling.Chapel:
					iconName = "Textures/chapel";
					break;
				case eDwelling.GuardHouse:
					iconName = "Textures/guard";
					break;
				case eDwelling.House:
					iconName = "Textures/house";
					break;
				case eDwelling.Inn:
					iconName = "Textures/inn";
					break;
				case eDwelling.LargeFire:
					iconName = "Textures/large_fire";
					break;
				case eDwelling.SmallFire:
					iconName = "Textures/small_fire";
					break;
				case eDwelling.Ghosts:
					break;
			}

			if (iconName != null)
			{
				Sprite texture = (Sprite)Resources.Load(iconName, typeof(Sprite));
				SpriteRenderer[] sprites = mCounter.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer sprite in sprites)
				{
					if (sprite.gameObject.name == "FrontSide" ||
					    sprite.gameObject.name == "BackSide")
					{
						sprite.sprite = texture;
					}
				}
			}
		}
	}

	public static MRDwelling Create()
	{
		GameObject root = new GameObject();
		MRDwelling chit = root.AddComponent<MRDwelling>();
		
		return chit;
	}

	#endregion

	#region Members

	private eDwelling mType;

	#endregion
}

}