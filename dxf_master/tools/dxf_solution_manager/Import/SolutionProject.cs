/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace com.dxfeed.master.tools.Import
{
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class SolutionProject
    {
        static readonly Type s_ProjectInSolution;
        static readonly PropertyInfo s_ProjectInSolution_Dependencies;
        static readonly PropertyInfo s_ProjectInSolution_ParentProjectGuid;
        static readonly PropertyInfo s_ProjectInSolution_ProjectName;
        static readonly PropertyInfo s_ProjectInSolution_RelativePath;
        static readonly PropertyInfo s_ProjectInSolution_ProjectGuid;
        static readonly PropertyInfo s_ProjectInSolution_ProjectType;

        static SolutionProject()
        {
            s_ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            if (s_ProjectInSolution != null)
            {
                s_ProjectInSolution_Dependencies = s_ProjectInSolution.GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ParentProjectGuid = s_ProjectInSolution.GetProperty("ParentProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectName = s_ProjectInSolution.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_RelativePath = s_ProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectType = s_ProjectInSolution.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        public IReadOnlyList<string> Dependencies { get; private set; }
        public string ParentProjectGuid { get; private set; }
        public string ProjectName { get; private set; }
        public string RelativePath { get; private set; }
        public string ProjectGuid { get; private set; }
        public string ProjectType { get; private set; }

        public SolutionProject(object solutionProject)
        {
            Dependencies = s_ProjectInSolution_Dependencies.GetValue(solutionProject, null) as IReadOnlyList<string>;
            ParentProjectGuid = s_ProjectInSolution_ParentProjectGuid.GetValue(solutionProject, null) as string;
            ProjectName = s_ProjectInSolution_ProjectName.GetValue(solutionProject, null) as string;
            RelativePath = s_ProjectInSolution_RelativePath.GetValue(solutionProject, null) as string;
            ProjectGuid = s_ProjectInSolution_ProjectGuid.GetValue(solutionProject, null) as string;
            ProjectType = s_ProjectInSolution_ProjectType.GetValue(solutionProject, null).ToString();
        }
    }
}
