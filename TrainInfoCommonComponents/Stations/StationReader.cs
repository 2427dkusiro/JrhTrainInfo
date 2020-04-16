using System;
using System.Collections.Generic;
using System.Linq;
using TrainInfo.Resources;

namespace TrainInfo.Stations
{
    /// <summary>
    /// 駅データの読み取りを提供します。
    /// </summary>
    public static class StationReader
    {
        static StationReader()
        {

        }

        private const int UnKnownStationID = -1;

        /// <summary>
        /// 駅IDから駅を検索して返します。
        /// </summary>
        /// <param name="id">検索対象の駅ID。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationById(int id)
        {
            if (id == UnKnownStationID)
            {
                throw new FormatException($"ID{id}は未知の駅として予約されており、利用できません");
            }

            if (id >= StationData.Stations.Length)
            {
                return null;
            }
            return StationData.Stations[id];
        }

        /// <summary>
        /// 駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var result = GetStationByKanjiName(name);
            if (result is null)
            {
                result = GetStationByHiraganaName(name);
            }

            if (result is null)
            {
                result = GetStationByKatakanaName(name);
            }

            if (result is null)
            {
                result = GetStationByRomanName(name);
            }

            return result;
        }

        /// <summary>
        /// 漢字駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByKanjiName(string name)
        {
            if (StationData.KanjiDictionary.TryGetValue(name, out var station))
            {
                return station;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 平仮名駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByHiraganaName(string name)
        {
            if (StationData.HiraganaDictionary.TryGetValue(name, out var station))
            {
                return station;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 片仮名駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByKatakanaName(string name)
        {
            if (StationData.KatakanaDictionary.TryGetValue(name, out var station))
            {
                return station;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// ローマ字駅名から駅を検索して返します。
        /// </summary>
        /// <param name="name">検索対象の駅名。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByRomanName(string name)
        {
            if (StationData.RomanDictionary.TryGetValue(name, out var station))
            {
                return station;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 路線とその路線上での位置から駅を検索して返します。
        /// </summary>
        /// <param name="jrhLine">検索対象の路線。</param>
        /// <param name="position">検索対象の駅の位置。</param>
        /// <returns>該当する駅。存在しない場合は<c>null</c></returns>
        public static Station GetStationByPosition(JrhLine jrhLine, int position)
        {
            return LineDataReader.GetStations(jrhLine).FirstOrDefault(sta => sta.Position[jrhLine] == position);
        }

        /// <summary>
        /// 該当する駅名の駅を探し、存在しなければ仮の駅名データを作成して返します。
        /// </summary>
        /// <param name="name">駅名。</param>
        /// <returns></returns>
        public static Station GetOrCreateStationByName(string name)
        {
            var station = GetStationByName(name);
            if (station == null)
            {
                station = new Station(UnKnownStationID, name);
            }
            return station;
        }

        public static IEnumerable<Station> GetAllStations()
        {
            return StationData.Stations.Where(std => std != null);
        }

        public static IEnumerable<Station> GetAllEnds()
        {
            return StationData.Stations.Where(std => std?.IsEndStation ?? false);
        }
    }
}
