using UnityEngine;

public class CopyPosition : MonoBehaviour
{
	public GameObject SrcObject;

	private void LateUpdate()
	{
		if (SrcObject != null && base.gameObject != null)
		{
			base.gameObject.transform.position = SrcObject.transform.position;
		}
	}
}
