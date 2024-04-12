namespace SmartSheetLoader.Models
{
    public class SheetsResponse
    {

      
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public int totalCount { get; set; }
        public List<Datum> data { get; set; }
        

        public class Datum
        {
            public long id { get; set; }
            public string name { get; set; }
            public int version { get; set; }
            public string accessLevel { get; set; }
            public string permalink { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime modifiedAt { get; set; }
        }

    }
}
