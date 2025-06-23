using EmuWarface.Core;

namespace EmuWarface.Xmpp.Query
{
	public static class GetContracts
	{
		[Query(IqType.Get, "get_contracts")]
		public static void GetContractsSerializer(Client client, Iq iq)
		{
			client.QueryResult(iq);
		}
	}
}
