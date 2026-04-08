using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaddieController : MonoBehaviour
{
	public enum Type
	{
		Crocodile = 0,
		Scorpion = 1,
		Snake = 2,
		Spider = 3,
		Max = 4
	}

	public GameObject Crocodile;

	public GameObject Scorpion;

	public GameObject Snake;

	public SoundFXData CrocSoundIdle;

	public SoundFXData CrocSoundAttack;

	public SoundFXData CrocSoundMiss;

	public SoundFXData CrocSoundAngry;

	public SoundFXData SnakeSoundIdle;

	public SoundFXData SnakeSoundAttack;

	public SoundFXData SnakeSoundMiss;

	public SoundFXData SnakeSoundDeath;

	public SoundFXData ScorpSoundIdle;

	public SoundFXData ScorpSoundAttack;

	public SoundFXData ScorpSoundMiss;

	public SoundFXData ScorpSoundDeath;

	[HideInInspector]
	public GameObject Spider;

	[HideInInspector]
	public GameObject Crocodile_Explode;

	[HideInInspector]
	public GameObject Scorpion_Explode;

	[HideInInspector]
	public GameObject Snake_Explode;

	[HideInInspector]
	public GameObject Spider_Explode;

	public Baddie KillBaddie;

	public static BaddieController instance;

	private bool[] mTypeAllowed = new bool[4];

	private List<Baddie> mActiveBaddies = new List<Baddie>();

	public static BaddieController Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		Reset();
	}

	private void Update()
	{
		for (int i = 0; i < mActiveBaddies.Count; i++)
		{
			if (!mActiveBaddies[i].IsValid())
			{
				mActiveBaddies[i].ResetSFX();
				mActiveBaddies.RemoveAt(i);
			}
			else
			{
				mActiveBaddies[i].Update();
			}
		}
	}

	public bool KillAllInRadius(Vector3 pos, float radius)
	{
		bool result = false;
		for (int i = 0; i < mActiveBaddies.Count; i++)
		{
			if (mActiveBaddies[i].IsAlive() && (mActiveBaddies[i].GetModel().transform.position - pos).sqrMagnitude < radius * radius)
			{
				float num = 2.5f;
				float num2 = mActiveBaddies[i].GetModel().transform.position.y - PlayerController.Instance().transform.position.y;
				if (num2 > 0f - num && num2 < num)
				{
					StartCoroutine(DoBaddieDeathAnim(mActiveBaddies[i]));
					mActiveBaddies[i].MakeHarmless();
					result = true;
				}
			}
		}
		return result;
	}

	public bool AreAnyBaddiesInRadius(Vector3 pos, float radius)
	{
		bool result = false;
		for (int i = 0; i < mActiveBaddies.Count; i++)
		{
			if (mActiveBaddies[i].IsAlive() && (mActiveBaddies[i].GetModel().transform.position - pos).sqrMagnitude < radius * radius)
			{
				float num = 2.5f;
				float num2 = mActiveBaddies[i].GetModel().transform.position.y - PlayerController.Instance().transform.position.y;
				if (num2 > 0f - num && num2 < num)
				{
					result = true;
				}
			}
		}
		return result;
	}

	protected IEnumerator DoBaddieDeathAnim(Baddie baddieToKill)
	{
		yield return new WaitForSeconds(0.1333f);
		baddieToKill.Kill();
	}

	public void Reset()
	{
		for (int i = 0; i < 4; i++)
		{
			mTypeAllowed[i] = true;
		}
		KillBaddie = null;
		for (int j = 0; j < mActiveBaddies.Count; j++)
		{
			mActiveBaddies[j].ResetSFX();
		}
	}

	public bool IsAllowed(Type type)
	{
		return mTypeAllowed[(int)type];
	}

	public void SetAllowBaddie(Type type, bool val)
	{
		mTypeAllowed[(int)type] = val;
	}

	public void SpawnNewPiece(PieceDescriptor piece, Type allowedType)
	{
		switch (allowedType)
		{
		case Type.Crocodile:
		{
			bool alternating = false;
			SpawnCrocodile(piece, alternating);
			break;
		}
		case Type.Scorpion:
			SpawnScorpion(piece);
			break;
		case Type.Snake:
			SpawnSnake(piece);
			break;
		case Type.Spider:
			SpawnSnake(piece);
			break;
		}
	}

	private void SpawnCrocodile(PieceDescriptor piece, bool alternating)
	{
		GameObject gameObject = Object.Instantiate(Crocodile) as GameObject;
		float spawnDistanceIntoPiece = piece.GetCachedLength() * 0.5f;
		SpawnBaddie(piece, gameObject, null, Type.Crocodile, spawnDistanceIntoPiece, 0f, CrocSoundIdle, CrocSoundAttack, CrocSoundMiss, CrocSoundAngry);
		if (Random.Range(0, 2) > 0)
		{
			mActiveBaddies[mActiveBaddies.Count - 1].SetTilePos(Baddie.TilePos.Left);
			gameObject.GetComponent<Animation>().Play("Idle");
		}
		else
		{
			mActiveBaddies[mActiveBaddies.Count - 1].SetTilePos(Baddie.TilePos.Right);
			gameObject.GetComponent<Animation>().Play("Idle_Right");
		}
		gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
		gameObject.GetComponent<Animation>().wrapMode = WrapMode.Loop;
	}

	private void SpawnScorpion(PieceDescriptor piece)
	{
		GameObject gameObject = Object.Instantiate(Scorpion) as GameObject;
		float spawnDistanceIntoPiece = piece.GetCachedLength() * 0.9f;
		SpawnBaddie(piece, gameObject, null, Type.Scorpion, spawnDistanceIntoPiece, 0f, ScorpSoundIdle, ScorpSoundAttack, ScorpSoundMiss, ScorpSoundDeath);
		gameObject.GetComponent<Animation>().Play("Walk");
		gameObject.GetComponent<Animation>().wrapMode = WrapMode.Loop;
	}

	private void SpawnSnake(PieceDescriptor piece)
	{
		GameObject gameObject = Object.Instantiate(Snake) as GameObject;
		float spawnDistanceIntoPiece = piece.GetCachedLength() * 0.5f;
		SpawnBaddie(piece, gameObject, null, Type.Snake, spawnDistanceIntoPiece, 0f, SnakeSoundIdle, SnakeSoundAttack, SnakeSoundMiss, SnakeSoundDeath);
		gameObject.GetComponent<Animation>().Play("IdleBreathe");
		gameObject.GetComponent<Animation>().wrapMode = WrapMode.Loop;
	}

	private void SpawnSpider(PieceDescriptor piece)
	{
		GameObject gameObject = Object.Instantiate(Spider) as GameObject;
		float spawnDistanceIntoPiece = piece.GetCachedLength() * 0.5f;
		SpawnBaddie(piece, gameObject, null, Type.Spider, spawnDistanceIntoPiece, 1.5f, null, null, null, null);
		gameObject.GetComponent<Animation>().Play("Idle");
		gameObject.GetComponent<Animation>().wrapMode = WrapMode.Loop;
	}

	private void SpawnBaddie(PieceDescriptor piece, GameObject baddieCloneModel, GameObject baddieExplodeCloneModel, Type baddieType, float spawnDistanceIntoPiece, float horizontalRange, SoundFXData idleSFX, SoundFXData hitSFX, SoundFXData missSFX, SoundFXData deathSFX)
	{
		int num = Random.Range(0, 3);
		Baddie.TilePos tilePos = Baddie.TilePos.Centre;
		float num2 = 0f;
		if (horizontalRange > 0.01f)
		{
			switch (num)
			{
			case 0:
				num2 = 0f - horizontalRange;
				tilePos = Baddie.TilePos.Left;
				break;
			case 1:
				num2 = 0f;
				tilePos = Baddie.TilePos.Centre;
				break;
			case 2:
				num2 = horizontalRange;
				tilePos = Baddie.TilePos.Right;
				break;
			}
		}
		Vector3 worldPosition = piece.GetWorldPosition(spawnDistanceIntoPiece, num2);
		baddieCloneModel.transform.position = worldPosition;
		baddieCloneModel.transform.forward = -piece.GetWorldDirection(spawnDistanceIntoPiece, num2);
		baddieCloneModel.transform.parent = piece.transform;
		baddieCloneModel.SetActiveRecursively(true);
		if (baddieExplodeCloneModel != null)
		{
			baddieExplodeCloneModel.transform.parent = piece.transform;
			baddieExplodeCloneModel.SetActiveRecursively(false);
			baddieExplodeCloneModel.GetComponent<Animation>().Rewind();
		}
		Baddie baddie = new Baddie(baddieType, baddieCloneModel, baddieExplodeCloneModel, piece, spawnDistanceIntoPiece, num2, tilePos);
		baddie.SetSFX(idleSFX, hitSFX, missSFX, deathSFX);
		baddie.PlayIdleSFX();
		mActiveBaddies.Add(baddie);
		piece.BaddieType = baddieType;
	}
}
