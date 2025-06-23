using System.Xml;
using EmuWarface.Core;
using EmuWarface.Xmpp;

namespace EmuWarface.Game.GameRooms
{
	public class GameRoomMaster : GameRoomExtension
	{
		public Client Client { get; private set; }

		public GameRoomMaster(Client master)
		{
			Set(master);
		}

		public void Set(Client master)
		{
			Client = master;
		}

		public override XmlElement Serialize()
		{
			return Xml.Element("room_master")
				.Attr("master", Client.Profile.Id)
				.Attr("revision", Revision);
		}
	}
}
