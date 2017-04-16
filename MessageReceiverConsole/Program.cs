using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace MessageReceiverConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri uri = ServiceBusEnvironment.CreateServiceUri("sb", "ocmmobiletest", string.Empty);

            string name = "RootManageSharedAccessKey";
            string key = "wg7KwOaE9H44iqmn7MidIfZ0688NGQvZRDZ4IcKIFJs=";
            TokenProvider tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(name, key);

            NamespaceManager namespaceManager = new NamespaceManager(uri, tokenProvider);
            namespaceManager.CreateTopic("DataCollectionTopic");

            namespaceManager.CreateSubscription("DataCollectionTopic", "Inventory");
            namespaceManager.CreateSubscription("DataCollectionTopic", "Dashboard");

            MessagingFactory factory = MessagingFactory.Create(uri, tokenProvider);
            BrokeredMessage bm = new BrokeredMessage("xxx");
            bm.Label = "SalesReport";
            bm.Properties["StoreName"] = "Redmond";
            bm.Properties["MachineID"] = "POS_1";

            MessageSender sender = factory.CreateMessageSender("DataCollectionTopic");
            sender.Send(bm);

            MessageReceiver receiver = factory.CreateMessageReceiver("DataCollectionTopic/subscriptions/Inventory");
            BrokeredMessage receivedMessage = receiver.Receive();
            try
            {
                //ProcessMessage(receivedMessage);
                receivedMessage.Complete();
            }
            catch (Exception e)
            {
                receivedMessage.Abandon();
            }
        }
    }
}
