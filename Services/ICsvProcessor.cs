using SmartSheetLoader.Models;

namespace SmartSheetLoader.Services
{
    public interface ICsvProcessor
    {
        Stream ConvertClassToCsvStream<T>(IEnumerable<T> data);
        List<CsvHeaderWithType> GetFileHeaders(byte[] fileBytes);
    }
}
