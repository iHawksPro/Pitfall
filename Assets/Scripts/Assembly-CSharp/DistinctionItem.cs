using System.Collections;
using UnityEngine;

public class DistinctionItem : MonoBehaviour
{
	private Achievements.AchievementData m_Data;

	public ConfirmDialog m_DialogPrefab;

	public string m_PatchName;

	public void OnItemPressed()
	{
		StartCoroutine(ShowInfo());
	}

	public void Awake()
	{
		m_Data = AchievementManager.Instance.GetAchievementById(m_PatchName);
	}

	private IEnumerator ShowInfo()
	{
		ConfirmDialog confirmDialog = (ConfirmDialog)Object.Instantiate(m_DialogPrefab);
		string Title = Language.Get(m_Data.Name);
		string Body = ((!AchievementManager.Instance.IsCompleted(m_Data.Identifier)) ? Language.Get(m_Data.PreEarnedDesc) : Language.Get(m_Data.EarnedDesc));
		yield return StartCoroutine(confirmDialog.Display(Title, Body, string.Empty, string.Empty, null, null, OnClose));
	}

	public void OnClose()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
