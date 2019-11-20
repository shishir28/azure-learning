using System;
using System.Collections.Generic;
using System.Text;

namespace Moand_IoT.Common
{
    public class Telemetry
    {
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public StatusType Status { get; set; }
    }
}
