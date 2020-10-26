using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBHTraiCay.Models
{
    public class BadRequestGet
    {
        public string Message { get; set; }
    }
    public class BadRequestPost
    {
        public string Message { get; set; }
        public Dictionary<string, string[]> ModelState { get; set; }
    }
}