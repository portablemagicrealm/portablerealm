//
// MRITouchable.cs
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
using System.Collections;

public interface MRITouchable
{
	/// <summary>
	/// Called when the object is touched.
	/// </summary>
	/// <param name="touchedObject">Touched object.</param>
	/// <returns>true if the event was handled, false if not</returns>
	bool OnTouched(GameObject touchedObject);

	/// <summary>
	/// Called when the object has stopped being touched (not hte same as being tapped)
	/// </summary>
	/// <param name="touchedObject">Released object.</param>
	/// <returns>true if the event was handled, false if not</returns>
	bool OnReleased(GameObject touchedObject);

	/// <summary>
	/// Called when the object is single-tapped/clicked.
	/// </summary>
	/// <param name="touchedObject">Touched object.</param>
	/// <returns>true if the event was handled, false if not</returns>
	bool OnSingleTapped(GameObject touchedObject);

	/// <summary>
	/// Called when the object is double-tapped/clicked.
	/// </summary>
	/// <param name="touchedObject">Touched object.</param>
	/// <returns>true if the event was handled, false if not</returns>
	bool OnDoubleTapped(GameObject touchedObject);

	/// <summary>
	/// Called when the object is touched and held.
	/// </summary>
	/// <param name="touchedObject">Touched object.</param>
	/// <returns>true if the event was handled, false if not</returns>
	bool OnTouchHeld(GameObject touchedObject);
}

