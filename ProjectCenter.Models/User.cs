using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Models
{
    public class User
    {
        public string Id
        {
            get;
            set;
        }

        public string LoginName
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
		
		public string Passwrod
        {
            get;
            set;
        }

        /// <summary>
        /// 权限级别（从高到低）：1、2、3
        /// </summary>
        public int RightLevel
        {
            get;
            set;
        }
    }
}
