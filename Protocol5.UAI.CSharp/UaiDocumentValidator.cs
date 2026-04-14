using System.Globalization;
using System.Text.RegularExpressions;

namespace Protocol5.UAI;

public sealed class UaiDocumentValidator
{
    private static readonly Regex SemVerRegex = new(@"^\d+\.\d+\.\d+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex ExtensionKeyRegex = new(@"^[a-z0-9]+(?:\.[a-z0-9-]+)+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex HashRegex = new(@"^[a-z0-9-]+:[A-Fa-f0-9]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly HashSet<string> AllowedPageTypes = new(StringComparer.Ordinal)
    {
        "generic",
        "homepage",
        "article",
        "landing-page",
        "navigation",
        "symbolic-manuscript",
        "wordpress-page",
        "gallery",
        "glossary",
        "reference"
    };

    private static readonly IReadOnlyDictionary<string, HashSet<string>> AllowedChildren = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
    {
        ["document"] = CreateSet("header", "section", "heading", "paragraph", "quote", "list", "table", "image", "figure", "button", "link", "navigation", "form", "glossaryEntry", "symbol", "glyphCluster", "diagram", "manuscriptPanel", "callout", "metadataBlock", "footer", "unknown"),
        ["header"] = CreateSet("section", "heading", "paragraph", "navigation", "image", "figure", "button", "link", "metadataBlock", "unknown"),
        ["footer"] = CreateSet("section", "paragraph", "navigation", "link", "button", "metadataBlock", "unknown"),
        ["section"] = CreateSet("heading", "paragraph", "quote", "list", "table", "image", "figure", "button", "link", "navigation", "form", "glossaryEntry", "symbol", "glyphCluster", "diagram", "manuscriptPanel", "callout", "metadataBlock", "section", "unknown"),
        ["navigation"] = CreateSet("link", "button", "list", "unknown"),
        ["list"] = CreateSet("listItem"),
        ["listItem"] = CreateSet("heading", "paragraph", "quote", "list", "link", "button", "image", "figure", "symbol", "glyphCluster", "diagram", "unknown"),
        ["figure"] = CreateSet("image", "caption", "symbol", "glyphCluster", "diagram", "paragraph", "unknown"),
        ["form"] = CreateSet("heading", "paragraph", "input", "button", "section", "unknown"),
        ["glyphCluster"] = CreateSet("symbol", "unknown"),
        ["manuscriptPanel"] = CreateSet("heading", "paragraph", "image", "figure", "symbol", "glyphCluster", "diagram", "caption", "unknown"),
        ["callout"] = CreateSet("heading", "paragraph", "quote", "list", "link", "button", "image", "figure", "metadataBlock", "symbol", "diagram", "section", "unknown")
    };

    public UaiValidationResult Validate(UaiDocument document)
    {
        Guard.NotNull(document, nameof(document));

        var errors = new List<UaiValidationError>();

        ValidateEquals(document.Spec, UaiConstants.SpecName, "$.spec", "uai.spec.invalid", errors);
        ValidateSemVer(document.Version, "$.version", "uai.version.invalid", errors);
        ValidateSemVer(document.SchemaVersion, "$.schemaVersion", "uai.schemaVersion.invalid", errors);
        ValidateRequired(document.DocumentId, "$.documentId", "uai.documentId.required", errors);
        ValidateRequired(document.Source.Uri, "$.source.uri", "uai.source.uri.required", errors);
        ValidateDateTime(document.Source.RetrievedAt, "$.source.retrievedAt", "uai.source.retrievedAt.invalid", errors);
        ValidateOptionalHash(document.Source.ContentHash, "$.source.contentHash", errors);
        ValidateRequired(document.Metadata.Title, "$.metadata.title", "uai.metadata.title.required", errors);
        ValidateRequired(document.Metadata.Language, "$.metadata.language", "uai.metadata.language.required", errors);

        if (!AllowedPageTypes.Contains(document.Metadata.PageType))
        {
            errors.Add(UaiValidationError.Error("$.metadata.pageType", "uai.metadata.pageType.invalid", $"Unsupported pageType '{document.Metadata.PageType}'."));
        }

        if (document.Structure.Count != 1)
        {
            errors.Add(UaiValidationError.Error("$.structure", "uai.structure.root.invalid", "UAI documents must contain exactly one root structure node."));
        }

        var nodeIds = new HashSet<string>(StringComparer.Ordinal);
        var semanticIds = new HashSet<string>(StringComparer.Ordinal);
        var symbolIds = new HashSet<string>(StringComparer.Ordinal);
        var assetIds = new HashSet<string>(StringComparer.Ordinal);
        var relationshipIds = new HashSet<string>(StringComparer.Ordinal);
        var annotationIds = new HashSet<string>(StringComparer.Ordinal);
        var allIds = new HashSet<string>(StringComparer.Ordinal);

        for (var index = 0; index < document.Structure.Count; index++)
        {
            ValidateNode(document.Structure[index], $"$.structure[{index}]", parentType: null, nodeIds, allIds, errors);
        }

        if (document.Structure.Count == 1 && document.Structure[0].Type == "document" && document.Structure[0].Id != document.DocumentId)
        {
            errors.Add(UaiValidationError.Error("$.structure[0].id", "uai.structure.root.idMismatch", "The root document node id must match documentId."));
        }

        for (var index = 0; index < document.Semantics.Count; index++)
        {
            var semantic = document.Semantics[index];
            var path = $"$.semantics[{index}]";
            AddUniqueId(semantic.Id, $"{path}.id", "uai.semantic.id.duplicate", semanticIds, errors);
            AddUniqueId(semantic.Id, $"{path}.id", "uai.id.duplicate", allIds, errors);
            ValidateRequired(semantic.Kind, $"{path}.kind", "uai.semantic.kind.required", errors);
            ValidateRequired(semantic.Value, $"{path}.value", "uai.semantic.value.required", errors);
            if (semantic.Targets.Count == 0)
            {
                errors.Add(UaiValidationError.Error($"{path}.targets", "uai.semantic.targets.required", "Semantic records must target at least one node or symbol."));
            }

            foreach (var target in semantic.Targets)
            {
                if (!nodeIds.Contains(target) && !symbolIds.Contains(target))
                {
                    // Symbols are collected later, so defer this specific check.
                    continue;
                }
            }

            ValidateInference(semantic.Inference, $"{path}.inference", errors);
        }

        for (var index = 0; index < document.Symbols.Count; index++)
        {
            var symbol = document.Symbols[index];
            var path = $"$.symbols[{index}]";
            AddUniqueId(symbol.Id, $"{path}.id", "uai.symbol.id.duplicate", symbolIds, errors);
            AddUniqueId(symbol.Id, $"{path}.id", "uai.id.duplicate", allIds, errors);
            ValidateRequired(symbol.VisualForm, $"{path}.visualForm", "uai.symbol.visualForm.required", errors);
            ValidateInference(symbol.Inference, $"{path}.inference", errors);

            foreach (var meaning in symbol.Meaning)
            {
                ValidateRequired(meaning.Value, $"{path}.meaning[].value", "uai.symbol.meaning.value.required", errors);
                ValidateRequired(meaning.MeaningType, $"{path}.meaning[].meaningType", "uai.symbol.meaning.type.required", errors);
                if (meaning.Origin == "inferred" && meaning.Confidence is null)
                {
                    errors.Add(UaiValidationError.Error($"{path}.meaning[].confidence", "uai.symbol.meaning.confidence.required", "Inferred symbol meanings must include confidence."));
                }

                ValidateConfidence(meaning.Confidence, $"{path}.meaning[].confidence", errors);
            }
        }

        for (var index = 0; index < document.Assets.Count; index++)
        {
            var asset = document.Assets[index];
            var path = $"$.assets[{index}]";
            AddUniqueId(asset.Id, $"{path}.id", "uai.asset.id.duplicate", assetIds, errors);
            AddUniqueId(asset.Id, $"{path}.id", "uai.id.duplicate", allIds, errors);
            ValidateRequired(asset.Kind, $"{path}.kind", "uai.asset.kind.required", errors);
            ValidateRequired(asset.Uri, $"{path}.uri", "uai.asset.uri.required", errors);
            ValidateRequired(asset.MimeType, $"{path}.mimeType", "uai.asset.mimeType.required", errors);
            ValidateOptionalHash(asset.ContentHash, $"{path}.contentHash", errors);
        }

        for (var index = 0; index < document.Relationships.Count; index++)
        {
            var relationship = document.Relationships[index];
            var path = $"$.relationships[{index}]";
            if (!string.IsNullOrWhiteSpace(relationship.Id))
            {
                AddUniqueId(relationship.Id, $"{path}.id", "uai.relationship.id.duplicate", relationshipIds, errors);
                AddUniqueId(relationship.Id, $"{path}.id", "uai.id.duplicate", allIds, errors);
            }

            ValidateRequired(relationship.Relation, $"{path}.relation", "uai.relationship.relation.required", errors);
            ValidateRequired(relationship.Source, $"{path}.source", "uai.relationship.source.required", errors);
            if (relationship.Target.Count == 0)
            {
                errors.Add(UaiValidationError.Error($"{path}.target", "uai.relationship.target.required", "Relationships must target at least one id."));
            }
        }

        for (var index = 0; index < document.Annotations.Count; index++)
        {
            var annotation = document.Annotations[index];
            var path = $"$.annotations[{index}]";
            AddUniqueId(annotation.Id, $"{path}.id", "uai.annotation.id.duplicate", annotationIds, errors);
            AddUniqueId(annotation.Id, $"{path}.id", "uai.id.duplicate", allIds, errors);
            ValidateRequired(annotation.Code, $"{path}.code", "uai.annotation.code.required", errors);
            ValidateRequired(annotation.Message, $"{path}.message", "uai.annotation.message.required", errors);
        }

        ValidateDateTime(document.Provenance.GeneratedAt, "$.provenance.generatedAt", "uai.provenance.generatedAt.invalid", errors);
        ValidateExtensions(document.Extensions, "$.extensions", errors);
        ValidateProvenance(document.Provenance, "$.provenance", errors);

        ValidateCrossReferences(document, nodeIds, semanticIds, symbolIds, assetIds, relationshipIds, annotationIds, errors);

        return new UaiValidationResult(errors);
    }

    private static void ValidateNode(
        UaiNode node,
        string path,
        string? parentType,
        HashSet<string> nodeIds,
        HashSet<string> allIds,
        List<UaiValidationError> errors)
    {
        ValidateRequired(node.Type, $"{path}.type", "uai.node.type.required", errors);
        ValidateRequired(node.Id, $"{path}.id", "uai.node.id.required", errors);

        if (!UaiConstants.NodeTypes.Contains(node.Type))
        {
            errors.Add(UaiValidationError.Error($"{path}.type", "uai.node.type.invalid", $"Unsupported node type '{node.Type}'."));
        }

        AddUniqueId(node.Id, $"{path}.id", "uai.node.id.duplicate", nodeIds, errors);
        AddUniqueId(node.Id, $"{path}.id", "uai.id.duplicate", allIds, errors);

        if (parentType is not null &&
            AllowedChildren.TryGetValue(parentType, out var allowedChildren) &&
            !allowedChildren.Contains(node.Type))
        {
            errors.Add(UaiValidationError.Error(path, "uai.node.child.disallowed", $"Node type '{node.Type}' is not allowed inside '{parentType}'."));
        }

        ValidateText(node.Text, $"{path}.text", required: node.Type is "heading" or "paragraph" or "quote" or "caption" or "button" or "link", errors);
        ValidateText(node.Term, $"{path}.term", required: node.Type == "glossaryEntry", errors);
        ValidateText(node.Definition, $"{path}.definition", required: node.Type == "glossaryEntry", errors);
        ValidateInference(node.Inference, $"{path}.inference", errors);
        ValidateExtensions(node.Extensions, $"{path}.extensions", errors);

        switch (node.Type)
        {
            case "document":
            case "header":
            case "footer":
            case "section":
            case "navigation":
            case "form":
            case "glyphCluster":
            case "manuscriptPanel":
            case "callout":
                ValidateChildrenRequired(node.Children, $"{path}.children", errors);
                break;
            case "list":
                if (node.Ordered is null)
                {
                    errors.Add(UaiValidationError.Error($"{path}.ordered", "uai.list.ordered.required", "List nodes must declare whether they are ordered."));
                }
                ValidateChildrenRequired(node.Children, $"{path}.children", errors);
                break;
            case "table":
                if (node.Columns is null || node.Columns.Count == 0)
                {
                    errors.Add(UaiValidationError.Error($"{path}.columns", "uai.table.columns.required", "Table nodes must include at least one column."));
                }
                if (node.Rows is null || node.Rows.Count == 0)
                {
                    errors.Add(UaiValidationError.Error($"{path}.rows", "uai.table.rows.required", "Table nodes must include at least one row."));
                }
                if (node.Columns is not null && node.Rows is not null)
                {
                    foreach (var row in node.Rows)
                    {
                        if (row.Cells.Count != node.Columns.Count)
                        {
                            errors.Add(UaiValidationError.Error($"{path}.rows", "uai.table.rows.cellCount", "Each table row must contain the same number of cells as the column count."));
                        }
                    }
                }
                break;
            case "image":
                ValidateRequired(node.AssetRef, $"{path}.assetRef", "uai.image.assetRef.required", errors);
                break;
            case "figure":
                ValidateChildrenRequired(node.Children, $"{path}.children", errors);
                break;
            case "button":
                if (node.Action is null)
                {
                    errors.Add(UaiValidationError.Error($"{path}.action", "uai.button.action.required", "Button nodes must define an action."));
                }
                break;
            case "link":
                ValidateRequired(node.Href, $"{path}.href", "uai.link.href.required", errors);
                break;
            case "input":
                ValidateRequired(node.InputType, $"{path}.inputType", "uai.input.type.required", errors);
                ValidateRequired(node.Name, $"{path}.name", "uai.input.name.required", errors);
                break;
            case "symbol":
                ValidateRequired(node.SymbolRef, $"{path}.symbolRef", "uai.symbol.ref.required", errors);
                break;
            case "diagram":
                ValidateRequired(node.AssetRef, $"{path}.assetRef", "uai.diagram.assetRef.required", errors);
                ValidateRequired(node.Description, $"{path}.description", "uai.diagram.description.required", errors);
                break;
            case "metadataBlock":
                if (node.Entries is null || node.Entries.Count == 0)
                {
                    errors.Add(UaiValidationError.Error($"{path}.entries", "uai.metadataBlock.entries.required", "Metadata blocks must include at least one entry."));
                }
                break;
            case "unknown":
                ValidateRequired(node.RawContent, $"{path}.rawContent", "uai.unknown.rawContent.required", errors);
                ValidateRequired(node.Reason, $"{path}.reason", "uai.unknown.reason.required", errors);
                break;
        }

        if (node.Type == "heading" && (node.Level is null || node.Level < 1 || node.Level > 6))
        {
            errors.Add(UaiValidationError.Error($"{path}.level", "uai.heading.level.invalid", "Heading levels must be between 1 and 6."));
        }

        if (node.Action is not null)
        {
            ValidateRequired(node.Action.Kind, $"{path}.action.kind", "uai.action.kind.required", errors);
        }

        if (node.Options is not null)
        {
            foreach (var option in node.Options)
            {
                ValidateRequired(option.Label, $"{path}.options[].label", "uai.input.option.label.required", errors);
                ValidateRequired(option.Value, $"{path}.options[].value", "uai.input.option.value.required", errors);
            }
        }

        if (node.Children is not null)
        {
            for (var childIndex = 0; childIndex < node.Children.Count; childIndex++)
            {
                ValidateNode(node.Children[childIndex], $"{path}.children[{childIndex}]", node.Type, nodeIds, allIds, errors);
            }
        }
    }

    private static void ValidateCrossReferences(
        UaiDocument document,
        HashSet<string> nodeIds,
        HashSet<string> semanticIds,
        HashSet<string> symbolIds,
        HashSet<string> assetIds,
        HashSet<string> relationshipIds,
        HashSet<string> annotationIds,
        List<UaiValidationError> errors)
    {
        foreach (var semantic in document.Semantics)
        {
            foreach (var target in semantic.Targets)
            {
                if (!nodeIds.Contains(target) && !symbolIds.Contains(target))
                {
                    errors.Add(UaiValidationError.Error("$.semantics", "uai.semantic.target.unknown", $"Semantic target '{target}' does not resolve to a node or symbol."));
                }
            }
        }

        foreach (var relationship in document.Relationships)
        {
            if (!nodeIds.Contains(relationship.Source) &&
                !symbolIds.Contains(relationship.Source) &&
                !assetIds.Contains(relationship.Source) &&
                relationship.Source != document.DocumentId)
            {
                errors.Add(UaiValidationError.Error("$.relationships", "uai.relationship.source.unknown", $"Relationship source '{relationship.Source}' is unknown."));
            }

            foreach (var target in relationship.Target)
            {
                if (!nodeIds.Contains(target) &&
                    !symbolIds.Contains(target) &&
                    !assetIds.Contains(target) &&
                    target != document.DocumentId)
                {
                    errors.Add(UaiValidationError.Error("$.relationships", "uai.relationship.target.unknown", $"Relationship target '{target}' is unknown."));
                }
            }
        }

        foreach (var annotation in document.Annotations)
        {
            if (!string.IsNullOrWhiteSpace(annotation.TargetId) &&
                !nodeIds.Contains(annotation.TargetId) &&
                !symbolIds.Contains(annotation.TargetId) &&
                !assetIds.Contains(annotation.TargetId))
            {
                errors.Add(UaiValidationError.Error("$.annotations", "uai.annotation.target.unknown", $"Annotation target '{annotation.TargetId}' is unknown."));
            }
        }

        foreach (var node in document.Structure.SelectMany(Flatten))
        {
            ValidateReferenceList(node.SemanticRefs, semanticIds, $"node '{node.Id}' semanticRefs", "uai.node.semanticRef.unknown", errors);
            ValidateReferenceList(node.RelationshipRefs, relationshipIds, $"node '{node.Id}' relationshipRefs", "uai.node.relationshipRef.unknown", errors);
            ValidateReferenceList(node.AnnotationRefs, annotationIds, $"node '{node.Id}' annotationRefs", "uai.node.annotationRef.unknown", errors);

            if (!string.IsNullOrWhiteSpace(node.AssetRef) && !assetIds.Contains(node.AssetRef))
            {
                errors.Add(UaiValidationError.Error("$.structure", "uai.node.assetRef.unknown", $"Asset reference '{node.AssetRef}' is unknown."));
            }

            if (!string.IsNullOrWhiteSpace(node.SymbolRef) && !symbolIds.Contains(node.SymbolRef))
            {
                errors.Add(UaiValidationError.Error("$.structure", "uai.node.symbolRef.unknown", $"Symbol reference '{node.SymbolRef}' is unknown."));
            }

            if (node.ContainsSymbolRefs is not null)
            {
                ValidateReferenceList(node.ContainsSymbolRefs, symbolIds, $"node '{node.Id}' containsSymbolRefs", "uai.node.containsSymbolRef.unknown", errors);
            }

            if (node.Columns is not null && node.Rows is not null)
            {
                var headerIds = node.Columns.Select(column => column.Id).ToHashSet(StringComparer.Ordinal);
                foreach (var row in node.Rows)
                {
                    foreach (var cell in row.Cells.Where(cell => !string.IsNullOrWhiteSpace(cell.HeaderRef)))
                    {
                        if (!headerIds.Contains(cell.HeaderRef!))
                        {
                            errors.Add(UaiValidationError.Error("$.structure", "uai.table.headerRef.unknown", $"Table headerRef '{cell.HeaderRef}' is unknown."));
                        }
                    }
                }
            }
        }
    }

    private static IEnumerable<UaiNode> Flatten(UaiNode node)
    {
        yield return node;

        if (node.Children is null)
        {
            yield break;
        }

        foreach (var child in node.Children.SelectMany(Flatten))
        {
            yield return child;
        }
    }

    private static void ValidateChildrenRequired(List<UaiNode>? children, string path, List<UaiValidationError> errors)
    {
        if (children is null || children.Count == 0)
        {
            errors.Add(UaiValidationError.Error(path, "uai.node.children.required", "This node type requires at least one child node."));
        }
    }

    private static void ValidateText(UaiTextValue? text, string path, bool required, List<UaiValidationError> errors)
    {
        if (text is null)
        {
            if (required)
            {
                errors.Add(UaiValidationError.Error(path, "uai.text.required", "A text object is required for this node type."));
            }

            return;
        }

        ValidateRequired(text.Literal, $"{path}.literal", "uai.text.literal.required", errors);
    }

    private static void ValidateInference(UaiInference? inference, string path, List<UaiValidationError> errors)
    {
        if (inference is null)
        {
            return;
        }

        ValidateConfidence(inference.Confidence, $"{path}.confidence", errors);
        if (inference.IsInferred)
        {
            ValidateRequired(inference.Rationale, $"{path}.rationale", "uai.inference.rationale.required", errors);
            if (inference.Confidence is null)
            {
                errors.Add(UaiValidationError.Error($"{path}.confidence", "uai.inference.confidence.required", "Inferred values must include confidence."));
            }
        }
    }

    private static void ValidateProvenance(UaiProvenance provenance, string path, List<UaiValidationError> errors)
    {
        if (provenance.Generator is not null)
        {
            ValidateRequired(provenance.Generator.Name, $"{path}.generator.name", "uai.provenance.generator.name.required", errors);
            ValidateRequired(provenance.Generator.Version, $"{path}.generator.version", "uai.provenance.generator.version.required", errors);
        }

        if (provenance.Translator is not null)
        {
            ValidateRequired(provenance.Translator.Name, $"{path}.translator.name", "uai.provenance.translator.name.required", errors);
            ValidateRequired(provenance.Translator.Version, $"{path}.translator.version", "uai.provenance.translator.version.required", errors);
            ValidateSemVer(provenance.Translator.ContractVersion, $"{path}.translator.contractVersion", "uai.provenance.translator.contractVersion.invalid", errors);
        }
    }

    private static void ValidateExtensions(SortedDictionary<string, UaiExtensionValue>? extensions, string path, List<UaiValidationError> errors)
    {
        if (extensions is null)
        {
            return;
        }

        foreach (var (key, value) in extensions)
        {
            if (!ExtensionKeyRegex.IsMatch(key))
            {
                errors.Add(UaiValidationError.Error(path, "uai.extension.key.invalid", $"Extension key '{key}' must use a reverse-DNS-style namespace such as 'vendor.feature'."));
            }

            ValidateSemVer(value.Version, $"{path}.{key}.version", "uai.extension.version.invalid", errors);
        }
    }

    private static void ValidateOptionalHash(string? value, string path, List<UaiValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!HashRegex.IsMatch(value))
        {
            errors.Add(UaiValidationError.Error(path, "uai.hash.invalid", "Hashes must use the form 'algorithm:hex'."));
        }
    }

    private static void ValidateSemVer(string? value, string path, string code, List<UaiValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value) || !SemVerRegex.IsMatch(value))
        {
            errors.Add(UaiValidationError.Error(path, code, "Version values must use semantic versioning, for example '1.0.0'."));
        }
    }

    private static void ValidateDateTime(string? value, string path, string code, List<UaiValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _))
        {
            errors.Add(UaiValidationError.Error(path, code, "Date values must be ISO-8601 timestamps."));
        }
    }

    private static void ValidateConfidence(decimal? confidence, string path, List<UaiValidationError> errors)
    {
        if (confidence is not null && (confidence < 0m || confidence > 1m))
        {
            errors.Add(UaiValidationError.Error(path, "uai.confidence.range", "Confidence values must be between 0.0 and 1.0 inclusive."));
        }
    }

    private static void ValidateRequired(string? value, string path, string code, List<UaiValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(UaiValidationError.Error(path, code, "A required value is missing."));
        }
    }

    private static void ValidateEquals(string? value, string expected, string path, string code, List<UaiValidationError> errors)
    {
        if (!string.Equals(value, expected, StringComparison.Ordinal))
        {
            errors.Add(UaiValidationError.Error(path, code, $"Expected '{expected}'."));
        }
    }

    private static void AddUniqueId(string id, string path, string code, HashSet<string> seen, List<UaiValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        if (!seen.Add(id))
        {
            errors.Add(UaiValidationError.Error(path, code, $"Duplicate id '{id}' was found."));
        }
    }

    private static void ValidateReferenceList(List<string>? references, HashSet<string> validIds, string label, string code, List<UaiValidationError> errors)
    {
        if (references is null)
        {
            return;
        }

        foreach (var reference in references)
        {
            if (!validIds.Contains(reference))
            {
                errors.Add(UaiValidationError.Error("$.structure", code, $"{label} contains unknown reference '{reference}'."));
            }
        }
    }

    private static HashSet<string> CreateSet(params string[] values)
    {
        return new HashSet<string>(values, StringComparer.Ordinal);
    }
}

public sealed class UaiValidationResult
{
    public UaiValidationResult(IReadOnlyList<UaiValidationError> errors)
    {
        Errors = errors;
    }

    public bool IsValid => Errors.Count == 0;

    public IReadOnlyList<UaiValidationError> Errors { get; }
}

public sealed class UaiValidationError
{
    public string Severity { get; set; } = "error";

    public string Path { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public static UaiValidationError Error(string path, string code, string message)
    {
        return new UaiValidationError
        {
            Severity = "error",
            Path = path,
            Code = code,
            Message = message
        };
    }
}
