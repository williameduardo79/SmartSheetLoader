using SmartSheetLoader.Enums;

namespace SmartSheetLoader.Helper
{
    public static class Utility
    {
        public static HeaderDataTypeEnum? DetermineDataType(object value)
        {
            if(value!=null)
            {
                if (long.TryParse(value.ToString(), out var type))
                {
                    return HeaderDataTypeEnum.number;
                }
                else if (Decimal.TryParse(value.ToString(), null, out var dec))
                {
                    return HeaderDataTypeEnum.number;
                }

                //else if (value is DateTime)
                //{
                //    return CsvDataTypeEnum.DateTime;
                //}
                return HeaderDataTypeEnum.text; // Default to text if data type is unknown
            }
            return null;

        }
    }
}
