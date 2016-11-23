﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Dse.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dse.Serialization
{
    internal class DseJsonConverter : JsonConverter
    {
        internal static readonly DseJsonConverter Instance = new DseJsonConverter();

        private DseJsonConverter()
        {
            
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is GeometryBase)
            {
                ((GeometryBase)value).WriteJson(writer, serializer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            if (typeof(GeometryBase).GetTypeInfo().IsAssignableFrom(objectType))
            {
                return true;
            }
            return false;
        }
    }
}
