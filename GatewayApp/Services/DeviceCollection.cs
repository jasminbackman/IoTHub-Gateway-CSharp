using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GatewayApp.Services
{
    public static class DeviceCollection
    {
        public static ConcurrentDictionary<string, DeviceClient> deviceClients = new ConcurrentDictionary<string, DeviceClient>();
    }
}
