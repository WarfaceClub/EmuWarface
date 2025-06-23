using System;

namespace EmuWarface.Xmpp.Query
{
	public static class NotifyExpiredItems
	{
		[Query(IqType.Get, "notify_expired_items")]
		public static void NotifyExpiredItemsSerializer(Client client, Iq iq)
		{
			if (client.Profile == null)
				throw new InvalidOperationException();

			//TODO
		}
	}
}
