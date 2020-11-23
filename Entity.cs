using System;
using System.Collections.Generic;
using System.Text;

namespace RSIST950
{
    public class Entity
    {
        public string CycleSecond { get; set; }
        public int ProgramNumber { get; set; }
        public int StageNumber { get; set; }
        // 32
        public List<string> Phases { get; set; }
        // 25
        public List<int> Detectors { get; set; }
        public int DL20 { get; set; }
        public int DP20 { get; set; }
        public int DM20 { get; set; }
        public int DS20 { get; set; }
        public int DQ20 { get; set; }
        public int DF20 { get; set; }
        public int DL21 { get; set; }
        public int DP21 { get; set; }
        public int DM21 { get; set; }
        public int DS21 { get; set; }
        public int DQ21 { get; set; }
        public int DF21 { get; set; }
        public int OOODM20 { get; set; }
        public int OOODM21 { get; set; }
        public int OOODP20 { get; set; }
        public int OOODP21 { get; set; }
        public int OOODQ20 { get; set; }
        public int OOODQ21 { get; set; }
        public int SITOK { get; set; }
        // 8
        public List<int> Outputs { get; set; }
    }
}
