using System;
using System.Collections.Generic;

namespace PopInGames
{
    public enum AdType
    {
        LongHorizontal,
        ShortHorizontal,
        LongVertical,
        ShortVertical,
        PortraitPoster,
        LandscapePoster,
        Logo,
        None
    }

    public static class Conversions
    {
        public static string AdTypeToCode(AdType type)
        {
            switch (type)
            {
                case AdType.LongHorizontal:
                    return "LH";
                case AdType.ShortHorizontal:
                    return "SH";
                case AdType.LongVertical:
                    return "LV";
                case AdType.ShortVertical:
                    return "SV";
                case AdType.PortraitPoster:
                    return "PP";
                case AdType.LandscapePoster:
                    return "LP";
                case AdType.Logo:
                    return "LG";
                default:
                    return null;
            }
        }

        public static float AdTypeAspectRatio(AdType type)
        {
            switch (type)
            {
                case AdType.LongHorizontal:
                    return 3.75f;
                case AdType.ShortHorizontal:
                    return 1.875f;
                case AdType.LongVertical:
                    return 0.27f;
                case AdType.ShortVertical:
                    return 0.53f;
                case AdType.PortraitPoster:
                    return 0.6f;
                case AdType.LandscapePoster:
                    return 1.66f;
                case AdType.Logo:
                    return 1;
                default:
                    return 1;
            }
        }

    }
}