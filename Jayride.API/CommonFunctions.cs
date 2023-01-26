using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Jayride.API
{
    public class CommonFunctions : ICommonFunctions
    {
        public string GetLocalIpAddress() {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public string GetPublicIpAddress() {
            var address = string.Empty;
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream())) {
                address = stream.ReadToEnd();
            }

            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);

            return address;
        }
    }

    public interface ICommonFunctions
    {
        public string GetLocalIpAddress();
        public string GetPublicIpAddress();
    }
}
