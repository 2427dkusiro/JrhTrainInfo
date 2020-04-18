using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace TrainInfo.Debuggers
{
    internal static class LogWriter
    {
        public static void WriteLog(string message)
        {
            WriteLine(message);
        }

        public static void WriteLog(string message, object source)
        {
            WriteLine($"{source.GetType()}からのログ:{message}");
        }

        public static void WriteExceptionLog(Exception exception)
        {
            WriteLine($"例外が発生しました。\n例外の詳細:{exception.ToString()}");
        }

        public static void WriteExceptionLog(Exception exception, string message)
        {
            WriteLine($"例外が発生しました。{message}\n例外の詳細:{exception.ToString()}");
        }

        public static void WriteObjectLog(object obj)
        {
            var str = JsonConvert.SerializeObject(obj);
            WriteLine($"{obj.GetType().Name}のログ:\n{str}");
        }

        public static void WriteObjectLog(object obj, string message)
        {
            var str = JsonConvert.SerializeObject(obj);
            WriteLine($"{obj.GetType().Name}のログ\nメッセージ:{message}\n{str}");
        }

        public static void WriteObjectLog(object obj, string message, object source)
        {
            var str = JsonConvert.SerializeObject(obj);
            WriteLine($"{source.GetType()}からの{obj.GetType().Name}のログ\nメッセージ:{message}\n{str}");
        }

        private static void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
