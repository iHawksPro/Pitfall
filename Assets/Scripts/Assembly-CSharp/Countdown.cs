using System;
using System.Collections;
using UnityEngine;

public class Countdown : MonoBehaviour
{
	[Serializable]
	public class CountEntry
	{
		public string m_sfxName;

		public GameObject m_gameObj;
	}

	public CountEntry[] m_countDownEntries;

	public float m_tweenTime = 0.15f;

	public float m_timeBetween = 0.8f;

	private Vector3 m_offscreenLeftPos = new Vector3(-2000f, 0f, 0f);

	private Vector3 m_offscreenRightPos = new Vector3(2000f, 0f, 0f);

	private void Start()
	{
		for (int i = 0; i < m_countDownEntries.Length; i++)
		{
			m_countDownEntries[i].m_gameObj.transform.position = m_offscreenLeftPos;
			m_countDownEntries[i].m_gameObj.SetActiveRecursively(false);
		}
	}

	public IEnumerator DisplayCountdown(Action onCountdownComplete)
	{
		yield return StartCoroutine(WaitForRealSeconds(0.4f));
		for (int iCount = 0; iCount <= m_countDownEntries.Length; iCount++)
		{
			int iPrev = iCount - 1;
			int iNext = ((iCount >= m_countDownEntries.Length) ? (-1) : iCount);
			yield return StartCoroutine(CountTransition(iPrev, iNext));
		}
		if (onCountdownComplete != null)
		{
			onCountdownComplete();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator CountTransition(int iCurrentEntry, int iNewEntry)
	{
		if (iCurrentEntry != -1)
		{
			m_countDownEntries[iCurrentEntry].m_gameObj.SetActiveRecursively(true);
			iTween.MoveTo(m_countDownEntries[iCurrentEntry].m_gameObj, iTween.Hash("ignoretimescale", true, "x", m_offscreenRightPos.x, "y", 0, "time", m_tweenTime, "easeType", iTween.EaseType.linear));
		}
		if (iNewEntry != -1)
		{
			m_countDownEntries[iNewEntry].m_gameObj.SetActiveRecursively(true);
			m_countDownEntries[iNewEntry].m_gameObj.transform.position = m_offscreenLeftPos;
			iTween.MoveTo(m_countDownEntries[iNewEntry].m_gameObj, iTween.Hash("ignoretimescale", true, "x", 0f, "y", 0, "time", m_tweenTime, "easeType", iTween.EaseType.linear));
			MenuSFX.Instance.Play2D(m_countDownEntries[iNewEntry].m_sfxName);
		}
		float timeToWait = ((iNewEntry != -1) ? m_timeBetween : m_tweenTime);
		yield return StartCoroutine(WaitForRealSeconds(timeToWait));
		if (iCurrentEntry != -1)
		{
			m_countDownEntries[iCurrentEntry].m_gameObj.SetActiveRecursively(false);
		}
	}

	private IEnumerator WaitForRealSeconds(float timeToWait)
	{
		float endTime = Time.realtimeSinceStartup + timeToWait;
		while (Time.realtimeSinceStartup < endTime)
		{
			yield return null;
		}
	}
}
