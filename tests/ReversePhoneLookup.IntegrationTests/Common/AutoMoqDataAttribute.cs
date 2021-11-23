using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace Common
{
    /// <summary>
    /// This helps us to use Moq in tandem with AutoFixture
    /// </summary>
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public static IFixture FixtureFactory()
        {
            var f = new Fixture();

            f.Customize(new AutoMoqCustomization { ConfigureMembers = true });

            return f;
        }

        public AutoMoqDataAttribute() : base(FixtureFactory) { }
    }
}
