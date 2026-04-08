using System;
using System.Collections.Generic;

[Serializable]
public class ReactionTimeSettings
{
	public List<ReactionTimePhase> Phases;

	public List<ReactionTimePhase> BikePhases;

	public List<ReactionTimePhase> MinecartPhases;

	public float MinimumTimeBetweenReactonDecrease = 5f;

	public float MaximumTimeBetweenReactonDecrease = 10f;

	public float MinimumReactonDecreaseTime = 5f;

	public float MaximumReactonDecreaseTime = 10f;
}
