using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrainInfoWPF.Debuger
{
    /// <summary>
    /// DebugConsoleWindow.xaml の相互作用ロジック
    /// </summary>
    partial class DebugConsoleWindow : Window
    {
        private bool CanReadUserInput = false;

        private string memoryedText;

        public DebugConsoleWindow()
        {
            InitializeComponent();

            WriteLine($"TrainInfo WPF [Version:{Assembly.GetExecutingAssembly().GetName().Version.ToString()}]");
            WriteLine(DateTime.Now);

        }

        #region WriteMethod
        /// <summary>
        /// コンソールに文字列を書き込みます
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            memoryedText += text;
            MainTextBox.Text = memoryedText;
            MainTextBox.CaretIndex = memoryedText.Length;
        }

        /// <summary>
        /// コンソールに入力を文字列として書き込みます
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        public void Write<T>(T text)
        {
            memoryedText += text.ToString();
            MainTextBox.Text = memoryedText;
            MainTextBox.CaretIndex = memoryedText.Length;
        }

        /// <summary>
        /// コンソールに入力を書き込んだ後改行します
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            Write(text + Environment.NewLine);
        }

        /// <summary>
        /// コンソールに入力を文字列として書き込んだ後改行します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        public void WriteLine<T>(T text)
        {
            Write(text.ToString() + Environment.NewLine);
        }
        #endregion

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainTextBox.Text.Length < memoryedText.Length || !CanReadUserInput)
            {
                MainTextBox.Text = memoryedText;
                MainTextBox.CaretIndex = memoryedText.Length;
            }
        }

        private void MainTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (CanReadUserInput)
            {
                if (e.Key == Key.Enter)
                {
                    string input = MainTextBox.Text.Remove(0, memoryedText.Length);
                    WriteLine(input);
                    memoryedText = MainTextBox.Text;
                }
            }
        }

        private void MainTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void ClearConsole()
        {
            memoryedText = "";
            MainTextBox.Text = "";

            WriteLine($"TrainInfo WPF [Version:{Assembly.GetExecutingAssembly().GetName().Version.ToString()}]");
            WriteLine(DateTime.Now);
        }
    }
}