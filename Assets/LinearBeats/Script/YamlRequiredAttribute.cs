using System;

namespace LinearBeats.Script
{
    /// <summary>
    /// Instructs the YamlSerializer should serialize the public field or public read/write property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class YamlRequiredAttribute : Attribute
    {
    }
}
