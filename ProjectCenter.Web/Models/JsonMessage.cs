using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Models
{
    public class JsonMessage
    {

        public JsonMessage()
        {
            Status = (int)MessageStatus.Ok;
        }

        public JsonMessage(object data)
        {
            Status = (int)MessageStatus.Ok;
            Data = data;
        }

        public int Status
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public object Data
        {
            get;
            set;
        }
    }

    public enum MessageStatus
    {
        Ok = 0,
        BusinessException = 1,
        SystemException = 2,
        AuthenticationException = 3
    }
}