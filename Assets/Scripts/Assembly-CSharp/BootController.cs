using System;
using System.Collections;
using UnityEngine;

public class BootController : StateController
{
	public override void Awake()
	{
		base.Awake();
		EtceteraPlatformWrapper.SetBadgeCount(0);
	}

	protected override void OnStateActivate(string oldStateName)
	{
		UIManager.instance.blockInput = true;
		LoadingScreen.Instance.Show();
		StartCoroutine("InitialLoad");
	}

	private IEnumerator MemoryStatsDebug()
	{
		int i = 0;
		while (true)
		{
			i++;
			long lBytes = GC.GetTotalMemory(false) / 1024;
			TBFUtils.DebugLog("MemoryStatsDebug: " + lBytes + "KB");
			yield return null;
		}
	}

	private IEnumerator InitialLoad()
	{
		ThreadPriority oldThreadPri = Application.backgroundLoadingPriority;
		string titleSceneName = SceneNameResolver.Resolve("Title");
		string gameSceneName = SceneNameResolver.Resolve("Game");
		string initialStateName = (!RecoveredCompatibility.SkipLegacySplash) ? "SplashIntro" : "Title";
		Application.backgroundLoadingPriority = ThreadPriority.High;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		TBFUtils.ForceRecalculateOnUIScales();
		if (TBFUtils.Is256mbDevice())
		{
			EtceteraPlatformWrapper.ShowAlert(string.Empty, Language.Get("S_ANDROID_LOW_MEM_DEVICE"), Language.Get("S_OK"));
			yield return Application.LoadLevelAdditiveAsync(titleSceneName);
			yield return Application.LoadLevelAdditiveAsync(gameSceneName);
			while (!ThemeManager.Instance.IsReady)
			{
				TBFUtils.DebugLog("Waiting for ThemeManager....");
				yield return new WaitForSeconds(0.5f);
			}
		}
		else
		{
			AsyncOperation aTitleOp = Application.LoadLevelAdditiveAsync(titleSceneName);
			AsyncOperation aGameOp = Application.LoadLevelAdditiveAsync(gameSceneName);
			yield return aTitleOp;
			yield return aGameOp;
		}
		Application.backgroundLoadingPriority = oldThreadPri;
		StateManager.Instance.LoadAndActivateState(initialStateName);
	}
}
