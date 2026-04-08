using System;

[Serializable]
public class ThemeState
{
	public enum LoadState
	{
		LS_UNLOADED = 0,
		LS_LOADING = 1,
		LS_LOADED = 2,
		LS_UNLOADREQUESTED = 3,
		LS_UNLOADING = 4
	}

	public PieceSet m_parentPieceSet;

	public LoadState m_loadingState;
}
