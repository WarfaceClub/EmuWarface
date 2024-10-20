namespace EmuWarface.Api.Xmpp;

public sealed class XmppParser
{
    public event Func<Element, Task>? OnStreamStart;
    public event Func<Element, Task>? OnStreamElement;
    public event Func<Task>? OnStreamEnd;
}

public class Element
{

}
