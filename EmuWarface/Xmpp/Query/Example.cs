namespace EmuWarface.Xmpp.Query
{
	public static class Example
	{
		[Query(IqType.Get, "example")]
		public static void ExampleSerializer(Client client, Iq iq)
		{

		}
	}
}
