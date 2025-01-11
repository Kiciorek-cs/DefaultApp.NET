using System;
using Demo.BLL.Interfaces.Services.Clock;

namespace Demo.BLL.Services.Clock;

public sealed class UtcClock : IClock
{
    public DateTimeOffset Current()
    {
        return DateTimeOffset.Now;
    }
}