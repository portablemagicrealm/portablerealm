//
// MREdge.cs
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

public class MREdge : MonoBehaviour
{
	#region Properties

	// this value corresponds to the facing of MRTile
	//
	//       0
	//     5   1
	//     4   2
	//       3
	//
	public int side;

	/// <summary>
	/// Returns the current road connected to the edge.
	/// </summary>
	/// <value>The road, or null if not connected to a road</value>
	public MRRoad Road
	{
		get{
			if (mTile != null)
			{
				if (mTile.Front == MRTileSide.eType.Normal)
					return mRoad;
				else
					return mEnchantedRoad;
			}
			return null;
		}
	}

	public MRRoad NormalRoad
	{
		get{
			return mRoad;
		}

		set{
			mRoad = value;
		}
	}

	public MRRoad EnchantedRoad
	{
		get{
			return mEnchantedRoad;
		}

		set{
			mEnchantedRoad = value;
		}
	}

	public MRTile Tile
	{
		get{
			return mTile;
		}

		set{
			mTile = value;
		}
	}

	#endregion

	#region Methods

	void Awake ()
	{
		GetComponent<Renderer>().enabled = false;
		gameObject.SetActive(false);
		enabled = false;
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	// 
	// Makes sure an edge index is between 0 and 5
	//
	public static int Normalize(int edge)
	{
		while (edge >= 6)
			edge -= 6;
		while (edge < 0)
			edge += 6;
		return edge;
	}

	#endregion

	#region Members
	
	private MRRoad mRoad;
	private MRRoad mEnchantedRoad;
	private MRTile mTile;

	#endregion
}

