namespace Blackjack
{
    /// <summary>
    /// Interaction logic for Double.xaml
    /// </summary>
    public partial class Double
    {
        public Double()
        {
            InitializeComponent();
            BtnSubmit.Click += (sender, args) => DialogResult = true;
        }
    }
}
