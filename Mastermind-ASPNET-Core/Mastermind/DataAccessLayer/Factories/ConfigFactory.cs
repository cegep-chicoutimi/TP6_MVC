using Mastermind.Models;
using MySql.Data.MySqlClient;

namespace Mastermind.DataAccessLayer.Factories
{
    public class ConfigFactory
    {
        private Config CreateFromReader(MySqlDataReader reader)
        {
            string key = reader["key"].ToString() ?? string.Empty;
            string value = reader["value"].ToString() ?? string.Empty;

            return new Config() { Key = key, Value = value };
        }

        public Dictionary<string, Config> GetAll()
        {
            Dictionary<string, Config> configByKey = new();

            MySqlConnection? cnn = null;
            MySqlDataReader? reader = null;

            try
            {
                cnn = new MySqlConnection(DAL.ConnectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "SELECT * FROM tp6_config";

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Config config = CreateFromReader(reader);
                    configByKey.Add(config.Key, config);
                }
            }
            finally
            {
                reader?.Close();
                cnn?.Close();
            }

            return configByKey;
        }

        public Config? Get(string key)
        {
            Config? config = null;

            MySqlConnection? cnn = null;
            MySqlDataReader? reader = null;

            try
            {
                cnn = new MySqlConnection(DAL.ConnectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "SELECT * FROM tp6_config WHERE `key` = @key";
                cmd.Parameters.Add(new MySqlParameter("@key", key));

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    config = CreateFromReader(reader);
                }
            }
            finally
            {
                reader?.Close();
                cnn?.Close();
            }

            return config;
        }

        public void Save(List<Config> configs)
        {
            foreach (Config config in configs)
            {
                Save(config);
            }
        }

        public void Save(Config config)
        {
            MySqlConnection? mySqlCnn = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "UPDATE tp6_config SET `value`=@value WHERE `key`=@key";

                mySqlCmd.Parameters.AddWithValue("@key", config.Key);
                mySqlCmd.Parameters.AddWithValue("@value", config.Value);

                mySqlCmd.ExecuteNonQuery();
            }
            finally
            {
                mySqlCnn?.Close();
            }
        }
    }
}
