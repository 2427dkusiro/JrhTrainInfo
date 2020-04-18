using Newtonsoft.Json;

namespace TrainInfo.TrainDatas
{
    /// <summary>
    /// Webから取得された運行情報の列車データを表現します。
    /// </summary>
    public class RawTrainData
    {
        /// <summary>
        /// 列車種別を取得または設定します。
        /// </summary>
        [JsonProperty("class")]
        public string TrainType { get; set; }

        /// <summary>
        /// 列車名及び種別などの注釈を取得または設定します。
        /// </summary>
        [JsonProperty("name")]
        public string TrainName { get; set; }

        /// <summary>
        /// 出発または到着の時刻を取得または設定します。
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }

        /// <summary>
        /// 目的地を取得または設定します。
        /// </summary>
        [JsonProperty("to")]
        public string Destination { get; set; }

        /// <summary>
        /// 運行状況を表す番号を取得または設定します。
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }

        /// <summary>
        /// 部分運休区間などの追加情報を取得または設定します。
        /// </summary>
        [JsonProperty("add")]
        public string Addition { get; set; }

        /// <summary>
        /// 現在位置を取得または設定します。
        /// </summary>
        [JsonProperty("now")]
        public string NowPosition { get; set; }

        /// <summary>
        /// 最終目的地を取得または設定します。
        /// </summary>
        [JsonProperty("dest")]
        public string FinalDestination { get; set; }
    }
}
