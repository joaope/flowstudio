// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Presentation.View
{
    using System.Windows;
    using System.Windows.Data;

    // http://www.codeproject.com/Articles/27432/Artificial-Inheritance-Contexts-in-WPF
    public class DataContextSpy : Freezable
    {
        public DataContextSpy()
        {
            BindingOperations.SetBinding(this, DataContextProperty, new Binding());

            IsSynchronizedWithCurrentItem = true;
        }

        public bool IsSynchronizedWithCurrentItem { get; set; }

        public object DataContext
        {
            get { return GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        public static readonly DependencyProperty DataContextProperty =
            FrameworkElement.DataContextProperty.AddOwner(
                typeof (DataContextSpy),
                new PropertyMetadata(null, null, OnCoerceDataContext));

        private static object OnCoerceDataContext(DependencyObject depObj, object value)
        {
            var spy = depObj as DataContextSpy;
            if (spy == null)
            {
                return value;
            }

            if (spy.IsSynchronizedWithCurrentItem)
            {
                var view = CollectionViewSource.GetDefaultView(value);
                if (view != null)
                    return view.CurrentItem;
            }

            return value;
        }

        protected override Freezable CreateInstanceCore()
        {
            return null;
        }
    }
}