using System;
using System.Data;

namespace EmuWarface.Core
{
	public static class SQL
	{
		static readonly MySqlConnectorFactory s_ConnectionFactory = MySqlConnectorFactory.Instance;

		static Lazy<string> s_ConnectionString = new(() =>
		{
			var result = new MySqlConnectionStringBuilder
			{
				Server = Config.Sql.Server,
				UserID = Config.Sql.User,
				Password = Config.Sql.Password,
				Database = Config.Sql.Database,
				Port = Config.Sql.Port,
				ConvertZeroDateTime = true,
				CancellationTimeout = 15_000,
				ConnectionProtocol = MySqlConnectionProtocol.Tcp,
			};

			if (!string.IsNullOrWhiteSpace(Config.Sql.CharacterSet))
				result.CharacterSet = Config.Sql.CharacterSet;

			return result.ToString();
		}, true);

		public static MySqlConnection OpenConnection()
		{
			var con = new MySqlConnection(s_ConnectionString.Value);
			con.Open();
			return con;
		}

		public static void Init()
		{
			try
			{
				using (_ = OpenConnection())
					Log.Info("[SQL] Connected to '{0}' database", Config.Sql.Database);
			}
			catch (Exception)
			{
				Log.Error("[SQL] Failed to connect to '{0}' database", Config.Sql.Database);
				throw;
			}
		}

		public static void Query(string command)
		{
			using (MySqlCommand cmd = new MySqlCommand(command))
				Query(cmd);
		}

		public static void Query(MySqlCommand command)
		{
			try
			{
				using (var con = OpenConnection())
				{
					command.Connection = con;
					command.ExecuteNonQuery();
				}
			}
			catch (Exception e)
			{
				//TODO
				Log.Error(e.ToString());
			}
		}

		public static DataTable QueryRead(string command)
		{

			using (var cmd = new MySqlCommand(command))
				return QueryRead(cmd, false);
		}

		public static DataTable QueryRead(MySqlCommand command, bool dispose = true)
		{
			try
			{
				using (var connection = OpenConnection())
				{
					command.Connection = connection;
					var result = new DataTable();

					var reader = command.ExecuteReader();

					if (dispose)
					{
						using (command)
							result.Load(reader);
					}
					else
					{
						result.Load(reader);
					}

					return result;
				}
			}
			catch (Exception e)
			{
				Log.Error("[SQL] Querying failed:" + e);
			}

			return new();
		}
	}
}
