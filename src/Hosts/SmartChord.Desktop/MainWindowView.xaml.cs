 namespace SmartChord.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView
    {
        public MainWindowView()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            InitializeComponent();
        }
    }
}
