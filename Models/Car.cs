using System;
using System.Collections.Generic;
using System.Text;

namespace WpfCef.Models
{
    public class Car: TreeItemBase
    {
        public string beizhu { get; set; }
        public string creator { get; set; }

        public string cretime { get; set; }
        public string custcode { get; set; }
        public string custname { get; set; }
        public string jt809_flag { get; set; }
        public string jt809_color { get; set; }
        public string jt809_owers_id { get; set; }
        public string jt809_owers_name { get; set; }
        public string jt809_owers_tel { get; set; }
        public string jt809_trans_type { get; set; }
        public string jt809_vehicle_nationality { get; set; }
        public string jt809_vehicle_type { get; set; }
        public string parkAccDelay { get; set; }
        public string rptcode { get; set; }

        public string serviceexpire { get; set; }
        public string telcode { get; set; }
        public string terminal_id { get; set; }
        public string terminal_makeid { get; set; }
        public string terminal_model { get; set; }
        public string terminal_otname1 { get; set; }
        public string terminal_otname2 { get; set; }
        public string vid { get; set; }
        public string vid2 { get; set; }
        public string xy { get; set; }

        public override string Title { get => vid; }
        public override string ImageName { get => "456"; }
        public override int type { get => 1; }
        public override string id { get => "car," + rptcode; }
    }
}
