using UnityEngine;

public class LevelDesignVisualiser : MonoBehaviour
{
	private static Rect VISUALISER_RECTANGLE = new Rect(20f, 600f, 250f, 250f);

	private static Rect DEATH_LOG_RECTANGLE = new Rect(10f, 10f, 400f, 250f);

	private static int BUTTON_INDENT_WIDTH = 10;

	private static int BUTTON_HEIGHT = 25;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		string text = "Level Design Visualiser";
		GUI.Box(VISUALISER_RECTANGLE, text);
		Rect vISUALISER_RECTANGLE = VISUALISER_RECTANGLE;
		vISUALISER_RECTANGLE.x += BUTTON_INDENT_WIDTH;
		vISUALISER_RECTANGLE.y += (float)BUTTON_HEIGHT * 1.5f;
		vISUALISER_RECTANGLE.width -= BUTTON_INDENT_WIDTH * 2;
		vISUALISER_RECTANGLE.height = BUTTON_HEIGHT;
		if (GUI.Button(vISUALISER_RECTANGLE, "Decrease Scaling [" + WorldConstructionHelper.StraightScalingMultiplier + "]"))
		{
			WorldConstructionHelper.StraightScalingMultiplier *= 0.5f;
		}
		Rect position = vISUALISER_RECTANGLE;
		position.y += (float)BUTTON_HEIGHT * 1.5f;
		if (GUI.Button(position, "Increase Scaling [" + WorldConstructionHelper.StraightScalingMultiplier + "]"))
		{
			WorldConstructionHelper.StraightScalingMultiplier *= 2f;
		}
		PlayerController playerController = PlayerController.Instance();
		LevelGenerator levelGenerator = LevelGenerator.Instance();
		if (!(playerController != null) || !(levelGenerator != null) || !playerController.IsDead())
		{
			return;
		}
		string text2 = "--Death Report--";
		for (int i = 0; i < levelGenerator.DeathReport.Count; i++)
		{
			text2 += "\n";
			if (i == levelGenerator.DeathReport.Count - 1)
			{
				text2 += "KILL PIECE - [ ";
			}
			text2 += levelGenerator.DeathReport[i];
			if (i == levelGenerator.DeathReport.Count - 1)
			{
				text2 += " ] ";
			}
		}
		GUI.Box(DEATH_LOG_RECTANGLE, text2);
	}
}
