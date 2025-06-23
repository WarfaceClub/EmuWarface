using System.Linq;

namespace EmuWarface.Commands
{
	public class OnlineCommand : ICmd
	{
		public Permission MinPermission => Permission.None;
		public string Usage => "online";
		public string Example => "online";
		public string[] Names => new[] { "online" };

		public string OnCommand(Permission permission, string[] args)
		{
			return "Online: " + Server.Clients.Where(x => !x.IsDedicated).Count();
		}
	}
}
