using UnityEngine;

public class InputDebug : MonoBehaviour
{
	private FloatValueSlider m_CentreTolerance;

	private FloatValueSlider m_TiltScale;

	private FloatValueSlider m_TiltSpeed;

	private IntValueSlider m_NumSamples;

	private FloatValueSlider m_TiltSideStepTolerance;

	private BoolValueSlider m_ReturnToCentre;

	private FloatValueSlider m_TurnRate;

	private FloatValueSlider m_TiltSmoothing;

	private FloatValueSlider m_TiltSlideModifier;

	private FloatValueSlider m_DodgeTiltSpeed;

	private FloatValueSlider m_DodgeTiltTime;

	private BoolValueSlider m_GodMode;

	private DebugButton m_AuthoredLevel;

	private BoolValueSlider m_GenerateMeteors;

	private BoolValueSlider m_DisplayCurrentSpeed;

	private BoolValueSlider m_OverrideHeuristics;

	private DebugLabel m_MetreMark;

	private DebugButton m_UpMetreMark;

	private DebugButton m_DownMetreMark;

	private DebugLabel m_ReactionTime;

	private DebugButton m_UpReactionTime;

	private DebugButton m_DownReactionTime;

	private DebugLabel m_PlayerSpeed;

	private DebugButton m_UpPlayerSpeed;

	private DebugButton m_DownPlayerSpeed;

	private BoolValueSlider m_HeuristicsSpam;

	private bool m_Visible;

	private bool m_Debounce;

	private void Start()
	{
		m_Visible = false;
		m_Debounce = false;
	}

	private void OnVisible()
	{
		float xpos = 20f;
		float ypos = 200f;
		PlayerThemeSettings playerThemeSettings = PlayerController.Instance().CurrentTheme();
		m_CentreTolerance = new FloatValueSlider("Centre Tolerance", xpos, ref ypos, playerThemeSettings.CentreTolerance, 0f, 0.4f);
		m_TiltScale = new FloatValueSlider("Tilt Scale", xpos, ref ypos, playerThemeSettings.TiltScale, 0f, 20f);
		m_TiltSpeed = new FloatValueSlider("Tilt Speed", xpos, ref ypos, playerThemeSettings.TiltSpeed, 0f, 20f);
		m_NumSamples = new IntValueSlider("Num Samples", xpos, ref ypos, playerThemeSettings.SampleSize, 1, 20);
		m_TiltSideStepTolerance = new FloatValueSlider("Tilt/Slide Tolerance", xpos, ref ypos, playerThemeSettings.TiltSideStepTolerance, 0f, 1f);
		m_ReturnToCentre = new BoolValueSlider("Return to centre", xpos, ref ypos, playerThemeSettings.ReturnToCentre);
		m_TurnRate = new FloatValueSlider("Turn Rate", xpos, ref ypos, playerThemeSettings.TurnRate, 0f, 2f);
		m_TiltSmoothing = new FloatValueSlider("Tilt smoothing", xpos, ref ypos, playerThemeSettings.TiltSmoothing, 0f, 30f);
		m_TiltSlideModifier = new FloatValueSlider("Tilt Slide Modifier", xpos, ref ypos, playerThemeSettings.SlideTiltSpeedModifier, 0f, 1f);
		xpos = 300f;
		ypos = 200f;
		m_DodgeTiltSpeed = new FloatValueSlider("Dodge Tilt Speed", xpos, ref ypos, playerThemeSettings.DodgeTiltSpeed, 0f, 20f);
		m_DodgeTiltTime = new FloatValueSlider("Dodge Tilt Time", xpos, ref ypos, playerThemeSettings.DodgeTiltTime, 0f, 2f);
		ypos += 60f;
		m_GodMode = new BoolValueSlider("God Mode", xpos, ref ypos, PlayerController.Instance().DebugInvincible);
		m_AuthoredLevel = new DebugButton("Authored Level", xpos, ref ypos);
		m_GenerateMeteors = new BoolValueSlider("Generate Meteors", xpos, ref ypos, LevelGenerator.Instance().GenerateMeteors);
		ypos += 60f;
		m_DisplayCurrentSpeed = new BoolValueSlider("Display Player Debug", xpos, ref ypos, PlayerController.Instance().DisplaySpeed);
		xpos = 580f;
		ypos = 200f;
		m_OverrideHeuristics = new BoolValueSlider("Override Level Heuristics", xpos, ref ypos, PlayerController.Instance().DisplaySpeed);
		m_MetreMark = new DebugLabel(xpos, ref ypos);
		m_UpMetreMark = new DebugButton("Plus 100 Metres", xpos, ref ypos);
		m_DownMetreMark = new DebugButton("Minus 100 Metres", xpos, ref ypos);
		m_ReactionTime = new DebugLabel(xpos, ref ypos);
		m_UpReactionTime = new DebugButton("Plus Reaction Time", xpos, ref ypos);
		m_DownReactionTime = new DebugButton("Minus Reaction Time", xpos, ref ypos);
		m_PlayerSpeed = new DebugLabel(xpos, ref ypos);
		m_UpPlayerSpeed = new DebugButton("Plus Player Speed", xpos, ref ypos);
		m_DownPlayerSpeed = new DebugButton("Minus Player Speed", xpos, ref ypos);
		ypos += 40f;
		m_HeuristicsSpam = new BoolValueSlider("Display Heuristics Debug", xpos, ref ypos, WorldConstructionController.Instance().OutputDebugSpam);
	}

	private void OnNotVisible()
	{
		m_CentreTolerance = null;
		m_TiltScale = null;
		m_TiltSpeed = null;
		m_NumSamples = null;
		m_TiltSideStepTolerance = null;
		m_ReturnToCentre = null;
		m_TurnRate = null;
		m_TiltSmoothing = null;
		m_TiltSlideModifier = null;
		m_DodgeTiltSpeed = null;
		m_DodgeTiltTime = null;
		m_GodMode = null;
		m_AuthoredLevel = null;
		m_GenerateMeteors = null;
		m_DisplayCurrentSpeed = null;
		m_OverrideHeuristics = null;
		m_MetreMark = null;
		m_UpMetreMark = null;
		m_DownMetreMark = null;
		m_ReactionTime = null;
		m_UpReactionTime = null;
		m_DownReactionTime = null;
		m_PlayerSpeed = null;
		m_UpPlayerSpeed = null;
		m_DownPlayerSpeed = null;
		m_HeuristicsSpam = null;
	}

	private void OnUpdate()
	{
		PlayerThemeSettings playerThemeSettings = PlayerController.Instance().CurrentTheme();
		playerThemeSettings.CentreTolerance = m_CentreTolerance.OnGui();
		playerThemeSettings.TiltScale = m_TiltScale.OnGui();
		playerThemeSettings.TiltSpeed = m_TiltSpeed.OnGui();
		playerThemeSettings.SampleSize = m_NumSamples.OnGui();
		playerThemeSettings.TiltSideStepTolerance = m_TiltSideStepTolerance.OnGui();
		playerThemeSettings.ReturnToCentre = m_ReturnToCentre.OnGui();
		playerThemeSettings.TurnRate = m_TurnRate.OnGui();
		playerThemeSettings.TiltSmoothing = m_TiltSmoothing.OnGui();
		playerThemeSettings.SlideTiltSpeedModifier = m_TiltSlideModifier.OnGui();
		playerThemeSettings.DodgeTiltSpeed = m_DodgeTiltSpeed.OnGui();
		playerThemeSettings.DodgeTiltTime = m_DodgeTiltTime.OnGui();
		PlayerController.Instance().DebugInvincible = m_GodMode.OnGui();
		LevelGenerator levelGenerator = LevelGenerator.Instance();
		levelGenerator.GenerateMeteors = m_GenerateMeteors.OnGui();
		PlayerController.Instance().DisplaySpeed = m_DisplayCurrentSpeed.OnGui();
		if (m_AuthoredLevel.OnGui())
		{
			AuthoredLevelController authoredLevelController = AuthoredLevelController.Instance();
			if (authoredLevelController == null)
			{
				return;
			}
			AuthoredLevelLayoutController debugMenuLevel = authoredLevelController.GetDebugMenuLevel();
			if (debugMenuLevel != null)
			{
				if (authoredLevelController.IsGlobalWorldActive())
				{
					authoredLevelController.SetGlobalWorld(null);
				}
				else
				{
					authoredLevelController.SetGlobalWorld(debugMenuLevel);
				}
			}
		}
		WorldConstructionController worldConstructionController = WorldConstructionController.Instance();
		worldConstructionController.DebugOverrides.OverrideSettings = m_OverrideHeuristics.OnGui();
		m_MetreMark.OnGui(worldConstructionController.DebugOverrides.MetreMark.ToString());
		if (m_UpMetreMark.OnGui())
		{
			worldConstructionController.DebugOverrides.MetreMark += 100;
		}
		if (m_DownMetreMark.OnGui())
		{
			worldConstructionController.DebugOverrides.MetreMark -= 100;
		}
		m_ReactionTime.OnGui(worldConstructionController.DebugOverrides.ReactionTime.ToString());
		if (m_UpReactionTime.OnGui())
		{
			worldConstructionController.DebugOverrides.ReactionTime += 0.1f;
		}
		if (m_DownReactionTime.OnGui())
		{
			worldConstructionController.DebugOverrides.ReactionTime -= 0.1f;
		}
		m_PlayerSpeed.OnGui(worldConstructionController.DebugOverrides.PlayerSpeed.ToString());
		if (m_UpPlayerSpeed.OnGui())
		{
			worldConstructionController.DebugOverrides.PlayerSpeed += 1f;
		}
		if (m_DownPlayerSpeed.OnGui())
		{
			worldConstructionController.DebugOverrides.PlayerSpeed -= 1f;
		}
		worldConstructionController.DebugOverrides.MetreMark = Mathf.Max(0, worldConstructionController.DebugOverrides.MetreMark);
		worldConstructionController.DebugOverrides.ReactionTime = Mathf.Clamp(worldConstructionController.DebugOverrides.ReactionTime, -5f, 5f);
		worldConstructionController.DebugOverrides.PlayerSpeed = Mathf.Clamp(worldConstructionController.DebugOverrides.PlayerSpeed, 14f, 40f);
		WorldConstructionController.Instance().OutputDebugSpam = m_HeuristicsSpam.OnGui();
	}

	private void OnGUI()
	{
		if (PlayerController.Instance().IsDebug() && GameController.Instance.Paused())
		{
			if (!m_Debounce)
			{
				if (m_Visible)
				{
					m_Visible = false;
					OnNotVisible();
				}
				else
				{
					m_Visible = true;
					OnVisible();
				}
				m_Debounce = true;
			}
		}
		else
		{
			m_Debounce = false;
		}
		if (m_Visible)
		{
			OnUpdate();
		}
	}
}
