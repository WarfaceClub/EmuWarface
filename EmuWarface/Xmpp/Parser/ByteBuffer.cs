using System;
using System.Diagnostics;
using System.Text;

namespace EmuWarface.Xmpp.Parser;

// COPYRIGHT: https://github.com/agnauck/XmppDotNet/blob/master/src/XmppDotNet.Core/Xml/Parser/ByteBuffer.cs
// Modified version of ByteBuffer from XmppDotNet library.
[DebuggerDisplay("{ToString(),nq}")]
public sealed class ByteBuffer
{
	private byte[] buf = Array.Empty<byte>();

	public void Write(byte[] bytes)
	{
		buf = Combine(buf, bytes);
	}

	// Attempt zero-copy allocation overhead for persistent buffers
	public void Write(ReadOnlySpan<byte> bytes)
	{
		buf = Combine(buf, bytes);
	}

	public byte[] GetBuffer()
	{
		return buf;
	}

	/// <summary>
	/// Removes the given number of bytes from the beginning of the buffer.
	/// </summary>
	/// <param name="offset">The offset.</param>
	public void RemoveFirst(int offset)
	{
		buf = RemoveFirst(buf, offset);
	}

	/// <summary>
	/// Clears the buffer
	/// </summary>
	public void Clear()
	{
		buf = Array.Empty<byte>();
	}

	/// <summary>
	/// To show the buffer as a nice string in the debugger
	/// </summary>
	public override string ToString()
	{
		return Encoding.UTF8.GetString(GetBuffer());
	}

	/// <summary>
	/// Removes the given number of bytes at the beginning of a byte array.
	/// </summary>
	/// <param name="buf">The buf.</param>
	/// <param name="x">The x.</param>
	/// <returns></returns>
	public static byte[] RemoveFirst(byte[] buf, int x)
	{
		if (x >= buf.Length)
			return Array.Empty<byte>();

		byte[] ret = new byte[buf.Length - x];
		Buffer.BlockCopy(buf, x, ret, 0, ret.Length);
		return ret;
	}

	private static byte[] Combine(byte[] first, byte[] second)
	{
		byte[] ret = new byte[first.Length + second.Length];
		Buffer.BlockCopy(first, 0, ret, 0, first.Length);
		Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
		return ret;
	}

	static byte[] Combine(byte[] first, ReadOnlySpan<byte> second)
	{
		byte[] ret = new byte[first.Length + second.Length];
		Buffer.BlockCopy(first, 0, ret, 0, first.Length);
		//Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
		// will directly put all bytes into new allocated buffer
		second.CopyTo(ret.AsSpan(first.Length));
		return ret;
	}
}