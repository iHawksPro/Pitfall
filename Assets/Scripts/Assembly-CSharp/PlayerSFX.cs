using System.Collections;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
	public PlayerController Player;

	private SFXBank m_costumeBank;

	public SoundFXData Collect;

	public SoundFXData Jump;

	public MaterialSFX Landing;

	public MaterialSFX Footsteps;

	public SoundFXData Slide;

	public SoundFXData SlideRoll;

	public SoundFXData Whip;

	public SoundFXData Rope;

	public SoundFXData CatStart;

	public SoundFXData CatEnd;

	public SoundFXData CatLoop;

	public SoundFXData BikeStartup;

	public SoundFXData BikeLoop;

	public SoundFXData BikeSlide;

	public SoundFXData BikeTurn;

	public SoundFXData BikeCrash;

	public SoundFXData CartLoop;

	public SoundFXData CartCrash;

	public SoundFXData CartTrackSwitch;

	public SoundFXData CartSpark;

	public SoundFXData CartJump;

	public SoundFXData CartLand;

	public SoundFXData BurstTonicActivate;

	public SoundFXData BurstTonicLoop;

	private PathMaterial GetMaterialUnderPlayer()
	{
		PathMaterial result = PathMaterial.Default;
		if (LevelGenerator.Instance() != null)
		{
			result = LevelGenerator.Instance().GetPathMaterialUnderPlayer();
		}
		return result;
	}

	public void SetCostumeSFX(GameObject sfxPrefab)
	{
		if (m_costumeBank != null)
		{
			Object.Destroy(m_costumeBank.gameObject);
		}
		GameObject gameObject = (GameObject)Object.Instantiate(sfxPrefab);
		m_costumeBank = gameObject.GetComponent<SFXBank>();
		TBFAssert.DoAssert(m_costumeBank != null);
	}

	public void PlayCollect(float Pitch)
	{
		if (Pitch < 0f)
		{
			Pitch = Mathf.Clamp(1f + Pitch * 0.1f, 0.7f, 1f);
		}
		SoundManager.Instance.Play2D(Collect, Pitch);
	}

	public void PlayJump()
	{
		if (Player.GetThemeType() == PlayerTheme.ThemeType.Cart)
		{
			SoundManager.Instance.Play(CartJump, base.gameObject);
		}
		else
		{
			SoundManager.Instance.Play(Jump, base.gameObject);
		}
		m_costumeBank.Play2D("Jump");
	}

	public void PlayLanding()
	{
		if (Player.GetThemeType() == PlayerTheme.ThemeType.Cart)
		{
			SoundManager.Instance.Play(CartLand, base.gameObject);
		}
		else
		{
			Landing.Play(GetMaterialUnderPlayer(), base.gameObject);
		}
		m_costumeBank.Play2D("Landing");
	}

	private void PlayFootstep()
	{
		Footsteps.Play(GetMaterialUnderPlayer(), base.gameObject);
	}

	public void PlayLeftFoot()
	{
		PlayFootstep();
	}

	public void PlayRightFoot()
	{
		PlayFootstep();
	}

	public void PlaySlide(bool IsRolling)
	{
		SoundManager.Instance.Play(Slide, base.gameObject);
		if (IsRolling)
		{
			SoundManager.Instance.Play(SlideRoll, base.gameObject);
		}
	}

	public void PlayDeath(PieceDescriptor.KillType KillType)
	{
		switch (KillType)
		{
		case PieceDescriptor.KillType.Crocodile:
			m_costumeBank.Play2D("CrocDeath");
			break;
		case PieceDescriptor.KillType.Pit:
			m_costumeBank.Play2D("PitDeath");
			break;
		default:
			m_costumeBank.Play2D("WallDeath");
			break;
		}
		switch (Player.GetThemeType())
		{
		case PlayerTheme.ThemeType.Bike:
			BikeRideEnd(0f);
			break;
		case PlayerTheme.ThemeType.Cart:
			CartEnd();
			break;
		}
		m_costumeBank.Stop2D("Poisoned");
	}

	public void RecoverFromDeath()
	{
		switch (Player.GetThemeType())
		{
		case PlayerTheme.ThemeType.Bike:
			BikeRideStart();
			break;
		case PlayerTheme.ThemeType.Cart:
			CartStart();
			break;
		}
	}

	public void PlayWhip()
	{
		SoundManager.Instance.Play(Whip, base.gameObject);
	}

	public void PlayRope()
	{
		SoundManager.Instance.Play(Rope, base.gameObject);
		m_costumeBank.Play2D("Rope");
		if (!TBFUtils.Is256mbDevice())
		{
			HarryVO.Instance.Play2D("RopeSwing", 0.5f);
		}
	}

	public void PlayStylishDismount()
	{
		if (!TBFUtils.Is256mbDevice())
		{
			HarryVO.Instance.Play2D("StylishDismount");
		}
	}

	public void PlaySideStep()
	{
		m_costumeBank.Play2D("Sidestep");
		if (Player.GetThemeType() == PlayerTheme.ThemeType.Cart)
		{
			SoundManager.Instance.Play(CartTrackSwitch, base.gameObject);
		}
	}

	public void Poisoned()
	{
		m_costumeBank.Play2D("Poisoned");
	}

	public void PoisonRecovery()
	{
		m_costumeBank.Stop2D("Poisoned");
		m_costumeBank.Play2D("Recovered");
	}

	public void PlayBaddieKill(BaddieController.Type baddieType)
	{
		if (!TBFUtils.Is256mbDevice())
		{
			switch (baddieType)
			{
			case BaddieController.Type.Snake:
				HarryVO.Instance.Play2D("WhipSnake");
				break;
			case BaddieController.Type.Scorpion:
				HarryVO.Instance.Play2D("WhipScorpion");
				break;
			case BaddieController.Type.Crocodile:
				HarryVO.Instance.Play2D("WhipCroc");
				break;
			}
		}
	}

	public void PlayRunStart()
	{
		if (!GameController.Instance.IsPlayingTrialsMode && !TBFUtils.Is256mbDevice())
		{
			HarryVO.Instance.Play2D("StartRun", 1.5f);
		}
	}

	public void CatRideStart()
	{
		SoundManager.Instance.Play(CatStart, base.gameObject);
		SoundManager.Instance.Play(CatLoop, base.gameObject);
		if (!TBFUtils.Is256mbDevice())
		{
			HarryVO.Instance.Play2D("StartJaguar", 2f);
		}
	}

	public void CatRideEnd()
	{
		SoundManager.Instance.Stop(CatLoop, base.gameObject);
		SoundManager.Instance.Play(CatEnd, base.gameObject);
	}

	public void BikeRideStart()
	{
		SoundManager.Instance.Play(Jump, base.gameObject);
		m_costumeBank.Play2D("Jump");
		SoundManager.Instance.Play(BikeStartup, base.gameObject, 0.9f);
		SoundManager.Instance.Play(BikeLoop, base.gameObject, 0.9f);
		if (!TBFUtils.Is256mbDevice())
		{
			HarryVO.Instance.Play2D("StartBike", 2f);
		}
	}

	public void BikeRideEnd(float delay)
	{
		StartCoroutine(BikeRideEndDelayed(delay));
	}

	private IEnumerator BikeRideEndDelayed(float delay)
	{
		yield return new WaitForSeconds(delay);
		SoundManager.Instance.Stop(BikeLoop, base.gameObject);
		SoundManager.Instance.Play(BikeCrash, base.gameObject);
	}

	public void BikeRideSlide()
	{
		SoundManager.Instance.Play(BikeSlide, base.gameObject);
	}

	public void BikeRideTurn()
	{
		SoundManager.Instance.Play(BikeTurn, base.gameObject);
	}

	public void CartStart()
	{
		SoundManager.Instance.Play(Jump, base.gameObject);
		m_costumeBank.Play2D("Jump");
		SoundManager.Instance.Play(CartLoop, base.gameObject, 0.9f);
		if (!TBFUtils.Is256mbDevice())
		{
			HarryVO.Instance.Play2D("StartMinecart", 2f);
		}
	}

	public void CartEnd()
	{
		SoundManager.Instance.Stop(CartLoop, base.gameObject);
		SoundManager.Instance.Play(CartCrash, base.gameObject);
	}

	public void CartSparks(bool On)
	{
		if (On)
		{
			SoundManager.Instance.Play(CartSpark, base.gameObject);
		}
		else
		{
			SoundManager.Instance.Stop(CartSpark, base.gameObject);
		}
	}

	public void ActivateBurstTonic()
	{
		SoundManager.Instance.Play2D(BurstTonicActivate);
		SoundManager.Instance.Play2D(BurstTonicLoop, 1f, 0.8f);
	}

	public void DeactivateBurstTonic()
	{
		SoundManager.Instance.Stop2D(BurstTonicLoop);
	}

	public void KillSoundLoops()
	{
		m_costumeBank.Stop2D("Poisoned");
		SoundManager.Instance.Stop(CartLoop, base.gameObject);
		SoundManager.Instance.Stop(CartSpark, base.gameObject);
		SoundManager.Instance.Stop(BikeLoop, base.gameObject);
		SoundManager.Instance.Stop(CatLoop, base.gameObject);
		SoundManager.Instance.Stop2D(BurstTonicLoop);
	}
}
