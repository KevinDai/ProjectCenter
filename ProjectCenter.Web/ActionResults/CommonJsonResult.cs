using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace ProjectCenter.Web.ActionResults
{
    public class CommonJsonResult : JsonResult
    {

        public CommonJsonResult()
        {
            //Newtonsoft.Json.JsonConverter.
            //Newtonsoft.Json.Converters.DateTimeConverterBase
        }

        private static readonly JsonConverter[] JavaScriptConverters = new JsonConverter[]{
          new  DateTimeConverter()
        };

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.Data != null)
            {
                //JsonConvert.SerializeObject(
                //JavaScriptSerializer serializer = GetJavaScriptSerializer();
                //response.Write(serializer.Serialize(Data));
                ////serializer.RegisterConverters(
                //string jsonString = jss.Serialize(Data);
                //string p = @"\\/Date\((\d+)\)\\/";
                //MatchEvaluator matchEvaluator = new MatchEvaluator(this.ConvertJsonDateToDateString);
                //Regex reg = new Regex(p);
                //jsonString = reg.Replace(jsonString, matchEvaluator);
                var result = JsonConvert.SerializeObject(Data, JavaScriptConverters);
                response.Write(result);
            }

        }

        //private JavaScriptSerializer GetJavaScriptSerializer()
        //{
        //    var serializer = new JavaScriptSerializer();
        //    serializer.RegisterConverters(JavaScriptConverters);
        //    return serializer;
        //}

        protected class DateTimeConverter : Newtonsoft.Json.Converters.DateTimeConverterBase
        {

            //public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            //{
            //    throw new NotImplementedException();
            //}

            //public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            //{
            //    if (obj is DateTime)
            //    {
            //        //return serializer. (obj as DateTime).ToString();
            //        var result = new Dictionary<string, object>();
            //        result.Add(null, ((DateTime)obj).ToString());
            //        return result;
            //    }
            //    else
            //    {
            //        throw new NotImplementedException();
            //    }
            //}

            //private IEnumerable<Type> _supportedTypes = new Type[] { typeof(DateTime) };

            //public override IEnumerable<Type> SupportedTypes
            //{
            //    get
            //    {
            //        return _supportedTypes;
            //    }
            //}
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }
}