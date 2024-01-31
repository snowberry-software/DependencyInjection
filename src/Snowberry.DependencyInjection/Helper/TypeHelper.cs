using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snowberry.DependencyInjection.Helper;

/// <summary>
/// Extension methods for instances and types.
/// </summary>
internal static class TypeHelper
{
    public static bool IsDisposable(this object? instance)
    {
        if (instance == null)
            return false;

        if (instance is IDisposable)
            return true;

#if NETCOREAPP
        if (instance is IAsyncDisposable)
            return true;
#endif

        return false;
    }
}
