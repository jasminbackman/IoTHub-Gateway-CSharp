using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GatewayApp.Models;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Options;

namespace GatewayApp.Services
{
    public class RegistryHandler
    {
        private RegistryManager manager;

        private void CreateManager(IOptions<IoTHubSettings> _iotHubSettings)
        {
            string connectionString = string.Format(
                "HostName={0};SharedAccessKeyName={1};SharedAccessKey={2}",
                _iotHubSettings.Value.Server,
                _iotHubSettings.Value.Registry.PolicyName,
                _iotHubSettings.Value.Registry.PolicyKey
                );
            manager = RegistryManager.CreateFromConnectionString(connectionString);
        }
        public async Task AddDeviceIfNotExistsAsync(string deviceId, IOptions<IoTHubSettings> _iotHubSettings)
        {
            if (manager == null)
            {
                CreateManager(_iotHubSettings);
            }
            try
            {
                await manager.AddDeviceAsync(new Device(deviceId));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            await manager.CloseAsync();
        }
    }
}
