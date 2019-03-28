using System;
using System.Collections.Generic;
using System.Text;
using TrainInfo.Models;

namespace TrainInfo.Spliters
{
    class TrainTypeSpliter
    {
        /// <summary>
        /// 列車種別を表す文字列から列車種別を返します
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static TrainTypes GetTrainTypes(string text)
        {
            switch (text)
            {
                case "普通":
                    return TrainTypes.Local;
                case "区快":
                    return TrainTypes.Semi_Rapid;
                case "区間快速":
                    return TrainTypes.Semi_Rapid;
                case "快速":
                    return TrainTypes.Rapid;
                case "特急":
                    return TrainTypes.Ltd_Exp;
                default:
                    return TrainTypes.Other;
            }
        }

        /// <summary>
        /// 列車種別を表すURLから列車種別を返します
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static TrainTypes GetTrainTypesFromImgTag(string text)
        {
            switch (text)
            {
                case @"img/common/icon-train-futsu.png":
                    return TrainTypes.Local;
                case @"img/common/icon-train-kukai.png":
                    return TrainTypes.Semi_Rapid;
                case @"img/common/icon-train-kaisoku.png":
                    return TrainTypes.Rapid;
                case @"img/common/icon-train-tokkyu.png":
                    return TrainTypes.Ltd_Exp;
                default:
                    return TrainTypes.Other;
            }
        }
    }
}
