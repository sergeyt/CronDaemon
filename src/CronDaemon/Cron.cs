using System;
using NCrontab;

namespace CronScheduling
{
	/// <summary>
	/// Typed wrapper for Cron expressions.
	/// </summary>
	public struct CronExpression
	{
		public readonly string Expression;

		public CronExpression(string expression)
		{
			Expression = expression;
		}

		public string ToHumanString()
		{
			return CrontabSchedule.Parse(Expression).ToString();
		}

		public static implicit operator CronExpression(string expression)
		{
			return new CronExpression(expression);
		}

		public static implicit operator string(CronExpression expression)
		{
			return expression.Expression;
		}
	}

	/// <summary>
	/// Cron expression builder.
	/// </summary>
	public static class Cron
	{
		/// <summary>
		/// Every minute.
		/// </summary>
		public static CronExpression Minutely()
		{
			return "* * * * *";
		}

		/// <summary>
		/// Every hour at the specified minute.
		/// </summary>
		public static CronExpression Hourly(int minute = 0)
		{
			return String.Format("{0} * * * *", minute);
		}

        /// <summary>
        /// Every day at the specified hour and minute in UTC.
        /// </summary>
        public static CronExpression Daily(int hour = 0, int minute = 0)
		{
			return String.Format("{0} {1} * * *", minute, hour);
		}

		/// <summary>
		/// Every week at the specified day of week, hour and minute in UTC.
		/// </summary>
		public static CronExpression Weekly(DayOfWeek dayOfWeek = DayOfWeek.Monday, int hour = 0, int minute = 0)
		{
			return String.Format("{0} {1} * * {2}", minute, hour, (int)dayOfWeek);
		}

		/// <summary>
		/// Every month at the specified day of month, hour and minute in UTC.
		/// </summary>
		public static CronExpression Monthly(int day = 1, int hour = 0, int minute = 0)
		{
			return String.Format("{0} {1} {2} * *", minute, hour, day);
		}

		/// <summary>
		/// Every year at the specified month, day, hour and minute in UTC.
		/// </summary>
		public static CronExpression Yearly(int month = 1, int day = 1, int hour = 0, int minute = 0)
		{
			return String.Format("{0} {1} {2} {3} *", minute, hour, day, month);
		}
	}
}
