using Smartsheet.Api.Models;
using SmartSheetLoader.Models;
using SmartSheetLoader.Pages;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using SmartsheetApi = Smartsheet.Api;

namespace SmartSheetLoader.Services
{
    public class SmartsheetClient : ISmartsheetClient
    {
        private IHttpClientFactory _clientFactory;
        private readonly SmartsheetApi.SmartsheetClient _smartsheetClient;
        public SmartsheetClient(IHttpClientFactory clientFactory, SmartsheetApi.SmartsheetClient smartsheetClient)
        {
            _clientFactory = clientFactory;
            _smartsheetClient = smartsheetClient;
        }
        public async Task<SheetsResponse> GetSheetsAsync()
        {
            var httpClient = _clientFactory.CreateClient("smartsheet");
            var queryParams = new Dictionary<string, string>
            {
                { "include", "sheetVersion" },
                { "includeAll", "false" },
                { "numericDates", "false" },
                { "page", "1" },
                { "pageSize", "100" },
                { "accessApiLevel", "0" }
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            var requestUrl = $"sheets?{queryString}";
            HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            SheetsResponse model = JsonSerializer.Deserialize<SheetsResponse>(responseString);
            return model;
        }
        public async Task<SheetResponse> GetSheetAsync(string sheetId)
        {
            var httpClient = _clientFactory.CreateClient("smartsheet");
            var queryParams = new Dictionary<string, string>
            {
                { "level", "1" },
                { "paperSize", "LETTER" },
                { "page", "1" },
                { "pageSize", "100" },
                { "accessApiLevel", "0" }
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            var requestUrl = $"sheets/{sheetId}?{queryString}";
            HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            SheetResponse model = JsonSerializer.Deserialize<SheetResponse>(responseString);
            return model;
        }
        public async Task<UploadFileResponse> PostCsvAsync(string sheetName, byte[] fileBytes)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "sheetName", sheetName },
                { "headerRowIndex", "3" }

            };
            var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            var requestUrl = $"sheets/import?{queryString}";

            var httpClient = _clientFactory.CreateClient("smartsheet");
           

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
           
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "data.csv"
            };


            MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("text/csv");
           
            var content = new MultipartFormDataContent();
            content.Headers.ContentType = contentType;
            content.Headers.ContentDisposition = contentDisposition;

            var fileContent = new ByteArrayContent(fileBytes);
           

            content.Add(fileContent, "file", "data.csv");
           
          
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = content
               
            };


            // Send the request
            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            UploadFileResponse model = JsonSerializer.Deserialize<UploadFileResponse>(responseString);
            if (model.message != "SUCCESS")
                throw new Exception(model.message);
            return model;
        }

        public async Task<CreateSheetResponse> CreateSheetAsync(CreateSheetRequest sheet)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "include", "ruleRecipients" },
                { "accessApiLevel", "0" }

            };
            var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            var requestUrl = $"sheets?{queryString}";

            var httpClient = _clientFactory.CreateClient("smartsheet");


            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

   
          var payload = JsonSerializer.Serialize(sheet);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");


            // Send the request
            HttpResponseMessage response = await httpClient.PostAsync(requestUrl, content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            CreateSheetResponse model = JsonSerializer.Deserialize<CreateSheetResponse>(responseString);
            if (model.message != "SUCCESS")
                throw new Exception(model.message);
            return model;
        }
        public void RemoveEmptyRows(long sheetId)
        {
            // Load the sheet data
            var sheet = _smartsheetClient.SheetResources.GetSheet(sheetId, null, null, null, null, null, null, null);

            if (sheet.Rows != null && sheet.Rows.Count > 0)
            {
                // List to store row IDs of empty rows
                List<long> emptyRowIds = new List<long>();

                // Iterate through each row to check if it's empty
                foreach (Row row in sheet.Rows)
                {
                    // Check if all cells in the row are empty
                    bool isEmptyRow = row.Cells.All(cell => string.IsNullOrWhiteSpace(cell.Value.ToString()));

                    if (isEmptyRow)
                    {
                        // Add the row ID to the list of empty rows
                        emptyRowIds.Add(row.Id.Value);
                    }
                }

                if (emptyRowIds.Any())
                {
                    // Delete empty rows from the sheet
                    _smartsheetClient.SheetResources.RowResources.DeleteRows(sheetId, emptyRowIds, false);
                    Console.WriteLine($"Deleted {emptyRowIds.Count} empty rows.");
                }
                else
                {
                    Console.WriteLine("No empty rows found to delete.");
                }
            }
            else
            {
                Console.WriteLine("The sheet is empty.");
            }
        
        
        }

        public void AddGroupingToSheet(long sheetId,string groupBy, string? sumBy)
        {
            //Get the sheet
            var sheet = _smartsheetClient.SheetResources.GetSheet(sheetId, null, null, null, null, null, null, null);
            long? groupById = sheet.Columns.FirstOrDefault(column => column.Title == groupBy)?.Id;
            long? primaryKeyId = sheet.Columns.FirstOrDefault(column => column.Primary == true)?.Id;
            //Create a dictionary for fast access
            IDictionary<string, long> groupDictionary = new Dictionary<string, long>();
            //Get the group by and Primary key column id's
            if (groupById != null && primaryKeyId != null)
            {
                //Flat cell data
                IEnumerable<Cell> allCells = sheet.Rows.SelectMany(row => row.Cells);
                var allGroupValues =allCells.Where(cell=>cell.ColumnId== groupById).Select(item => item.Value).Distinct().ToList();
                //Get the group values to add new rows for indentation
                List<Row> rows = new List<Row>();
                int rowNumber = 1;
                long? sibblingId = 0;
                foreach (var groupValue in allGroupValues)
                {
                    if(groupValue != null)
                    {
                      
                        //If it is the first row inserted get the ID to use as sibbling
                        if (rowNumber == 1)
                        {
                            Row dynamicRow = new Row.AddRowBuilder(true, null, null, null, null)
                           .SetCells(new Cell[] {
                                    new Cell.AddCellBuilder(primaryKeyId, groupValue.ToString()).Build(),
                                    new Cell.AddCellBuilder(groupById, groupValue.ToString()).Build(),
                               // Add more cells as needed for additional columns
                           })
                           .Build();
                            var thisRow = _smartsheetClient.SheetResources.RowResources.AddRows(sheetId, new List<Row> { dynamicRow }).FirstOrDefault();
                            var valueAdded = thisRow.Cells.FirstOrDefault(cell => cell.ColumnId == groupById)?.Value?.ToString();
                            if (valueAdded != null)
                            {
                                groupDictionary.Add(valueAdded, thisRow.Id.Value);
                            }
                            sibblingId = thisRow.Id;
                        }
                        else
                        {
                            Row dynamicRow = new Row.AddRowBuilder(false, null, null, sibblingId.Value, false)
                            .SetCells(new Cell[] {
                                    new Cell.AddCellBuilder(primaryKeyId, groupValue.ToString()).Build(),
                                    new Cell.AddCellBuilder(groupById, groupValue.ToString()).Build(),
                                // Add more cells as needed for additional columns
                            })
                            .Build();
                            rows.Add(dynamicRow);
                        }

                        rowNumber++;
                    }
                    
                }
                //Add all rows
                IList<Row> addedRows = _smartsheetClient.SheetResources.RowResources.AddRows(sheetId, rows);
                
                foreach(var row in addedRows)
                {
                    var valueAdded = row.Cells.FirstOrDefault(cell => cell.ColumnId == groupById)?.Value?.ToString();
                    if (valueAdded != null)
                    {
                        groupDictionary.Add(valueAdded, row.Id.Value);
                    }
                    
                }

                //** START Indent rows
                List<Row> rowsToUpdate = new List<Row>();
                foreach (var row in sheet.Rows)
                {
                    var rowfromSheet = _smartsheetClient.SheetResources.RowResources.GetRow(sheetId, row.Id.Value,null,null);
                    string groupCellValue = rowfromSheet.Cells.FirstOrDefault(cell=>cell.ColumnId== groupById)?.Value?.ToString();
                    if (groupCellValue != null)
                    {
                       
                        var parentRowId = groupDictionary[groupCellValue];
                       

                        Row updatedRow = new Row.UpdateRowBuilder(row.Id.Value)
                            .SetParentId(parentRowId)
                            //.SetSiblingId(rowfromSheet.SiblingId)
                            .SetAbove(false)
                            .SetToTop(false)
                            .Build();
                        var singleUpdate = _smartsheetClient.SheetResources.RowResources.UpdateRows(sheetId, new List<Row> { updatedRow });
                       // rowsToUpdate.Add(updatedRow);
                    }
                   
                }
               //IList<Row> updatedRows = _smartsheetClient.SheetResources.RowResources.UpdateRows(sheetId, rowsToUpdate);


            }
           

        }
    }

}
