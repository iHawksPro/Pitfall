using UnityEngine;

[AddComponentMenu("TBF/UIScalingButton")]
public class UIScalingButton : MonoBehaviour
{
	public const string LeftComponentName = "Left";

	public const string RightComponentName = "Right";

	public const string MiddleComponentName = "Middle";

	public const float PulseTime = 0.5f;

	public Color color = new Color(1f, 1f, 1f);

	public Material material;

	public SpriteRoot left;

	public SpriteRoot middle;

	public SpriteRoot right;

	private float? lastPulseTime;

	protected virtual void Start()
	{
		SetupReferences();
		Rebuild();
	}

	public void Rebuild()
	{
		RebuildInternal();
	}

	protected virtual void RebuildInternal()
	{
		float num = Mathf.Ceil(right.transform.position.x - left.transform.position.x);
		middle.transform.localScale = new Vector3(Mathf.Ceil(num / middle.width), middle.transform.localScale.y, middle.transform.localScale.z);
		middle.transform.position = new Vector3(left.transform.position.x, left.transform.position.y, left.transform.position.z);
		right.transform.position = new Vector3(right.transform.position.x, left.transform.position.y, right.transform.position.z);
		left.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT;
		right.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
		middle.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
		SetColor(color);
	}

	public SpriteRoot FindSprite(string name)
	{
		Transform transform = base.transform.Find(name);
		if (transform == null)
		{
			Debug.LogWarning("Could not locate the game object '" + name + "', Please ensure it is a child of the composite button");
			return null;
		}
		GameObject gameObject = transform.gameObject;
		SpriteRoot component = gameObject.GetComponent<SpriteRoot>();
		if (component == null)
		{
			Debug.LogWarning("Could not locate the component 'SimpleSprite' in game object '" + name + "'.");
			return null;
		}
		return component;
	}

	protected virtual void SetupReferences()
	{
		if (left == null)
		{
			left = FindSprite("Left");
		}
		if (middle == null)
		{
			middle = FindSprite("Middle");
		}
		if (right == null)
		{
			right = FindSprite("Right");
		}
	}

	public virtual void SetColor(Color color)
	{
		left.SetColor(color);
		right.SetColor(color);
		middle.SetColor(color);
		this.color = color;
	}

	public virtual void Hide(bool isHidden)
	{
		left.Hide(isHidden);
		right.Hide(isHidden);
		middle.Hide(isHidden);
	}

	public void Pulse()
	{
		if (!lastPulseTime.HasValue || Time.realtimeSinceStartup > lastPulseTime.Value + 0.5f)
		{
			SpriteRoot[] array = new SpriteRoot[3] { left, middle, right };
			foreach (SpriteRoot spriteRoot in array)
			{
				iTween.ColorFrom(spriteRoot.gameObject, iTween.Hash("color", Color.white, "time", 0.25f, "easetype", iTween.EaseType.easeInBounce, "ignoretimescale", true));
			}
			iTween.PunchScale(base.gameObject, iTween.Hash("amount", new Vector3(0.15f, 0.15f, 0f), "time", 0.5f, "ignoretimescale", true));
			lastPulseTime = Time.realtimeSinceStartup;
		}
	}
}
