using System;
using UnityEngine;

public class WorldConstructionHelper
{
	public enum Theme
	{
		Jungle = 0,
		Mountain = 1,
		Cave = 2,
		SlippedMountain = 3,
		Minecart = 4,
		Bike = 5,
		Menu = 6
	}

	public enum Group
	{
		Invalid = 0,
		A = 1,
		B = 2,
		C = 3,
		D = 4,
		E = 5,
		F = 6,
		G = 7,
		H = 8,
		I = 9,
		J = 10,
		K = 11,
		L = 12,
		M = 13,
		N = 14,
		O = 15,
		P = 16,
		Q = 17,
		R = 18,
		S = 19,
		T = 20,
		U = 21,
		V = 22,
		W = 23,
		X = 24,
		Y = 25,
		Z = 26,
		Transition = 27
	}

	public enum PieceType
	{
		Straight = 0,
		Straight_Feature = 1,
		TrackReduction = 2,
		Junction = 3,
		Junction_L = 4,
		Junction_R = 5,
		Junction_T = 6,
		Junction_Optional = 7,
		Junction_Optional_L = 8,
		Junction_Optional_R = 9,
		Junction_Optional_T = 10,
		Jump = 11,
		Duck = 12,
		JumpOrDuck = 13,
		Baddie = 14,
		RopeSwing = 15,
		Pickup = 16,
		Checkpoint = 17,
		Pit_Type1_Entry = 18,
		Pit_Type1_Entry_Down = 19,
		Pit_Type1_Loop = 20,
		Pit_Type1_Exit = 21,
		Pit_Type1_Exit_Up = 22,
		Pit_Type2_Entry = 23,
		Pit_Type2_Loop = 24,
		Pit_Type2_Exit = 25,
		Pit_Type3_Entry = 26,
		Pit_Type3_Loop = 27,
		Pit_Type3_Exit = 28,
		Pit_Type4_Entry = 29,
		Pit_Type4_Loop = 30,
		Pit_Type4_Exit = 31,
		Pit_Type5_Entry = 32,
		Pit_Type5_Loop = 33,
		Pit_Type5_Exit = 34,
		Pit_Type6_Entry = 35,
		Pit_Type6_Loop = 36,
		Pit_Type6_Exit = 37,
		RopeSwing_Entry = 38,
		RopeSwing_Node = 39,
		RopeSwing_Tile = 40,
		RopeSwing_Exit = 41,
		Crocodile_Entry = 42,
		Crocodile_Loop = 43,
		Crocodile_Exit = 44,
		Crocodile = 45,
		Scorpion = 46,
		Snake = 47,
		Meteor = 48,
		ThemeTransition = 49,
		Exclude = 50
	}

	public enum PieceTutorialType
	{
		Tutorial_Jump = 0,
		Tutorial_Duck = 1,
		Tutorial_JumpOrDuck = 2,
		Tutorial_Baddie = 3,
		Tutorial_RopeSwing = 4,
		Tutorial_Tilt_Left = 5,
		Tutorial_Tilt_Right = 6,
		Tutorial_Swipe_Left = 7,
		Tutorial_Swipe_Right = 8,
		Tutorial_Tap = 9,
		Exclude = 10,
		Tutorial_WellDone = 11,
		Tutorial_Finish = 12,
		Tutorial_Poison = 13,
		Tutorial_AntiVenom = 14
	}

	public enum DifficultyType
	{
		Turn = 0,
		EasyJumpOrDuck = 1,
		EasyJumpAndDuck = 2,
		OneTrackGone = 3,
		TwoTracksGoneEasy = 4,
		TwoTracksGoneHard = 5,
		MiddleTrackDanger = 6,
		WallMiddle = 7,
		WallUpper = 8,
		WallLower = 9
	}

	public static int Theme_ListSize = 7;

	public static int Group_ListSize = 28;

	private static PieceType[,] TypeTranslationTable = new PieceType[51, 2]
	{
		{
			PieceType.Straight,
			PieceType.Straight
		},
		{
			PieceType.Straight_Feature,
			PieceType.Straight
		},
		{
			PieceType.TrackReduction,
			PieceType.TrackReduction
		},
		{
			PieceType.Junction,
			PieceType.Junction
		},
		{
			PieceType.Junction_L,
			PieceType.Junction
		},
		{
			PieceType.Junction_R,
			PieceType.Junction
		},
		{
			PieceType.Junction_T,
			PieceType.Junction
		},
		{
			PieceType.Junction_Optional,
			PieceType.Junction
		},
		{
			PieceType.Junction_Optional_L,
			PieceType.Junction
		},
		{
			PieceType.Junction_Optional_R,
			PieceType.Junction
		},
		{
			PieceType.Junction_Optional_T,
			PieceType.Junction
		},
		{
			PieceType.Jump,
			PieceType.Jump
		},
		{
			PieceType.Duck,
			PieceType.Duck
		},
		{
			PieceType.JumpOrDuck,
			PieceType.JumpOrDuck
		},
		{
			PieceType.Baddie,
			PieceType.Baddie
		},
		{
			PieceType.RopeSwing,
			PieceType.RopeSwing
		},
		{
			PieceType.Pickup,
			PieceType.Pickup
		},
		{
			PieceType.Checkpoint,
			PieceType.Checkpoint
		},
		{
			PieceType.Pit_Type1_Entry,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type1_Entry_Down,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type1_Loop,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type1_Exit,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type1_Exit_Up,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type2_Entry,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type2_Loop,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type2_Exit,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type3_Entry,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type3_Loop,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type3_Exit,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type4_Entry,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type4_Loop,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type4_Exit,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type5_Entry,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type5_Loop,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type5_Exit,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type6_Entry,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type6_Loop,
			PieceType.Jump
		},
		{
			PieceType.Pit_Type6_Exit,
			PieceType.Jump
		},
		{
			PieceType.RopeSwing_Entry,
			PieceType.RopeSwing
		},
		{
			PieceType.RopeSwing_Node,
			PieceType.RopeSwing
		},
		{
			PieceType.RopeSwing_Tile,
			PieceType.RopeSwing
		},
		{
			PieceType.RopeSwing_Exit,
			PieceType.RopeSwing
		},
		{
			PieceType.Crocodile_Entry,
			PieceType.Baddie
		},
		{
			PieceType.Crocodile_Loop,
			PieceType.Baddie
		},
		{
			PieceType.Crocodile_Exit,
			PieceType.Baddie
		},
		{
			PieceType.Crocodile,
			PieceType.Baddie
		},
		{
			PieceType.Scorpion,
			PieceType.Baddie
		},
		{
			PieceType.Snake,
			PieceType.Baddie
		},
		{
			PieceType.Meteor,
			PieceType.Baddie
		},
		{
			PieceType.ThemeTransition,
			PieceType.Straight
		},
		{
			PieceType.Exclude,
			PieceType.Straight
		}
	};

	public static int Type_ListSize = 51;

	private static PieceType[] Pits = new PieceType[25]
	{
		PieceType.Pit_Type1_Entry,
		PieceType.Pit_Type1_Entry_Down,
		PieceType.Pit_Type1_Loop,
		PieceType.Pit_Type1_Exit,
		PieceType.Pit_Type1_Exit_Up,
		PieceType.Pit_Type2_Entry,
		PieceType.Pit_Type2_Loop,
		PieceType.Pit_Type2_Exit,
		PieceType.Pit_Type3_Entry,
		PieceType.Pit_Type3_Loop,
		PieceType.Pit_Type3_Exit,
		PieceType.Pit_Type4_Entry,
		PieceType.Pit_Type4_Loop,
		PieceType.Pit_Type4_Exit,
		PieceType.Pit_Type5_Entry,
		PieceType.Pit_Type5_Loop,
		PieceType.Pit_Type5_Exit,
		PieceType.Pit_Type6_Entry,
		PieceType.Pit_Type6_Loop,
		PieceType.Pit_Type6_Exit,
		PieceType.RopeSwing,
		PieceType.RopeSwing_Entry,
		PieceType.RopeSwing_Node,
		PieceType.RopeSwing_Tile,
		PieceType.RopeSwing_Exit
	};

	private static PieceType[] RopeSwing = new PieceType[5]
	{
		PieceType.RopeSwing,
		PieceType.RopeSwing_Entry,
		PieceType.RopeSwing_Node,
		PieceType.RopeSwing_Tile,
		PieceType.RopeSwing_Exit
	};

	private static PieceType[] ExclusiveJumps = new PieceType[6]
	{
		PieceType.Pit_Type1_Entry,
		PieceType.Pit_Type1_Entry_Down,
		PieceType.Pit_Type1_Loop,
		PieceType.Pit_Type1_Exit,
		PieceType.Pit_Type1_Exit_Up,
		PieceType.Jump
	};

	private static PieceType[] Junctions = new PieceType[8]
	{
		PieceType.Junction,
		PieceType.Junction_L,
		PieceType.Junction_R,
		PieceType.Junction_T,
		PieceType.Junction_Optional,
		PieceType.Junction_Optional_L,
		PieceType.Junction_Optional_R,
		PieceType.Junction_Optional_T
	};

	private static PieceType[] OptionalJunctions = new PieceType[4]
	{
		PieceType.Junction_Optional,
		PieceType.Junction_Optional_L,
		PieceType.Junction_Optional_R,
		PieceType.Junction_Optional_T
	};

	private static PieceType[] MultiTilePiecePartial = new PieceType[18]
	{
		PieceType.Pit_Type1_Entry,
		PieceType.Pit_Type1_Entry_Down,
		PieceType.Pit_Type1_Loop,
		PieceType.Pit_Type2_Entry,
		PieceType.Pit_Type2_Loop,
		PieceType.Pit_Type3_Entry,
		PieceType.Pit_Type3_Loop,
		PieceType.Pit_Type4_Entry,
		PieceType.Pit_Type4_Loop,
		PieceType.Pit_Type5_Entry,
		PieceType.Pit_Type5_Loop,
		PieceType.Pit_Type6_Entry,
		PieceType.Pit_Type6_Loop,
		PieceType.RopeSwing_Entry,
		PieceType.RopeSwing_Node,
		PieceType.RopeSwing_Tile,
		PieceType.Crocodile_Entry,
		PieceType.Crocodile_Loop
	};

	private static PieceType[] MultiTilePieceExcludingEntry = new PieceType[17]
	{
		PieceType.Pit_Type1_Loop,
		PieceType.Pit_Type1_Exit,
		PieceType.Pit_Type2_Loop,
		PieceType.Pit_Type2_Exit,
		PieceType.Pit_Type3_Loop,
		PieceType.Pit_Type3_Exit,
		PieceType.Pit_Type4_Loop,
		PieceType.Pit_Type4_Exit,
		PieceType.Pit_Type5_Loop,
		PieceType.Pit_Type5_Exit,
		PieceType.Pit_Type6_Loop,
		PieceType.Pit_Type6_Exit,
		PieceType.RopeSwing_Node,
		PieceType.RopeSwing_Tile,
		PieceType.RopeSwing_Exit,
		PieceType.Crocodile_Loop,
		PieceType.Crocodile_Exit
	};

	private static PieceType[] MultiTilePieceEntry = new PieceType[9]
	{
		PieceType.Pit_Type1_Entry,
		PieceType.Pit_Type1_Entry_Down,
		PieceType.Pit_Type2_Entry,
		PieceType.Pit_Type3_Entry,
		PieceType.Pit_Type4_Entry,
		PieceType.Pit_Type5_Entry,
		PieceType.Pit_Type6_Entry,
		PieceType.RopeSwing_Entry,
		PieceType.Crocodile_Entry
	};

	private static PieceType[] MultiTilePieceExit = new PieceType[8]
	{
		PieceType.Pit_Type1_Exit,
		PieceType.Pit_Type2_Exit,
		PieceType.Pit_Type3_Exit,
		PieceType.Pit_Type4_Exit,
		PieceType.Pit_Type5_Exit,
		PieceType.Pit_Type6_Exit,
		PieceType.RopeSwing_Exit,
		PieceType.Crocodile_Exit
	};

	private static PieceType[] TrackReductions = new PieceType[15]
	{
		PieceType.Pit_Type2_Entry,
		PieceType.Pit_Type2_Loop,
		PieceType.Pit_Type2_Exit,
		PieceType.Pit_Type3_Entry,
		PieceType.Pit_Type3_Loop,
		PieceType.Pit_Type3_Exit,
		PieceType.Pit_Type4_Entry,
		PieceType.Pit_Type4_Loop,
		PieceType.Pit_Type4_Exit,
		PieceType.Pit_Type5_Entry,
		PieceType.Pit_Type5_Loop,
		PieceType.Pit_Type5_Exit,
		PieceType.Pit_Type6_Entry,
		PieceType.Pit_Type6_Loop,
		PieceType.Pit_Type6_Exit
	};

	private static PieceType[] BlindHazards = new PieceType[5]
	{
		PieceType.Junction,
		PieceType.Junction_L,
		PieceType.Junction_R,
		PieceType.Junction_T,
		PieceType.Duck
	};

	private static PieceType[] Hazards = new PieceType[10]
	{
		PieceType.TrackReduction,
		PieceType.Junction,
		PieceType.Junction_L,
		PieceType.Junction_R,
		PieceType.Junction_T,
		PieceType.Jump,
		PieceType.Duck,
		PieceType.JumpOrDuck,
		PieceType.Baddie,
		PieceType.RopeSwing
	};

	private static PieceType[] Baddies = new PieceType[4]
	{
		PieceType.Crocodile,
		PieceType.Scorpion,
		PieceType.Snake,
		PieceType.Meteor
	};

	public static float StraightScalingMultiplier = 1f;

	public static PieceType GetSimpleType(PieceType type)
	{
		PieceType pieceType = TypeTranslationTable[(int)type, 0];
		if (pieceType != type)
		{
			throw new Exception("Type Translation Table is incorrect: " + type.ToString() + " != " + pieceType);
		}
		return TypeTranslationTable[(int)type, 1];
	}

	public static bool IsPit(PieceType type)
	{
		PieceType[] pits = Pits;
		foreach (PieceType pieceType in pits)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsRopeSwing(PieceType type)
	{
		PieceType[] ropeSwing = RopeSwing;
		foreach (PieceType pieceType in ropeSwing)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsExclusiveJump(PieceType type)
	{
		PieceType[] exclusiveJumps = ExclusiveJumps;
		foreach (PieceType pieceType in exclusiveJumps)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsJunction(PieceType type)
	{
		PieceType[] junctions = Junctions;
		foreach (PieceType pieceType in junctions)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsOptionalJunction(PieceType type)
	{
		PieceType[] optionalJunctions = OptionalJunctions;
		foreach (PieceType pieceType in optionalJunctions)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsMultiTilePiecePartial(PieceType type)
	{
		PieceType[] multiTilePiecePartial = MultiTilePiecePartial;
		foreach (PieceType pieceType in multiTilePiecePartial)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsMultiTilePieceExcludingEntry(PieceType type)
	{
		PieceType[] multiTilePieceExcludingEntry = MultiTilePieceExcludingEntry;
		foreach (PieceType pieceType in multiTilePieceExcludingEntry)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsMultiTilePieceEntry(PieceType type)
	{
		PieceType[] multiTilePieceEntry = MultiTilePieceEntry;
		foreach (PieceType pieceType in multiTilePieceEntry)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsMultiTilePieceExit(PieceType type)
	{
		PieceType[] multiTilePieceExit = MultiTilePieceExit;
		foreach (PieceType pieceType in multiTilePieceExit)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsTrackReduction(PieceType type)
	{
		PieceType[] trackReductions = TrackReductions;
		foreach (PieceType pieceType in trackReductions)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsBlindHazard(PieceType type)
	{
		PieceType[] blindHazards = BlindHazards;
		foreach (PieceType pieceType in blindHazards)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsHazard(PieceType type)
	{
		PieceType[] hazards = Hazards;
		foreach (PieceType pieceType in hazards)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsBaddie(PieceType type)
	{
		PieceType[] baddies = Baddies;
		foreach (PieceType pieceType in baddies)
		{
			if (pieceType == type)
			{
				return true;
			}
		}
		return false;
	}

	public static float GetCurrentSpeedDelta()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		PlayerController playerController = PlayerController.Instance();
		if (playerController != null)
		{
			num = playerController.Settings.InitialSpeed;
			num2 = playerController.GetCurrentSpeed();
			if (num2 == 0f)
			{
				num2 = num;
			}
			num3 = playerController.Settings.MaxSpeed;
		}
		if (num == num3)
		{
			num = 22f;
			num3 = 50f;
			num2 = num3;
		}
		float num4 = num3 - num;
		float num5 = Mathf.Clamp(num2 - num, 1f, num4);
		return num5 / num4;
	}

	public static float GetLengthScaling(float length)
	{
		float num = 1f + GetCurrentSpeedDelta() * 2f;
		return num * StraightScalingMultiplier;
	}
}
