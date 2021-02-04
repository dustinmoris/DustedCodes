namespace DustedCodes

[<RequireQualifiedAccess>]
module GoogleAnalytics =
    open System
    open System.Diagnostics
    open System.Threading.Tasks
    open Google.Apis.Auth.OAuth2
    open Google.Apis.AnalyticsReporting.v4
    open Google.Apis.Services
    open Google.Apis.AnalyticsReporting.v4.Data
    open FSharp.Control.Tasks.NonAffine

    type PageStatistic =
        {
            Path      : string
            ViewCount : int64
        }

    type GetReportFunc = string -> int -> Task<PageStatistic list>

    let getMostViewedPagesAsync (analyticsKey : string) =

        let credential =
            GoogleCredential
                .FromJson(analyticsKey)
                .CreateScoped(AnalyticsReportingService.Scope.AnalyticsReadonly)

        fun (viewId : string) (maxCount : int) ->
            task {
                let timer = Stopwatch.StartNew()
                use service =
                    new AnalyticsReportingService(
                        BaseClientService.Initializer(
                            ApplicationName       = "Dusted Codes Website",
                            HttpClientInitializer = credential))

                Log.debugF "Created AnalyticsReportingService in %s" (timer.Elapsed.ToMs())

                let reportRequest =
                    ReportRequest(
                        ViewId     = viewId,
                        DateRanges = [|
                            DateRange(
                                StartDate = DateTime(2015, 02, 16).ToString("yyyy-MM-dd"),
                                EndDate   = DateTime.UtcNow.ToString("yyyy-MM-dd")) |],
                        Metrics    = [| Metric (Expression = "ga:pageviews") |],
                        Dimensions = [| Dimension(Name = "ga:pagePath") |],
                        OrderBys   = [| OrderBy(FieldName = "ga:pageviews", SortOrder = "DESCENDING") |]
                    )

                let request =
                    service.Reports.BatchGet(
                        GetReportsRequest(
                            ReportRequests = [| reportRequest |]))

                let! response = request.ExecuteAsync()

                timer.Stop()
                Log.debugF "Retrieved Google Analytics report in %s" (timer.Elapsed.ToMs())

                let report    = response.Reports.[0]
                let maxRows   = min report.Data.Rows.Count maxCount

                return
                    report.Data.Rows
                    |> Seq.take maxRows
                    |> Seq.map (fun r -> { Path      = r.Dimensions.[0];
                                           ViewCount = int64 r.Metrics.[0].Values.[0] })
                    |> Seq.toList
            }
