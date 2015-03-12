//
// JSONBoolean.cs
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
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Text;

namespace AssemblyCSharp
{
	public class JSONBoolean : JSONValue
	{
		#region Properties

		public bool Value
		{
			get{
				return mValue;
			}

			set{
				mValue = value;
			}
		}

		#endregion

		#region Methods

		public JSONBoolean ()
		{
		}

		public JSONBoolean (bool value)
		{
			Value = value;
		}

		public JSONBoolean (StringBuilder data)
		{
			Decode(data);
		}

		public void Decode(StringBuilder data)
		{
			while (Char.IsWhiteSpace(data[0]))
				data.Remove(0, 1);

			if (data.Length >= 4 && data[0] == 't' && data[1] == 'r' && data[2] == 'u' && data[3] == 'e')
			{
				mValue = true;
				data.Remove(0, 4);
			}
			else if (data.Length >= 5 && data[0] == 'f' && data[1] == 'a' && data[2] == 'l' && data[3] == 's' && data[4] == 'e')
			{
				mValue = false;
				data.Remove(0, 5);
			}
		}

		public void Encode(StringBuilder data)
		{
			if (mValue)
				data.Append("true");
			else
				data.Append("false");
		}

		#endregion

		#region Members

		private bool mValue;

		#endregion
	}
}

