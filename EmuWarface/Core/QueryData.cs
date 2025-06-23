
using System.Reflection;
using EmuWarface.Xmpp;

namespace EmuWarface.Core
{
	public class QueryData
	{
		public MethodInfo Method { get; }
		public string[] QueryNames { get; }
		public IqType QueryType { get; }

		public QueryData(MethodInfo method, string[] queryNames, IqType queryType)
		{
			Method = method;
			QueryNames = queryNames;
			QueryType = queryType;
		}
	}
}
