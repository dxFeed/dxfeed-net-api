/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Reflection;

namespace com.dxfeed.master.tools.Import
{
    public class SolutionConfiguration
    {
        static readonly Type s_ConfigInSolution;
        static readonly PropertyInfo configInSolution_configurationname;
        static readonly PropertyInfo configInSolution_fullName;
        static readonly PropertyInfo configInSolution_platformName;

        static SolutionConfiguration()
        {
            s_ConfigInSolution = Type.GetType("Microsoft.Build.Construction.ConfigurationInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            if (s_ConfigInSolution != null)
            {
                configInSolution_configurationname = s_ConfigInSolution.GetProperty("ConfigurationName", BindingFlags.NonPublic | BindingFlags.Instance);
                configInSolution_fullName = s_ConfigInSolution.GetProperty("FullName", BindingFlags.NonPublic | BindingFlags.Instance);
                configInSolution_platformName = s_ConfigInSolution.GetProperty("PlatformName", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        public string ConfigurationName { get; private set; }
        public string FullName { get; private set; }
        public string PlatformName { get; private set; }

        public SolutionConfiguration(object solutionConfiguration)
        {
            ConfigurationName = configInSolution_configurationname.GetValue(solutionConfiguration, null) as string;
            FullName = configInSolution_fullName.GetValue(solutionConfiguration, null) as string;
            PlatformName = configInSolution_platformName.GetValue(solutionConfiguration, null) as string;
        }
    }
}
