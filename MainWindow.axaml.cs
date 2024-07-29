using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using Tmds.DBus.Protocol;

namespace ServerSwitcherV2
{
    public partial class MainWindow : Window
    {
        string SelectedFile = "";
        private string CoopNetServer = "net.coop64.us";
        private string BurritoNetServer = "coop.burritobot.net";

        private Dictionary<string, (string url, string label)> servers = new Dictionary<string, (string, string)>
        {
            { "CoopNet", ("net.coop64.us", "Connected to CoopNet") },
            { "BurritoNet", ("coop.burritobot.net", "Connected to BurritoNet") }
        };


        public MainWindow()
        {
            InitializeComponent();
            switch (Environment.OSVersion.Platform.ToString())
            {
                case "Win32NT":
                case "Win32Windows":
                case "Win32S":
                    SelectedFile = Environment.ExpandEnvironmentVariables(@"%APPDATA%\");
                    break;
                case "Unix":
                    SelectedFile = Environment.ExpandEnvironmentVariables(@"%HOME%/.local/share/");
                    break;
                case "MacOSX":
                    SelectedFile = Environment.ExpandEnvironmentVariables(@"%HOME%/Library/Application Support/");
                    break; 
            }
        }

        private void GetServerState(string path)
        {
            if (File.Exists(path))
            {
                string currentServer = ReadLine(path);
                foreach (var server in servers.Values)
                {
                    if (currentServer.Contains(server.url))
                    {
                        ConnectLabel.Content = server.label;
                        break;
                    }
                }
            }
            else
            {
                ConnectLabel.Content = "SM64 Directory not found...";
                ConnectButton.IsEnabled = false;
            }
        }

        public void EXCoop(object sender, RoutedEventArgs args)
        {
            SelectedFile += "sm64ex-coop/sm64config.txt";
            SwapWindow(1);
            GetServerState(SelectedFile);
        }

        public void CoopDX(object sender, RoutedEventArgs args)
        {
            SelectedFile += "sm64coopdx/sm64config.txt";
            SwapWindow(1);
            GetServerState(SelectedFile);
        }

        public void ConnectHandle(object sender, RoutedEventArgs args)
        {
            ComboBoxItem cbi = (ComboBoxItem)DropDown.SelectedItem;
            string selectedText = cbi.Content.ToString();
            ConnectToServer(selectedText);
        }

        public void SwapWindow(int win)
        {
            switch (win)
            {
                //Implementing this for a "Back button"
                //When I decide to take 18 hours reorganizing the damn UI...
                case 0:
                    MainPanel.IsVisible = true;
                    ConnectPanel.IsVisible = false;
                    break;
                case 1:
                    MainPanel.IsVisible = false;
                    ConnectPanel.IsVisible = true;
                    break;
            }
        }

        private void ConnectToServer(string selectedText)
        {
            if (servers.TryGetValue(selectedText, out var serverInfo))
            {
                ConnectLabel.Content = serverInfo.label;
                ReplaceLineInFile(SelectedFile, serverInfo.url);
            }
            else
            {
                ConnectLabel.Content = "Unknown server";
            }
        }

        static string ReadLine(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("coopnet_ip"))
                {
                    return lines[i];
                }
            }

            return string.Empty;
        }

        static void ReplaceLineInFile(string filePath, string newLine)
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("coopnet_ip"))
                {
                    lines[i] = "coopnet_ip " + newLine;
                    break;
                }
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}