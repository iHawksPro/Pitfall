using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorTutorialHelper : MonoBehaviour
{
	public enum State
	{
		Off = 0,
		TextOn = 1,
		AnimOn = 2,
		AnimOff = 3,
		Ignore = 4
	}

	public enum CurrentSection
	{
		Jump = 0,
		Duck = 1,
		JumpAndDuck = 2,
		Turns = 3,
		Baddies = 4,
		Treasure = 5,
		Complete = 6
	}

	private State state;

	private CurrentSection mSection;

	public GameObject SwipeGraphicUp;

	public GameObject SwipeGraphicDown;

	public GameObject SwipeGraphicLeft;

	public GameObject SwipeGraphicRight;

	public GameObject SwipeGraphicTap;

	public GameObject TiltGraphic;

	public float TutorialSpawnDistanceDefault = 10f;

	public float TutorialTextSpawnDistance = 20f;

	public bool OverideOn;

	private GameObject mSwipeGraphicInstance;

	private bool m_bShowGraphic;

	private List<PieceDescriptor> mPiecDescList = new List<PieceDescriptor>();

	public static LevelGeneratorTutorialHelper instance;

	[HideInInspector]
	public int m_nLastSectionID = -1;

	public void Awake()
	{
		instance = this;
	}

	public void Update()
	{
		if (m_bShowGraphic || state == State.Ignore)
		{
			UpdateGraphicState();
		}
		else
		{
			UpdateCheckingState();
		}
	}

	private void UpdateGraphicState()
	{
		switch (state)
		{
		case State.AnimOn:
			if (!mSwipeGraphicInstance.GetComponentInChildren<Animation>().isPlaying)
			{
				state = State.AnimOff;
			}
			break;
		case State.AnimOff:
			if (!mSwipeGraphicInstance.GetComponentInChildren<Animation>().isPlaying)
			{
				m_bShowGraphic = false;
				mSwipeGraphicInstance.SetActiveRecursively(false);
				mPiecDescList.RemoveAt(0);
				if (mPiecDescList.Count == 0)
				{
					base.enabled = false;
				}
				state = State.Off;
			}
			break;
		case State.Ignore:
			m_bShowGraphic = false;
			mPiecDescList.RemoveAt(0);
			if (mPiecDescList.Count == 0)
			{
				base.enabled = false;
			}
			state = State.Off;
			break;
		}
	}

	private void UpdateCheckingState()
	{
		if (PlayerController.Instance().IsDead())
		{
			return;
		}
		PieceDescriptor pieceDescriptor = LevelGenerator.Instance().GetCurrentPiece();
		PieceDescriptor pieceDescriptor2 = mPiecDescList[0];
		float num = 0f - LevelGenerator.Instance().GetCurrentPathDistTravelled();
		while (pieceDescriptor != pieceDescriptor2 && pieceDescriptor != null)
		{
			if (WorldConstructionHelper.IsHazard(pieceDescriptor.TypeId))
			{
				return;
			}
			num += pieceDescriptor.GetCachedLength();
			pieceDescriptor = pieceDescriptor.GetNextPiece();
		}
		float num2 = ((!(pieceDescriptor2.TutorialSpawnDistanceOveride > 0.001f)) ? TutorialSpawnDistanceDefault : pieceDescriptor2.TutorialSpawnDistanceOveride);
		if (num < TutorialTextSpawnDistance && state == State.Off)
		{
			ShowText(pieceDescriptor2.TutorialTypeId);
		}
		if (num < num2 && state != State.Ignore)
		{
			SetSwipeGraphic(pieceDescriptor2.TutorialTypeId);
			m_bShowGraphic = true;
			m_nLastSectionID = pieceDescriptor2.AuthoredSection;
		}
	}

	private void SetSwipeGraphic(WorldConstructionHelper.PieceTutorialType tutorialType)
	{
		if (mSwipeGraphicInstance != null)
		{
			Object.DestroyImmediate(mSwipeGraphicInstance);
		}
		state = State.AnimOn;
		switch (tutorialType)
		{
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Duck:
			mSwipeGraphicInstance = Object.Instantiate(SwipeGraphicDown) as GameObject;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Jump:
			mSwipeGraphicInstance = Object.Instantiate(SwipeGraphicUp) as GameObject;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_JumpOrDuck:
			mSwipeGraphicInstance = Object.Instantiate(SwipeGraphicUp) as GameObject;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_RopeSwing:
			mSwipeGraphicInstance = Object.Instantiate(SwipeGraphicUp) as GameObject;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Swipe_Left:
			mSwipeGraphicInstance = Object.Instantiate(SwipeGraphicLeft) as GameObject;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Swipe_Right:
			mSwipeGraphicInstance = Object.Instantiate(SwipeGraphicRight) as GameObject;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Baddie:
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Tap:
			mSwipeGraphicInstance = Object.Instantiate(SwipeGraphicTap) as GameObject;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Tilt_Left:
			mSwipeGraphicInstance = Object.Instantiate(TiltGraphic) as GameObject;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Tilt_Right:
			mSwipeGraphicInstance = Object.Instantiate(TiltGraphic) as GameObject;
			break;
		default:
			Debug.LogError("Unknown Tutorial Type in LevelGenertorTutorialHelper");
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_WellDone:
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Finish:
			break;
		}
		mSwipeGraphicInstance.GetComponentInChildren<Animation>().Play("Play");
	}

	public void ShowText(WorldConstructionHelper.PieceTutorialType tutorialType)
	{
		state = State.TextOn;
		switch (tutorialType)
		{
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Duck:
			GameTutorial.Instance.Notify_DuckApproaching();
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Jump:
			GameTutorial.Instance.Notify_JumpApproaching();
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_JumpOrDuck:
		case WorldConstructionHelper.PieceTutorialType.Tutorial_RopeSwing:
			GameTutorial.Instance.Notify_JumpApproaching();
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Swipe_Left:
			GameTutorial.Instance.Notify_TurnApproaching();
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Swipe_Right:
			GameTutorial.Instance.Notify_TurnApproaching();
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Baddie:
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Tap:
			GameTutorial.Instance.Notify_EnemyApproaching();
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Tilt_Left:
			GameTutorial.Instance.Notify_TrackReductionApproaching();
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Tilt_Right:
			GameTutorial.Instance.Notify_TrackReductionApproaching();
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Poison:
			GameTutorial.Instance.Notify_Poison();
			state = State.Ignore;
			mSection++;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_AntiVenom:
			GameTutorial.Instance.Notify_AntiVenom();
			state = State.Ignore;
			mSection++;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_WellDone:
			GameTutorial.Instance.Notify_WellDone();
			state = State.Ignore;
			UpdateSWRVEPass();
			mSection++;
			break;
		case WorldConstructionHelper.PieceTutorialType.Tutorial_Finish:
			GameTutorial.Instance.Notify_Finish();
			state = State.Ignore;
			GameTutorial.Instance.IsEnabled = false;
			PlayerController.Instance().Score().TutorialPlayed();
			SecureStorage.Instance.TutorialViewed = true;
			break;
		default:
			Debug.LogError("Unknown Tutorial Type in LevelGenertorTutorialHelper");
			break;
		}
	}

	public void Reset(bool bResetFromFail)
	{
		base.enabled = false;
		m_bShowGraphic = false;
		mPiecDescList.Clear();
		state = State.Off;
		if (bResetFromFail)
		{
			UpdateSWRVEFail();
		}
		else
		{
			mSection = CurrentSection.Jump;
		}
	}

	public void AddPieceToList(PieceDescriptor pieceDesc)
	{
		mPiecDescList.Add(pieceDesc);
		base.enabled = true;
	}

	public void CheckForTutorialHelper(PieceDescriptor pD)
	{
		if (pD.TutorialTypeId != WorldConstructionHelper.PieceTutorialType.Exclude && (GameTutorial.Instance.IsEnabled || OverideOn))
		{
			GetComponent<LevelGeneratorTutorialHelper>().AddPieceToList(pD);
		}
	}

	public void UpdateSWRVEPass()
	{
		switch (mSection)
		{
		case CurrentSection.Duck:
			SwrveEventsProgression.TutorialDuckCompleted();
			break;
		case CurrentSection.Jump:
			SwrveEventsProgression.TutorialJumpCompleted();
			break;
		case CurrentSection.JumpAndDuck:
			SwrveEventsProgression.TutorialJumpDuckCompleted();
			break;
		case CurrentSection.Turns:
			SwrveEventsProgression.TutorialTurnCompleted();
			break;
		case CurrentSection.Baddies:
			SwrveEventsProgression.TutorialEnemiesCompleted();
			break;
		case CurrentSection.Treasure:
			SwrveEventsProgression.TutorialTreasureCompleted();
			break;
		}
	}

	public void UpdateSWRVEFail()
	{
		string section = "Unknown";
		switch (mSection)
		{
		case CurrentSection.Duck:
			section = "Duck";
			break;
		case CurrentSection.Jump:
			section = "Jump";
			break;
		case CurrentSection.JumpAndDuck:
			section = "JumpAndDuck";
			break;
		case CurrentSection.Turns:
			section = "Turns";
			break;
		case CurrentSection.Baddies:
			section = "Baddies";
			break;
		case CurrentSection.Treasure:
			section = "Treasure";
			break;
		}
		SwrveEventsProgression.TutorialFailed(section);
	}
}
