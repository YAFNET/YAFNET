using System.Collections;
using System.Linq;

namespace YAF.Tests.Infrastructure;

/// <summary>
/// Provides the scenarios every <see cref="DatabaseTestBase"/> class runs against.
/// NUnit runs the fixtures class by class, so with all scenarios the docker stack has to be
/// switched for every class. Set the YAF_TEST_SCENARIOS environment variable (e.g. "SqlServer"
/// or "MySql,Sqlite") to only run the given scenarios, so each stack is only started once.
/// </summary>
public class DatabaseFixtureSource : IEnumerable
{
    private readonly static ComposeScenario[] Scenarios =
    [
        ComposeScenario.SqlServer,
        ComposeScenario.MySql,
        ComposeScenario.PostgreSQL,
        ComposeScenario.Sqlite
    ];

    public IEnumerator GetEnumerator()
    {
        var filter = Environment.GetEnvironmentVariable("YAF_TEST_SCENARIOS")?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return Scenarios
            .Where(scenario => filter is not { Length: > 0 } ||
                               filter.Contains(scenario.Name, StringComparer.OrdinalIgnoreCase))
            .Select(scenario => new object[] { scenario })
            .GetEnumerator();
    }
}
