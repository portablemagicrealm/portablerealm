//
// MRTradeItem.cs
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
using UnityEngine.UI;
using System;
using System.Collections;

namespace PortableRealm
{
	
public class MRTradeItem : MonoBehaviour, MRITouchable
{
	#region Properties

	public Image itemImage;
	public Text itemName;
	public Text priceText;

	public MRItem Item
	{
		get {
			return mItem;
		}

		set {
			mItem = value;
			itemName.text = mItem.Name.DisplayName();
		}
	}

	public int Price
	{
		get{
			return mPrice;
		}

		set{
			mPrice = value;
			priceText.text = mPrice.ToString();
		}
	}

	public Camera ItemCamera
	{
		set {
			mItemCamera = value;
		}
	}

	public MRGamePieceStack ItemSnapshotStack
	{
		set {
			mItemSnapshotStack = value;
		}
	}

	#endregion

	#region Methods

	public MRTradeItem ()
	{
	}

	void Start()
	{
		mCreatedTexture = false;
	}

	void Update()
	{
		if (!mCreatedTexture && mItem != null && mItemCamera != null)
		{
			StartCoroutine(RenderItem());
		}
	}

	/// <summary>
	/// The scroll view won't clip non-ui items in its view, so we render the item to a texture and apply that to our image.
	/// </summary>
	/// <returns>The item.</returns>
	private IEnumerator RenderItem()
	{
		// we need to wait until we're not rendering to the screen
		yield return new WaitForEndOfFrame();

		// create the texture to render to
		int width = itemImage.sprite.texture.width;
		int height = itemImage.sprite.texture.height;
		RenderTexture rt = new RenderTexture(width, height, 24);
		rt.Create();

		// set up the camera and render the item image to the texture
		MRGamePieceStack itemOrgStack = mItem.Stack;
		mItemSnapshotStack.AddPieceToTop(mItem);
		int cameraOrgMask = mItemCamera.cullingMask;
		float cameraOrgSize = mItemCamera.orthographicSize;
		float cameraOrgAspect = mItemCamera.aspect;
		Vector3 orgPosition = new Vector3(mItemCamera.transform.position.x, mItemCamera.transform.position.y, mItemCamera.transform.position.z);
		Vector3 newPosition = mItem.Position + new Vector3(0, 0, -1);
		mItemCamera.transform.position = newPosition;
		mItemCamera.cullingMask = 1 << mItem.Layer;
		mItemCamera.aspect = 1.0f;
		mItemCamera.orthographicSize = mItemCamera.WorldToViewportPoint(mItem.Bounds.extents).y;
		mItemCamera.targetTexture = rt;
		mItemCamera.Render();

		// copy the rendered texture to our UI image
		RenderTexture.active = rt;
		Texture2D texture = new Texture2D(width, height);
		texture.ReadPixels(new Rect(0,0,width,height), 0, 0, false);
		texture.Apply();
		RenderTexture.active = null;
		itemImage.sprite = Sprite.Create(texture, new Rect(0,0,width,height), new Vector2(0,0));

		// clean up
		rt.Release();
		mItemCamera.targetTexture = null;
		mItemCamera.transform.position = orgPosition;
		mItemCamera.cullingMask = cameraOrgMask;
		mItemCamera.orthographicSize = cameraOrgSize;
		mItemCamera.aspect = cameraOrgAspect;
		mItemCamera = null;
		if (itemOrgStack != null)
		{
			itemOrgStack.AddPieceToTop(mItem);
			itemOrgStack.SortBySize();	
		}
		else if (mItem.StartStack != null)
		{
			mItem.StartStack.AddPieceToTop(mItem);
			mItem.StartStack.SortBySize();
		}
		mCreatedTexture = true;
	}

	public bool OnTouched(GameObject touchedObject)
	{
		return true;
	}

	public bool OnReleased(GameObject touchedObject)
	{
		return true;
	}

	public bool OnSingleTapped(GameObject touchedObject)
	{
		//Debug.Log("Trade " + mItem.Name + " tapped ");
		SendMessageUpwards("OnItemSelected", this, SendMessageOptions.DontRequireReceiver);
		return true;
	}

	public bool OnDoubleTapped(GameObject touchedObject)
	{
		return true;
	}

	public bool OnTouchHeld(GameObject touchedObject)
	{
		return true;
	}

	public bool OnTouchMove(GameObject touchedObject, float delta_x, float delta_y)
	{
		//Debug.Log("Trade " + mItem.Name + " slide " + delta_x + " " + delta_y);
		return true;
	}

	public bool OnButtonActivate(GameObject touchedObject)
	{
		return true;
	}

	public bool OnPinchZoom(float pinchDelta)
	{
		return true;
	}

	#endregion

	#region Members

	private MRItem mItem;
	private int mPrice;
	private bool mCreatedTexture;
	private Camera mItemCamera;
	private MRGamePieceStack mItemSnapshotStack;

	#endregion
}
		
}