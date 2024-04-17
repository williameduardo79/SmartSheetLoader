using Microsoft.AspNetCore.Components;
using SmartSheetLoader.Services;
using System.Net.NetworkInformation;
using SmartSheetLoader.Models;
using SmartSheetLoader.Enums;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Security;
using System.IO;
using Syncfusion.Blazor.Inputs;
using Microsoft.AspNetCore.Components.Forms;
using System;

namespace SmartSheetLoader.Pages
{
    public partial class SheetGrouped:ComponentBase
    {

        [Inject] 
        ISmartsheetClient SSC { get; set; }
        [Inject]
        ICsvProcessor CSV { get; set; }
        [Inject]
        NavigationManager NavManager { get; set; }
        [Parameter]
        public string sheetId { get; set; }
        public SheetResponse? sheet;
        public List<DataFromAssignmentGrouped>? assignmentData;
        public bool groupAvailable { get; set; }
        public List<string> groupByOptions = new List<string>();
        public List<string> sumOptions = new List<string>();
        public string selectedOption;
        public string sumOption;

       
        protected override async Task OnInitializedAsync()
        {
            await LoadVariables();
         
        }
        private async Task LoadVariables()
        {
            groupAvailable = false;
            assignmentData = null;
            sheet = await SSC.GetSheetAsync(sheetId);
            var totalRows = sheet.rows.Count;
            var totalWithOutParentId = sheet.rows.Where(x => x.parentId == null).Count();
            if (totalRows == totalWithOutParentId)
            {
                groupAvailable = true;
                var headers = SSC.GetFileHeaders(sheet);
                groupByOptions = headers.Where(x => x.DataTypeEnum == HeaderDataTypeEnum.text).Select(y => y.HeaderTitle).ToList();
                sumOptions = headers.Where(x => x.DataTypeEnum == HeaderDataTypeEnum.number).Select(y => y.HeaderTitle).ToList();
            }

            if (sheet.columns.Any(x => x.title == "arr"))
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
                try
                {
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


            }
        }
        public async Task ProcessSubmit()
        {
            sheet = null;
            StateHasChanged();
          
                try
                {
                    long.TryParse(sheetId, out var id);

                    if (selectedOption != "**None")
                    {
                        SSC.AddGroupingToSheet(id, selectedOption);

                    }
                    if (sumOption != "**None")
                    {
                        SSC.AddSumToSheet(id, sumOption);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
           await LoadVariables();
            StateHasChanged();


            // You can also access the 'uploadFormModel.Name' here
        }
        protected async Task UploadGroupingAsync()
        {
            var csvFile = CSV.ConvertClassToCsvStream<DataFromAssignmentGrouped>(assignmentData);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Copy the data from the input Stream to the MemoryStream
                csvFile.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                //CreateSheetRequest createSheetRequest = new();
                //CreateSheetRequest.Column id = new CreateSheetRequest.Column();
                //id.title = "Id";
                //id.type = "TEXT_NUMBER";
                //id.width = 150;
                //id.primary = "true";
                //CreateSheetRequest.Column country = new CreateSheetRequest.Column();
                //country.title = "Country";
                //country.type = "PICKLIST";
                //country.width = 150;
                //country.options = new List<string>{ "United States", "Canada" };
                //CreateSheetRequest.Column state = new CreateSheetRequest.Column();
                //state.title = "State";
                //state.type = "TEXT_NUMBER";
                //state.width = 150;
                //CreateSheetRequest.Column arr = new CreateSheetRequest.Column();
                //arr.title = "Arr";
                //arr.type = "TEXT_NUMBER";
                //arr.width = 150;
                //createSheetRequest.name = "Test New Sheet";
                //createSheetRequest.columns.Add(id);
                //createSheetRequest.columns.Add(country);
                //createSheetRequest.columns.Add(state);
                //createSheetRequest.columns.Add(arr);

                //await SSC.CreateSheetAsync(createSheetRequest);
                
                await SSC.PostCsvAsync("CountryAndState",memoryStream.ToArray());
            }

            csvFile.Dispose();
            NavManager.NavigateTo("/");
        }
       
    }
}
