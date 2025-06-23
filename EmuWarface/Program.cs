using System;
using System.Threading;
using EmuWarface.Game.Clans;
using EmuWarface.Game.Shops;

namespace EmuWarface
{
	public class Program
	{
		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += UnhandledException;

			Log.Info("Starting...");

			SQL.Init();
			CommandHandler.Init();
			QueryBinder.Init();
			QueryCache.Init();
			GameData.Init();
			Shop.Init();
			Server.Init();
			Clan.GenerateClanList();
			Thread.Sleep(-1);
		}

		private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;
			Log.Error(ex.ToString());
			Environment.Exit(1);
		}
	}
}
