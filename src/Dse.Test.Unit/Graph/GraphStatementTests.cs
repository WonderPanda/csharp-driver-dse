using System.Collections.Generic;
using Dse.Graph;
using NUnit.Framework;

namespace Dse.Test.Unit.Graph
{
    public class GraphStatementTests : BaseUnitTest
    {
        private const string Query = "g.V().hasLabel('God', 'Name', 'Poseidon')";
        private readonly object _objParams1 = new {Key = "Value1"};
        private readonly object _objParams2 = new {Key = "Value2"};

        private readonly Dictionary<string, object> _dictParams1 = new Dictionary<string, object>
        {
            {"Key1", "Value1"},
            {"Key2", 42}
        };

        private readonly Dictionary<string, object> _dictParams2 = new Dictionary<string, object>
        {
            {"Key1", "Value2"},
            {"Key2", 42}
        };

        [Test]
        public void SimpleGraphStatements_With_Matching_Queries_Should_Be_Equal()
        {
            var statement1 = new SimpleGraphStatement(Query);
            var statement2 = new SimpleGraphStatement(Query);

            Assert.AreEqual(statement1, statement2);
        }

        [Test]
        public void SimpleGraphStatements_With_Matching_Queries_And_Different_ObjParams_Should_Not_Be_Equal()
        {
            var statement1 = new SimpleGraphStatement(Query, _objParams1);
            var statement2 = new SimpleGraphStatement(Query, _objParams2);

            Assert.AreNotEqual(statement1, statement2);
        }

        [Test]
        public void SimpleGraphStatements_With_Matching_Queries_And_ObjParams_Should_Be_Equal()
        {
            var statement1 = new SimpleGraphStatement(Query, _objParams1);
            var statement2 = new SimpleGraphStatement(Query, _objParams1);

            Assert.AreEqual(statement1, statement2);
        }

        [Test]
        public void SimpleGraphStatements_With_Matching_Queries_And_Different_DictParams_Should_Not_Be_Equal()
        {
            var statement1 = new SimpleGraphStatement(_dictParams1, Query);
            var statement2 = new SimpleGraphStatement(_dictParams2, Query);

            Assert.AreNotEqual(statement1, statement2);
        }

        [Test]
        public void SimpleGraphStatements_With_Matching_Queries_And_DictParams_Should_Be_Equal()
        {
            var statement1 = new SimpleGraphStatement(_dictParams1, Query);
            var statement2 = new SimpleGraphStatement(_dictParams1, Query);

            Assert.AreEqual(statement1, statement2);
        }
    }
}
