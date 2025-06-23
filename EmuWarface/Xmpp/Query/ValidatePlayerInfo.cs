using System;

namespace EmuWarface.Xmpp.Query
{
	public static class ValidatePlayerInfo
	{
		[Query(IqType.Get, "validate_player_info")]
		public static void ValidatePlayerInfoSerializer(Client client, Iq iq)
		{
			if (client.Profile == null)
				throw new InvalidOperationException();

			//TODO

			client.QueryResult(iq.SetQuery(Xml.Element("validate_player_info")));
		}
	}
}
