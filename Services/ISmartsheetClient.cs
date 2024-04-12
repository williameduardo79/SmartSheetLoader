using SmartSheetLoader.Models;

namespace SmartSheetLoader.Services
{
    public interface ISmartsheetClient
    {
        Task<SheetsResponse> GetSheetsAsync();
        Task<SheetResponse> GetSheetAsync(string sheetId);
        Task<UploadFileResponse> PostCsvAsync(string sheetName, byte[] fileBytes);
    }
}
