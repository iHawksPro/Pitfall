using UnityEngine;

public class LevelGenerationVisualiser : MonoBehaviour
{
	private static Rect VISUALISER_RECTANGLE = new Rect(20f, 600f, 250f, 250f);

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
		string text = "Level Generation Visualiser";
		GUI.Box(VISUALISER_RECTANGLE, text);
		Rect vISUALISER_RECTANGLE = VISUALISER_RECTANGLE;
		vISUALISER_RECTANGLE.x += BUTTON_INDENT_WIDTH;
		vISUALISER_RECTANGLE.y += (float)BUTTON_HEIGHT * 1.5f;
		vISUALISER_RECTANGLE.width -= BUTTON_INDENT_WIDTH * 2;
		vISUALISER_RECTANGLE.height = BUTTON_HEIGHT;
		if (GUI.Button(vISUALISER_RECTANGLE, "Decrease Blind Hazard tolerance [" + WorldConstructionController.BlindHazardOutAdditiveLengthMultiplier + "]"))
		{
			WorldConstructionController.BlindHazardOutAdditiveLengthMultiplier *= 0.5f;
		}
		Rect position = vISUALISER_RECTANGLE;
		position.y += (float)BUTTON_HEIGHT * 1.5f;
		if (GUI.Button(position, "Increase Blind Hazard tolerance [" + WorldConstructionController.BlindHazardOutAdditiveLengthMultiplier + "]"))
		{
			WorldConstructionController.BlindHazardOutAdditiveLengthMultiplier *= 2f;
		}
	}
}
