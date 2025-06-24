using System;
using System.Xml;
using EmuWarface.Xmpp.Parser;

namespace EmuWarface.Xmpp;

public sealed class XmppParser : IDisposable
{
	private XmlParser parser;
	private XmlNamespaceManager namespaces = new(new NameTable());

	public event Action<XmlElement> OnStreamStart;
	public event Action<XmlElement> OnStreamElement;
	public event Action OnStreamEnd;

	XmlDocument document;
	XmlElement current;

	public XmppParser()
	{
		parser = new XmlParser();

		parser.OnStartElement += (name, attrs) =>
		{
			namespaces.PushScope();

			// Populate namespaces
			foreach (var (attName, attValue) in attrs)
			{
				if (attName == "xmlns")
					namespaces.AddNamespace(string.Empty, attValue);
				else if (attName.StartsWith("xmlns:"))
					namespaces.AddNamespace(attName[6..], attValue);
			}

			var ofs = name.IndexOf(':');
			var defaultNamespace = namespaces.LookupNamespace(ofs == -1 ? string.Empty : name[0..ofs]);

			document ??= new();

			var element = document.CreateElement(name, defaultNamespace);

			foreach (var (key, value) in attrs)
				element.SetAttribute(key, value);

			if (element.Name == "stream:stream")
				OnStreamStart?.Invoke(element);
			else
			{
				current?.AppendChild(element);
				current = element;
			}
		};

		parser.OnEndElement += name =>
		{
			namespaces.PopScope();

			if (name == "stream:stream")
				OnStreamEnd?.Invoke();
			else
			{
				var parent = current.ParentNode as XmlElement;

				if (parent == null)
					OnStreamElement?.Invoke(current);

				current = parent;
			}
		};
	}

	public void Reset()
	{
		lock (this)
		{
			while (namespaces.PopScope())
				;

			parser.Reset();
		}
	}

	public void Write(byte[] buf, int len)
	{
		lock (this)
			parser.Write(buf, 0, len);
	}

	public void Write(byte[] buf)
	{
		lock (this)
			parser.Write(buf);
	}

	public void Dispose()
	{
		parser?.Dispose();
		parser = null;
	}
}
