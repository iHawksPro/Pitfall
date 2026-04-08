public static class SceneNameResolver
{
	public static string Resolve(string sceneOrStateName)
	{
		switch (sceneOrStateName)
		{
		case "Game":
			return "game";
		case "Credits":
			return "credits";
		case "title":
			return "Title";
		default:
			return sceneOrStateName;
		}
	}
}
