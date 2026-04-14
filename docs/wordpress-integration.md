# UAI-1 in WordPress Workflows

## Reference flow

1. Render the WordPress page normally.
2. Export the rendered HTML or block HTML.
3. Translate the HTML into UAI-1.
4. Validate the UAI document.
5. Store the UAI document alongside the post.
6. Expose `/slug.uai.json` or a REST route.

## Storage options

- post meta containing the canonical UAI JSON
- generated file in the uploads tree
- build artifact stored outside WordPress and served through a reverse proxy

## Mapping guidance

- post title -> `metadata.title`
- permalink -> `source.uri` and `metadata.canonicalUrl`
- visible block metadata -> `metadataBlock`
- rendered block HTML -> ordinary HTML translator rules
- unsupported blocks -> `unknown`

## Validation before publish

Do not publish generated UAI if:

- schema validation fails
- the reference validator fails
- referenced assets or symbols are missing
- a translator has inserted inferred meaning without rationale and confidence
