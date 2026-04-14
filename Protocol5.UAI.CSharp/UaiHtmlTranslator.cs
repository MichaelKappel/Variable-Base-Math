using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

namespace Protocol5.UAI;

public sealed class UaiHtmlTranslator
{
    public UaiDocument Translate(string html, UaiHtmlTranslationOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            throw new ArgumentException("HTML input cannot be null or whitespace.", nameof(html));
        }

        options ??= new UaiHtmlTranslationOptions();

        var htmlDocument = new HtmlDocument();
        htmlDocument.OptionAutoCloseOnEnd = true;
        htmlDocument.LoadHtml(html);

        var htmlNode = htmlDocument.DocumentNode.SelectSingleNode("/html");
        var headNode = htmlDocument.DocumentNode.SelectSingleNode("//head");
        var bodyNode = htmlDocument.DocumentNode.SelectSingleNode("//body") ?? htmlDocument.DocumentNode;

        var sourceUri = options.SourceUri ?? GetCanonicalSourceUri(headNode) ?? "about:blank";
        var title = GetTitle(headNode) ?? GetFirstHeading(bodyNode) ?? "Untitled";
        var description = GetMetaContent(headNode, "description");
        var language = options.Language ?? htmlNode?.GetAttributeValue("lang", null) ?? options.DefaultLanguage;
        var documentId = options.DocumentId ?? BuildSlug(sourceUri, title);
        var retrievedAt = (options.RetrievedAt ?? DateTimeOffset.UtcNow).ToString("O", CultureInfo.InvariantCulture);

        var document = new UaiDocument
        {
            DocumentId = documentId,
            Source = new UaiSourceDescriptor
            {
                Uri = sourceUri,
                Title = title,
                RetrievedAt = retrievedAt,
                ContentHash = options.ContentHash,
                MimeType = "text/html",
                HtmlLanguage = language,
                CanonicalUri = GetCanonicalSourceUri(headNode) ?? sourceUri
            },
            Metadata = new UaiMetadata
            {
                Title = title,
                Description = description,
                Language = language,
                SiteName = GetMetaProperty(headNode, "og:site_name") ?? options.SiteName,
                PageType = options.PageType
            },
            Provenance = new UaiProvenance
            {
                GeneratedAt = retrievedAt,
                Generator = new UaiAgentIdentity
                {
                    Name = options.GeneratorName,
                    Version = options.GeneratorVersion
                },
                Translator = new UaiTranslatorIdentity
                {
                    Name = options.TranslatorName,
                    Version = options.TranslatorVersion,
                    ContractVersion = UaiConstants.CurrentTranslatorContractVersion,
                    Mode = "deterministic-html"
                },
                Capture = new UaiCaptureInfo
                {
                    Method = "html-dom-translation",
                    Notes = options.CaptureNotes
                }
            }
        };

        var context = new TranslationContext(document, documentId, options);
        var rootNode = new UaiNode
        {
            Type = "document",
            Id = documentId,
            Label = title,
            Children = new List<UaiNode>()
        };

        foreach (var child in bodyNode.ChildNodes.Where(IsMeaningfulNode))
        {
            foreach (var translatedNode in TranslateNode(child, context))
            {
                rootNode.Children!.Add(translatedNode);
            }
        }

        if (rootNode.Children.Count == 0)
        {
            rootNode.Children.Add(new UaiNode
            {
                Type = "unknown",
                Id = context.NextId("unknown"),
                Reason = "empty-body",
                RawContent = NormalizeText(bodyNode.InnerHtml),
                SourceFragment = bodyNode.OuterHtml
            });
        }

        document.Structure.Add(rootNode);
        UaiDocumentNormalizer.Normalize(document);
        return document;
    }

    private static IEnumerable<UaiNode> TranslateNode(HtmlNode node, TranslationContext context)
    {
        if (!IsMeaningfulNode(node))
        {
            yield break;
        }

        switch (node.Name.ToLowerInvariant())
        {
            case "header":
                yield return TranslateContainer(node, context, "header");
                yield break;
            case "footer":
                yield return TranslateContainer(node, context, "footer");
                yield break;
            case "nav":
                yield return TranslateNavigation(node, context);
                yield break;
            case "main":
                foreach (var child in TranslateContainerChildren(node, context))
                {
                    yield return child;
                }
                yield break;
            case "section":
            case "article":
            case "aside":
            case "div":
                if (IsDecorativeOnly(node))
                {
                    yield break;
                }

                if (ShouldFlattenContainer(node))
                {
                    foreach (var child in TranslateContainerChildren(node, context))
                    {
                        yield return child;
                    }
                    yield break;
                }

                if (TryTranslateDefinitionList(node, context, out var definitionNode))
                {
                    yield return definitionNode;
                    yield break;
                }

                if (IsNavigationLike(node))
                {
                    yield return TranslateNavigation(node, context);
                    yield break;
                }

                if (IsCalloutLike(node))
                {
                    yield return TranslateContainer(node, context, "callout", node.GetAttributeValue("data-uai-callout-type", "info"));
                    yield break;
                }

                yield return TranslateContainer(node, context, "section");
                yield break;
            case "h1":
            case "h2":
            case "h3":
            case "h4":
            case "h5":
            case "h6":
                yield return new UaiNode
                {
                    Type = "heading",
                    Id = context.NextId("heading"),
                    Level = int.Parse(node.Name.Substring(1), CultureInfo.InvariantCulture),
                    Text = BuildText(node, context),
                    SourceRef = BuildSourceRef(node)
                };
                yield break;
            case "p":
                yield return new UaiNode
                {
                    Type = "paragraph",
                    Id = context.NextId("paragraph"),
                    Text = BuildText(node, context),
                    SourceRef = BuildSourceRef(node)
                };
                yield break;
            case "blockquote":
                yield return new UaiNode
                {
                    Type = "quote",
                    Id = context.NextId("quote"),
                    Text = BuildText(node, context),
                    SourceRef = BuildSourceRef(node)
                };
                yield break;
            case "ul":
            case "ol":
                yield return TranslateList(node, context);
                yield break;
            case "table":
                yield return TranslateTable(node, context);
                yield break;
            case "figure":
                yield return TranslateFigure(node, context);
                yield break;
            case "img":
                yield return TranslateImageLike(node, context);
                yield break;
            case "figcaption":
                yield return new UaiNode
                {
                    Type = "caption",
                    Id = context.NextId("caption"),
                    Text = BuildText(node, context),
                    SourceRef = BuildSourceRef(node)
                };
                yield break;
            case "a":
                yield return TranslateAnchor(node, context);
                yield break;
            case "button":
                yield return TranslateButton(node, context);
                yield break;
            case "form":
                yield return TranslateForm(node, context);
                yield break;
            case "input":
            case "select":
            case "textarea":
                yield return TranslateInput(node, context);
                yield break;
            case "dl":
                if (TryTranslateDefinitionList(node, context, out var dlNode))
                {
                    yield return dlNode;
                }
                else if (context.Options.PreserveUnsupportedAsUnknown)
                {
                    yield return TranslateUnknown(node, context, "unsupported-definition-list");
                }
                yield break;
            case "svg":
                yield return TranslateSvg(node, context);
                yield break;
            case "canvas":
            case "iframe":
            case "video":
            case "audio":
                if (context.Options.PreserveUnsupportedAsUnknown)
                {
                    yield return TranslateUnknown(node, context, $"unsupported-{node.Name.ToLowerInvariant()}");
                }
                yield break;
            default:
                if (context.Options.PreserveUnsupportedAsUnknown)
                {
                    yield return TranslateUnknown(node, context, $"unsupported-{node.Name.ToLowerInvariant()}");
                }
                yield break;
        }
    }

    private static UaiNode TranslateContainer(HtmlNode node, TranslationContext context, string nodeType, string? variant = null)
    {
        var translatedChildren = TranslateContainerChildren(node, context).ToList();
        if (translatedChildren.Count == 0)
        {
            translatedChildren.Add(TranslateUnknown(node, context, "empty-container"));
        }

        return new UaiNode
        {
            Type = nodeType,
            Id = context.NextId(nodeType),
            Label = GetAriaLabel(node),
            CalloutType = nodeType == "callout" ? variant : null,
            SectionKind = nodeType == "section" ? DetectSectionKind(node) : null,
            Children = translatedChildren,
            SourceRef = BuildSourceRef(node)
        };
    }

    private static IEnumerable<UaiNode> TranslateContainerChildren(HtmlNode node, TranslationContext context)
    {
        foreach (var child in node.ChildNodes.Where(IsMeaningfulNode))
        {
            foreach (var translatedNode in TranslateNode(child, context))
            {
                yield return translatedNode;
            }
        }
    }

    private static UaiNode TranslateNavigation(HtmlNode node, TranslationContext context)
    {
        var translatedChildren = new List<UaiNode>();
        foreach (var child in node.ChildNodes.Where(IsMeaningfulNode))
        {
            if (child.Name.Equals("a", StringComparison.OrdinalIgnoreCase))
            {
                translatedChildren.Add(TranslateAnchor(child, context));
                continue;
            }

            if (child.Name.Equals("button", StringComparison.OrdinalIgnoreCase))
            {
                translatedChildren.Add(TranslateButton(child, context));
                continue;
            }

            if (child.Name.Equals("ul", StringComparison.OrdinalIgnoreCase) ||
                child.Name.Equals("ol", StringComparison.OrdinalIgnoreCase) ||
                child.Name.Equals("div", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var translatedChild in TranslateContainerChildren(child, context))
                {
                    translatedChildren.Add(translatedChild);
                }
            }
        }

        if (translatedChildren.Count == 0 && context.Options.PreserveUnsupportedAsUnknown)
        {
            translatedChildren.Add(TranslateUnknown(node, context, "empty-navigation"));
        }

        return new UaiNode
        {
            Type = "navigation",
            Id = context.NextId("navigation"),
            Label = GetAriaLabel(node),
            Children = translatedChildren,
            SourceRef = BuildSourceRef(node)
        };
    }

    private static UaiNode TranslateList(HtmlNode node, TranslationContext context)
    {
        var listItems = new List<UaiNode>();
        foreach (var item in node.Elements("li"))
        {
            var translatedChildren = item.ChildNodes
                .Where(IsMeaningfulNode)
                .SelectMany(child => TranslateNode(child, context))
                .ToList();

            var textValue = BuildText(item, context);
            if (translatedChildren.Count == 1 && translatedChildren[0].Type is "paragraph" or "heading")
            {
                textValue = translatedChildren[0].Text;
                translatedChildren.Clear();
            }

            listItems.Add(new UaiNode
            {
                Type = "listItem",
                Id = context.NextId("listItem"),
                Text = textValue,
                Children = translatedChildren.Count > 0 ? translatedChildren : null,
                SourceRef = BuildSourceRef(item)
            });
        }

        return new UaiNode
        {
            Type = "list",
            Id = context.NextId("list"),
            Ordered = node.Name.Equals("ol", StringComparison.OrdinalIgnoreCase),
            Children = listItems,
            SourceRef = BuildSourceRef(node)
        };
    }

    private static UaiNode TranslateTable(HtmlNode node, TranslationContext context)
    {
        var table = new UaiNode
        {
            Type = "table",
            Id = context.NextId("table"),
            Columns = new List<UaiTableColumn>(),
            Rows = new List<UaiTableRow>(),
            SourceRef = BuildSourceRef(node)
        };

        var headerCells = node.SelectNodes(".//thead//th|.//tr[1]/th");
        if (headerCells is not null)
        {
            foreach (var headerCell in headerCells)
            {
                table.Columns.Add(new UaiTableColumn
                {
                    Id = context.NextId("column"),
                    Label = ExtractLiteralText(headerCell)
                });
            }
        }

        if (table.Columns.Count == 0)
        {
            var firstRowCells = node.SelectNodes(".//tr[1]/td");
            if (firstRowCells is not null)
            {
                var index = 1;
                foreach (var _ in firstRowCells)
                {
                    table.Columns.Add(new UaiTableColumn
                    {
                        Id = context.NextId("column"),
                        Label = $"Column {index++}"
                    });
                }
            }
        }

        var rowNodes = node.SelectNodes(".//tbody/tr|.//tr") ?? new HtmlNodeCollection(node);
        foreach (var rowNode in rowNodes)
        {
            var cells = rowNode.SelectNodes("./th|./td");
            if (cells is null || cells.Count == 0)
            {
                continue;
            }

            var row = new UaiTableRow
            {
                Id = context.NextId("row")
            };

            for (var index = 0; index < cells.Count; index++)
            {
                var literal = ExtractLiteralText(cells[index]);
                row.Cells.Add(new UaiTableCell
                {
                    HeaderRef = index < table.Columns.Count ? table.Columns[index].Id : null,
                    Text = new UaiTextValue
                    {
                        Literal = literal,
                        Normalized = UaiDocumentNormalizer.NormalizeLiteralText(literal)
                    }
                });
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private static UaiNode TranslateFigure(HtmlNode node, TranslationContext context)
    {
        var children = new List<UaiNode>();
        foreach (var child in node.ChildNodes.Where(IsMeaningfulNode))
        {
            foreach (var translatedChild in TranslateNode(child, context))
            {
                children.Add(translatedChild);
            }
        }

        return new UaiNode
        {
            Type = "figure",
            Id = context.NextId("figure"),
            Children = children,
            SourceRef = BuildSourceRef(node)
        };
    }

    private static UaiNode TranslateImageLike(HtmlNode node, TranslationContext context)
    {
        if (TryCreateSymbolNode(node, context, out var symbolNode))
        {
            return symbolNode;
        }

        var assetId = context.GetOrCreateAsset(
            node.GetAttributeValue("src", string.Empty),
            "image",
            InferImageMimeType(node.GetAttributeValue("src", string.Empty)),
            node.GetAttributeValue("alt", null),
            TryParseInt(node.GetAttributeValue("width", null)),
            TryParseInt(node.GetAttributeValue("height", null)),
            node.GetAttributeValue("aria-hidden", "false").Equals("true", StringComparison.OrdinalIgnoreCase));

        if (LooksLikeDiagram(node))
        {
            return new UaiNode
            {
                Type = "diagram",
                Id = context.NextId("diagram"),
                AssetRef = assetId,
                Description = node.GetAttributeValue("alt", null) ?? node.GetAttributeValue("title", null) ?? "diagram",
                SourceRef = BuildSourceRef(node)
            };
        }

        return new UaiNode
        {
            Type = "image",
            Id = context.NextId("image"),
            AssetRef = assetId,
            AltText = node.GetAttributeValue("alt", null),
            Decorative = node.GetAttributeValue("aria-hidden", "false").Equals("true", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(node.GetAttributeValue("alt", null)),
            SourceRef = BuildSourceRef(node)
        };
    }

    private static UaiNode TranslateAnchor(HtmlNode node, TranslationContext context)
    {
        if (LooksLikeButton(node))
        {
            return TranslateButton(node, context);
        }

        return new UaiNode
        {
            Type = "link",
            Id = context.NextId("link"),
            Text = BuildInteractiveText(node, context),
            Href = node.GetAttributeValue("href", string.Empty),
            Rel = node.GetAttributeValue("rel", null),
            Target = node.GetAttributeValue("target", null),
            LinkPurpose = DetermineLinkPurpose(node),
            SourceRef = BuildSourceRef(node)
        };
    }

    private static UaiNode TranslateButton(HtmlNode node, TranslationContext context)
    {
        var actionTarget = node.GetAttributeValue("href", null) ?? node.GetAttributeValue("formaction", null);
        var actionKind = node.Name.Equals("a", StringComparison.OrdinalIgnoreCase)
            ? "link"
            : node.GetAttributeValue("type", "button").Equals("submit", StringComparison.OrdinalIgnoreCase)
                ? "submit"
                : "button";

        return new UaiNode
        {
            Type = "button",
            Id = context.NextId("button"),
            Text = BuildInteractiveText(node, context),
            Action = new UaiAction
            {
                Kind = actionKind,
                Target = actionTarget
            },
            SourceRef = BuildSourceRef(node)
        };
    }

    private static UaiNode TranslateForm(HtmlNode node, TranslationContext context)
    {
        var children = new List<UaiNode>();
        foreach (var child in node.ChildNodes.Where(IsMeaningfulNode))
        {
            foreach (var translatedChild in TranslateNode(child, context))
            {
                children.Add(translatedChild);
            }
        }

        return new UaiNode
        {
            Type = "form",
            Id = context.NextId("form"),
            Method = node.GetAttributeValue("method", "post"),
            Action = new UaiAction
            {
                Kind = "submit",
                Target = node.GetAttributeValue("action", null)
            },
            Children = children,
            SourceRef = BuildSourceRef(node)
        };
    }

    private static UaiNode TranslateInput(HtmlNode node, TranslationContext context)
    {
        if (node.Name.Equals("select", StringComparison.OrdinalIgnoreCase))
        {
            return new UaiNode
            {
                Type = "input",
                Id = context.NextId("input"),
                InputType = "select",
                Name = node.GetAttributeValue("name", string.Empty),
                Label = GetAssociatedLabel(node),
                Options = node.Elements("option")
                    .Select(option => new UaiInputOption
                    {
                        Label = ExtractLiteralText(option),
                        Value = option.GetAttributeValue("value", ExtractLiteralText(option))
                    })
                    .ToList(),
                Required = node.Attributes["required"] is not null,
                SourceRef = BuildSourceRef(node)
            };
        }

        if (node.Name.Equals("textarea", StringComparison.OrdinalIgnoreCase))
        {
            return new UaiNode
            {
                Type = "input",
                Id = context.NextId("input"),
                InputType = "textarea",
                Name = node.GetAttributeValue("name", string.Empty),
                Label = GetAssociatedLabel(node),
                Value = ExtractLiteralText(node),
                Placeholder = node.GetAttributeValue("placeholder", null),
                Required = node.Attributes["required"] is not null,
                SourceRef = BuildSourceRef(node)
            };
        }

        return new UaiNode
        {
            Type = "input",
            Id = context.NextId("input"),
            InputType = node.GetAttributeValue("type", "text"),
            Name = node.GetAttributeValue("name", string.Empty),
            Label = GetAssociatedLabel(node),
            Value = node.GetAttributeValue("value", null),
            Placeholder = node.GetAttributeValue("placeholder", null),
            Required = node.Attributes["required"] is not null,
            SourceRef = BuildSourceRef(node)
        };
    }

    private static bool TryTranslateDefinitionList(HtmlNode node, TranslationContext context, out UaiNode translatedNode)
    {
        translatedNode = null!;

        HtmlNode? definitionListNode = node.Name.Equals("dl", StringComparison.OrdinalIgnoreCase)
            ? node
            : node.Descendants("dl").FirstOrDefault();

        if (definitionListNode is null)
        {
            return false;
        }

        var dtNodes = definitionListNode.Elements("dt").ToList();
        var ddNodes = definitionListNode.Elements("dd").ToList();
        if (dtNodes.Count == 0 || ddNodes.Count == 0)
        {
            return false;
        }

        if (HasClass(definitionListNode, "glossary") || HasClass(node, "glossary"))
        {
            translatedNode = new UaiNode
            {
                Type = "section",
                Id = context.NextId("section"),
                SectionKind = "glossary",
                Children = dtNodes.Zip(ddNodes, (term, definition) => new UaiNode
                {
                    Type = "glossaryEntry",
                    Id = context.NextId("glossaryEntry"),
                    Term = BuildText(term, context),
                    Definition = BuildText(definition, context),
                    SourceRef = BuildSourceRef(term.ParentNode)
                }).ToList(),
                SourceRef = BuildSourceRef(node)
            };
            return true;
        }

        translatedNode = new UaiNode
        {
            Type = "metadataBlock",
            Id = context.NextId("metadataBlock"),
            Entries = dtNodes.Zip(ddNodes, (key, value) => new UaiMetadataEntry
            {
                Key = ExtractLiteralText(key),
                Value = ExtractLiteralText(value),
                Source = "source"
            }).ToList(),
            SourceRef = BuildSourceRef(node)
        };
        return true;
    }

    private static UaiNode TranslateSvg(HtmlNode node, TranslationContext context)
    {
        if (TryCreateSymbolNode(node, context, out var symbolNode))
        {
            return symbolNode;
        }

        if (LooksLikeDiagram(node))
        {
            var assetId = context.GetOrCreateAsset(
                $"{context.Document.DocumentId}:{context.NextId("svgAsset")}",
                "image",
                "image/svg+xml",
                node.GetAttributeValue("aria-label", null),
                null,
                null,
                node.GetAttributeValue("aria-hidden", "false").Equals("true", StringComparison.OrdinalIgnoreCase));

            return new UaiNode
            {
                Type = "diagram",
                Id = context.NextId("diagram"),
                AssetRef = assetId,
                Description = node.GetAttributeValue("aria-label", null) ?? node.GetAttributeValue("title", null) ?? "diagram",
                SourceRef = BuildSourceRef(node)
            };
        }

        return TranslateUnknown(node, context, "unsupported-svg");
    }

    private static UaiNode TranslateUnknown(HtmlNode node, TranslationContext context, string reason)
    {
        return new UaiNode
        {
            Type = "unknown",
            Id = context.NextId("unknown"),
            Reason = reason,
            RawContent = NormalizeText(node.OuterHtml),
            SourceFragment = node.OuterHtml,
            SourceRef = BuildSourceRef(node)
        };
    }

    private static bool TryCreateSymbolNode(HtmlNode node, TranslationContext context, out UaiNode symbolNode)
    {
        symbolNode = null!;
        var symbolId = node.GetAttributeValue("data-uai-symbol-id", null);
        var sourceSystem = node.GetAttributeValue("data-uai-source-system", null);
        var label = node.GetAttributeValue("data-uai-symbol-label", null) ??
            node.GetAttributeValue("aria-label", null) ??
            node.GetAttributeValue("alt", null) ??
            node.GetAttributeValue("title", null);

        var looksLikeSymbol = !string.IsNullOrWhiteSpace(symbolId) ||
            HasClassLike(node, "symbol") ||
            HasClassLike(node, "glyph") ||
            HasClassLike(node, "sigil") ||
            HasClassLike(node, "seal") ||
            HasClassLike(node, "icon");

        if (!looksLikeSymbol)
        {
            return false;
        }

        symbolId ??= $"symbol.{BuildSlug(label ?? node.Name, label ?? node.Name)}";
        context.EnsureSymbol(
            symbolId,
            label,
            label ?? "unlabeled symbol",
            sourceSystem,
            node.GetAttributeValue("data-uai-symbol-meaning", null),
            node.GetAttributeValue("data-uai-symbol-meaning-type", null));

        symbolNode = new UaiNode
        {
            Type = "symbol",
            Id = context.NextId("symbol"),
            SymbolRef = symbolId,
            Label = label,
            Usage = node.GetAttributeValue("data-uai-usage", HasClassLike(node, "decorative") ? "decorative" : "semantic"),
            SourceRef = BuildSourceRef(node)
        };

        return true;
    }

    private static UaiTextValue BuildText(HtmlNode node, TranslationContext context)
    {
        var literal = ExtractLiteralText(node);
        return new UaiTextValue
        {
            Literal = literal,
            Normalized = UaiDocumentNormalizer.NormalizeLiteralText(literal),
            Language = context.Document.Metadata.Language
        };
    }

    private static UaiTextValue BuildInteractiveText(HtmlNode node, TranslationContext context)
    {
        var text = BuildText(node, context);
        if (!string.IsNullOrWhiteSpace(text.Literal))
        {
            return text;
        }

        var fallback = node.GetAttributeValue("aria-label", null) ??
            node.GetAttributeValue("title", null) ??
            node.Descendants("img")
                .Select(image => image.GetAttributeValue("alt", null))
                .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        fallback = NormalizeText(fallback ?? string.Empty);
        return new UaiTextValue
        {
            Literal = fallback,
            Normalized = UaiDocumentNormalizer.NormalizeLiteralText(fallback),
            Language = context.Document.Metadata.Language
        };
    }

    private static string ExtractLiteralText(HtmlNode node)
    {
        var builder = new StringBuilder();
        AppendLiteralText(node, builder);
        return NormalizeText(builder.ToString());
    }

    private static void AppendLiteralText(HtmlNode node, StringBuilder builder)
    {
        if (node.NodeType == HtmlNodeType.Text)
        {
            builder.Append(WebUtility.HtmlDecode(node.InnerText));
            return;
        }

        if (node.Name.Equals("br", StringComparison.OrdinalIgnoreCase))
        {
            builder.AppendLine();
            return;
        }

        foreach (var child in node.ChildNodes)
        {
            AppendLiteralText(child, builder);
        }
    }

    private static string NormalizeText(string value)
    {
        var normalized = value.Replace("\r\n", "\n").Replace('\r', '\n');
        normalized = Regex.Replace(normalized, @"[ \t\f\v]+", " ");
        normalized = Regex.Replace(normalized, @"\n\s+", "\n");
        return normalized.Trim();
    }

    private static UaiSourceReference BuildSourceRef(HtmlNode node)
    {
        return new UaiSourceReference
        {
            Selector = node.GetAttributeValue("id", null) is { Length: > 0 } id ? $"#{id}" : null,
            DomPath = GetDomPath(node),
            HtmlFragment = node.OuterHtml
        };
    }

    private static string GetDomPath(HtmlNode node)
    {
        var segments = new Stack<string>();
        var current = node;
        while (current is not null && current.NodeType == HtmlNodeType.Element)
        {
            var index = current.ParentNode?
                .ChildNodes
                .Where(sibling => sibling.Name.Equals(current.Name, StringComparison.OrdinalIgnoreCase))
                .TakeWhile(sibling => sibling != current)
                .Count() ?? 0;
            segments.Push($"{current.Name}[{index}]");
            current = current.ParentNode;
        }

        return "/" + string.Join('/', segments);
    }

    private static bool IsMeaningfulNode(HtmlNode node)
    {
        if (node.NodeType == HtmlNodeType.Comment)
        {
            return false;
        }

        if (node.NodeType == HtmlNodeType.Text)
        {
            return !string.IsNullOrWhiteSpace(node.InnerText);
        }

        if (node.Name is "script" or "style" or "noscript")
        {
            return false;
        }

        return true;
    }

    private static string? GetTitle(HtmlNode? headNode)
    {
        return headNode?.SelectSingleNode("./title")?.InnerText?.Trim();
    }

    private static string? GetMetaContent(HtmlNode? headNode, string name)
    {
        return headNode?
            .SelectSingleNode($"./meta[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{name.ToLowerInvariant()}']")
            ?.GetAttributeValue("content", null);
    }

    private static string? GetMetaProperty(HtmlNode? headNode, string propertyName)
    {
        return headNode?
            .SelectSingleNode($"./meta[translate(@property,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{propertyName.ToLowerInvariant()}']")
            ?.GetAttributeValue("content", null);
    }

    private static string? GetCanonicalSourceUri(HtmlNode? headNode)
    {
        return headNode?
            .SelectSingleNode("./link[translate(@rel,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='canonical']")
            ?.GetAttributeValue("href", null)
            ?? GetMetaProperty(headNode, "og:url");
    }

    private static string? GetFirstHeading(HtmlNode bodyNode)
    {
        return bodyNode.SelectSingleNode(".//h1|.//h2")?.InnerText?.Trim();
    }

    private static string BuildSlug(string source, string fallback)
    {
        var raw = source;
        if (Uri.TryCreate(source, UriKind.Absolute, out var uri))
        {
            raw = string.IsNullOrWhiteSpace(uri.AbsolutePath) || uri.AbsolutePath == "/"
                ? uri.Host
                : uri.AbsolutePath.Trim('/');
        }

        raw = string.IsNullOrWhiteSpace(raw) ? fallback : raw;
        raw = WebUtility.UrlDecode(raw).ToLowerInvariant();
        raw = Regex.Replace(raw, @"[^a-z0-9]+", "-");
        raw = raw.Trim('-');
        return string.IsNullOrWhiteSpace(raw) ? "document" : raw;
    }

    private static string? GetAriaLabel(HtmlNode node)
    {
        return node.GetAttributeValue("aria-label", null) ??
            node.GetAttributeValue("data-uai-label", null);
    }

    private static string DetectSectionKind(HtmlNode node)
    {
        if (HasClass(node, "hero"))
        {
            return "hero";
        }

        if (HasClass(node, "content-grid") || HasClass(node, "tool-grid"))
        {
            return "grid";
        }

        if (HasClass(node, "detail-grid"))
        {
            return "detail-grid";
        }

        return "generic";
    }

    private static bool IsNavigationLike(HtmlNode node)
    {
        if (node.Name.Equals("nav", StringComparison.OrdinalIgnoreCase) ||
            node.GetAttributeValue("role", null)?.Equals("navigation", StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (!HasClass(node, "site-nav") && !HasClass(node, "menu") && !HasClass(node, "link-list"))
        {
            return false;
        }

        return node.Descendants("a").Any();
    }

    private static bool ShouldFlattenContainer(HtmlNode node)
    {
        return HasClass(node, "site-shell") || HasClass(node, "doc-prose");
    }

    private static bool IsDecorativeOnly(HtmlNode node)
    {
        if (node.GetAttributeValue("aria-hidden", "false").Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return HasClass(node, "boot-overlay");
    }

    private static bool IsCalloutLike(HtmlNode node)
    {
        return HasClass(node, "callout") || HasClass(node, "alert") || HasClass(node, "notice");
    }

    private static bool LooksLikeButton(HtmlNode node)
    {
        return node.Name.Equals("button", StringComparison.OrdinalIgnoreCase) ||
            node.GetAttributeValue("role", null)?.Equals("button", StringComparison.OrdinalIgnoreCase) == true ||
            HasClass(node, "btn") ||
            HasClass(node, "button") ||
            HasClass(node, "cta");
    }

    private static bool LooksLikeDiagram(HtmlNode node)
    {
        return HasClass(node, "diagram") ||
            HasClass(node, "chart") ||
            HasClass(node, "graph") ||
            HasClass(node, "map") ||
            HasClass(node, "schema");
    }

    private static string DetermineLinkPurpose(HtmlNode node)
    {
        if (node.GetAttributeValue("href", string.Empty).StartsWith("#", StringComparison.Ordinal))
        {
            return "fragment";
        }

        if (HasClass(node, "external") || node.GetAttributeValue("target", null) == "_blank")
        {
            return "external";
        }

        return "navigation";
    }

    private static string? GetAssociatedLabel(HtmlNode node)
    {
        var id = node.GetAttributeValue("id", null);
        if (!string.IsNullOrWhiteSpace(id))
        {
            var label = node.OwnerDocument.DocumentNode.SelectSingleNode($"//label[@for='{id}']");
            if (label is not null)
            {
                return ExtractLiteralText(label);
            }
        }

        var wrappingLabel = node.Ancestors("label").FirstOrDefault();
        return wrappingLabel is null ? null : ExtractLiteralText(wrappingLabel);
    }

    private static int? TryParseInt(string? value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static string InferImageMimeType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }

    private static bool HasClass(HtmlNode node, string className)
    {
        var classValue = node.GetAttributeValue("class", string.Empty);
        return Guard.SplitAndTrim(classValue, ' ')
            .Contains(className, StringComparer.OrdinalIgnoreCase);
    }

    private static bool HasClassLike(HtmlNode node, string token)
    {
        var classValue = node.GetAttributeValue("class", string.Empty);
        return classValue.Contains(token, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class TranslationContext
    {
        private readonly Dictionary<string, int> _counters = new(StringComparer.Ordinal);
        private readonly Dictionary<string, string> _assetIdsByUri = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _symbolIdsByKey = new(StringComparer.OrdinalIgnoreCase);

        public TranslationContext(UaiDocument document, string idPrefix, UaiHtmlTranslationOptions options)
        {
            Document = document;
            IdPrefix = idPrefix;
            Options = options;
        }

        public UaiDocument Document { get; }

        public string IdPrefix { get; }

        public UaiHtmlTranslationOptions Options { get; }

        public string NextId(string type)
        {
            _counters.TryGetValue(type, out var counter);
            counter++;
            _counters[type] = counter;
            return $"{IdPrefix}.{type}.{counter}";
        }

        public string GetOrCreateAsset(string uri, string kind, string mimeType, string? altText, int? width, int? height, bool decorative)
        {
            uri = string.IsNullOrWhiteSpace(uri) ? $"inline:{NextId("asset")}" : uri;
            if (_assetIdsByUri.TryGetValue(uri, out var existingId))
            {
                return existingId;
            }

            var assetId = NextId("asset");
            _assetIdsByUri[uri] = assetId;
            Document.Assets.Add(new UaiAsset
            {
                Id = assetId,
                Kind = kind,
                Uri = uri,
                MimeType = mimeType,
                AltText = altText,
                Width = width,
                Height = height,
                Decorative = decorative
            });
            return assetId;
        }

        public void EnsureSymbol(string symbolId, string? label, string visualForm, string? sourceSystem, string? meaning, string? meaningType)
        {
            if (_symbolIdsByKey.ContainsKey(symbolId))
            {
                return;
            }

            _symbolIdsByKey[symbolId] = symbolId;
            var symbol = new UaiSymbolDefinition
            {
                Id = symbolId,
                Label = label,
                VisualForm = visualForm,
                SourceSystem = sourceSystem,
                Inference = new UaiInference
                {
                    IsInferred = false
                }
            };

            if (!string.IsNullOrWhiteSpace(meaning))
            {
                symbol.Meaning.Add(new UaiSymbolMeaning
                {
                    Value = meaning,
                    MeaningType = meaningType ?? "source-authored",
                    Origin = "source-provided",
                    Confidence = 1m
                });
            }

            Document.Symbols.Add(symbol);
        }
    }
}

public sealed class UaiHtmlTranslationOptions
{
    public string? SourceUri { get; set; }

    public string? DocumentId { get; set; }

    public DateTimeOffset? RetrievedAt { get; set; }

    public string? ContentHash { get; set; }

    public string? Language { get; set; }

    public string DefaultLanguage { get; set; } = "en";

    public string SiteName { get; set; } = "Protocol5";

    public string PageType { get; set; } = "generic";

    public bool PreserveUnsupportedAsUnknown { get; set; } = true;

    public string GeneratorName { get; set; } = "Protocol5.UAI.CSharp";

    public string GeneratorVersion { get; set; } = UaiConstants.CurrentDocumentVersion;

    public string TranslatorName { get; set; } = "Protocol5.UAI.CSharp";

    public string TranslatorVersion { get; set; } = UaiConstants.CurrentDocumentVersion;

    public string? CaptureNotes { get; set; }
}
