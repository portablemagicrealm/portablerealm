//
// JSONDecoder.cs
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
using System.Reflection;
using System.Text;

namespace AssemblyCSharp
{
	public class JSONDecoder
	{
		public JSONDecoder ()
		{
		}

		public static JSONValue CreateJSONValue(StringBuilder data)
		{
			while (Char.IsWhiteSpace(data[0]))
				data.Remove(0, 1);

			switch (data[0])
			{
				case '{':
					return new JSONObject(data);
				case '[':
					return new JSONArray(data);
				case '\"':
					return new JSONString(data);
				case 't':
				case 'f':
					return new JSONBoolean(data);
				case 'n':
					return new JSONNull(data);
				default:
					if (data[0] == '-' || Char.IsDigit(data[0]))
						return new JSONNumber(data);
					break;
			}
			throw new InvalidJSONException();
		}

		public static object[] DecodeObjects(StringBuilder data)
		{
			object[] result = null;

			JSONValue jsonData = CreateJSONValue(data);
			if (jsonData is JSONObject)
			{
				result = DecodeObjects((JSONObject)jsonData);
			}

			return result;
		}

		public static object[] DecodeObjects(JSONObject jsonData)
		{
			object[] result = null;
			
			JSONValue className = jsonData["class"];
			if (className != null && className is JSONString)
			{
				int amount = ((JSONNumber)jsonData["amount"]).IntValue;
				result = new object[amount];
				Type t = Type.GetType(((JSONString)className).Value);
				if (t == null)
					throw new InvalidJSONException();
				ConstructorInfo cinfo = t.GetConstructor(new Type[] {typeof(JSONObject), typeof(int)});
				if (cinfo == null)
					throw new InvalidJSONException();
				for (int i = 0; i < amount; ++i)
				{
					result[i] = cinfo.Invoke(new object[] {jsonData, i});
				}
			}

			return result;
		}
	}
}
