using System;
using NUnit.Framework;
using Orquvia.Starter.Domain;

namespace Orquvia.Starter.Tests.Domain
{
    public sealed class GameRulesTests
    {
        [Test]
        public void Constructor_ValidValues_KeepsValues()
        {
            var rules = new GameRules(20f, 10);

            Assert.That(rules.DurationSeconds, Is.EqualTo(20f));
            Assert.That(rules.PointsPerHit, Is.EqualTo(10));
        }

        [TestCase(0f)]
        [TestCase(-1f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        public void Constructor_InvalidDuration_Throws(float durationSeconds)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new GameRules(durationSeconds, 10));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_NonPositivePointsPerHit_Throws(int pointsPerHit)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new GameRules(20f, pointsPerHit));
        }
    }
}
