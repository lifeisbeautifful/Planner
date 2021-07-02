
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    
    public partial class Form1 : Form
    {
        SqlConnection sqlConnection;
        public static DateTime userDate { get; set; }
        public static DateTime date = DateTime.Now;
        
        public static int Month = date.Month;
        public static int Year = date.Year;

        Color red = Color.FromName("Red");
        Color grey = Color.FromName("LightGray");
        Color white = Color.FromName("White");

        DateTime start = new DateTime(Year, Month, 1);
        public int countDays = DateTime.DaysInMonth(Year, Month);

        public Form1()
        {
            InitializeComponent();
        }

        public async void Form1_Load(object sender, EventArgs e)
        {
           sqlConnection = new SqlConnection(DBQueries.connection);
           await sqlConnection.OpenAsync();
          
            for (int i = 1; i < 32; i++)
            {
                (Controls["label" + i] as Label).Text = start.ToShortDateString();

                if (start.Month > Month)
                {
                    (Controls["label" + i] as Label).Visible = false;
                    (Controls["richTextBox" + i] as RichTextBox).Visible = false;
                }
                if (start > date.AddDays(-1))
                {
                    if ((int)start.DayOfWeek == 0 || (int)start.DayOfWeek == 6)
                    {
                        (Controls["richTextBox" + i] as RichTextBox).BackColor = red;
                    }
                }
                else
                {
                    (Controls["richTextBox" + i] as RichTextBox).BackColor = grey;
                }
                start = start.AddDays(1);
            }
           
            SqlSelect((Controls["label1"] as Label).Text, (Controls["label31"] as Label).Text);
        }
        private void CalcLoad(object sender, EventArgs e)
        {
            int load = 0;

            for (int i = 1; i <= countDays; i++)
            {
                if ((Controls["richTextBox" + i] as RichTextBox).Text.Equals(""))
                {
                    load++;
                }
            }

            int busyHours = (countDays - load) * 8;
            int freeHours = load * 8;

            textBox1.Text = "Free days: " + load.ToString() + ", Hours: " + freeHours;
            textBox2.Text = "Busy days: " + (countDays - load).ToString() + ", Hours: " + busyHours;
        }
     
        private void NavigateToChosenDate(object sender, EventArgs e)
        {
            try
            {
                userDate = DateTime.Parse(this.textBox3.Text);
                int userMonth = userDate.Month + 1;
                SqlSelect((Controls["label1"] as Label).Text, (Controls["label31"] as Label).Text);

                countDays = DateTime.DaysInMonth(userDate.Year, userDate.Month);
                textBox1.Text = "";
                textBox2.Text = "";

                for (int i = 1; i < 32; i++)
                {
                    if (userDate.Month < userMonth)
                    {
                        (Controls["label" + i] as Label).Text = userDate.ToShortDateString();
                        (Controls["label" + i] as Label).Visible = true;
                        (Controls["richTextBox" + i] as RichTextBox).Visible = true;
                        (Controls["richTextBox" + i] as RichTextBox).Text = "";

                        if (userDate < DateTime.Now)
                        {
                            (Controls["richTextBox" + i] as RichTextBox).BackColor = grey;
                        }
                        else
                        {
                            if ((int)userDate.DayOfWeek == 0 || (int)userDate.DayOfWeek == 6)
                            {
                                (Controls["richTextBox" + i] as RichTextBox).BackColor = red;
                            }
                            else
                            {
                                (Controls["richTextBox" + i] as RichTextBox).BackColor = white;
                            }
                        }
                    }
                    else
                    {
                        (Controls["label" + i] as Label).Visible = false;
                        (Controls["richTextBox" + i] as RichTextBox).Visible = false;
                    }
                    userDate = userDate.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Enter year and month as following: 2021,7");
            }
        }

        private void buttonSaveClick(object sender, EventArgs e)
        {
            SqlUpdate();
            SelectIfNeedInsert((Controls["label1"] as Label).Text, (Controls["label31"] as Label).Text);

           for (int i = 1; i < 32; i++)
            {
                if ((Controls["richTextBox" + i] as RichTextBox).Text == "")
                {
                    SqlDelete((Controls["label" + i] as Label).Text);
                }
            }
        }
       
         private async void SqlSelect(string start,string end)
        {
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand(DBQueries.Select, sqlConnection);
            command.Parameters.AddWithValue("Start", start.ToString());
            command.Parameters.AddWithValue("End", end.ToString());
            sqlReader = await command.ExecuteReaderAsync();

            while (await sqlReader.ReadAsync())
            {
                for (int i = 1; i < 32; i++)
                {
                    if ((Controls["label" + i] as Label).Text == Convert.ToString(sqlReader["Dates"]))
                    {
                        (Controls["richTextBox" + i] as RichTextBox).Text = Convert.ToString(sqlReader["Note"]);
                    }
                }
            }
            sqlReader.Close(); 
        }

        private void SelectIfNeedInsert(string startDate,string endDate)
        {
            string[] dates = new string[40];
            int counter1 = 0;

            SqlCommand command = new SqlCommand(DBQueries.Select, sqlConnection);
            command.Parameters.AddWithValue("Start", startDate.ToString());
            command.Parameters.AddWithValue("End", endDate.ToString());
            SqlDataReader sqlReader = null;
            sqlReader = command.ExecuteReader();

            while (sqlReader.Read())
            {
                for (int i = 1; i < 32; i++)
                {
                    if ((Controls["label" + i] as Label).Text == Convert.ToString(sqlReader["Dates"]))
                    {
                        dates[counter1] = (Controls["label" + i] as Label).Text;
                        counter1++;
                    }
                }
            }
            sqlReader.Close();

            for (int i = 1; i < 32; i++)
            {
                if ((Controls["richTextBox" + i] as RichTextBox).Text != "")
                {
                    for (int j = 0; j < dates.Length; j++)
                    {
                        if ((Controls["label" + i] as Label).Text == dates[j])
                        {
                            break;
                        }
                        else if (j == dates.Length - 1)
                        {
                            SqlInsert((Controls["label" + i] as Label).Text, (Controls["richTextBox" + i] as RichTextBox).Text);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private void SqlUpdate()
        {
            for (int i = 1; i < 32; i++)
            {
                if ((Controls["richTextBox" + i] as RichTextBox).Text != "")
                {
                    SqlCommand command = new SqlCommand(DBQueries.Update, sqlConnection);
                    command.Parameters.AddWithValue("Dates", (Controls["label" + i] as Label).Text);
                    command.Parameters.AddWithValue("Note", (Controls["richTextBox" + i] as RichTextBox).Text);
                    command.ExecuteNonQuery();
                }
            }
        }
        private void SqlInsert(string date,string note)
        {
            SqlCommand command3 = new SqlCommand(DBQueries.Insert, sqlConnection);
            command3.Parameters.AddWithValue("Dates", date);
            command3.Parameters.AddWithValue("Note", note);
            command3.ExecuteNonQuery();
        }
        private void SqlDelete(string date)
        {
            SqlCommand command4 = new SqlCommand(DBQueries.Delete, sqlConnection);
            command4.Parameters.AddWithValue("Dates", date);
            command4.ExecuteNonQuery();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                 sqlConnection.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void richTextBox1_TextChanged(object sender, EventArgs e) { }
        private void richTextBox18_TextChanged(object sender, EventArgs e) { }
        private void richTextBox29_TextChanged(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e) { }
        private async void button2_Click_1(object sender, EventArgs e) { }
    }
}

    
    

   