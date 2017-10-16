using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TCPBridge
{
    class TCPBridge
    {

        static void Main(string[] args)
        {

            TcpListener firstServer = null;
            TcpListener secoundServer = null;

            try
            {

                string endIPString = Console.ReadLine();
                IPAddress endIP = IPAddress.Parse(endIPString);
                Int32 portOne = 5001;
                Int32 portTwo = 5002;

                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                IPEndPoint ipEndPointFirst = new IPEndPoint(endIP, portOne);
                IPEndPoint ipEndPointSecound = new IPEndPoint(endIP, portTwo);

                firstServer = new TcpListener(localAddr, portOne);
                secoundServer = new TcpListener(localAddr, portTwo);

                firstServer.Start();
                secoundServer.Start();
                Byte[] bytes = new Byte[256];
                String data = null;

                while (true)
                {

                    ConnectTo5001(firstServer, bytes, data, ipEndPointFirst).Wait(100);

                    ConnectTo5002(secoundServer, bytes, data, ipEndPointSecound).Wait(100);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
                Console.ReadKey();

            }


        }

        static async Task ConnectTo5001(TcpListener firstServer, Byte[] bytes, String data, IPEndPoint ipEndPointFirst)
        {
            TcpClient firstClient = await firstServer.AcceptTcpClientAsync();
            NetworkStream firstStream = firstClient.GetStream();

            Console.Write(firstClient.Client.RemoteEndPoint);

            data = null;
            int i;

            while ((i = firstStream.Read(bytes, 0, bytes.Length)) != 0)
            {

                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                WriteInformation(firstClient.Client.RemoteEndPoint.ToString(), ipEndPointFirst.ToString(), data);
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                TcpClient firstSender = new TcpClient();
                firstSender.Connect(ipEndPointFirst);

                NetworkStream firstStreamSend = firstSender.GetStream();
                firstStreamSend.Write(msg, 0, msg.Length);

                firstStreamSend.Close();
                firstStream.Close();
                firstSender.Close();
            }

        }

        static async Task ConnectTo5002(TcpListener secoundServer, Byte[] bytes, String data, IPEndPoint ipEndPointSecound)
        {
            TcpClient secoundClient = await secoundServer.AcceptTcpClientAsync();
            NetworkStream secoundStream = secoundClient.GetStream();

            data = null;
            int i;

            while ((i = secoundStream.Read(bytes, 0, bytes.Length)) != 0)
            {

                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                WriteInformation(secoundClient.Client.RemoteEndPoint.ToString(), ipEndPointSecound.ToString(), data);
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                TcpClient secoundSender = new TcpClient();
                secoundSender.Connect(ipEndPointSecound);

                NetworkStream secoundStreamSend = secoundSender.GetStream();
                secoundStreamSend.Write(msg, 0, msg.Length);

                secoundStreamSend.Close();
                secoundStream.Close();
                secoundSender.Close();
            }

        }

        private static void WriteInformation(string ipFrom, string ipTo, string data)
        {
            using (StreamWriter outputFile = new StreamWriter("log.txt", true))
            {
                outputFile.WriteLine("From: " + ipFrom + " Data: " + data + " To: " + ipTo);
            }
        }
    }
}
