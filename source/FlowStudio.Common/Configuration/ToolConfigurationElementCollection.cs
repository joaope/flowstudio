// Copyright (C) 2013 João Pedro Correia - All Rights Reserved
// Code under the terms of the GPLv2 license
// URL: https://github.com/JoaoPe/flowstudio

namespace FlowStudio.Common.Configuration
{
    using System.Configuration;

    public class ToolConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ToolConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ToolConfigurationElement) element).Name;
        }

        public ToolConfigurationElement this[int index]
        {
            get { return (ToolConfigurationElement)BaseGet(index); }
        }
    }
}
