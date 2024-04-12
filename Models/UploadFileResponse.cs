namespace SmartSheetLoader.Models
{
    public class UploadFileResponse
    {

      
        public string message { get; set; }
        public int resultCode { get; set; }
        public Result result { get; set; }
        

        public class Result
        {
            public long id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string accessLevel { get; set; }
            public string permalink { get; set; }
        }

    }
}
