using AutoFixture;

namespace Merchant.CORE.UnitTest
{
    public abstract class BaseFixture
    {
        protected Fixture Fixture { get; }

        protected BaseFixture()
        {
            Fixture = new Fixture()
                .AutoMoq()
                .IgnoreRecursion();
        }
    }
}
