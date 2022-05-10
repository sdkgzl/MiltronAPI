using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class RocketDto
    {

        public string id { get; set; }
        public string model { get; set; }
        public uint mass { get; set; }
        public string status { get; set; }
        public uint altitude { get; set; }
        public uint speed { get; set; }
        public uint acceleration { get; set; }
        public uint thrust { get; set; }
        public float temperature { get; set; }
        public payload payload { get; set; }
        public telemetry telemetry { get; set; }
        public timestamps timestamps { get; set; }       
    }
}
