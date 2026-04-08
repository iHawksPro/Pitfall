using UnityEngine;

public class GameEventController : MonoBehaviour
{
	private enum PlayerTilePosition
	{
		Left = 0,
		Centre = 1,
		Right = 2
	}

	public enum eHazardReaction
	{
		Swipe_Left = 0,
		Swipe_Right = 1,
		Jump = 2,
		Slide = 3,
		None = 4
	}

	private bool mHasInitiatedTurnLeft;

	private bool mHasInitiatedTurnRight;

	private bool mHasInitiatedSwipeLeft;

	private bool mHasInitiatedSwipeRight;

	private bool mHasInitiatedJump;

	private bool mHasInitiatedSlide;

	private PieceDescriptor mPreviousPiece;

	private PlayerAnimationController mPac;

	private bool bSpeedBoostPreviouslyTakenIntoCalculations;

	private float mStartDistance;

	private float mReactDistance;

	private PlayerTilePosition mPlayerTile = PlayerTilePosition.Centre;

	private eHazardReaction mHazardReaction = eHazardReaction.None;

	private float mCurrentTimeToHazard = -1f;

	private bool mLastTileWasBranchSwipeLeft;

	private bool mLastTileWasBranchSwipeRight;

	public bool HasInitiatedTurnLeft()
	{
		return mHasInitiatedTurnLeft;
	}

	public bool HasInitiatedTurnRight()
	{
		return mHasInitiatedTurnRight;
	}

	public bool HasInitiatedSwipeLeft()
	{
		return mHasInitiatedSwipeLeft;
	}

	public void SetInitiatedSwipeLeft(bool YesNo)
	{
		mHasInitiatedSwipeLeft = YesNo;
	}

	public bool HasInitiatedSwipeRight()
	{
		return mHasInitiatedSwipeRight;
	}

	public void SetInitiatedSwipeRight(bool YesNo)
	{
		mHasInitiatedSwipeRight = YesNo;
	}

	public bool HasInitiatedJump()
	{
		return mHasInitiatedJump;
	}

	public void SetInitiatedJump(bool YesNo)
	{
		mHasInitiatedJump = YesNo;
	}

	public bool HasInitiatedSlide()
	{
		return mHasInitiatedSlide;
	}

	public void SetInitiatedSlide(bool YesNo)
	{
		mHasInitiatedSlide = YesNo;
	}

	private void Start()
	{
		mPac = PlayerController.Instance().GetComponentInChildren<PlayerAnimationController>();
	}

	private void Update()
	{
	}

	public void Reset()
	{
		mHasInitiatedTurnLeft = (mHasInitiatedTurnRight = false);
		mLastTileWasBranchSwipeLeft = (mLastTileWasBranchSwipeRight = false);
	}

	public bool ProcessCurrentTile(Tile tile, PlayerController player)
	{
		Reset();
		if (tile.IsOfType(Tile.ResponseType.Kill) && !player.IsInvincible())
		{
			return false;
		}
		if (tile.IsOfType(Tile.ResponseType.KillLeft) && !player.IsInvincible())
		{
			if (player.GetLeftRight() >= 0.52f && player.GetLeftRight() <= 0.6f)
			{
				return false;
			}
			if (player.GetLeftRight() >= -0.5f && player.GetLeftRight() <= -0.4f)
			{
				return false;
			}
			if (player.GetLeftRight() < -1.4f)
			{
				return false;
			}
		}
		else if (tile.IsOfType(Tile.ResponseType.KillRight) && !player.IsInvincible())
		{
			if (player.GetLeftRight() <= -0.52f && player.GetLeftRight() >= -0.6f)
			{
				return false;
			}
			if (player.GetLeftRight() <= 0.5f && player.GetLeftRight() >= 0.4f)
			{
				return false;
			}
			if (player.GetLeftRight() > 1.4f)
			{
				return false;
			}
		}
		bool result = true;
		int num = 0;
		int num2 = 0;
		if (tile.IsOfType(Tile.ResponseType.SwipeUp))
		{
			num++;
			if (player.IsSwipingUp() || player.IsInvincible())
			{
				num2++;
			}
		}
		if (tile.IsOfType(Tile.ResponseType.SwipeDown))
		{
			num++;
			if (player.IsSwipingDown() || player.IsInvincible())
			{
				num2++;
			}
		}
		if ((num > 0 && num2 == 0) || tile.IsOfType(Tile.ResponseType.EndOfPath))
		{
			result = false;
		}
		mLastTileWasBranchSwipeLeft = false;
		mLastTileWasBranchSwipeRight = false;
		if (tile.IsOfType(Tile.ResponseType.SwipeLeft))
		{
			mHasInitiatedTurnLeft = player.IsTurningLeft() || player.IsInSpeedBoostBonus() || player.DebugInvincible || player.IsImmuneFromDamage();
			mLastTileWasBranchSwipeLeft = true;
		}
		if (tile.IsOfType(Tile.ResponseType.SwipeRight))
		{
			mHasInitiatedTurnRight = player.IsTurningRight() || player.IsInSpeedBoostBonus() || player.DebugInvincible || player.IsImmuneFromDamage();
			mLastTileWasBranchSwipeRight = true;
		}
		if (!player.InNoDodgeZone())
		{
			if (player.IsTurningLeft())
			{
				player.DodgeLeft();
			}
			else if (player.IsTurningRight())
			{
				player.DodgeRight();
			}
		}
		return result;
	}

	public bool CanSavePlayer(PlayerController player, float pathDistance, PieceDescriptor previousPiece)
	{
		if (previousPiece == null)
		{
			return false;
		}
		if (mLastTileWasBranchSwipeLeft && player.IsTurningLeft())
		{
			return true;
		}
		if (mLastTileWasBranchSwipeRight && player.IsTurningRight())
		{
			return true;
		}
		return false;
	}

	public void ProcessAheadHazards(float distanceAlongCurrentPiece, PieceDescriptor currentPiece)
	{
		PlayerController playerController = PlayerController.Instance();
		bool flag = playerController.IsInSpeedBoostBonus() && playerController.SpeedBoostAffectsSpeed();
		if (flag != bSpeedBoostPreviouslyTakenIntoCalculations)
		{
			bSpeedBoostPreviouslyTakenIntoCalculations = flag;
			ResetAheadHazard();
		}
		if ((playerController.IsInSpeedBoostBonus() && playerController.SpeedBoostAffectsSpeed()) || playerController.DebugInvincible || playerController.IsImmuneFromDamage())
		{
			if (mPlayerTile != GetPlayerTilePosition(playerController))
			{
				if (mPac == null)
				{
					mPac = PlayerController.Instance().GetComponentInChildren<PlayerAnimationController>();
				}
				if (mPac != null && !mPac.IsJumping() && !mPac.IsSliding())
				{
					ResetAheadHazard();
					mPlayerTile = GetPlayerTilePosition(playerController);
				}
			}
			if (mHazardReaction != eHazardReaction.None)
			{
				if (playerController.Score().DistanceTravelled() > mReactDistance)
				{
					switch (mHazardReaction)
					{
					case eHazardReaction.Swipe_Left:
						mHasInitiatedSwipeLeft = true;
						break;
					case eHazardReaction.Swipe_Right:
						mHasInitiatedSwipeRight = true;
						break;
					case eHazardReaction.Jump:
						mHasInitiatedJump = true;
						break;
					case eHazardReaction.Slide:
						mHasInitiatedSlide = true;
						break;
					}
					mHazardReaction = eHazardReaction.None;
				}
			}
			else
			{
				FindNextHazard(distanceAlongCurrentPiece, currentPiece);
			}
		}
		else if (playerController.IsInSpeedBoostBonus() && !playerController.SpeedBoostAffectsSpeed())
		{
			ResetAheadHazard();
		}
	}

	private void FindNextHazard(float distanceAlongCurrentPiece, PieceDescriptor currentPiece)
	{
		PlayerController playerController = PlayerController.Instance();
		if (!playerController.IsInSpeedBoostBonus() && !playerController.DebugInvincible && !playerController.IsInvincible() && !playerController.IsImmuneFromDamage())
		{
			return;
		}
		PieceDescriptor pieceDescriptor = currentPiece;
		int num = -1;
		float num2 = currentPiece.GetLength() - distanceAlongCurrentPiece;
		if (pieceDescriptor != null)
		{
			num = pieceDescriptor.HazardRowStart;
		}
		while (num == -1 && pieceDescriptor != null)
		{
			pieceDescriptor = pieceDescriptor.GetNextPiece();
			if (pieceDescriptor != null)
			{
				num = pieceDescriptor.HazardRowStart;
				num2 += pieceDescriptor.GetLength();
			}
			else
			{
				num = -1;
			}
		}
		if (!(pieceDescriptor != null) || !(pieceDescriptor != mPreviousPiece))
		{
			return;
		}
		for (int i = 0; i < pieceDescriptor.EntryMarkup.Length; i++)
		{
			if (mPlayerTile == PlayerTilePosition.Left)
			{
				foreach (Tile.ResponseType type in pieceDescriptor.EntryMarkup[i].Left.Types)
				{
					if (!SetAResponse(type, pieceDescriptor, num2))
					{
					}
				}
			}
			else if (mPlayerTile == PlayerTilePosition.Centre)
			{
				foreach (Tile.ResponseType type2 in pieceDescriptor.EntryMarkup[i].Centre.Types)
				{
					if (type2 == Tile.ResponseType.Kill)
					{
						float num3 = Random.Range(1, 100);
						if (num3 < 50f)
						{
							if (!pieceDescriptor.EntryMarkup[i].Left.IsOfType(Tile.ResponseType.Kill))
							{
								mHazardReaction = eHazardReaction.Swipe_Left;
							}
							else
							{
								mHazardReaction = eHazardReaction.Swipe_Right;
							}
						}
						else if (!pieceDescriptor.EntryMarkup[i].Right.IsOfType(Tile.ResponseType.Kill))
						{
							mHazardReaction = eHazardReaction.Swipe_Right;
						}
						else
						{
							mHazardReaction = eHazardReaction.Swipe_Left;
						}
						SetHazardPosition(pieceDescriptor, num2);
					}
					else if (!SetAResponse(type2, pieceDescriptor, num2))
					{
					}
				}
			}
			else
			{
				foreach (Tile.ResponseType type3 in pieceDescriptor.EntryMarkup[i].Right.Types)
				{
					if (!SetAResponse(type3, pieceDescriptor, num2))
					{
					}
				}
			}
			if (mHazardReaction != eHazardReaction.None)
			{
				i = pieceDescriptor.EntryMarkup.Length;
			}
		}
		mPreviousPiece = pieceDescriptor;
	}

	private bool SetAResponse(Tile.ResponseType tileType, PieceDescriptor projectedPiece, float distanceToPiece)
	{
		switch (tileType)
		{
		case Tile.ResponseType.Kill:
			if (mPlayerTile == PlayerTilePosition.Left)
			{
				mHazardReaction = eHazardReaction.Swipe_Right;
			}
			else if (mPlayerTile == PlayerTilePosition.Right)
			{
				mHazardReaction = eHazardReaction.Swipe_Left;
			}
			SetHazardPosition(projectedPiece, distanceToPiece);
			return true;
		case Tile.ResponseType.SwipeDown:
			if (PlayerController.Instance().GetThemeType() == PlayerTheme.ThemeType.Jaguar)
			{
				mHazardReaction = eHazardReaction.Jump;
			}
			else
			{
				mHazardReaction = eHazardReaction.Slide;
			}
			SetHazardPosition(projectedPiece, distanceToPiece);
			return true;
		case Tile.ResponseType.SwipeUp:
		case Tile.ResponseType.RopeSwing:
			mHazardReaction = eHazardReaction.Jump;
			SetHazardPosition(projectedPiece, distanceToPiece);
			return true;
		default:
			return false;
		}
	}

	private void SetHazardPosition(PieceDescriptor projectedPiece, float distanceToPiece)
	{
		PlayerController playerController = PlayerController.Instance();
		float length = projectedPiece.GetLength();
		distanceToPiece -= length;
		Vector3 forward = playerController.PlayerModel.transform.forward;
		Vector3 vector = playerController.PlayerModel.transform.position + distanceToPiece * forward;
		int hazardRowStart = projectedPiece.HazardRowStart;
		if (hazardRowStart != -1)
		{
			vector += forward * hazardRowStart;
		}
		float magnitude = (vector - playerController.PlayerModel.transform.position).magnitude;
		mCurrentTimeToHazard = magnitude / playerController.GetCurrentSpeedWithoutSpeedBonuses();
		mStartDistance = playerController.Score().DistanceTravelled();
		float num = playerController.GetJumpDuration() * playerController.GetCurrentSpeed() * 0.33f;
		mReactDistance = mStartDistance + magnitude - num;
	}

	private PlayerTilePosition GetPlayerTilePosition(PlayerController player)
	{
		PlayerTilePosition result = PlayerTilePosition.Centre;
		if (player.GetLeftRight() < -0.5f)
		{
			result = PlayerTilePosition.Left;
		}
		else if (player.GetLeftRight() > 0.5f)
		{
			result = PlayerTilePosition.Right;
		}
		return result;
	}

	private PieceDescriptor GetPieceAheadAtDistance(PieceDescriptor currentPiece, float distance)
	{
		if (currentPiece != null)
		{
			float num = 0f;
			PieceDescriptor pieceDescriptor = currentPiece;
			while (num + pieceDescriptor.GetLength() < distance)
			{
				num += pieceDescriptor.GetLength();
				pieceDescriptor = pieceDescriptor.GetNextPiece();
				if (pieceDescriptor == null)
				{
					return null;
				}
			}
			if ((bool)pieceDescriptor)
			{
				return pieceDescriptor;
			}
		}
		return null;
	}

	public void ResetAheadHazard()
	{
		mHazardReaction = eHazardReaction.None;
		mCurrentTimeToHazard = 0f;
		mPreviousPiece = null;
	}
}
