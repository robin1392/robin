using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;


namespace RandomWarsService.Network
{
    public class Utils
    {
        /// <summary>
        /// 포트 유효성 체크
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool CheckAvailablePort(int port)
        {
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }


        /// <summary>
        /// 사용 가능한 포트 검색
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static int SearchAvailablePort(int startPort, int limit)
        {
            for (int i = startPort; i < startPort + limit; i++)
            {
                if (CheckAvailablePort(i) == true)
                {
                    return i;
                }
            }
            
            return -1;
        }
    }
}
