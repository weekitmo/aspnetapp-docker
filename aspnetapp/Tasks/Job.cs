
using Quartz;
using Quartz.Impl;

namespace Aspnetapp.Tasks
{
  internal sealed class LogJob : IJob
  {
    public async Task Execute(IJobExecutionContext context)
    {
      await Console.Out.WriteLineAsync($"{DateTime.Now}:Hello quartz world!");
    }
  }
  internal class SchedulerTask
  {
    public async Task DayTask()
    {
      //创建一个工作
      IJobDetail job = JobBuilder.Create<LogJob>()
       .WithIdentity("DayTask", "job-group")
      .Build();

      var trigger = TriggerBuilder.Create()
      .WithIdentity("DayTaskTrigger", "trigger-group")
      .ForJob("DayTask", "job-group")
      // second minute hour (day of month) month (day of week) year?
      .WithCronSchedule("0 30 14 * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")))
      .Build();

      StdSchedulerFactory factory = new StdSchedulerFactory();

      IScheduler scheduler = await factory.GetScheduler();
      Console.WriteLine($"DayTask starting... {TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai")}");
      await scheduler.Start();
      await scheduler.ScheduleJob(job, trigger);
    }
  }
}