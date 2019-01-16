using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientConsole
{
    internal class Program
    {
        const string appName = "Client console";

        private static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder().WithUrl("https://localhost:44318/msg").Build();

            connection.On("ReceiveMessage", (string sender, string message) => Console.WriteLine($"{sender} says: {message}"));

            connection.Closed += async error =>
            {
                Console.WriteLine(error);
                await connection.StartAsync();
            };

            connection.StartAsync().Wait();

            var exitKey = string.Empty;

            while (true)
            {
                var line = Console.ReadLine();
                if (line == "/q")
                {
                    break;
                }

                var joinMatch = new Regex("/j (.+)");
                if (joinMatch.IsMatch(line))
                {
                    var groupName = joinMatch.Match(line).Groups[1].ToString();
                    // Console.WriteLine("join " + groupName);
                    connection.SendCoreAsync("AddToGroup", new[] {appName, groupName});
                    continue;
                }
                
                var tellMatch = new Regex("/t ([^ ]+) (.+)");
                if (tellMatch.IsMatch(line))
                {
                    var groupName = tellMatch.Match(line).Groups[1].ToString();
                    var message = tellMatch.Match(line).Groups[2].ToString();
                    connection.SendCoreAsync("SendGroup", new[] {appName, groupName, message});
                    continue;
                }



                connection.SendCoreAsync("Broadcast", new[] {appName, line});
            }
        }
    }
}
