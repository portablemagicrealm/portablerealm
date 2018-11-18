//
// MRSpell.cs
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AssemblyCSharp;

namespace PortableRealm
{
	
public abstract class MRSpell
{
	#region Properties

	public uint Id
	{
		get {
			return mId;
		}
	}

	public string Name
	{
		get {
			return mName;
		}
	}

	public int BaseMagicType 
	{
		get {
			return mBaseType;
		}
	}

	public int CurrentMagicType 
	{
		get {
			return mCurrentType;
		}

		set{
			mCurrentType = value;
		}
	}

	public MRGame.eMagicColor Color
	{
		get {
			return mColor;
		}
	}

	public MRGame.eSpellDuration Duration
	{
		get {
			return mDuration;
		}
	}

	public bool Awakened
	{
		get {
			return mAwakened;
		}

		set {
			mAwakened = value;
		}
	}

	public bool Known
	{
		get {
			return mKnown;
		}

		set {
			mKnown = value;
		}
	}

	public bool Hidden
	{
		get{
			return !Awakened && !Known;
		}
	}

	public ReadOnlyCollection<MRGame.eSpellTarget> Targets
	{
		get{
			return mTargetTypes;
		}
	}

	#endregion

	#region Methods

	protected MRSpell()
	{
	}

	protected MRSpell(JSONObject data, int index)
	{
		mAwakened = false;
		mKnown = false;

		mName = ((JSONString)data["name"]).Value;
		mBaseType = ((JSONNumber)data["type"]).IntValue;
		mColor = ((JSONString)data["color"]).Value.ToColor();
		mDuration = ((JSONString)data["duration"]).Value.ToSpellDuration();

		JSONArray targets = ((JSONArray)data["target"]);
		MRGame.eSpellTarget[] targetTypes = new MRGame.eSpellTarget[targets.Count];
		for (int i = 0; i < targetTypes.Length; ++i)
		{
			targetTypes[i] = ((JSONString)targets[i]).Value.ToSpellTarget();
		}
		mTargetTypes = new ReadOnlyCollection<MRGame.eSpellTarget>(targetTypes);

		mCurrentType = mBaseType;
		mId = MRUtility.IdForName(mName);
	}

	/// <summary>
	/// Activates the spell effect.
	/// </summary>
	/// <param name="caster">Character casting the spell.</param>
	/// <param name="magic">Magic chit used to activate the spell.</param>
	/// <param name="source">Color source for the spell.</param>
	/// <param name="spellTargets">Targets of the spell.</param>
	public abstract void Activate(MRCharacter caster, MRMagicChit magic, MRIColorSource source, List<MRISpellTarget> spellTargets);

	#endregion

	#region Members

	private uint mId;
	private string mName;
	private int mBaseType;
	private int mCurrentType;
	private bool mAwakened;
	private bool mKnown;
	private MRGame.eMagicColor mColor;
	private MRGame.eSpellDuration mDuration;
	private ReadOnlyCollection<MRGame.eSpellTarget> mTargetTypes;

	#endregion

}

}