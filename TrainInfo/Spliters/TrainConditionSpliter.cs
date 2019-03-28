using System;
using System.Collections.Generic;
using System.Text;
using TrainInfo.Models;
using System.Linq;

namespace TrainInfo.Spliters
{
    class TrainConditionSpliter
    {
        /// <summary>
        /// 列車運行状況を設定します。
        /// </summary>
        /// <param name="condition">運行状況を表す文字列</param>
        /// <param name="suspendRange">運休区間を表す文字列。存在しない場合null</param>
        /// <param name="delayText">遅延状況を表す文字列</param>
        [Obsolete("Html版用メソッドです")]
        public static TrainCondition SetCondition(string condition, string suspendRange = null, string delayText = null)
        {
            TrainConditions conditions = GetTrainCondition(condition);
            LineRange range = suspendRange == null ? null : SuspendRangeSpliter(suspendRange);
            if (delayText != null && !delayText.StartsWith("始発駅の出発が遅れます") && !delayText.StartsWith("("))
            {
                delayText = delayText.GetRangeWithEnd('(');
                (int max, int min) = DelayTimeSpliter(delayText);
                return new TrainCondition(conditions, range, max, min);
            }
            else
            {
                return new TrainCondition(conditions, range, null, null);
            }
        }

        /// <summary>
        /// 列車運行状況を設定します。
        /// </summary>
        /// <param name="condition">運行状況を表す整数値</param>
        /// <param name="suspendRange">運休区間を表す文字列。存在しない場合null</param>
        /// <param name="delayText">遅延状況を表す文字列</param>
        public static TrainCondition SetCondition(int condition, string suspendRange = null, string delayText = null)
        {
            TrainConditions conditions = (TrainConditions)condition;
            LineRange range = string.IsNullOrEmpty(suspendRange) ? null : SuspendRangeSpliter(suspendRange);
            if (!string.IsNullOrEmpty(delayText) && !delayText.StartsWith("始発駅の出発が遅れます") && !delayText.StartsWith("("))
            {
                delayText = delayText.GetRangeWithEnd('(');
                (int max, int min) = DelayTimeSpliter(delayText);
                return new TrainCondition(conditions, range, max, min);
            }
            else
            {
                return new TrainCondition(conditions, range, null, null);
            }
        }

        /// <summary>
        /// 運休区間を表す文字列から区間情報を返します
        /// </summary>
        /// <example>"運休(小樽-手稲間)</example>    
        /// <param name="text">運休区間を表す文字列</param>
        /// <returns>運休区間</returns>
        private static LineRange SuspendRangeSpliter(string text)
        {
            if (text.StartsWith("運休"))
            {
                text = text.Substring(2);
                return TrainPositionSpliter.GetTrainPosition(text);
            }
            else
            {
                throw new FormatException();
            }
        }

        /// <summary>
        /// 遅延時間を表す文字列から遅延時刻の幅の最小値と最大値を返します。幅がない場合両端とも同じ値になります。
        /// </summary>
        /// <example><c>DelayTimeSpliter("約25～30分遅れ")</c></example>
        /// <param name="delayString">遅延時間を表す文字列</param>
        /// <returns></returns>
        private static (int min, int max) DelayTimeSpliter(string delayString)
        {
            if (delayString.Contains("遅れ"))
            {
                if (delayString.Contains("～"))
                {
                    string[] delayTexts = delayString.Split('～');
                    var result = delayTexts.Select(s => GetIntInString(s)).ToArray();
                    return (result[0], result[1]);
                }
                else
                {
                    int result = SplitTimeString(delayString);
                    return (result, result);
                }
            }
            else
            {
                throw new FormatException();
            }
        }

        /// <summary>
        /// 日本語の時間表現を分単位の時間に直します
        /// </summary>
        /// <param name="timeString">時間・分で表現された時刻</param>
        /// <returns></returns>
        private static int SplitTimeString(string timeString)
        {
            int hours = 0;
            int minutes = 0;

            if (timeString.Contains("約"))
                timeString = timeString.Replace("約", "");

            if (timeString.Contains("時"))
            {
                hours = GetIntInString(timeString.GetRangeWithEnd('時'));
                timeString = timeString.GetRangeWithStart('時');
            }
            if (timeString.Contains("分"))
            {
                minutes = GetIntInString(timeString.GetRangeWithEnd('分'));
                timeString = timeString.GetRangeWithStart('分');
            }

            return hours * 60 + minutes;
        }

        private static int GetIntInString(string s) => int.Parse(new string(s.Where(c => int.TryParse(c.ToString(), out int n)).ToArray()));


        /// <summary>
        /// 運行状況を表す文字列から運行状況を返します
        /// </summary>
        /// <param name="condition">運行状況を表す文字列</param>
        /// <returns></returns>
        public static TrainConditions GetTrainCondition(string condition)
        {
            switch (condition)
            {
                case "表示区間外":
                    return TrainConditions.OutsideArea;
                case "出発前":
                    return TrainConditions.NotDeparted;
                case "通常運行":
                    return TrainConditions.OnSchedule;
                case "列車遅延":
                    return TrainConditions.Delayed;
                case "部分運休":
                    return TrainConditions.PartiallySuspended;
                case "全区間運休":
                    return TrainConditions.Suspended;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
