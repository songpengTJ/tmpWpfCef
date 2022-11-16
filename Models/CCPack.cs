using System;
using System.Collections.Generic;
using System.Text;

namespace WpfCef.Models
{
    public class CCPack
    {
        public int code { get; set; }
        public string desc { get; set; }
        public __DATA data { get; set; }


        public class __DATA
        {
            public List<Car> cars { get; set; }
            public List<Cust> cust { get; set; }

            public Dictionary<string, string> cfgmap { get; set; }
        }
    }
}
