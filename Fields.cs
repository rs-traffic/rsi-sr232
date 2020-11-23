using System;
using System.Collections.Generic;
using System.Text;

namespace RSIST950
{
    public static class Enums
    {

        public enum FieldsToXml
        {
            CycleSecond,
            ProgramNumber,
            StageNumber,
            Phases,
            Detectors,
            SpecialDetectors,


            
            SITOK,
            Outputs
        }


        //DM21,
        //    DS21,
        //    DQ21,
        //    DF21,
        //    OOODM20,
        //    OOODM21,
        //    OOODP20,
        //    OOODP21,
        //    OOODQ20,
        //    OOODQ21




        public enum SpecialDetectors
        {
            DL20,
            DP20,
            DM20,
            DS20,
            DQ20,
            DF20,
            DL21,
            DP21,
            DM21,
            DS21,
            DQ21,
            DF21,
            OOODM20,
            OOODM21,
            OOODP20,
            OOODP21,
            OOODQ20,
            OOODQ21,
            SITOK
        }

        public enum ColorIndex
        {
            Red = 1,
            RedYellow = 2,
            Green = 3,
            Yellow = 4,
            FlashingGreen = 5,
          //  FlashingYellow = 6,
            GreenYellow = 6,
           // RedFlshingYellow = 8
        }

        public static class Color
        {
            public const string Red = "1000";
            public const string RedYellow = "1100";
            public const string Green = "0011";
            public const string Yellow = "0100";
            public const string FlashingGreen = "0010";
            //   FlashingYellow = 6,
            public const string GreenYellow = "0110";
           // public const string RedFlshingYellow = "1100";
            //public const string ee = "0000";
        }


    }
}
