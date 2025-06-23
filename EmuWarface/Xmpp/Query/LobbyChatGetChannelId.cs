using System.Xml;
using EmuWarface.Game.Enums;

namespace EmuWarface.Xmpp.Query
{
	public static class LobbyChatGetChannelId
	{
		[Query(IqType.Get, "lobbychat_getchannelid")]
		public static void LobbyChatGetChannelIdSerializer(Client client, Iq iq)
		{
			LobbyChatChannel channel = (LobbyChatChannel)int.Parse(iq.Query.GetAttribute("channel"));

			XmlElement lobbychat_getchannelid = Xml.Element("lobbychat_getchannelid")
				.Attr("channel_id", Chat.GetChannelId(client, channel))
				.Attr("channel", (int)channel)
				.Attr("service_id", "conference.warface");

			iq.SetQuery(lobbychat_getchannelid);
			client.QueryResult(iq);
		}
	}
}
