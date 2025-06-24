using System;
using System.Collections.Generic;
using System.Xml;
using XpNet;
using StringBuilder = System.Text.StringBuilder;
using SystemEncoding = System.Text.Encoding;

namespace EmuWarface.Xmpp.Parser;

// Based on parser from XmppDotNet but using SAX-like events.
// COPYRIGHT: https://github.com/agnauck/XmppDotNet/blob/master/src/XmppDotNet.Core/Xml/Parser/StreamParser.cs
public class XmlParser : IDisposable
{
	private volatile bool disposed;
	private ByteBuffer byteBuf = new();
	private readonly SystemEncoding _encoding = SystemEncoding.UTF8;
	private readonly UTF8Encoding _tokenizer = new();
	private StringBuilder cdata = null;
	private bool isCdata;

	public event Action<string, IReadOnlyDictionary<string, string>> OnStartElement;
	public event Action<string> OnEndElement;
	public event Action<string> OnText;
	public event Action<string> OnCdata;
	public event Action<string> OnComment;

	public void Write(byte[] buf)
		=> Write(buf, 0, buf.Length);

	protected void ThrowIfDisposed()
	{
		if (disposed)
			throw new ObjectDisposedException(GetType().FullName);
	}

	public void Write(byte[] buf, int offset, int count)
	{
		lock (this)
		{
			ThrowIfDisposed();

			byteBuf.Write(buf.AsSpan(offset, count));

			var b = byteBuf.GetBuffer(); // memory or span is better to match with Sysem.Text.Encoding overloads.
			int off = 0;
			var ct = new ContentToken();

			try
			{
				while (off < b.Length)
				{
					Tokens tok;

					if (isCdata)
						tok = _tokenizer.TokenizeCdataSection(b, off, b.Length, ct);
					else
						tok = _tokenizer.TokenizeContent(b, off, b.Length, ct);

					switch (tok)
					{
						case Tokens.PartialChar:
						case Tokens.PartialToken:
						case Tokens.ExtensibleToken:
							return;

						case Tokens.EmptyElementWithAtts:
						case Tokens.EmptyElementNoAtts:
							StartTag(b, off, ct, tok);
							EndTag(b, off, ct, tok);
							break;

						case Tokens.StartTagNoAtts:
						case Tokens.StartTagWithAtts:
							StartTag(b, off, ct, tok);
							break;

						case Tokens.EndTag:
							EndTag(b, off, ct, tok);
							break;

						case Tokens.DataChars:
						case Tokens.DataNewline:
							Content(_encoding.GetString(b, off, ct.TokenEnd - off));
							break;

						case Tokens.CharReference:
						case Tokens.MagicEntityReference:
							Content(new string(new[] { ct.RefChar1 }));
							break;

						case Tokens.CharPairReference:
							Content(new string(new[] { ct.RefChar1, ct.RefChar2 }));
							break;

						case Tokens.Comment:
							{
								var startPos = off + 4 * _tokenizer.MinBytesPerChar;
								var endPos = ct.TokenEnd - off - 7 * _tokenizer.MinBytesPerChar;
								var text = _encoding.GetString(b, startPos, endPos);
								OnComment?.Invoke(text);
							}
							break;

						case Tokens.CdataSectOpen:
							isCdata = true;
							break;

						case Tokens.CdataSectClose:

							if (cdata != null)
								OnCdata?.Invoke(cdata.ToString());

							cdata = null;
							isCdata = false;

							break;

						// skip <?xml ... ?>
						case Tokens.XmlDeclaration:
							break;

						case Tokens.EntityReference:
						case Tokens.ProcessingInstruction:
							throw new XmlException("Invalid XML token.");
					}

					off = ct.TokenEnd;
				}
			}
			// no catch here, propagate exception to underlying parser (eg: XmppParser).
			finally
			{
				byteBuf.RemoveFirst(off);
			}
		}
	}

	void StartTag(byte[] buf, int offset, ContentToken ct, Tokens tok)
	{
		var attributes = new Dictionary<string, string>();

		string name, value;
		int start, end;

		if (tok == Tokens.StartTagWithAtts || tok == Tokens.EmptyElementWithAtts)
		{
			for (int i = 0; i < ct.GetAttributeSpecifiedCount(); i++)
			{
				start = ct.GetAttributeNameStart(i);
				end = ct.GetAttributeNameEnd(i);
				name = _encoding.GetString(buf, start, end - start);

				start = ct.GetAttributeValueStart(i);
				end = ct.GetAttributeValueEnd(i);
				value = NormalizeAttributeValue(buf, start, end - start);

				attributes.Add(name, value);
			}
		}

		name = _encoding.GetString(buf, offset + _tokenizer.MinBytesPerChar,
			ct.NameEnd - offset - _tokenizer.MinBytesPerChar);

		Log.Debug("Start tag");

		OnStartElement?.Invoke(name, attributes);
	}

	void EndTag(byte[] buf, int offset, ContentToken ct, Tokens tok)
	{
		var nameStart = offset + 2 * _tokenizer.MinBytesPerChar; // </[tagname]>
		var name = _encoding.GetString(buf, nameStart, ct.NameEnd - nameStart);
		Log.Debug("End tag");
		OnEndElement?.Invoke(name);
	}

	string NormalizeAttributeValue(byte[] buf, int offset, int length)
	{
		if (length == 0)
			return string.Empty;

		string val = null;
		var buffer = new ByteBuffer();
		var copy = new byte[length];
		Buffer.BlockCopy(buf, offset, copy, 0, length);
		buffer.Write(copy);
		byte[] b = buffer.GetBuffer();
		int off = 0;
		var ct = new ContentToken();
		try
		{
			while (off < b.Length)
			{
				Tokens tok = _tokenizer.TokenizeAttributeValue(b, off, b.Length, ct);

				switch (tok)
				{
					case Tokens.PartialToken:
					case Tokens.PartialChar:
					case Tokens.ExtensibleToken:
						return null;

					case Tokens.AttributeValueS:
					case Tokens.DataChars:
					case Tokens.DataNewline:
						val += _encoding.GetString(b, off, ct.TokenEnd - off);
						break;
					case Tokens.CharReference:
					case Tokens.MagicEntityReference:
						val += new string(new[] { ct.RefChar1 });
						break;
					case Tokens.CharPairReference:
						val += new string(new[] { ct.RefChar1, ct.RefChar2 });
						break;
					case Tokens.EntityReference:
						throw new XmlException("Invalid XML attribute token.");
				}
				off = ct.TokenEnd;
			}
		}
		finally
		{
			buffer.RemoveFirst(off);
		}

		return val;
	}

	void Content(string s)
	{
		if (isCdata)
		{
			cdata ??= new();
			cdata.Append(s);
			Log.Debug("add cdata text");
		}
		else
		{
			Log.Debug("text handler");
			OnText?.Invoke(s);
		}
	}

	public void Reset()
	{
		ThrowIfDisposed();

		lock (this)
		{
			Log.Debug("reset");
			isCdata = false;
			byteBuf.Clear();
		}
	}

	public void Dispose()
	{
		lock (this)
		{
			if (disposed)
				return;

			disposed = true;
			byteBuf.Clear();
			byteBuf = null;
		}

		GC.SuppressFinalize(this);
	}
}