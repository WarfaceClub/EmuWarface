using System.Linq;

namespace EmuWarface.Commands
{
	public class KickCommand : ICmd
	{
		public Permission MinPermission => Permission.Moderator;
		public string Usage => "kick <nickname>";
		public string Example => "kick user1";
		public string[] Names => new[] { "kick", "k" };

		public string OnCommand(Permission permission, string[] args)
		{
			if (args.Length == 0)
				return $"Invalid arguments.\nExample:\n{Example}";

			string nickname = args[0];

			try
			{
				Client client;
				lock (Server.Clients)
				{
					client = Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == nickname);
				}

				if (client == null)
				{
					return $"Player with nickname '{nickname}' not online.";
				}

				client.Dispose();

				return $"Player with nickname '{nickname}' is kicked.";
			}
			catch (ServerException e)
			{
				return e.Message;
			}
		}
	}
}
