using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public class Attachment
    {
        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string ProjectId
        {
            get;
            set;
        }

        public string UploadUserId
        {
            get;
            set;
        }

        public string UploadUserName
        {
            get;
            set;
        }

        public DateTime UploadTime
        {
            get;
            set;
        }
    }
}
