namespace FlowStudio.Presentation.View
{
    using FlowStudio.Common;
    using GalaSoft.MvvmLight.Messaging;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using ViewModel;
    using ViewModel.Messages;
    using MessageBox = System.Windows.MessageBox;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
    using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
    using TextBox = System.Windows.Controls.TextBox;

    public partial class MainWindow : IMessengerAware
    {
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Register<SaveBeforeCloseMessage>(this, OnSaveBeforeCloseMessage);
            Messenger.Register<PerformCloseProgramMessage>(this, OnPerformCloseProgramMessage);
            Messenger.Register<SaveFileMessage>(this, OnSaveFileMessage);
            Messenger.Register<OpenFileMessage>(this, OnOpenFileMessage);
            Messenger.Register<OpenOptionsMessage>(this, OnOpenOptionsMessage);
            Messenger.Register<OpenDirectoryMessage>(this, OnOpenDirectoryMessage);
            Messenger.Register<SelectActivitiesTypesMessage>(this, OnSelectActivitiesTypesMessage);
            Messenger.Register<ClosingWhenExecutingMessage>(this, OnClosingWhenExecutingMessage);
            Messenger.Register<OpenAboutDialogMessage>(this, OnOpenAboutDialogMessage);
        }

        private void OnOpenAboutDialogMessage(OpenAboutDialogMessage openAboutDialogMessage)
        {
            var dlg = new AboutWindow
            {
                Owner = this
            };

            dlg.ShowDialog();
        }

        private void OnClosingWhenExecutingMessage(ClosingWhenExecutingMessage closingWhenExecutingMessage)
        {
            MessageBox.Show(
                "There are workflows executing/debugging.\n\nClose then to continue.",
                "Flow Studio",
                MessageBoxButton.OK,
                MessageBoxImage.Hand,
                MessageBoxResult.OK);
        }

        private void OnSelectActivitiesTypesMessage(SelectActivitiesTypesMessage selectActivitiesTypesMessage)
        {
            var addDlg = new AddActivitiesWindow(selectActivitiesTypesMessage.AvailableActivitiesTypes)
            {
                Owner = this
            };

            selectActivitiesTypesMessage.Execute(addDlg.ShowDialog().GetValueOrDefault()
                                                     ? new SelectActivitiesTypesResult(addDlg.SelectedActivitiesTypes)
                                                     : SelectActivitiesTypesResult.DoNothing);
        }

        private void OnOpenDirectoryMessage(OpenDirectoryMessage openDirectoryMessage)
        {
            var folderDlg = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            openDirectoryMessage.Execute(folderDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
                                             ? new OpenDirectoryResult(folderDlg.SelectedPath)
                                             : OpenDirectoryResult.DoNothing);
        }

        private void OnOpenOptionsMessage(OpenOptionsMessage openOptionsMessage)
        {
            var optDlg = new OptionsWindow
            {
                Owner = this
            };

            optDlg.ShowDialog();
        }

        private void OnOpenFileMessage(OpenFileMessage openFileMessage)
        {
            var openDlg = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                Multiselect = openFileMessage.Multiselect,
                Filter = "All Supported|*.xaml;*.xamlx|Activities|*.xaml|Workflow Services|*.xamlx"
            };

            if (openDlg.ShowDialog().GetValueOrDefault())
            {
                openFileMessage.Execute(openDlg.Multiselect
                                            ? new OpenFileResult(openDlg.FileNames)
                                            : new OpenFileResult(openDlg.FileName));
            }
            else
            {
                openFileMessage.Execute(OpenFileResult.DoNothing);
            }
        }

        private static void OnSaveFileMessage(SaveFileMessage saveFileMessage)
        {
            var saveDlg = new SaveFileDialog
            {
                Title = "Save - FlowStudio",
                FileName = saveFileMessage.WorkflowFileInfo.FilePath,
                CheckFileExists = false,
                AddExtension = true,
                Filter = "Activity|*.xaml|Workflow Service|*.xamlx",
                FilterIndex = saveFileMessage.WorkflowFileInfo.WorkflowType == WorkflowType.Activity ? 1 : 2
            };

            var r = saveDlg.ShowDialog();

            saveFileMessage.Execute(r.GetValueOrDefault()
                                        ? new SaveFileResult(saveDlg.FileName)
                                        : SaveFileResult.DoNothing);
        }

        private void OnPerformCloseProgramMessage(PerformCloseProgramMessage closeProgramMessage)
        {
            Close();
        }

        private static void OnSaveBeforeCloseMessage(SaveBeforeCloseMessage saveBeforeCloseMessage)
        {
            var msg = new StringBuilder("Save changes to the following workflows?");

            msg.AppendLine();
            msg.AppendLine();

            foreach (var workflowInfo in saveBeforeCloseMessage.WorkflowsInfos)
            {
                var never = workflowInfo.IsDirty
                    ? " (never saved before)"
                    : string.Empty;

                msg.AppendLine(string.Concat(workflowInfo.Title, never));
            }

            var result = MessageBox.Show(
                msg.ToString(),
                "Save Confirmation - FlowStudio",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel);

            var questionResult = QuestionResult.Cancel;

            if (result == MessageBoxResult.Yes)
            {
                questionResult = QuestionResult.Yes;
            }
            else if (result == MessageBoxResult.No)
            {
                questionResult = QuestionResult.No;
            }

            saveBeforeCloseMessage.Execute(questionResult);
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                textBox.ScrollToEnd();
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Messenger.Send(new RequestCloseProgramMessage(
                this,
                null,
                result =>
                {
                    if (!result)
                    {
                        e.Cancel = true;
                    }
                }));
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public IMessenger Messenger
        {
            get { return TypeLocator.GetInstance<IMessenger>(); }
        }
    }
}
