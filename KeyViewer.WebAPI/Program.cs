using KeyViewer.WebAPI.Core.Utils;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace KeyViewer.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddMvc(options =>
            {
                options.InputFormatters.Insert(0, new BinaryInputFormatter());
            });

            builder.Services.AddRateLimiter(limiterOptions =>
            {
                limiterOptions.OnRejected = (ctx, cTok) =>
                {
                    if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        ctx.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }
                    ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    return ValueTask.CompletedTask;
                };
                // 글로벌 속도 제한 설정
                limiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                {
                    // 요청 IPAddress
                    IPAddress? remoteIpAddress = IPAddress.Parse(context.GetIpAddress() ?? "127.0.0.1");

                    // 요청된 IPAddress가 루프백이 아닌 경우
                    if (IPAddress.IsLoopback(remoteIpAddress!) == false)
                    {
                        // IPAddress에 대해 속도 제한 설정
                        return RateLimitPartition.GetSlidingWindowLimiter
                        (remoteIpAddress!, _ =>
                        new SlidingWindowRateLimiterOptions
                        {
                            // 요청 허용 갯수 : 100
                            PermitLimit = 3,
                            // 창 이동시간 30초
                            Window = TimeSpan.FromSeconds(5),
                            // 창 분할 세그먼트 갯수
                            SegmentsPerWindow = 3,  // 1개의 세그먼트 : 5s / 3
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            // 제한시 3개의 요청만 대기열에 추가
                            QueueLimit = 3,
                        });
                    }

                    // 루프백 IPAddress는 속도 제한 없음.
                    return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();
            // 레이트 리미터 사용
            app.UseRateLimiter();

            app.MapControllers();

            app.MapFallback("{*path}", Fallback);

            app.Run("http://127.0.0.1:1111");
        }
        public static void Fallback(HttpContext context)
        {
            //var isBot = IsBot(context.Request.Headers["User-Agent"]);
            context.Response.Redirect("https://5hanayome.adofai.dev", false, true);
        }
        public static bool IsBot(string? userAgent)
        {
            if (userAgent == null) return false;
            return userAgent.Contains("kakaotalk", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("scrap", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("discord", StringComparison.OrdinalIgnoreCase);
        }
    }
}