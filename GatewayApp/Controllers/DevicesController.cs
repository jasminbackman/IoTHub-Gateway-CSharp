using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GatewayApp.Services;
using Microsoft.Extensions.Options;
using GatewayApp.Models;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GatewayApp.Controllers
{
    [Route("api/[controller]")]
    public class DevicesController : Controller
    {
        private readonly IOptions<IoTHubSettings> _iotHubSettings;

        public DevicesController(IOptions<IoTHubSettings> iotHubSettings)
        {
            _iotHubSettings = iotHubSettings;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            if (Debugger.IsAttached)
            {
                return new string[] {
                    "Server: " +_iotHubSettings.Value.Server,
                    "Default device name key: " + _iotHubSettings.Value.DefaultDeviceNameKey
                    };
            }
            return new string[] { "Only POST messages are supported." };
        }

        // Message with device id in url
        [HttpPost]
        [Route("{entity}/{deviceId}")]
        public async Task Post(string entity, string deviceId)
        {
            if(string.IsNullOrWhiteSpace(entity) || string.IsNullOrWhiteSpace(deviceId))
            {
                Response.StatusCode = 400;
                return;
            }
            Debug.WriteLine("Processing request");
            string message;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                message = await reader.ReadToEndAsync();
            }
            // UPDATE: 5.4.2018. Inserting deviceid to the message might not be necessary due to how the device name is formed in IoT Hub. 
            // However, this method might still be used in the future.
            //await Gateway.MoveDataInsertDeviceId(message, _iotHubSettings, Request.ContentType, deviceId, entity);
            await Gateway.MoveData(message, _iotHubSettings, Request.ContentType, entity, deviceId);
        }

        // Message without device id
        [HttpPost]
        [Route("{entity}")]
        public async Task Post(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                Response.StatusCode = 400;
                return;
            }
            Debug.WriteLine("Processing request");
            string message;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                message = await reader.ReadToEndAsync();
            }
            await Gateway.MoveData(message, _iotHubSettings, Request.ContentType, entity);
        }

    }
}
