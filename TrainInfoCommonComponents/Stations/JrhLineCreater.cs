using System;

namespace TrainInfo.Stations
{
    /// <summary>
    /// <see cref="JrhLine"/>の作成を提供します。
    /// </summary>
    public class JrhLineCreater
    {
        /// <summary>
        /// 路線名を表す文字列から <see cref="JrhLine"/> を返します。
        /// </summary>
        /// <param name="text">路線名。</param>
        /// <returns></returns>
        public static JrhLine FromString(string text)
        {
            switch (text)
            {
                case "函館本線岩見沢方面":
                    return JrhLine.Hakodate_Iwamizawa;
                case "函館本線小樽方面":
                    return JrhLine.Hakodate_Otaru;
                case "千歳線":
                case "千歳線苫小牧方面":
                    return JrhLine.Chitose_Tomakomai;
                case "学園都市線":
                    return JrhLine.Sassyo;
                case "石勝線":
                    return JrhLine.Sekisyo;
                case "根室本線富良野方面":
                    return JrhLine.Nemuro_Furano;
                case "根室本線帯広方面":
                    return JrhLine.Nemuro_Obihiro;
                case "室蘭本線":
                    return JrhLine.Muroran;
                default:
                    throw new NotSupportedException("路線が見つかりません");
            }
        }
    }
}
