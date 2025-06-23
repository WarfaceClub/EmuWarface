using System;
using System.Diagnostics;
using System.Threading;
using EmuWarface.Game.Clans;
using EmuWarface.Game.Shops;

namespace EmuWarface
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				Console.Title = "EmuWarface";
			}
			catch
			{
				// ignore
			}

			AppDomain.CurrentDomain.UnhandledException += UnhandledException;

			Log.Info("Starting...");

#if RELEASE
			var watch = Stopwatch.StartNew();
#endif
			SQL.Init();
			CommandHandler.Init();
			QueryBinder.Init();
			QueryCache.Init();
			GameData.Init();
			Shop.Init();
			Server.Init();
			Clan.GenerateClanList();

#if RELEASE
			var elapsed = watch.Elapsed;
			watch.Stop();
			Log.Info("Started in {0}.", elapsed);
#endif

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
