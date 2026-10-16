

using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberSecurityAwarenessBot
{
    public partial class MainWindow : Window
    {
        private Chatbot chatbot;
        private bool nameCaptured = false;

        public MainWindow()
        {
            InitializeComponent();

            chatbot = new Chatbot("User");

            // ASCII art shown in the GUI (monospace font set in XAML).
            AsciiLogo.Text =
@"            _________
           /  _____  \
          |  |     |  |
          |  |     |  |
      ____|__|_____|__|____
     |                     |
     |         ( )         |
     |          |          |
     |_____________________|

   ============================
   ||  C Y B E R   S A F E   ||
   || STAY ALERT. STAY SAFE! ||
   ============================";

            PlayVoiceGreeting();

            AddMessage("BOT: Welcome to the CyberSecurity Awareness Bot!", Brushes.LightGreen);
            AddMessage("BOT: What is your name?", Brushes.LightGreen);

            SendButton.Click += SendButton_Click;
            MessageInput.KeyDown += MessageInput_KeyDown;

            MessageInput.Focus();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string path = Path.Combine(AppContext.BaseDirectory, "Audio", "greeting.wav");

                if (File.Exists(path))
                {
                    new SoundPlayer(path).Play();
                }
                else
                {
                    AddMessage("BOT: (Voice greeting file was not found.)", Brushes.Orange);
                }
            }
            catch (Exception)
            {
                AddMessage("BOT: (The voice greeting could not be played.)", Brushes.Orange);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string message = MessageInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(message))
                return;

            AddMessage("YOU: " + message, Brushes.White);

            string response;

            if (!nameCaptured)
            {
                // First reply is treated as the user's name.
                string name = message;

                if (name.StartsWith("my name is ", StringComparison.OrdinalIgnoreCase))
                    name = name.Substring(11).Trim();

                chatbot.UserName = name;
                nameCaptured = true;

                response = $"Nice to meet you, {chatbot.UserName}! " +
                           "Ask me about passwords, scams, privacy, phishing, malware or safe browsing.";
            }
            else
            {
                response = chatbot.ProcessResponse(message);
            }

            AddMessage("BOT: " + response, Brushes.LightGreen);

            MessageInput.Clear();
            MessageInput.Focus();
        }

        private void AddMessage(string message, Brush colour)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = message,
                Foreground = colour,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 5, 0, 12)
            };

            ChatPanel.Children.Add(textBlock);
            ChatScrollViewer.ScrollToEnd();
        }
    }
}
