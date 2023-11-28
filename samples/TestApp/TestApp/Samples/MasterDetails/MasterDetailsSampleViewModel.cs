using System.Collections.Generic;

namespace TestApp.Samples.MasterDetails
{
    internal class MasterDetailsSampleViewModel
    {
        public MasterDetailsSampleViewModel()
        {
            Sections = new List<SampleSection>()
            {
                new SampleSection()
                {
                    Title = "Sample 1",
                    Content = "This is a sample 1"
                },
                new SampleSection()
                {
                    Title = "Sample 2",
                    Content = "This is a sample 2"
                },
                new SampleSection()
                {
                    Title = "Sample 3",
                    Content = "This is a sample 3"
                },
            };
        }

        public IEnumerable<SampleSection> Sections { get; }
    }

    public class SampleSection
    {
        public string Title { get; set; }
        public object Content { get; set; }
    }
}
