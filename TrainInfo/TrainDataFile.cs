using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainInfo;
using TrainInfo.Stations;
using TrainInfo.Models;

namespace TrainInfo
{
    /// <summary>
    /// 解析済みの列車データセットを表します。
    /// </summary>
    public class TrainDataFile
    {
        /// <summary>
        /// データの取得対象の駅を取得または設定します。
        /// </summary>
        public Station Station { get; set; }

        /// <summary>
        /// 取得日時を取得または設定します。
        /// </summary>
        public DateTime GetedDateTime { get; set; }

        /// <summary>
        /// 出発列車の情報を取得または設定します。
        /// </summary>
        public TrainDataSet[] DepartureTrainDatas { get; set; }

        /// <summary>
        /// 到着列車の情報を取得または設定します。
        /// </summary>
        public TrainDataSet[] ArrivalTrainDatas { get; set; }

        /// <summary>
        /// 路線を指定してその路線上のすべての列車を取得します。
        /// </summary>
        /// <param name="lineDestPair">対象の路線。</param>
        /// <returns></returns>
        public IEnumerable<(TrainData trainData, decimal position)> SearchArrivalTrainDataWithPosition(JrhLine jrhLine, JrhDestType destType)
        {
            var trainDataSet = ArrivalTrainDatas.FirstOrDefault(tds => tds.DestType == destType);
            if (trainDataSet == null) yield break;

            foreach (TrainData trainData in trainDataSet.TrainDatas)
            {
                decimal? position = trainData.NowPosition?.GetNowPosition(jrhLine);
                if (position is decimal notNull)
                {
                    yield return (trainData, notNull);
                }
            }
        }
    }
}

/// <summary>
/// 同じ方面の列車データのセットを表します。
/// </summary>
public class TrainDataSet
{
    /// <summary>
    /// 列車データのセットを取得または設定します。
    /// </summary>
    public TrainData[] TrainDatas { get; set; }

    /// <summary>
    /// 行先の方面を取得または設定します。
    /// </summary>
    public JrhDestType DestType { get; set; }

    /// <summary>
    /// このクラスを表す文字列を取得します。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{DestType.GetName()}の列車{TrainDatas.Length}個のセット";
    }
}
