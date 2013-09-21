using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCenter.Web.HtmlHelpers
{
    public static class HtmlHelperExtensions
    {
        //public static string ToJsonString<T>(this HtmlHelper<T> helper, object value)
        //{
        //    return JsonConvert.SerializeObject(value);
        //}

        public static HtmlString ToJsonString(this HtmlHelper helper, object value)
        {
            return new HtmlString(JsonConvert.SerializeObject(value));
        }
    }
}