//
// MRDuckChit.cs
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

public class MRDuckChit : MRMoveChit
{
	#region Properties
	
	public override eType Type
	{
		get{
			return eType.Duck;
		}
	}

	#endregion

	#region Methods

	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
		
		StringBuilder buffer = new StringBuilder("DUCK\n");
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

	public override bool CanBeUsedFor(eAction action, MRGame.eStrength strength)
	{
		bool canBeUsed = false;
		if (strength == MRGame.eStrength.Any || strength <= BaseStrength)
		{
			switch (action)
			{
				case eAction.Duck:
				case eAction.Fatigue:
				case eAction.FatigueMove:
					canBeUsed = true;
					break;
				case eAction.FatigueChange:
				case eAction.FatigueChangeMove:
					canBeUsed = (BaseAsterisks == 1);
					break;
			}
		}
		return canBeUsed;
	}

	#endregion
}

