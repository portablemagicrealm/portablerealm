//
// MRILocation.cs
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
using System.Collections.Generic;

namespace PortableRealm
{
	
/// <summary>
/// Interface for location a controllable can be, either a clearing or road (if retreating).
/// </summary>
public interface MRILocation : MRISerializable
{
	#region Properties

	GameObject Owner { get; }

	uint Id { get; }

	ICollection<MRRoad> Roads { get; }

	MRTileSide MyTileSide { get; set; }

	MRGamePieceStack Pieces { get; }

	MRGamePieceStack AbandonedItems { get; }

	/// <summary>
	/// Returns a list of the magic colors available in this location.
	/// </summary>
	/// <value>The magic available.</value>
	IList<MRGame.eMagicColor> MagicSupplied { get; }

	#endregion

	#region Methods

	/// <summary>
	/// Returns the road connecting this location to another location, or null if the locations aren't connected.
	/// </summary>
	/// <returns>The road.</returns>
	/// <param name="clearing">Clearing.</param>
	MRRoad RoadTo(MRILocation target);

	/// <summary>
	/// Adds a piece to the top of the location.
	/// </summary>
	/// <param name="piece">the piece</param>
	void AddPieceToTop(MRIGamePiece piece);

	/// <summary>
	/// Removes a piece from the location.
	/// </summary>
	/// <param name="piece">the piece to remove</param>
	void RemovePiece(MRIGamePiece piece);

	#endregion
}
		
}