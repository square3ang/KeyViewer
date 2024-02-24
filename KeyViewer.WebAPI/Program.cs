using Microsoft.AspNetCore.RateLimiting;
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
            builder.Services.AddRateLimiter(_ => _
            .AddSlidingWindowLimiter(policyName: "sliding", options =>
            {
                // 요청 허용 갯수 : 3
                options.PermitLimit = 3;
                // 10초 동안 최대 PermitLimit개의 요청만 처리 가능
                options.Window = TimeSpan.FromSeconds(10);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                // 제한시 5개의 요청만 대기열에 추가
                options.QueueLimit = 5;
            }));

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run("http://127.0.0.1:1111");
        }
    }
}