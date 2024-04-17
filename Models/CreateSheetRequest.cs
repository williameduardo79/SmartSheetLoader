namespace SmartSheetLoader.Models
{
    public class CreateSheetRequest
    {
        public CreateSheetRequest()
        {
            columns = new List<Column>();
           
        }
        public List<Column> columns { get; set; }
            public string name { get; set; }
        

        public class Column
        {
            public Column()
            {
                options = new List<string>();
            }
            public Autonumberformat autoNumberFormat { get; set; }
            public List<Contactoption> contactOptions { get; set; }
            public List<string> options { get; set; }
            public string primary { get; set; }
            public string symbol { get; set; }
            public string systemColumnType { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public int width { get; set; }
        }

        public class Autonumberformat
        {
            public string fill { get; set; }
            public string prefix { get; set; }
            public string startingNumber { get; set; }
            public string suffix { get; set; }
        }

        public class Contactoption
        {
            public string email { get; set; }
            public string name { get; set; }
        }

    }
}
