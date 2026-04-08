using System.Collections.Generic;
using UnityEngine;

public class PieceTriggerFunctions : MonoBehaviour
{
	private struct PieceSFX
	{
		public SoundFXData m_sfxData;

		public GameObject m_source;
	}

	private List<PieceSFX> m_loopingSounds = new List<PieceSFX>();

	public void SwitchToShoulderCam(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.ShoulderCam);
	}

	public void SwitchToHeliLeftCam(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.HeliLeftCam);
	}

	public void SwitchToHeliRightCam(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.HeliRightCam);
	}

	public void HeliRightCam_SetSmoothed(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.HeliRightCam.GetComponent<HeliCam>().GoSilky();
	}

	public void HeliRightCam_SetUnsmoothed(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.HeliRightCam.GetComponent<HeliCam>().GoHessian();
	}

	public void SwitchToMineCartCam(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.MineCartCam);
	}

	public void SwitchToBikeCam(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.BikeCam);
	}

	public void SwitchToMountainSlideCam(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.MountainSlideCam);
	}

	public void SwitchToCineCam1(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.CineCam1, CameraTransitionData.JumpCut);
	}

	public void SwitchToSideCam(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.SideCam);
	}

	public void SwitchToGordonsCam(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.GordonsCam);
	}

	public void SwitchToCineCam2(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.CineCam2, CameraTransitionData.JumpCut);
	}

	public void SwitchToCheckpointCam(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.CheckpointCam);
	}

	public void SwitchToBikeHeliCamLeft(PieceTriggerParameters parameters)
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.BikeHeliCamLeft);
	}

	public void SwitchToCutSceneCam(PieceTriggerParameters parameters)
	{
		GameObject gameObject = base.transform.Find("CutSceneCam").gameObject;
		if (gameObject != null)
		{
			CameraBase component = gameObject.GetComponent<CameraBase>();
			component.Target = GameController.Instance.Player.gameObject;
			component.TargetTransform = GameController.Instance.Player.gameObject.transform;
			component.mLookAheadObj = GameController.Instance.Player.gameObject;
			CameraManager.Instance.SwitchToCameraMode(component, new CameraTransitionData(null, TweenFunctions.TweenType.easeInOutCubic, 1f));
			(component as CutSceneCam).Trigger();
		}
	}

	public void ToggleMeteors(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().GenerateMeteors = !LevelGenerator.Instance().GenerateMeteors;
		TBFUtils.DebugLog("-----------------Meteors Toggled--------------------");
	}

	public void CreateSmallExplodingMeteorHorizontal(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.TO_NODE, Meteor.METEOR_TYPE.SMALL, MeteorController.DIRECTION.LEFT_OR_RIGHT, false);
	}

	public void CreateSmallExplodingMeteorVertical(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.TO_NODE, Meteor.METEOR_TYPE.SMALL, MeteorController.DIRECTION.STRAIGHT_DOWN, false);
	}

	public void CreateSmallGeneralMeteorHorizontal(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.THROUGH_NODE, Meteor.METEOR_TYPE.SMALL, MeteorController.DIRECTION.LEFT_OR_RIGHT, false);
	}

	public void CreateSmallGeneralMeteorVertical(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.THROUGH_NODE, Meteor.METEOR_TYPE.SMALL, MeteorController.DIRECTION.LEFT_OR_RIGHT, false);
	}

	public void CreateLargeExplodingMeteorHorizontal(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.TO_NODE, Meteor.METEOR_TYPE.LARGE, MeteorController.DIRECTION.LEFT_OR_RIGHT, false);
	}

	public void CreateLargeExplodingMeteorVertical(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.TO_NODE, Meteor.METEOR_TYPE.LARGE, MeteorController.DIRECTION.STRAIGHT_DOWN, false);
	}

	public void CreateLargeGeneralMeteorHorizontal(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.THROUGH_NODE, Meteor.METEOR_TYPE.LARGE, MeteorController.DIRECTION.LEFT_OR_RIGHT, false);
	}

	public void CreateLargeGeneralMeteorVertical(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.THROUGH_NODE, Meteor.METEOR_TYPE.LARGE, MeteorController.DIRECTION.LEFT_OR_RIGHT, false);
	}

	public void CreateHugeExplodeAndRemoveMeteorHorizontal(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.TO_NODE, Meteor.METEOR_TYPE.HUGE, MeteorController.DIRECTION.LEFT_OR_RIGHT, true);
	}

	public void CreateHugeExplodeAndRemoveMeteorVertical(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.TO_NODE, Meteor.METEOR_TYPE.HUGE, MeteorController.DIRECTION.STRAIGHT_DOWN, true);
	}

	public void CreateHugeExplodingMeteorHorizontal(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.TO_NODE, Meteor.METEOR_TYPE.HUGE, MeteorController.DIRECTION.LEFT_OR_RIGHT, false);
	}

	public void CreateHugeExplodingMeteorVertical(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.TO_NODE, Meteor.METEOR_TYPE.HUGE, MeteorController.DIRECTION.STRAIGHT_DOWN, false);
	}

	public void CreateHugeGeneralMeteorHorizontal(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.THROUGH_NODE, Meteor.METEOR_TYPE.HUGE, MeteorController.DIRECTION.LEFT_OR_RIGHT, false);
	}

	public void CreateHugeGeneralMeteorVertical(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().CreateTargetedMeteor(Meteor.TARGET.THROUGH_NODE, Meteor.METEOR_TYPE.HUGE, MeteorController.DIRECTION.LEFT_OR_RIGHT, false);
	}

	public void SmallQuake(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().DoQuake(0.1f, 3f);
	}

	public void MediumQuake(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().DoQuake(0.15f, 4f);
	}

	public void LargeQuake(PieceTriggerParameters parameters)
	{
		LevelGenerator.Instance().DoQuake(0.2f, 5f);
	}

	public void StopReturnToCentre(PieceTriggerParameters parameters)
	{
		PlayerController.Instance().OverrideReturnToCentre(false);
	}

	public void StartReturnToCentre(PieceTriggerParameters parameters)
	{
		PlayerController.Instance().OverrideReturnToCentre(true);
	}

	public void ChangeTheme(PieceTriggerParameters parameters)
	{
		PlayerController.Instance().StartThemeTransition();
	}

	public void StartAnim1(PieceTriggerParameters parameters)
	{
		Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			if (animation.clip != null)
			{
				animation.Play();
				if (parameters.IsLate)
				{
					animation[animation.clip.name].normalizedTime = 1f;
				}
			}
		}
	}

	public void PlaySFX(PieceTriggerParameters parameters)
	{
		PieceDescriptor component = base.gameObject.GetComponent<PieceDescriptor>();
		if (!(component != null))
		{
			return;
		}
		string param = parameters.Param1;
		SoundFXData sFXDataFromName = EnvironmentSFX.Instance.GetSFXDataFromName(param);
		if (sFXDataFromName == null)
		{
			return;
		}
		GameObject entryAnchor = component.EntryAnchor;
		if (parameters.Param2 != string.Empty)
		{
			Transform transform = component.transform;
			foreach (Transform item2 in transform)
			{
				if (item2.gameObject.name == parameters.Param2)
				{
					entryAnchor = item2.gameObject;
					break;
				}
			}
		}
		SoundManager.Instance.Play(sFXDataFromName, entryAnchor);
		if (sFXDataFromName.m_loop)
		{
			PieceSFX item = new PieceSFX
			{
				m_sfxData = sFXDataFromName,
				m_source = entryAnchor
			};
			m_loopingSounds.Add(item);
		}
	}

	public void StopLoopingSFX()
	{
		PieceDescriptor component = base.gameObject.GetComponent<PieceDescriptor>();
		if (component != null)
		{
			foreach (PieceSFX loopingSound in m_loopingSounds)
			{
				SoundManager.Instance.Stop(loopingSound.m_sfxData, loopingSound.m_source);
			}
		}
		m_loopingSounds.Clear();
	}

	private void OnDestroy()
	{
		StopLoopingSFX();
	}

	public void TrialsModeCrossedLine(PieceTriggerParameters parameters)
	{
		if (GameController.Instance != null && GameController.Instance.IsPlayingTrialsMode)
		{
			GameController.Instance.TrialsModeCrossedLine();
		}
	}
}
