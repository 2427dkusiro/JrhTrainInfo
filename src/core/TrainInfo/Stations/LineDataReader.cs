using System;

using TrainInfo.Resources;

namespace TrainInfo.Stations
{
    /// <summary>
    /// 路線データの読み取りを提供します。
    /// </summary>
    public static class LineDataReader
    {
        /// <summary>
        /// 路線を指定してその路線にある全ての駅を取得します。
        /// </summary>
        /// <param name="jrhLine">取得対象の路線。</param>
        /// <returns>該当する駅。札幌が起点です。</returns>
        /// <exception cref="NotSupportedException">路線が対応していない路線の場合にスローされる例外。</exception>
        public static Station[] GetStations(JrhLine jrhLine)
        {
            return StationData.LineDataDictionary[jrhLine];
        }
    }
}
