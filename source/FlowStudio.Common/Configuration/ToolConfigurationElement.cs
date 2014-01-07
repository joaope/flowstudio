// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: http://flowstudio.codeplex.com

namespace FlowStudio.Common.Configuration
{
    using System;
    using System.Configuration;

    public class ToolConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("category")]
        public ToolCategoryType? Category
        {
            get { return this["category"] as ToolCategoryType?; }
            set { this["category"] = value; }
        }

        public Type GetToolType()
        {
            return Type.GetType(Name, false);
        }
    }
}
