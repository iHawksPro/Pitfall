using UnityEngine;

public class CheckPointBird : MonoBehaviour
{
	private bool mStarted;

	private void Awake()
	{
		mStarted = false;
	}

	private void Start()
	{
		if (!SecureStorage.Instance.SfxMuted)
		{
			base.GetComponent<AudioSource>().enabled = true;
		}
		base.GetComponent<Animation>().Play("Checkpoint_Fly");
		mStarted = false;
	}

	private void Update()
	{
		float distanceMoved = PlayerController.Instance().GetDistanceMoved();
		base.transform.position += PlayerController.Instance().PlayerModel.transform.forward * distanceMoved;
	}

	public bool HasFinished()
	{
		if (!mStarted && base.GetComponent<Animation>().IsPlaying("Checkpoint_Fly"))
		{
			mStarted = true;
		}
		if (mStarted && !base.GetComponent<Animation>().IsPlaying("Checkpoint_Fly"))
		{
			return true;
		}
		return false;
	}
}
