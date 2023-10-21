using DnsClient;


namespace Unifiedpost.LogParser.Services
{
    internal class IpAnalyzer
    {
        public async Task AnalyzeLogData(string logFilePath, LookupClient dnsClient)
        {

            try
            {
                //Firstly, we get the index of 'c-ip' column so we can perform the further fetching all ip addresses from the log file.
                var header = File.ReadLines(logFilePath).FirstOrDefault(x => x.StartsWith("#Fields"));

                if (header is null)
                    return;
                int cipIndex = Array.IndexOf(header.Split(' '), "c-ip") - 1; ;  //we got the index of 'c-ip' column

                //now we perform fetching and analyzing of ip adresses by processing every line in parallel
                await Parallel.ForEachAsync(File.ReadLinesAsync(logFilePath), async (line, _) =>
                {
                    //skipping lines with no ip address 
                    if (line.StartsWith("#"))
                    {
                        return;
                    }
                    var ip = line.Split(' ').ElementAt(cipIndex);
                    var dns = await dnsClient.GetHostEntryAsync(ip);    //DnsClient method for geting DNS server for the given ip address

                    //if the ip address is already saved in IpAdddressRegistry, we increase it Count (Number of hits), else we add it
                    //I decided to use Concurrent dictionary for this purpose because I needed thread-safe collection due the fact I am 
                    //doing multithread access of it.
                    if (LogData.IpAddressRegistry.ContainsKey(ip))
                        LogData.IpAddressRegistry[ip].Count++;
                    else
                        LogData.IpAddressRegistry[ip] = new Models.IpAddressData { Count = 1, Dns = dns?.HostName };
                });
            }
            catch (Exception ex)
            {
                //For some ip addresses I've got exception due the timeout of performing dns lookup,
                //I would suggest using Polly for handling transient errors like this and performing multiple repeated action
                //with exponentionally increased timeout
            }

           
        }
    }
}
