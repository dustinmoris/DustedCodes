using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace DustedCodes.Core.Analytics
{
    public class GoogleAnalyticsClient : IGoogleAnalyticsClient
    {
        private readonly string _privateKeyPath;
        private readonly string _viewId;

        public GoogleAnalyticsClient(string privateKeyPath, string viewId)
        {
            _privateKeyPath = privateKeyPath;
            _viewId = viewId;
        }

        public async Task<IEnumerable<PageResult>> GetTrendingPagesAsync(byte maxCount)
        {
            const string applicationName = "Dusted Codes Website";
            
            var blogLaunchDate = new DateTime(2015, 02, 16).ToString("yyyy-MM-dd");
            var endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var credential = GoogleCredential.FromStream(
                new FileStream(
                    _privateKeyPath,
                    FileMode.Open,
                    FileAccess.Read))
                .CreateScoped(AnalyticsReportingService.Scope.AnalyticsReadonly);

            using (var service =
                new AnalyticsReportingService(
                    new BaseClientService.Initializer
                    {
                        ApplicationName = applicationName,
                        HttpClientInitializer = credential
                    }))
            {

                var request = service.Reports.BatchGet(
                    new GetReportsRequest
                    {
                        ReportRequests = new List<ReportRequest>
                        {
                            new ReportRequest
                            {
                                ViewId = _viewId,
                                DateRanges = new List<DateRange>
                                {
                                    new DateRange
                                    {
                                        StartDate = blogLaunchDate,
                                        EndDate = endDate
                                    }
                                },
                                Metrics = new List<Metric>
                                {
                                    new Metric
                                    {
                                        Expression = "ga:pageviews"
                                    }
                                },
                                Dimensions = new List<Dimension>
                                {
                                    new Dimension
                                    {
                                        Name = "ga:pagePath"
                                    }
                                },
                                OrderBys = new List<OrderBy>
                                {
                                    new OrderBy
                                    {
                                        FieldName = "ga:pageviews",
                                        SortOrder = "DESCENDING"
                                    }
                                }
                            }
                        }
                    });

                var response = await request.ExecuteAsync().ConfigureAwait(false);
                var trendingPages = new List<PageResult>();

                for (var i = 0; i < maxCount; i++)
                {
                    var row = response.Reports[0].Data.Rows[i];
                    var path = row.Dimensions[0];
                    var count = row.Metrics[0].Values[0];
                    trendingPages.Add(new PageResult { Path = path, ViewCount = count });
                }

                return trendingPages;
            }
        }
    }
}
