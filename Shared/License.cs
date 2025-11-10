using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class License
    {
  
        public string ProductName { get; set; }
        public string TargetMachineIdentity { get; set; }
        public string PublicKey { get; set; }
        public string Signature { get; set; }
        public string LicenseData { get; set; }
    }
}
