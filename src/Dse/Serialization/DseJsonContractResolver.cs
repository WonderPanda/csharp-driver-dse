﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dse.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dse.Serialization
{
    internal class DseJsonContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// A single instance of a JsonSerializerSettings that uses this ContractResolver.
        /// </summary>
        internal static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DseJsonContractResolver()
        };

        protected DseJsonContractResolver()
        {

        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            if (typeof(GeometryBase).GetTypeInfo().IsAssignableFrom(objectType))
            {
                contract.Converter = DseJsonConverter.Instance;
            }
            return contract;
        }
    }
}
