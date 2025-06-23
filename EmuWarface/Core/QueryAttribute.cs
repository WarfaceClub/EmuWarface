
using System;
using EmuWarface.Xmpp;

namespace EmuWarface.Core
{
	[AttributeUsage(AttributeTargets.Method)]
	public class QueryAttribute : Attribute
	{
		public string[] Names { get; private set; }
		public IqType Type { get; private set; }

		public QueryAttribute(IqType iq_type, params string[] query_names)
		{
			Type = iq_type;
			Names = query_names;
		}
	}
}
