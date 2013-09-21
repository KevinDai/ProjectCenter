using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectCenter.Web.Models
{
    public class ItemSelectViewModel
    {
        public ItemSelectViewModel(string value, string text)
        {
            Value = value;
            Text = text;
        }

        public string Value
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }
    }
}