using UnityEngine;

public class WorldDebugText : MonoBehaviour
{
	public Camera CameraToAlignTo;

	private TextMesh mTextMesh;

	private float mCharacterSize;

	private void Start()
	{
		mTextMesh = base.gameObject.GetComponent<TextMesh>();
		mCharacterSize = mTextMesh.characterSize;
	}

	private void Update()
	{
		base.transform.forward = CameraToAlignTo.transform.forward;
		Vector3 vector = base.transform.position - CameraToAlignTo.transform.position;
		mTextMesh.characterSize = mCharacterSize * vector.magnitude;
	}
}
