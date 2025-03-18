// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GeoJSON.Net.Converters;
using Newtonsoft.Json;

namespace GeoJSON.Net.Geometry
{
    /// <summary>
    /// Defines the MultiPolyLine type.
    /// </summary>
    /// <remarks>
    /// See https://tools.ietf.org/html/rfc7946#section-3.1.7
    /// </remarks>
    public class MultiPolyLine : GeoJSONObject, IGeometryObject, IEqualityComparer<MultiPolyLine>, IEquatable<MultiPolyLine>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiPolyLine" /> class.
        /// </summary>
        /// <param name="PolyLines">The PolyLines contained in this MultiPolyLine.</param>
        public MultiPolyLine(IEnumerable<PolyLine> PolyLines)
        {
            Coordinates = new ReadOnlyCollection<PolyLine>(
                PolyLines?.ToArray() ?? throw new ArgumentNullException(nameof(PolyLines)));
        }

        /// <summary>
        /// Initializes a new <see cref="MultiPolyLine" /> from a 4-d array of <see cref="double" />s
        /// that matches the "coordinates" field in the JSON representation.
        /// </summary>
        [JsonConstructor]
        public MultiPolyLine(IEnumerable<IEnumerable<IEnumerable<IEnumerable<IEnumerable<double>>>>> coordinates)
            : this(coordinates?.Select(PolyLine => new PolyLine(PolyLine))
                   ?? throw new ArgumentNullException(nameof(coordinates)))
        {
        }

        public override GeoJSONObjectType Type => GeoJSONObjectType.MultiPolyLine;

        /// <summary>
        /// The list of PolyLines enclosed in this <see cref="MultiPolyLine"/>.
        /// </summary>
        [JsonProperty("coordinates", Required = Required.Always)]
        [JsonConverter(typeof(PolyLineEnumerableConverter))]
        public ReadOnlyCollection<PolyLine> Coordinates { get; }

        #region IEqualityComparer, IEquatable

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(this, obj as MultiPolyLine);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        public bool Equals(MultiPolyLine other)
        {
            return Equals(this, other);
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(MultiPolyLine left, MultiPolyLine right)
        {
            if (base.Equals(left, right))
            {
                return left.Coordinates.SequenceEqual(right.Coordinates);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public static bool operator ==(MultiPolyLine left, MultiPolyLine right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (ReferenceEquals(null, right))
            {
                return false;
            }
            return left != null && left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified object instances are not considered equal
        /// </summary>
        public static bool operator !=(MultiPolyLine left, MultiPolyLine right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            foreach (var item in Coordinates)
            {
                hash = (hash * 397) ^ item.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Returns the hash code for the specified object
        /// </summary>
        public int GetHashCode(MultiPolyLine other)
        {
            return other.GetHashCode();
        }

        #endregion
    }
}