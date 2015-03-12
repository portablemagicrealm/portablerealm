//
// MRRoad.cs
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

public class MRRoad : MonoBehaviour 
{
	#region Constants

	public enum eRoadType
	{
		Road,
		Tunnel,
		HiddenPath,
		SecretPassage,
	}

	#endregion

	#region Properties

	public MRClearing clearingConnection0;
	public MRClearing clearingConnection1;
	public MREdge edgeConnection;
	public eRoadType type;

	public string Name
	{
		get
		{
			if (clearingConnection0 != null && clearingConnection1 != null)
			{
				return clearingConnection0.Name + "-" + clearingConnection1.Name;
			}
			else if (clearingConnection0 != null && edgeConnection != null)
			{
				return clearingConnection0.Name + "-Edge" + edgeConnection.side;
			}
			else
			{
				Debug.LogError("Road with no connections");
				return "unknown";
			}
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	void Start () 
	{
		renderer.enabled = false;
		gameObject.SetActive(false);
		enabled = false;
		try
		{
			MRGame.TheGame.TheMap.Roads[Name] = this;
		}
		catch (Exception err)
		{
			Debug.LogError("Road " + Name + " : " + err.ToString());
		}
	}

	void OnDestroy()
	{
		try
		{
			// remove ourself from the road map
			MRRoad test;
			if (MRGame.TheGame.TheMap.Roads.TryGetValue(Name, out test))
			{
				if (test == this)
					MRGame.TheGame.TheMap.Roads.Remove(Name);
			}
		}
		catch (Exception err)
		{
			Debug.LogError("Road " + Name + " : " + err.ToString());
		}
	}

	// Update is called once per frame
	void Update () 
	{
	
	}

	#endregion
}
