using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrainInfo.TrainDatas
{
    internal static class JsonDeserializer
    {
        /// <summary>
        /// 運行情報の Json データをデシリアライズします。
        /// </summary>
        /// <param name="trainInfoJson">列車情報の Json データ。</param>
        /// <returns>運行情報の生データ。</returns>
        public static RawTrainDataFile DeserializeTrainInfo(string trainInfoJson)
        {
            RawTrainDataFile result;

            try
            {
                var jObj = JObject.Parse(trainInfoJson);
                jObj = (JObject)jObj.Properties().First().Value;
                var id = int.Parse(jObj.Properties().First().Name);
                jObj = (JObject)jObj.Properties().First().Value;
                result = jObj.ToObject<RawTrainDataFile>();
                result.StationId = id;
            }

            catch (Exception ex)
            {
                throw;
            }

            if (result == null)
            {
                throw new RawTrainDataDeserializeExeption("列車Jsonデータのデシリアライズに失敗しました");
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// 運行情報の Json データをデシリアライズします。
        /// </summary>
        /// <param name="trainInfoJson">列車情報の Json データ。</param>
        /// <returns>運行情報の生データ。</returns>
        public static RawTrainDataFile[] DeserializeSpecialTrainInfo(string trainInfoJson)
        {
            var jObj = JObject.Parse(trainInfoJson);
            var result = new List<RawTrainDataFile>();

            jObj = (JObject)jObj.Properties().First().Value;
            foreach (var dataObj in jObj)
            {
                var data = dataObj.Value.ToObject<RawTrainDataFile>();
                data.StationId = int.Parse(dataObj.Key);
                result.Add(data);
            }

            if (!result.Any())
            {
                throw new RawTrainDataDeserializeExeption("列車Jsonデータのデシリアライズに失敗しました");
            }
            else
            {
                return result.ToArray();
            }
        }
    }

    /// <summary>
    /// 未加工列車データの正式が正しくないときに発生する例外です。
    /// </summary>
    public class RawTrainDataDeserializeExeption : Exception
    {
        /// <summary>
        /// エラーを発生させたデータを取得します。
        /// </summary>
        public string JsonData { get; private set; }

        /// <summary>
        /// <see cref="RawTrainDataDeserializeExeption"/> クラスの新しいインスタンスを、規定のエラーメッセージで初期化します。
        /// </summary>
        public RawTrainDataDeserializeExeption(string data) : base("未加工列車データの形式が正しくありません")
        {
            Debuggers.LogWriter.WriteExceptionLog(this);
            Debuggers.LogWriter.WriteObjectLog(data);
            JsonData = data;
        }


        /// <summary>
        /// <see cref="RawTrainDataDeserializeExeption"/> クラスの新しいインスタンスを、エラーメッセージを指定して初期化します。
        /// </summary>
        /// <param name="data">エラーを発生させたデータ。</param>
        /// <param name="message">エラーメッセージ。</param>
        public RawTrainDataDeserializeExeption(string data, string message) : base(message)
        {
            Debuggers.LogWriter.WriteExceptionLog(this);
            Debuggers.LogWriter.WriteObjectLog(data);
            JsonData = data;
        }


        /// <summary>
        /// <see cref="RawTrainDataDeserializeExeption"/> クラスの新しいインスタンスを、エラーメッセージと内部例外を指定して初期化します。
        /// </summary>
        /// <param name="data">エラーを発生させたデータ。</param>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="exception">内部例外。</param>
        public RawTrainDataDeserializeExeption(string data, string message, Exception exception) : base(message, exception)

        {
            Debuggers.LogWriter.WriteExceptionLog(this);
            Debuggers.LogWriter.WriteObjectLog(data);
            JsonData = data;
        }
    }
}
