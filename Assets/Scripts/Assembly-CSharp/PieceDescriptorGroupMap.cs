using System;
using UnityEngine;

public class PieceDescriptorGroupMap
{
	private static PieceDescriptorGroupMapElement[] Map = new PieceDescriptorGroupMapElement[26]
	{
		new PieceDescriptorGroupMapElement("A", WorldConstructionHelper.Group.A),
		new PieceDescriptorGroupMapElement("B", WorldConstructionHelper.Group.B),
		new PieceDescriptorGroupMapElement("C", WorldConstructionHelper.Group.C),
		new PieceDescriptorGroupMapElement("D", WorldConstructionHelper.Group.D),
		new PieceDescriptorGroupMapElement("E", WorldConstructionHelper.Group.E),
		new PieceDescriptorGroupMapElement("F", WorldConstructionHelper.Group.F),
		new PieceDescriptorGroupMapElement("G", WorldConstructionHelper.Group.G),
		new PieceDescriptorGroupMapElement("H", WorldConstructionHelper.Group.H),
		new PieceDescriptorGroupMapElement("I", WorldConstructionHelper.Group.I),
		new PieceDescriptorGroupMapElement("J", WorldConstructionHelper.Group.J),
		new PieceDescriptorGroupMapElement("K", WorldConstructionHelper.Group.K),
		new PieceDescriptorGroupMapElement("L", WorldConstructionHelper.Group.L),
		new PieceDescriptorGroupMapElement("M", WorldConstructionHelper.Group.M),
		new PieceDescriptorGroupMapElement("N", WorldConstructionHelper.Group.N),
		new PieceDescriptorGroupMapElement("O", WorldConstructionHelper.Group.O),
		new PieceDescriptorGroupMapElement("P", WorldConstructionHelper.Group.P),
		new PieceDescriptorGroupMapElement("Q", WorldConstructionHelper.Group.Q),
		new PieceDescriptorGroupMapElement("R", WorldConstructionHelper.Group.R),
		new PieceDescriptorGroupMapElement("S", WorldConstructionHelper.Group.S),
		new PieceDescriptorGroupMapElement("T", WorldConstructionHelper.Group.T),
		new PieceDescriptorGroupMapElement("U", WorldConstructionHelper.Group.U),
		new PieceDescriptorGroupMapElement("V", WorldConstructionHelper.Group.V),
		new PieceDescriptorGroupMapElement("W", WorldConstructionHelper.Group.W),
		new PieceDescriptorGroupMapElement("X", WorldConstructionHelper.Group.X),
		new PieceDescriptorGroupMapElement("Y", WorldConstructionHelper.Group.Y),
		new PieceDescriptorGroupMapElement("Z", WorldConstructionHelper.Group.Z)
	};

	public static WorldConstructionHelper.Group ParseGroupData(string pieceName, out WorldConstructionHelper.Group transitionEntry, out WorldConstructionHelper.Group transitionExit)
	{
		string[] array = pieceName.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		if (array.Length < 2 || array[array.Length - 2].Length > 2)
		{
			Debug.Log("WARNING(0) - ParseGroupData: pieceName=" + pieceName + " does not conform to Group naming convention. Defaulting to Group 'A'");
			transitionEntry = (transitionExit = WorldConstructionHelper.Group.Invalid);
			return WorldConstructionHelper.Group.A;
		}
		string text = string.Format("_{0}_", array[array.Length - 2]);
		WorldConstructionHelper.Group obj = WorldConstructionHelper.Group.Invalid;
		WorldConstructionHelper.Group obj2 = WorldConstructionHelper.Group.Invalid;
		for (int i = 0; i < Map.Length; i++)
		{
			string value = "_" + Map[i].GroupLabel;
			if (text.Contains(value))
			{
				obj = Map[i].GroupId;
			}
			string value2 = Map[i].GroupLabel + "_";
			if (text.Contains(value2))
			{
				obj2 = Map[i].GroupId;
			}
		}
		if (obj == WorldConstructionHelper.Group.Invalid && obj2 == WorldConstructionHelper.Group.Invalid)
		{
			Debug.Log("WARNING(1) - ParseGroupData: pieceName=" + pieceName + " does not conform to Group naming convention. Defaulting to Group 'A'");
			transitionEntry = (transitionExit = WorldConstructionHelper.Group.Invalid);
			return WorldConstructionHelper.Group.A;
		}
		if (obj == obj2)
		{
			transitionEntry = (transitionExit = WorldConstructionHelper.Group.Invalid);
			return obj;
		}
		transitionEntry = obj;
		transitionExit = obj2;
		return WorldConstructionHelper.Group.Transition;
	}
}
