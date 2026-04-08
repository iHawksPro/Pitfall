using System;
using System.Collections;
using UnityEngine;

public class DailyDoubleController : MonoBehaviour
{
	public enum TreasureMultipliers
	{
		NONE = 0,
		TREASURE_X2 = 1,
		TREASURE_X3 = 2,
		TREASURE_X4 = 3,
		NUM_TREASUREMULTIPLIERS = 4
	}

	public enum XPMultipliers
	{
		NONE = 0,
		XP_X2 = 1,
		XP_X3 = 2,
		XP_X4 = 3,
		NUM_XPMultipliers = 4
	}

	public enum DiamondsMultipliers
	{
		NONE = 0,
		DIAMONDS_X2 = 1,
		DIAMONDS_X3 = 2,
		DIAMONDS_X4 = 3,
		NUM_DiamondsMultipliers = 4
	}

	private bool m_inDDTimeWindow;

	public bool ShowResultsDDXPMult;

	public MarqueeText BottomTicker;

	public Vector3 BottomTickerOrigPos = new Vector3(-1365.333f, -655f, 0f);

	public GameObject TickerAnimator;

	public GameObject TickerDiamond;

	public static DailyDoubleController Instance;

	public XPMultipliers XPMultiplier
	{
		get
		{
			if (SwrveServerVariables.Instance.DD_xpMultiplier != 1f && m_inDDTimeWindow)
			{
				switch ((int)SwrveServerVariables.Instance.DD_xpMultiplier)
				{
				case 2:
					return XPMultipliers.XP_X2;
				case 3:
					return XPMultipliers.XP_X3;
				case 4:
					return XPMultipliers.XP_X4;
				default:
					return XPMultipliers.NONE;
				}
			}
			return XPMultipliers.NONE;
		}
	}

	public TreasureMultipliers TreasureMultiplier
	{
		get
		{
			if (SwrveServerVariables.Instance.DD_treasureMultiplier != 1f && m_inDDTimeWindow)
			{
				switch ((int)SwrveServerVariables.Instance.DD_treasureMultiplier)
				{
				case 2:
					return TreasureMultipliers.TREASURE_X2;
				case 3:
					return TreasureMultipliers.TREASURE_X3;
				case 4:
					return TreasureMultipliers.TREASURE_X4;
				default:
					return TreasureMultipliers.NONE;
				}
			}
			return TreasureMultipliers.NONE;
		}
	}

	public DiamondsMultipliers DiamondsMultiplier
	{
		get
		{
			if (SwrveServerVariables.Instance.DD_diamondsMultiplier != 1f && m_inDDTimeWindow)
			{
				switch ((int)SwrveServerVariables.Instance.DD_diamondsMultiplier)
				{
				case 2:
					return DiamondsMultipliers.DIAMONDS_X2;
				case 3:
					return DiamondsMultipliers.DIAMONDS_X3;
				case 4:
					return DiamondsMultipliers.DIAMONDS_X4;
				default:
					return DiamondsMultipliers.NONE;
				}
			}
			return DiamondsMultipliers.NONE;
		}
	}

	public float DD_xpMultiplier
	{
		get
		{
			if (InDDTimeWindow())
			{
				return SwrveServerVariables.Instance.DD_xpMultiplier;
			}
			return 1f;
		}
	}

	public float DD_treasureMultiplier
	{
		get
		{
			if (InDDTimeWindow())
			{
				return SwrveServerVariables.Instance.DD_treasureMultiplier;
			}
			return 1f;
		}
	}

	public float DD_diamondsMultiplier
	{
		get
		{
			if (InDDTimeWindow())
			{
				return SwrveServerVariables.Instance.DD_diamondsMultiplier;
			}
			return 1f;
		}
	}

	public void SetBottomTicker(bool AnimateIn)
	{
		if (DiamondsMultiplier == DiamondsMultipliers.NONE)
		{
			return;
		}
		if (AnimateIn)
		{
			if (BottomTicker != null)
			{
				BottomTicker.gameObject.SetActiveRecursively(false);
				StartCoroutine(RepeatTickerPosition());
				TickerAnimator.transform.localPosition = new Vector3(0f, -200f, 0f);
				MenuSFX.Instance.Play2D("DailyDoubleMarker");
				WriteTickerInfo(DiamondsMultiplier);
			}
		}
		else if (BottomTicker != null)
		{
			HudElement hudElement = TickerAnimator.GetComponent("HudElement") as HudElement;
			hudElement.GoOffScreen();
			StartCoroutine(FinalizeTickerMoveOff());
		}
	}

	private IEnumerator RepeatTickerPosition()
	{
		float StartTime = Time.time;
		float Length = 0.4f;
		while (Time.time < StartTime + Length)
		{
			BottomTicker.transform.position = BottomTickerOrigPos;
			if (BottomTicker.transform.localPosition.z != -70f)
			{
				BottomTicker.transform.localPosition += new Vector3(0f, 0f, -70f);
			}
			BottomTicker.gameObject.SetActiveRecursively(true);
			TickerDiamond.SetActiveRecursively(true);
			yield return null;
		}
	}

	public void WriteTickerInfo(DiamondsMultipliers myMult)
	{
		int num = 0;
		switch (myMult)
		{
		case DiamondsMultipliers.DIAMONDS_X2:
			num = 2;
			break;
		case DiamondsMultipliers.DIAMONDS_X3:
			num = 3;
			break;
		case DiamondsMultipliers.DIAMONDS_X4:
			num = 4;
			break;
		default:
			num = 0;
			break;
		}
		BottomTicker.m_text.Color = Color.white;
		BottomTicker.SetMessage(string.Format(Language.Get("S_DAILY_DOUBLE_TICKER"), num));
	}

	public IEnumerator FinalizeTickerMoveOff()
	{
		yield return new WaitForSeconds(0.5f);
		BottomTicker.transform.localPosition = BottomTickerOrigPos;
		BottomTicker.gameObject.SetActiveRecursively(false);
	}

	public void SetDDTimeWindow()
	{
		m_inDDTimeWindow = false;
		if (SwrveServerVariables.Instance.DD_enabled)
		{
			uint secondsSinceUnixEpoch = TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow);
			m_inDDTimeWindow = secondsSinceUnixEpoch >= SwrveServerVariables.Instance.DD_startDate && secondsSinceUnixEpoch < SwrveServerVariables.Instance.DD_startDate + SwrveServerVariables.Instance.DD_duration;
		}
	}

	public bool InDDTimeWindow()
	{
		SetDDTimeWindow();
		return m_inDDTimeWindow;
	}

	private void Awake()
	{
		Instance = this;
	}
}
