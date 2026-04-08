using UnityEngine;

public class AnimationLoader : MonoBehaviour
{
	public AnimationGroup Run;

	public AnimationGroup Bike;

	public AnimationGroup Cart;

	public AnimationGroup Slope;

	public AnimationGroup Jaguar;

	public AnimationGroup Bear;

	public AnimationGroup Fairy;

	public AnimationGroup Modern;

	public AnimationGroup Ninja;

	public AnimationGroup Pilot;

	public AnimationGroup Pirate;

	public AnimationGroup Swat;

	public AnimationGroup Super;

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
		LoadAnimations();
	}

	public string GetAnimName(AnimationGroup.AnimType type)
	{
		bool usedFallback = false;
		int index = -1;
		AnimationClip anim = GetAnim(type, out usedFallback, ref index, -1);
		if ((bool)anim)
		{
			return anim.name;
		}
		return null;
	}

	public string GetAnimName(AnimationGroup.AnimType type, int forceIndex)
	{
		bool usedFallback = false;
		int index = -1;
		AnimationClip anim = GetAnim(type, out usedFallback, ref index, forceIndex);
		if ((bool)anim)
		{
			return anim.name;
		}
		return null;
	}

	public string GetAnimName(AnimationGroup.AnimType type, out int chosenIndex)
	{
		bool usedFallback = false;
		chosenIndex = -1;
		AnimationClip anim = GetAnim(type, out usedFallback, ref chosenIndex, -1);
		if ((bool)anim)
		{
			return anim.name;
		}
		return null;
	}

	public bool HasSpecificAnim(AnimationGroup.AnimType type)
	{
		bool usedFallback = false;
		int index = -1;
		AnimationClip anim = GetAnim(type, out usedFallback, ref index, -1);
		if (anim != null && !usedFallback)
		{
			return true;
		}
		return false;
	}

	private AnimationClip GetAnim(AnimationGroup.AnimType type, out bool usedFallback, ref int index, int forceIndex)
	{
		AnimationClip animationClip = null;
		usedFallback = false;
		PlayerTheme.ThemeType themeType = PlayerController.Instance().GetThemeType();
		if (themeType == PlayerTheme.ThemeType.Run)
		{
			if (type == AnimationGroup.AnimType.PreTurnLeft || type == AnimationGroup.AnimType.PreTurnRight)
			{
				animationClip = Run.GetAnim(type, ref index, forceIndex);
			}
			else
			{
				switch (SecureStorage.Instance.GetCurrentCostumeType())
				{
				case Costume.Bear:
					animationClip = Bear.GetAnim(type, ref index, forceIndex);
					break;
				case Costume.Fairy:
					animationClip = Fairy.GetAnim(type, ref index, forceIndex);
					break;
				case Costume.Modern:
					animationClip = Modern.GetAnim(type, ref index, forceIndex);
					break;
				case Costume.Ninja:
					animationClip = Ninja.GetAnim(type, ref index, forceIndex);
					break;
				case Costume.Pilot:
					animationClip = Pilot.GetAnim(type, ref index, forceIndex);
					break;
				case Costume.Pirate:
					animationClip = Pirate.GetAnim(type, ref index, forceIndex);
					break;
				case Costume.Swat:
					animationClip = Swat.GetAnim(type, ref index, forceIndex);
					break;
				case Costume.Super:
					animationClip = Super.GetAnim(type, ref index, forceIndex);
					break;
				default:
					animationClip = Run.GetAnim(type, ref index, forceIndex);
					break;
				}
			}
			if (animationClip == null)
			{
				usedFallback = true;
				animationClip = Run.GetAnim(type, ref index, forceIndex);
			}
		}
		if (themeType == PlayerTheme.ThemeType.Bike)
		{
			animationClip = Bike.GetAnim(type, ref index, forceIndex);
			if (animationClip == null)
			{
				usedFallback = true;
				animationClip = Run.GetAnim(type, ref index, forceIndex);
			}
		}
		if (themeType == PlayerTheme.ThemeType.Cart)
		{
			animationClip = Cart.GetAnim(type, ref index, forceIndex);
			if (animationClip == null)
			{
				usedFallback = true;
				animationClip = Run.GetAnim(type, ref index, forceIndex);
			}
		}
		if (themeType == PlayerTheme.ThemeType.Slope)
		{
			animationClip = Slope.GetAnim(type, ref index, forceIndex);
			if (animationClip == null)
			{
				usedFallback = true;
				animationClip = Run.GetAnim(type, ref index, forceIndex);
			}
		}
		if (themeType == PlayerTheme.ThemeType.Jaguar)
		{
			animationClip = Jaguar.GetAnim(type, ref index, forceIndex);
			if (animationClip == null)
			{
				usedFallback = true;
				animationClip = Run.GetAnim(type, ref index, forceIndex);
			}
		}
		return animationClip;
	}

	private void Init()
	{
		Run.Init();
		Bike.Init();
		Cart.Init();
		Slope.Init();
		Jaguar.Init();
		Bear.Init();
		Fairy.Init();
		Modern.Init();
		Ninja.Init();
		Pilot.Init();
		Pirate.Init();
		Swat.Init();
		Super.Init();
	}

	private void LoadAnimations()
	{
		Animation component = GetComponent<Animation>();
		if ((bool)component)
		{
			Run.LoadAnimations(component);
			Bike.LoadAnimations(component);
			Cart.LoadAnimations(component);
			Slope.LoadAnimations(component);
			Jaguar.LoadAnimations(component);
			Bear.LoadAnimations(component);
			Fairy.LoadAnimations(component);
			Modern.LoadAnimations(component);
			Ninja.LoadAnimations(component);
			Pilot.LoadAnimations(component);
			Pirate.LoadAnimations(component);
			Swat.LoadAnimations(component);
			Super.LoadAnimations(component);
		}
	}

	private void UnloadAnimations()
	{
		Animation component = GetComponent<Animation>();
		if ((bool)component)
		{
			Run.UnloadAnimations(component);
			Bike.UnloadAnimations(component);
			Cart.UnloadAnimations(component);
			Slope.UnloadAnimations(component);
			Jaguar.UnloadAnimations(component);
			Bear.UnloadAnimations(component);
			Fairy.UnloadAnimations(component);
			Modern.UnloadAnimations(component);
			Ninja.UnloadAnimations(component);
			Pilot.UnloadAnimations(component);
			Pirate.UnloadAnimations(component);
			Swat.UnloadAnimations(component);
			Super.UnloadAnimations(component);
		}
	}
}
