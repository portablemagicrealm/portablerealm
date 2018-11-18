//
// MRRandom.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2015 Steve Jakab
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
using System.Security.Cryptography;
using AssemblyCSharp;

namespace PortableRealm
{
	
/// <summary>
/// Random number generator. Uses Xorshift* from http://en.wikipedia.org/wiki/Xorshift.
/// </summary>
public class MRRandom
{
	#region Properties

	public static ulong seed
	{
		get{
			return mSeed;
		}

		set{
			mSeed = value;
			mRandomizer = mSeed;
			mIndex = 0;
			for (int i = 0; i < mS.Length; ++i)
			{
				mS[i] = xorshift64star();
			}
		}
	}

	#endregion

	#region Methods

	static MRRandom()
	{
		ulong time = (ulong)DateTime.Now.Ticks;
		
		// hash the time to spread out the bits
		MD5 md5 = MD5.Create();
		int arraySize = sizeof(ulong);
		byte[] inArray = new byte[arraySize];
		for (int i = 0; i < arraySize; ++i)
			inArray[i] = (byte)((time >> (i << 3)) & 0x0ff);
		byte[] outArray = md5.ComputeHash(inArray);
		mSeed = 0;
		for (int i = 0; i < arraySize; ++i)
			mSeed |= (ulong)outArray[i] << (i << 3);
		
		seed = mSeed;
	}

	/// <summary>
	/// Returns a random float number between and min [inclusive] and max [inclusive].
	/// </summary>
	/// <param name="min">Min value.</param>
	/// <param name="max">Max value.</param>
	public static float Range(float min, float max)
	{
		float r = ((float)(xorshift1024star())) / ((float)(ulong.MaxValue));
		return min + r * (max - min);
	}

	/// <summary>
	/// Returns a random integer number between min [inclusive] and max [exclusive].
	/// </summary>
	/// <param name="min">Min value.</param>
	/// <param name="max">Max value.</param>
	public static int Range(int min, int max)
	{
		return Range(min, max, false);
	}

	/// <summary>
	/// Returns a random integer number between min [inclusive] and max [exclusive].
	/// </summary>
	/// <param name="min">Min value.</param>
	/// <param name="max">Max value.</param>
	/// <param name="ignoreSequence">Flag to ignore the pregenerated sequence.</param>
	public static int Range(int min, int max, bool ignoreSequence)
	{
		if (!ignoreSequence && mPregeneratedSequence.Count > 0)
		{
			int value = mPregeneratedSequence.Dequeue() - 1;
			Debug.Log("Pregen roll " + value);
			return value;
		}
		else
		{
			float r = ((float)(xorshift1024star())) / ((float)(ulong.MaxValue));
			return min + (int)(r * (max - min));
		}
	}

	private static ulong xorshift1024star()
	{
		ulong s0 = mS[mIndex++];
		mIndex &= 15;
		ulong s1 = mS[mIndex];
		s1 ^= s1 << 31; // a
		s1 ^= s1 >> 11; // b
		s0 ^= s0 >> 30; // c
		mS[mIndex] = s0 ^ s1;
		return mS[mIndex] * 1181783497276652981UL;
	}

	private static ulong xorshift64star()
	{
		mRandomizer ^= mRandomizer >> 12; // a
		mRandomizer ^= mRandomizer << 25; // b
		mRandomizer ^= mRandomizer >> 27; // c
		return mRandomizer * 2685821657736338717UL;
	}

	public static bool Load(JSONObject root)
	{
		if (root["seed"] != null)
			mSeed = ((JSONNumber)root["seed"]).ULongValue;
		if (root["rnd"] != null)
			mRandomizer = ((JSONNumber)root["rnd"]).ULongValue;
		if (root["index"] != null)
			mIndex = ((JSONNumber)root["index"]).IntValue;
		for (int i = 0; i < 16; ++i)
		{
			if (root["s" + i] != null)
				mS[i] = ((JSONNumber)root["s" + i]).ULongValue;
		}
		if (root["sequence"] != null)
		{
			JSONArray sequence = (JSONArray)root["sequence"];
			for (int i = 0; i < sequence.Count; ++i)
			{
				mPregeneratedSequence.Enqueue(((JSONNumber)sequence[i]).IntValue);
			}
		}
		return true;
	}

	public static void Save(JSONObject root)
	{
		root["seed"] = new JSONNumber(mSeed);
		root["rnd"] = new JSONNumber(mRandomizer);
		root["index"] = new JSONNumber(mIndex);
		for (int i = 0; i < 16; ++i)
		{
			root["s" + i] = new JSONNumber(mS[i]);
		}
		if (mPregeneratedSequence.Count > 0)
		{
			JSONArray sequence = new JSONArray(mPregeneratedSequence.Count);
			int[] values = mPregeneratedSequence.ToArray();
			for (int i = 0; i < values.Length; ++i)
			{
				sequence[i] = new JSONNumber(values[i]);
			}
			root["sequence"] = sequence;
		}
	}

	#endregion

	#region Members

	private static ulong mSeed;
	private static ulong mRandomizer;
	private static ulong[] mS = new ulong[16];
	private static int mIndex;
	private static Queue<int> mPregeneratedSequence = new Queue<int>();

	#endregion
}

}