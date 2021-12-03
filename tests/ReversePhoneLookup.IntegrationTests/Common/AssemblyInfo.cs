using Xunit;
//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "ReversePhoneLookup")]


namespace ReversePhoneLookup.IntegrationTests.Common
{
    public class AssemblyInfo
    {
        
    }
}