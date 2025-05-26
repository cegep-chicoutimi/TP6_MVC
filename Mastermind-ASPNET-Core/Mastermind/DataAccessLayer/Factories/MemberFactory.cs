using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Mastermind.Models;
using MySql.Data.MySqlClient;
using Mastermind.Helper;

namespace Mastermind.DataAccessLayer.Factories
{
    public class MemberFactory
    {
        private readonly string _connectionString;

        public MemberFactory()
        {
            _connectionString = DAL.ConnectionString ?? throw new InvalidOperationException("Connection string is not set");
        }

        private Member CreateFromReader(MySqlDataReader reader)
        {
            int id = (int)reader["Id"];
            string fullName = reader["FullName"].ToString() ?? string.Empty;
            string email = reader["Email"].ToString() ?? string.Empty;
            string username = reader["Username"].ToString() ?? string.Empty;
            string password = reader["Password"].ToString() ?? string.Empty;
            string role = reader["Role"].ToString() ?? string.Empty;
            string imagePath = reader["ImagePath"].ToString() ?? string.Empty;
            DateTime registrationDate = (DateTime)reader["RegistrationDate"];

            return new Member(id, fullName, email, username, password, role, imagePath, registrationDate);
        }

        public Member CreateEmpty()
        {
            return new Member(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue);
        }

        public List<Member> GetAll()
        {
            List<Member> members = new();

            MySqlConnection? cnn = null;
            MySqlDataReader? reader = null;

            try
            {
                cnn = new MySqlConnection(_connectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "SELECT * FROM tp6_members";

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    members.Add(CreateFromReader(reader));
                }
            }
            finally
            {
                reader?.Close();
                cnn?.Close();
            }

            return members;
        }

        public Member? GetById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand("SELECT * FROM tp6_members WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return CreateFromReader(reader);
                        }
                        return null;
                    }
                }
            }
        }

        public Member? GetByUsername(string username)
        {
            Member? member = null;
            MySqlConnection? cnn = null;
            MySqlDataReader? reader = null;

            try
            {
                cnn = new MySqlConnection(_connectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "SELECT * FROM tp6_members WHERE Username = @Username";
                cmd.Parameters.AddWithValue("@Username", username);

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    member = CreateFromReader(reader);
                }
            }
            finally
            {
                reader?.Close();
                cnn?.Close();
            }

            return member;
        }

        public bool IsUsernameUnique(string username, int? excludeId = null)
        {
            MySqlConnection? cnn = null;
            MySqlDataReader? reader = null;

            try
            {
                cnn = new MySqlConnection(_connectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM tp6_members WHERE Username = @Username";
                if (excludeId.HasValue)
                {
                    cmd.CommandText += " AND Id != @ExcludeId";
                    cmd.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
                }
                cmd.Parameters.AddWithValue("@Username", username);

                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) == 0;
            }
            finally
            {
                reader?.Close();
                cnn?.Close();
            }
        }

        public void Create(Member member)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(@"
                    INSERT INTO tp6_members (FullName, Email, Username, Password, Role, ImagePath, RegistrationDate)
                    VALUES (@FullName, @Email, @Username, @Password, @Role, @ImagePath, @RegistrationDate);
                    SELECT LAST_INSERT_ID();", connection))
                {
                    command.Parameters.AddWithValue("@FullName", member.FullName);
                    command.Parameters.AddWithValue("@Email", member.Email);
                    command.Parameters.AddWithValue("@Username", member.Username);
                    command.Parameters.AddWithValue("@Password", CryptographyHelper.HashPassword(member.Password));
                    command.Parameters.AddWithValue("@Role", member.Role);
                    command.Parameters.AddWithValue("@ImagePath", member.ImagePath);
                    command.Parameters.AddWithValue("@RegistrationDate", member.RegistrationDate);

                    member.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public bool Update(Member member)
        {
            MySqlConnection? cnn = null;

            try
            {
                cnn = new MySqlConnection(_connectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = @"UPDATE tp6_members 
                                  SET FullName = @FullName, 
                                      Email = @Email, 
                                      Username = @Username, 
                                      Role = @Role, 
                                      ImagePath = @ImagePath,
                                      RegistrationDate = @RegistrationDate";

                // Only update password if it's not empty
                if (!string.IsNullOrEmpty(member.Password))
                {
                    cmd.CommandText += ", Password = @Password";
                    cmd.Parameters.AddWithValue("@Password", CryptographyHelper.HashPassword(member.Password));
                }

                cmd.CommandText += " WHERE Id = @Id";

                cmd.Parameters.AddWithValue("@Id", member.Id);
                cmd.Parameters.AddWithValue("@FullName", member.FullName);
                cmd.Parameters.AddWithValue("@Email", member.Email);
                cmd.Parameters.AddWithValue("@Username", member.Username);
                cmd.Parameters.AddWithValue("@Role", member.Role);
                cmd.Parameters.AddWithValue("@ImagePath", member.ImagePath);
                cmd.Parameters.AddWithValue("@RegistrationDate", member.RegistrationDate);

                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                cnn?.Close();
            }
        }

        public bool Delete(int id)
        {
            MySqlConnection? cnn = null;

            try
            {
                cnn = new MySqlConnection(_connectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "DELETE FROM tp6_members WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                cnn?.Close();
            }
        }

        public bool IsLoginSuccessful(string username, string password)
        {
            Member? member = GetByUsername(username);
            if (member == null)
                return false;

            return CryptographyHelper.ValidateHashedPassword(password, member.Password);
        }

        public List<MonthlySignup> GetMonthlySignups(int months = 6)
        {
            var signups = new List<MonthlySignup>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(@"
                    SELECT 
                        DATE_FORMAT(RegistrationDate, '%Y-%m') AS Month,
                        COUNT(*) AS NewMembers
                    FROM tp6_members
                    WHERE RegistrationDate >= DATE_SUB(CURDATE(), INTERVAL @Months MONTH)
                    GROUP BY DATE_FORMAT(RegistrationDate, '%Y-%m')
                    ORDER BY Month", connection))
                {
                    command.Parameters.AddWithValue("@Months", months);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            signups.Add(new MonthlySignup
                            {
                                Month = (string)reader["Month"],
                                Count = (int)reader["NewMembers"]
                            });
                        }
                    }
                }
            }
            return signups;
        }
    }

    public class MonthlySignup
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
