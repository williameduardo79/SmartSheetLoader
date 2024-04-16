namespace SmartSheetLoader.Models
{
    public class CreateSheetResponse
    {

       
            public string message { get; set; }
            public int resultCode { get; set; }
            public Result result { get; set; }
        

        public class Result
        {
            public long id { get; set; }
            public string name { get; set; }
            public string accessLevel { get; set; }
            public string permalink { get; set; }
            public List<Column> columns { get; set; }
        }

        public class Column
        {
            public long id { get; set; }
            public int version { get; set; }
            public int index { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public bool primary { get; set; }
            public bool validation { get; set; }
            public int width { get; set; }
            public List<string> options { get; set; }
        }

    }
}
