using System.Collections;
using UnityEngine;

public class PlayerFXController : MonoBehaviour
{
	public enum SlideDirection
	{
		eFaceFirst = 0,
		eLegsFirst = 1
	}

	public GameObject FootFallPrefab_Dust;

	public GameObject FootFallPrefab_Water;

	private GameObject mFootFallDustPrefab_Instance;

	private GameObject mFootFallWaterPrefab_Instance;

	private ParticleSystem m_PSDust;

	private ParticleSystem m_PSWater;

	public GameObject FootSlidePrefab_Dust;

	public GameObject FootSlidePrefab_Mud;

	public GameObject FootSlidePrefab_WaterSplash;

	public GameObject FootSlidePrefab_Stones;

	public GameObject FootSlidePrefab_Grass;

	private Transform mFootTransform;

	public GameObject JaguarPfxPrefab;

	private GameObject mJaguarEffectInstance;

	public GameObject WhipPfxPrefab;

	private GameObject mWhipEffectInstance;

	private GameObject mSlideParticleInstance;

	private GameObject mSlideParticleInstanceWaterAdditional;

	public GameObject CoinCollect;

	public GameObject SpiritPrefab;

	private GameObject mSpiritInstance;

	public GameObject HastePrefab;

	private GameObject mHasteInstance;

	public GameObject AntiVenomPrefab;

	private GameObject mAntiVenomInstance;

	private SlideDirection mSlideDirection;

	private void Update()
	{
		if (mSlideParticleInstance != null)
		{
			Transform transform = ((mSlideDirection != SlideDirection.eFaceFirst) ? PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Pelvis") : PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Spine"));
			mSlideParticleInstance.transform.position = transform.position;
			if (mSlideParticleInstanceWaterAdditional != null)
			{
				mSlideParticleInstanceWaterAdditional.transform.position = transform.position;
			}
		}
		if (mHasteInstance != null)
		{
			Transform transform2 = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Pelvis");
			mHasteInstance.transform.position = transform2.position;
			mHasteInstance.transform.rotation = PlayerController.Instance().transform.rotation;
		}
		if (mSpiritInstance != null)
		{
			Transform transform3 = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Pelvis");
			mSpiritInstance.transform.position = transform3.position;
			mSpiritInstance.transform.rotation = PlayerController.Instance().transform.rotation;
		}
		if (mJaguarEffectInstance != null)
		{
			Transform transform4 = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Pelvis");
			mJaguarEffectInstance.transform.position = transform4.position;
			mJaguarEffectInstance.transform.rotation = PlayerController.Instance().transform.rotation;
		}
		if (mAntiVenomInstance != null)
		{
			Transform transform5 = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Pelvis");
			mAntiVenomInstance.transform.position = transform5.position;
			mAntiVenomInstance.transform.rotation = PlayerController.Instance().transform.rotation;
		}
	}

	public void CreateFootstep()
	{
		if (TBFUtils.Is256mbDevice())
		{
			return;
		}
		if (mFootTransform == null)
		{
			mFootTransform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Footsteps");
		}
		Vector3 position = mFootTransform.position;
		if (mFootFallDustPrefab_Instance == null)
		{
			mFootFallDustPrefab_Instance = Object.Instantiate(FootFallPrefab_Dust, position, Quaternion.identity) as GameObject;
			m_PSDust = (ParticleSystem)mFootFallDustPrefab_Instance.GetComponent(typeof(ParticleSystem));
		}
		if (mFootFallWaterPrefab_Instance == null)
		{
			mFootFallWaterPrefab_Instance = Object.Instantiate(FootFallPrefab_Water, position, Quaternion.identity) as GameObject;
			m_PSWater = (ParticleSystem)mFootFallWaterPrefab_Instance.GetComponent(typeof(ParticleSystem));
		}
		switch (LevelGenerator.Instance().GetPathMaterialUnderPlayer())
		{
		case PathMaterial.Mud1:
		case PathMaterial.Stones1:
		case PathMaterial.Dust1:
		case PathMaterial.Grass1:
			if (mFootFallDustPrefab_Instance != null)
			{
				if (!m_PSDust.IsAlive())
				{
					m_PSDust.Play();
				}
				mFootFallDustPrefab_Instance.transform.position = position;
			}
			break;
		case PathMaterial.Water1:
			if (mFootFallWaterPrefab_Instance != null)
			{
				if (!m_PSWater.IsAlive())
				{
					m_PSWater.Play();
				}
				mFootFallWaterPrefab_Instance.transform.position = position;
			}
			break;
		case PathMaterial.Default:
		{
			PieceDescriptor currentPiece = LevelGenerator.Instance().GetCurrentPiece();
			switch (currentPiece.Theme)
			{
			case WorldConstructionHelper.Theme.Cave:
				break;
			case WorldConstructionHelper.Theme.Jungle:
			case WorldConstructionHelper.Theme.Mountain:
				if (mFootFallDustPrefab_Instance != null)
				{
					if (!m_PSDust.IsAlive())
					{
						m_PSDust.Play();
					}
					mFootFallDustPrefab_Instance.transform.position = position;
				}
				break;
			}
			break;
		}
		case PathMaterial.None:
			break;
		}
	}

	public void StartSlideEffect(SlideDirection eDirection)
	{
		if (!(mSlideParticleInstance == null))
		{
			return;
		}
		mSlideDirection = eDirection;
		PathMaterial pathMaterialUnderPlayer = LevelGenerator.Instance().GetPathMaterialUnderPlayer();
		Transform transform = ((mSlideDirection != SlideDirection.eFaceFirst) ? PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Pelvis") : PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Spine"));
		Vector3 position = transform.position;
		if ((bool)LevelGenerator.Instance() && LevelGenerator.Instance().GetCurrentPiece() != null)
		{
			position.y = LevelGenerator.Instance().GetPathCentreAtCurrentPoint().y;
		}
		switch (pathMaterialUnderPlayer)
		{
		case PathMaterial.Mud1:
			mSlideParticleInstance = (GameObject)Object.Instantiate(FootSlidePrefab_Mud, position, Quaternion.identity);
			break;
		case PathMaterial.Dust1:
			mSlideParticleInstance = (GameObject)Object.Instantiate(FootSlidePrefab_Dust, position, Quaternion.identity);
			break;
		case PathMaterial.Water1:
			if (mSlideParticleInstanceWaterAdditional == null)
			{
				mSlideParticleInstanceWaterAdditional = (GameObject)Object.Instantiate(FootSlidePrefab_WaterSplash, position, Quaternion.identity);
			}
			break;
		case PathMaterial.Grass1:
			mSlideParticleInstance = (GameObject)Object.Instantiate(FootSlidePrefab_Grass, position, Quaternion.identity);
			break;
		case PathMaterial.Stones1:
			mSlideParticleInstance = (GameObject)Object.Instantiate(FootSlidePrefab_Stones, position, Quaternion.identity);
			break;
		case PathMaterial.Default:
		{
			PieceDescriptor currentPiece = LevelGenerator.Instance().GetCurrentPiece();
			switch (currentPiece.Theme)
			{
			case WorldConstructionHelper.Theme.Cave:
				mSlideParticleInstance = null;
				break;
			case WorldConstructionHelper.Theme.Jungle:
				mSlideParticleInstance = (GameObject)Object.Instantiate(FootSlidePrefab_Mud, position, Quaternion.identity);
				break;
			case WorldConstructionHelper.Theme.Mountain:
				mSlideParticleInstance = (GameObject)Object.Instantiate(FootSlidePrefab_Dust, position, Quaternion.identity);
				break;
			case WorldConstructionHelper.Theme.SlippedMountain:
				mSlideParticleInstance = (GameObject)Object.Instantiate(FootSlidePrefab_Stones, position, Quaternion.identity);
				break;
			}
			break;
		}
		case PathMaterial.None:
			break;
		}
	}

	public void StopSlideEffect()
	{
		if (mSlideParticleInstance != null)
		{
			Object.Destroy(mSlideParticleInstance);
			if (mSlideParticleInstanceWaterAdditional != null)
			{
				Object.Destroy(mSlideParticleInstanceWaterAdditional);
			}
		}
	}

	public void StartJaguarEffect()
	{
		if (mJaguarEffectInstance == null && JaguarPfxPrefab != null)
		{
			mJaguarEffectInstance = Object.Instantiate(JaguarPfxPrefab) as GameObject;
		}
	}

	public void EndJaguarEffect()
	{
		if (mJaguarEffectInstance != null)
		{
			Object.Destroy(mJaguarEffectInstance);
		}
	}

	public void ShowWhipEffectInit()
	{
		if (mWhipEffectInstance == null)
		{
			StartCoroutine(ShowWhipEffect());
		}
	}

	protected IEnumerator ShowWhipEffect()
	{
		yield return new WaitForSeconds(0.1333f);
		if (WhipPfxPrefab != null)
		{
			Transform playerAnimatedTransform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bone024");
			mWhipEffectInstance = Object.Instantiate(WhipPfxPrefab) as GameObject;
			mWhipEffectInstance.transform.position = playerAnimatedTransform.position;
			ParticleSystem ps = mWhipEffectInstance.GetComponent<ParticleSystem>();
			ps.Play();
			Object.Destroy(mWhipEffectInstance, ps.duration);
		}
	}

	public void PlayCoinCollectEffect()
	{
		if (CoinCollect != null)
		{
			CoinCollect.GetComponent<Animation>().Play("Take 001");
		}
	}

	public void PlaySpiritEffect()
	{
		if (mSpiritInstance == null && SpiritPrefab != null)
		{
			mSpiritInstance = Object.Instantiate(SpiritPrefab) as GameObject;
		}
	}

	public void StopSpiritEffect()
	{
		if (mSpiritInstance != null)
		{
			Object.Destroy(mSpiritInstance);
		}
	}

	public void PlayHasteEffect()
	{
		if (mHasteInstance == null && HastePrefab != null)
		{
			mHasteInstance = Object.Instantiate(HastePrefab) as GameObject;
		}
	}

	public void StopHasteEffect()
	{
		if (mHasteInstance != null)
		{
			Object.Destroy(mHasteInstance);
			mHasteInstance = null;
		}
	}

	public void PlayAntiVenomEffect()
	{
		if (mAntiVenomInstance == null && AntiVenomPrefab != null)
		{
			mAntiVenomInstance = Object.Instantiate(AntiVenomPrefab) as GameObject;
		}
	}

	public void StopAntiVenumEffect()
	{
		if (mHasteInstance != null)
		{
			Object.Destroy(mHasteInstance);
		}
	}
}
