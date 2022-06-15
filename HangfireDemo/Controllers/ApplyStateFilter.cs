using Hangfire.States;
using Hangfire.Storage;

namespace HangfireDemo.Controllers
{
    //[AttributeUsage(AttributeTargets.All)]

    public class ApplyStateFilter : IApplyStateFilter
    {
        public ApplyStateFilter()
        {
        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            var deletedState = context.NewState as DeletedState;
            var failedState = context.NewState as FailedState;
            var methodName = context.Job.Method.Name;
            if (deletedState != null)
            {
                Console.WriteLine("---------- DeletedState --> Hangfire Retrie Last Attamp ----------");
                SendEmail();
                // Execute code when the job has failed on all its attempts
            }

            if (failedState != null)
            {
                Console.WriteLine("---------- FailedState --> Hangfire Retrie Last Attamp ----------");
                SendEmail();
                // Execute code when the job has failed on all its attempts
            }
        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
        }


        public async void SendEmail()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync("https://localhost:7240/SendEmail");
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("---------- Send Email Success!! ----------");
                    }
                    else
                    {
                        Console.WriteLine("----------  Send Email Fail!!! ----------");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("---------- Send Email Error!!! ----------");
                }
            }
        }
    }
}
