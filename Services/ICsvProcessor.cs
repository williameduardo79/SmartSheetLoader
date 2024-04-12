namespace SmartSheetLoader.Services
{
    public interface ICsvProcessor
    {
        Stream ConvertClassToCsvStream<T>(IEnumerable<T> data);
    }
}
