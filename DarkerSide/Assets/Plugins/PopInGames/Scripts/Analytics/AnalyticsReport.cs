using System;
using System.Collections.Generic;

namespace PopInGames
{
    [Serializable]
    public class AnalyticsReport
    {
        public TimeRange time;
        public List<Record> records = new List<Record>();

        [Serializable]
        public struct TimeRange
        {
            public long start;
            public long end;
        }

        [Serializable]
        public class Record
        {
            public Ad ad;
            public Visible visible = new Visible();
            public Distance distance = new Distance();
        }

        [Serializable]
        public struct Ad
        {
            public string banner_id;
            public string campaign_id;
            public AdType type;
        }

        [Serializable]
        public struct Visible
        {
            public float partially;
            public float completely;
            public float total;
        }

        [Serializable]
        public class Distance
        {
            public Range range;
            public float average;
            public float median;
            public float mode;

            [NonSerialized]
            public List<float> Values = new List<float>();
        }

        [Serializable]
        public struct Range
        {
            public float min;
            public float max;
        }
    }
}