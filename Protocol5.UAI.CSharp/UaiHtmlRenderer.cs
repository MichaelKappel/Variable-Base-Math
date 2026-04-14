using System.Net;
using System.Text;

namespace Protocol5.UAI;

public sealed class UaiHtmlRenderer
{
    public string Render(UaiDocument document)
    {
        Guard.NotNull(document, nameof(document));

        var assetLookup = document.Assets.ToDictionary(asset => asset.Id, StringComparer.Ordinal);
        var symbolLookup = document.Symbols.ToDictionary(symbol => symbol.Id, StringComparer.Ordinal);
        var builder = new StringBuilder();

        builder.AppendLine("<!DOCTYPE html>");
        builder.AppendLine($"<html lang=\"{Encode(document.Metadata.Language)}\">");
        builder.AppendLine("<head>");
        builder.AppendLine("  <meta charset=\"utf-8\" />");
        builder.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />");
        builder.AppendLine($"  <title>{Encode(document.Metadata.Title)}</title>");

        if (!string.IsNullOrWhiteSpace(document.Metadata.Description))
        {
            builder.AppendLine($"  <meta name=\"description\" content=\"{Encode(document.Metadata.Description)}\" />");
        }

        builder.AppendLine("</head>");
        builder.AppendLine("<body>");

        foreach (var node in document.Structure)
        {
            builder.AppendLine(RenderNode(node, assetLookup, symbolLookup, 1));
        }

        builder.AppendLine("</body>");
        builder.AppendLine("</html>");

        return builder.ToString();
    }

    private static string RenderNode(
        UaiNode node,
        IReadOnlyDictionary<string, UaiAsset> assetLookup,
        IReadOnlyDictionary<string, UaiSymbolDefinition> symbolLookup,
        int depth)
    {
        var indent = new string(' ', depth * 2);
        var text = node.Text?.Literal ?? node.Text?.Normalized ?? string.Empty;

        return node.Type switch
        {
            "document" => RenderContainer("main", node.Children, assetLookup, symbolLookup, depth),
            "header" => RenderContainer("header", node.Children, assetLookup, symbolLookup, depth),
            "footer" => RenderContainer("footer", node.Children, assetLookup, symbolLookup, depth),
            "section" => RenderContainer("section", node.Children, assetLookup, symbolLookup, depth, node.Label),
            "heading" => $"{indent}<h{node.Level ?? 2}>{Encode(text)}</h{node.Level ?? 2}>",
            "paragraph" => $"{indent}<p>{Encode(text)}</p>",
            "quote" => $"{indent}<blockquote><p>{Encode(text)}</p></blockquote>",
            "list" => RenderList(node, assetLookup, symbolLookup, depth),
            "listItem" => RenderListItem(node, assetLookup, symbolLookup, depth),
            "table" => RenderTable(node, depth),
            "image" => RenderImage(node, assetLookup, depth),
            "figure" => RenderContainer("figure", node.Children, assetLookup, symbolLookup, depth),
            "caption" => $"{indent}<figcaption>{Encode(text)}</figcaption>",
            "button" => RenderButton(node, depth),
            "link" => $"{indent}<a href=\"{Encode(node.Href ?? "#")}\">{Encode(text)}</a>",
            "navigation" => RenderContainer("nav", node.Children, assetLookup, symbolLookup, depth, node.Label),
            "form" => RenderForm(node, assetLookup, symbolLookup, depth),
            "input" => RenderInput(node, depth),
            "glossaryEntry" => $"{indent}<section class=\"uai-glossary-entry\"><h3>{Encode(node.Term?.Literal ?? string.Empty)}</h3><p>{Encode(node.Definition?.Literal ?? string.Empty)}</p></section>",
            "symbol" => RenderSymbol(node, symbolLookup, depth),
            "glyphCluster" => RenderContainer("div", node.Children, assetLookup, symbolLookup, depth, cssClass: "uai-glyph-cluster"),
            "diagram" => RenderDiagram(node, assetLookup, depth),
            "manuscriptPanel" => RenderContainer("section", node.Children, assetLookup, symbolLookup, depth, cssClass: "uai-manuscript-panel"),
            "callout" => RenderContainer("aside", node.Children, assetLookup, symbolLookup, depth, cssClass: $"uai-callout uai-callout--{Encode(node.CalloutType ?? "info")}"),
            "metadataBlock" => RenderMetadataBlock(node, depth),
            "unknown" => $"{indent}<template data-uai-node=\"unknown\" data-reason=\"{Encode(node.Reason ?? "unsupported")}\">{Encode(node.RawContent ?? string.Empty)}</template>",
            _ => $"{indent}<div data-uai-node=\"{Encode(node.Type)}\"></div>"
        };
    }

    private static string RenderContainer(
        string tagName,
        List<UaiNode>? children,
        IReadOnlyDictionary<string, UaiAsset> assetLookup,
        IReadOnlyDictionary<string, UaiSymbolDefinition> symbolLookup,
        int depth,
        string? label = null,
        string? cssClass = null)
    {
        var indent = new string(' ', depth * 2);
        var attributes = new List<string>();

        if (!string.IsNullOrWhiteSpace(cssClass))
        {
            attributes.Add($"class=\"{Encode(cssClass)}\"");
        }

        if (!string.IsNullOrWhiteSpace(label))
        {
            attributes.Add($"aria-label=\"{Encode(label)}\"");
        }

        var builder = new StringBuilder();
        builder.Append($"{indent}<{tagName}");
        if (attributes.Count > 0)
        {
            builder.Append(' ');
            builder.Append(string.Join(' ', attributes));
        }
        builder.AppendLine(">");

        if (children is not null)
        {
            foreach (var child in children)
            {
                builder.AppendLine(RenderNode(child, assetLookup, symbolLookup, depth + 1));
            }
        }

        builder.Append($"{indent}</{tagName}>");
        return builder.ToString();
    }

    private static string RenderList(
        UaiNode node,
        IReadOnlyDictionary<string, UaiAsset> assetLookup,
        IReadOnlyDictionary<string, UaiSymbolDefinition> symbolLookup,
        int depth)
    {
        var tag = node.Ordered == true ? "ol" : "ul";
        return RenderContainer(tag, node.Children, assetLookup, symbolLookup, depth);
    }

    private static string RenderListItem(
        UaiNode node,
        IReadOnlyDictionary<string, UaiAsset> assetLookup,
        IReadOnlyDictionary<string, UaiSymbolDefinition> symbolLookup,
        int depth)
    {
        var indent = new string(' ', depth * 2);
        var builder = new StringBuilder();
        builder.Append($"{indent}<li>");

        if (node.Text is not null)
        {
            builder.Append(Encode(node.Text.Literal));
        }

        if (node.Children is not null && node.Children.Count > 0)
        {
            builder.AppendLine();
            foreach (var child in node.Children)
            {
                builder.AppendLine(RenderNode(child, assetLookup, symbolLookup, depth + 1));
            }
            builder.Append(indent);
        }

        builder.Append("</li>");
        return builder.ToString();
    }

    private static string RenderTable(UaiNode node, int depth)
    {
        var indent = new string(' ', depth * 2);
        var builder = new StringBuilder();
        builder.AppendLine($"{indent}<table>");

        if (node.Columns is not null && node.Columns.Count > 0)
        {
            builder.AppendLine($"{indent}  <thead>");
            builder.AppendLine($"{indent}    <tr>");
            foreach (var column in node.Columns)
            {
                builder.AppendLine($"{indent}      <th>{Encode(column.Label)}</th>");
            }
            builder.AppendLine($"{indent}    </tr>");
            builder.AppendLine($"{indent}  </thead>");
        }

        builder.AppendLine($"{indent}  <tbody>");
        if (node.Rows is not null)
        {
            foreach (var row in node.Rows)
            {
                builder.AppendLine($"{indent}    <tr>");
                foreach (var cell in row.Cells)
                {
                    builder.AppendLine($"{indent}      <td>{Encode(cell.Text.Literal)}</td>");
                }
                builder.AppendLine($"{indent}    </tr>");
            }
        }
        builder.AppendLine($"{indent}  </tbody>");
        builder.Append($"{indent}</table>");
        return builder.ToString();
    }

    private static string RenderImage(UaiNode node, IReadOnlyDictionary<string, UaiAsset> assetLookup, int depth)
    {
        var indent = new string(' ', depth * 2);
        UaiAsset? asset = null;
        var src = node.AssetRef is not null && assetLookup.TryGetValue(node.AssetRef, out asset)
            ? asset.Uri
            : node.AssetRef ?? string.Empty;
        var alt = node.AltText ?? asset?.AltText ?? string.Empty;
        return $"{indent}<img src=\"{Encode(src)}\" alt=\"{Encode(alt)}\" />";
    }

    private static string RenderButton(UaiNode node, int depth)
    {
        var indent = new string(' ', depth * 2);
        var text = Encode(node.Text?.Literal ?? string.Empty);
        var actionTarget = node.Action?.Target;
        if (string.Equals(node.Action?.Kind, "link", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(actionTarget))
        {
            return $"{indent}<a class=\"uai-button\" href=\"{Encode(actionTarget)}\">{text}</a>";
        }

        var buttonType = string.Equals(node.Action?.Kind, "submit", StringComparison.OrdinalIgnoreCase)
            ? "submit"
            : "button";
        return $"{indent}<button type=\"{buttonType}\">{text}</button>";
    }

    private static string RenderForm(
        UaiNode node,
        IReadOnlyDictionary<string, UaiAsset> assetLookup,
        IReadOnlyDictionary<string, UaiSymbolDefinition> symbolLookup,
        int depth)
    {
        var indent = new string(' ', depth * 2);
        var method = Encode(node.Method ?? "post");
        var action = Encode(node.Action?.Target ?? string.Empty);
        var builder = new StringBuilder();
        builder.AppendLine($"{indent}<form method=\"{method}\" action=\"{action}\">");
        if (node.Children is not null)
        {
            foreach (var child in node.Children)
            {
                builder.AppendLine(RenderNode(child, assetLookup, symbolLookup, depth + 1));
            }
        }
        builder.Append($"{indent}</form>");
        return builder.ToString();
    }

    private static string RenderInput(UaiNode node, int depth)
    {
        var indent = new string(' ', depth * 2);
        if (string.Equals(node.InputType, "select", StringComparison.OrdinalIgnoreCase))
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{indent}<label>{Encode(node.Label ?? node.Name ?? string.Empty)}");
            builder.AppendLine($"{indent}  <select name=\"{Encode(node.Name ?? string.Empty)}\">");
            if (node.Options is not null)
            {
                foreach (var option in node.Options)
                {
                    builder.AppendLine($"{indent}    <option value=\"{Encode(option.Value)}\">{Encode(option.Label)}</option>");
                }
            }
            builder.AppendLine($"{indent}  </select>");
            builder.Append($"{indent}</label>");
            return builder.ToString();
        }

        return $"{indent}<input type=\"{Encode(node.InputType ?? "text")}\" name=\"{Encode(node.Name ?? string.Empty)}\" value=\"{Encode(node.Value ?? string.Empty)}\" placeholder=\"{Encode(node.Placeholder ?? string.Empty)}\" />";
    }

    private static string RenderSymbol(UaiNode node, IReadOnlyDictionary<string, UaiSymbolDefinition> symbolLookup, int depth)
    {
        var indent = new string(' ', depth * 2);
        var label = node.Label;
        if (string.IsNullOrWhiteSpace(label) &&
            !string.IsNullOrWhiteSpace(node.SymbolRef) &&
            symbolLookup.TryGetValue(node.SymbolRef, out var symbol))
        {
            label = symbol.Label ?? symbol.VisualForm;
        }

        return $"{indent}<span class=\"uai-symbol\" data-symbol-ref=\"{Encode(node.SymbolRef ?? string.Empty)}\">{Encode(label ?? node.SymbolRef ?? "symbol")}</span>";
    }

    private static string RenderDiagram(UaiNode node, IReadOnlyDictionary<string, UaiAsset> assetLookup, int depth)
    {
        var indent = new string(' ', depth * 2);
        var src = node.AssetRef is not null && assetLookup.TryGetValue(node.AssetRef, out var asset)
            ? asset.Uri
            : node.AssetRef ?? string.Empty;
        return $"{indent}<figure class=\"uai-diagram\"><img src=\"{Encode(src)}\" alt=\"{Encode(node.Description ?? string.Empty)}\" /><figcaption>{Encode(node.Description ?? string.Empty)}</figcaption></figure>";
    }

    private static string RenderMetadataBlock(UaiNode node, int depth)
    {
        var indent = new string(' ', depth * 2);
        var builder = new StringBuilder();
        builder.AppendLine($"{indent}<dl class=\"uai-metadata-block\">");
        if (node.Entries is not null)
        {
            foreach (var entry in node.Entries)
            {
                builder.AppendLine($"{indent}  <dt>{Encode(entry.Key)}</dt>");
                builder.AppendLine($"{indent}  <dd>{Encode(entry.Value)}</dd>");
            }
        }
        builder.Append($"{indent}</dl>");
        return builder.ToString();
    }

    private static string Encode(string value)
    {
        return WebUtility.HtmlEncode(value);
    }
}
