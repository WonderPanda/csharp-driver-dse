﻿//
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
using Dse.Serialization;
using Newtonsoft.Json;

namespace Dse.Graph
{
    /// <summary>
    /// Represents a graph query.
    /// </summary>
    public class SimpleGraphStatement : GraphStatement
    {
        private readonly string _query;
        private readonly object _values;
        private readonly IDictionary<string, object> _valuesDictionary;

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
            _query = query;
            if (values != null && !IsAnonymous(values))
            {
                throw new ArgumentException("Expected anonymous object containing the parameters as properties", "values");
            }
            _values = values;
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
            _query = query;
            _valuesDictionary = values;
        }

        internal override IStatement GetIStatement(GraphOptions options)
        {
            string jsonParams = null;
            if (_valuesDictionary != null)
            {
                jsonParams = JsonConvert.SerializeObject(_valuesDictionary);
            }
            else if (_values != null)
            {
                jsonParams = JsonConvert.SerializeObject(_values);
            }
            IStatement stmt;
            if (jsonParams != null)
            {
                stmt = new TargettedSimpleStatement(_query, jsonParams);
            }
            else
            {
                stmt = new TargettedSimpleStatement(_query);
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
    }
}