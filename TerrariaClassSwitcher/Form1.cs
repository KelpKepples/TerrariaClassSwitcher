namespace TerrariaClassSwitcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            MainScreen ms = new MainScreen();
            this.Controls.Add(ms);
        }
    }
}