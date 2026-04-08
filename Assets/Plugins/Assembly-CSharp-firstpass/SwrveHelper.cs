using System;
using System.Collections.Generic;

public static class SwrveHelper
{
	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		Random random = new Random();
		while (num > 1)
		{
			int index = random.Next(0, num) % num;
			num--;
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
