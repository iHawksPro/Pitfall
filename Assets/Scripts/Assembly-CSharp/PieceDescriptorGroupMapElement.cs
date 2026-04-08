public class PieceDescriptorGroupMapElement
{
	public string GroupLabel;

	public WorldConstructionHelper.Group GroupId;

	public PieceDescriptorGroupMapElement(string label, WorldConstructionHelper.Group id)
	{
		GroupLabel = label;
		GroupId = id;
	}
}
