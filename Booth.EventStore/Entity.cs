using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("Booth.EventStore.Test")]

namespace Booth.EventStore
{
    public interface IEntity
    {
        Guid Id { get; }
    }
}
