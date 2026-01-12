using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên!");
                return;
            }

            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(txtIP.Text, 8888);
                _stream = _client.GetStream();

                byte[] nameBuffer = Encoding.UTF8.GetBytes(txtUserName.Text);
                await _stream.WriteAsync(nameBuffer, 0, nameBuffer.Length);

                btnConnect.IsEnabled = false;
                txtUserName.IsEnabled = false;
                lstChat.Items.Add("Hệ thống: Đã kết nối tới Server!");

                _ = Task.Run(() => ReceiveMessages());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (true)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Dispatcher.Invoke(() => lstChat.Items.Add(message));
                }
            }
            catch
            {
                Dispatcher.Invoke(() => lstChat.Items.Add("Hệ thống: Mất kết nối tới server."));
            }
        }

        private void btnEmoji_Click(object sender, RoutedEventArgs e)
        {
            popEmoji.IsOpen = !popEmoji.IsOpen;
        }

        private void AddEmoji_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                // Thêm emoji vào vị trí con trỏ đang đứng trong TextBox
                int cursorPosition = txtMsg.SelectionStart;
                txtMsg.Text = txtMsg.Text.Insert(cursorPosition, btn.Content.ToString());

                // Đưa con trỏ ra sau emoji vừa chèn
                txtMsg.SelectionStart = cursorPosition + btn.Content.ToString().Length;
                txtMsg.Focus();

                // Đóng bảng emoji sau khi chọn
                popEmoji.IsOpen = false;
            }
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private async Task SendMessage()
        {
            if (_stream != null && !string.IsNullOrEmpty(txtMsg.Text))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(txtMsg.Text);
                await _stream.WriteAsync(buffer, 0, buffer.Length);
                txtMsg.Clear();
            }
        }

        private async void txtMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) await SendMessage();
        }
    }
}