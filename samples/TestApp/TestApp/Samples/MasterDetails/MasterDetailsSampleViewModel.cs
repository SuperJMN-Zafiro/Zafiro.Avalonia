using System.Collections.Generic;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace TestApp.Samples.MasterDetails;

public partial class MasterDetailsSampleViewModel : ReactiveObject
{
    [Reactive] private SampleSection? selectedSection;
        
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