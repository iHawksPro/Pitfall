using UnityEngine;

[AddComponentMenu("TBF/Pitfall/ChallengeButton")]
public class ChallengeButton : MonoBehaviour
{
	public UIButton m_Button;

	public GameObject m_Unlocked;

	public GameObject m_NoMedalIcon;

	public GameObject m_BronzeIcon;

	public GameObject m_SilverIcon;

	public GameObject m_GoldIcon;

	public GameObject m_CollectedAllRelics;

	public SpriteText m_RelicText;

	public string m_Level = "jungle.easy";

	private void Start()
	{
	}

	private void OnDisable()
	{
		m_Unlocked.SetActiveRecursively(false);
	}

	private void OnEnable()
	{
		Setup();
	}

	private void LateUpdate()
	{
		Setup();
	}

	private void OnButtonPressedWhileDisabled()
	{
		MenuSFX.Instance.Play2D("MenuConfirm");
		CommonAnimations.AnimateButton(m_Button.gameObject);
		TrialsDataManager instance = TrialsDataManager.Instance;
		TrialsDataManager.TrialState trialState = instance.FindTrialState(m_Level);
		TrialsDataManager.TrialState trialState2 = instance.FindTrialState(trialState.m_srcData.m_unlockedbycompleting);
		string text = string.Format(Language.Get("S_COMPLETE_X_TO_UNLOCK"), Language.Get(trialState2.m_srcData.m_shorttitle));
		Debug.Log(text);
		DialogManager.Instance.SpawnDialog(Language.Get("S_SUPERHARRY_LOCKED"), text, Language.Get("S_OK"), Language.Get("S_OK"), null, null, null);
	}

	private void Setup()
	{
		m_Button.methodToInvokeWhenDisabled = "OnButtonPressedWhileDisabled";
		m_Button.scriptWithMethodToInvokeWhenDisabled = this;
		TrialsDataManager instance = TrialsDataManager.Instance;
		TrialsDataManager.TrialState trialState = instance.FindTrialState(m_Level);
		int num = 0;
		if (trialState != null)
		{
			for (int i = 0; i < trialState.m_srcData.m_numCollectables; i++)
			{
				if (trialState.m_relicCollected[i])
				{
					num++;
				}
			}
			m_RelicText.Text = num + "/" + trialState.m_srcData.m_numCollectables;
		}
		else
		{
			Debug.LogError("Could not find trial " + m_Level);
		}
		bool flag = true;
		TrialsDataManager.TrialState trialState2 = instance.FindTrialState(trialState.m_srcData.m_unlockedbycompleting);
		if (trialState2 != null)
		{
			flag = trialState2.m_completed;
		}
		if (flag)
		{
			m_Button.controlIsEnabled = true;
			m_Unlocked.SetActiveRecursively(true);
			m_CollectedAllRelics.SetActiveRecursively(num == trialState.m_srcData.m_numCollectables);
			if (trialState.m_completed)
			{
				m_NoMedalIcon.SetActiveRecursively(false);
				m_BronzeIcon.SetActiveRecursively(false);
				m_SilverIcon.SetActiveRecursively(false);
				m_GoldIcon.SetActiveRecursively(false);
				switch (trialState.MedalForTime(trialState.m_timeInSecs))
				{
				case TrialsDataManager.TrialState.eMedalType.eMEDAL_GOLD:
					m_GoldIcon.SetActiveRecursively(true);
					break;
				case TrialsDataManager.TrialState.eMedalType.eMEDAL_SILVER:
					m_SilverIcon.SetActiveRecursively(true);
					break;
				case TrialsDataManager.TrialState.eMedalType.eMEDAL_BRONZE:
					m_BronzeIcon.SetActiveRecursively(true);
					break;
				default:
					m_NoMedalIcon.SetActiveRecursively(true);
					break;
				}
			}
			else
			{
				m_NoMedalIcon.SetActiveRecursively(true);
				m_BronzeIcon.SetActiveRecursively(false);
				m_SilverIcon.SetActiveRecursively(false);
				m_GoldIcon.SetActiveRecursively(false);
			}
		}
		else
		{
			m_Button.controlIsEnabled = false;
			m_Unlocked.SetActiveRecursively(false);
		}
	}
}
