using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ApplicationCapabilities.Publish.Contracts;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Web.WindowsAzure.CommonContracts;
using Microsoft.VisualStudio.WindowsAzure.MicrosoftWeb;
using Microsoft.WindowsAzure.Client.Entities;
using Microsoft.WindowsAzure.Client.MicrosoftWeb;
using Microsoft.WindowsAzure.Client.MicrosoftWeb.Entities;
using ProjectHelpers;
using PublishProfileManager.Models;

namespace WebJobsProvider
{
    public class ReproProfileVisualFactory : IProfileVisualFactory
    {
        IPublishServiceProvider _serviceProvider;
        IVsHierarchy _hierarchy;
        public ReproProfileVisualFactory(IPublishServiceProvider publishServiceProvider)
        {
            _serviceProvider = publishServiceProvider;
            Icon = null;
        }
        public int ExistingProfileHandlingPriority => 1;

        public object Icon { get; }

        public string DisplayName => "CPS Repro";

        public Func<Task<bool>> ImportProfilesAsync => null;

        public Func<string, bool> FilterProfiles => null;

        public Task<IProfileVisual> CreateProfileAsync(IVsHierarchy hierarchy, object optionsControl)
        {
            CustomPublishProfile customProfile = new CustomPublishProfile()
            {
                ProfileName="ReproProfile",
                Configuration="Release"
            };
            string profileFullPath = Path.Combine(PublishProfilesFolder, "CustomProfile.pubxml");
            File.WriteAllText(profileFullPath, customProfile.ToString());
            IProfileVisual visual = new ReproVisual(customProfile, profileFullPath, hierarchy);
            return System.Threading.Tasks.Task.FromResult(visual);
        }

        public string PublishProfilesFolder
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(_hierarchy.GetProjectFullPath()), "Properties", "PublishProfiles");
            }
        }

        public bool IsApplicableTo(IVsHierarchy hierarchy, out IReadOnlyList<string> tags)
        {
            tags = null;
            _hierarchy = hierarchy;
            return hierarchy.IsApplicableTo("(CSharp|VB)&CPS&!Web");
        }

        public bool TryGetProfileVisual(string profileId, out IProfileVisual profileVisual)
        {
            profileVisual = null;
            try
            {
                string profileContents = File.ReadAllText(profileId);
                CustomPublishProfile profile = new CustomPublishProfile();
                profile.LoadModel(profileContents);
                bool isCustomProfile = !string.IsNullOrEmpty(profile.ProfileName) && !string.IsNullOrEmpty(profile.Configuration);
                if (isCustomProfile)
                {
                    profileVisual = new ReproVisual(profile, profileId, _hierarchy);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public object GetOptionsControl(IVsHierarchy hierarchy, Action<string, bool> statusUpdater)
        {
            return null;
        }

        private static IComponentModel _componentModel;

        private static IComponentModel ComponentModel
        {
            get
            {
                if (_componentModel == null)
                {
                    _componentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
                }

                return _componentModel;
            }
        }
    }
}
