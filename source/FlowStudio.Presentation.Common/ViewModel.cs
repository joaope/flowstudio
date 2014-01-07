namespace FlowStudio.Presentation.Common
{
    using FlowStudio.Common;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Messaging;

    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            MessengerInstance = TypeLocator.GetInstance<IMessenger>();
        }
    }
}
