using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;


namespace HabitTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=habit-tracker.db";

            var sqlConnection = new SQLiteConnection(connectionString);

            //Old Sql Connection
            /*
            sqlConnection.Open();

            string sqlSentence = @"CREATE TABLE IF NOT EXISTS Prueba
                (Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DayDate TEXT,
                Quantity INTEGER)";

            SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);

            command.ExecuteNonQuery();

            sqlConnection.Close();
            */

            int userOption = 1;

            do
            {
                try
                {
                    Console.WriteLine("Main Menu \nType 0 to Close Application.\nType 1 to View all Records.\nType 2 to create a Habit.\nType 3 to Insert a Record\nType 4 to Delete a Record\nType 5 to Update a Record\n");
                    Console.Write("Select an option: ");
                    userOption = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.Clear();
                    Console.WriteLine(e.Message);
                    continue;
                }

                GetUserInput(userOption, sqlConnection);

            } while(userOption != 0);
        }

        public static void GetUserInput(int option, SQLiteConnection sqlConnection)
        {
            switch (option)
            {
                case 0: break;
                case 1:
                    ViewHabit(sqlConnection, true);
                    break;
                case 2:
                    CreateHabit(sqlConnection);
                    break;
                case 3:
                    InsertRecord(sqlConnection);
                    break;
                case 4:
                    DeleteRecord(sqlConnection);
                    break;
                case 5:
                    UpdateRecord(sqlConnection);
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Select a valid option!");
                    return;
            }
        }

        public static bool TableExists(SQLiteConnection sqlConnection)
        {
            try
            {
                sqlConnection.Open();

                //string sqlSentence = @"SELECT * FROM sqlite_master WHERE TYPE='table'";
                string sqlSentence = @"SELECT COUNT(*) FROM sqlite_master WHERE TYPE='table'";

                SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);

                var count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    sqlConnection.Close();
                    return true;
                }
                else
                {
                    sqlConnection.Close();
                    return false;
                }

            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                sqlConnection.Close();
                return false;
            }
        }
        public static void ViewHabit(SQLiteConnection sqlConnection, bool functionVerificator)
        {
            Console.Clear();
            try
            {

                bool verifyIfTableExists = TableExists(sqlConnection);

                    if (verifyIfTableExists) 
                    {
                        int i = 1;
                        sqlConnection.Open();
                        string sqlSentence = @"SELECT * FROM sqlite_master WHERE TYPE='table'";
                        SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);
                        var reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                            var name = reader.GetString(1);
                            Console.WriteLine($"{i}. {name}");
                            i++;
                            }
                        i = 0;
                        if (functionVerificator)
                        {
                            Console.Write("Select the habit you want to see\nType cancel to abort\n Select an option: ");
                            string habitSended = Console.ReadLine();
                            if(habitSended.ToLower() == "cancel")
                            {
                                sqlConnection.Close();
                                return;
                            }
                            ViewRecordsFromHabits(habitSended, sqlConnection);
                        }
                        sqlConnection.Close();
                        }
                        else
                        {
                            Console.WriteLine("No habits found.");
                            sqlConnection.Close();
                            return;
                        }
                    }
            }
            catch (Exception e)
            {
                    Console.WriteLine(e.Message);
                    sqlConnection.Close();
                    return;
            }
        }

        public static void ViewRecordsFromHabits(string habitTable, SQLiteConnection sqlConnection)
        {
            try
            {
                string sqlSentence = $"SELECT * From {habitTable}";

                SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);

                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("ID  | Quantity     | Day Date");
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var dayDate = reader.GetString(1);
                        var Quantity = reader.GetInt32(2);
                        Console.WriteLine($"{id}   |    {Quantity}         | {dayDate}");
                    }
                    sqlConnection.Close();
                    return;
                }
                else
                {
                    Console.WriteLine("No habits found.");
                    sqlConnection.Close();
                    return;
                }

            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                sqlConnection.Close();
                return;
            }
              
        }

        public static void CreateHabit(SQLiteConnection sqlConnection)
        {
            Console.Clear();
  
            try
            {
                Console.Write("Insert the habit name you want to start\nType cancel to exit\nSelect an option:  ");
                string habitName = Console.ReadLine();
                if(habitName.ToLower() == "cancel")
                {
                    sqlConnection.Close();
                    return;
                }
                string habitNameTable = habitName.ToLower().Replace(" ", "_");
                //string habitNameTable = habitName.Replace(" ", "_");
                //Console.WriteLine(habitNameTable + " --- Name Debugger");

                bool tableRepeated = CheckIfTableIsRepeated(sqlConnection, habitNameTable);

                if (tableRepeated)
                {
                    Console.WriteLine("The table exists!");
                    sqlConnection.Close();
                    return;
                }
                else
                {
                    string sqlSentence = $"CREATE TABLE IF NOT EXISTS {habitNameTable} (Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, DayDate TEXT, Quantity INTEGER)";

                    SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);

                    command.ExecuteNonQuery();

                    Console.WriteLine("Habit created!");
                    sqlConnection.Close();
                }
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                sqlConnection.Close();
                return;
            }
        }

        public static bool CheckIfTableIsRepeated(SQLiteConnection sqlConnection, string TableName)
        {

            //Old Veryfing method
            /*
            try
            {
                sqlConnection.Open();

                string sqlSentence = @"SELECT * FROM sqlite_master WHERE TYPE='table'";

                SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(TableName + "-- Table name Sent by Parameter");
                    Console.WriteLine(reader.GetString(1) + " -- Table name got by Command Execution");
                    if (TableName.Equals(reader.GetString(1), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;            //try
                {
                    sqlConnection.Open();

                    string sqlSentence = @"SELECT * FROM sqlite_master WHERE TYPE='table'";

                    SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        //Console.WriteLine(TableName + "-- Table name Sent by Parameter");
                        //Console.WriteLine(reader.GetString(1) + " -- Table name got by Command Execution");
                        if (TableName.Equals(reader.GetString(1), StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                    return false;
            */

        try
            {
                sqlConnection.Open();

                string sqlSentence = "SELECT name FROM sqlite_master WHERE TYPE='table' AND name = @name";

                SQLiteCommand command = new SQLiteCommand( sqlSentence, sqlConnection );

                command.Parameters.AddWithValue("@name", TableName);

                var reader = Convert.ToString(command.ExecuteScalar());

                if(TableName.Equals(reader, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return true;
            }
        }

        public static void InsertRecord(SQLiteConnection sqlConnection)
        {
            Console.Clear();
            TableExists(sqlConnection);

            ViewHabit(sqlConnection, false);

            try
            {
                Console.Write("Select the habit you want to make a record: ");
                string tableName = Console.ReadLine();

                Console.Write("Quantity: ");
                int tableQuantity = Int32.Parse(Console.ReadLine());

                string tableDayDate = DateTime.Now.ToString();

                //Console.WriteLine(tableName);
                //Console.WriteLine(tableQuantity);
                //Console.WriteLine(tableDayDate);

                string sqlSentence = $"INSERT INTO {tableName}(Quantity, dayDate) VALUES (@Quantity, @dayDate)";

                sqlConnection.Open();

                SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);

                command.Parameters.AddWithValue("@Quantity", tableQuantity);
                command.Parameters.AddWithValue("@dayDate", tableDayDate);

                command.ExecuteNonQuery();

                Console.WriteLine("Record created in "+ tableName + " with a total of "+ tableQuantity + " on date: "+ tableDayDate);

                sqlConnection.Close();

                return;

            } catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                sqlConnection.Close();
                return;
            }



        }
        public static void DeleteRecord(SQLiteConnection sqlConnection)
        {
            Console.Clear();
            TableExists(sqlConnection);
            int option = 1;
            try
            {
                do
                {
                    Console.Write("Type 0 to cancel\nType 1 to delete a habit\nType 2 to delete a record\nSelect an option: ");
                    option = Convert.ToInt32(Console.ReadLine());

                    switch (option)
                    {
                        case 0:
                            sqlConnection.Close();
                            break;
                        case 1:
                            DeleteHabit(sqlConnection);
                            option = 0;
                            break;
                        case 2:
                            DeleteRow(sqlConnection);
                            option = 0;
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("Select a valid option!");
                            sqlConnection.Close();
                            DeleteRecord(sqlConnection);
                            break;

                    }
                }while (option != 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                sqlConnection.Close();
                DeleteRecord(sqlConnection);
            }



        }

        public static void DeleteHabit(SQLiteConnection sqlConnection)
        {
            try
            {
                ViewHabit(sqlConnection, false);

                Console.Write("Write the habit name table: ");
                string habitTableName = Console.ReadLine();

                string sqlSentence = $"DROP TABLE {habitTableName}";

                sqlConnection.Open();

                SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);
                command.ExecuteNonQuery();

                Console.WriteLine($"{habitTableName} habit eliminated");

                sqlConnection.Close();
                return;
            }
            catch (Exception e)
            {
                sqlConnection.Close();
                Console.WriteLine(e.Message);
                return;
            }

        }
        
        public static void DeleteRow(SQLiteConnection sqlConnection)
        {
            try
            {
                ViewHabit(sqlConnection, false);

                Console.Write("Write the habit name table: ");
                string habitTableName = Console.ReadLine();

                sqlConnection.Open();

                ViewRecordsFromHabits(habitTableName, sqlConnection);

                Console.Write("Write the ID of the habit: ");
                int habitId = Convert.ToInt32(Console.ReadLine());


                string sqlSentence = $"DELETE FROM {habitTableName} WHERE Id = @id";

                sqlConnection.Open();

                SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);

                command.Parameters.AddWithValue("@id", habitId);

                command.ExecuteNonQuery();

                Console.WriteLine($"{habitId} row eliminated");

                sqlConnection.Close();
                return;

            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
                sqlConnection.Close();
                return;
            }

        }

        public static void UpdateRecord(SQLiteConnection sqlConnection)
        {
            TableExists(sqlConnection);

            try
            {
                ViewHabit(sqlConnection, false);

                Console.Write("Write the habit name table: ");
                string habitTableName = Console.ReadLine();

                sqlConnection.Open();

                ViewRecordsFromHabits(habitTableName, sqlConnection);

                Console.Write("Write the ID of the habit: ");
                int habitId = Convert.ToInt32(Console.ReadLine());

                Console.Write("Write the Quantity of the habit ID "+habitId+": ");
                int quantityUpdated = Convert.ToInt32(Console.ReadLine());

                string tableDayDate = DateTime.Now.ToString();


                string sqlSentence = $"UPDATE {habitTableName} SET Quantity = @quantity, dayDate = @datetoday WHERE Id = @id";

                sqlConnection.Open();

                SQLiteCommand command = new SQLiteCommand(sqlSentence, sqlConnection);

                command.Parameters.AddWithValue("@id", habitId);
                command.Parameters.AddWithValue("@quantity", quantityUpdated);
                command.Parameters.AddWithValue("@datetoday", tableDayDate);

                command.ExecuteNonQuery();

                Console.WriteLine($"{habitId} row updated");

                sqlConnection.Close();
                return;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                sqlConnection.Close();
                return;
            }
        }
    }
}