using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialSFX
{
	public List<MaterialEntry> m_materialSfx;

	public void Play(PathMaterial MatType, GameObject source)
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < m_materialSfx.Count; i++)
		{
			if (m_materialSfx[i].m_pathMaterial == MatType)
			{
				num = i;
				break;
			}
			if (m_materialSfx[i].m_pathMaterial == PathMaterial.Default)
			{
				num2 = i;
			}
		}
		int num3 = -1;
		if (num != -1)
		{
			num3 = num;
		}
		else if (num2 != -1)
		{
			num3 = num2;
		}
		if (num3 != -1)
		{
			SoundManager.Instance.Play(m_materialSfx[num3].m_sfx, source);
		}
	}
}
