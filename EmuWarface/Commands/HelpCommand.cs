using System.Linq;
using EmuWarface.Core;

namespace EmuWarface.Commands
{
	public class HelpCommand : ICmd
	{
		public Permission MinPermission => Permission.None;
		public string Usage => "help";
		public string Example => "help";
		public string[] Names => new[] { "help" };

		public string OnCommand(Permission permission, string[] args)
		{
			return "Command list:\n" + string.Join('\n', CommandHandler.Handlers.Where(c => permission >= c.MinPermission).Select(c => c.Usage).ToArray());
		}
	}
}
