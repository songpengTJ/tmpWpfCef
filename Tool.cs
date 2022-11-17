using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using WpfCef.Models;

namespace WpfCef
{
    public class Tool
    {
        public static Models.CCPack pack = null;

        public static void parseJson() {
            string jsonfile = "cars.txt";
            string ret = File.ReadAllText(jsonfile);
            if (ret == null) 
                return;
            pack = Newtonsoft.Json.JsonConvert.DeserializeObject<CCPack>(ret);
            pack.data.cust = pack.data.cust.OrderBy(s => s.custcode).ToList();

        }
    }
}
