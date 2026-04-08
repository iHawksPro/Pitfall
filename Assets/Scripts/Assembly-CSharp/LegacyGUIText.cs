using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[DisallowMultipleComponent]
public class LegacyGUIText : MonoBehaviour
{
	[SerializeField]
	private string mText = string.Empty;

	[SerializeField]
	private TextAnchor mAnchor = TextAnchor.UpperLeft;

	[SerializeField]
	private TextAlignment mAlignment = TextAlignment.Left;

	[SerializeField]
	private Vector2 mPixelOffset = Vector2.zero;

	[SerializeField]
	private Material mMaterial;

	private Canvas mCanvas;

	private RectTransform mRectTransform;

	private Text mTextComponent;

	public string text
	{
		get
		{
			return mText;
		}
		set
		{
			mText = value ?? string.Empty;
			Apply();
		}
	}

	public TextAnchor anchor
	{
		get
		{
			return mAnchor;
		}
		set
		{
			mAnchor = value;
			Apply();
		}
	}

	public TextAlignment alignment
	{
		get
		{
			return mAlignment;
		}
		set
		{
			mAlignment = value;
			Apply();
		}
	}

	public Vector2 pixelOffset
	{
		get
		{
			return mPixelOffset;
		}
		set
		{
			mPixelOffset = value;
			Apply();
		}
	}

	public Material material
	{
		get
		{
			EnsureComponents();
			EnsureMaterial();
			ApplyVisuals();
			return mMaterial;
		}
		set
		{
			mMaterial = value;
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
		ApplyVisuals();
		ApplyLayout();
	}

	private void EnsureComponents()
	{
		if (mCanvas == null)
		{
			Transform transform2 = transform.Find("__LegacyGUITextCanvas");
			GameObject gameObject = ((transform2 != null) ? transform2.gameObject : new GameObject("__LegacyGUITextCanvas"));
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
		if (mTextComponent == null)
		{
			Transform transform3 = mCanvas.transform.Find("__LegacyGUITextLabel");
			GameObject gameObject2 = ((transform3 != null) ? transform3.gameObject : new GameObject("__LegacyGUITextLabel"));
			gameObject2.transform.SetParent(mCanvas.transform, false);
			mRectTransform = gameObject2.GetComponent<RectTransform>();
			if (mRectTransform == null)
			{
				mRectTransform = gameObject2.AddComponent<RectTransform>();
			}
			mTextComponent = gameObject2.GetComponent<Text>();
			if (mTextComponent == null)
			{
				mTextComponent = gameObject2.AddComponent<Text>();
			}
			mTextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
			mTextComponent.verticalOverflow = VerticalWrapMode.Overflow;
			mTextComponent.raycastTarget = false;
		}
		if (mTextComponent.font == null)
		{
			mTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		}
	}

	private void EnsureMaterial()
	{
		if (mMaterial == null)
		{
			Shader shader = Shader.Find("UI/Default");
			if (shader == null)
			{
				shader = Shader.Find("Sprites/Default");
			}
			mMaterial = new Material(shader);
			mMaterial.color = Color.white;
		}
	}

	private void Apply()
	{
		EnsureComponents();
		EnsureMaterial();
		ApplyLayout();
		ApplyVisuals();
	}

	private void ApplyLayout()
	{
		if (mRectTransform == null)
		{
			return;
		}
		Vector3 position = transform.position;
		Vector2 vector = new Vector2(Mathf.Clamp01(position.x), Mathf.Clamp01(position.y));
		Vector2 pivot = PivotForAnchor(mAnchor);
		mRectTransform.anchorMin = vector;
		mRectTransform.anchorMax = vector;
		mRectTransform.pivot = pivot;
		mRectTransform.anchoredPosition = mPixelOffset;
		mRectTransform.sizeDelta = new Vector2(1200f, 200f);
	}

	private void ApplyVisuals()
	{
		if (mTextComponent == null)
		{
			return;
		}
		mTextComponent.text = mText;
		mTextComponent.alignment = AnchorForAlignment(mAnchor, mAlignment);
		mTextComponent.material = mMaterial;
		mTextComponent.color = ((mMaterial != null) ? mMaterial.color : Color.white);
	}

	private static Vector2 PivotForAnchor(TextAnchor anchor)
	{
		switch (anchor)
		{
		case TextAnchor.UpperLeft:
			return new Vector2(0f, 1f);
		case TextAnchor.UpperCenter:
			return new Vector2(0.5f, 1f);
		case TextAnchor.UpperRight:
			return new Vector2(1f, 1f);
		case TextAnchor.MiddleLeft:
			return new Vector2(0f, 0.5f);
		case TextAnchor.MiddleCenter:
			return new Vector2(0.5f, 0.5f);
		case TextAnchor.MiddleRight:
			return new Vector2(1f, 0.5f);
		case TextAnchor.LowerLeft:
			return new Vector2(0f, 0f);
		case TextAnchor.LowerCenter:
			return new Vector2(0.5f, 0f);
		case TextAnchor.LowerRight:
			return new Vector2(1f, 0f);
		default:
			return new Vector2(0f, 1f);
		}
	}

	private static TextAnchor AnchorForAlignment(TextAnchor anchor, TextAlignment alignment)
	{
		int num = 0;
		switch (anchor)
		{
		case TextAnchor.MiddleLeft:
		case TextAnchor.MiddleCenter:
		case TextAnchor.MiddleRight:
			num = 1;
			break;
		case TextAnchor.LowerLeft:
		case TextAnchor.LowerCenter:
		case TextAnchor.LowerRight:
			num = 2;
			break;
		}
		int num2 = 0;
		switch (alignment)
		{
		case TextAlignment.Center:
			num2 = 1;
			break;
		case TextAlignment.Right:
			num2 = 2;
			break;
		}
		return num switch
		{
			0 => (TextAnchor)num2,
			1 => (TextAnchor)(num2 + 3),
			2 => (TextAnchor)(num2 + 6),
			_ => TextAnchor.UpperLeft,
		};
	}
}
