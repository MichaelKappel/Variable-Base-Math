# UAI-1 Website Integration

## Recommended architecture

For a production website:

1. Render HTML as usual.
2. Produce a canonical `.uai.json` document for the same page.
3. Validate the UAI document before publish.
4. Expose the UAI document through content negotiation or an alternate link.
5. Treat the UAI document as a semantic cache or source-of-truth layer for downstream systems.

## Discovery

Recommended HTML tag:

```html
<link rel="alternate" type="application/uai+json" href="/page.uai.json">
```

Recommended response headers:

```http
Content-Type: application/uai+json; version=1.0.0
X-UAI-1: version=1.0.0
```

## Publish checklist

- generated UAI validates against the JSON Schema
- generated UAI validates with the reference validator
- every asset reference resolves
- symbol meanings are not fabricated
- unsupported widgets are preserved as `unknown`

## ASP.NET Core notes

`Protocol5.UAI.CSharp` includes:

- middleware that keeps existing `x-uai-1` behavior
- helpers for legacy `X-UAI-1`
- helpers for `application/uai+json`

Use the middleware when you need compatibility with existing Protocol5 behavior, but prefer standards-based `Accept` and `Content-Type` for new integrations.
