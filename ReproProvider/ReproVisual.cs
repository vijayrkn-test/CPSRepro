using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ApplicationCapabilities.Publish.Contracts;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectHelpers;
using PublishProfileManager.Models;

namespace WebJobsProvider
{
    public class ReproVisual : IProfileVisual
    {
        private CustomPublishProfile _dataModel;
        private IVsHierarchy _hierarchy;
        public ReproVisual(CustomPublishProfile dataModel, string profileFullPath, IVsHierarchy hierarchy)
        {
            ProfileIcon = null;
            ProfileId = profileFullPath;
            ProfileName = Path.GetFileNameWithoutExtension(profileFullPath);
            _dataModel = dataModel;
            _hierarchy = hierarchy;
        }

        public string ProfileId { get; }

        public object ProfileIcon { get; }

        public string ProfileName { get;}

        public IReadOnlyList<IProfileCommand> ProfileCommands => null;
        public IReadOnlyList<KeyValuePair<string, object>> SummaryEntries
        {
            get
            {
                return _dataModel?.GetProfileProperties();
            }
        }

        public IReadOnlyDictionary<string, object> TelemetryData => null;

        public bool IsReadyToPublish => true;

        public string InformationMessage => null;

        public void ConfigurePublishSteps(IPublishSteps publishSteps)
        {
            publishSteps.CorePublish = PublishWebJob;
        }

        public async Task<bool> PublishWebJob(string profileId)
        {
            CPSBuildManager cpsBuildManager = new CPSBuildManager(_hierarchy);
            var buildProperties = new Dictionary<string, string>();
            CustomPublishProfile profile = new CustomPublishProfile();
            profile.LoadModel(File.ReadAllText(profileId));
            buildProperties["Configuration"] = profile.Configuration;
            var result = await cpsBuildManager.BuildAsync("publish", buildProperties);

            return result.Errors == 0;
        }
    }
}
