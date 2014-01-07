namespace FlowStudio.Presentation.View
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Navigation;

    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
            Close();
        }
    }
}
