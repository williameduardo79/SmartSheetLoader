using SmartSheetLoader.Models;

namespace SmartSheetLoader.Services
{
    public interface ISmartsheetClient
    {
        Task<SheetsResponse> GetSheetsAsync();
        Task<SheetResponse> GetSheetAsync(string sheetId);
        Task<UploadFileResponse> PostCsvAsync(string sheetName, byte[] fileBytes);
        Task<CreateSheetResponse> CreateSheetAsync(CreateSheetRequest sheet);
        void RemoveEmptyRows(long sheetId);
        void AddGroupingToSheet(long sheetId, string groupBy);
        void AddSumToSheet(long sheetId, string sumBy);
        List<HeaderWithType> GetFileHeaders(SheetResponse sheetResponse);
    }
}
