using System;
using System.Collections.Generic;
using System.Text;

namespace ap_cli.MicrosoftGraphBeta
{
    class AccessPackageResourceRequest
    {
        public string catalogId { get; set; }
        public string requestType { get; set; }
        public AccessPackageResource accessPackageResource { get; set; }
    }

    class AccessPackageResource
    {
        public string originId { get; set; }
        public string originSystem { get; set; }


    }
}
