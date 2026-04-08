using System.Collections.Generic;
using UnityEngine;

public class MarqueeText : MonoBehaviour
{
	private struct TickerMessage
	{
		public string message;

		public bool bAppropriateForTrials;
	}

	public SpriteText m_text;

	public float m_pixelsPerSecond = 400f;

	public bool m_includeSalesMessages;

	private float m_textWidth;

	private float m_screenWidth;

	public Color MarqueeBaseColor;

	private List<TickerMessage> m_tickerMessages = new List<TickerMessage>();

	private int m_currentMessage;

	private int m_messageTypeIndex;

	private void OnEnable()
	{
		m_tickerMessages.Clear();
		AddTickerMessage(Language.Get("S_MOTD"), true);
		AddTickerMessage(Language.Get("S_GAME_TIP_1"), false);
		AddTickerMessage(Language.Get("S_GAME_TIP_2"), false);
		AddTickerMessage(Language.Get("S_GAME_TIP_3"), true);
		AddTickerMessage(Language.Get("S_GAME_TIP_4"), false);
		AddTickerMessage(Language.Get("S_GAME_TIP_5"), false);
		AddTickerMessage(Language.Get("S_GAME_TIP_6"), false);
		AddTickerMessage(Language.Get("S_GAME_TIP_7"), true);
		AddTickerMessage(Language.Get("S_GAME_TIP_8"), true);
		AddTickerMessage(Language.Get("S_GAME_TIP_9"), true);
		AddTickerMessage(Language.Get("S_GAME_TIP_10"), true);
		AddTickerMessage(Language.Get("S_GAME_TIP_11"), false);
		AddTickerMessage(Language.Get("S_GAME_TIP_12"), false);
		AddTickerMessage(Language.Get("S_GAME_TIP_13"), false);
		AddTickerMessage(Language.Get("S_GAME_TIP_14"), true);
		AddTickerMessage(Language.Get("S_GAME_TIP_15"), true);
		m_currentMessage = 0;
		m_messageTypeIndex = 0;
		NextMessage();
		MoveToRightEdgeOfScreen();
	}

	public void SetMessage(string Message)
	{
		m_textWidth = m_text.GetWidth(Message);
		m_text.Text = Message;
	}

	private void NextMessage()
	{
		if (DailyDoubleController.Instance != null && DailyDoubleController.Instance.DiamondsMultiplier != DailyDoubleController.DiamondsMultipliers.NONE && (BaseCampController.Instance.ExchangeLink.active || InGameStorePopup.Instance != null))
		{
			DailyDoubleController.Instance.WriteTickerInfo(DailyDoubleController.Instance.DiamondsMultiplier);
			return;
		}
		if (m_includeSalesMessages && m_messageTypeIndex % 2 == 0 && StoreProductManager.Instance.IsSaleActive)
		{
			string title = StoreProductManager.Instance.SaleItem.GetTitle();
			string arg = string.Format(Language.Get("S_SALES_REDUCED"), (int)StoreProductManager.Instance.SalePercentOff);
			string message = string.Format("{0} : {1}", title, arg);
			m_text.Color = MarqueeBaseColor;
			SetMessage(message);
		}
		else if (m_tickerMessages.Count > 0)
		{
			int num = m_currentMessage;
			while (num == m_currentMessage)
			{
				num = Random.Range(0, m_tickerMessages.Count);
				if (GameController.Instance != null && GameController.Instance.IsPlayingTrialsMode && !m_tickerMessages[num].bAppropriateForTrials)
				{
					num = m_currentMessage;
				}
			}
			m_text.Color = MarqueeBaseColor;
			SetMessage(m_tickerMessages[num].message);
			m_currentMessage = num;
		}
		m_messageTypeIndex++;
	}

	private void MoveToRightEdgeOfScreen()
	{
		m_text.transform.position = new Vector3(Screen.width, m_text.transform.position.y, m_text.transform.position.z);
	}

	private void Update()
	{
		if (m_text.transform.position.x + m_textWidth < (float)(-Screen.width))
		{
			NextMessage();
			MoveToRightEdgeOfScreen();
		}
		else
		{
			m_text.transform.position -= new Vector3(m_pixelsPerSecond * Time.deltaTime, 0f, 0f);
		}
	}

	public void AddTickerMessage(string NewMessage, bool bAppropriateForTrials)
	{
		TickerMessage item = new TickerMessage
		{
			message = NewMessage,
			bAppropriateForTrials = bAppropriateForTrials
		};
		m_tickerMessages.Add(item);
	}
}
