internal class TutorialMessageData
{
	private string m_Text;

	private bool m_Seen;

	public TutorialMessageData(string text)
	{
		m_Text = text;
		Reset();
	}

	public void Reset()
	{
		m_Seen = false;
	}

	public void Shown()
	{
		m_Seen = true;
	}

	public bool HasBeenShown()
	{
		return m_Seen;
	}

	public string Text()
	{
		return Language.Get(m_Text);
	}
}
