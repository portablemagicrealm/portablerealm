//
// MRFightChit.cs
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
using System.Text;

public class MRFightChit : MRActionChit
{
	#region Properties

	public override eType Type
	{
		get{
			return eType.fight;
		}
	}

	public MRGame.eStrength BaseStrength
	{
		get{
			return mBaseStrength;
		}

		set{
			mBaseStrength = value;
			CurrentStrength = value;
		}
	}

	public MRGame.eStrength CurrentStrength
	{
		get{
			return mCurrentStrength;
		}
		
		set{
			mCurrentStrength = value;
		}
	}

	#endregion

	#region Methods

	// Use this for initialization
	public override void Start ()
	{
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();

		StringBuilder buffer = new StringBuilder("FIGHT\n");
		buffer.Append(CurrentStrength.ToChitString());
		buffer.Append(" ");
		buffer.Append(CurrentTime);
		buffer.Append("\n");
		for (int i = 0; i < CurrentAsterisks; ++i)
		{
			buffer.Append("*");
			if (i < CurrentAsterisks - 1)
				buffer.Append(" ");
		}
		TextMesh text = mCounter.GetComponentInChildren<TextMesh>();
		text.text = buffer.ToString();
	}

	#endregion

	#region Members

	private MRGame.eStrength mBaseStrength;
	private MRGame.eStrength mCurrentStrength;

	#endregion
}

