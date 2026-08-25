namespace INFASS.Models
{
    public class User
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public User(string fullName, string email, string password)
        {
            FullName = fullName;
            Email = email;
            Password = password;
        }

        // CREATE / INSERT
        public string GenerateInsertQuery(
            string tableName,
            string[] fields,
            object[] values)
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

        // SELECT
        public string GenerateSelectQuery(
            string tableName,
            string field,
            object value)
        {
            string query =
                "SELECT * FROM " + tableName +
                " WHERE " + field + " = ";

            if (value is string)
                query += "'" + value + "'";
            else
                query += value;

            return query;
        }

        // UPDATE
        public string GenerateUpdateQuery(
            string tableName,
            string[] fields,
            object[] values,
            string whereField,
            object whereValue)
        {
            string query =
                "UPDATE " + tableName + " SET ";

            for (int i = 0; i < fields.Length; i++)
            {
                query += fields[i] + " = ";

                if (values[i] is string)
                    query += "'" + values[i] + "'";
                else
                    query += values[i];

                if (i < fields.Length - 1)
                    query += ", ";
            }

            query += " WHERE " + whereField + " = ";

            if (whereValue is string)
                query += "'" + whereValue + "'";
            else
                query += whereValue;

            return query;
        }

        // DELETE
        public string GenerateDeleteQuery(
            string tableName,
            string field,
            object value)
        {
            string query =
                "DELETE FROM " + tableName +
                " WHERE " + field + " = ";

            if (value is string)
                query += "'" + value + "'";
            else
                query += value;

            return query;
        }
    }

}
