
namespace PublishProfileManager.Models
{
    public class CustomPublishProfile : PublishProfileBase
    {
        public CustomPublishProfile()
            : base()
        {
        }

        public string ProfileName { get; set; }

        public string Configuration { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
