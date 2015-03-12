//
// MRDiePool.cs
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

public class MRDiePool
{
	#region Properties

	// Returns the standard 2-die pool. Callers should not modify the pool, 
	// call NewDicePool if the values need to be modified.
	public static MRDiePool DefaultPool
	{
		get{
			if (msDefaultPool == null)
			{
				msDefaultPool = new MRDiePool();
				msDefaultPool.NumDice = 2;
				msDefaultPool.DieMod = 0;
				msDefaultPool.ClampLow = true;
				msDefaultPool.ClampHigh = true;
			}
			return msDefaultPool;
		}
	}

	// Returns a new generic 2-die pool
	public static MRDiePool NewDicePool
	{
		get{
			MRDiePool pool = new MRDiePool();
			pool.NumDice = 2;
			pool.DieMod = 0;
			pool.ClampLow = true;
			pool.ClampHigh = true;
			return pool;
		}
	}

	// Returns a new generic 1-die pool
	public static MRDiePool NewDiePool
	{
		get{
			MRDiePool pool = new MRDiePool();
			pool.NumDice = 1;
			pool.DieMod = 0;
			pool.ClampLow = true;
			pool.ClampHigh = true;
			return pool;
		}
	}

	public int[] DieRolls
	{
		get{
			return mDieRolls;
		}
	}

	public int Roll
	{
		get{
			return mRoll;
		}
	}

	public bool RollReady
	{
		get{
			return mRollReady;
		}
	}

	#endregion

	#region Methods

	// use the static properties to create a die pool
	private MRDiePool()
	{
	}

	public void RollDice()
	{
		//todo: add "manual" roll with animated dice
		RollDiceNow();
	}

	/// <summary>
	/// Rolls the dice immediately, without showing any animation or results
	/// </summary>
	public void RollDiceNow()
	{
		mRoll = 0;
		mDieRolls = new int[NumDice];
		for (int i = 0; i < NumDice; ++i)
		{
			mDieRolls[i] = Random.Range(0, 6) + 1;
			if (mDieRolls[i] > mRoll)
				mRoll = mDieRolls[i];
		}
		mRoll += DieMod;
		Debug.Log("Die roll = " + mRoll);
		if (ClampLow && mRoll < 1)
			mRoll = 1;
		if (ClampHigh && mRoll > 6)
			mRoll = 6;
		mRollReady = true;
	}

	#endregion

	#region Members

	public int NumDice;
	public int DieMod;
	public bool ClampLow;
	public bool ClampHigh;

	private int mRoll;
	private int[] mDieRolls;
	private bool mRollReady;

	private static MRDiePool msDefaultPool = null;

	#endregion
}

