using System;
using System.Security.Cryptography.X509Certificates;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Security;
using System.Threading;
using System.Text;

namespace AMAZON_MQTT_TEST
{
    class DeviceSimulator
    {
        #region Contructor
        public DeviceSimulator()
        {
            CaCert = X509Certificate.CreateFromCertFile(path + "\\certificates\\root-CA.crt");
            ClientCert = new X509Certificate2(path + "\\certificates\\2.pfx", "");

            IotClient = new MqttClient(iotendpoint, brokerPort, true, CaCert, ClientCert, MqttSslProtocols.TLSv1_2);

            IotClient.Subscribe(topics, qosLevels);
            IotClient.MqttMsgPublishReceived += IotClient_MqttMsgPublishReceived;
        }

        private void IotClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string s = System.Text.Encoding.Default.GetString(e.Message);
            Console.WriteLine($"Received Message from AWS Server: {s}");
        }
        #endregion

        #region Fields
        MqttClient IotClient;
        string iotendpoint = "a16pmueljx93c-ats.iot.us-east-1.amazonaws.com";
        int brokerPort = 8883;
        const string Topic = "DeviceSimulator/Hello";


        string[] topics = { Topic, "DeviceSimulator/Hello" };
        byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE };

        X509Certificate CaCert;
        X509Certificate2 ClientCert;

        string Message = "Test message from ";
        string ClientId = Guid.NewGuid().ToString();

        string path
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }
        #endregion

        #region Properties
        #endregion

        public void SendValueTest(string name)
        {
            Console.WriteLine("Connecting to AWS server...");
            try
            {
                IotClient.Connect(ClientId);
                Console.WriteLine("Connected");
            }
            catch
            {
                Console.WriteLine("Connection can not be created!");
                return;
            }

            for (int i = 0; i < 1000; i++)
            {
                string msg = Message + name + "   /   " + DateTime.Now.ToString("HH:mm:ss");
                IotClient.Publish(Topic, Encoding.UTF8.GetBytes(msg));
                Console.WriteLine("published" + msg);
                Thread.Sleep(3000);
            }
        }
    }
}
