using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Beha_Reader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        SerialPort _serialPort = new SerialPort();
        bool _continue = false;
        BackgroundWorker backGroundWorker;




        public MainWindow()
        {
            InitializeComponent();

            string[] lista_portas = System.IO.Ports.SerialPort.GetPortNames();

            foreach (var item in lista_portas)
            {
                ComboBox1.Items.Add(item);
            }
            ComboBox1.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort.IsOpen == false)
            {
                _serialPort = new SerialPort();

                // Allow the user to set the appropriate properties.
                _serialPort.PortName = ComboBox1.Text;
                _serialPort.BaudRate = 2400;
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;

                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;
                try
                {
                    _serialPort.Open();
                }
                catch (Exception messa)
                {
                    MessageBox.Show(messa.Message);
                }
                
                _continue = true;


                backGroundWorker = new BackgroundWorker();
                backGroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
                backGroundWorker.RunWorkerAsync();

                backGroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
                backGroundWorker.WorkerReportsProgress = true;
                ButtonOpenClose.Content = "Fechar";
            }
            else
            {
                _continue = false;
                _serialPort.Close();
                ButtonOpenClose.Content = "Abrir";
            }
      

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        { 
            while ( _continue)
            {

                 string message = _serialPort.ReadExisting();

           
                Thread.Sleep(1);
            

                backGroundWorker.ReportProgress(0, message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update the UI here
            //        _userController.UpdateUsersOnMap();

            texbox1.AppendText(e.UserState as string);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            texbox1.Clear();
        }

        private void buttonsavetofile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.FilterIndex = 1;


            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, texbox1.Text);
            }
        }
    }
}
