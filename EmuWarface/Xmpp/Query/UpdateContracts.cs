using System;
using EmuWarface.Core;

namespace EmuWarface.Xmpp.Query
{
	public static class UpdateContracts
	{
		[Query(IqType.Get, "update_contracts")]
		public static void UpdateContractsSerializer(Client client, Iq iq)
		{
			if (!client.IsDedicated)
				throw new InvalidOperationException();

			//TODO


		}
	}
}
