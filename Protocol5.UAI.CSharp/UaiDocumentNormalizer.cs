using System.Text.RegularExpressions;

namespace Protocol5.UAI;

public static class UaiDocumentNormalizer
{
    private static readonly Regex CollapseWhitespaceRegex = new(@"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static UaiDocument Normalize(UaiDocument document)
    {
        Guard.NotNull(document, nameof(document));

        document.Spec = NormalizeString(document.Spec) ?? UaiConstants.SpecName;
        document.Version = NormalizeString(document.Version) ?? UaiConstants.CurrentDocumentVersion;
        document.SchemaVersion = NormalizeString(document.SchemaVersion) ?? UaiConstants.CurrentSchemaVersion;
        document.DocumentId = NormalizeString(document.DocumentId) ?? string.Empty;

        NormalizeSource(document.Source);
        NormalizeMetadata(document.Metadata);

        document.Structure ??= new List<UaiNode>();
        document.Semantics ??= new List<UaiSemanticRecord>();
        document.Symbols ??= new List<UaiSymbolDefinition>();
        document.Assets ??= new List<UaiAsset>();
        document.Relationships ??= new List<UaiRelationship>();
        document.Annotations ??= new List<UaiAnnotation>();
        document.Extensions ??= new SortedDictionary<string, UaiExtensionValue>(StringComparer.Ordinal);

        foreach (var node in document.Structure)
        {
            NormalizeNode(node);
        }

        foreach (var semantic in document.Semantics)
        {
            semantic.Id = NormalizeString(semantic.Id) ?? string.Empty;
            semantic.Kind = NormalizeString(semantic.Kind) ?? string.Empty;
            semantic.Value = NormalizeString(semantic.Value) ?? string.Empty;
            semantic.Source = NormalizeString(semantic.Source) ?? "source";
            semantic.Targets = semantic.Targets?
                .Select(target => NormalizeString(target))
                .Where(target => !string.IsNullOrWhiteSpace(target))
                .Cast<string>()
                .Distinct(StringComparer.Ordinal)
                .ToList() ?? new List<string>();
            NormalizeInference(semantic.Inference);
        }

        foreach (var symbol in document.Symbols)
        {
            symbol.Id = NormalizeString(symbol.Id) ?? string.Empty;
            symbol.Label = NormalizeString(symbol.Label);
            symbol.VisualForm = NormalizeString(symbol.VisualForm) ?? string.Empty;
            symbol.SourceSystem = NormalizeString(symbol.SourceSystem);
            symbol.Notes = NormalizeString(symbol.Notes);
            symbol.StrokeLogic = NormalizeStringList(symbol.StrokeLogic);
            symbol.Variants = NormalizeStringList(symbol.Variants);
            NormalizeInference(symbol.Inference);

            if (symbol.Geometry is not null)
            {
                symbol.Geometry.Primitives = NormalizeStringList(symbol.Geometry.Primitives);
                symbol.Geometry.Symmetry = NormalizeString(symbol.Geometry.Symmetry);
                symbol.Geometry.Closure = NormalizeString(symbol.Geometry.Closure);
            }

            if (symbol.SourceEvidence is not null)
            {
                foreach (var evidence in symbol.SourceEvidence)
                {
                    evidence.Kind = NormalizeString(evidence.Kind) ?? string.Empty;
                    evidence.Value = NormalizeString(evidence.Value) ?? string.Empty;
                    evidence.Uri = NormalizeString(evidence.Uri);
                }
            }

            if (symbol.Relationships is not null)
            {
                foreach (var relationship in symbol.Relationships)
                {
                    NormalizeRelationship(relationship);
                }
            }

            foreach (var meaning in symbol.Meaning)
            {
                meaning.Value = NormalizeString(meaning.Value) ?? string.Empty;
                meaning.MeaningType = NormalizeString(meaning.MeaningType) ?? string.Empty;
                meaning.Origin = NormalizeString(meaning.Origin) ?? "source-provided";
            }
        }

        foreach (var asset in document.Assets)
        {
            asset.Id = NormalizeString(asset.Id) ?? string.Empty;
            asset.Kind = NormalizeString(asset.Kind) ?? string.Empty;
            asset.Uri = NormalizeString(asset.Uri) ?? string.Empty;
            asset.MimeType = NormalizeString(asset.MimeType) ?? string.Empty;
            asset.ContentHash = NormalizeString(asset.ContentHash);
            asset.Title = NormalizeString(asset.Title);
            asset.AltText = NormalizeString(asset.AltText);
        }

        foreach (var relationship in document.Relationships)
        {
            NormalizeRelationship(relationship);
        }

        foreach (var annotation in document.Annotations)
        {
            annotation.Id = NormalizeString(annotation.Id) ?? string.Empty;
            annotation.TargetId = NormalizeString(annotation.TargetId);
            annotation.Severity = NormalizeString(annotation.Severity) ?? "info";
            annotation.Code = NormalizeString(annotation.Code) ?? string.Empty;
            annotation.Message = NormalizeString(annotation.Message) ?? string.Empty;
            annotation.Source = NormalizeString(annotation.Source);
        }

        NormalizeProvenance(document.Provenance);

        document.Semantics = document.Semantics.OrderBy(item => item.Id, StringComparer.Ordinal).ToList();
        document.Symbols = document.Symbols.OrderBy(item => item.Id, StringComparer.Ordinal).ToList();
        document.Assets = document.Assets.OrderBy(item => item.Id, StringComparer.Ordinal).ToList();
        document.Relationships = document.Relationships.OrderBy(item => item.Id ?? item.Source, StringComparer.Ordinal).ToList();
        document.Annotations = document.Annotations.OrderBy(item => item.Id, StringComparer.Ordinal).ToList();

        return document;
    }

    public static string NormalizeLiteralText(string value)
    {
        Guard.NotNull(value, nameof(value));
        return CollapseWhitespaceRegex.Replace(value.Replace("\r\n", "\n").Replace('\r', '\n'), " ").Trim();
    }

    private static void NormalizeSource(UaiSourceDescriptor source)
    {
        source.Uri = NormalizeString(source.Uri) ?? string.Empty;
        source.Title = NormalizeString(source.Title);
        source.RetrievedAt = NormalizeString(source.RetrievedAt) ?? string.Empty;
        source.ContentHash = NormalizeString(source.ContentHash);
        source.MimeType = NormalizeString(source.MimeType);
        source.HtmlLanguage = NormalizeString(source.HtmlLanguage);
        source.CanonicalUri = NormalizeString(source.CanonicalUri);
    }

    private static void NormalizeMetadata(UaiMetadata metadata)
    {
        metadata.Title = NormalizeString(metadata.Title) ?? string.Empty;
        metadata.Description = NormalizeString(metadata.Description);
        metadata.Language = NormalizeString(metadata.Language) ?? "en";
        metadata.SiteName = NormalizeString(metadata.SiteName);
        metadata.PageType = NormalizeString(metadata.PageType) ?? "generic";
        metadata.Keywords = NormalizeStringList(metadata.Keywords);
        metadata.Authors = NormalizeStringList(metadata.Authors);
        metadata.PublishedAt = NormalizeString(metadata.PublishedAt);
        metadata.UpdatedAt = NormalizeString(metadata.UpdatedAt);
        metadata.CanonicalUrl = NormalizeString(metadata.CanonicalUrl);
        metadata.AlternateUrls = NormalizeStringList(metadata.AlternateUrls);
    }

    private static void NormalizeNode(UaiNode node)
    {
        node.Type = NormalizeString(node.Type) ?? string.Empty;
        node.Id = NormalizeString(node.Id) ?? string.Empty;
        node.Role = NormalizeString(node.Role);
        node.Label = NormalizeString(node.Label);
        node.SectionKind = NormalizeString(node.SectionKind);
        node.ListStyle = NormalizeString(node.ListStyle);
        node.AltText = NormalizeString(node.AltText);
        node.Href = NormalizeString(node.Href);
        node.Rel = NormalizeString(node.Rel);
        node.Target = NormalizeString(node.Target);
        node.LinkPurpose = NormalizeString(node.LinkPurpose);
        node.Method = NormalizeString(node.Method);
        node.Name = NormalizeString(node.Name);
        node.InputType = NormalizeString(node.InputType);
        node.Value = NormalizeString(node.Value);
        node.Placeholder = NormalizeString(node.Placeholder);
        node.Usage = NormalizeString(node.Usage);
        node.SymbolRef = NormalizeString(node.SymbolRef);
        node.Description = NormalizeString(node.Description);
        node.DiagramType = NormalizeString(node.DiagramType);
        node.Folio = NormalizeString(node.Folio);
        node.CalloutType = NormalizeString(node.CalloutType);
        node.RawContent = NormalizeString(node.RawContent);
        node.Reason = NormalizeString(node.Reason);
        node.SourceFragment = NormalizeString(node.SourceFragment);
        node.AssetRef = NormalizeString(node.AssetRef);
        node.ContainsSymbolRefs = NormalizeStringList(node.ContainsSymbolRefs);
        node.Aliases = NormalizeStringList(node.Aliases);
        node.SemanticRefs = NormalizeStringList(node.SemanticRefs);
        node.RelationshipRefs = NormalizeStringList(node.RelationshipRefs);
        node.AnnotationRefs = NormalizeStringList(node.AnnotationRefs);

        NormalizeText(node.Text);
        NormalizeText(node.Term);
        NormalizeText(node.Definition);
        NormalizeAction(node.Action);
        NormalizeInference(node.Inference);

        if (node.Options is not null)
        {
            foreach (var option in node.Options)
            {
                option.Label = NormalizeString(option.Label) ?? string.Empty;
                option.Value = NormalizeString(option.Value) ?? string.Empty;
            }
        }

        if (node.Columns is not null)
        {
            foreach (var column in node.Columns)
            {
                column.Id = NormalizeString(column.Id) ?? string.Empty;
                column.Label = NormalizeString(column.Label) ?? string.Empty;
            }
        }

        if (node.Rows is not null)
        {
            foreach (var row in node.Rows)
            {
                row.Id = NormalizeString(row.Id);
                foreach (var cell in row.Cells)
                {
                    NormalizeText(cell.Text);
                    cell.HeaderRef = NormalizeString(cell.HeaderRef);
                }
            }
        }

        if (node.Entries is not null)
        {
            foreach (var entry in node.Entries)
            {
                entry.Key = NormalizeString(entry.Key) ?? string.Empty;
                entry.Value = NormalizeString(entry.Value) ?? string.Empty;
                entry.Source = NormalizeString(entry.Source);
            }
        }

        if (node.SourceRef is not null)
        {
            node.SourceRef.Selector = NormalizeString(node.SourceRef.Selector);
            node.SourceRef.XPath = NormalizeString(node.SourceRef.XPath);
            node.SourceRef.DomPath = NormalizeString(node.SourceRef.DomPath);
            node.SourceRef.HtmlFragment = NormalizeString(node.SourceRef.HtmlFragment);
        }

        if (node.Children is not null)
        {
            foreach (var child in node.Children)
            {
                NormalizeNode(child);
            }
        }
    }

    private static void NormalizeAction(UaiAction? action)
    {
        if (action is null)
        {
            return;
        }

        action.Kind = NormalizeString(action.Kind) ?? "unknown";
        action.Target = NormalizeString(action.Target);
    }

    private static void NormalizeText(UaiTextValue? text)
    {
        if (text is null)
        {
            return;
        }

        text.Literal = NormalizeString(text.Literal) ?? string.Empty;
        text.Normalized = NormalizeString(text.Normalized) ?? NormalizeLiteralText(text.Literal);
        text.Language = NormalizeString(text.Language);
    }

    private static void NormalizeInference(UaiInference? inference)
    {
        if (inference is null)
        {
            return;
        }

        inference.Rationale = NormalizeString(inference.Rationale);
    }

    private static void NormalizeRelationship(UaiRelationship relationship)
    {
        relationship.Id = NormalizeString(relationship.Id);
        relationship.Relation = NormalizeString(relationship.Relation) ?? string.Empty;
        relationship.Source = NormalizeString(relationship.Source) ?? string.Empty;
        relationship.Target = relationship.Target?
            .Select(target => NormalizeString(target))
            .Where(target => !string.IsNullOrWhiteSpace(target))
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .ToList() ?? new List<string>();
    }

    private static void NormalizeProvenance(UaiProvenance provenance)
    {
        provenance.GeneratedAt = NormalizeString(provenance.GeneratedAt) ?? string.Empty;

        if (provenance.Generator is not null)
        {
            provenance.Generator.Name = NormalizeString(provenance.Generator.Name) ?? string.Empty;
            provenance.Generator.Version = NormalizeString(provenance.Generator.Version) ?? string.Empty;
        }

        if (provenance.Translator is not null)
        {
            provenance.Translator.Name = NormalizeString(provenance.Translator.Name) ?? string.Empty;
            provenance.Translator.Version = NormalizeString(provenance.Translator.Version) ?? string.Empty;
            provenance.Translator.ContractVersion = NormalizeString(provenance.Translator.ContractVersion) ?? UaiConstants.CurrentTranslatorContractVersion;
            provenance.Translator.Mode = NormalizeString(provenance.Translator.Mode) ?? "deterministic-html";
        }

        provenance.SourceAuthors = NormalizeStringList(provenance.SourceAuthors);

        if (provenance.Capture is not null)
        {
            provenance.Capture.Method = NormalizeString(provenance.Capture.Method) ?? string.Empty;
            provenance.Capture.Notes = NormalizeString(provenance.Capture.Notes);
        }

        if (provenance.History is not null)
        {
            foreach (var historyEntry in provenance.History)
            {
                historyEntry.Timestamp = NormalizeString(historyEntry.Timestamp) ?? string.Empty;
                historyEntry.Actor = NormalizeString(historyEntry.Actor) ?? string.Empty;
                historyEntry.Change = NormalizeString(historyEntry.Change) ?? string.Empty;
            }
        }
    }

    private static List<string>? NormalizeStringList(List<string>? values)
    {
        return values?
            .Select(NormalizeString)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    private static string? NormalizeString(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
