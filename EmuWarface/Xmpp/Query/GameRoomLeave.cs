using System;

namespace EmuWarface.Xmpp.Query
{
	public static class GameRoomLeave
	{
		[Query(IqType.Get, "gameroom_leave")]
		public static void GameRoomLeaveSerializer(Client client, Iq iq)
		{
			if (client.Profile == null)
				throw new InvalidOperationException();

			if (client.Profile.RoomPlayer == null)
				return;

			client.Profile.Room?.LeftPlayer(client);

			iq.SetQuery(Xml.Element("gameroom_leave"));
			client.QueryResult(iq);
		}
	}
}
