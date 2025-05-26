using System;
using System.Data;
using System.Data.SqlClient;
using Mastermind.Models;
using Org.BouncyCastle.Crypto.Engines;

namespace Mastermind.DataAccessLayer.Factories
{
    public class GameStatsFactory
    {
        private readonly string _connectionString;

        public GameStatsFactory()
        {
            _connectionString = DAL.ConnectionString ?? throw new InvalidOperationException("Connection string is not set");
        }

        public GameStats GetStatsByMemberId(int memberId)
        {
            using SqlConnection connection = new SqlConnection(DAL.ConnectionString);
            connection.Open();
            using SqlCommand command = new SqlCommand("SELECT * FROM tp6_GameStats WHERE MemberId = @MemberId", connection);
            command.Parameters.AddWithValue("@MemberId", memberId);
            using SqlDataReader reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                return new GameStats
                {
                    MemberId = (int)reader["MemberId"],
                    GamesWon = (int)reader["GamesWon"],
                    GamesLost = (int)reader["GamesLost"],
                    BestScore = reader["BestScore"] == DBNull.Value ? null : (int?)reader["BestScore"]
                };
            }
            return null;
        }

        public void CreateOrUpdateStats(int memberId, bool isWin, int? attempts = null)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            using SqlCommand command = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM tp6_GameStats WHERE MemberId = @MemberId)
                BEGIN
                    UPDATE tp6_GameStats 
                    SET GamesWon = GamesWon + @GamesWon,
                        GamesLost = GamesLost + @GamesLost,
                        BestScore = CASE 
                            WHEN @IsWin = 1 AND (@BestScore IS NULL OR @Attempts < BestScore) 
                            THEN @Attempts 
                            ELSE BestScore 
                        END
                    WHERE MemberId = @MemberId
                END
                ELSE
                BEGIN
                    INSERT INTO tp6_GameStats (MemberId, GamesWon, GamesLost, BestScore)
                    VALUES (@MemberId, @GamesWon, @GamesLost, @BestScore)
                END", connection);

            command.Parameters.AddWithValue("@MemberId", memberId);
            command.Parameters.AddWithValue("@GamesWon", isWin ? 1 : 0);
            command.Parameters.AddWithValue("@GamesLost", isWin ? 0 : 1);
            command.Parameters.AddWithValue("@IsWin", isWin);
            command.Parameters.AddWithValue("@Attempts", (object)attempts ?? DBNull.Value);
            command.Parameters.AddWithValue("@BestScore", (object)attempts ?? DBNull.Value);

            command.ExecuteNonQuery();
        }
    }
} 