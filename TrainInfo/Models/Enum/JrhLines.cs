using System;
using System.Collections.Generic;
using System.Text;

namespace TrainInfo.Models
{
    /// <summary>
    /// JR北海道の路線を表します。
    /// </summary>
    public enum JrhLine
    {
        /// <summary>
        /// 函館線　札幌-岩見沢間。
        /// </summary>
        Hakodate_Iwamizawa,

        /// <summary>
        /// 函館線　札幌-小樽間。
        /// </summary>
        Hakodate_Otaru,

        /// <summary>
        /// 千歳線　札幌-苫小牧間。
        /// </summary>
        Chitose_Tomakomai,

        /// <summary>
        /// 札沼線。
        /// </summary>
        Sassyo,

        /// <summary>
        /// 石勝線。
        /// </summary>
        Sekisyo,

        /// <summary>
        /// 石勝線 上り
        /// </summary>
        Sekisyo_Nobiri,

        /// <summary>
        /// 石勝線 下り
        /// </summary>
        Sekisyo_Kudari,

        /// <summary>
        /// 室蘭線。
        /// </summary>
        Muroran,

        /// <summary>
        /// 根室本線。
        /// </summary>
        Nemuro,

        /// <summary>
        /// 花咲線。
        /// </summary>
        Hanasaki,

        /// <summary>
        /// 釧網本線。
        /// </summary>
        Senmo,

        /// <summary>
        /// 石北本線。
        /// </summary>
        Sekihoku,

        /// <summary>
        /// 富良野線。
        /// </summary>
        Hurano
    }
}
