using UnityEngine;

public class StatBar : MonoBehaviour
{
	public SpriteText m_Description;

	public SpriteText m_Amount;

	public void PopulateItem(Stats.StatData data)
	{
		m_Description.Text = Language.Get(data.Name);
		m_Amount.Text = SecureStorage.Instance.GetStatRaw(data.Identifier) + data.Suffix;
	}
}
