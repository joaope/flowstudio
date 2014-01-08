namespace FlowStudio.Presentation.View
{
    using ViewModel.Messages;

    public partial class OptionsWindow
    {
        public OptionsWindow()
        {
            InitializeComponent();

            Messenger.Register<CloseOptionsMessage>(this, OnCloseOptionsMessage);
        }

        private void OnCloseOptionsMessage(CloseOptionsMessage closeOptionsMessage)
        {
            Close();
        }
    }
}
