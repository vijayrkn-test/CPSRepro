using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectHelpers
{
    public class BuildManager
    {
        private IVsHierarchy _hierarchy;
        public BuildManager(IVsHierarchy hierarchy)
        {
            _hierarchy = hierarchy;
        }

        public IReadOnlyList<string> GetSolutionBuildConfigurations()
        {
            List<string> solutionConfigurationNames = new List<string>();
            SolutionConfigurations solutionConfigurations = _hierarchy.GetDTEProject().DTE.Solution.SolutionBuild.SolutionConfigurations;
            foreach (SolutionConfiguration2 config in solutionConfigurations)
            {
                solutionConfigurationNames.Add(config.Name);
            }

            return solutionConfigurationNames;
        }

        public IReadOnlyList<string> GetMsBuildPropertyValues(string propertyName)
        {
            ProjectCollection projectCollection = new ProjectCollection();
            Microsoft.Build.Evaluation.Project project = projectCollection.LoadProject(_hierarchy.GetProjectFullPath());
            string[] propertyValues = project.GetPropertyValue(propertyName)?.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            return propertyValues != null ? propertyValues.ToList() : null;
        }

        public string GetMsBuildPropertyValue(string propertyName)
        {
            return GetMsBuildPropertyValues(propertyName)?.FirstOrDefault();
        }
    }
}
