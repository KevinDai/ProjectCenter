using ProjectCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCenter.Services.Models
{
    public class LoginResult
    {
        public User User
        {
            get;
            set;
        }

        public string FailMessage
        {
            get;
            set;
        }

    }
}
