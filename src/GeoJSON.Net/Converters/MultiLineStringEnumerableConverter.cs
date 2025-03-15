// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using System;
using System.Collections.Generic;
using System.Linq;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeoJSON.Net.Converters
{
    /// <summary>
    /// Converter to read and write the <see cref="IEnumerable{MultiLineString}" /> type.
    /// </summary>
    public class MultiLineStringEnumerableConverter : JsonConverter
    {
        private static readonly LineStringEnumerableConverter MultiLineStringConverter = new LineStringEnumerableConverter();

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<MultiLineString>).IsAssignableFromType(objectType);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var rings = existingValue as JArray ?? serializer.Deserialize<JArray>(reader);
            return rings.Select(ring => new MultiLineString((IEnumerable<LineString>)MultiLineStringConverter.ReadJson(
                    reader,
                    typeof(IEnumerable<IPosition>),
                    ring,
                    serializer)))
                .ToArray();
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is IEnumerable<MultiLineString> coordinateElements)
            {
                writer.WriteStartArray();
                foreach (var subPolygon in coordinateElements)
                {
                    MultiLineStringConverter.WriteJson(writer, subPolygon.Coordinates, serializer);
                }
                writer.WriteEndArray();
            }
            else
            {
                throw new ArgumentException($"{nameof(MultiLineStringEnumerableConverter)}: unsupported value type");
            }
        }
    }
}