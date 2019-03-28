using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TrainInfo.Models;

namespace TrainInfo.Stations
{
    public static class StationReader
    {
        /// <summary>
        /// サービス提供範囲内のすべての駅を取得します。
        /// </summary>
        public static Station[] Stations { get; private set; }

        /// <summary>
        /// <see cref="StationReader"/> クラスを初期化します。
        /// </summary>
        static StationReader()
        {
            Stations = GenerateStationList();
        }

        private static Station[] GenerateStationList()
        {
            List<Station> result = new List<Station>();
            foreach (Station[] stations in LineDataReader.GetAllStations())
            {
                foreach (Station station in stations)
                {
                    Station st = default(Station);
                    if (result.Any())
                    {
                        st = result.FirstOrDefault(sta => sta == station);
                    }
                    if (st == null)
                    {
                        result.Add(station);
                    }
                    else
                    {
                        st.Position.Add(station.Position.First().Key, station.Position.First().Value);
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 駅IDから駅を検索して返します。
        /// </summary>
        /// <param name="id">検索対象の駅ID。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationById(int id) =>
            Stations.FirstOrDefault(sta => sta.StationID == id);

        /// <summary>
        /// 駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByName(string name)
        {
            var result = GetStationByKanjiName(name);
            if (result is null)
                result = GetStationByHiraganaName(name);
            if (result is null)
                result = GetStationByKatakanaName(name);
            if (result is null)
                result = GetStationByRomanName(name);
            return result;
        }

        /// <summary>
        /// 漢字駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByKanjiName(string name)
        {
            return Stations.FirstOrDefault(sta => sta.Name == name);
        }

        /// <summary>
        /// 平仮名駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByHiraganaName(string name)
        {
            return Stations.FirstOrDefault(sta => sta.HiraName == name);
        }

        /// <summary>
        /// 片仮名駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByKatakanaName(string name)
        {
            return Stations.FirstOrDefault(sta => sta.KataName == name);
        }

        /// <summary>
        /// ローマ字駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByRomanName(string name)
        {
            return Stations.FirstOrDefault(sta => sta.EnglishName == name);
        }

        /// <summary>
        /// 該当する駅名の駅を探し、存在しなければ仮の駅名データを作成して返します。
        /// </summary>
        /// <param name="name">駅名。</param>
        /// <returns></returns>
        public static Station GetOrCreateStationByName(string name)
        {
            Station station = GetStationByName(name);
            if (station == null)
            {
                station = new Station(-1, name);
            }
            return station;
        }

    }
}
