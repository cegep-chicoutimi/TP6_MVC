using System;
using System.Data;
using MySql.Data.MySqlClient;
using Mastermind.Models;

namespace Mastermind.DataAccessLayer.Factories
{
    public class MemberPreferencesFactory
    {
        private readonly string _connectionString;

        public MemberPreferencesFactory()
        {
            _connectionString = DAL.ConnectionString ?? throw new InvalidOperationException("Connection string is not set");
        }

        public MemberPreferences? GetByMemberId(int memberId)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();
            using MySqlCommand command = new MySqlCommand("SELECT * FROM tp6_member_preferences WHERE IdMembre = @MemberId", connection);
            command.Parameters.AddWithValue("@MemberId", memberId);
            using MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new MemberPreferences
                {
                    Id = (int)reader["Id"],
                    MemberId = (int)reader["IdMembre"],
                    NbColors = (int)reader["NbColors"],
                    NbPositions = (int)reader["NbPositions"],
                    NbAttempts = (int)reader["NbAttempts"]
                };
            }
            return null;
        }

        public void CreateOrUpdate(MemberPreferences preferences)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();
            using MySqlCommand command = new MySqlCommand(@"
                INSERT INTO tp6_member_preferences (IdMembre, NbColors, NbPositions, NbAttempts)
                VALUES (@MemberId, @NbColors, @NbPositions, @NbAttempts)
                ON DUPLICATE KEY UPDATE
                    NbColors = @NbColors,
                    NbPositions = @NbPositions,
                    NbAttempts = @NbAttempts", connection);

            command.Parameters.AddWithValue("@MemberId", preferences.MemberId);
            command.Parameters.AddWithValue("@NbColors", preferences.NbColors);
            command.Parameters.AddWithValue("@NbPositions", preferences.NbPositions);
            command.Parameters.AddWithValue("@NbAttempts", preferences.NbAttempts);

            command.ExecuteNonQuery();
        }

        public void Delete(int memberId)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();
            using MySqlCommand command = new MySqlCommand("DELETE FROM tp6_member_preferences WHERE IdMembre = @MemberId", connection);
            command.Parameters.AddWithValue("@MemberId", memberId);
            command.ExecuteNonQuery();
        }
    }
} 