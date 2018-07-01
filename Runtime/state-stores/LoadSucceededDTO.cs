using System;


namespace BeatThat.StateStores
{
    [Serializable]
	public struct LoadSucceededDTO<DataType>
	{
        public DataType data;
	}
}

