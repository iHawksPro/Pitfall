using UnityEngine;

public class PlayerTrialsBoost
{
	private float m_timeRemaining;

	private bool m_isActive;

	public bool IsActive
	{
		get
		{
			return m_isActive;
		}
	}

	public float TimeRemaining
	{
		get
		{
			return m_timeRemaining;
		}
	}

	public void Reset()
	{
		m_isActive = false;
		m_timeRemaining = 0f;
	}

	public bool CanBeUsed()
	{
		return !PlayerController.Instance().IsDead() && !GameController.Instance.IsPaused && GameController.Instance.IsPlayingTrialsMode && !GameTutorial.Instance.IsEnabled;
	}

	public void Activate()
	{
		if (IsActive)
		{
			return;
		}
		if (CanBeUsed())
		{
			if (m_timeRemaining > 0f)
			{
				m_isActive = true;
			}
			else
			{
				TrialsDataManager instance = TrialsDataManager.Instance;
				if (instance.NumBoostsAvailable > 0)
				{
					instance.UseBoost();
					m_timeRemaining = instance.BoostTime;
					m_isActive = true;
				}
			}
		}
		if (m_isActive)
		{
			StartEffect();
		}
	}

	public void Deactivate()
	{
		m_isActive = false;
		StopEffect();
	}

	public void Update()
	{
		if (m_isActive)
		{
			m_timeRemaining -= Time.deltaTime;
			if (m_timeRemaining <= 0f)
			{
				m_timeRemaining = 0f;
				Deactivate();
			}
		}
	}

	public void StartEffect()
	{
		PlayerController playerController = PlayerController.Instance();
		if (playerController != null)
		{
			playerController.GetPlayerAnimationController().PlayerFXController.PlayHasteEffect();
			playerController.SFX.ActivateBurstTonic();
		}
	}

	public void StopEffect()
	{
		PlayerController playerController = PlayerController.Instance();
		if (playerController != null)
		{
			playerController.GetPlayerAnimationController().PlayerFXController.StopHasteEffect();
			playerController.SFX.DeactivateBurstTonic();
		}
	}
}
