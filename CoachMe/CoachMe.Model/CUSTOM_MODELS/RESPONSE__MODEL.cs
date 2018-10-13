using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.MODEL.CUSTOM_MODELS
{
    public class RESPONSE__MODEL
    {
        public bool STATUS { get; set; }

        public dynamic OUTPUT_DATA { get; set; }

        public string ErrorMessage { get; set; }

        public string InnerException { get; set; }

        public string Path { get; set; }

    }
}
