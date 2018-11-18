//
// MRTables.cs
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
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AssemblyCSharp;

namespace PortableRealm
{
	
public class MRTables
{
	#region Properties

	public MRTable this[string tableName]
	{
		get{
			return mTables[tableName];
		}
	}

	#endregion

	#region Methods

	public MRTables()
	{
		try
		{
			TextAsset rawText = (TextAsset)Resources.Load("tables");
			StringBuilder jsonText = new StringBuilder(rawText.text);
			JSONObject jsonData = (JSONObject)JSONDecoder.CreateJSONValue(jsonText);
			readTables(jsonData);
		}
		catch (Exception err)
		{
			Debug.LogError("Error loading tables: " + err);
		}
	}

	/// <summary>
	/// Creates the tables from the JSON data.
	/// </summary>
	/// <param name="tables">Root tables json data.</param>
	private void readTables(JSONObject tables)
	{
		foreach (var data in tables)
		{
			KeyValuePair<string, JSONValue> keyValue = (KeyValuePair<string, JSONValue>)data;
			string tableName = (string)keyValue.Key;
			MRTable table = new MRTable((JSONObject)keyValue.Value);
			mTables.Add(tableName, table);
		}
	}

	#endregion

	#region Members

	private Dictionary<string, MRTable> mTables = new Dictionary<string, MRTable>();

	#endregion
}

}