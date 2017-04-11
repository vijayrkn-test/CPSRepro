using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ApplicationCapabilities.Publish.Contracts;
using Microsoft.VisualStudio.Utilities;

namespace WebJobsProvider
{
    [Export(typeof(IProfileVisualFactoryProvider))]
    [Name(nameof(ReproProfileVisualFactoryProvider))]
    public class ReproProfileVisualFactoryProvider : IProfileVisualFactoryProvider
    {
        public IProfileVisualFactory CreateFactory(IPublishServiceProvider provider)
        {
            return new ReproProfileVisualFactory(provider);
        }
    }
}
