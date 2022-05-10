using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    public class HangfireController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly IBackgroundJobClient backgroundJobClient;
        public HangfireController(IBackgroundJobClient backgroundJobClient, IHttpClientFactory httpClientFactory)
        {
            this.backgroundJobClient=backgroundJobClient;
            this.httpClient=httpClientFactory.CreateClient("ApiService");
        }
        [HttpGet("{datetime}")]
        public void ScheduleHangfire(DateTime dateTime)
        {
            DateTimeOffset dateTimeOffset = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            var jobId = backgroundJobClient.Schedule(() => TestHangfire2(dateTime), dateTimeOffset);
            Console.WriteLine("Hangfire Schedule Begin!!");
            //RecurringJob.AddOrUpdate(() => TestHangfire2(), "*/15 * * * *");
            //RecurringJob.AddOrUpdate("myrecurringjob", () => Console.WriteLine("Recurring!"), Cron.Daily);
        }

        [HttpGet("{datetime}")]
        public async Task TestHangfire2(DateTime dateTime)
        {
            await httpClient.GetAsync("https://localhost:7084/Testapi");
            Console.WriteLine($"Hangfire Schedule Done!! {dateTime.ToString("dd MM yyyy HH:mm")}");
        }

        [HttpGet("{datetime}/{jobId}")]
        public void EnqueueHangfire(DateTime dateTime, string jobId)
        {
            backgroundJobClient.Enqueue(() => TestHangfire2(dateTime));
            backgroundJobClient.Delete(jobId);
            Console.WriteLine("Hangfire Enqueue Job Now!!");
        }

    }
}
