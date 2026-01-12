using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpListener _server;
        private Dictionary<TcpClient, string> _clients = new Dictionary<TcpClient, string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _server = new TcpListener(IPAddress.Any, 8888);
                _server.Start();
                lblStatus.Text = " Status: Running (Port 8888)";
                lblStatus.Foreground = System.Windows.Media.Brushes.Green;
                btnStart.IsEnabled = false;

                _ = Task.Run(() => AcceptClients()); // Chạy nền để chờ kết nối
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task AcceptClients()
        {
            while (true)
            {
                TcpClient client = await _server.AcceptTcpClientAsync();
                _clients.Add(client, "New Client");
                //UpdateLog("Một Client mới đã kết nối.");
                _ = Task.Run(() => HandleClient(client));
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            string clientName = "";

            try
            {
                // 1. Đọc tin nhắn đầu tiên (chính là Tên của Client)
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                lock (_clients) { _clients[client] = clientName; }
                UpdateLog($"{clientName} đã tham gia phòng chat.");
                BroadcastMessage($"Hệ thống: {clientName} đã vào phòng.");

                while (true)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    //UpdateLog("Client: " + message);
                    // Gửi lại tin nhắn cho tất cả các client (Broadcast)
                    // Gửi tin nhắn kèm theo tên người gửi
                    string formattedMsg = $"{clientName}: {message}";
                    UpdateLog(formattedMsg);
                    BroadcastMessage($"{clientName}: " + message);
                }
            }
            catch { }
            finally
            {
                lock (_clients) { _clients.Remove(client); }
                client.Close();
                if (!string.IsNullOrEmpty(clientName))
                {
                    UpdateLog($"{clientName} đã thoát.");
                    BroadcastMessage($"Hệ thống: {clientName} đã rời phòng.");
                }
            }
        }

        private void BroadcastMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            // Tạo một bản sao danh sách để duyệt (tránh lỗi khi có client thoát giữa chừng)
            List<TcpClient> currentClients;
            lock (_clients) { currentClients = new List<TcpClient>(_clients.Keys); }

            foreach (var client in currentClients)
            {
                try
                {
                    if (client.Connected)
                    {
                        client.GetStream().Write(buffer, 0, buffer.Length);
                    }
                }
                catch {
                    BroadcastMessage("Hệ thống: 1 Client bị mất kết nối.");
                }
            }
        }

        private void UpdateLog(string text)
        {
            // Vì Update UI từ thread khác nên dùng Dispatcher
            Dispatcher.Invoke(() => lstMessages.Items.Add($"{DateTime.Now:HH:mm:ss}: {text}"));
        }
    }
}