using System;


namespace BeatThat.StateStores
{
    [Serializable]
	public struct StoreStateDTO<DataType>
	{
        public DataType data;
	}
}

