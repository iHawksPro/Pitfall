namespace Swrve
{
	public abstract class SwrveWidget
	{
		public Point Position;

		public Point Size;

		public SwrveWidget()
		{
			Position = new Point(0, 0);
			Size = new Point(0, 0);
		}

		public Point getCenteredPosition(float Scale, float FormatScale)
		{
			int x = (int)((double)((float)(-Size.X) * Scale) / 2.0 + (double)((float)Position.X * FormatScale));
			int y = (int)((double)((float)(-Size.Y) * Scale) / 2.0 + (double)((float)Position.Y * FormatScale));
			return new Point(x, y);
		}
	}
}
