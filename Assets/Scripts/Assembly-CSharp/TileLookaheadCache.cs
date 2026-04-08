using System.Collections.Generic;

public class TileLookaheadCache
{
	private List<TileLookaheadEvent> mEvents = new List<TileLookaheadEvent>();

	public void Reset()
	{
		mEvents.Clear();
	}

	public void RegisterEvent(Tile.ResponseType type, float distance)
	{
		for (int i = 0; i < mEvents.Count; i++)
		{
			if (mEvents[i].Type == type && mEvents[i].Distance < distance)
			{
				return;
			}
		}
		TileLookaheadEvent tileLookaheadEvent = new TileLookaheadEvent();
		tileLookaheadEvent.Type = type;
		tileLookaheadEvent.Distance = distance;
		mEvents.Add(tileLookaheadEvent);
	}

	public bool IsEventRegistered(Tile.ResponseType type, out float distance)
	{
		for (int i = 0; i < mEvents.Count; i++)
		{
			if (mEvents[i].Type == type)
			{
				distance = mEvents[i].Distance;
				return true;
			}
		}
		distance = -1f;
		return false;
	}
}
