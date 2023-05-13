using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace TCP_Clinet
{
    internal class Program
    {
        public class Response
        {
            public TcpClient Connection { get; set; }
            public SslStream SSL { get; set; }
        }
        public static byte[] buffer = new byte[4096];
        public static BlockingCollection<Response> ResponseChannel = new BlockingCollection<Response>();

        public static Random random = new Random();
        static void Main(string[] args)
        {
            new Thread(ReadResponse).Start();

            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36";
                headers["Content-Type"] = "application/x-www-form-urlencoded; charset=UTF-8";
                headers["X-Csrftoken"] = "missing";
                headers["Host"] = "i.instagram.com";
                headers["Connection"] = "Close";
                string Data = $"username=kkk0";
                string Packet = ParseRequest("POST", "i.instagram.com", "/accounts/web_create_ajax/attempt/", headers, null, Data);
                TcpClient client = Set_Client("i.instagram.com", 443, false);
                byte[] packets = Encoding.UTF8.GetBytes(Packet);
                SslStream sslStream = WriteSSl(client, packets);
                ResponseChannel.Add(new Response
                {
                    SSL = sslStream,
                    Connection = client
                });

            }
            catch { }

            Console.ReadKey();

        }



        public static void ReadResponse()
        {
            while (true)
            {
                foreach (var response in ResponseChannel.GetConsumingEnumerable())
                {
                    string ResponseBody = "";
                    int bytes;
                    do
                    {
                        bytes = response.SSL.Read(buffer, 0, buffer.Length);
                        ResponseBody += Encoding.UTF8.GetString(buffer, 0, bytes);
                        ResetBuffer(buffer);
                        if (ResponseBody.IndexOf("<EOF>") != -1)
                        {
                            break;
                        }

                    } while (bytes != 0);
                    Console.WriteLine(ResponseBody);
                    response.Connection.Close();
                    response.SSL.Close();
                }
            }
        }

        public static void ResetBuffer(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }

        }


        public static TcpClient Set_Client(string Server, int Port, bool Proxy)
        {

            TcpClient client = new TcpClient();
            client.Connect(Server, Port);

            if (Proxy)
            {


                string request = $"CONNECT {Server}:443 HTTP/1.1\r\n\r\n";


                Stream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(request);
                writer.Flush();
                int n = stream.Read(buffer, 0, buffer.Length);
                if (n < buffer.Length)
                {
                    Array.Resize(ref buffer, n);
                }
            }

            return client;
        }
        public static SslStream WriteSSl(TcpClient connection, byte[] packets)
        {
            SslStream ssl = new SslStream(connection.GetStream(), false, ValidateServerCertificate, null);
            ssl.AuthenticateAsClient("i.instagram.com");
            ssl.Write(packets);
            return ssl;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }
        public static string ParseRequest(string Method, string host, string Path, Dictionary<string, string> HttpsRequestHeader, Dictionary<string, string> Cookies, string PostData)
        {
            Method = Method.ToUpper();
            string Request = ($"{Method} {Path} HTTP/1.1\r\n");
            string Headers = "";
            foreach (KeyValuePair<string, string> Header in HttpsRequestHeader)
            {

                Headers += $"{Header.Key}: {Header.Value}\r\n";

            }
            if (!Headers.Contains("Host"))
            {
                Headers += $"Host: {host}\r\n";

            }
            if (Method == "POST")
            {
                if (!Headers.Contains("Content-Length"))
                {
                    Headers += ($"Content-Length: {PostData.Length}\r\n");
                }
            }
            if (Cookies != null)
            {
                string Cookie = "";
                foreach (KeyValuePair<string, string> Cookies1 in Cookies)
                {
                    Cookie += $"{Cookies1.Key}={Cookies1.Value}";
                }
                Request += $"Cookie: {Cookie}";
            }
            Request += ($"{Headers}\r\n");

            if (PostData != null)
            {
                Request += PostData;
            }
            return Request;

        }




    
}
}
