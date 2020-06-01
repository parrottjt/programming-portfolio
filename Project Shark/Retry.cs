using System.Collections.Generic;
using System;
using System.Threading;

public static class Retry
{
    public static void Do(
        Action action,
        int milliSeconds,
        int maxAttemptCount = 3)
    {
        Do<object>(() =>
        {
            action();
            return null;
        }, milliSeconds, maxAttemptCount);
    }

    public static T Do<T>(
        Func<T> action,
        int milliSeconds,
        int maxAttemptCount = 3)
    {
        var exceptions = new List<Exception>();

        for (int attempted = 0; attempted < maxAttemptCount; attempted++)
        {
            try
            {
                if (attempted > 0)
                {
                    Thread.Sleep(milliSeconds);
                }

                return action();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }
        throw new AggregateException(exceptions);
    }
}