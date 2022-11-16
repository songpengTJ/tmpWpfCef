using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfCef
{
    public class Tool
    {
        public static Models.CCPack pack = null;

        public static void parseJson() {
            string jsonfile = "cars.txt";
            string ret = File.ReadAllText(jsonfile);
            pack = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.CCPack>(ret);
            pack.data.cust = pack.data.cust.OrderBy(s => s.custcode).ToList();
        }
    }
}
