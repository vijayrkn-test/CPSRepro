using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectHelpers
{
    public static class HierarchyExtensions
    {
        public static UnconfiguredProject GetUnconfiguredProject(this IVsHierarchy hierarchy)
        {
            IVsBrowseObjectContext context = hierarchy as IVsBrowseObjectContext;
            if (context == null)
            {
                EnvDTE.Project dteProject = hierarchy.GetDTEProject();
                if (dteProject != null)
                {
                    context = dteProject.Object as IVsBrowseObjectContext;
                }
            }
            return context?.UnconfiguredProject;
        }

        /// <summary>
        /// Returns EnvDTE.Project object for the hierarchy
        /// </summary>
        public static EnvDTE.Project GetDTEProject(this IVsHierarchy hierarchy)
        {
            VerifyOnUIThread();
            if (ErrorHandler.Succeeded(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out object extObject)))
            {
                return extObject as EnvDTE.Project;
            }

            return null;
        }

        public static void VerifyOnUIThread([CallerMemberName] string memberName = "")
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread(memberName);
            }
            catch
            {
            }
        }

        public static string GetProjectFullPath(this IVsHierarchy hierarchy)
        {
                string projectFile = string.Empty;
                IVsProject proj = hierarchy as IVsProject;
                if (proj != null)
                    proj.GetMkDocument((uint)VSConstants.VSITEMID_ROOT, out projectFile);
                return projectFile;
        }

        public static string GetProjectName(this IVsHierarchy hierarchy)
        {
            return hierarchy.GetDTEProject().Name;
        }

        public static bool IsApplicableTo(this IVsHierarchy hierarchy, string search)
        {
            return hierarchy.IsCapabilityMatch(search);
        }
    }
}
