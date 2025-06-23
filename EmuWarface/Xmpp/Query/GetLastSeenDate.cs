using System.Xml;

namespace EmuWarface.Xmpp.Query
{
	public static class GetLastSeenDate
	{
		[Query(IqType.Get, "get_last_seen_date")]
		public static void GetLastSeenDateSerializer(Client client, Iq iq)
		{
			ulong profile_id = ulong.Parse(iq.Query.GetAttribute("profile_id"));

			if (profile_id == 0)
				throw new QueryException(1);

			long lastSeen = Profile.GetLastSeenDate(profile_id);

			XmlElement response = Xml.Element(iq.Query.LocalName)
				.Attr("profile_id", profile_id)
				.Attr("last_seen", lastSeen);

			iq.SetQuery(response);
			client.QueryResult(iq);
		}
	}
}
