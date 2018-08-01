namespace BeatThat.StateStores
{
    public enum ResolveAdvice
	{
		PROCEED = 0,
		CANCEL_LOADED_AND_UNEXPIRED = 1,
		CANCEL_IN_PROGRESS = 2,
		CANCEL_ERROR_COOL_DOWN = 3
	}

}

