namespace FlowStudio.Presentation.Common
{
    using System.Windows;
    using FlowStudio.Common;
    using GalaSoft.MvvmLight.Messaging;

    public class MessengerAwareWindow : Window, IMessengerAware
    {
        public IMessenger Messenger
        {
            get { return TypeLocator.GetInstance<IMessenger>(); }
        }
    }
}
