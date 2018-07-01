using System;


namespace BeatThat.StateStores
{
    [Serializable]
	public struct LoadResponseDTO<DataType>
	{
        public string status;
        public string message;
        public DataType data;
	}
}

