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
    /// Defines the ConstrainedPolygon type.
    /// Coordinates of a ConstrainedPolygon are a list of closing MultilineString arrays. The first element in 
    /// the array represents the exterior ring. Any subsequent elements represent interior rings (or holes).
    /// The main difference with the Polygon type is that with ConstrainedPolygon each inner Lines extremities should not be affected by polygon transformation algorithms (mainly simplification).
    /// </summary>
    /// <remarks>
    /// See https://tools.ietf.org/html/rfc7946#section-3.1.6
    /// </remarks>
    public class ConstrainedPolygon : GeoJSONObject, IGeometryObject, IEqualityComparer<ConstrainedPolygon>, IEquatable<ConstrainedPolygon>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstrainedPolygon" /> class.
        /// </summary>
        /// <param name="coordinates">
        /// The linear rings with the first element in the array representing the exterior ring. 
        /// Any subsequent elements represent interior rings (or holes).
        /// </param>
        public ConstrainedPolygon(IEnumerable<MultiLineString> coordinates)
        {
            Coordinates = new ReadOnlyCollection<MultiLineString>(
                coordinates?.ToArray() ?? throw new ArgumentNullException(nameof(coordinates)));
            if (!Coordinates.Any() ^ Coordinates.Any(linearRing => !linearRing.IsLinearRing()))
            {
                throw new ArgumentException("All elements must be closed MultiLineStrings" +
                                            " (see GeoJSON spec at 'https://tools.ietf.org/html/rfc7946#section-3.1.6').", nameof(coordinates));
            }

            
        }

        /// <summary>
        /// Initializes a new <see cref="ConstrainedPolygon" /> from a 3-d array of <see cref="double" />s
        /// that matches the "coordinates" field in the JSON representation.
        /// </summary>
        [JsonConstructor]
        public ConstrainedPolygon(IEnumerable<IEnumerable<IEnumerable<IEnumerable<double>>>> coordinates)
            : this(coordinates?.Select(line => new MultiLineString(line))
              ?? throw new ArgumentNullException(nameof(coordinates)))
        {
        }

        public override GeoJSONObjectType Type => GeoJSONObjectType.ConstrainedPolygon;

        /// <summary>
        /// Gets the list of linestrings defining this <see cref="ConstrainedPolygon" />.
        /// </summary>
        [JsonProperty("coordinates", Required = Required.Always)]
        [JsonConverter(typeof(MultiLineStringEnumerableConverter))]
        public ReadOnlyCollection<MultiLineString> Coordinates { get; }

        #region IEqualityComparer, IEquatable

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(this, obj as ConstrainedPolygon);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        public bool Equals(ConstrainedPolygon other)
        {
            return Equals(this, other);
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal
        /// </summary>
        public bool Equals(ConstrainedPolygon left, ConstrainedPolygon right)
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
        public static bool operator ==(ConstrainedPolygon left, ConstrainedPolygon right)
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
        public static bool operator !=(ConstrainedPolygon left, ConstrainedPolygon right)
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
        public int GetHashCode(ConstrainedPolygon other)
        {
            return other.GetHashCode();
        }

        #endregion
    }
}