using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace EventSimulator {
    class Program {

        private static EventHubClient eventHubClient;
        private const string EventHubConnectionString = "<Event hub connection string>";
        private const string EventHubName = "<Event hub name>";

        private static List<string> DeviceIds = new List<string> { "a45184f7-ac74-4f48-a4a3-2a550d079fbf", "84914b37-a0d6-458e-b018-8787ff334b08", "2fcc96ae-c1f4-45e8-ba54-c989e602da39", "1c77b223-6bdc-4e14-916c-483c0f59d46e", "0e66d5f6-6110-468b-b0be-54c6c95d68a5" };
        private static List<string> ClientIds = new List<string> { "2312132b-e139-4c84-895d-25c733b242aa", "98411af8-e614-4d62-80f3-900db4bce084", "d799ce6c-2971-4f5f-af19-87846885c5fb", "efea0e71-0d21-4015-bf4b-bd5f4a9e8945", "a760b4bc-8eff-42a4-afaa-cebb7ff7032c", "06dedb07-3d5b-4896-991d-312ebb7bd922", "5f089049-5e9f-4ede-be5e-5b2d895e98b0", "eae737c5-6a44-4721-bb44-6393f21dfe0d", "ff957272-3dd2-4cf6-b216-d00e536d98ba" };

        static void Main (string[] args) {
            MainAsync (args).GetAwaiter ().GetResult ();
        }

        private static async Task MainAsync (string[] args) {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but this simple scenario
            // uses the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder (EventHubConnectionString) {
                EntityPath = EventHubName
            };

            eventHubClient = EventHubClient.CreateFromConnectionString (connectionStringBuilder.ToString ());

            await SendMessagesToEventHub (100);

            await eventHubClient.CloseAsync ();

            Console.WriteLine ("Press ENTER to exit.");
            Console.ReadLine ();
        }

        // Uses the event hub client to send 100 messages to the event hub.
        private static async Task SendMessagesToEventHub (int numMessagesToSend) {
            for (var i = 1; i <= numMessagesToSend; i++) {
                var deviceIdIndex = GetRandomNumber (5);
                var clientIdIndex = GetRandomNumber (9);

                var data = new SensorData {
                    Id = i,
                    ClientId = ClientIds[clientIdIndex],
                    DeviceId = DeviceIds[deviceIdIndex],
                    Humidity = GetRandomDouble (50.0, 100.0),
                    Temperature = GetRandomDouble (15.0, 30.0),
                    TimeStamp = DateTime.Now
                };

                try {
                    var eventData = JsonConvert.SerializeObject (data);

                    Console.WriteLine ($"Sending event: {eventData}");
                    await eventHubClient.SendAsync (new EventData (Encoding.UTF8.GetBytes (eventData)));
                } catch (Exception exception) {
                    Console.WriteLine ($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay (2);
            }

            Console.WriteLine ($"{numMessagesToSend} events sent.");
        }

        public static double GetRandomDouble (double minimum, double maximum) {
            Random random = new Random ();
            return random.NextDouble () * (maximum - minimum) + minimum;
        }

        public static int GetRandomNumber (int maximum) {
            Random random = new Random ();
            return random.Next (maximum);
        }
    }

    public class SensorData {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string DeviceId { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public DateTime TimeStamp { get; set; }

    }
}