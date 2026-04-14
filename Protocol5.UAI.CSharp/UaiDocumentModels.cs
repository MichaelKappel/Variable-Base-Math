using System.Text.Json;
using System.Text.Json.Serialization;

namespace Protocol5.UAI;

public sealed class UaiDocument
{
    [JsonPropertyName("spec")]
    public string Spec { get; set; } = UaiConstants.SpecName;

    [JsonPropertyName("version")]
    public string Version { get; set; } = UaiConstants.CurrentDocumentVersion;

    [JsonPropertyName("schemaVersion")]
    public string SchemaVersion { get; set; } = UaiConstants.CurrentSchemaVersion;

    [JsonPropertyName("documentId")]
    public string DocumentId { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public UaiSourceDescriptor Source { get; set; } = new();

    [JsonPropertyName("metadata")]
    public UaiMetadata Metadata { get; set; } = new();

    [JsonPropertyName("structure")]
    public List<UaiNode> Structure { get; set; } = new();

    [JsonPropertyName("semantics")]
    public List<UaiSemanticRecord> Semantics { get; set; } = new();

    [JsonPropertyName("symbols")]
    public List<UaiSymbolDefinition> Symbols { get; set; } = new();

    [JsonPropertyName("assets")]
    public List<UaiAsset> Assets { get; set; } = new();

    [JsonPropertyName("relationships")]
    public List<UaiRelationship> Relationships { get; set; } = new();

    [JsonPropertyName("annotations")]
    public List<UaiAnnotation> Annotations { get; set; } = new();

    [JsonPropertyName("provenance")]
    public UaiProvenance Provenance { get; set; } = new();

    [JsonPropertyName("extensions")]
    public SortedDictionary<string, UaiExtensionValue> Extensions { get; set; } = new(StringComparer.Ordinal);
}

public sealed class UaiSourceDescriptor
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("retrievedAt")]
    public string RetrievedAt { get; set; } = string.Empty;

    [JsonPropertyName("contentHash")]
    public string? ContentHash { get; set; }

    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    [JsonPropertyName("htmlLanguage")]
    public string? HtmlLanguage { get; set; }

    [JsonPropertyName("canonicalUri")]
    public string? CanonicalUri { get; set; }
}

public sealed class UaiMetadata
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("language")]
    public string Language { get; set; } = "en";

    [JsonPropertyName("siteName")]
    public string? SiteName { get; set; }

    [JsonPropertyName("pageType")]
    public string PageType { get; set; } = "generic";

    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; set; }

    [JsonPropertyName("authors")]
    public List<string>? Authors { get; set; }

    [JsonPropertyName("publishedAt")]
    public string? PublishedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }

    [JsonPropertyName("canonicalUrl")]
    public string? CanonicalUrl { get; set; }

    [JsonPropertyName("alternateUrls")]
    public List<string>? AlternateUrls { get; set; }
}

public sealed class UaiNode
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("level")]
    public int? Level { get; set; }

    [JsonPropertyName("text")]
    public UaiTextValue? Text { get; set; }

    [JsonPropertyName("children")]
    public List<UaiNode>? Children { get; set; }

    [JsonPropertyName("sectionKind")]
    public string? SectionKind { get; set; }

    [JsonPropertyName("ordered")]
    public bool? Ordered { get; set; }

    [JsonPropertyName("listStyle")]
    public string? ListStyle { get; set; }

    [JsonPropertyName("columns")]
    public List<UaiTableColumn>? Columns { get; set; }

    [JsonPropertyName("rows")]
    public List<UaiTableRow>? Rows { get; set; }

    [JsonPropertyName("assetRef")]
    public string? AssetRef { get; set; }

    [JsonPropertyName("altText")]
    public string? AltText { get; set; }

    [JsonPropertyName("decorative")]
    public bool? Decorative { get; set; }

    [JsonPropertyName("href")]
    public string? Href { get; set; }

    [JsonPropertyName("rel")]
    public string? Rel { get; set; }

    [JsonPropertyName("target")]
    public string? Target { get; set; }

    [JsonPropertyName("linkPurpose")]
    public string? LinkPurpose { get; set; }

    [JsonPropertyName("action")]
    public UaiAction? Action { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("inputType")]
    public string? InputType { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonPropertyName("options")]
    public List<UaiInputOption>? Options { get; set; }

    [JsonPropertyName("term")]
    public UaiTextValue? Term { get; set; }

    [JsonPropertyName("definition")]
    public UaiTextValue? Definition { get; set; }

    [JsonPropertyName("aliases")]
    public List<string>? Aliases { get; set; }

    [JsonPropertyName("symbolRef")]
    public string? SymbolRef { get; set; }

    [JsonPropertyName("usage")]
    public string? Usage { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("diagramType")]
    public string? DiagramType { get; set; }

    [JsonPropertyName("containsSymbolRefs")]
    public List<string>? ContainsSymbolRefs { get; set; }

    [JsonPropertyName("panelNumber")]
    public int? PanelNumber { get; set; }

    [JsonPropertyName("folio")]
    public string? Folio { get; set; }

    [JsonPropertyName("calloutType")]
    public string? CalloutType { get; set; }

    [JsonPropertyName("entries")]
    public List<UaiMetadataEntry>? Entries { get; set; }

    [JsonPropertyName("rawContent")]
    public string? RawContent { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("sourceFragment")]
    public string? SourceFragment { get; set; }

    [JsonPropertyName("semanticRefs")]
    public List<string>? SemanticRefs { get; set; }

    [JsonPropertyName("relationshipRefs")]
    public List<string>? RelationshipRefs { get; set; }

    [JsonPropertyName("annotationRefs")]
    public List<string>? AnnotationRefs { get; set; }

    [JsonPropertyName("sourceRef")]
    public UaiSourceReference? SourceRef { get; set; }

    [JsonPropertyName("inference")]
    public UaiInference? Inference { get; set; }

    [JsonPropertyName("extensions")]
    public SortedDictionary<string, UaiExtensionValue>? Extensions { get; set; }
}

public sealed class UaiTextValue
{
    [JsonPropertyName("literal")]
    public string Literal { get; set; } = string.Empty;

    [JsonPropertyName("normalized")]
    public string? Normalized { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }
}

public sealed class UaiTableColumn
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}

public sealed class UaiTableRow
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("cells")]
    public List<UaiTableCell> Cells { get; set; } = new();
}

public sealed class UaiTableCell
{
    [JsonPropertyName("text")]
    public UaiTextValue Text { get; set; } = new();

    [JsonPropertyName("headerRef")]
    public string? HeaderRef { get; set; }
}

public sealed class UaiAction
{
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "unknown";

    [JsonPropertyName("target")]
    public string? Target { get; set; }
}

public sealed class UaiInputOption
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

public sealed class UaiMetadataEntry
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string? Source { get; set; }
}

public sealed class UaiSourceReference
{
    [JsonPropertyName("selector")]
    public string? Selector { get; set; }

    [JsonPropertyName("xpath")]
    public string? XPath { get; set; }

    [JsonPropertyName("domPath")]
    public string? DomPath { get; set; }

    [JsonPropertyName("htmlFragment")]
    public string? HtmlFragment { get; set; }
}

public sealed class UaiSemanticRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("targets")]
    public List<string> Targets { get; set; } = new();

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; set; } = "source";

    [JsonPropertyName("inference")]
    public UaiInference? Inference { get; set; }
}

public sealed class UaiSymbolDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("visualForm")]
    public string VisualForm { get; set; } = string.Empty;

    [JsonPropertyName("geometry")]
    public UaiSymbolGeometry? Geometry { get; set; }

    [JsonPropertyName("strokeLogic")]
    public List<string>? StrokeLogic { get; set; }

    [JsonPropertyName("meaning")]
    public List<UaiSymbolMeaning> Meaning { get; set; } = new();

    [JsonPropertyName("sourceSystem")]
    public string? SourceSystem { get; set; }

    [JsonPropertyName("sourceEvidence")]
    public List<UaiSourceEvidence>? SourceEvidence { get; set; }

    [JsonPropertyName("inference")]
    public UaiInference Inference { get; set; } = new();

    [JsonPropertyName("variants")]
    public List<string>? Variants { get; set; }

    [JsonPropertyName("relationships")]
    public List<UaiRelationship>? Relationships { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

public sealed class UaiSymbolGeometry
{
    [JsonPropertyName("primitives")]
    public List<string>? Primitives { get; set; }

    [JsonPropertyName("symmetry")]
    public string? Symmetry { get; set; }

    [JsonPropertyName("closure")]
    public string? Closure { get; set; }
}

public sealed class UaiSymbolMeaning
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("meaningType")]
    public string MeaningType { get; set; } = string.Empty;

    [JsonPropertyName("origin")]
    public string Origin { get; set; } = "source-provided";

    [JsonPropertyName("confidence")]
    public decimal? Confidence { get; set; }
}

public sealed class UaiSourceEvidence
{
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}

public sealed class UaiAsset
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;

    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;

    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; } = string.Empty;

    [JsonPropertyName("contentHash")]
    public string? ContentHash { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("altText")]
    public string? AltText { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("decorative")]
    public bool? Decorative { get; set; }
}

public sealed class UaiRelationship
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("relation")]
    public string Relation { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("target")]
    public List<string> Target { get; set; } = new();
}

public sealed class UaiAnnotation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("targetId")]
    public string? TargetId { get; set; }

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = "info";

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string? Source { get; set; }
}

public sealed class UaiProvenance
{
    [JsonPropertyName("generatedAt")]
    public string GeneratedAt { get; set; } = string.Empty;

    [JsonPropertyName("generator")]
    public UaiAgentIdentity? Generator { get; set; }

    [JsonPropertyName("translator")]
    public UaiTranslatorIdentity? Translator { get; set; }

    [JsonPropertyName("sourceAuthors")]
    public List<string>? SourceAuthors { get; set; }

    [JsonPropertyName("capture")]
    public UaiCaptureInfo? Capture { get; set; }

    [JsonPropertyName("history")]
    public List<UaiRevisionEntry>? History { get; set; }
}

public class UaiAgentIdentity
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}

public sealed class UaiTranslatorIdentity : UaiAgentIdentity
{
    [JsonPropertyName("contractVersion")]
    public string ContractVersion { get; set; } = UaiConstants.CurrentTranslatorContractVersion;

    [JsonPropertyName("mode")]
    public string Mode { get; set; } = "deterministic-html";
}

public sealed class UaiCaptureInfo
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

public sealed class UaiRevisionEntry
{
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("actor")]
    public string Actor { get; set; } = string.Empty;

    [JsonPropertyName("change")]
    public string Change { get; set; } = string.Empty;
}

public sealed class UaiInference
{
    [JsonPropertyName("isInferred")]
    public bool IsInferred { get; set; }

    [JsonPropertyName("rationale")]
    public string? Rationale { get; set; }

    [JsonPropertyName("confidence")]
    public decimal? Confidence { get; set; }
}

public sealed class UaiExtensionValue
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("payload")]
    public JsonElement Payload { get; set; }
}
