// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Common.Configuration
{
    using System.Configuration;

    public class FlowStudioConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("tools")]
        [ConfigurationCollection(typeof(ToolConfigurationElementCollection))]
        public ToolConfigurationElementCollection InitialTools
        {
            get { return (ToolConfigurationElementCollection) this["tools"]; }
        }

        public static FlowStudioConfigurationSection Current
        {
            get { return (FlowStudioConfigurationSection) ConfigurationManager.GetSection("flowStudio"); }
        }
    }
}
