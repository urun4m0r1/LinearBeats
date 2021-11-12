#nullable enable
using System;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core;
using Unity.VisualScripting.YamlDotNet.Serialization;

namespace LinearBeats.Script
{
    public sealed class ValidatingDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer _nodeDeserializer;

        public ValidatingDeserializer(INodeDeserializer nodeDeserializer) => _nodeDeserializer = nodeDeserializer;

        public bool Deserialize(IParser parser, Type expectedType,
            Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
        {
            if (!_nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value) || value == null)
                return false;

            var attrType = typeof(YamlRequiredAttribute);
            var properties = expectedType.GetProperties();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes(attrType, false).FirstOrDefault();
                if (attribute is YamlRequiredAttribute && property.GetValue(value) == null)
                    throw new YamlException(parser.Current.Start, parser.Current.End,
                        $"YamlRequiredAttribute {property.Name} is null");
            }

            return true;
        }
    }
}
