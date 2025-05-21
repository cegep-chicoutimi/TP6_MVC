using Mastermind.Models;
using MySql.Data.MySqlClient;
using Mastermind.Helper;

namespace Mastermind.DataAccessLayer.Factories
{
    public class MemberFactory
    {
        private Member CreateFromReader(MySqlDataReader reader)
        {
            int id = (int)reader["Id"];
            string fullName = reader["FullName"].ToString() ?? string.Empty;
            string email = reader["Email"].ToString() ?? string.Empty;
            string username = reader["Username"].ToString() ?? string.Empty;
            string password = reader["Password"].ToString() ?? string.Empty;
            string role = reader["Role"].ToString() ?? string.Empty;
            string imagePath = reader["ImagePath"].ToString() ?? string.Empty;

            return new Member(id, fullName, email, username, password, role, imagePath);
        }

        public Member CreateEmpty()
        {
            return new Member(0, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public List<Member> GetAll()
        {
            List<Member> members = new();

            MySqlConnection? cnn = null;
            MySqlDataReader? reader = null;

            try
            {
                cnn = new MySqlConnection(DAL.ConnectionString);
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
            Member? member = null;
            MySqlConnection? cnn = null;
            MySqlDataReader? reader = null;

            try
            {
                cnn = new MySqlConnection(DAL.ConnectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "SELECT * FROM tp6_members WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", id);

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

        public Member? GetByUsername(string username)
        {
            Member? member = null;
            MySqlConnection? cnn = null;
            MySqlDataReader? reader = null;

            try
            {
                cnn = new MySqlConnection(DAL.ConnectionString);
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
                cnn = new MySqlConnection(DAL.ConnectionString);
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

        public int Create(Member member)
        {
            MySqlConnection? cnn = null;

            try
            {
                cnn = new MySqlConnection(DAL.ConnectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = @"INSERT INTO tp6_members (FullName, Email, Username, Password, Role, ImagePath) 
                                  VALUES (@FullName, @Email, @Username, @Password, @Role, @ImagePath);
                                  SELECT LAST_INSERT_ID();";

                cmd.Parameters.AddWithValue("@FullName", member.FullName);
                cmd.Parameters.AddWithValue("@Email", member.Email);
                cmd.Parameters.AddWithValue("@Username", member.Username);
                cmd.Parameters.AddWithValue("@Password", CryptographyHelper.HashPassword(member.Password));
                cmd.Parameters.AddWithValue("@Role", member.Role);
                cmd.Parameters.AddWithValue("@ImagePath", member.ImagePath);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                cnn?.Close();
            }
        }

        public bool Update(Member member)
        {
            MySqlConnection? cnn = null;

            try
            {
                cnn = new MySqlConnection(DAL.ConnectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = @"UPDATE tp6_members 
                                  SET FullName = @FullName, 
                                      Email = @Email, 
                                      Username = @Username, 
                                      Role = @Role, 
                                      ImagePath = @ImagePath";

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
                cnn = new MySqlConnection(DAL.ConnectionString);
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
    }
}
