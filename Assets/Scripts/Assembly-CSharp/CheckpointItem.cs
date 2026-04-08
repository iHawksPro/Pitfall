using System;
using System.Collections;
using UnityEngine;

public class CheckpointItem : BaseCampProductItem
{
	public static float Size = 514f;

	public SpriteText m_ownedCount;

	public SpriteText m_DistanceText;

	public GameObject m_DistanceTab;

	public GameObject m_MapLineTop;

	public GameObject m_MapLineBottom;

	public GameObject m_MacawIcon;

	public GameObject m_PurchasedIcon;

	public GameObject m_UnPurchasedIcon;

	public GameObject m_LockedIcon;

	public ConfirmDialog m_ConfirmDialogPrefab;

	public ConfirmDialog m_TutorialPrefab;

	public GameObject m_SparkleOverlayPrefab;

	public BigMessage m_FreeMacawDialog;

	private GameObject m_SparkleOverlay;

	private CheckPointController.CHECKPOINT_TYPE m_Type;

	private bool m_IsLast;

	private bool m_IsFirst;

	private int m_Distance;

	public void Awake()
	{
	}

	public override void PopulateItem(BaseCampProducts.ProductData Data)
	{
		m_productData = Data;
		m_ItemBtn.scriptWithMethodToInvoke = this;
		m_ItemBtn.methodToInvoke = "OnItemPressed";
	}

	public GameObject GetPurchaseableCheckpoint()
	{
		return m_UnPurchasedIcon;
	}

	public void SetDistanceText(int dist)
	{
		if (dist == 2000)
		{
			dist = CheckPointController.Instance().FirstCheckPointDistance();
		}
		m_DistanceText.Text = string.Format("{0}m", dist);
		m_Distance = dist;
	}

	protected override void OnItemPressed()
	{
		BaseCampController.Instance.RegisterItemPressedForTutorial();
		base.OnItemPressed();
	}

	public void SetIconFromType(CheckPointController.CHECKPOINT_TYPE type, bool isLast, bool isFirst)
	{
		m_Type = type;
		m_IsLast = isLast;
		m_IsFirst = isFirst;
	}

	public void LateUpdate()
	{
		if (m_Distance == 0)
		{
			m_DistanceTab.SetActiveRecursively(false);
		}
		switch (m_Type)
		{
		case CheckPointController.CHECKPOINT_TYPE.MACAW:
			SetVisibility(m_MacawIcon, true);
			SetVisibility(m_LockedIcon, false);
			SetVisibility(m_PurchasedIcon, false);
			SetVisibility(m_UnPurchasedIcon, false);
			break;
		case CheckPointController.CHECKPOINT_TYPE.PURCHASABLE:
			SetVisibility(m_MacawIcon, false);
			SetVisibility(m_LockedIcon, false);
			SetVisibility(m_PurchasedIcon, false);
			SetVisibility(m_UnPurchasedIcon, true);
			break;
		case CheckPointController.CHECKPOINT_TYPE.PURCHASED:
			SetVisibility(m_MacawIcon, false);
			SetVisibility(m_LockedIcon, false);
			SetVisibility(m_PurchasedIcon, true);
			SetVisibility(m_UnPurchasedIcon, false);
			break;
		case CheckPointController.CHECKPOINT_TYPE.LOCKED:
			SetVisibility(m_MacawIcon, false);
			SetVisibility(m_LockedIcon, true);
			SetVisibility(m_PurchasedIcon, false);
			SetVisibility(m_UnPurchasedIcon, false);
			break;
		case CheckPointController.CHECKPOINT_TYPE.NONE:
			SetVisibility(m_MacawIcon, false);
			SetVisibility(m_LockedIcon, false);
			SetVisibility(m_PurchasedIcon, false);
			SetVisibility(m_UnPurchasedIcon, false);
			SetVisibility(m_DistanceTab, false);
			break;
		}
		if (m_IsFirst)
		{
			SetVisibility(m_MapLineBottom, false);
		}
		if (m_IsLast)
		{
			SetVisibility(m_MapLineTop, false);
		}
	}

	public void SetVisibility(GameObject ob, bool vis)
	{
		if (ob.active != vis)
		{
			ob.SetActiveRecursively(vis);
		}
	}

	private void SetOwnedCount()
	{
		m_ownedCount.Text = SecureStorage.Instance.GetItemCount(m_productData.Identifier).ToString();
	}

	public void OnLockedPressed()
	{
		if (!BaseCampProductItem.m_BuyDialogActive)
		{
			StartCoroutine(DisplayDialog("S_CHECKPOINT_LOCKED_TITLE", "S_CHECKPOINT_LOCKED"));
		}
	}

	public void OnMacawPressed()
	{
		if (!BaseCampProductItem.m_BuyDialogActive)
		{
			if (SecureStorage.Instance.HasMacaws && SecureStorage.Instance.FurthestDistanceTravelled > m_Distance)
			{
				StartCoroutine(DisplayDialog("S_CHECKPOINT_PURCHASED_TITLE", "S_RUN_FROM_ACTIVE_CHECKPOINT", "S_START_RUNNING", OnCheckpointRestart));
			}
			else
			{
				StartCoroutine(DisplayDialog("S_CHECKPOINT_PURCHASED_TITLE", "S_ACTIVE_CHECKPOINT_INFO"));
			}
		}
	}

	public void OnPurchasedPressed()
	{
		if (!BaseCampProductItem.m_BuyDialogActive)
		{
			if (SecureStorage.Instance.HasMacaws)
			{
				StartCoroutine(DisplayDialog("S_CHECKPOINT_PURCHASED_TITLE", "S_RUN_FROM_ACTIVE_CHECKPOINT", "S_START_RUNNING", OnCheckpointRestart));
			}
			else
			{
				StartCoroutine(DisplayDialog("S_CHECKPOINT_PURCHASED_TITLE", "S_ACTIVE_CHECKPOINT_INFO"));
			}
		}
	}

	private void OnCheckpointRestart()
	{
		Debug.Log("RESTART FROM CHECKPOINT: " + m_Distance);
		TitleController.m_pendingCheckpointRestart = m_Distance;
		StateManager.Instance.LoadAndActivateState("Title");
	}

	private IEnumerator DisplayDialog(string TitleKey, string BodyKey, string Opt1Key, Action OnYesAction)
	{
		BaseCampProductItem.m_BuyDialogActive = true;
		string Title = Language.Get(TitleKey);
		string Body = Language.Get(BodyKey);
		string Opt1 = Language.Get(Opt1Key);
		string Opt2 = Language.Get("S_OK");
		ConfirmDialog confirmDialog = (ConfirmDialog)UnityEngine.Object.Instantiate(m_ConfirmDialogPrefab);
		yield return StartCoroutine(confirmDialog.Display(Title, Body, Opt1, Opt2, OnYesAction, null, OnYes));
		BaseCampProductItem.m_BuyDialogActive = false;
	}

	private IEnumerator DisplayDialog(string TitleKey, string BodyKey)
	{
		yield return StartCoroutine(DisplayDialog(TitleKey, BodyKey, "S_OK", OnYes));
	}

	public void OnYes()
	{
	}

	protected override void OnPurchaseConfirmed()
	{
		base.OnPurchaseConfirmed();
		AwardFreeMacaws();
	}

	public void AwardFreeMacaws()
	{
		int macawsPerCheckpoint = SwrveServerVariables.Instance.MacawsPerCheckpoint;
		StartCoroutine(FreeMacawsDialog(macawsPerCheckpoint));
		SecureStorage.Instance.AwardFreeMacaws(macawsPerCheckpoint);
	}

	private IEnumerator FreeMacawsDialog(int numMacaws)
	{
		string Title = Language.Get("S_ACTIVATED");
		string Body = "x " + numMacaws;
		BigMessage freeMacawMessage = (BigMessage)UnityEngine.Object.Instantiate(m_FreeMacawDialog);
		yield return StartCoroutine(freeMacawMessage.Display(Title, Body, 3f));
		if (BaseCampController.Instance.TutorialActive())
		{
			GameObject backButton = GameObject.Find("BaseCampBackButton");
			yield return StartCoroutine(BaseCampController.Instance.SpawnTutorialDialog(m_TutorialPrefab, "S_CHECK_TUT_4_TITLE", "S_CHECK_TUT_4_BODY", backButton));
		}
	}
}
