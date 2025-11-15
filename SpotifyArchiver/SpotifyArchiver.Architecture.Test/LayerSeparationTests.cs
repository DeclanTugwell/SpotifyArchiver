using Shouldly;
using System.Reflection;

namespace SpotifyArchiver.Architecture.Test
{
    [TestFixture]
    public class LayerSeparationTests
    {
        private bool ReferencesAssembly(Assembly source, string targetNameFragment) =>
            source.GetReferencedAssemblies()
                  .Any(a => a.Name!.Contains(targetNameFragment, StringComparison.OrdinalIgnoreCase));

        [Test]
        public void Presentation_ShouldNotReference_DataAccess_Implementation()
        {
            ReferencesAssembly(
                Assembly.Load("SpotifyArchiver.Presentation"),
                "SpotifyArchiver.DataAccess.Implementation"
            ).ShouldBeFalse();
        }

        [Test]
        public void Presentation_ShouldNotReference_Application_Implementation()
        {
            ReferencesAssembly(
                Assembly.Load("SpotifyArchiver.Presentation"),
                "SpotifyArchiver.Application.Implementation"
            ).ShouldBeFalse();
        }

        [Test]
        public void Application_Abstraction_ShouldNotReference_DataAccess_Implementation()
        {
            ReferencesAssembly(
                Assembly.Load("SpotifyArchiver.Application.Abstraction"),
                "SpotifyArchiver.DataAccess.Implementation"
            ).ShouldBeFalse();
        }

        [Test]
        public void Application_Implementation_ShouldNotReference_DataAccess_Implementation()
        {
            ReferencesAssembly(
                Assembly.Load("SpotifyArchiver.Application.Implementation"),
                "SpotifyArchiver.DataAccess.Implementation"
            ).ShouldBeFalse();
        }

        [Test]
        public void Application_Abstraction_ShouldNotReference_Application_Implementation()
        {
            ReferencesAssembly(
                Assembly.Load("SpotifyArchiver.Application.Abstraction"),
                "SpotifyArchiver.Application.Implementation"
            ).ShouldBeFalse();
        }

        [Test]
        public void DataAccess_Abstraction_ShouldNotReference_Application_Implementation()
        {
            ReferencesAssembly(
                Assembly.Load("SpotifyArchiver.DataAccess.Abstraction"),
                "SpotifyArchiver.Application.Implementation"
            ).ShouldBeFalse();
        }

        [Test]
        public void DataAccess_Implementation_ShouldNotReference_Application_Implementation()
        {
            ReferencesAssembly(
                Assembly.Load("SpotifyArchiver.DataAccess.Implementation"),
                "SpotifyArchiver.Application.Implementation"
            ).ShouldBeFalse();
        }

        [Test]
        public void ImplementationReferenceBanScore()
        {
            bool[] results =
            [
                !ReferencesAssembly(Assembly.Load("SpotifyArchiver.Presentation"), "SpotifyArchiver.DataAccess.Implementation"),
                !ReferencesAssembly(Assembly.Load("SpotifyArchiver.Presentation"), "SpotifyArchiver.Application.Implementation"),
                !ReferencesAssembly(Assembly.Load("SpotifyArchiver.Application.Abstraction"), "SpotifyArchiver.DataAccess.Implementation"),
                !ReferencesAssembly(Assembly.Load("SpotifyArchiver.Application.Implementation"), "SpotifyArchiver.DataAccess.Implementation"),
                !ReferencesAssembly(Assembly.Load("SpotifyArchiver.Application.Abstraction"), "SpotifyArchiver.Application.Implementation"),
                !ReferencesAssembly(Assembly.Load("SpotifyArchiver.DataAccess.Abstraction"), "SpotifyArchiver.Application.Implementation"),
                !ReferencesAssembly(Assembly.Load("SpotifyArchiver.DataAccess.Implementation"), "SpotifyArchiver.Application.Implementation")
            ];

            var score = (results.Count(passed => passed) * 100) / results.Length;
            Console.WriteLine($"Implementation Reference Ban Score: {score}%");
            score.ShouldBeGreaterThanOrEqualTo(85);
        }
    }
}
