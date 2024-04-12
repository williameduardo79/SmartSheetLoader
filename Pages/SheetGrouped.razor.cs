using Microsoft.AspNetCore.Components;
using SmartSheetLoader.Services;
using System.Net.NetworkInformation;
using SmartSheetLoader.Services;
using SmartSheetLoader.Models;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Newtonsoft.Json;

namespace SmartSheetLoader.Pages
{
    public partial class SheetGrouped:ComponentBase
    {

        [Inject] 
        ISmartsheetClient SSC { get; set; }
        [Parameter]
        public string sheetId { get; set; }
        public SheetResponse? sheet;
        public List<DataFromAssignmentGrouped>? assignmentData;
       
        protected override async Task OnInitializedAsync()
        {
            sheet = await SSC.GetSheetAsync(sheetId);
            //testData = new List<SimpleModel>()
            //{
            //    new SimpleModel() {Id=1,Name="william"},
            //    new SimpleModel() {Id=2,Name = "Jhon"},
            //    new SimpleModel() {Id=3,Name = "Liset"}
            //};

            if(sheet.columns.Any(x=>x.title== "arr"))
            {
                Dictionary<long, String> Columns = sheet.columns.ToDictionary(c => c.id, c => c.title);



                // Create the JObject for flat data
                JArray jsonArray = new JArray();

                // Iterate over rows
                foreach (var row in sheet.rows)
                {
                    // Create a JObject for the current row
                    JObject rowObject = new JObject();

                    // Iterate over cells in the current row
                    foreach (var cell in row.cells)
                    {
                        // Check if the columnId exists in the Columns dictionary
                        if (Columns.ContainsKey(cell.columnId))
                        {
                            // Get the column title based on columnId
                            string columnName = Columns[cell.columnId];

                            // Add the column value to the rowObject
                            rowObject.Add(columnName, cell.displayValue);
                        }
                    }

                    // Add the rowObject to the flatData JObject
                    jsonArray.Add(rowObject);
                }
                string jsonString = jsonArray.ToString();
                var dataFromAssignment = JsonConvert.DeserializeObject<List<DataFromAssignment>>(jsonString);

                assignmentData = dataFromAssignment
                                       .GroupBy(a => new { a.country, a.state })
                                        .Select(g => new DataFromAssignmentGrouped
                                        {
                                            country = g.Key.country,
                                            state = g.Key.state,
                                            arr = g.Sum(a => a.arr)
                                        }).ToList();




            }


            
        }
        protected async Task GroupBy(string group)
        {

        }
    }
}
