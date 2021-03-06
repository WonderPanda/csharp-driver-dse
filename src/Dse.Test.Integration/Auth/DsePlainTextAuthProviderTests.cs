﻿//
//  Copyright (C) 2016 DataStax, Inc.
//
//  Please see the license for details:
//  http://www.datastax.com/terms/datastax-dse-driver-license-terms
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Cassandra;
using Cassandra.IntegrationTests.TestBase;
using NUnit.Framework;
using Dse.Auth;
using Dse.Test.Integration.ClusterManagement;

namespace Dse.Test.Integration.Auth
{
    public class DsePlainTextAuthProviderTests : BaseIntegrationTest
    {
        private static void AssertCanQuery(ISession session)
        {
            for (var i = 0; i < 10; i++)
            {
                Assert.DoesNotThrow(() => session.Execute("SELECT key FROM system.local"));
            }
        }

        [Test, TestDseVersion(5,0)]
        public void Should_Authenticate_Against_Dse_5_DseAuthenticator()
        {
            CcmHelper.Start(
                1,
                new[] { "authentication_options.default_scheme: internal" },
                new[] { "authenticator: com.datastax.bdp.cassandra.auth.DseAuthenticator" },
                new[] { "-Dcassandra.superuser_setup_delay_ms=0" });
            Trace.TraceInformation("Waiting additional time for test Cluster to be ready");
            Thread.Sleep(15000);
            var authProvider = new DsePlainTextAuthProvider("cassandra", "cassandra");
            using (var cluster = Cluster.Builder()
                .AddContactPoint(CcmHelper.InitialContactPoint)
                .WithAuthProvider(authProvider)
                .Build())
            {
                var session = cluster.Connect();
                AssertCanQuery(session);
            }
        }

        [Test]
        public void Should_Authenticate_Against_Dse_Daemon_With_PasswordAuthenticator()
        {
            CcmHelper.Start(
                1,
                cassYamlOptions: new[] { "authenticator: PasswordAuthenticator" },
                jvmArgs: new[] { "-Dcassandra.superuser_setup_delay_ms=0" });
            Trace.TraceInformation("Waiting additional time for test Cluster to be ready");
            Thread.Sleep(15000);
            var authProvider = new DsePlainTextAuthProvider("cassandra", "cassandra");
            using (var cluster = Cluster.Builder()
                .AddContactPoint(CcmHelper.InitialContactPoint)
                .WithAuthProvider(authProvider)
                .Build())
            {
                var session = cluster.Connect();
                AssertCanQuery(session);
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                CcmHelper.Remove();
            }
            catch
            {
                //Tried to remove, never mind if it fails.
            }
        }
    }
}
