namespace SmartSheetLoader.Models
{
    public class DataFromAssignment
    {



        public long id { get; set; }
        public long arr { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string street_address { get; set; }
        public string zipcode { get; set; }


    }
    public class DataFromAssignmentGrouped
    {
       
       
        public long arr { get; set; }
       
        public string country { get; set; }
        public string state { get; set; }
    


    }
}
