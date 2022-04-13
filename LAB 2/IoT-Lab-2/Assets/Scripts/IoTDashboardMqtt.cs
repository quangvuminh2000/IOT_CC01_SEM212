using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;



namespace IoT_dashboard
{
    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    /// 
    public class Status_Data
    {
        public int temperature { get; set; }
        public int humidity { get; set; }
    }

    public class ControlDevice_Data
    {
        public string device { get; set; }
        public string status { get; set; }
    }

    public class IoTDashboardMqtt : M2MqttUnityClient
    {
        public InputField addressInputField;
        public InputField userInputField;
        public InputField pwdInputField;
        public Text errorDisplay;


        public List<string> topics = new List<string>();

        public string msg_received_from_topic_status = "";
        [SerializeField]
        public Status_Data _status_data;
        [SerializeField]
        public ControlDevice_Data _control_data;
        public Text text_display;

        
        public void UpdateBeforeConnect()
        {

            this.brokerAddress = addressInputField.text;
            this.brokerPort = 1883;
            this.mqttUserName = userInputField.text;
            this.mqttPassword = pwdInputField.text;

            Debug.Log("Connecting");
            this.Connect();

            Debug.Log(errorDisplay.text);
            if (!errorDisplay.text.Contains("ERROR"))
            {
                GetComponent<DashboardManager>().SwitchLayer();
            }
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

        public void Message(string msg)
        {
            if (errorDisplay != null)
            {
                errorDisplay.text = msg;
            }
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
        }

        protected override void OnConnected()
        {
            base.OnConnected();
        }

        protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { topics[0] }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            Debug.Log("Subscribed topics. ");
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { topics[0] });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log(errorMessage);
            Message("ERROR: \n" + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
            Message("ERROR: \n" + "CONNECTION DISCONNECTED! ");
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
            Message("ERROR: \n" + "CONNECTION LOST! ");
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);
        }

        private void ProcessMessageStatus(string msg)
        {
            _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<DashboardManager>().Update_Status(_status_data);
        }

        public void PublishLED()
        {
            _control_data = new ControlDevice_Data();
            _control_data = GetComponent<DashboardManager>().OnLEDValueChange(_control_data);
            string msg_config = JsonConvert.SerializeObject(_control_data);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish led" + msg_config);
        }

        public void PublishPump()
        {
            _control_data = new ControlDevice_Data();
            _control_data = GetComponent<DashboardManager>().OnPumpValueChange(_control_data);
            string msg_config = JsonConvert.SerializeObject(_control_data);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish pump" + msg_config);
        }

    }
}
