// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// Specifies the maximum value allowed by the attributed int or float field.
    /// See also: <see cref="MinValueAttribute"/> and <see cref="ClampValueAttribute"/>.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class MaxValueAttribute : ValidatorAttribute
    {
        /************************************************************************************************************************/

        /// <summary>The maximum allowed integer value.</summary>
        public readonly long MaxLong;

        /// <summary>The maximum allowed floating point value.</summary>
        public readonly double MaxDouble;

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="MaxValueAttribute"/> with the specified maximum value.</summary>
        public MaxValueAttribute(int max) : this((long)max) { }

        /// <summary>Creates a new <see cref="MaxValueAttribute"/> with the specified maximum value.</summary>
        public MaxValueAttribute(long max)
        {
            MaxLong = max;
            MaxDouble = max;
        }

        /// <summary>Creates a new <see cref="MaxValueAttribute"/> with the specified maximum value.</summary>
        public MaxValueAttribute(float max) : this((double)max) { }

        /// <summary>Creates a new <see cref="MaxValueAttribute"/> with the specified maximum value.</summary>
        public MaxValueAttribute(double max)
        {
            MaxLong = (long)max;
            MaxDouble = max;
        }

        /************************************************************************************************************************/
#if UNITY_EDITOR
        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override void Validate(UnityEditor.SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case UnityEditor.SerializedPropertyType.Integer:
                    if (property.longValue > MaxLong)
                        property.longValue = (int)MaxLong;
                    break;

                case UnityEditor.SerializedPropertyType.Float:
                    if (property.doubleValue > MaxDouble)
                        property.doubleValue = MaxDouble;
                    break;
            }
        }

        /************************************************************************************************************************/
#endif
    }
}

