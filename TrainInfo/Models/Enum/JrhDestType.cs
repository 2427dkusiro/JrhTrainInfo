using System;
using System.Collections.Generic;
using System.Text;

namespace TrainInfo.Models
{
    /// <summary>
    /// 行先の方面を表します
    /// </summary>
    public enum JrhDestType
    {
        /// <summary>
        /// 函館本線札幌方面
        /// </summary>
        Sapporo_Hakodate,

        /// <summary>
        /// 千歳線札幌方面
        /// </summary>
        Sapporo_Chitose,

        /// <summary>
        /// 学園都市線札幌方面
        /// </summary>
        Sapporo_Sassyo,

        /// <summary>
        /// 函館本線小樽方面
        /// </summary>
        Hakodate_Otaru,

        /// <summary>
        /// 函館本線岩見沢方面
        /// </summary>
        Hakodate_Iwamizawa,

        /// <summary>
        /// 函館本線余市方面
        /// </summary>
        Hakodate_Yoichi,

        /// <summary>
        /// 函館本線滝川方面
        /// </summary>
        Hakodate_Takikawa,

        /// <summary>
        /// 千歳線千歳方面
        /// </summary>
        Chitose_Chitose,

        /// <summary>
        /// 千歳線新千歳空港行
        /// </summary>
        Chitose_AP,

        /// <summary>
        /// 千歳線新千歳空港行快速列車
        /// </summary>
        Chitose_Rapid_AP,

        /// <summary>
        /// 千歳線札幌方面普通・快速列車
        /// </summary>
        Chitose_LocalRapid_Sapporo,

        /// <summary>
        /// 千歳線札幌方面特急列車
        /// </summary>
        Chitose_LimExp_Sapporo,

        /// <summary>
        /// 千歳線苫小牧方面
        /// </summary>
        chitose_Tomakomai,

        /// <summary>
        /// 千歳線室蘭方面
        /// </summary>
        Chitose_Muroran,

        /// <summary>
        /// 札沼線石狩当別方面
        /// </summary>
        Sassyo_IshikariTobetsu,

        /// <summary>
        /// 札沼線浦臼方面
        /// </summary>
        Sassyo_Urausu,

        /// <summary>
        /// 室蘭線追分方面
        /// </summary>
        Muroran_Oiwake,

        /// <summary>
        /// 室蘭線苫小牧方面
        /// </summary>
        Muroran_Tomakomai,

        /// <summary>
        /// 室蘭線岩見沢方面
        /// </summary>
        Muroran_Iwamizawa,

        /// <summary>
        /// 石勝線追分・帯広方面
        /// </summary>
        Sekisyo_Oiwake_Obihiro,

        /// <summary>
        /// 石勝線追分・札幌方面
        /// </summary>
        Sekisyo_Oiwake_Sapporo,

        /// <summary>
        /// 石勝線南千歳・札幌方面
        /// </summary>
        Sekisyo_MinamiChitose_Sapporo,

        /// <summary>
        /// 石勝線新夕張方面
        /// </summary>
        Sekisyo_Shiyubari,

        /// <summary>
        /// 石勝線夕張方面
        /// </summary>
        Sekisyo_Yubari,

        /// <summary>
        /// 石勝線帯広方面
        /// </summary>
        Sekisyo_Obihiro,

        Nemuro_Takikawa,

        Nemuro_Hurano,

        Nemuro_Obihiro,

        Nemuro_Shintoku,

        Nemuro_Kusiro,

        Hanasaki_Nemuro,

        Hanasaki_Kusiro,

        Senmo_Abashiri,

        Senmo_Kusiro,

        Sekihoku_Asahikawa,

        Hurano_Asahikawa,
    }
}
