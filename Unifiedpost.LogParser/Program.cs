using DnsClient;
using System.Reflection;
using Unifiedpost.LogParser;
using Unifiedpost.LogParser.Services;

//The whole logic is inside the IpAnalyzer class which performs method AnalyzeLogData.
//Processed data is stored inside the concurrent static dictionary named IpAddressRegistry inside LogData static class.
//This method fetches the index of 'c-ip' column from log file and performs parallel processing of every line.
//During the processing IpAnalyzer gets IpAddresses and performs DNS Lookup using DnsClient library.
//After this process, IpAnalyzer adds new ip address in IpAddressRegistry if it doesn't exist, or performs update of its Count property.

//At the end I performed synchronous foreach just for printing cases.

var ipAnalyzer = new IpAnalyzer();

//I decided to use DnsClient library because of its easy-to-use interface and because I can set the timeout of dns lookup.
//I am not sure if it's possible inside the built-in Dns lookup in C#, so I didn't wanted to waste the time in searching for hard-code solution.
//I've set Timeout to be 3 seconds due the fact that some lookups can be very slow, but, as you will see, I've gave the potential sollution for that problem.
var dnsClient = new LookupClient(new LookupClientOptions() { Timeout = TimeSpan.FromSeconds(3) });


var file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Logs\ex120326.log"); ;
await ipAnalyzer.AnalyzeLogData(file, dnsClient);

//Writing in Console output
foreach (var kvp in LogData.IpAddressRegistry.OrderByDescending(x => x.Value.Count))
{
    Console.WriteLine($"{kvp.Value.Dns} ({kvp.Key}) - {kvp.Value.Count}");
}



