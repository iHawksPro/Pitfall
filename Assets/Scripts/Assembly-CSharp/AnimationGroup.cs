using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationGroup
{
	public enum AnimType
	{
		Run = 0,
		LeanLeft = 1,
		LeanRight = 2,
		CallOfTheJaguar = 3,
		Slide = 4,
		SlideHill = 5,
		Whip = 6,
		Jump = 7,
		SwingLong = 8,
		SwingLong2 = 9,
		SwingShort = 10,
		HitWall = 11,
		JumpFall = 12,
		SlideHitWall = 13,
		ExtraJump = 14,
		JumpPit = 15,
		Idle = 16,
		Skid = 17,
		SlideEnter = 18,
		SlideExit = 19,
		RunSlow = 20,
		RunSprint = 21,
		RunSlowLeanLeft = 22,
		RunSprintLeanLeft = 23,
		RunSlowLeanRight = 24,
		RunSprintLeanRight = 25,
		SidestepLeft = 26,
		SidestepRight = 27,
		Cutscene = 28,
		EatenByCrocodile = 29,
		EatenByCrocodile_Right = 30,
		CutsceneExit = 31,
		RunPoisoned = 32,
		TurnLeft = 33,
		TurnRight = 34,
		PreTurnLeft = 35,
		PreTurnRight = 36,
		VehicleTransitionStart = 37,
		VehicleTransitionStop = 38,
		JumpFromVehicle = 39,
		BirdDropOff = 40,
		RunDrinkMedicine = 41,
		SlideRunHit = 42,
		PitShallowHit = 43,
		FallEdgeDie_FrontLeft = 44,
		FallEdgeDie_FrontRight = 45,
		FallEdgeDie_Left = 46,
		FallEdgeDie_Right = 47,
		FallCliffCornerDie = 48,
		PoisonDeath = 49,
		DeathUndergrowth_Left = 50,
		DeathUndergrowth_Right = 51,
		DeathImpaleDoor = 52,
		RunDrinkSpirit = 53,
		RunDrinkSpeed = 54,
		TrialsRunStart = 55,
		TrialsRunEnd = 56
	}

	public AnimationList[] Anims;

	private Dictionary<int, AnimationList> mLookupTable;

	public void Init()
	{
		mLookupTable = new Dictionary<int, AnimationList>();
		AnimationList[] anims = Anims;
		foreach (AnimationList animationList in anims)
		{
			mLookupTable.Add((int)animationList.Type, animationList);
		}
	}

	public AnimationClip GetAnim(AnimType type, ref int chosenIndex, int forceIndex)
	{
		AnimationList animationListFromType = GetAnimationListFromType(type);
		if (animationListFromType != null)
		{
			if (forceIndex >= 0)
			{
				return animationListFromType.GetAnim(forceIndex);
			}
			return animationListFromType.GetAnim(ref chosenIndex);
		}
		return null;
	}

	private AnimationList GetAnimationListFromType(AnimType type)
	{
		AnimationList value = null;
		mLookupTable.TryGetValue((int)type, out value);
		return value;
	}

	public void LoadAnimations(Animation animComponent)
	{
		AnimationList[] anims = Anims;
		foreach (AnimationList animationList in anims)
		{
			animationList.LoadAnimations(animComponent);
		}
	}

	public void UnloadAnimations(Animation animComponent)
	{
		AnimationList[] anims = Anims;
		foreach (AnimationList animationList in anims)
		{
			animationList.UnloadAnimations(animComponent);
		}
	}
}
