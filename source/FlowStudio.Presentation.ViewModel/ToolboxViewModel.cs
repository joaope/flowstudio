// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Presentation.ViewModel
{
    using System;
    using System.Activities;
    using System.Activities.Presentation;
    using System.Activities.Presentation.Toolbox;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FlowStudio.Common.Configuration;
    using Messages;
    using Properties;

    public class ToolboxViewModel : ToolViewModel
    {
        public const string ToolboxContentId = "ToolboxTool";

        private bool isAddingActivities;

        public ToolboxViewModel()
            : base(ToolboxContentId, "Toolbox")
        {
            InitializeDefaultCategories();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        private ObservableCollection<ToolboxCategory> categories = new ObservableCollection<ToolboxCategory>();

        public ObservableCollection<ToolboxCategory> Categories
        {
            get { return categories; }
            set { Set(() => Categories, ref categories, value); }
        }

        public void AddReference()
        {
            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            worker.ProgressChanged += (sender, args) => StatusViewModel.SetProgressValue(args.ProgressPercentage, 100);

            worker.DoWork += (sender, args) =>
            {
                StatusViewModel.SetProgressText("Searching activities on references directories");

                var directories = Settings.Default.ReferencesPaths;
                var actitiesTypes = new List<Type>();

                if (directories != null)
                {
                    for (var i = 0; i < directories.Count; i++)
                    {
                        worker.ReportProgress(i * 100 / directories.Count);

                        var directory = directories[i];
                        var fileNames = Directory.GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly);

                        foreach (var fileName in fileNames)
                        {
                            try
                            {
                                var assembly = Assembly.LoadFile(fileName);

                                actitiesTypes.AddRange(assembly.GetExportedTypes().Where(IsValidActivityType));
                            }
                            catch (BadImageFormatException)
                            {
                            }
                            catch (ReflectionTypeLoadException)
                            {
                            }
                            catch (TypeLoadException)
                            {
                            }
                        }
                    }

                    args.Result = actitiesTypes;
                }
            };

            worker.RunWorkerCompleted += (o, eventArgs) =>
            {
                var activitiesTypes = eventArgs.Result as IEnumerable<Type>;

                var selectMsg = new SelectActivitiesTypesMessage(
                    activitiesTypes,
                    this,
                    null,
                    result =>
                    {
                        if (result.PerformAdd)
                        {
                            foreach (var activityType in result.SelectedActivitiesTypes)
                            {
                                AddActivity(activityType);
                            }
                        }
                    });

                StatusViewModel.HideProgress();
                MessengerInstance.Send(selectMsg);
            };

            worker.RunWorkerAsync();
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender,
                                                ResolveEventArgs args)
        {
            if (isAddingActivities)
            {
                var name = new AssemblyName(args.Name);

                foreach (var path in Settings.Default.ReferencesPaths)
                {
                    var fileName = Path.Combine(path, string.Concat(name.Name, ".dll"));

                    if (File.Exists(fileName))
                    {
                        return Assembly.LoadFrom(fileName);
                    }
                }
            }

            return null;
        }

        private void InitializeDefaultCategories()
        {
            var toolsByCategory = FlowStudioConfigurationSection
                .Current
                .InitialTools
                .Cast<ToolConfigurationElement>()
                .GroupBy(element => element.Category)
                .Select(g => g.Key != null ? new
                {
                    Category = g.Key.Value.ToString(),
                    ToolsTypes = g.Select(t => t.GetToolType())
                } : null)
                .OrderBy(e => e.Category);

            foreach (var category in toolsByCategory)
            {
                AddCategory(category.Category, category.ToolsTypes);
            }
        }

        private ToolboxCategory GetCategory(string categoryName)
        {
            var cat = Categories
                .SingleOrDefault(c => c.CategoryName == categoryName);

            if (cat != null)
            {
                return cat;
            }

            cat = new ToolboxCategory(categoryName);

            Categories.Add(cat);

            return cat;
        }

        public void AddCategory(string categoryName, IEnumerable<Type> activitiesTypes)
        {
            var category = GetCategory(categoryName);

            foreach (var activityType in activitiesTypes)
            {
                var splitName = activityType.Name.Split('`');
                var displayName = splitName.Length == 1 ? activityType.Name : string.Format("{0}<T>", splitName[0]);

                category.Add(new ToolboxItemWrapper(activityType.FullName, activityType.Assembly.FullName, null, displayName));
            }
        }

        public void AddActivity(Type activityType)
        {
            var category = GetCategory(activityType.Namespace);

            if (category.Tools.All(w => w.ToolName != activityType.FullName))
            {
                isAddingActivities = true;
                category.Add(new ToolboxItemWrapper(activityType.FullName, activityType.Assembly.FullName, null, activityType.Name));
                isAddingActivities = false;
            }
        }

        public static bool IsValidActivityType(Type activityType)
        {
            return activityType.IsPublic &&
                !activityType.IsNested &&
                !activityType.IsAbstract &&
                (typeof(Activity).IsAssignableFrom(activityType) || typeof(IActivityTemplateFactory).IsAssignableFrom(activityType));
        }
    }
}
