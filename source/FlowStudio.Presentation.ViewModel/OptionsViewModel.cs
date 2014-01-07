// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Input;
    using Data;
    using Extensions;
    using GalaSoft.MvvmLight.Command;
    using Messages;
    using Common;
    using Properties;

    public class OptionsViewModel : ViewModel
    {
        private readonly Settings settings = Settings.Default;

        private bool changesHereMade;

        public OptionsViewModel()
        {
            ReloadFromSettings();
        }

        private void ReloadFromSettings()
        {
            referencesPaths = new ObservableCollection<ReferencePathItem>();

            if (settings.ReferencesPaths != null)
            {
                foreach (var referencesPath in settings.ReferencesPaths)
                {
                    referencesPaths.Add(new ReferencePathItem(referencesPath));
                }
            }

            millisecondsBetweenDebugSteps = settings.MillisecondsBetweenDebugSteps >=
                                            MinimumMillisecondsBetweenDebugSteps
                                            &&
                                            settings.MillisecondsBetweenDebugSteps <=
                                            MaximumMillisecondsBetweenDebugSteps
                                                ? settings.MillisecondsBetweenDebugSteps
                                                : 1000;
        }

        private int millisecondsBetweenDebugSteps;

        public int MillisecondsBetweenDebugSteps 
        { 
            get { return millisecondsBetweenDebugSteps; }
            set
            {
                if (millisecondsBetweenDebugSteps != value)
                {
                    changesHereMade = true;
                    millisecondsBetweenDebugSteps = value;
                    RaisePropertyChanged("MillisecondsBetweenDebugSteps");
                }
            }
        }

        public int MinimumMillisecondsBetweenDebugSteps
        {
            get { return 1; }
        }

        public int MaximumMillisecondsBetweenDebugSteps
        {
            get { return int.MaxValue; }
        }

        private ObservableCollection<ReferencePathItem> referencesPaths;

        public ObservableCollection<ReferencePathItem> ReferencesPaths
        {
            get { return referencesPaths; }
            set { referencesPaths = value; }
        }

        private ICommand addReferencePathCommand;

        public ICommand AddReferencePathCommand
        {
            get
            {
                if (addReferencePathCommand == null)
                {
                    addReferencePathCommand = new RelayCommand(
                        () =>
                            {
                                var openMsg = new OpenDirectoryMessage(
                                    this,
                                    null,
                                    result =>
                                        {
                                            if (result.PerformOpen &&
                                                !referencesPaths.Any(r => r.DirectoryPath == result.DirectoryPath))
                                            {

                                                referencesPaths.Add(new ReferencePathItem(result.DirectoryPath));
                                                changesHereMade = true;
                                            }
                                        });

                                MessengerInstance.Send(openMsg);
                            });
                }

                return addReferencePathCommand;
            }
        }

        private ICommand removeSelectedReferencesPathsCommand;

        public ICommand RemoveSelectedReferencesPathsCommand
        {
            get
            {
                if (removeSelectedReferencesPathsCommand == null)
                {
                    removeSelectedReferencesPathsCommand = new RelayCommand(
                        () =>
                            {
                                ReferencesPaths.RemoveItems(ReferencesPaths.Where(r => r.IsSelected).ToList());
                                changesHereMade = true;
                            },
                        () => ReferencesPaths.Any(r => r.IsSelected));
                }

                return removeSelectedReferencesPathsCommand;
            }
        }

        private ICommand removeAllReferencesPathsCommand;

        public ICommand RemoveAllReferencesPathsCommand
        {
            get
            {
                if (removeAllReferencesPathsCommand == null)
                {
                    removeAllReferencesPathsCommand = new RelayCommand(
                        () =>
                            {
                                referencesPaths.Clear();
                                changesHereMade = true;
                            },
                        () => referencesPaths.Count > 0);
                }

                return removeAllReferencesPathsCommand;
            }
        }

        private ICommand closeOptionsCommand;

        public ICommand CloseOptionsCommmand
        {
            get
            {
                if (closeOptionsCommand == null)
                {
                    closeOptionsCommand = new RelayCommand(
                        () =>
                            {
                                // Reload model settings
                                ReloadFromSettings();
                                changesHereMade = false;

                                // Close dialog
                                MessengerInstance.Send(new CloseOptionsMessage());
                            });
                }

                return closeOptionsCommand;
            }
        }

        private ICommand saveOptionsCommand;

        public ICommand SaveOptionsCommand
        {
            get
            {
                if (saveOptionsCommand == null)
                {
                    saveOptionsCommand = new RelayCommand(
                        () =>
                            {
                                // Debug
                                settings.MillisecondsBetweenDebugSteps = MillisecondsBetweenDebugSteps;

                                // References Paths
                                settings.ReferencesPaths = new StringCollection();

                                foreach (var referencesPath in ReferencesPaths)
                                {
                                    settings.ReferencesPaths.Add(referencesPath.DirectoryPath);
                                }

                                // Save
                                settings.Save();

                                // Reload model with settings
                                ReloadFromSettings();
                                changesHereMade = false;

                                // Close dialog
                                MessengerInstance.Send(new CloseOptionsMessage());
                            },
                        () => changesHereMade);
                }

                return saveOptionsCommand;
            }
        }
    }
}
