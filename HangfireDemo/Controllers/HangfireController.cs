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
        //[ApplyStateFilter]
        [AutomaticRetry(Attempts = 1, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        [HttpGet("{datetime}")]
        public async Task TestHangfire2(DateTime dateTime)
        {
            Console.WriteLine($"Hangfire Schedule Done!! {dateTime.ToString("dd MM yyyy HH:mm")}");
            var response = await httpClient.GetAsync("https://localhost:7084/Testapi");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("call api Success!!");
            }
            else
            {
                Console.WriteLine("call api Fail!!!");
            }
        }

        [HttpGet("{datetime}/{jobId}")]
        public void EnqueueHangfire(DateTime dateTime, string jobId)
        {
            backgroundJobClient.Enqueue(() => TestHangfire2(dateTime));
            backgroundJobClient.Delete(jobId);
            Console.WriteLine("Hangfire Enqueue Job Now!!");
        }
        [HttpGet]
        public void Enqueue()
        {
            var jobId = backgroundJobClient.Enqueue(() => TestHangfire2(new DateTime()));
            //backgroundJobClient.Delete(jobId);
            Console.WriteLine($"Hangfire Enqueue Job Now!! {jobId}");
        }

        [HttpGet("{jobId}")]
        public void Continue(string jobId)
        {
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine($"Continuation!! {jobId}"));
        }

        [HttpGet("{jobId}")]
        public void Requeue(string jobId)
        {
            BackgroundJob.Requeue(jobId);
        }

    }
}
