using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace LinearBeats.Script
{
    public static class YamlDotNetExtensions
    {
        public static DeserializerBuilder WithRequiredPropertyValidation(this DeserializerBuilder builder)
        {
            return builder
                .WithNodeDeserializer(inner => new ValidatingDeserializer(inner),
                    s => s.InsteadOf<ObjectNodeDeserializer>());
        }
    }
}
