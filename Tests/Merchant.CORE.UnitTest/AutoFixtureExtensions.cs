using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;

namespace Merchant.CORE.UnitTest
{
    public static class AutoFixtureExtensions
    {
        public static Fixture IgnoreRecursion(this Fixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture;
        }

        public static Fixture AutoMoq(this Fixture fixture, bool configureMembers = true)
        {
            fixture.Customize(new AutoMoqCustomization { ConfigureMembers = configureMembers });
            return fixture;
        }

        public static Mock<T> FreezeMoq<T>(this Fixture fixture) where T : class
        {
            var mock = new Mock<T>();
            fixture.Inject(mock.Object);
            return mock;
        }
    }
}
