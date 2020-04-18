using System;
using TrainInfo.ExtensionMethods;

namespace TrainInfo.Stations
{
    /// <summary>
    /// <see cref="JrhDestType"/>の作成を提供します。
    /// </summary>
    public class JrhDestTypeCreater
    {
        /// <summary>
        /// 行先の方面を表す文字列から <see cref="JrhDestType"/> を取得します。
        /// </summary>
        /// <param name="destTypeString">行先の方面を表す文字列。</param>
        /// <param name="lineTypeString">路線名を表す文字列。</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">未対応の行先のときにスローされる例外。</exception>
        /// <exception cref="ArgumentException">引数の組が正しくないときにスローされる例外。</exception>
        public static JrhDestType FromString(string destTypeString, string lineTypeString)
        {
            switch (destTypeString)
            {
                case "札幌方面":
                    switch (lineTypeString)
                    {
                        case "函館線":
                        case "函館本線":
                            return JrhDestType.Sapporo_Hakodate;
                        case "千歳線":
                            return JrhDestType.Sapporo_Chitose;
                        case "学園都市線":
                            return JrhDestType.Sapporo_Sassyo;
                        case "函館線・千歳線":
                        case "函館本線・千歳線":
                            return JrhDestType.Sapporo_Hakodate_Chitose;
                        case "函館線・学園都市線":
                        case "函館本線・学園都市線":
                            return JrhDestType.Sapporo_Hakodate_Sassyo;
                        default:
                            throw new ArgumentException("引数の組が不正です。この行先は指定の路線に存在しません。");
                    }
                case "小樽方面":
                    return JrhDestType.Hakodate_Otaru;
                case "岩見沢方面":
                    if (lineTypeString == "室蘭本線")
                    {
                        return JrhDestType.Muroran_Iwamizawa;
                    }
                    else
                    {
                        return JrhDestType.Hakodate_Iwamizawa;
                    }

                case "余市・倶知安方面":
                    return JrhDestType.Hakodate_Yoichi;
                case "滝川・旭川方面":
                    return JrhDestType.Hakodate_Takikawa;
                case "千歳方面":
                case "（普通・快速）千歳方面":
                    return JrhDestType.Chitose_Chitose;
                case "新千歳空港":
                    return JrhDestType.Chitose_AP;
                case "（快速）新千歳空港":
                    return JrhDestType.Chitose_Rapid_AP;
                case "（普通・快速）札幌方面":
                    return JrhDestType.Chitose_LocalRapid_Sapporo;
                case "（特急）札幌":
                case "（特急）札幌方面":
                    return JrhDestType.Chitose_LimExp_Sapporo;
                case "苫小牧方面":
                    if (lineTypeString == "室蘭本線")
                    {
                        return JrhDestType.Muroran_Tomakomai;
                    }
                    else
                    {
                        return JrhDestType.chitose_Tomakomai;
                    }
                case "（特急）帯広・釧路方面":
                    return JrhDestType.Chitose_LimExp_Chitose_Sekisyo;

                case "（特急）室蘭・函館方面":
                case "苫小牧・東室蘭・函館方面":
                    return JrhDestType.chitose_Tomakomai;
                case "室蘭・函館方面":
                    return JrhDestType.Chitose_Muroran;
                case "石狩当別方面":
                    return JrhDestType.Sassyo_IshikariTobetsu;
                case "浦臼・新十津川方面":
                    return JrhDestType.Sassyo_Urausu;
                case "追分・苫小牧方面":
                case "追分・岩見沢方面":
                    return JrhDestType.Muroran_Oiwake;
                case "追分方面":
                case "追分・帯広・釧路方面":
                    return JrhDestType.Sekisyo_Oiwake_Obihiro;
                case "追分・新千歳空港・札幌方面":
                    return JrhDestType.Sekisyo_Oiwake_Sapporo;
                case "南千歳・札幌方面":
                    return JrhDestType.Sekisyo_MinamiChitose_Sapporo;
                case "新夕張・帯広方面":
                case "新夕張方面":
                case "夕張・トマム・帯広・釧路方面":
                case "トマム・帯広・釧路方面":
                    return JrhDestType.Sekisyo_Shiyubari;
                case "夕張":
                case "夕張方面":
                    return JrhDestType.Sekisyo_Yubari;
                case "帯広・釧路方面":
                    return JrhDestType.Sekisyo_Obihiro;
                case "滝川":
                case "滝川方面":
                    return JrhDestType.Nemuro_Takikawa;
                case "富良野方面":
                    return JrhDestType.Nemuro_Hurano;
                case "帯広方面":
                    return JrhDestType.Nemuro_Obihiro;
                case "新得方面":
                    return JrhDestType.Nemuro_Shintoku;
                case "釧路方面":
                    if (lineTypeString == "根室本線")
                    {
                        return JrhDestType.Nemuro_Kusiro;
                    }
                    else if (lineTypeString == "花咲線")
                    {
                        return JrhDestType.Hanasaki_Kusiro;
                    }
                    else
                    {
                        return JrhDestType.Senmo_Kusiro;
                    }

                case "根室方面":
                    return JrhDestType.Hanasaki_Nemuro;
                case "網走方面":
                    return JrhDestType.Senmo_Abashiri;
                case "旭川方面":
                    if (lineTypeString == "石北本線")
                    {
                        return JrhDestType.Sekihoku_Asahikawa;
                    }
                    else
                    {
                        return JrhDestType.Hurano_Asahikawa;
                    }

                default:
                    throw new NotSupportedException("路線が存在しません");
            }
        }

        /// <summary>
        /// 行先の方面を取得します。
        /// </summary>
        /// <param name="text">行先の方面を表す文字列。</param>
        /// <example>函館本線[小樽方面へ]</example>
        /// <returns></returns>
        /// <exception cref="FormatException">引数が解釈できない形式の場合にスローされる例外。</exception>
        public static JrhDestType FromParseText(string text)
        {
            var lineTypeString = text.GetRangeWithEnd('[');
            var destTypeString = text.GetRange('[', ']');
            if (destTypeString.Contains("へ"))
            {
                destTypeString = destTypeString.Substring(0, destTypeString.Length - 1);
            }
            else if (destTypeString.Contains("から"))
            {
                destTypeString = destTypeString.Substring(0, destTypeString.Length - 2);
            }
            else
            {
                throw new FormatException("行先の文字を認識できません。");
            }
            return FromString(destTypeString, lineTypeString);
        }
    }
}
