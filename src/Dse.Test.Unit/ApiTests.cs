﻿//
//  Copyright (C) 2016 DataStax, Inc.
//
//  Please see the license for details:
//  http://www.datastax.com/terms/datastax-dse-driver-license-terms
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dse.Auth;
using NUnit.Framework;

namespace Dse.Test.Unit
{
    public class ApiTests : BaseUnitTest
    {
        [Test]
        public void Dse_Auth_Namespace_Public_Test()
        {
            var types = GetTypesInNamespace("Dse.Auth", true);
            CollectionAssert.AreEquivalent(
                new[]
                {
#if !NETCORE
                    typeof(Dse.Auth.Sspi.SspiException), typeof(DseGssapiAuthProvider),
#endif
                    typeof(DsePlainTextAuthProvider)
                },
                types);
        }

        private static IEnumerable<Type> GetTypesInNamespace(string nameSpace, bool recursive)
        {
            Func<string, bool> isMatch = n => n.StartsWith(nameSpace);
            if (!recursive)
            {
                isMatch = n => n == nameSpace;
            }
            var assembly = typeof (IDseSession).GetTypeInfo().Assembly;
            return assembly.GetTypes().Where(t => isMatch(t.Namespace) && t.GetTypeInfo().IsPublic);
        }
    }
}
