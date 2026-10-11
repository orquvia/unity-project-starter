using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Orquvia.Starter.Domain;

namespace Orquvia.Starter.Tests.Architecture
{
    // GameSession を変更できるのが UseCase 層だけであることを検証する。
    // public に戻されたり、InternalsVisibleTo に Presentation が足されたりすると失敗する。
    public sealed class DomainEncapsulationTests
    {
        [Test]
        public void GameSession_HasNoPublicMutators()
        {
            var publicMethods = typeof(GameSession)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => !(method.IsSpecialName && method.Name.StartsWith("get_")))
                .Select(method => method.Name)
                .ToArray();

            Assert.That(publicMethods, Is.Empty, "GameSession に public な変更メソッドがあります");
        }

        [Test]
        public void DomainInternals_AreVisibleOnlyToUseCaseAndTests()
        {
            var friends = typeof(GameSession).Assembly
                .GetCustomAttributes<InternalsVisibleToAttribute>()
                .Select(attribute => attribute.AssemblyName)
                .ToArray();

            Assert.That(
                friends,
                Is.EquivalentTo(new[]
                {
                    "Orquvia.Starter.UseCase",
                    "Orquvia.Starter.Tests.EditMode"
                }));
        }
    }
}
