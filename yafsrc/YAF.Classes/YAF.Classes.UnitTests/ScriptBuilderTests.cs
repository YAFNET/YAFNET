namespace YAF.Classes.UnitTests
{
  #region Using

  using Autofac;

  using Xunit;
  using Xunit.Should;

  using YAF.Core;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The script builder tests.
  /// </summary>
  public class ScriptBuilderTests : IHaveServiceLocator
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptBuilderTests"/> class.
    /// </summary>
    public ScriptBuilderTests()
    {
      GlobalContainer.Container.Resolve<IInjectServices>().Inject(this);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    [Inject]
    public IServiceLocator ServiceLocator { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The test_ script building_ assumptions.
    /// </summary>
    [Fact]
    public void Test_ScriptBuilding_Assumptions()
    {
      var sb = this.Get<IScriptBuilder>();

      var str =
        sb.CreateStatement().AddSelectorFormat("'Blah{0}'", 10).Dot().AddCall("html", "donkey's").Dot().AddCall(
          "click", sb.CreateFunction().AddCall("alert", "It's clicked!").End()).End().Build();

      str.ShouldBe(
        @"jQuery('Blah10').html(""donkey\'s"").click(function(){{ alert(""It\'s clicked!"");{0} }});{0}".FormatWith("\r\n"));
    }

    #endregion
  }
}