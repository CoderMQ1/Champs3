// Inspector Gadgets // https://kybernetik.com.au/inspector-gadgets // Copyright 2021 Kybernetik //

namespace InspectorGadgets.Attributes
{
    /// <summary>[Pro-Only]
    /// Specifies the minimum value allowed by the attributed int or float field.
    /// See also: <see cref="MaxValueAttribute"/> and <see cref="ClampValueAttribute"/>.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public sealed class MinValueAttribute : ValidatorAttribute
    {
        /************************************************************************************************************************/

        /// <summary>The minimum allowed integer value.</summary>
        public readonly long MinLong;

        /// <summary>The minimum allowed floating point value.</summary>
        public readonly double MinDouble;

        /************************************************************************************************************************/

        /// <summary>Creates a new <see cref="MinValueAttribute"/> with the specified minimum value.</summary>
        public MinValueAttribute(int min) : this((long)min) { }

        /// <summary>Creates a new <see cref="MinValueAttribute"/> with the specified minimum value.</summary>
        public MinValueAttribute(long min)
        {
            MinLong = min;
            MinDouble = min;
        }

        /// <summary>Creates a new <see cref="MinValueAttribute"/> with the specified minimum value.</summary>
        public MinValueAttribute(float min) : this((double)min) { }

        /// <summary>Creates a new <see cref="MinValueAttribute"/> with the specified minimum value.</summary>
        public MinValueAttribute(double min)
        {
            MinLong = (long)min;
            MinDouble = min;
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
                    if (property.longValue < MinLong)
                        property.longValue = (int)MinLong;
                    break;

                case UnityEditor.SerializedPropertyType.Float:
                    if (property.doubleValue < MinDouble)
                        property.doubleValue = MinDouble;
                    break;
            }
        }

        /************************************************************************************************************************/
#endif
    }
}

