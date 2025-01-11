using System;

namespace Demo.BLL.Interfaces.Services.Clock;

public interface IClock
{
    DateTimeOffset Current();
}