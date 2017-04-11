using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Build;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectHelpers
{
    public class CPSBuildManager
    {
        IVsHierarchy _hier;
        IBuildProject _buildProject;
        public CPSBuildManager(IVsHierarchy hierarchy)
        {
            _hier = hierarchy;
        }

        public async Task<IBuildProject> GetBuildProject(UnconfiguredProject unConfiguredProject)
        {
            ConfiguredProject configuredProject = await unConfiguredProject.GetSuggestedConfiguredProjectAsync();
            return configuredProject?.Services?.Build;
        }

        public async Task<IBuildResult> BuildAsync(string target, IDictionary<string, string> globalProperties)
        {
            if (globalProperties == null)
            {
                globalProperties = new Dictionary<string, string>();
            }

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsOutputWindowPane outputWindowPane = null;
            IVsOutputWindow outputWindow = ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            if (outputWindow != null)
            {
                outputWindow.GetPane(VSConstants.GUID_BuildOutputWindowPane, out outputWindowPane);
            }

            var hostObjects = new HashSet<IHostObject>();

            ILogger logger = null;
            if (outputWindowPane != null)
            {
                outputWindowPane.Activate();
                logger = new PublishLogger(outputWindowPane);
            }

            var loggers = new HashSet<ILogger>() { logger};

            if (_buildProject == null)
            {
                _buildProject = await GetBuildProject(_hier.GetUnconfiguredProject());
            }

            _hier.GetDTEProject().DTE.Solution.SolutionBuild.BuildProject(globalProperties["Configuration"], _hier.GetDTEProject().UniqueName, true);
            IBuildResult result = await _buildProject?.BuildAsync(new string[] { target }, CancellationToken.None, true, ImmutableDictionary.ToImmutableDictionary(globalProperties), hostObjects.ToImmutableHashSet(), BuildRequestPriority.High, loggers.ToImmutableHashSet());
            return result;
        }

        public async Task<IReadOnlyDictionary<string, string>> GetAllBuildProperties()
        {
            if (_buildProject == null)
            {
                _buildProject = await GetBuildProject(_hier.GetUnconfiguredProject());
            }

            return await _buildProject.GetFullBuildPropertiesAsync(CancellationToken.None); 
        }
    }
}
