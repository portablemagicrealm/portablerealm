//
// MRSiteChit.cs
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
using AssemblyCSharp;

namespace PortableRealm
{
	
public static class SiteChitExtensions
{
	public static int ClearingNumber(this MRMapChit.eSiteChitType type)
	{
		switch (type)
		{
			case MRMapChit.eSiteChitType.Altar:
				return 1;
			case MRMapChit.eSiteChitType.Cairns:
				return 5;
			case MRMapChit.eSiteChitType.Hoard:
				return 6;
			case MRMapChit.eSiteChitType.Lair:
				return 3;
			case MRMapChit.eSiteChitType.Pool:
				return 6;
			case MRMapChit.eSiteChitType.Shrine:
				return 4;
			case MRMapChit.eSiteChitType.Statue:
				return 2;
			case MRMapChit.eSiteChitType.Vault:
				return 3;
		}
		return 1;
	}
}

public class MRSiteChit : MRMapChit
{
	#region Properties

	public MRMapChit.eSiteChitType SiteType
	{
		get{
			return mSiteType;
		}

		set{
			mSiteType = value;
			switch (mSiteType)
			{
				case MRMapChit.eSiteChitType.Altar:
					LongName = "ALTAR\n" + ClearingNumber;
					ShortName = "AL" + ClearingNumber;
					break;
				case MRMapChit.eSiteChitType.Cairns:
					LongName = "CAIRNS\n" + ClearingNumber;
					ShortName = "CA" + ClearingNumber;
					break;
				case MRMapChit.eSiteChitType.Hoard:
					LongName = "HOARD\n" + ClearingNumber;
					ShortName = "HO" + ClearingNumber;
					break;
				case MRMapChit.eSiteChitType.Lair:
					LongName = "LAIR\n" + ClearingNumber;
					ShortName = "LA" + ClearingNumber;
					break;
				case MRMapChit.eSiteChitType.Pool:
					LongName = "POOL\n" + ClearingNumber;
					ShortName = "PO" + ClearingNumber;
					break;
				case MRMapChit.eSiteChitType.Shrine:
					LongName = "SHRINE\n" + ClearingNumber;
					ShortName = "SH" + ClearingNumber;
					break;
				case MRMapChit.eSiteChitType.Statue:
					LongName = "STATUE\n" + ClearingNumber;
					ShortName = "ST" + ClearingNumber;
					break;
				case MRMapChit.eSiteChitType.Vault:
					LongName = "VAULT\n" + ClearingNumber;
					ShortName = "VA" + ClearingNumber;
					break;
			}
		}
	}

	public int ClearingNumber
	{
		get{
			return SiteType.ClearingNumber();
		}
	}

	public override int SummonSortIndex
	{
		get{
			// site chits only summon one box, so the order doesn't matter
			return 0;
		}
	}

	public static bool VaultOpened
	{
		get{
			return msVaultOpened;
		}

		set{
			msVaultOpened = value;
		}
	}

	#endregion

	#region Methods

	static MRSiteChit()
	{
		MapChitTypes[eMapChitType.Site] = typeof(MRSiteChit);
	}

	// dummy function to reference the class
	static public void Init() {}

	// Use this for initialization
	public override void Start ()
	{
		mChitType = eMapChitType.Site;
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
	}

	public void OnLooted(bool success, MRIControllable looter)
	{
		if (looter is MRCharacter)
		{
			MRCharacter character = (MRCharacter)looter;
			if (mSiteType == eSiteChitType.Cairns)
				character.SetFatigueBalance(1);
			if (mSiteType == eSiteChitType.Pool && success)
				character.SetFatigueBalance(1);
			else if (mSiteType == eSiteChitType.Vault && !msVaultOpened)
			{
				msVaultOpened = true;
				if (!character.HasActiveItem(MRItem.GetItem(MRUtility.IdForName("lost keys"))) &&
				    !character.HasActiveItem(MRItem.GetItem(MRUtility.IdForName("7-league boots"))) &&
				    !character.HasActiveItem(MRItem.GetItem(MRUtility.IdForName("gloves of strength"))))
				{
					character.SetFatigueBalance(1, MRActionChit.eType.Any, MRGame.eStrength.Tremendous);
				}
			}
		}
	}

	public override bool Load(JSONObject root)
	{
		base.Load(root);

		if (root["site"] is JSONNumber)
		{
			SiteType = (MRMapChit.eSiteChitType)((JSONNumber)root["site"]).IntValue;
		}
		else if (root["site"] is JSONString)
		{
			if (MRMapChit.ChitSiteMap.TryGetValue(((JSONString)root["site"]).Value, out mSiteType))
			{
				SiteType = mSiteType;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}

		return true;
	}

	public override void Save(JSONObject root)
	{
		base.Save(root);
		root["site"] = new JSONNumber((int)SiteType);
	}

	#endregion

	#region Members

	private MRMapChit.eSiteChitType mSiteType;
	private static bool msVaultOpened = false;

	#endregion
}

}