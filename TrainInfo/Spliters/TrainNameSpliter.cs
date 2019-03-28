using System;
using System.Collections.Generic;
using System.Text;
using TrainInfo.Models;
using System.Linq;

namespace TrainInfo.Spliters
{
    /// <summary>
    /// 列車名を表す文字列の解析を提供します
    /// </summary>
    public static class TrainNameSpliter
    {
        /// <summary>
        /// 列車名を表す文字列から列車情報を設定します。
        /// </summary>
        /// <param name="trainNameString">列車名を表す文字列</param>
        /// <param name="trainTypeString">列車種別を表す文字列</param>
        public static TrainName SetTrainName(string trainNameString, string trainTypeString)
        {
            TrainTypes? subType = null;
            LineRange range = null;

            if (trainNameString.Contains("（"))
            {
                string rangetext = trainNameString.GetRange('（', '）');//全角括弧

                if (rangetext.Contains("間"))//いしかりライナー及びAP普通用
                {
                    subType = TrainTypeSpliter.GetTrainTypes(rangetext.GetRangeWithStart('間'));
                    var stations = rangetext.GetRangeWithEnd('間').Split('～');
                    range = new LineRange(stations[0], stations[1]);
                }
                else//すずらん室蘭行用
                {
                    subType = TrainTypes.Local;
                    range = new LineRange("東室蘭", "室蘭");
                }
                trainNameString = trainNameString.GetRangeWithEnd('（');
            }

            string name = SplitTrainNumber(trainNameString, out int? number);
            TrainTypes mainTrainType = TrainTypeSpliter.GetTrainTypes(trainTypeString);

            return new TrainName(mainTrainType, name, number, subType, range);
        }

        /// <summary>
        /// 号数を含む可能性のある文字列を列車名と号数に分離します
        /// </summary>
        /// <param name="nameString">列車名</param>
        /// <param name="number">号数。号数を含まない場合<c>null</c></param>
        /// <returns></returns>
        private static string SplitTrainNumber(string nameString, out int? number)
        {
            if (nameString.EndsWith("号"))
            {
                string editedNameString = nameString.Substring(0, nameString.Length - 1).ToHankakuNum();
                string numberString = new string(editedNameString.Where(c => int.TryParse(c.ToString(), out int n)).ToArray());

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
}
