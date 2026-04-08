using UnityEngine;

public class Baddie
{
	public enum TilePos
	{
		Left = 0,
		Centre = 1,
		Right = 2
	}

	private bool mDead;

	private float mDeadTimer;

	private bool mHarmless;

	private TilePos mTilePos;

	private BaddieController.Type mBaddieType;

	private GameObject mModel;

	private GameObject mModelExplode;

	private PieceDescriptor mPiece;

	private float mDistanceIntoPiece;

	private float mHorizontalPosition;

	private float mWalkSpeed;

	private bool mHasPoisonedPlayer;

	private SoundFXData mIdleSFX;

	private SoundFXData mHitSfx;

	private SoundFXData mMissSfx;

	private SoundFXData mDeathSfx;

	public BaddieController.Type BaddieType
	{
		get
		{
			return mBaddieType;
		}
	}

	public float HorizontalPosition
	{
		get
		{
			return mHorizontalPosition;
		}
	}

	public Baddie(BaddieController.Type baddieType, GameObject model, GameObject modelExplode, PieceDescriptor piece, float spawnDistanceIntoPiece, float horizontalPosition, TilePos tilePos)
	{
		mTilePos = tilePos;
		mBaddieType = baddieType;
		mModel = model;
		mModelExplode = modelExplode;
		mPiece = piece;
		mDistanceIntoPiece = spawnDistanceIntoPiece;
		mHorizontalPosition = horizontalPosition;
		mWalkSpeed = Random.Range(1f, 5f);
		Vector3 localScale = mModel.transform.localScale;
		mModel.transform.localScale = localScale;
		if (mModelExplode != null)
		{
			mModelExplode.transform.localScale = localScale;
		}
		mDead = (mHasPoisonedPlayer = false);
		mDeadTimer = 0f;
	}

	public void MakeHarmless()
	{
		mHarmless = true;
	}

	public void Kill()
	{
		if (!mDead)
		{
			PlayerController playerController = PlayerController.Instance();
			if (playerController != null)
			{
				playerController.Score().AddAnimalKill(mBaddieType);
				playerController.SFX.PlayBaddieKill(mBaddieType);
			}
			mDead = true;
			mDeadTimer = 2f;
			PlayKilledAnim();
			ResetSFX();
		}
	}

	public GameObject GetModel()
	{
		return mModel;
	}

	public void Update()
	{
		if (PlayerController.Instance().IsDead())
		{
			return;
		}
		if (mDead)
		{
			if (!(mDeadTimer > 0f))
			{
				return;
			}
			mDeadTimer -= Time.deltaTime;
			if (mDeadTimer <= 0f && (bool)mModel)
			{
				if (mModelExplode != null)
				{
					mModelExplode.SetActiveRecursively(true);
					mModelExplode.transform.position = mModel.transform.position;
					mModelExplode.transform.forward = mModel.transform.forward;
				}
				mModel.transform.position = new Vector3(mModel.transform.position.x, mModel.transform.position.y + 1000f, mModel.transform.position.z);
			}
		}
		else
		{
			if (mHarmless)
			{
				return;
			}
			PlayerController playerController = PlayerController.Instance();
			float sqrMagnitude = (playerController.transform.position - mModel.transform.position).sqrMagnitude;
			float num = 0.6f;
			if (mBaddieType == BaddieController.Type.Snake || mBaddieType == BaddieController.Type.Scorpion)
			{
				num = 0.3f;
			}
			float num2 = playerController.GetCurrentSpeed() * num;
			if (PlayerController.Instance().IsDead())
			{
				return;
			}
			if (sqrMagnitude < num2 * num2)
			{
				if (playerController.GetThemeType() == PlayerTheme.ThemeType.Bike || playerController.GetThemeType() == PlayerTheme.ThemeType.Cart)
				{
					float num3 = 0.3f * playerController.GetCurrentSpeed();
					if (sqrMagnitude < num3 * num3)
					{
						Kill();
					}
					return;
				}
				if (!IsPlayingAttackAnimation())
				{
					PlayAttackAnimation(playerController);
					if (BaddieType != BaddieController.Type.Crocodile)
					{
						SoundManager.Instance.Play(mHitSfx, mModel);
					}
				}
				if (!(sqrMagnitude < 8f))
				{
					return;
				}
				bool flag = CanKillPlayer();
				if (OutfitOfTheDayManager.Instance.BonusApplies(Costume.Bear) && BaddieType != BaddieController.Type.Crocodile)
				{
					flag = false;
				}
				if (flag)
				{
					if (mBaddieType == BaddieController.Type.Crocodile)
					{
						playerController.Kill(PieceDescriptor.KillType.Crocodile);
						BaddieController.Instance().KillBaddie = this;
						SoundManager.Instance.Play(mHitSfx, mModel);
					}
					else if (playerController.IsPoisoned())
					{
						playerController.Kill(PieceDescriptor.KillType.Poison);
					}
					else
					{
						playerController.Poison(mBaddieType);
					}
					mHasPoisonedPlayer = true;
				}
			}
			else
			{
				if (mBaddieType != BaddieController.Type.Scorpion)
				{
					return;
				}
				float num4 = playerController.GetCurrentSpeed() * 8f;
				if (sqrMagnitude < num4 * num4)
				{
					mDistanceIntoPiece -= Time.deltaTime * mWalkSpeed;
					if (mDistanceIntoPiece <= 0f)
					{
						mDistanceIntoPiece = 0f;
					}
					mModel.transform.position = mPiece.GetWorldPosition(mDistanceIntoPiece, mHorizontalPosition);
					mModel.transform.forward = -mPiece.GetWorldDirection(mDistanceIntoPiece, mHorizontalPosition);
				}
			}
		}
	}

	public bool CanKillPlayer()
	{
		PlayerController playerController = PlayerController.Instance();
		if (mHasPoisonedPlayer || playerController.IsInvincible() || playerController.IsInSpeedBoostBonus() || playerController.IsImmuneFromDamage())
		{
			return false;
		}
		float leftRight = playerController.GetLeftRight();
		bool flag = playerController.GetPlayerAnimationController().IsJumping();
		if (mBaddieType == BaddieController.Type.Crocodile)
		{
			switch (mTilePos)
			{
			case TilePos.Left:
				if (leftRight > 0.5f)
				{
					return false;
				}
				if (!flag)
				{
					return true;
				}
				break;
			case TilePos.Centre:
				Debug.LogWarning("Croc tile pos == centre which should not happen!");
				return false;
			case TilePos.Right:
				if (leftRight < -0.5f)
				{
					return false;
				}
				if (!flag)
				{
					return true;
				}
				break;
			}
		}
		else
		{
			switch (mTilePos)
			{
			case TilePos.Left:
				if (leftRight > 0.5f)
				{
					return false;
				}
				if (leftRight < -0.5f)
				{
					return true;
				}
				if (!flag)
				{
					return true;
				}
				break;
			case TilePos.Centre:
				if (!flag)
				{
					return true;
				}
				if (Mathf.Abs(leftRight) < 0.5f)
				{
					return true;
				}
				break;
			case TilePos.Right:
				if (leftRight < -0.5f)
				{
					return false;
				}
				if (leftRight > 0.5f)
				{
					return true;
				}
				if (!flag)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	public bool IsValid()
	{
		return mModel != null;
	}

	public bool IsAlive()
	{
		return !mDead && mModel != null;
	}

	public string GetAttackAnimation()
	{
		switch (mBaddieType)
		{
		case BaddieController.Type.Crocodile:
			if (mTilePos == TilePos.Left)
			{
				return "DeathRoll";
			}
			return "DeathRoll_Right";
		case BaddieController.Type.Spider:
			return "Attack";
		default:
			return string.Empty;
		}
	}

	private bool IsPlayingAttackAnimation()
	{
		switch (mBaddieType)
		{
		case BaddieController.Type.Scorpion:
			return mModel.GetComponent<Animation>().IsPlaying("Attack");
		case BaddieController.Type.Snake:
			return mModel.GetComponent<Animation>().IsPlaying("AttackLeft") || mModel.GetComponent<Animation>().IsPlaying("AttackRight") || mModel.GetComponent<Animation>().IsPlaying("AttackUp") || mModel.GetComponent<Animation>().IsPlaying("AttackFront");
		case BaddieController.Type.Crocodile:
			if (!mModel.GetComponent<Animation>().IsPlaying("Idle") && !mModel.GetComponent<Animation>().IsPlaying("Idle_Right"))
			{
				return true;
			}
			return false;
		default:
			return mModel.GetComponent<Animation>().IsPlaying(GetAttackAnimation());
		}
	}

	public void PlayKilledAnim()
	{
		mModel.GetComponent<Animation>().wrapMode = WrapMode.Once;
		switch (mBaddieType)
		{
		case BaddieController.Type.Snake:
			mModel.GetComponent<Animation>().Play("Die");
			SoundManager.Instance.Play(mDeathSfx, mModel);
			break;
		case BaddieController.Type.Scorpion:
			mModel.GetComponent<Animation>().Play("Die");
			SoundManager.Instance.Play(mDeathSfx, mModel);
			break;
		case BaddieController.Type.Crocodile:
			if (mTilePos == TilePos.Left)
			{
				mModel.GetComponent<Animation>().Play("Whipped");
			}
			else
			{
				mModel.GetComponent<Animation>().Play("WhippedRight");
			}
			SoundManager.Instance.Play(mDeathSfx, mModel);
			break;
		}
	}

	private void PlayAttackAnimation(PlayerController player)
	{
		mModel.GetComponent<Animation>().wrapMode = WrapMode.Once;
		switch (mBaddieType)
		{
		case BaddieController.Type.Scorpion:
			mModel.GetComponent<Animation>().Play("Attack");
			break;
		case BaddieController.Type.Snake:
		{
			float leftRight = player.GetLeftRight();
			if (Mathf.Abs(leftRight - mHorizontalPosition) <= 0.5f)
			{
				if (player.GetPlayerAnimationController() != null)
				{
					if (player.GetPlayerAnimationController().IsJumping())
					{
						mModel.GetComponent<Animation>().Play("AttackUp");
					}
					else
					{
						mModel.GetComponent<Animation>().Play("AttackFront");
					}
				}
			}
			else if (leftRight < mHorizontalPosition)
			{
				mModel.GetComponent<Animation>().Play("AttackLeft");
			}
			else
			{
				mModel.GetComponent<Animation>().Play("AttackRight");
			}
			break;
		}
		case BaddieController.Type.Crocodile:
			if (mTilePos == TilePos.Left)
			{
				mModel.GetComponent<Animation>().CrossFade("ReadyToAttack", 0.2f);
			}
			else
			{
				mModel.GetComponent<Animation>().CrossFade("ReadyToAttack_Right", 0.2f);
			}
			break;
		default:
			mModel.GetComponent<Animation>().Play(GetAttackAnimation());
			break;
		}
	}

	public void SetTilePos(TilePos tilePos)
	{
		mTilePos = tilePos;
	}

	public void SetSFX(SoundFXData idleSFX, SoundFXData hitSFX, SoundFXData missSFX, SoundFXData deathSFX)
	{
		mIdleSFX = idleSFX;
		mHitSfx = hitSFX;
		mMissSfx = missSFX;
		mDeathSfx = deathSFX;
	}

	public void PlayIdleSFX()
	{
		SoundManager.Instance.Play(mIdleSFX, mModel);
	}

	public void ResetSFX()
	{
		SoundManager.Instance.Stop(mIdleSFX, mModel);
		SoundManager.Instance.Stop(mHitSfx, mModel);
		SoundManager.Instance.Stop(mMissSfx, mModel);
	}
}
