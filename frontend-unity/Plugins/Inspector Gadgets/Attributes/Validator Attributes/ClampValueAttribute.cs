// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

using UnityEngine;

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// Specifies the range of values allowed by the attributed int or float field.
    /// See also: <see cref="MinValueAttribute"/> and <see cref="MaxValueAttribute"/>.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class ClampValueAttribute : ValidatorAttribute
    {
        /************************************************************************************************************************/

        /// <summary>The minimum allowed integer value.</summary>
        public readonly long MinLong;

        /// <summary>The minimum allowed floating point value.</summary>
        public readonly double MinDouble;

        /// <summary>The maximum allowed integer value.</summary>
        public readonly long MaxLong;

        /// <summary>The maximum allowed floating point value.</summary>
        public readonly double MaxDouble;

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="ClampValueAttribute"/> with the specified range.</summary>
        public ClampValueAttribute(int min, int max)
            : this((long)min, (long)max)
        { }

        /// <summary>Creates a new <see cref="ClampValueAttribute"/> with the specified range.</summary>
        public ClampValueAttribute(long min, long max)
        {
            MinLong = min;
            MinDouble = min;
            MaxLong = max;
            MaxDouble = max;

            Debug.Assert(min < max, "min must be less than max");
        }

        /// <summary>Creates a new <see cref="ClampValueAttribute"/> with the specified range.</summary>
        public ClampValueAttribute(float min, float max)
            : this((double)min, (double)max)
        { }

        /// <summary>Creates a new <see cref="ClampValueAttribute"/> with the specified range.</summary>
        public ClampValueAttribute(double min, double max)
        {
            MinLong = (long)min;
            MinDouble = min;
            MaxLong = (long)max;
            MaxDouble = max;

            Debug.Assert(min < max, "min must be less than max");
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Validate(UnityEditor.SerializedProperty property)
        {
            if (property.propertyType == UnityEditor.SerializedPropertyType.Integer)
            {
                var value = property.longValue;
                if (value < MinLong)
                    property.longValue = (int)MinLong;
                else if (value > MaxLong)
                    property.longValue = (int)MaxLong;
            }
            else if (property.propertyType == UnityEditor.SerializedPropertyType.Float)
            {
                var value = property.doubleValue;
                if (value < MinDouble)
                    property.doubleValue = (float)MinDouble;
                else if (value > MaxDouble)
                    property.doubleValue = (float)MaxDouble;
            }
        }

        /************************************************************************************************************************/
#endif
    }
}

