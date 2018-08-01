using System;


namespace BeatThat.StateStores
{
    [Serializable]
	public struct ResolveResponseDTO<DataType>
	{
        public string status;
        public string message;
        public DataType data;
	}
}

