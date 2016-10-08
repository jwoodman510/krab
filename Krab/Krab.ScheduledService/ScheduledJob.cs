using System;
using NCron;

namespace Krab.ScheduledService
{
    public interface IScheduledJob : ICronJob
    {
        
    }

    public class ScheduledJob : CronJob, IScheduledJob
    {
        public override void Execute()
        {
            Console.WriteLine($"{DateTime.Now}: I'm an executing scheduled job!");
        }
    }
}
