using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TrainInfo.TrainDatas
{
    internal static class TrainDataGeter
    {
        private static readonly HttpClient httpClient;

        private const int retryTimeDelay = 1000;
        private const string baseUrl = @"https://www3.jrhokkaido.co.jp/monitor/unkou_info/json/services";
        private static readonly DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static TrainInfoReader.ITrainDataSource trainDataSource { get; set; }

        static TrainDataGeter()
        {
            httpClient = new HttpClient();
        }

        /// <summary>
        /// 駅IDを指定して列車情報の Json データを取得します。
        /// </summary>
        /// <param name="id">取得対象の駅 ID。</param>
        /// <returns>列車情報を表す Json データ。</returns>
        public static async Task<string> GetTrainDataJsonAsync(int id)
        {
            if (trainDataSource != null)
            {
                try
                {
                    return trainDataSource.GetTrainDataAsync(id);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            string idString;
            if (id.ToString().Length == 2)
            {
                idString = "0" + id.ToString();
            }
            else
            {
                idString = id.ToString();
            }

            string result;
            for (var i = 0; ; i++)
            {
                try
                {
                    result = await httpClient.GetStringAsync(new Uri($"{baseUrl}{idString}.json?_={GetUnixTime()}"));
                    break;
                }
                catch (HttpRequestException ex)
                {
                    if (i < 3)
                    {
                        Debuggers.LogWriter.WriteExceptionLog(ex, $"データ取得に失敗しました...再試行します({i}回目)");
                        await Task.Delay(retryTimeDelay);
                        continue;
                    }
                    else
                    {
                        throw new TrainDataGetException(id, "データ取得に失敗しました", ex);
                    }
                }
            }
            return result;
        }

        private static ulong GetUnixTime()
        {
            var time = DateTime.Now.ToUniversalTime() - unixTime;
            return (ulong)time.TotalSeconds;
        }
    }

    /// <summary>
    /// 列車データの取得に失敗したときに発生する例外です。
    /// </summary>
    [Serializable]
    public class TrainDataGetException : Exception
    {
        /// <summary>
        /// 取得が要求されたIDを取得します。
        /// </summary>
        public int RequestedId { get; private set; }

        /// <summary>
        /// <see cref="TrainDataGetException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="id"></param>
        public TrainDataGetException(int id)

        {
            Debuggers.LogWriter.WriteExceptionLog(this);
            Debuggers.LogWriter.WriteObjectLog(id);
            RequestedId = id;
        }

        /// <summary>
        /// <see cref="TrainDataGetException"/>クラスの新しいインスタンスを、エラーメッセージを指定して初期化します。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public TrainDataGetException(int id, string message) : base(message)

        {
            Debuggers.LogWriter.WriteExceptionLog(this);
            Debuggers.LogWriter.WriteObjectLog(id);
            RequestedId = id;
        }

        /// <summary>
        /// <see cref="TrainDataGetException"/>クラスの新しいインスタンスを、エラーメッセージと内部例外を指定して初期化します。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public TrainDataGetException(int id, string message, Exception inner) : base(message, inner)

        {
            Debuggers.LogWriter.WriteExceptionLog(this);
            Debuggers.LogWriter.WriteObjectLog(id);
            RequestedId = id;
        }
    }
}
