using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Socket Web類
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SocketServer
{
    class Program
    {
        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //埠
        static Socket serverSocket;
        static void Main(string[] args)
        {
            //伺服器IP地址
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //繫結IP地址：埠
            serverSocket.Listen(10);    //設定最多10個排隊連線請求
            Console.WriteLine("啟動監聽{0}成功", serverSocket.LocalEndPoint.ToString());
            //通過Clientsoket傳送資料
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }

        /// <summary>
        /// 監聽客戶端連線
        /// </summary>
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                clientSocket.Send(Encoding.ASCII.GetBytes("Server Say Hello"));
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }

        /// <summary>
        /// 接收訊息
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int id;
                    //通過clientSocket接收資料
                    int receiveNumber = myClientSocket.Receive(result);
                    Console.WriteLine("接收客戶端{0}訊息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
                    id = Int32.Parse(Encoding.ASCII.GetString(result, 0, receiveNumber));
                    serverConn(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }

        //與Wowza主機連線Test
        public static void serverConn(int id)
        {
            Program work = new Program();
            //ParameterizedThreadStart s = new ParameterizedThreadStart(Hello);
            Thread thread = new Thread(new ParameterizedThreadStart(ThreadWork));
            thread.Start(id);
        }

        //Thread Work
        public static void ThreadWork(object id)
        {
            Console.Write(id);
        }
    }
}
