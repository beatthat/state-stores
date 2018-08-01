using System;


namespace BeatThat.StateStores
{
    [Serializable]
	public struct ResolveSucceededDTO<DataType>
	{
        public DataType data;
	}
}

