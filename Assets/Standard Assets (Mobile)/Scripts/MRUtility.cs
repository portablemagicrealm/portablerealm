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

namespace PortableRealm
{
	
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

	/// <summary>
	/// Converts a number from 1-8 to its Roman numeral equivalent.
	/// </summary>
	/// <returns>The roman numeral.</returns>
	/// <param name="value">Value.</param>
	public static string ToRomanNumeral(this Int32 value)
	{
		switch (value)
		{
			case 1:
				return "I";
			case 2:
				return "II";
			case 3:
				return "III";
			case 4:
				return "IV";
			case 5:
				return "V";
			case 6:
				return "VI";
			case 7:
				return "VII";
			case 8:
				return "VIII";
			default:
				return "??";
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

	/// <summary>
	/// Converts a dwelling name to a dwelling enum.
	/// </summary>
	/// <returns>The dwelling.</returns>
	/// <param name="name">Name.</param>
	public static MRDwelling.eDwelling Dwelling(this string name)
	{
		switch (name.ToLower())
		{
			case "chapel":
				return MRDwelling.eDwelling.Chapel;
			case "guard":
			case "guardhouse":
				return MRDwelling.eDwelling.GuardHouse;
			case "house":
				return MRDwelling.eDwelling.House;
			case "inn":
				return MRDwelling.eDwelling.Inn;
			case "ghosts":
				return MRDwelling.eDwelling.Ghosts;
			case "largefire":
				return MRDwelling.eDwelling.LargeFire;
			case "smallfire":
				return MRDwelling.eDwelling.SmallFire;
			default:
				Debug.LogError("Unhandled dwelling name " + name);
				return MRDwelling.eDwelling.Inn;
		}
	}

	/// <summary>
	/// Converts a native name to a native enum.
	/// </summary>
	/// <returns>The native.</returns>
	/// <param name="name">Name.</param>
	public static MRGame.eNatives Native(this string name)
	{
		switch (name.ToLower())
		{
			case "bashkars":
				return MRGame.eNatives.Bashkars;
			case "company":
				return MRGame.eNatives.Company;
			case "guard":
				return MRGame.eNatives.Guard;
			case "lancers":
				return MRGame.eNatives.Lancers;
			case "order":
				return MRGame.eNatives.Order;
			case "patrol":
				return MRGame.eNatives.Patrol;
			case "rogues":
				return MRGame.eNatives.Rogues;
			case "soldiers":
				return MRGame.eNatives.Soldiers;
			case "woodfolk":
				return MRGame.eNatives.Woodfolk;
			default:
				Debug.LogError("Unhandled native name " + name);
				return MRGame.eNatives.Bashkars;
		}
	}

	/// <summary>
	/// Converts a relationship name to a relationship enum.
	/// </summary>
	/// <returns>The relationship.</returns>
	/// <param name="name">Name.</param>
	public static MRGame.eRelationship Relationship(this string name)
	{
		switch (name.ToLower())
		{
			case "enemy":
				return MRGame.eRelationship.Enemy;
			case "unfriendly":
				return MRGame.eRelationship.Unfriendly;
			case "neutral":
				return MRGame.eRelationship.Neutral;
			case "friendly":
				return MRGame.eRelationship.Friendly;
			case "ally":
				return MRGame.eRelationship.Ally;
			default:
				Debug.LogError("Unhandled relationship name " + name);
				return MRGame.eRelationship.Neutral;
		}
	}

	/// <summary>
	/// Converts a site name to a site enum.
	/// </summary>
	/// <returns>The site.</returns>
	/// <param name="name">Name.</param>
	public static MRMapChit.eSiteChitType Site(this string name)
	{
		switch (name.ToLower())
		{
			case "altar":
				return MRMapChit.eSiteChitType.Altar;
			case "cairns":
				return MRMapChit.eSiteChitType.Cairns;
			case "hoard":
				return MRMapChit.eSiteChitType.Hoard;
			case "lair":
				return MRMapChit.eSiteChitType.Lair;
			case "pool":
				return MRMapChit.eSiteChitType.Pool;
			case "shrine":
				return MRMapChit.eSiteChitType.Shrine;
			case "statue":
				return MRMapChit.eSiteChitType.Statue;
			case "vault":
				return MRMapChit.eSiteChitType.Vault;
			default:
				Debug.LogError("Unhandled site name " + name);
				return 0;
		}
	}

	/// <summary>
	/// Converts a color name to a color enum.
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="name">Name.</param>
	public static MRGame.eMagicColor ToColor(this string name)
	{
		switch (name.ToLower())
		{
			case "white":
				return MRGame.eMagicColor.White;
			case "grey":
			case "gray":
				return MRGame.eMagicColor.Grey;
			case "gold":
				return MRGame.eMagicColor.Gold;
			case "purple":
				return MRGame.eMagicColor.Purple;
			case "black":
				return MRGame.eMagicColor.Black;
			case "any":
				return MRGame.eMagicColor.Any;
			default:
				Debug.LogError("Unhandled color name " + name);
				return MRGame.eMagicColor.None;
		}
	}

	/// <summary>
	/// Converts a spell duration name to a duration enum.
	/// </summary>
	/// <returns>The spell duration.</returns>
	/// <param name="name">Name.</param>
	public static MRGame.eSpellDuration ToSpellDuration(this string name)
	{
		switch (name.ToLower())
		{
			case "attack":
				return MRGame.eSpellDuration.Attack;
			case "combat":
				return MRGame.eSpellDuration.Combat;
			case "day":
				return MRGame.eSpellDuration.Day;
			case "fly":
				return MRGame.eSpellDuration.Fly;
			case "instant":
				return MRGame.eSpellDuration.Instant;
			case "permanent":
				return MRGame.eSpellDuration.Permanent;
			case "phase":
				return MRGame.eSpellDuration.Phase;
			default:
				Debug.LogError("Unhandled spell duration name " + name);
				return MRGame.eSpellDuration.None;
		}
	}

	/// <summary>
	/// Converts a spell target name to a target enum.
	/// </summary>
	/// <returns>The spell target.</returns>
	/// <param name="name">Name.</param>
	public static MRGame.eSpellTarget ToSpellTarget(this string name)
	{
		switch (name.ToLower())
		{
			case "artifact":
				return MRGame.eSpellTarget.Artifact;
			case "bats":
				return MRGame.eSpellTarget.Bats;
			case "caveclearing":
				return MRGame.eSpellTarget.CaveClearing;
			case "character":
				return MRGame.eSpellTarget.Character;
			case "characters":
				return MRGame.eSpellTarget.Characters;
			case "clearing":
				return MRGame.eSpellTarget.Clearing;
			case "controlledmonster":
				return MRGame.eSpellTarget.ControlledMonster;
			case "curse":
				return MRGame.eSpellTarget.Curse;
			case "demon":
				return MRGame.eSpellTarget.Demon;
			case "giants":
				return MRGame.eSpellTarget.Giants;
			case "goblin":
				return MRGame.eSpellTarget.Goblin;
			case "goblins":
				return MRGame.eSpellTarget.Goblins;
			case "hex":
				return MRGame.eSpellTarget.Hex;
			case "hiredleader":
				return MRGame.eSpellTarget.HiredLeader;
			case "lightcharacter":
				return MRGame.eSpellTarget.LightCharacter;
			case "monster":
				return MRGame.eSpellTarget.Monster;
			case "monsters":
				return MRGame.eSpellTarget.Monsters;
			case "native":
				return MRGame.eSpellTarget.Native;
			case "natives":
				return MRGame.eSpellTarget.Natives;
			case "nativegroup":
				return MRGame.eSpellTarget.NativeGroup;
			case "octopus":
				return MRGame.eSpellTarget.Octopus;
			case "ogre":
				return MRGame.eSpellTarget.Ogre;
			case "ogres":
				return MRGame.eSpellTarget.Ogres;
			case "soundchit":
				return MRGame.eSpellTarget.SoundChit;
			case "spell":
				return MRGame.eSpellTarget.Spell;
			case "spellbook":
				return MRGame.eSpellTarget.SpellBook;
			case "spellchitany":
				return MRGame.eSpellTarget.SpellChitAny;
			case "spellchit1":
				return MRGame.eSpellTarget.SpellChit1;
			case "spellchit2":
				return MRGame.eSpellTarget.SpellChit2;
			case "spellchit3":
				return MRGame.eSpellTarget.SpellChit3;
			case "spellchit4":
				return MRGame.eSpellTarget.SpellChit4;
			case "spellchit5":
				return MRGame.eSpellTarget.SpellChit5;
			case "spellchit6":
				return MRGame.eSpellTarget.SpellChit6;
			case "spellchit7":
				return MRGame.eSpellTarget.SpellChit7;
			case "spellchit8":
				return MRGame.eSpellTarget.SpellChit8;
			case "spider":
				return MRGame.eSpellTarget.Spider;
			case "weapon":
				return MRGame.eSpellTarget.Weapon;
			case "wingeddemon":
				return MRGame.eSpellTarget.WingedDemon;
			default:
				Debug.LogError("Unhandled spell target name " + name);
				return MRGame.eSpellTarget.None;
		}
	}

	/// <summary>
	/// For a given string, returns the string with each word made upper-case.
	/// </summary>
	/// <returns>The base name</returns>
	/// <param name="name">The upper-case name</param>
	public static string DisplayName(this string name)
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
		string idName = name.ToLower() + index;
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

}