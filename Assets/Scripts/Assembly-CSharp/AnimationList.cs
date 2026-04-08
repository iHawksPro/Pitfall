using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationList
{
	public AnimationGroup.AnimType Type;

	public int AnimationLayer;

	public List<AnimationClip> Anims;

	public AnimationClip GetAnim(ref int index)
	{
		if (Anims.Count > 0 && index < Anims.Count - 1)
		{
			index = UnityEngine.Random.Range(0, Anims.Count);
			return Anims[index];
		}
		return null;
	}

	public AnimationClip GetAnim(int index)
	{
		if (Anims.Count > 0 && index < Anims.Count)
		{
			return Anims[index];
		}
		return null;
	}

	public void LoadAnimations(Animation animComponent)
	{
		foreach (AnimationClip anim in Anims)
		{
			if ((bool)anim && !animComponent[anim.name])
			{
				animComponent.AddClip(anim, anim.name);
				animComponent[anim.name].layer = AnimationLayer;
			}
		}
	}

	public void UnloadAnimations(Animation animComponent)
	{
		foreach (AnimationClip anim in Anims)
		{
			if ((bool)anim)
			{
				animComponent.RemoveClip(anim);
			}
		}
	}
}
