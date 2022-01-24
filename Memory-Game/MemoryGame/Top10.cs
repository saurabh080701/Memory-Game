using System;
using System.Data.OleDb;
using System.Text;
using System.Windows.Forms;

namespace MemoryGame
{
    public partial class Top10 : Form
    {
        OleDbConnection conn; // Set connection with the DB

        public Top10()
        {
            InitializeComponent();
            find10();
        }

        // Find the TOP 10 records of the DB (the records are shorted in ascending order)
        internal void find10()
        {
            try
            {
                conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Memory.mdb;");
                conn.Open(); // Connection with DB

                // Select the top 10 records (users with less time and/or tries than others) in the DB shorted by Time and TimeInSec of each user
                OleDbCommand cmd = new OleDbCommand("SELECT TOP 10 Username,Tries,TimeInSec FROM Users ORDER BY TimeInSec,Tries;", conn);

                OleDbDataReader reader = cmd.ExecuteReader();

                StringBuilder sb = new StringBuilder(); // Make a StringBuilder object  

                if (reader.HasRows) // If there are records in the DB then show them to the user (add them in a textBox)
                {
                    int counter = 1;
                    while (reader.Read()) // It will be excecuted 10 times
                    {
                        sb.Append(counter.ToString());
                        sb.Append(". ");
                        sb.Append("Name: ");
                        sb.Append(reader.GetString(0));
                        sb.Append(" , ");
                        sb.Append("Time: ");
                        sb.Append(reader.GetInt16(2).ToString());
                        sb.Append(" , ");
                        sb.Append("Tries: ");
                        sb.Append(reader.GetInt16(1).ToString());
                        sb.Append(Environment.NewLine);

                        counter++;
                    }

                    textBox1.Text = sb.ToString(); // Add to a textBox the top 10 best players
                }

            }

            catch (Exception ex) // If a problem occurs then show a message to the user
            
            {
                MessageBox.Show("A problem with the database occured!");
                MessageBox.Show(ex.Message);
            }

            finally
            
            {
                conn.Close(); // Close connection with the DB
            }
        }

    }
}
