using System.Windows.Forms;

namespace MemoryGame
{
    public partial class Results : Form
    {
        public Results(string name, string tries, string time)
        {
            InitializeComponent();

            // Set values to the textBoxes of the form "Results"
            textBox1.Text = name;
            textBox3.Text = tries;
            textBox4.Text = time;
        }
    }
}
