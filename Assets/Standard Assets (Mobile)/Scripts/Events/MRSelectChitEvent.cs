//
// MRSelectChitEvent.cs
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

public class MRSelectChitEvent : MRUpdateEvent
{
	#region Constants
	
	public enum eCompare
	{
		LessThan,
		LessThanEqualTo,
		EqualTo,
		GreaterThanEqualTo,
		GreaterThan
	}
	
	#endregion

	#region Callback class for dialog buttons

	public delegate void OnChitSelected(MRActionChit chit);

	#endregion

	#region Filter class for chit selection

	public class MRSelectChitFilter
	{
		#region Properties

		public MRActionChit.eAction Action
		{
			get{
				return mAction;
			}
		}
		
		public MRGame.eStrength Strength
		{
			get{
				return mStrength;
			}
		}
		
		public eCompare StrengthDirection
		{
			get{
				return mStrengthDirection;
			}
		}
		
		public int Speed
		{
			get{
				return mSpeed;
			}
		}
		
		public eCompare SpeedDirection
		{
			get{
				return mSpeedDirection;
			}
		}

		#endregion

		#region Methods

		public MRSelectChitFilter(MRActionChit.eAction action) : this(action, MRGame.eStrength.Any, eCompare.EqualTo,
		                                                              0, eCompare.GreaterThanEqualTo)
		{
		}

		public MRSelectChitFilter(MRActionChit.eAction action, 
		                          MRGame.eStrength strength, eCompare strengthDirection,
		                          int speed, eCompare speedDirection)
		{
			mAction = action;
			mStrength = strength;
			mStrengthDirection = strengthDirection;
			mSpeed = speed;
			mSpeedDirection = speedDirection;
		}

		/// <summary>
		/// Returns if a given chit can be selected.
		/// </summary>
		/// <returns><c>true</c> if the chit is valid; otherwise, <c>false</c>.</returns>
		/// <param name="chit">Chit.</param>
		public bool IsValidSelectChit(MRChit chit)
		{
			bool isValid = false;
			
			MRActionChit action = chit as MRActionChit;
			if (action != null)
			{
				// verify the type
				if (action.CanBeUsedFor(mAction))
				{
					// verify the speed
					if (SpeedDirection.compare(action.CurrentTime, Speed))
					{
						// verify the strength
						MRGame.eStrength strength = MRGame.eStrength.Any;
						if (action is MRFightChit)
							strength = ((MRFightChit)action).CurrentStrength;
						else if (action is MRMoveChit)
							strength = ((MRMoveChit)action).CurrentStrength;
						if (strength != MRGame.eStrength.Any)
						{
							if (StrengthDirection.compare(strength, Strength))
							{
								isValid = true;
							}
						}
					}
				}
			}
			
			return isValid;
		}

		#endregion

		#region Members

		private MRActionChit.eAction mAction;
		private MRGame.eStrength mStrength;
		private eCompare mStrengthDirection;
		private int mSpeed;
		private eCompare mSpeedDirection;

		#endregion
	}

	#endregion

	#region Properties

	public override ePriority Priority 
	{ 
		get {
			return ePriority.SelectChitEvent;
		}
	}

	public MRSelectChitFilter Filter
	{
		get{
			return mFilter;
		}
	}

	public MRActionChit.eAction Action
	{
		get{
			return mFilter.Action;
		}
	}

	public MRGame.eStrength Strength
	{
		get{
			return mFilter.Strength;
		}
	}

	public eCompare StrengthDirection
	{
		get{
			return mFilter.StrengthDirection;
		}
	}

	public int Speed
	{
		get{
			return mFilter.Speed;
		}
	}

	public eCompare SpeedDirection
	{
		get{
			return mFilter.SpeedDirection;
		}
	}

	public MRActionChit SelectedChit
	{
		get{
			return mChit;
		}

		set{
			if (value != null && mFilter.IsValidSelectChit(value))
			{
				mChit = value;
				MRMainUI.TheUI.HideAttackManeuverDialog();
				MRMainUI.TheUI.DisplayInstructionMessage(null);
				MRGame.TheGame.PopView();
			}
		}
	}

	#endregion

	#region Methods

	public MRSelectChitEvent(MRCharacter character, MRActionChit.eAction action, MRGame.eStrength strength,
	                         eCompare strengthDirection, int speed, eCompare speedDirection,
	                         OnChitSelected callback)
	{
		mFilter = new MRSelectChitFilter(action, strength, strengthDirection, speed, speedDirection);
		mCharacter = character;
		mCallback = callback;
		mInitialized = false;
	}

	/// <summary>
	/// Updates this instance.
	/// </summary>
	/// <returns>true if other events in the update loop should be processed this frame, false if not</returns>
	public override bool Update ()
	{
		if (!mInitialized)
		{
			mInitialized = true;
			mCharacter.SelectChitData = this;
			MRGame.TheGame.CharacterMat.Controllable = mCharacter;
			MRGame.TheGame.PushView(MRGame.eViews.SelectChit);
			return false;
		}

		// check if we are done
		if (mInitialized)
		{
			if (MRGame.TheGame.CurrentView != MRGame.eViews.SelectChit)
			{
				MRGame.TheGame.RemoveUpdateEvent(this);
				if (mCallback != null)
					mCallback(mChit);
				return false;
			}
			return false;
		}
		return true;
	}

	#endregion

	#region Members

	private bool mInitialized;
	private MRSelectChitFilter mFilter;
	private MRCharacter mCharacter;
	private MRActionChit mChit;
	private OnChitSelected mCallback;

	#endregion
}

