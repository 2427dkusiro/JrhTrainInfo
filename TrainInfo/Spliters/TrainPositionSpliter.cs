using System;
using System.Collections.Generic;
using System.Text;
using TrainInfo;
using TrainInfo.Models;

namespace TrainInfo.Spliters
{
    /// <summary>
    /// 列車位置情報の解析を提供します。
    /// </summary>
    public static class TrainPositionSpliter
    {
        /// <summary>
        /// 列車位置を表す文字列から列車が存在する区間の情報を返します。駅上に存在する場合両端とも同じ駅になります。
        /// </summary>
        /// <param name="positionString">列車位置を表す文字列。</param>
        /// <returns></returns>
        public static LineRange GetTrainPosition(string positionString)
        {
            if (TryParse(positionString, out LineRange range))
            {
                return range;
            }
            else
            {
                string stationName = positionString.GetRange('(', ')');
                if (string.IsNullOrEmpty(stationName))
                {
                    return null;
                }

                if (stationName.EndsWith("駅"))
                {
                    stationName = stationName.Substring(0, stationName.Length - 1);
                }
                return new LineRange(stationName, stationName);
            }
        }

        /// <summary>
        /// 列車位置を表す文字列から列車が存在する区間の情報を返します。駅上に存在する場合両端とも同じ駅になります。
        /// </summary>
        /// <param name="positionString">列車位置を表す文字列。</param>
        /// <param name="delimiter">2つの駅を区切る文字。</param>
        /// <returns></returns>
        public static LineRange GetTrainPosition(string positionString, char delimiter)
        {
            if (TryParse(positionString, delimiter, out LineRange range))
            {
                return range;
            }
            else
            {
                string stationName = positionString.GetRange('(', ')');
                if (string.IsNullOrEmpty(stationName))
                {
                    throw new FormatException();
                }

                if (stationName.EndsWith("駅"))
                {
                    stationName = stationName.Substring(0, stationName.Length - 1);
                }
                return new LineRange(stationName, stationName);
            }
        }

        /// <summary>
        /// 文字列を路線区間に変換します。
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static LineRange Parse(string s)
        {
            if (TryParse(s, out LineRange range))
                return range;
            else
                throw new FormatException();
        }

        /// <summary>
        /// 文字列を路線区間に変換します。
        /// </summary>
        /// <param name="lineRangeString">変換対象の文字列</param>
        /// <param name="delimiter">両端の駅を区切る文字</param>
        /// <param name="range">変換の結果。変換に失敗した場合は<c>null</c>を返します</param>
        /// <returns>変換が成功したかどうか</returns>
        private static bool TryParse(string lineRangeString, char delimiter, out LineRange range)
        {
            //サッポロビール庭園対策
            if (lineRangeString.Contains("サッポロビ-ル庭園"))
            {
                lineRangeString = lineRangeString.Replace("サッポロビ-ル庭園", "サッポロビール庭園");
            }

            if (lineRangeString.Contains("("))
                lineRangeString = lineRangeString.GetRange('(', ')');

            string[] str = lineRangeString.Split(new char[] { delimiter, '間' });
            if (str.Length != 3)
            {
                range = null;
                return false;
            }
            else
            {
                range = new LineRange(str[0], str[1]);
                return true;
            }
        }

        /// <summary>
        /// 文字列を路線区間に変換します。
        /// </summary>
        /// <param name="lineRangeString">変換対象の文字列</param>
        /// <param name="range">変換の結果。変換に失敗した場合は<c>null</c>を返します</param>
        /// <returns>変換が成功したかどうか</returns>
        private static bool TryParse(string lineRangeString, out LineRange range)
        {
            return TryParse(lineRangeString, '-', out range);
        }
    }
}
