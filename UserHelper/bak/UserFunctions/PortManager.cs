using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UserHelper.Database;

namespace UserHelper.UserFunctions
{
    public class PortManager
    {
        public static List<int> OpenPorts;

        public PortManager()
        {
            using (StudentsModel db = new StudentsModel())
            {
            }

            OpenPorts = new List<int>();
            ListAvailableTCPPort(ref OpenPorts);
        }
        public void ListAvailableTCPPort(ref List<int> usedPort)
        {
            usedPort.Clear();
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            IEnumerator myEnum = tcpConnInfoArray.GetEnumerator();

            while (myEnum.MoveNext())
            {
                TcpConnectionInformation TCPInfo = (TcpConnectionInformation)myEnum.Current;
                Console.WriteLine("Port {0} {1} {2} ", TCPInfo.LocalEndPoint, TCPInfo.RemoteEndPoint, TCPInfo.State);
                usedPort.Add(TCPInfo.LocalEndPoint.Port);
            }

        }
        public int GetFirstAvailablePort()
        {
            int maxPortNumber = 60000;
            int chosenPort = 1;
            for (int i = 1; i <= maxPortNumber; i++)
            {
                if (!OpenPorts.Contains(chosenPort))
                {
                    chosenPort = i;
                    break;
                }

            }
            return chosenPort;

        }
        public void UpdateOpenPortList()
        {
            ListAvailableTCPPort(ref OpenPorts);
        }
     
    }
}
