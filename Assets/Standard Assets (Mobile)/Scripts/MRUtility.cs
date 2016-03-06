//
// MRUtility.cs
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
using System.Collections.Generic;
using System.Text;
using AssemblyCSharp;
using DamienG.Security.Cryptography;

public static class MRUtility
{
	//
	// Boolean enum that has an uninitialized/unknown state.
	//
	public enum eTriState
	{
		no,
		yes,
		unknown,
	}

	public class IntVector2
	{
		public int x;
		public int y;

		public IntVector2()
		{
			x = y = 0;
		}

		public IntVector2(int _x, int _y)
		{
			x = _x;
			y = _y;
		}

		public override string ToString()
		{
			return x + " : " + y;
		}

		public override bool Equals (object obj)
		{
			if (obj is IntVector2)
				return this == (IntVector2)obj;
			return false;
		}

		public override int GetHashCode()
		{
			return new {x, y}.GetHashCode();
		}

		public static IntVector2 operator +(IntVector2 lhs, IntVector2 rhs)
		{
			return new IntVector2(lhs.x + rhs.x, lhs.y + rhs.y);
		}

		public static IntVector2 operator -(IntVector2 lhs, IntVector2 rhs)
		{
			return new IntVector2(lhs.x - rhs.x, lhs.y - rhs.y);
		}

		public static bool operator ==(IntVector2 lhs, IntVector2 rhs)
		{
			bool lhsNull = object.ReferenceEquals(lhs, null);
			bool rhsNull = object.ReferenceEquals(rhs, null);
			if (!lhsNull && !rhsNull)
				return lhs.x == rhs.x && lhs.y == rhs.y;
			return lhsNull && rhsNull;
		}

		public static bool operator !=(IntVector2 lhs, IntVector2 rhs)
		{
			return !(lhs == rhs);
		}
	}

	//
	// Randomizes a list.
	//
	public static void Shuffle<T>(this IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) 
		{  
			n--;  
			int k = MRRandom.Range(0, n + 1);
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}
	}

	public static MRGame.eStrength Strength(this string name)
	{
		switch (char.ToLower(name[0]))
		{
			case 'n':
				return MRGame.eStrength.Negligable;
			case 'l':
				return MRGame.eStrength.Light;
			case 'm':
				return MRGame.eStrength.Medium;
			case 'h':
				return MRGame.eStrength.Heavy;
			case 't':
				return MRGame.eStrength.Tremendous;
			case 'i':
				return MRGame.eStrength.Immobile;
			default:
				throw new FormatException();
		}
	}

	public static string ToChitString(this MRGame.eStrength strength)
	{
		switch (strength)
		{
			case MRGame.eStrength.Negligable:
				return "-";
			case MRGame.eStrength.Light:
				return "L";
			case MRGame.eStrength.Medium:
				return "M";
			case MRGame.eStrength.Heavy:
				return "H";
			case MRGame.eStrength.Tremendous:
				return "T";
			case MRGame.eStrength.Immobile:
				return "";
			default:
				return "";
		}
	}

	public static MRArmor.eType Armor(this string name)
	{
		switch (char.ToLower(name[0]))
		{
			case 'b':
				return MRArmor.eType.Breastplate;
			case 'f':
				return MRArmor.eType.Full;
			case 'h':
				return MRArmor.eType.Helmet;
			case 's':
				return MRArmor.eType.Shield;
			default:
				throw new FormatException();
		}
	}

	public static string StringFromCurse(MRGame.eCurses curse)
	{
		string name = "";
		switch (curse)
		{
			case MRGame.eCurses.Eyemist:
				name = "Eyemist";
				break;
			case MRGame.eCurses.Squeak:
				name = "Squeak";
				break;
			case MRGame.eCurses.Wither:
				name = "Wither";
				break;
			case MRGame.eCurses.IllHealth:
				name = "Ill Health";
				break;
			case MRGame.eCurses.Ashes:
				name = "Ashes";
				break;
			case MRGame.eCurses.Disgust:
				name = "Disgust";
				break;
		}

		return name;
	}

	public static MRDwelling.eDwelling Dwelling(this string name)
	{
		switch (name.ToLower())
		{
			case "chapel":
				return MRDwelling.eDwelling.Chapel;
			case "guard":
				return MRDwelling.eDwelling.Guard;
			case "house":
				return MRDwelling.eDwelling.House;
			case "inn":
				return MRDwelling.eDwelling.Inn;
			case "ghosts":
				return MRDwelling.eDwelling.Ghosts;
			default:
				Debug.LogError("Unhandled dwelling name " + name);
				return MRDwelling.eDwelling.Inn;
		}
	}

	/// <summary>
	/// For a given string, returns the string with each word made upper-case.
	/// </summary>
	/// <returns>The base name</returns>
	/// <param name="name">The upper-case name</param>
	public static string DisplayName(string name)
	{
		StringBuilder buffer = new StringBuilder(name);
		bool nextUpper = true;
		for (int i = 0; i < buffer.Length; ++i)
		{
			if (nextUpper && !Char.IsWhiteSpace(buffer[i]))
			{
				buffer[i] = Char.ToUpper(buffer[i]);
				nextUpper = false;
			}
			else if (Char.IsWhiteSpace(buffer[i]))
				nextUpper = true;
		}
		return buffer.ToString();
	}

	/// <summary>
	/// Makes and object and all of its children visible or not.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="visible">If set to <c>true</c> visible.</param>
	public static void SetObjectVisibility(GameObject obj, bool visible)
	{
		if (obj != null)
		{
			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderers)
			{
				renderer.enabled = visible;
			}
		}
	}

	public static Type ClazzType(this MRMapChit.eMapChitType type)
	{
		return MRMapChit.MapChitTypes[type];
	}

	public static uint IdForName(string name)
	{
		return IdForName(name, 0);
	}

	public static uint IdForName(string name, int index)
	{
		uint id = 0;
		string idName = name + index;
		Crc32 crc32 = new Crc32();
		byte[] bytes = new byte[idName.Length * sizeof(char)];
		System.Buffer.BlockCopy(idName.ToCharArray(), 0, bytes, 0, bytes.Length);
		byte[] crcBytes = crc32.ComputeHash(bytes);
		foreach (byte b in crcBytes)
			id = id * 256 + b;
		return id;
	}

	public static bool compare(this MRSelectChitEvent.eCompare comparision, int lhs, int rhs)
	{
		switch (comparision)
		{
			case MRSelectChitEvent.eCompare.LessThan:
				return lhs < rhs;
			case MRSelectChitEvent.eCompare.LessThanEqualTo:
				return lhs <= rhs;
			case MRSelectChitEvent.eCompare.GreaterThanEqualTo:
				return lhs >= rhs;
			case MRSelectChitEvent.eCompare.GreaterThan:
				return lhs > rhs;
			default:
				break;
		}
		return lhs == rhs;
	}

	public static bool compare(this MRSelectChitEvent.eCompare comparision, MRGame.eStrength lhs, MRGame.eStrength rhs)
	{
		if (lhs == MRGame.eStrength.Any || rhs == MRGame.eStrength.Any)
			return true;

		switch (comparision)
		{
			case MRSelectChitEvent.eCompare.LessThan:
				return lhs < rhs;
			case MRSelectChitEvent.eCompare.LessThanEqualTo:
				return lhs <= rhs;
			case MRSelectChitEvent.eCompare.GreaterThanEqualTo:
				return lhs >= rhs;
			case MRSelectChitEvent.eCompare.GreaterThan:
				return lhs > rhs;
			default:
				break;
		}
		return lhs == rhs;
	}
}

