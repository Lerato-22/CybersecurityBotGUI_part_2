using CybersecurityBotGUI_part_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
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


namespace CybersecurityBotGUI_part_2
{
    public partial class MainWindow : Window
    {
        private string _userName = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SoundPlayer player = new SoundPlayer(
                    System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, "greeting.wav"));
                player.PlaySync();
            }
            catch { }
        }

        private void NameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) StartChat_Click(null, null);
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                NameInput.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }

            _userName = name;
            Responses.Remember("name", name);
            SidebarUserName.Text = name;

            NamePanel.Visibility = Visibility.Collapsed;
            ChatScroll.Visibility = Visibility.Visible;
            UserInput.Focus();

            AddBotMessage($"Hey {name}! 👋 Welcome to the Cybersecurity Awareness Bot.\n\nI'm here to help you stay safe online. Type 'help' to see all the topics I can assist you with, or just ask me anything about cybersecurity!");
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_userName)) return;
            Button btn = sender as Button;
            string topic = btn?.Tag?.ToString() ?? "";
            UserInput.Text = topic;
            SendMessage();
        }

        private void SendMessage()
        {
            string input = UserInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AddUserMessage(input);
            UserInput.Clear();

            string response = Responses.GetResponse(input, _userName);

            if (response == "__UNKNOWN__")
                response = "🤔 Hmm, I didn't quite understand that. Could you rephrase?\n\nType 'help' to see all the topics I can help you with!";

            AddBotMessage(response);

            // Disable input after goodbye
            if (response.Contains("Stay safe online"))
            {
                UserInput.IsEnabled = false;
                UserInput.Text = "Chat ended — thank you for chatting!";
            }
        }

        private void AddUserMessage(string text)
        {
            Border bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(123, 47, 190)),
                CornerRadius = new CornerRadius(12, 12, 2, 12),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(80, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxWidth = 450
            };

            TextBlock tb = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = tb;
            ChatPanel.Children.Add(bubble);
            ScrollToBottom();
        }

        private void AddBotMessage(string text)
        {
            StackPanel row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5, 5, 80, 5)
            };

            Border avatar = new Border
            {
                Width = 36,
                Height = 36,
                Background = new SolidColorBrush(Color.FromRgb(123, 47, 190)),
                CornerRadius = new CornerRadius(18),
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Top
            };

            TextBlock avatarText = new TextBlock
            {
                Text = "🤖",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            avatar.Child = avatarText;

            Border bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(26, 26, 46)),
                CornerRadius = new CornerRadius(2, 12, 12, 12),
                Padding = new Thickness(12, 8, 12, 8),
                MaxWidth = 500,
                BorderBrush = new SolidColorBrush(Color.FromRgb(123, 47, 190)),
                BorderThickness = new Thickness(1)
            };

            TextBlock tb = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 20
            };

            bubble.Child = tb;
            row.Children.Add(avatar);
            row.Children.Add(bubble);
            ChatPanel.Children.Add(row);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            ChatScroll.UpdateLayout();
            ChatScroll.ScrollToEnd();
        }
    }
}