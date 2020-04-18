using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TrainInfoWPF.Debuger
{
    /// <summary>
    /// DebugConsoleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DebugConsoleWindow : UserControl
    {
        private bool CanReadUserInput = false;

        private string memoryedText;

        public DebugConsoleWindow()
        {
            InitializeComponent();

            WriteLine($"TrainInfo WPF [Version:{Assembly.GetExecutingAssembly().GetName().Version.ToString()}] ");
        }

        private object writingObject = new object();

        #region WriteMethod
        /// <summary>
        /// コンソールに文字列を書き込みます
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            lock (writingObject)
            {
                memoryedText += text;
                Dispatcher.Invoke(() =>
                {
                    MainTextBox.Text = memoryedText;
                    MainTextBox.CaretIndex = memoryedText.Length;
                });
            }
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
                    var input = MainTextBox.Text.Remove(0, memoryedText.Length);
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
        }
    }
}