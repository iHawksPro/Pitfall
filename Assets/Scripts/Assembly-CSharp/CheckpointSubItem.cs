using UnityEngine;

public class CheckpointSubItem : MonoBehaviour
{
	public bool IsVisible { get; set; }

	public void Awake()
	{
		IsVisible = false;
		base.gameObject.GetComponent<Renderer>().enabled = false;
	}

	public void Start()
	{
		base.gameObject.GetComponent<Renderer>().enabled = IsVisible;
	}

	public void OnDisable()
	{
	}

	public void OnEnable()
	{
		base.gameObject.GetComponent<Renderer>().enabled = IsVisible;
	}
}
