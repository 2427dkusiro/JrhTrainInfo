using System;
using System.Linq;

using TrainInfo.ExtensionMethods;

using TrainInfo.Stations;

namespace TrainInfo.TrainDatas
{
    /// <summary>
    /// 列車データの解析を提供します。
    /// </summary>
    internal static class TrainDataParser
    {
        public static TrainData ParseTrainData(RawTrainData rawTrainData, int getedHour, Station station, TrainData.ArrivalTypes arrivalTypes)
        {
            try
            {
                var name = TrainNameParser.GetTrainName(rawTrainData.TrainName, rawTrainData.TrainType);
                var time = TrainTimeParser.GetTrainTime(rawTrainData.Time, getedHour);
                var destination = TrainDestParser.GetTrainDestination(rawTrainData.Destination);
                var condition = TrainConditionParser.GetTrainCondition((TrainData.TrainConditions)rawTrainData.Status, rawTrainData.Addition, rawTrainData.NowPosition);
                var nowPosition = TrainPositionTextParser.GetTrainPosition(rawTrainData.NowPosition);
                return new TrainData(name, time, station, arrivalTypes, destination, condition, nowPosition);
            }
            catch (Exception ex)
            {
                throw new TrainDataParseException(rawTrainData, "列車データのパースに失敗しました", ex);
            }
        }


        /// <summary>
        /// 列車運行状況の解析を提供します。
        /// </summary>
        private static class TrainConditionParser
        {
            public static TrainData.TrainCondition GetTrainCondition(TrainData.TrainConditions conditions, string suspendRange, string delayText)
            {
                var range = string.IsNullOrEmpty(suspendRange) ? null : SuspendRangeParser(suspendRange);
                if (!string.IsNullOrEmpty(delayText) && !delayText.StartsWith("始発駅の出発が遅れます") && !delayText.StartsWith("("))
                {
                    delayText = delayText.GetRangeWithEnd('(');
                    (var max, var min) = DelayTimeParser(delayText);
                    return new TrainData.TrainCondition(conditions, max, min);
                }
                else
                {
                    return new TrainData.TrainCondition(conditions, range);
                }
            }

            /// <summary>
            /// 運休区間を表す文字列から区間情報を返します
            /// </summary>
            /// <example>"運休(小樽-手稲間)</example>    
            /// <param name="text">運休区間を表す文字列</param>
            /// <returns>運休区間</returns>
            /// <exception cref="FormatException">運休区間が解釈できない形式のときにスローされる例外。</exception>
            private static LineRange SuspendRangeParser(string text)
            {
                if (text.StartsWith("運休"))
                {
                    text = text.Substring(2);
                    return TrainPositionTextParser.GetTrainPosition(text);
                }
                else
                {
                    throw new FormatException("運休区間を認識できません。");
                }
            }

            /// <summary>
            /// 遅延時間を表す文字列から遅延時刻の幅の最小値と最大値を返します。幅がない場合両端とも同じ値になります。
            /// </summary>
            /// <example><c>DelayTimeSpliter("約25～30分遅れ")</c></example>
            /// <param name="delayString">遅延時間を表す文字列</param>
            /// <returns></returns>
            /// <exception cref="FormatException">遅延時刻を解釈できないときにスローされる例外。</exception>
            private static (int min, int max) DelayTimeParser(string delayString)
            {
                if (delayString.Contains("遅れ"))
                {
                    if (delayString.Contains("～"))
                    {
                        var delayTexts = delayString.Split('～');
                        var result = delayTexts.Select(s => GetIntInString(s)).ToArray();
                        return (result[0], result[1]);
                    }
                    else
                    {
                        var result = ParseTimeString(delayString);
                        return (result, result);
                    }
                }
                else
                {
                    throw new FormatException("遅れ時刻を解釈できません。");
                }
            }

            /// <summary>
            /// 日本語の時間表現を分単位の時間に直します
            /// </summary>
            /// <param name="timeString">時間・分で表現された時刻</param>
            /// <returns></returns>
            private static int ParseTimeString(string timeString)
            {
                var hours = 0;
                var minutes = 0;

                if (timeString.Contains("約"))
                {
                    timeString = timeString.Replace("約", "");
                }
                if (timeString.Contains("時"))
                {
                    hours = GetIntInString(timeString.GetRangeWithEnd('時'));
                    timeString = timeString.GetRangeWithStart('時');
                }
                if (timeString.Contains("分"))
                {
                    minutes = GetIntInString(timeString.GetRangeWithEnd('分'));
                    //timeString = timeString.GetRangeWithStart('分');
                }

                return hours * 60 + minutes;
            }

            private static int GetIntInString(string s)
            {
                return int.Parse(new string(s.Where(c => int.TryParse(c.ToString(), out var n)).ToArray()));
            }
        }

        /// <summary>
        /// 列車行先の解析を提供します。
        /// </summary>
        private static class TrainDestParser
        {
            /// <summary>
            /// 列車の行先を設定します。
            /// </summary>
            /// <param name="destinationString">行先を表す文字列。</param>
            public static Station GetTrainDestination(string destinationString)
            {
                if (destinationString.Contains("("))
                {
                    destinationString = destinationString.GetRange('(', ')');
                }

                if (destinationString.EndsWith("行"))
                {
                    return StationReader.GetOrCreateStationByName(destinationString.Substring(0, destinationString.Length - 1));
                }
                else
                {
                    return StationReader.GetOrCreateStationByName(destinationString);
                }
            }
        }

        /// <summary>
        /// 列車名データの解析を提供します。
        /// </summary>
        private static class TrainNameParser
        {
            /// <summary>
            /// <see cref="TrainData.TrainName"/>クラスのインスタンスを<see cref="RawTrainData"/>をパースして作成します。
            /// </summary>
            public static TrainData.TrainName GetTrainName(string trainNameString, string trainTypeString)
            {
                TrainData.TrainTypes? subType = null;
                LineRange range = null;

                if (trainNameString.Contains("（"))
                {
                    var rangetext = trainNameString.GetRange('（', '）');//全角括弧

                    if (rangetext.Contains("間"))//いしかりライナー及びAP普通用
                    {
                        subType = TrainData.TrainTypesCreater.FromString(rangetext.GetRangeWithStart('間'));
                        var stations = rangetext.GetRangeWithEnd('間').Split('～');
                        range = new LineRange(stations[0], stations[1]);
                    }
                    else//すずらん室蘭行用
                    {
                        subType = TrainData.TrainTypes.Local;
                        range = new LineRange("東室蘭", "室蘭");
                    }
                    trainNameString = trainNameString.GetRangeWithEnd('（');
                }

                var name = ParseTrainNumber(trainNameString, out var number);
                var mainTrainType = TrainData.TrainTypesCreater.FromString(trainTypeString);

                return new TrainData.TrainName(mainTrainType, name, number, subType, range);
            }

            /// <summary>
            /// 号数を含む可能性のある文字列を列車名と号数に分離します
            /// </summary>
            /// <param name="nameString">列車名</param>
            /// <param name="number">号数。号数を含まない場合<c>null</c></param>
            /// <returns></returns>
            private static string ParseTrainNumber(string nameString, out int? number)
            {
                if (nameString.EndsWith("号"))
                {
                    var editedNameString = nameString.Substring(0, nameString.Length - 1).ToHankakuNum();
                    var numberString = new string(editedNameString.Where(c => int.TryParse(c.ToString(), out var n)).ToArray());

                    if (string.IsNullOrEmpty(numberString))
                    {
                        number = null;
                        return nameString;
                    }
                    else
                    {
                        number = int.Parse(numberString);
                        return editedNameString.Replace(numberString, "");
                    }
                }
                else
                {
                    number = null;
                    return nameString;
                }
            }
        }

        /// <summary>
        /// 列車位置情報を表すテキストの解析を提供します。
        /// </summary>
        private static class TrainPositionTextParser
        {
            /// <summary>
            /// 列車位置を表す文字列から列車が存在する区間の情報を返します。駅上に存在する場合両端とも同じ駅になります。
            /// </summary>
            /// <param name="positionString">列車位置を表す文字列。</param>
            /// <returns></returns>
            public static LineRange GetTrainPosition(string positionString)
            {
                if (TryParse(positionString, out var range))
                {
                    return range;
                }
                else
                {
                    var stationName = positionString.GetRange('(', ')');
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
            /// <exception cref="FormatException">列車位置が解釈できない場合にスローされる例外。</exception>
            public static LineRange GetTrainPosition(string positionString, char delimiter)
            {
                if (TryParse(positionString, delimiter, out var range))
                {
                    return range;
                }
                else
                {
                    var stationName = positionString.GetRange('(', ')');
                    if (string.IsNullOrEmpty(stationName))
                    {
                        throw new FormatException("列車位置が解釈できません。");
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
                {
                    lineRangeString = lineRangeString.GetRange('(', ')');
                }

                var str = lineRangeString.Split(new char[] { delimiter, '間' });
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

        /// <summary>
        /// 列車時刻情報の解析を提供します。
        /// </summary>
        private static class TrainTimeParser
        {
            /// <summary>
            /// 列車の出発または到着の時刻を設定します。
            /// </summary>
            /// <param name="timeString">時刻を表す文字列。</param>
            /// <param name="getedHour">現在の時間。日付の補正に使用します。</param>
            public static DateTime GetTrainTime(string timeString, int getedHour)
            {
                var result = DateTime.Parse(timeString);

                if (getedHour < 1 && result.Hour > 4)
                {
                    result = result.AddDays(-1);
                }
                else if (getedHour > 5 && result.Hour < 4)
                {
                    result = result.AddDays(1);
                }
                return result;
            }

            /// <summary>
            /// 2つの<see cref="DateTime"/>の時間以下を比較します。
            /// </summary>
            /// <param name="a">比較元</param>
            /// <param name="b">比較対象</param>
            /// <returns></returns>
            private static bool CompareTime(DateTime a, DateTime b)
            {
                if (a.Hour == b.Hour)
                {
                    if (a.Minute == b.Minute)
                    {
                        if (a.Second == b.Second)
                        {
                            return true;
                        }
                        else
                        {
                            return a.Second > b.Second;
                        }
                    }
                    else
                    {
                        return a.Minute > b.Minute;
                    }
                }
                else
                {
                    return a.Hour > b.Hour;
                }

            }
        }
    }

    /// <summary>
    /// 列車データの解析に失敗したときに発生する例外です。
    /// </summary>
    [Serializable]
    public class TrainDataParseException : Exception
    {
        /// <summary>
        /// エラーを発生させたデータを取得します。
        /// </summary>
        public RawTrainData RawTrainData { get; private set; }

        /// <summary>
        /// <see cref="TrainDataParseException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="rawTrainData">エラーを発生させたデータ。</param>
        public TrainDataParseException(RawTrainData rawTrainData)

        {
            Debuggers.LogWriter.WriteExceptionLog(this);
            Debuggers.LogWriter.WriteObjectLog(rawTrainData);
            RawTrainData = rawTrainData;
        }

        /// <summary>
        /// <see cref="TrainDataParseException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="rawTrainData">エラーを発生させたデータ。</param>
        /// <param name="message">エラーメッセージ。</param>
        public TrainDataParseException(RawTrainData rawTrainData, string message) : base(message)

        {
            Debuggers.LogWriter.WriteExceptionLog(this);
            Debuggers.LogWriter.WriteObjectLog(rawTrainData);
            RawTrainData = rawTrainData;
        }

        /// <summary>
        /// <see cref="TrainDataParseException"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="rawTrainData">エラーを発生させたデータ。</param>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="inner">内部例外。</param>
        public TrainDataParseException(RawTrainData rawTrainData, string message, Exception inner) : base(message, inner)

        {
            Debuggers.LogWriter.WriteExceptionLog(this);
            Debuggers.LogWriter.WriteObjectLog(rawTrainData);
            RawTrainData = rawTrainData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected TrainDataParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
