using System;
using System.Text;
using EventStore.ClientAPI;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ExpectedVersion = EventStore.ClientAPI.ExpectedVersion;

namespace ReactiveDomain.Persistence.Samples
{
    public static class TestConnections
    {

        public static void TestEventStoreConnection(IEventStoreConnection esConn, string streamName)
        {
            esConn.ConnectAsync().Wait();

            for (var x = 0; x < 2; x++)
            {
                var data = new EventStore.ClientAPI.EventData(
                    Guid.NewGuid(),
                    "event-type",
                    true,
                    Encoding.ASCII.GetBytes("{\"somedata\" : " + x + "}"),
                    Encoding.ASCII.GetBytes("{\"metadata\" : " + x + "}")
                );
                esConn.AppendToStreamAsync(streamName, ExpectedVersion.Any, data).Wait();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"{x} Events written to {streamName}");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
        }

        public static void TestReactiveDomainConnection(IStreamStoreConnection rdConn, string streamName)
        {
            var testId = Guid.NewGuid();
            IStreamNameBuilder nameBuilder = new PrefixedCamelCaseStreamNameBuilder();
            IEventSerializer serializer = new JsonMessageSerializer();
            var repository = new StreamStoreRepository(nameBuilder, rdConn, serializer);
            IListener listener = new StreamListener(streamName, rdConn, nameBuilder, serializer);
            // var readModel = new TestReadModel(() => listener, testId);

            try
            {
                repository.Save(new TestDataUtility(testId));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"TestData: {testId} created");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Exception: {e.Message}");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                return;
            }

            for (var x = 0; x < 2; x++)
            {
                var myTestData = repository.GetById<TestDataUtility>(testId);
                myTestData.AddValue(x);
                repository.Save(myTestData);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Added a new value: {x} to {myTestData}");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
        }
    }
}
