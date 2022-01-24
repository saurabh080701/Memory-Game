using System.Windows.Forms;
using System.Data.OleDb;
using System;
using System.Media;

namespace MemoryGame
{
    class User
    {
        // Set connection with the DB
        OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Memory.mdb;");

        // Object that plays the sound of an error
        SoundPlayer player = new SoundPlayer("error_sound.wav");

        bool flag = false; // If it is true, then update the DB, if it is false, then insert to the DB

        // When the user finishes playing, his results will be written to the DB (if the name does not exist in the DB).
        // If the name exists already in the DB, then the DB will be updated only if PREVIOUS time of the player is 
        // WORSE than this one in the "time" variable. So, the time variable represents the "new" time of the user.
        // If the value in the "time" is worse that the previous time of the user, the DB will be not updated.
        int time = -1;

        // Constructor is visible only within the same namespace
        internal User()
        {

        }

        void updateDB(int tries,int time,string name)
        {
                try
                {
                    conn.Open(); // Open connection with the DB
                    OleDbCommand command1 = new OleDbCommand("UPDATE Users SET Tries=@tries,TimeInSec=@time WHERE Username=@name;", conn);
                    command1.Parameters.AddWithValue("@tries", tries);
                    command1.Parameters.AddWithValue("@time", time);
                    command1.Parameters.AddWithValue("@name", name);

                    command1.Connection = conn;
                    command1.ExecuteNonQuery(); // Excecute the query

                }
                catch (Exception e)
                {
                    player.Play(); // Play an "error" sound

                    // If an error occurs, show a message to the user
                    MessageBox.Show("Problem with the database!");
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    conn.Close(); // Close the conection with the DB
                }
            
        }

        void readFromDb(string name)
        {
            try
            {
                conn.Open(); // Open connection with the DB

                // Check if the name of User is already in the DB
                OleDbCommand command = new OleDbCommand("SELECT TimeInSec FROM Users WHERE Username=@user;", conn);
                command.Parameters.AddWithValue("@user", name);

                // Read the records
                OleDbDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // If there is such a name in the DB, then use an UPDATE query
                {
                    flag = true;

                    while (reader.Read())
                    {
                        this.time = reader.GetInt16(0); // Read the previous time of the user.
                    }

                    reader.Close(); // Close the reader
                }
            }

            catch (Exception e)
            
            {
                player.Play(); // Play an "error" sound

                // If an error occurs, show a message to the user
                MessageBox.Show("Problem with the database!");
                MessageBox.Show(e.Message);
            }

            finally
            
            {
                conn.Close(); // Close the conection with the DB
            }
        }

        
        // Adds to a DB all the characteristics of the user who plays the game
        void addToDB(string name, int tries, int time)
        {
                try
                {
                    conn.Open(); // Open connection with the DB
                    OleDbCommand cmd = new OleDbCommand();

                    // Insert query
                    cmd.CommandText = @"INSERT INTO Users (Username,Tries,TimeInSec) VALUES (@userName,@tries,@time)";

                    // Add this values in the same record
                    cmd.Parameters.AddWithValue("@userName", name);
                    cmd.Parameters.AddWithValue("@tries", tries);
                    cmd.Parameters.AddWithValue("@time", time);

                    cmd.Connection = conn;

                    int i = cmd.ExecuteNonQuery();

                    if (i != 1) // If the Query was not executed then show a message to the user
                    {
                        MessageBox.Show("The Database was not updated!");
                    }
                }
                catch (Exception ex) // If a problem occurs then show a message to the user
                {
                    player.Play(); // Play an "error" sound

                    MessageBox.Show("A problem with the database occured!");
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close(); // Close connection with the DB
                }
            
        }

        internal void databaseOperations(string name, int tries, int time)
        {
            readFromDb(name); // Check the DB if user exists (Username field is UNIQUE)

            if (flag == true) // If found such a name in the DB then update the record
            {
                // Update the DB only if the new time is less that the previous time of the user 
                if (this.time >= time)
                {
                    updateDB(tries, time, name);
                }
            }
            else // If not found such a name in the DB then insert new record in the DB
            {
                addToDB(name, tries, time);
            }

            flag = false;
            this.time = -1;
        }
    }
}
