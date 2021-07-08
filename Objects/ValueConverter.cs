using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Bitstamp.DataPuller.Objects
{
    public static class ValueConverter
    {
        static NumberFormatInfo _numberFormat;
        static ValueConverter()
        {
            _numberFormat = new NumberFormatInfo();
            _numberFormat.NumberDecimalSeparator = ",";
        }
        public static decimal ToDecimal(JToken token)
        {
            try
            {
                return Convert.ToDecimal(double.Parse(token.Value<string>().Replace('.', ','), _numberFormat));
            }
            catch (Exception e) { Log.Instance.Error($"Failed to convert '{token}' to decimal {e.StackTrace}"); }
            return 0;
        }
    }
}
