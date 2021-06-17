using JetBrains.Annotations;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace LinearBeats.Script
{
    public static class YamlDotNetExtensions
    {
        public static DeserializerBuilder WithRequiredPropertyValidation([NotNull] this DeserializerBuilder builder) =>
            builder.WithNodeDeserializer(
                inner => new ValidatingDeserializer(inner),
                s => s.InsteadOf<ObjectNodeDeserializer>());
    }
}
