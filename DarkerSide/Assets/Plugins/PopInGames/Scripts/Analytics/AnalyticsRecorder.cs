using System;
using System.Collections;
using System.Linq;

namespace PopInGames
{
    public class AnalyticsRecorder
    {
        private AnalyticsReport report = new AnalyticsReport();

        public AnalyticsRecorder()
        {
            this.Reset();
        }

        public bool IsEmpty => this.report.records.Count == 0;

        public IEnumerator SaveAndReset_Coroutine(ApiClient client)
        {
            yield return this.Save_Coroutine(client);
            this.Reset();
        }

        public IEnumerator Save_Coroutine(ApiClient client)
        {
            this.report.time.end = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            foreach (var record in this.report.records)
            {
                record.distance.average = record.distance.Values.Average();
                record.distance.median = record.distance.Values.ToArray().Median();
                record.distance.mode = record.distance.Values.GroupBy(v => v)
                    .OrderByDescending(g => g.Count())
                    .First()
                    .Key;
                record.distance.range.min = record.distance.Values.Min();
                record.distance.range.max = record.distance.Values.Max();
            }
            yield return client.GetIPAddress(null, null);
            yield return client.StoreAnalytics_Coroutine(this.report);
        }

        public void Reset()
        {
            this.report.time.start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.report.time.end = 0;
            this.report.records.Clear();
        }

        public void AddTime(AdMesh adMesh, float seconds, float seconds100, float distance)
        {
            var record = this.report.records.FirstOrDefault(r => r.ad.banner_id == adMesh.BannerData.banner_id);

            if (record == null)
            {
                record = new AnalyticsReport.Record();
                record.ad = new AnalyticsReport.Ad
                {
                    banner_id = adMesh.BannerData.banner_id,
                    campaign_id=adMesh.BannerData.campaign_id,
                    type = adMesh.Type
                };
                this.report.records.Add(record);
            }

            record.visible.total += seconds;
            record.visible.completely += seconds100;
            record.visible.partially += seconds - seconds100;
            record.distance.Values.Add(distance);
        }
    }
}