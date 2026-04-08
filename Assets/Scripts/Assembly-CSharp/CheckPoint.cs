using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	private bool mAnimating;

	private void Start()
	{
		if (!SecureStorage.Instance.SfxMuted)
		{
			base.GetComponent<AudioSource>().enabled = true;
		}
	}

	private void Update()
	{
		base.transform.eulerAngles = new Vector3(0f, base.transform.eulerAngles.y + Time.deltaTime * -360f * 4f, 0f);
	}
}
