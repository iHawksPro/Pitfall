using System.Collections.Generic;
using UnityEngine;

public class AuthoredLevelLayoutController : MonoBehaviour
{
	public string AuthoredLevelName = string.Empty;

	public bool DebugMenu;

	public int SpeedMinimum;

	public int SpeedMaximum;

	public int OccurenceTimer;

	public List<AuthoredLevelLayoutSection> SectionList = new List<AuthoredLevelLayoutSection>();

	public bool SetPiece;

	public bool AuthoredSectionsActive;

	public bool TestWorld;

	public int TestSection = -1;

	private bool mReady;

	private float mAvailabilityTimer;

	private int mLap;

	private void Awake()
	{
		if (SetPiece)
		{
			base.gameObject.name = "SetPiece-" + AuthoredLevelName;
		}
		else
		{
			base.gameObject.name = "AuthoredLevel-" + AuthoredLevelName;
		}
		LevelGenerator levelGenerator = LevelGenerator.Instance();
		if (levelGenerator != null)
		{
			mReady = false;
		}
		else if (TestWorld)
		{
			Object.Destroy(base.gameObject);
			Application.LoadLevel(SceneNameResolver.Resolve("title"));
		}
	}

	private void Start()
	{
		Reset();
	}

	private void Update()
	{
		MakeReady();
		if (mAvailabilityTimer > 0f)
		{
			mAvailabilityTimer -= Time.deltaTime;
		}
	}

	public void MakeReady()
	{
		if (mReady)
		{
			return;
		}
		PieceSet loadedPieces = ThemeManager.Instance.LoadedPieces;
		bool flag = true;
		foreach (AuthoredLevelLayoutSection section in SectionList)
		{
			flag &= section.OnLoad(loadedPieces);
		}
		mReady = flag;
		if (mReady)
		{
			TBFUtils.DebugLog("AuthoredLevelLayoutController - ready!");
			AuthoredLevelController authoredLevelController = AuthoredLevelController.Instance();
			if (TestWorld)
			{
				authoredLevelController.SetGlobalWorld(this);
			}
			else if (AuthoredSectionsActive)
			{
				authoredLevelController.RegisterLayoutController(this);
			}
			if (DebugMenu)
			{
				authoredLevelController.SetDebugMenuLevel(this);
			}
			ResetAvailabilityForSpawn();
		}
	}

	public void Reset()
	{
		mLap = 0;
		ResetAvailabilityForSpawn();
	}

	public bool AreAuthoredSectionsActive()
	{
		return AuthoredSectionsActive;
	}

	public bool IsTestingAuthoredSections()
	{
		return TestWorld;
	}

	public int GetTestSection()
	{
		return TestSection;
	}

	public void TestSpecificSection(int section)
	{
		AuthoredSectionsActive = false;
		TestSection = section;
		TestWorld = true;
	}

	public void ClearSpecificTestSection()
	{
		TestSection = -1;
	}

	public void StopTestingSections()
	{
		TestSection = -1;
		TestWorld = false;
	}

	public void SetAuthoredSectionsActive(bool active)
	{
		AuthoredSectionsActive = active;
		if (active)
		{
			StopTestingSections();
		}
	}

	public bool IsNextPieceSelfLoop(int parentSection, int parentElement)
	{
		if (TestSection != -1)
		{
			return true;
		}
		if (parentElement + 1 >= SectionList[parentSection].ElementList.Count)
		{
			return SectionList[parentSection].StraightAheadSection == -1;
		}
		return false;
	}

	public bool IsNextPieceType(int parentSection, int parentElement, WorldConstructionHelper.PieceType type)
	{
		if (parentElement + 1 < SectionList[parentSection].ElementList.Count)
		{
			PieceDescriptor elementCached = SectionList[parentSection].ElementList[parentElement + 1].GetElementCached();
			return (bool)elementCached && elementCached.TypeId == type;
		}
		return false;
	}

	public PieceDescriptor GetPiece(int parentSection, int parentElement)
	{
		PieceDescriptor result = null;
		if (parentElement >= 0 && parentElement < SectionList[parentSection].ElementList.Count)
		{
			PieceDescriptor elementCached = SectionList[parentSection].ElementList[parentElement].GetElementCached();
			result = elementCached;
		}
		return result;
	}

	public int GetNumberOfElements(int parentSection)
	{
		if (parentSection < SectionList.Count)
		{
			return SectionList[parentSection].ElementList.Count;
		}
		return 0;
	}

	public PieceDescriptor CreateNextPiece(int parentSection, int parentElement, int parentBranch)
	{
		int num = parentSection;
		int num2 = parentElement;
		num2++;
		if (num2 >= SectionList[num].ElementList.Count)
		{
			int num3 = SectionList[num].StraightAheadSection;
			switch (parentBranch)
			{
			case -1:
				num3 = SectionList[num].BranchLeftSection;
				break;
			case 1:
				num3 = SectionList[num].BranchRightSection;
				break;
			}
			if (TestSection != -1)
			{
				num3 = -1;
			}
			if (num3 != -1)
			{
				num = num3;
			}
			else
			{
				if (!TestWorld)
				{
					return null;
				}
				mLap++;
			}
			num2 = 0;
		}
		GameTutorial.Instance.IsEnabled = SectionList[num].TutorialSection;
		AuthoredLevelLayoutElement authoredLevelLayoutElement = SectionList[num].ElementList[num2];
		PieceDescriptor elementCached = authoredLevelLayoutElement.GetElementCached();
		PieceDescriptor component = (Object.Instantiate(elementCached.gameObject) as GameObject).GetComponent<PieceDescriptor>();
		component.name = "_AUG_" + mLap + "_" + num + "_" + num2 + "_" + component.name;
		component.AuthoredSection = num;
		component.AuthoredElement = num2;
		if (GameController.Instance != null && GameController.Instance.IsPlayingTrialsMode)
		{
			TrialsCollectableRelic[] componentsInChildren = component.gameObject.GetComponentsInChildren<TrialsCollectableRelic>();
			foreach (TrialsCollectableRelic trialsCollectableRelic in componentsInChildren)
			{
				if (!(trialsCollectableRelic.m_trial == AuthoredLevelName) || trialsCollectableRelic.m_index != authoredLevelLayoutElement.relicIndex || trialsCollectableRelic.HasBeenCollected())
				{
					Object.Destroy(trialsCollectableRelic.gameObject);
				}
				else
				{
					trialsCollectableRelic.SpawnRelic();
				}
			}
		}
		switch (authoredLevelLayoutElement.Spawn)
		{
		case AuthoredLevelLayoutElement.Placement.Crocodile:
			BaddieController.Instance().SpawnNewPiece(component, BaddieController.Type.Crocodile);
			break;
		case AuthoredLevelLayoutElement.Placement.Scorpion:
			BaddieController.Instance().SpawnNewPiece(component, BaddieController.Type.Scorpion);
			break;
		case AuthoredLevelLayoutElement.Placement.Snake:
			BaddieController.Instance().SpawnNewPiece(component, BaddieController.Type.Snake);
			break;
		case AuthoredLevelLayoutElement.Placement.Powerup:
			PickupController.Instance().SpawnNewPiecePowerup(component);
			break;
		case AuthoredLevelLayoutElement.Placement.Coin_L:
			PickupController.Instance().SpawnNewPieceCoinsWithPos(component, true, false, false);
			break;
		case AuthoredLevelLayoutElement.Placement.Coin_C:
			PickupController.Instance().SpawnNewPieceCoinsWithPos(component, false, true, false);
			break;
		case AuthoredLevelLayoutElement.Placement.Coin_R:
			PickupController.Instance().SpawnNewPieceCoinsWithPos(component, false, false, true);
			break;
		}
		if (GameTutorial.Instance.IsEnabled && authoredLevelLayoutElement.Spawn == AuthoredLevelLayoutElement.Placement.Snake)
		{
			component.TutorialTypeId = WorldConstructionHelper.PieceTutorialType.Tutorial_Baddie;
		}
		if (component.TypeId == WorldConstructionHelper.PieceType.ThemeTransition)
		{
			CreateNextPieceVehicle(component);
			ThemeManager.Instance.OnThemeChange(component.Theme);
		}
		return component;
	}

	private void CreateNextPieceVehicle(PieceDescriptor clonedPiece)
	{
		if (clonedPiece.Theme == WorldConstructionHelper.Theme.Bike)
		{
			GameObject gameObject = GameObjectUtils.FindChildWithName(clonedPiece.gameObject, "TransitionObjectNode");
			if ((bool)gameObject)
			{
				GameObject gameObject2 = Object.Instantiate(PlayerController.Instance().BikeModel) as GameObject;
				if ((bool)gameObject2)
				{
					gameObject2.SetActiveRecursively(true);
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.identity;
					gameObject2.transform.Rotate(new Vector3(0f, 1f, 0f), -90f);
					gameObject2.tag = "TransitionThemeObject";
				}
			}
		}
		else
		{
			if (clonedPiece.Theme != WorldConstructionHelper.Theme.Minecart)
			{
				return;
			}
			GameObject gameObject3 = GameObjectUtils.FindChildWithName(clonedPiece.gameObject, "TransitionObjectNode");
			if ((bool)gameObject3)
			{
				GameObject gameObject4 = Object.Instantiate(PlayerController.Instance().MineCartModel, gameObject3.transform.position, Quaternion.identity) as GameObject;
				if ((bool)gameObject4)
				{
					gameObject4.SetActiveRecursively(true);
					gameObject4.transform.parent = gameObject3.transform;
					gameObject4.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
					gameObject4.transform.localPosition = Vector3.zero;
					gameObject4.transform.localRotation = Quaternion.identity;
					gameObject4.transform.Rotate(new Vector3(0f, 1f, 0f), -90f);
					gameObject4.tag = "TransitionThemeObject";
				}
			}
		}
	}

	public PieceDescriptor CreateSpecifiedPiece(int sectionIndex, int index, PieceSet pieceSet)
	{
		string element = SectionList[sectionIndex].ElementList[index].Element;
		PieceDescriptor pieceDescriptor = null;
		foreach (PieceDescriptor piece in pieceSet.Pieces)
		{
			if (piece.name == element)
			{
				pieceDescriptor = piece;
				break;
			}
		}
		PieceDescriptor component = (Object.Instantiate(pieceDescriptor.gameObject) as GameObject).GetComponent<PieceDescriptor>();
		component.name = "_AUG_" + sectionIndex + "_" + index + "_" + component.name;
		return component;
	}

	public PieceDescriptor CreateExtraStraightPiece(PieceSet pieces, WorldConstructionHelper.Theme theme, WorldConstructionHelper.Group grp, float extraLength)
	{
		PieceDescriptor straightAtRequiredLength = CreateNextPieceHelper.GetStraightAtRequiredLength(pieces, theme, grp, extraLength, false, false, LevelGenerator.Instance().CurrentGameType);
		PieceDescriptor component = (Object.Instantiate(straightAtRequiredLength.gameObject) as GameObject).GetComponent<PieceDescriptor>();
		component.name = "_XTR_" + component.name;
		return component;
	}

	public void RemoveSection(int sectionIndex)
	{
		SectionList.RemoveAt(sectionIndex);
	}

	public void RemoveElement(int sectionIndex, int elementIndex)
	{
		SectionList[sectionIndex].ElementList.RemoveAt(elementIndex);
	}

	public bool IsAvailableForSpawn()
	{
		return mAvailabilityTimer <= 0f;
	}

	public void ResetAvailabilityForSpawn()
	{
		mAvailabilityTimer = OccurenceTimer;
	}
}
