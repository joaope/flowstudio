namespace FlowStudio.Presentation.View
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using ViewModel.Data;

    public partial class AddActivitiesWindow
    {
        private readonly ObservableCollection<ActivityTypeItem> activitiesTypesItems;

        public ObservableCollection<ActivityTypeItem> ActivityTypeItems
        {
            get { return activitiesTypesItems; }
        }

        public IList<Type> SelectedActivitiesTypes
        {
            get
            {
                return activitiesTypesItems
                    .Where(a => a.IsSelected)
                    .Select(a => a.ActivityType)
                    .ToList();
            }
        }

        public AddActivitiesWindow(IEnumerable<Type> activitiesTypes)
        {
            InitializeComponent();

            activitiesTypesItems = new ObservableCollection<ActivityTypeItem>(
                activitiesTypes.Select(a => new ActivityTypeItem(a)).OrderBy(a => a.ActivityType.Name));

            DataContext = this;
        }

        private ICommand addCommand;

        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                {
                    addCommand = new RelayCommand(
                        () =>
                        {
                            DialogResult = true;
                            Close();
                        },
                        () => SelectedActivitiesTypes.Count > 0);
                }

                return addCommand;
            }
        }

        private bool activitiesSelectionToggle;

        public bool ActivitiesSelectionToggle
        {
            get { return activitiesSelectionToggle; }
            set
            {
                activitiesSelectionToggle = value;

                foreach (var activityTypeItem in ActivityTypeItems)
                {
                    activityTypeItem.IsSelected = value;
                }
            }
        }
    }
}
