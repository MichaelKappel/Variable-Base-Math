#if NET8_0_OR_GREATER
namespace Protocol5.UAI;

public sealed class Protocol5UaiEndpointOptions
{
    public string DocumentVersion { get; set; } = UaiConstants.CurrentDocumentVersion;

    public bool EmitLegacyHeader { get; set; } = true;

    public bool EmitVaryHeader { get; set; } = true;

    public bool EmitDescribedByHeaders { get; set; } = true;

    public string RegistryPath { get; set; } = UaiConstants.CanonicalRegistryPublicPath;

    public string SchemaPath { get; set; } = UaiConstants.CanonicalSchemaPublicPath;
}
#endif