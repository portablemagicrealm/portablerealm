//
// JSONNumber.cs
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
using UnityEngine;
using System;
using System.Text;

namespace AssemblyCSharp
{
	/// <summary>
	/// JSON numeric value. Note that this differs from standard JSON in that we keep track of unsigned vs signed int values.
	/// </summary>
	public class JSONNumber : JSONValue
	{
		#region Constants

		public enum eNumericType
		{
			Integer,
			UnsignedInteger,
			Float,
			Long,
			UnsignedLong,
		}

		#endregion

		#region Properties

		public eNumericType Type
		{
			get{
				return mType;
			}
		}

		public int IntValue
		{
			get{
				switch (Type)
				{
					case eNumericType.Float:
						return (int)mFloatValue;
					case eNumericType.Integer:
						return mIntValue;
					case eNumericType.UnsignedInteger:
						return (int)mUintValue;
					case eNumericType.Long:
						return (int)mLongValue;
					case eNumericType.UnsignedLong:
						return (int)mULongValue;
					default:
						Debug.LogError("Invalid JSON numeric type");
						break;
				}
				return 0;
			}

			set{
				mType = eNumericType.Integer;
				mIntValue = value;
			}
		}

		public uint UintValue
		{
			get{
				switch (Type)
				{
					case eNumericType.Float:
						return (uint)mFloatValue;
					case eNumericType.Integer:
						return (uint)mIntValue;
					case eNumericType.UnsignedInteger:
						return mUintValue;
					case eNumericType.Long:
						return (uint)mLongValue;
					case eNumericType.UnsignedLong:
						return (uint)mULongValue;
					default:
						Debug.LogError("Invalid JSON numeric type");
						break;
				}
				return 0;
			}
			
			set{
				mType = eNumericType.UnsignedInteger;
				mUintValue = value;
			}
		}

		public long LongValue
		{
			get{
				switch (Type)
				{
					case eNumericType.Float:
						return (long)mFloatValue;
					case eNumericType.Integer:
						return mIntValue;
					case eNumericType.UnsignedInteger:
						return (long)mUintValue;
					case eNumericType.Long:
						return mLongValue;
					case eNumericType.UnsignedLong:
						return (long)mULongValue;
					default:
						Debug.LogError("Invalid JSON numeric type");
						break;
				}
				return 0;
			}
			
			set{
				mType = eNumericType.Long;
				mLongValue = value;
			}
		}

		public ulong ULongValue
		{
			get{
				switch (Type)
				{
					case eNumericType.Float:
						return (ulong)mFloatValue;
					case eNumericType.Integer:
						return (ulong)mIntValue;
					case eNumericType.UnsignedInteger:
						return mUintValue;
					case eNumericType.Long:
						return (ulong)mLongValue;
					case eNumericType.UnsignedLong:
						return mULongValue;
					default:
						Debug.LogError("Invalid JSON numeric type");
						break;
				}
				return 0;
			}
			
			set{
				mType = eNumericType.UnsignedLong;
				mULongValue = value;
			}
		}

		public float FloatValue
		{
			get{
				switch (Type)
				{
					case eNumericType.Float:
						return mFloatValue;
					case eNumericType.Integer:
						return (float)mIntValue;
					case eNumericType.UnsignedInteger:
						return (float)mUintValue;
					case eNumericType.Long:
						return (float)mLongValue;
					case eNumericType.UnsignedLong:
						return (float)mULongValue;
					default:
						Debug.LogError("Invalid JSON numeric type");
						break;
				}
				return 0;
			}

			set{
				mType = eNumericType.Float;
				mFloatValue = value;
			}
		}

		#endregion

		#region Methods

		public JSONNumber ()
		{
			mType = eNumericType.Integer;
			mIntValue = 0;
			mUintValue = 0;
			mLongValue = 0;
			mULongValue = 0;
			mFloatValue = 0;
		}

		public JSONNumber (int value)
		{
			IntValue = value;
		}

		public JSONNumber (uint value)
		{
			UintValue = value;
		}

		public JSONNumber (long value)
		{
			LongValue = value;
		}
		
		public JSONNumber (ulong value)
		{
			ULongValue = value;
		}

		public JSONNumber (float value)
		{
			FloatValue = value;
		}

		public JSONNumber (StringBuilder data)
		{
			Decode(data);
		}

		public void Decode(StringBuilder data)
		{
			while (Char.IsWhiteSpace(data[0]))
				data.Remove(0, 1);

			// NOTE: not allowing spaces in a number
			mType = eNumericType.Integer;
			bool isNegative = false;
			ulong ivalue = 0;
			float fvalue = 0;

			if (data[0] == '-')
			{
				isNegative = true;
				data.Remove(0, 1);
			}

			while (Char.IsDigit(data[0]))
			{
				ivalue = ivalue * 10 + (ulong)(data[0] - '0');
				data.Remove(0, 1);
			}

			if (data[0] == '.')
			{
				data.Remove(0, 1);
				if (Char.IsDigit(data[0]))
				{
					mType = eNumericType.Float;
					fvalue = ivalue;
					float fraction = 0.1f;
					while (Char.IsDigit(data[0]))
					{
						fvalue += (data[0] - '0') * fraction;
						fraction *= .1f;
						data.Remove(0, 1);
					}
					mFloatValue = fvalue;
				}
				else
					throw new InvalidJSONException();
			}
			float multiplier = 1.0f;
			if (data[0] == 'e' || data[0] == 'E')
			{
				data.Remove(0, 1);
				int exponential = 0;
				bool isExponentialNegative = false;
				if (data[0] == '+')
					data.Remove(0, 1);
				else if (data[0] == '-')
				{
					data.Remove(0, 1);
					isExponentialNegative = true;
				}
				if (!Char.IsDigit(data[0]))
					throw new InvalidJSONException();
				while (Char.IsDigit(data[0]))
				{
					exponential = exponential * 10 + (data[0] - '0');
					data.Remove(0, 1);
				}
				if (isExponentialNegative)
					exponential = -exponential;
				multiplier = (float)Math.Pow(10.0, (float)exponential);
			}
			if (mType != eNumericType.Float && multiplier >= 1)
			{
				ivalue *= (ulong)multiplier;
				if (!isNegative)
				{
					if (ivalue > uint.MaxValue)
					{
						mType = eNumericType.UnsignedLong;
						mULongValue = ivalue;
					}
					else
					{
						mType = eNumericType.UnsignedInteger;
						mUintValue = (uint)ivalue;
					}
				}
				else
				{
					if (ivalue > int.MaxValue)
					{
						mType = eNumericType.Long;
						mLongValue = (long)ivalue;
						mLongValue = -mLongValue;
					}
					else
					{
						mType = eNumericType.Integer;
						mIntValue = (int)ivalue;
						mIntValue = -mIntValue;
					}
				}
			}
			else
			{
				if (mType != eNumericType.Float)
				{
					mType = eNumericType.Float;
					mFloatValue = (float)ivalue;
				}
				mFloatValue *= multiplier;
				if (isNegative)
					mFloatValue = -mFloatValue;
			}

		}

		public void Encode(StringBuilder data)
		{
			switch (mType)
			{
				case eNumericType.Float:
					data.Append(mFloatValue);
					break;
				case eNumericType.Integer:
					data.Append(mIntValue);
					break;
				case eNumericType.UnsignedInteger:
					data.Append(mUintValue);
					break;
				case eNumericType.Long:
					data.Append(mLongValue);
					break;
				case eNumericType.UnsignedLong:
					data.Append(mULongValue);
					break;
			}
		}

		#endregion

		#region Members

		private eNumericType mType;
		private int mIntValue;
		private uint mUintValue;
		private long mLongValue;
		private ulong mULongValue;
		private float mFloatValue;

		#endregion
	}
}

