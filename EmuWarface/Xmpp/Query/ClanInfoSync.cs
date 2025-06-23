using System;
using EmuWarface.Core;
using EmuWarface.Game.Clans;

namespace EmuWarface.Xmpp.Query
{
	public static class ClanInfoSync
	{
		[Query(IqType.Get, "clan_info_sync")]
		public static void ClanInfoSyncSerializer(Client client, Iq iq)
		{
			if (client.Profile == null)
				throw new InvalidOperationException();

			Clan.ClanInfo(client.Profile.ClanId);
		}
	}
}
