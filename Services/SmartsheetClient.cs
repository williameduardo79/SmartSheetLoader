using SmartSheetLoader.Models;
using SmartSheetLoader.Pages;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace SmartSheetLoader.Services
{
    public class SmartsheetClient : ISmartsheetClient
    {
        private IHttpClientFactory _clientFactory;
        public SmartsheetClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
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

    }

}
