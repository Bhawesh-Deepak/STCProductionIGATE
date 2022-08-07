using System.Collections.Generic;

namespace STCAPI.Core.ViewModel.ResponseModel
{
    public class AdminPortalResponseModel
    {
        public string userName { get; set; }
        public string directory { get; set; }
        public List<Stage> stages { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class Datum
    {
        public int PortalId { get; set; }
        public bool accessLevel { get; set; }
        public string name { get; set; }
    }

    public class ObjectData
    {
        public string name { get; set; }
        public bool accessLevel { get; set; }
        public List<Datum> data { get; set; }
    }

    public class SubStream
    {
        public string subStreamName { get; set; }
        public bool accessLevel { get; set; }
        public List<ObjectData> Object { get; set; }
    }

    public class MainStream
    {
        public string streamName { get; set; }
        public bool accessLevel { get; set; }
        public List<SubStream> subStream { get; set; }
    }

    public class Stage
    {
        public string stageName { get; set; }
        public bool accessLevel { get; set; }
        public List<MainStream> mainStream { get; set; }
    }
}
