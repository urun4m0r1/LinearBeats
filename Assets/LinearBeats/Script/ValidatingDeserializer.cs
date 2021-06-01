using System;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace LinearBeats.Script
{
    public class ValidatingDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer _nodeDeserializer;

        public ValidatingDeserializer(INodeDeserializer nodeDeserializer)
        {
            _nodeDeserializer = nodeDeserializer;
        }

        public bool Deserialize(IParser parser, Type expectedType,
            Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
        {
            if (!_nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value) ||
                value == null)
            {
                return false;
            }

            var attrType = typeof(YamlRequiredAttribute);
            var properties = expectedType.GetProperties();

            foreach (var property in properties)
            {
                var attrs = property.GetCustomAttributes(attrType, false).FirstOrDefault();
                if (attrs is YamlRequiredAttribute && property.GetValue(value) == null)
                    throw new YamlException(parser.Current.Start, parser.Current.End, "NullException");
            }

            return true;
        }
    }
}
