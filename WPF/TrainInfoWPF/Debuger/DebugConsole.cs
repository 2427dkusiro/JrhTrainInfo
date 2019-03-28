using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace TrainInfoWPF.Debuger
{
    class DebugConsoleWriter : TextWriter
    {
        private DebugConsoleWindow debugConsoleWindow;

        public override Encoding Encoding => Encoding.GetEncoding("Shift_JIS");

        public DebugConsoleWriter()
        {
            debugConsoleWindow = new DebugConsoleWindow();
            debugConsoleWindow.Show();
        }

        #region WriteMethod
        /// <summary>
        /// コンソールに文字列を書き込みます
        /// </summary>
        /// <param name="text"></param>
        public override void Write(string text)
        {
            debugConsoleWindow.Write(text);
        }

        /// <summary>
        /// コンソールに入力を文字列として書き込みます
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        public void Write<T>(T text)
        {
            debugConsoleWindow.Write(text);
        }

        /// <summary>
        /// コンソールに入力を書き込んだ後改行します
        /// </summary>
        /// <param name="text"></param>
        public override void WriteLine(string text)
        {
            debugConsoleWindow.WriteLine(text);
        }

        /// <summary>
        /// コンソールに入力を文字列として書き込んだ後改行します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        public void WriteLine<T>(T text)
        {
            debugConsoleWindow.WriteLine(text);
        }

        /// <summary>
        /// 現在時刻とともにコンソールに入力を書き込みます。
        /// </summary>
        /// <param name="text"></param>
        public void WriteWithTime(string text)
        {
            Write($"{DateTime.Now}: {text}");
        }

        /// <summary>
        /// 現在時刻とともにコンソールに入力を文字列として書き込みます。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        public void WriteWithTime<T>(T text)
        {
            Write($"{DateTime.Now}: {text.ToString()}");
        }

        /// <summary>
        /// 現在時刻とともにコンソールに入力を書き込んだ後改行します。
        /// </summary>
        /// <param name="text"></param>
        public void WriteLineWithTime(string text)
        {
            WriteLine($"{DateTime.Now}: {text}");
        }

        /// <summary>
        /// 現在時刻とともにコンソールに入力を文字列として書き込んだ後改行します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        public void WriteLineWithTime<T>(T text)
        {
            WriteLine($"{DateTime.Now}: {text.ToString()}");
        }

        #endregion
    }
}
