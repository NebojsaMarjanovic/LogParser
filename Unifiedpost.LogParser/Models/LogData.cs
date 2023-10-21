using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unifiedpost.LogParser.Models;

namespace Unifiedpost.LogParser
{
    public static class LogData
    {
        //Thread safe collection for performing multithread actions. 
        //The key is IP Address and the value is IpAddressData class that has Count (Number of hits) and Dns properties.
        public static ConcurrentDictionary<string, IpAddressData> IpAddressRegistry { get; set; } = new();

    }
}
