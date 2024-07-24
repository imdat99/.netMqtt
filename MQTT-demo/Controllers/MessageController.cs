using Microsoft.AspNetCore.Mvc;
using MQTT_demo.Models;
using MQTTnet;
using MQTTnet.Client;

namespace MQTT_demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMqttClient _mqttClient;
        private readonly ILogger<MessageController> _logger;
        public MessageController(IMqttClient mqttClient, ILogger<MessageController> logger)
        {
            _mqttClient = mqttClient;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Message message)
        {
            if (_mqttClient == null)
            {
                string msg = "MQTT client is not initialized.";
                _logger.LogError(msg);
                return StatusCode(500, msg);
            }

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic("test/hallo")
                .WithPayload(message.Body)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(mqttMessage, System.Threading.CancellationToken.None);
            return Ok("Message published!");
        }
    }
}
