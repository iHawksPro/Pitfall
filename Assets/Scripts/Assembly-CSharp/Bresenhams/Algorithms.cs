using System;

namespace Bresenhams
{
	public static class Algorithms
	{
		public delegate bool PlotFunction(int x, int y);

		private static void Swap<T>(ref T lhs, ref T rhs)
		{
			T val = lhs;
			lhs = rhs;
			rhs = val;
		}

		public static void Line(int x0, int y0, int x1, int y1, PlotFunction plot)
		{
			bool flag = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
			if (flag)
			{
				Swap(ref x0, ref y0);
				Swap(ref x1, ref y1);
			}
			if (x0 > x1)
			{
				Swap(ref x0, ref x1);
				Swap(ref y0, ref y1);
			}
			int num = x1 - x0;
			int num2 = Math.Abs(y1 - y0);
			int num3 = num / 2;
			int num4 = ((y0 < y1) ? 1 : (-1));
			int num5 = y0;
			for (int i = x0; i <= x1 && ((!flag) ? plot(i, num5) : plot(num5, i)); i++)
			{
				num3 -= num2;
				if (num3 < 0)
				{
					num5 += num4;
					num3 += num;
				}
			}
		}
	}
}
