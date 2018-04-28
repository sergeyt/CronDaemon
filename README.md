[![Build Status](https://drone.io/github.com/sergeyt/CronDaemon/status.png)](https://drone.io/github.com/sergeyt/CronDaemon/latest)
[![Build status](https://ci.appveyor.com/api/projects/status/n5yevt7shkvtej9q)](https://ci.appveyor.com/project/sergeyt/crondaemon)
[![NuGet Version](http://img.shields.io/nuget/v/CronDaemon.svg?style=flat)](https://www.nuget.org/packages/CronDaemon/)
[![NuGet Downloads](http://img.shields.io/nuget/dt/CronDaemon.svg?style=flat)](https://www.nuget.org/packages/CronDaemon/)

# CronDaemon

Small .NET package with simple generic implementation of cron scheduling based
on [ncrontab](https://github.com/atifaziz/NCrontab)
and [System.Threading.Timer](http://msdn.microsoft.com/en-us/library/system.threading.timer(v=vs.110).aspx).

## Sample code

```c#
var crond = CronDaemon.Start<string>(
  value => {
    Console.WriteLine(value);
  });

crond.Add("Print hi hourly", Cron.Hourly());
crond.Add("Print hi daily 5 times", Cron.Daily(), 5);
crond.Add("Print hi at 9AM UTC daily.  The cron expression is always evaluated in UTC", "0 9 * * *")
```
