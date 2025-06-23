using System;

namespace EmuWarface.Core
{
	public class ServerException : Exception
	{
		public ServerException(string message)
			: base(message)
		{

		}
	}
}
