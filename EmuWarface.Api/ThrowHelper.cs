using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EmuWarface.Api;

public static class ThrowHelper
{
    [StackTraceHidden]
    static void ThrowImpl<T>
    (
        string? paramName,
        T? value,
        Func<T?, bool> condition
    )
    {
        if (condition(value))
            throw new ArgumentException(paramName);
    }

    [DoesNotReturn]
    public static void ThrowArgument(object? value, [CallerArgumentExpression(nameof(value))] string? paramName = default)
    {
        throw new ArgumentException(paramName);
    }

    public static void ThrowIfNull
    (
        object? value,

        [CallerArgumentExpression(nameof(value))]
        string? paramName = default
    )
    {
        ThrowImpl(paramName, value, x => x is null);
    }

    public static void ThrowIfNullOrEmpty
    (
        string? value,

        [CallerArgumentExpression(nameof(value))]
        string? paramName = default
    )
    {
        ThrowImpl(paramName, value, string.IsNullOrEmpty);
    }

    public static void ThrowIfNullOrWhiteSpace
    (
        string? value,

        [CallerArgumentExpression(nameof(value))]
        string? paramName = default
    )
    {
        ThrowImpl(paramName, value, string.IsNullOrWhiteSpace);
    }

    public static void ThrowIfDisposed(Type type, bool isDisposed)
    {
        if (isDisposed)
            throw new ObjectDisposedException(type.FullName);
    }
}
