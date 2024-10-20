namespace EmuWarface.Api;

public class AsyncEventArgs
{
    public virtual bool Handled
    {
        get;
        set;
    }
}

public delegate Task AsyncEventHandler<E>(E args) where E : AsyncEventArgs;

public sealed class AsyncEvent : AsyncEvent<AsyncEventArgs>
{

}

public interface IAsyncEventRegistration : IDisposable
{

}

public class AsyncEvent<E> where E : AsyncEventArgs
{
    readonly List<AsyncEventRegistration> _handlerList = new();

    class AsyncEventRegistration : IAsyncEventRegistration
    {
        public AsyncEvent<E>? _event;
        public AsyncEventHandler<E>? _function;

        void IDisposable.Dispose()
        {
            _event?.UnregisterHandler(this);
            _event = null;
            _function = null;
        }
    }

    public IAsyncEventRegistration RegisterHandler(AsyncEventHandler<E> handler)
    {
        var entry = new AsyncEventRegistration
        {
            _event = this,
            _function = handler
        };

        lock (_handlerList)
            _handlerList.Add(entry);

        return entry;
    }

    public void UnregisterHandler(IAsyncEventRegistration handler)
    {
        ThrowHelper.ThrowIfNull(handler);

        if (handler is not AsyncEventRegistration token)
            ThrowHelper.ThrowArgument(handler);
        else
        {
            if (token._event != this)
                throw new InvalidOperationException();

            lock (_handlerList)
                _handlerList.Remove(token);
        }
    }

    public async Task InvokeAsync(E @event)
    {
        var exceptions = new List<Exception>();

        AsyncEventRegistration[] handlers;

        lock (_handlerList)
            handlers = _handlerList.ToArray();

        foreach (var handler in handlers)
        {
            try
            {
                await handler._function!(@event).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            if (@event.Handled)
                break;
        }

        if (exceptions.Count > 0)
            throw new AggregateException(exceptions);
    }
}
