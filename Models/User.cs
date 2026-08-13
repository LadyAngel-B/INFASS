namespace INFASS.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public User(string fullName, string email, string password)
        {
            FullName = fullName;
            Email = email;
            Password = password;
        }


        public string GenerateInsertQuery(string tableName, string[] fields, object[] values)
        {
            string query = "INSERT INTO " + tableName + " (";

            for (int i = 0; i < fields.Length; i++)
            {
                query += fields[i];

                if (i < fields.Length - 1)
                    query += ", ";
            }

            query += ") VALUES (";

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is string)
                    query += "'" + values[i] + "'";
                else
                    query += values[i];

                if (i < values.Length - 1)
                    query += ", ";
            }

            query += ")";

            return query;
        }
    }
}


