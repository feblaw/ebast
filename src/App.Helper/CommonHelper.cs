using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;

namespace App.Helper
{
    public class CommonHelper
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ConfigHelper _configHelper;

        public CommonHelper(IHostingEnvironment env,
            IConfiguration config,
            ConfigHelper configHelper)
        {
            _env = env;
            _config = config;
            _configHelper = configHelper;
        }

        public string GetSetting(string key)
        {
            return _config[key];
        }

        public string GetShortDateFormat()
        {
            return _configHelper.GetConfig("format.datetime.short");
            //return GetSetting("DateTime:Short");
        }

        public string GetShortDatePickerFormat()
        {
            return _configHelper.GetConfig("format.datetime.short.picker");
            //return GetSetting("DateTime:ShortDatePicker");
        }

        public string GetShortMomentFormat()
        {
            return _configHelper.GetConfig("format.datetime.short.moment");
            //return GetSetting("DateTime:ShortMoment");
        }

        public string GetShortMomentSystemFormat()
        {
            return _configHelper.GetConfig("format.datetime.short.moment.system");
            //return GetSetting("DateTime:ShortMomentSystem");
        }

        public string GetLongDateFormat()
        {
            return _configHelper.GetConfig("format.datetime.long");
            //return GetSetting("DateTime:Long");
        }

        public string GetLongDatePickerFormat()
        {
            return _configHelper.GetConfig("format.datetime.long.picker");
            //return GetSetting("DateTime:LongDatePicker");
        }

        public DateTime? ToNullableDateTime(string data, bool longFormat = false)
        {
            DateTime date;
            var format = longFormat ? GetLongDateFormat() : GetShortDateFormat();

            if (DateTime.TryParseExact(data,
                format,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date))
            {
                return date;
            }

            return null;
        }

        public DateTime ToDateTime(string data, bool longFormat = false)
        {
            return ToNullableDateTime(data, longFormat) ?? new DateTime();
        }

        public string ToShortString(DateTime date)
        {
            return date.ToString(GetShortDateFormat());
        }

        public string ToShortString(DateTime? date)
        {
            if (date.HasValue)
            {
                return ToShortString(date.Value);
            }

            return "";
        }

        public string ToLongString(DateTime date)
        {
            return date.ToString(GetLongDateFormat());
        }

        public string ToLongString(DateTime? date)
        {
            if (date.HasValue)
            {
                return ToLongString(date.Value);
            }

            return "";
        }
    }
}
