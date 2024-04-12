namespace SmartSheetLoader.Models
{
    public class SheetResponse
    {

       
        public long id { get; set; }
        public string name { get; set; }
        public int version { get; set; }
        public int totalRowCount { get; set; }
        public string accessLevel { get; set; }
        public string[] effectiveAttachmentOptions { get; set; }
        public bool ganttEnabled { get; set; }
        public bool dependenciesEnabled { get; set; }
        public bool resourceManagementEnabled { get; set; }
        public string resourceManagementType { get; set; }
        public bool cellImageUploadEnabled { get; set; }
        public Usersettings userSettings { get; set; }
        public Userpermissions userPermissions { get; set; }
        public bool hasSummaryFields { get; set; }
        public string permalink { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime modifiedAt { get; set; }
        public bool isMultiPicklistEnabled { get; set; }
        public List<Column> columns { get; set; }
        public List<Row> rows { get; set; }
        

        public class Usersettings
        {
            public bool criticalPathEnabled { get; set; }
            public bool displaySummaryTasks { get; set; }
        }

        public class Userpermissions
        {
            public string summaryPermissions { get; set; }
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
            public string[] options { get; set; }
        }

        public class Row
        {
            public long id { get; set; }
            public int rowNumber { get; set; }
            public bool expanded { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime modifiedAt { get; set; }
            public List<Cell> cells { get; set; }
            public long siblingId { get; set; }
        }

        public class Cell
        {
            public long columnId { get; set; }
            public object value { get; set; }
            public string displayValue { get; set; }
        }

    }
}
