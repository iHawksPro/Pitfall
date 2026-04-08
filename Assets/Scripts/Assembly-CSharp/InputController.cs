using UnityEngine;

public class InputController : MonoBehaviour
{
	public enum SwipeBias
	{
		Horizontal = 0,
		Vertical = 1,
		None = 2
	}

	public delegate void ButtonPressFunction(PlayerYoke instance);

	public PlayerController CurrentPlayerController;

	public static InputController instance;

	private KeyHandler mKeyHandler;

	private SwipeHandler mSwipeHandler;

	private TiltHandler mTiltHandler;

	private SwipeBias mSwipeBias;

	private float mTapBias;

	private bool mEnableMouseInput;

	public static InputController Instance()
	{
		return instance;
	}

	public void SetSwipeBias(SwipeBias bias)
	{
		mSwipeBias = bias;
	}

	public void SetTapBias(float bias)
	{
		mTapBias = bias;
	}

	public void EnableMouseInput(bool setting)
	{
		mEnableMouseInput = setting;
	}

	private void Awake()
	{
		instance = this;
		mKeyHandler = new KeyHandler();
		mSwipeHandler = new SwipeHandler();
		mTiltHandler = new TiltHandler();
		mSwipeBias = SwipeBias.None;
		mTapBias = 1f;
	}

	private void Start()
	{
		mKeyHandler.Add("Jump", PlayerYoke.SetJump, KeyHandler.ButtonInputType.BUTTON_PRESS);
		mKeyHandler.Add("Slide", PlayerYoke.SetSlide, KeyHandler.ButtonInputType.BUTTON_PRESS);
		mKeyHandler.Add("TurnLeft", PlayerYoke.SetTurnLeft, KeyHandler.ButtonInputType.BUTTON_PRESS);
		mKeyHandler.Add("TurnRight", PlayerYoke.SetTurnRight, KeyHandler.ButtonInputType.BUTTON_PRESS);
		mKeyHandler.Add("TiltLeft", PlayerYoke.SetTiltLeft, KeyHandler.ButtonInputType.BUTTON_HOLD);
		mKeyHandler.Add("TiltRight", PlayerYoke.SetTiltRight, KeyHandler.ButtonInputType.BUTTON_HOLD);
		mKeyHandler.Add("Attack", PlayerYoke.SetTap, KeyHandler.ButtonInputType.BUTTON_PRESS);
		mEnableMouseInput = Application.isMobilePlatform;
	}

	public void Reset()
	{
		PlayerYoke playerYoke = CurrentPlayerController.GetComponent(typeof(PlayerYoke)) as PlayerYoke;
		playerYoke.Reset();
		mKeyHandler.Reset();
		mSwipeHandler.Reset();
		mTiltHandler.Reset();
	}

	private void Update()
	{
		PlayerYoke playerYoke = CurrentPlayerController.GetComponent(typeof(PlayerYoke)) as PlayerYoke;
		playerYoke.Reset();
		mKeyHandler.Update(playerYoke);
		mSwipeHandler.Update(playerYoke, mSwipeBias, mTapBias, mEnableMouseInput);
		mTiltHandler.Update(playerYoke);
	}

	public void DebugRender()
	{
	}

	public void RegisterHoldTouch(int holdTouchId)
	{
		TBFUtils.DebugLog("Register Hold Touch: " + holdTouchId);
		mSwipeHandler.SetHoldTouchId(holdTouchId);
	}

	public void DeregisterHoldTouch(int holdTouchId)
	{
		TBFUtils.DebugLog("De-register Hold Touch: " + holdTouchId);
		mSwipeHandler.SetHoldTouchId(-1);
	}
}
