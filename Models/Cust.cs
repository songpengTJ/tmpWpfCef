using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WpfCef.Models
{
    public class Cust: TreeItemBase
    {
        public string custcode { get; set; }
        public string custname { get; set; }
        public int carsize { get; set; }

        public override string Title { get => custname + "["+carsize+"]"; }
        public override string ImageName { get => "123"; }
        public override int type { get => 0; }
        public override string id { get => custcode; }

        public string qryParentId() {
            int len = custcode.Length;
            if (len < 8) 
                return "1234";
            return custcode.Substring(0,len-4);
        }
    }
}
