using System.Collections;

namespace YAF.Tests.Infrastructure;

public class DatabaseFixtureSource : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        yield return new object[] { ComposeScenario.SqlServer };
        yield return new object[] { ComposeScenario.MySql };
        yield return new object[] { ComposeScenario.PostgreSQL };
        yield return new object[] { ComposeScenario.Sqlite };
    }
}