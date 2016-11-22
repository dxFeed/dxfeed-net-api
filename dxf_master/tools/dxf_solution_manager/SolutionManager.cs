/// Copyright (C) 2010-2016 Devexperts LLC
///
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// http://mozilla.org/MPL/2.0/.

using com.dxfeed.master.tools.Import;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace com.dxfeed.master.tools
{
    /// <summary>
    /// This class modifies the copy of solution file by filtering required 
    /// projects.
    /// The source solution file (.sln) specifies by ParamSource command line 
    /// parameter and not modified while program working.
    /// If you want filter projects in resulting solution file specify the 
    /// list through ParamProjects command line parameter.If ParamProjects 
    /// is empty all projects from source solution will be included into
    /// output solution file.
    /// Overall exclude project list configures with ParamExclude command line 
    /// parameter.
    /// The output file or directory specified with ParamOut.
    /// </summary>
    public class SolutionManager
    {
        private static readonly string AssemblyLocationPath = 
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string TemplatesLocationPath = Path.Combine(
            AssemblyLocationPath, "Templates");
        private static readonly string SolutionTemplatePath = Path.Combine(
            TemplatesLocationPath, "SolutionTemplate.txt");
        private static readonly string ProjectRowTemplatePath = Path.Combine(
            TemplatesLocationPath, "ProjectRowTemplate.txt");
        private static readonly string ProjectConfigRowTemplatePath = Path.Combine(
            TemplatesLocationPath, "ProjectConfigRowTemplate.txt" );
        private static readonly string ProjectParentRowTemplatePath = Path.Combine(
            TemplatesLocationPath, "ProjectParentRowTemplate.txt" );

        private const int CodeSuccess = 0;
        private const int CodeError = 1;
        /// <summary>
        /// Absolute or relative path to Visual Studio solution file (.sln).
        /// </summary>
        private const string ParamSource = "/source:";
        /// <summary>
        /// The list of include projects - the list of project names separated 
        /// with SubParamSeparator without spaces and other signs.
        /// </summary>
        private const string ParamProjects = "/projects:";
        /// <summary>
        /// The list of eXclude projects - the list of project names separated 
        /// with SubParamSeparator without spaces and other signs.
        /// </summary>
        private const string ParamExclude = "/exclude:";
        /// <summary>
        /// The path to output solution file or directory. If parameter is empty
        /// the result solution file will be located in assembly directory with 
        /// the same name as source solution file. If you specify by this 
        /// parameter the output directory the source solution file name will be 
        /// used.
        /// </summary>
        private const string ParamOut = "/out:";
        private const char SubParamSeparator = ';';
        /// <summary>
        /// Set of only complile projects (without solution directories and etc.).
        /// </summary>
        private static readonly ISet<string> codeProjectsTypeSet = new SortedSet<string>(new string[] { "KnownToBeMSBuildFormat" });
        /// <summary>
        /// The map of project types to microsoft type guid.
        /// </summary>
        private static readonly IDictionary<string, string> projectTypeToGuid = new Dictionary<string, string> {
            { "KnownToBeMSBuildFormat", "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}" },
            { "SolutionFolder", "{2150E333-8FDC-42A3-9474-1A3956D46DE8}" }
        };

        private static string GetGuidByProjectType(string projectType)
        {
            string guid;
            if (!projectTypeToGuid.TryGetValue(projectType, out guid))
                throw new InvalidOperationException(string.Format("The project type '{0}' is unknown!", projectType));
            return guid;
        }

        static int Main(string[] args)
        {
            string sourceSolutionPath = string.Empty;
            ISet<string> includeProjectNames = new SortedSet<string>();
            ISet<string> excludeProjectNames = new SortedSet<string>();
            string outSolutionPath = string.Empty;
            foreach (string arg in args)
            {
                if (arg.StartsWith(ParamSource))
                    sourceSolutionPath = arg.Substring(ParamSource.Length);
                else if (arg.StartsWith(ParamProjects))
                    includeProjectNames.UnionWith(arg.Substring(ParamProjects.Length).Split(SubParamSeparator));
                else if (arg.StartsWith(ParamExclude))
                    excludeProjectNames.UnionWith(arg.Substring(ParamExclude.Length).Split(SubParamSeparator));
                else if (arg.StartsWith(ParamOut))
                    outSolutionPath = arg.Substring(ParamOut.Length);
            }

            if (string.IsNullOrEmpty(sourceSolutionPath))
                throw new ArgumentException(string.Format("The source solution file is not specified! You must run with '{0}' parameter.", ParamSource));

            if (string.IsNullOrEmpty(outSolutionPath))
                outSolutionPath = Path.GetFileName(sourceSolutionPath);
            else if (Path.GetFileName(outSolutionPath) == string.Empty)
                outSolutionPath = Path.Combine(outSolutionPath, Path.GetFileName(sourceSolutionPath));

            try
            {
                string solutionTemplate = File.ReadAllText(SolutionTemplatePath);
                string projectRowTemplate = File.ReadAllText(ProjectRowTemplatePath);
                string projectConfigTemplate = File.ReadAllText(ProjectConfigRowTemplatePath);
                string projectParentTemplate = File.ReadAllText(ProjectParentRowTemplatePath);

                Solution sourceSolution = new Solution(sourceSolutionPath);
                StringBuilder projectsList = new StringBuilder();
                StringBuilder projectConfigurationsList = new StringBuilder();
                StringBuilder projectDependenciesList = new StringBuilder();
                foreach (SolutionProject proj in sourceSolution.Projects)
                {
                    if (codeProjectsTypeSet.Contains(proj.ProjectType))
                    {
                        if (includeProjectNames.Count > 0 && !includeProjectNames.Contains(proj.ProjectName))
                            continue;
                    }
                    if (excludeProjectNames.Contains(proj.ProjectName))
                        continue;
                    projectsList.AppendLine(string.Format(projectRowTemplate,
                        GetGuidByProjectType(proj.ProjectType), proj.ProjectName,
                        proj.RelativePath, proj.ProjectGuid));
                    if (codeProjectsTypeSet.Contains(proj.ProjectType))
                        projectConfigurationsList.AppendLine(string.Format(projectConfigTemplate, proj.ProjectGuid));
                    if (!string.IsNullOrEmpty(proj.ParentProjectGuid))
                        projectDependenciesList.AppendLine(string.Format(projectParentTemplate, proj.ProjectGuid, proj.ParentProjectGuid));
                }
                File.WriteAllText(outSolutionPath, string.Format(solutionTemplate,
                    projectsList.ToString().TrimEnd('\r', '\n'),
                    projectConfigurationsList.ToString().TrimEnd('\r', '\n'),
                    projectDependenciesList.ToString().TrimEnd('\r', '\n')));
            }
            catch (Exception exc)
            {
                System.Console.WriteLine(exc);
                return CodeError;
            }
            return CodeSuccess;
        }
    }
}
