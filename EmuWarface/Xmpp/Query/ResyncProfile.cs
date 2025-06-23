using System;
using EmuWarface.Core;

namespace EmuWarface.Xmpp.Query
{
	public static class ResyncProfile
	{
		[Query(IqType.Get, "resync_profile")]
		public static void Serializer(Client client, Iq iq)
		{
			if (client.Profile == null)
				throw new InvalidOperationException();

			iq.SetQuery(client.Profile.ResyncProfie());
			client.QueryResult(iq);
		}
	}
}