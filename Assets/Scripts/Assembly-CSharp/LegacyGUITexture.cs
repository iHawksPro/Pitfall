using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[DisallowMultipleComponent]
public class LegacyGUITexture : MonoBehaviour
{
	[SerializeField]
	private Texture mTexture;

	[SerializeField]
	private Color mColor = Color.white;

	private Canvas mCanvas;

	private RectTransform mRectTransform;

	private RawImage mRawImage;

	public Texture texture
	{
		get
		{
			return mTexture;
		}
		set
		{
			mTexture = value;
			Apply();
		}
	}

	public Color color
	{
		get
		{
			return mColor;
		}
		set
		{
			mColor = value;
			Apply();
		}
	}

	private void Awake()
	{
		Apply();
	}

	private void OnEnable()
	{
		Apply();
	}

	private void OnValidate()
	{
		Apply();
	}

	private void LateUpdate()
	{
		Apply();
	}

	private void EnsureComponents()
	{
		if (mCanvas == null)
		{
			Transform transform2 = transform.Find("__LegacyGUITextureCanvas");
			GameObject gameObject = ((transform2 != null) ? transform2.gameObject : new GameObject("__LegacyGUITextureCanvas"));
			gameObject.transform.SetParent(transform, false);
			mCanvas = gameObject.GetComponent<Canvas>();
			if (mCanvas == null)
			{
				mCanvas = gameObject.AddComponent<Canvas>();
			}
			mCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			mCanvas.sortingOrder = 32000;
			CanvasScaler canvasScaler = gameObject.GetComponent<CanvasScaler>();
			if (canvasScaler == null)
			{
				canvasScaler = gameObject.AddComponent<CanvasScaler>();
			}
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
		}
		if (mRawImage == null)
		{
			Transform transform3 = mCanvas.transform.Find("__LegacyGUITextureImage");
			GameObject gameObject2 = ((transform3 != null) ? transform3.gameObject : new GameObject("__LegacyGUITextureImage"));
			gameObject2.transform.SetParent(mCanvas.transform, false);
			mRectTransform = gameObject2.GetComponent<RectTransform>();
			if (mRectTransform == null)
			{
				mRectTransform = gameObject2.AddComponent<RectTransform>();
			}
			mRawImage = gameObject2.GetComponent<RawImage>();
			if (mRawImage == null)
			{
				mRawImage = gameObject2.AddComponent<RawImage>();
			}
			mRawImage.raycastTarget = false;
		}
	}

	private void Apply()
	{
		EnsureComponents();
		mRectTransform.anchorMin = Vector2.zero;
		mRectTransform.anchorMax = Vector2.one;
		mRectTransform.offsetMin = Vector2.zero;
		mRectTransform.offsetMax = Vector2.zero;
		mRawImage.texture = mTexture;
		mRawImage.color = mColor;
	}
}
