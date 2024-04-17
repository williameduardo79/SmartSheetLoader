using CsvHelper;
using CsvHelper.Configuration;
using SmartSheetLoader.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;
using SmartSheetLoader.Enums;
using System.Numerics;

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
        public List<HeaderWithType> GetFileHeaders(byte[] fileBytes)
        {
            var encoding = Encoding.UTF8;
            List<HeaderWithType> csvHeaderWithTypes = new List<HeaderWithType>();

            using (var memoryStream = new MemoryStream(fileBytes))
            using (var reader = new StreamReader(memoryStream, encoding))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.Read();
                csv.ReadHeader();
                var headers = csv.HeaderRecord.Where(item => item.ToLower()!="id");
                IEnumerable<dynamic> records = csv.GetRecords<dynamic>();
                if (records != null)
                {
                   //Check each row of the CSV to property match the column type (numeric and text)
                    foreach (dynamic record in records)
                    {
                      foreach (var field in headers)
                        {
                            //if exists then we have already assign the column and type from the previous row
                            var csvHeaderWithType = csvHeaderWithTypes.Where(item => item.HeaderTitle == field).FirstOrDefault();
                            if (csvHeaderWithType != null)
                            {
                                //No need to check the type since it already has been set to text for this column based on previous values
                                if(csvHeaderWithType.DataTypeEnum == HeaderDataTypeEnum.text)
                                {
                                    continue;
                                }
                                else
                                {
                                    //This column was set as non-text (numeric) and now it seems to be text, should be updated to text.
                                    //This happens with Zip code for example where it can be set as numeric based on one row and then text is found on a different row
                                    if(SetHeaderWithType(record, field) == HeaderDataTypeEnum.text)
                                    {
                                        csvHeaderWithType.DataTypeEnum = HeaderDataTypeEnum.text;
                                    }
                                }
                            }
                            else
                            {
                                //New column not set previusly
                                csvHeaderWithType = new HeaderWithType();

                                csvHeaderWithType.HeaderTitle = field;

                                csvHeaderWithType.DataTypeEnum = SetHeaderWithType(record, field);

                                csvHeaderWithTypes.Add(csvHeaderWithType);

                            }
    
                        }
                    } 
                }
            }

            // Convert headers to a list and return
            return csvHeaderWithTypes;
        }
        private HeaderDataTypeEnum SetHeaderWithType(dynamic record, string field)
        {

            try
            {
                // Use CsvHelper's TypeConverter to parse the value with correct data type
                object value = ((IDictionary<string, object>)record)[field];
                var type = Helper.Utility.DetermineDataType(value);
             
                
                return type == null? HeaderDataTypeEnum.text : type.Value;
            }
            catch (Exception ex)
            {
                // Handle parsing errors gracefully (e.g., log or skip problematic fields)
                return HeaderDataTypeEnum.text; // or handle specific error cases
            }
        }
       
    }
       
}
