using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayApp.Models
{
    public class IoTHubSettings
    {
        public string Server { get; set; }
        public string DefaultDeviceNameKey { get; set; }
        public string DefaultDeviceName { get; set; }
        public IoTHubPolicy Device { get; set; }
        public IoTHubPolicy Registry { get; set; }
    }
    public class IoTHubPolicy
    {
        public string PolicyName { get; set; }
        public string PolicyKey { get; set; }
    }
}
