using System;


namespace BeatThat.StateStores
{
    [Serializable]
	public struct ResolveResultDTO<DataType>
	{
        public string status;
        public string message;
        public DataType data;

        public bool GetData(out DataType data)
        {
            if (this.status != ResolveStatusCode.OK)
            {
                data = default(DataType);
                return false;
            }
            data = this.data;
            return true;
        }

        public static ResolveResultDTO<DataType> ResolveSucceeded(
            DataType data,
            int maxAgeSecs = 0,
            DateTimeOffset? timestamp = null
        )
        {
            return new ResolveResultDTO<DataType>
            {
                status = ResolveStatusCode.OK,
                message = "ok",
                data = data
            };
        }


        public static ResolveResultDTO<DataType> ResolveError(
            string error
        )
        {
            return new ResolveResultDTO<DataType>
            {
                status = ResolveStatusCode.ERROR,
                message = error
            };
        }

        public static ResolveResultDTO<DataType> ResolveError(
            ResolveRequestDTO req,
            Exception error,
            string message = null
        )
        {
            return new ResolveResultDTO<DataType>
            {
                status = ResolveStatusCode.ERROR,
                message = !string.IsNullOrEmpty(message) ? 
                                 message : error.Message
            };
        }

        public static ResolveResultDTO<DataType> ResolveNotFound(
            ResolveRequestDTO req
        )
        {
            return new ResolveResultDTO<DataType>
            {
                status = ResolveStatusCode.NOT_FOUND,
                message = "not found"
            };
        }
	}
}

