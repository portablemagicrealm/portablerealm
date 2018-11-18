//
// MRTable.cs
//
// Author:
//       Steve Jakab <>
//
// Copyright (c) 2017 Steve Jakab
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
using System;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

namespace PortableRealm
{
	
public class MRTable
{
	#region Methods

	public MRTable (JSONObject data)
	{
		mColumnMap = new Dictionary<string, int>();
		mTable = new List<List<string>>();

		JSONArray columns = (JSONArray)data["columns"];
		for (int i = 0; i < columns.Count; ++i)
		{
			List<string> column = new List<string>();
			mTable.Add(column);
			JSONObject columnData = (JSONObject)columns[i];
			if (columnData["name"] != null)
			{
				mColumnMap.Add(((JSONString)columnData["name"]).Value, i);
			}
			JSONArray rows = (JSONArray)columnData["rows"];
			for (int j = 0; j < rows.Count; ++j)
			{
				column.Add(((JSONString)rows[j]).Value);
			}
		}
	}

	/// <summary>
	/// Gets a table value.
	/// </summary>
	/// <returns>The value.</returns>
	/// <param name="row">Row index.</param>
	/// <param name="column">Column index.</param>
	public string GetValue(int row, int column)
	{
		String value = null;
		if (column >= 0 && column < mTable.Count)
		{
			List<string> columnData = mTable[column];
			if (row >= 0 && row < columnData.Count)
			{
				value = columnData[row];
			}
		}
		return value;
	}

	/// <summary>
	/// Gets a table value.
	/// </summary>
	/// <returns>The value.</returns>
	/// <param name="row">Row index.</param>
	/// <param name="column">Column name.</param>
	public string GetValue(int row, string column)
	{
		String value = null;
		int columnIndex;
		if (mColumnMap.TryGetValue(column, out columnIndex))
		{
			value = GetValue(row, columnIndex);
		}
		return value;
	}

	#endregion

	#region Members

	private Dictionary<string, int> mColumnMap;
	private List<List<string>> mTable;

	#endregion
}

}