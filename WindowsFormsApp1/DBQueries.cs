using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class DBQueries : Form1
    {
        public static string connection = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Oksana\source\repos\WindowsFormsApp1\WindowsFormsApp1\Database1.mdf;Integrated Security=True";
        public static string Select = @"SELECT Dates, Note FROM Notes WHERE Dates between @Start and @End order by Dates";
        public static string Update = @"UPDATE [Notes] SET [Note]=@Note WHERE [Dates]=@Dates";
        public static string Insert = @"INSERT INTO [Notes] (Dates,Note)VALUES(@Dates,@Note)";
        public static string Delete = @"DELETE FROM [Notes] WHERE [Dates]=@Dates";
    }
}
