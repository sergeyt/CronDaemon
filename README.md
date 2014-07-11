[![Build Status](https://drone.io/github.com/sergeyt/CronDaemon/status.png)](https://drone.io/github.com/sergeyt/CronDaemon/latest)
[![Build status](https://ci.appveyor.com/api/projects/status/n5yevt7shkvtej9q)](https://ci.appveyor.com/project/sergeyt/crondaemon)
[![NuGet version](https://badge.fury.io/nu/CronDaemon.png)](http://badge.fury.io/nu/CronDaemon)

# CronDaemon

.NET library with single CronDaemon class
with generic implementation of cron scheduling
for .NET based on [ncrontab](http://ncrontab.googlecode.com/).

## Sample code

```c#
var crond = new CronDaemon<string>(
  value => {
    Console.WriteLine(value);
  });

crond.Add("Print hi hourly", Cron.Hourly());
crond.Add("Print hi daily 5 times", Cron.Daily(), 5);
```
