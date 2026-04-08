using System.Collections;
using UnityEngine;

public class PatchItem : MonoBehaviour
{
	private Achievements.AchievementData m_Data;

	public ConfirmDialog m_PatchDialogPrefab;

	public SimpleSprite m_Icon;

	public SpriteText m_Percent;

	public void OnItemSelected()
	{
		StartCoroutine(ShowPatchInfo());
	}

	public void Awake()
	{
	}

	private IEnumerator ShowPatchInfo()
	{
		ConfirmDialog confirmDialog = (ConfirmDialog)Object.Instantiate(m_PatchDialogPrefab);
		string Title = Language.Get(m_Data.Name);
		string Body = ((!AchievementManager.Instance.IsCompleted(m_Data.Identifier)) ? Language.Get(m_Data.PreEarnedDesc) : Language.Get(m_Data.EarnedDesc));
		yield return StartCoroutine(confirmDialog.Display(Title, Body, string.Empty, string.Empty, null, null, OnClose));
	}

	public void PopulateItem(Achievements.AchievementData Data)
	{
		m_Data = Data;
		string text = "Textures/Patches/patchlock.circle";
		if (AchievementManager.Instance.IsCompleted(Data.Identifier))
		{
			text = string.Format("Textures/Patches/{0}", Data.Identifier);
			m_Percent.Text = string.Empty;
		}
		else if (AchievementManager.Instance.IsOneShot(m_Data.Identifier))
		{
			m_Percent.Text = string.Empty;
		}
		else
		{
			int num = (int)AchievementManager.Instance.GetPercentComplete(m_Data.Identifier);
			m_Percent.Text = string.Format("{0}%", num);
		}
		if (m_Icon.GetComponent<Renderer>().material.mainTexture.name != text)
		{
			m_Icon.GetComponent<Renderer>().material.mainTexture = Resources.Load(text) as Texture;
		}
		float a = 0.5f;
		if (AchievementManager.Instance.IsCompleted(Data.Identifier))
		{
			a = 1f;
		}
		m_Icon.SetColor(new Color(1f, 1f, 1f, a));
	}

	public void OnClose()
	{
	}
}
