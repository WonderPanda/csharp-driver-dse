//
//  Copyright (C) 2016 DataStax, Inc.
//
//  Please see the license for details:
//  http://www.datastax.com/terms/datastax-dse-driver-license-terms
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cassandra;
using Newtonsoft.Json;

namespace Dse.Graph
{
    /// <summary>
    /// Represents a graph query.
    /// </summary>
    public class SimpleGraphStatement : GraphStatement
    {
        public readonly string Query;
        public readonly object Values;
        public readonly IDictionary<string, object> ValuesDictionary;

        /// <summary>
        /// Creates a new instance of <see cref="SimpleGraphStatement"/> using a query with no parameters.
        /// </summary>
        /// <param name="query">The graph query string.</param>
        public SimpleGraphStatement(string query) : this(query, null)
        {
            
        }

        /// <summary>
        /// Creates a new instance of <see cref="SimpleGraphStatement"/> using a query with named parameters.
        /// </summary>
        /// <param name="query">The graph query string.</param>
        /// <param name="values">An anonymous object containing the parameters as properties.</param>
        /// <example>
        /// <code>new SimpleGraphStatement(&quot;g.V().has('name', myName)&quot;, new { myName = &quot;mark&quot;})</code>
        /// </example>
        public SimpleGraphStatement(string query, object values)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            Query = query;
            if (values != null && !IsAnonymous(values))
            {
                throw new ArgumentException("Expected anonymous object containing the parameters as properties", "values");
            }
            Values = values;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SimpleGraphStatement"/> using a query with named parameters.
        /// </summary>
        /// <param name="values">An Dictionary object containing the parameters name and values as key and values.</param>
        /// <param name="query">The graph query string.</param>
        /// <example>
        /// <code>
        /// new SimpleGraphStatement(
        ///     new Dictionary&lt;string, object&gt;{ { &quot;myName&quot;, &quot;mark&quot; } }, 
        ///     &quot;g.V().has('name', myName)&quot;)
        /// </code>
        /// </example>
        public SimpleGraphStatement(IDictionary<string, object> values, string query)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            Query = query;
            ValuesDictionary = values;
        }

        internal override IStatement GetIStatement(GraphOptions options)
        {
            string jsonParams = null;
            if (ValuesDictionary != null)
            {
                jsonParams = JsonConvert.SerializeObject(ValuesDictionary);
            }
            else if (Values != null)
            {
                jsonParams = JsonConvert.SerializeObject(Values);
            }
            IStatement stmt;
            if (jsonParams != null)
            {
                stmt = new TargettedSimpleStatement(Query, jsonParams);
            }
            else
            {
                stmt = new TargettedSimpleStatement(Query);
            }
            //Set Cassandra.Statement properties
            if (Timestamp != null)
            {
                stmt.SetTimestamp(Timestamp.Value);
            }
            var readTimeout = ReadTimeoutMillis != 0 ? ReadTimeoutMillis : options.ReadTimeoutMillis;
            if (readTimeout <= 0)
            {
                // Infinite (-1) is not supported in the core driver, set an arbitrarily large int
                readTimeout = int.MaxValue;
            }
            return stmt
                .SetIdempotence(false)
                .SetConsistencyLevel(ConsistencyLevel)
                .SetReadTimeoutMillis(readTimeout)
                .SetOutgoingPayload(options.BuildPayload(this));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SimpleGraphStatement)) return false;
            var comparisonStatement = (SimpleGraphStatement) obj;

            return comparisonStatement.Query.Equals(Query) 
                && JsonConvert.SerializeObject(comparisonStatement.Values).Equals(JsonConvert.SerializeObject(Values))
                && JsonConvert.SerializeObject(comparisonStatement.ValuesDictionary).Equals(JsonConvert.SerializeObject(ValuesDictionary));
        }

        public override int GetHashCode()
        {
            var valuesString = JsonConvert.SerializeObject(Values);
            var valuesDictionaryString = JsonConvert.SerializeObject(ValuesDictionary);

            int hash = 27;
            hash = (13 * hash) + Query.GetHashCode();
            hash = (13 * hash) + valuesString.GetHashCode();
            hash = (13 * hash) + valuesDictionaryString.GetHashCode();
            return hash;
        }
    }
}