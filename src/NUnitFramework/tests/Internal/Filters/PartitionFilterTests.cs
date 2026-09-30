// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Tests.Internal.Filters
{
    public class PartitionFilterTests : TestFilterTests
    {
        private PartitionFilter _filter;
        private ITest _testMatchingPartition;
        private ITest _testNotMatchingPartition;

        [SetUp]
        public void CreateFilter()
        {
            // Configure a new PartitionFilter with the provided partition count and number
            _filter = new TestPartitionFilter(6, 10);

            _testMatchingPartition = FixtureWithMultipleTestsSuite.Tests[0];
            _testNotMatchingPartition = FixtureWithMultipleTestsSuite.Tests[1];
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.That(_filter.IsEmpty, Is.False);
        }

        [Test]
        public void MatchTest()
        {
            // Validate
            Assert.That(_filter.ComputePartitionNumber(_testMatchingPartition), Is.EqualTo(6));
            Assert.That(_filter.ComputePartitionNumber(_testNotMatchingPartition), Is.EqualTo(9));

            // Assert
            Assert.That(_filter.Match(_testMatchingPartition), Is.True);
            Assert.That(_filter.Match(_testNotMatchingPartition), Is.False);
        }

        [Test]
        public void PassTest()
        {
            // This test fixture contains both one matching and one non-matching test
            // The fixture should therefore pass as True because one of the child tests are a match
            Assert.That(_filter.Pass(FixtureWithMultipleTestsSuite), Is.True);

            // Validate that our matching and non-matching tests return the correct Pass result
            Assert.That(_filter.Pass(_testMatchingPartition), Is.True);
            Assert.That(_filter.Pass(_testNotMatchingPartition), Is.False);

            // This other test fixture has no matching tests for this partition number
            Assert.That(_filter.Pass(SpecialFixtureSuite), Is.False);
        }

        [Test]
        public void ExplicitMatchTest()
        {
            // Top level TestFixture should always Pass
            Assert.That(_filter.IsExplicitMatch(FixtureWithMultipleTestsSuite));

            // Assert
            Assert.That(_filter.IsExplicitMatch(_testMatchingPartition), Is.True);
            Assert.That(_filter.IsExplicitMatch(_testNotMatchingPartition), Is.False);
        }

        [Test]
        public async Task ComputePartitionNumberThreadSafe()
        {
            var tests = Enumerable.Range(0, 10).Select(i => FixtureWithMultipleTestsSuite.Tests[i % 2]).ToArray();
            var expected = tests.Select(test => _filter.ComputePartitionNumber(test)).ToArray();

            var tasks = tests.Select(test => Task.Run(() => _filter.ComputePartitionNumber(test))).ToArray();
            await Task.WhenAll(tasks);

            Assert.That(expected, Is.EqualTo(tasks.Select(t => t.Result)));
        }

        [Test]
        public void ComputeParitionNumberHandlesLongTestCaseName()
        {
            var fixtureWithLongNames = FixtureWithLongTestCaseNamesSuite.Tests[0];

            Assert.DoesNotThrow(() => _filter.ComputePartitionNumber(fixtureWithLongNames.Tests[0]));
            Assert.DoesNotThrow(() => _filter.ComputePartitionNumber(fixtureWithLongNames.Tests[1]));
        }

        [TestCaseSource(nameof(FromXmlTestCases))]
        public static void FromXml(string xml, TestFilter expected)
        {
            TestFilter filter = TestFilter.FromXml($@"<filter>{xml}</filter>");

            Assert.That(filter, Is.TypeOf(expected.GetType()));
            Assert.That(filter, Is.EqualTo(expected).UsingPropertiesComparer());
        }
        private static readonly TestCaseData[] FromXmlTestCases =
        {
            TestCaseData.Create(@"<partition>7/10</partition>", new TestPartitionFilter(7, 10))
        };

        [TestCaseSource(nameof(ToXmlTestCases))]
        public static string ToXml(TestFilter filter)
            => filter.ToXml(false).OuterXml;

        private static readonly TestCaseData[] ToXmlTestCases =
        {
            TestCaseData.Create(new TestPartitionFilter(7, 10)).Returns(@"<partition>7/10</partition>")
        };

        [TestCase(@"<partition>7/10</partition>")]
        public static void RoundTripXml(string xml)
        {
            TestFilter filter1 = TestFilter.FromXml($@"<filter>{xml}</filter>");
            string xml2 = filter1.ToXml(false).OuterXml;
            TestFilter filter2 = TestFilter.FromXml($@"<filter>{xml}</filter>");

            Assert.That(xml, Is.EqualTo(xml2));
            Assert.That(filter1, Is.TypeOf(filter2.GetType()));
            Assert.That(filter1, Is.EqualTo(filter2).UsingPropertiesComparer());
        }

        [TestCase("1 /1n")]
        [TestCase("1")]
        public static void TryCreateFailure(string input)
        {
            Assert.That(PartitionFilter.TryCreate(input, out _), Is.False);
        }

        [TestCase("1/2")]
        [TestCase(" 1/2")]
        [TestCase("1/2 ")]
        public static void TryCreateSuccess(string input)
        {
            var result = PartitionFilter.TryCreate(input, out var filter);

            Assert.That(result, Is.True);
            Assert.That(filter, Is.Not.Null);

            Assert.That(filter.PartitionNumber, Is.EqualTo(1));
            Assert.That(filter.PartitionCount, Is.EqualTo(2));
        }
    }
}
