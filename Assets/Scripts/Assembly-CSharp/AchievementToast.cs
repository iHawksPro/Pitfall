using System.Collections;
using UnityEngine;

public class AchievementToast : MonoBehaviour
{
	public GameObject TransformNode;

	public SpriteText TextItem;

	public SimpleSprite m_Icon;

	private bool m_bActive;

	private bool IsActive
	{
		get
		{
			return m_bActive;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public void Kick(string strAchievementName)
	{
		StartCoroutine(DoToast(strAchievementName));
	}

	private IEnumerator DoToast(string strAchievementName)
	{
		while (IsActive)
		{
			yield return new WaitForSeconds(1f);
		}
		m_bActive = true;
		Achievements.AchievementData aData = AchievementManager.Instance.GetAchievementById(strAchievementName);
		if (aData != null)
		{
			string strPatchName = string.Format("Textures/Patches/{0}", aData.Identifier);
			if (m_Icon.GetComponent<Renderer>().material.mainTexture.name != base.name)
			{
				m_Icon.GetComponent<Renderer>().material.mainTexture = Resources.Load(strPatchName) as Texture;
			}
			TextItem.Text = Language.Get(aData.Name);
			TransformNode.MoveAdd(new Vector3(0f, -450f, 0f), 1f, 0f);
			yield return new WaitForSeconds(3f);
			TransformNode.MoveAdd(new Vector3(0f, 450f, 0f), 1f, 0f);
			yield return new WaitForSeconds(1f);
		}
		m_bActive = false;
	}
}
