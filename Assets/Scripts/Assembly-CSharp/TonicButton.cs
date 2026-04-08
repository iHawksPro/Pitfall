using UnityEngine;

public class TonicButton : MonoBehaviour
{
	public SpriteText m_Text;

	public UIButton m_Button;

	private void Start()
	{
	}

	private void Update()
	{
		m_Text.Text = string.Format("{0}/{1}", TrialsDataManager.Instance.NumBoostsAvailable, TrialsDataManager.Instance.MaxBoostsAvailable);
		m_Button.controlIsEnabled = TrialsDataManager.Instance.NumBoostsAvailable != TrialsDataManager.Instance.MaxBoostsAvailable;
	}
}
