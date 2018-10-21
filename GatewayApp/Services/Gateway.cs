using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using GatewayApp.Models;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace GatewayApp.Services
{
    public static class Gateway
    {

        // Might be used in the future, but not in use for now ( Updated: 5.4.2018 ) //
        //
        //
        //public static async Task MoveDataInsertDeviceId(string msg, IOptions<IoTHubSettings> _iotHubSettings, string contentType, string deviceName, string companyName)
        //{
        //    if (string.IsNullOrWhiteSpace(msg) || _iotHubSettings == null || string.IsNullOrWhiteSpace(contentType))
        //    {
        //        return;
        //    }
        //    switch (contentType)
        //    {
        //        case "application/json":
        //            msg = InsertDeviceIdToBodyJSON(msg, _iotHubSettings.Value.DeviceNameKey, deviceName);
        //            break;

        //    }
        //    await MoveData(msg, _iotHubSettings, contentType, companyName, deviceName);
        //}
        //
        //
        ///
        public static async Task MoveData(string message, IOptions<IoTHubSettings> _iotHubSettings, string contentType, string companyName, string deviceName = null)
        {
            if(string.IsNullOrWhiteSpace(message) || _iotHubSettings == null || string.IsNullOrWhiteSpace(companyName) || string.IsNullOrWhiteSpace(contentType))
            {
                return;
            }

            // UPDATE: 5.4.2018 Changing to use a default device name instead for now
            //
            //
            // If deviceName isn't set and we're not sending the messages to the default device
            //if(string.IsNullOrWhiteSpace(deviceName))
            //{
            //    // Find device name key for the given entity
            //    var entities = _iotHubSettings.Value.Entities.Where(entity => entity.Name == companyName);

            //    if (contentType == "application/json")
            //    {
            //        deviceName = GetDeviceIdFromBodyJSON(
            //            message,
            //            ((entities == null || entities.Count() < 1) ? _iotHubSettings.Value.DefaultDeviceNameKey : entities.First().DeviceNameKey)
            //            );
            //        Debug.WriteLine(deviceName);
            //    }
            //}

            // If device id is null, use default name
            if (string.IsNullOrWhiteSpace(deviceName))
            {
                deviceName = _iotHubSettings.Value.DefaultDeviceName;
            }
            // Create device id using company name and provided device name
            string deviceId = companyName+":"+deviceName;
            // If device client with the specific name hasn't been created yet
            if (!DeviceCollection.deviceClients.ContainsKey(deviceId))
            {
                await new RegistryHandler().AddDeviceIfNotExistsAsync(deviceId, _iotHubSettings);
                DeviceCollection.deviceClients.TryAdd(deviceId, DeviceClient.Create(_iotHubSettings.Value.Server,
                    new DeviceAuthenticationWithSharedAccessPolicyKey(
                        deviceId,
                        _iotHubSettings.Value.Device.PolicyName,
                        _iotHubSettings.Value.Device.PolicyKey
                    ), TransportType.Amqp));
            }
            message = "{ data: '" + message + "' }";
            using (MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(message ?? "")))
            {
                await DeviceCollection.deviceClients[deviceId].SendEventAsync(new Message(mem));
            }
        }

        // Private utility methods //

        private static string GetDeviceIdFromBodyJSON(string message, string keyToSearch)
        {
            try
            {
                JObject jObj = JObject.Parse(message);
                return jObj.Value<string>(keyToSearch);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return "";
        }
        private static string InsertDeviceIdToBodyJSON(string message, string devicekey, string deviceName)
        {
            try
            {
                JObject jObj = JObject.Parse(message);
                jObj.Add(devicekey, deviceName);
                return jObj.ToString();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return message;
        }

    }
}
