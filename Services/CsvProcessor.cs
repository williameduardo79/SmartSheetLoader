using CsvHelper;
using System.Globalization;

namespace SmartSheetLoader.Services
{
    public class CsvProcessor : ICsvProcessor
    {
        public Stream ConvertClassToCsvStream<T>(IEnumerable<T> data)
        {
            var memoryStream = new MemoryStream();

            // Create a StreamWriter without automatically closing the underlying MemoryStream
            var writer = new StreamWriter(memoryStream);

            // Create a CsvWriter using the StreamWriter and configure it
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // Write records to the CsvWriter
            csv.WriteRecords(data);

            // Flush the StreamWriter to ensure all data is written to the MemoryStream
            writer.Flush();

            // Reset the memory stream position to the beginning
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
