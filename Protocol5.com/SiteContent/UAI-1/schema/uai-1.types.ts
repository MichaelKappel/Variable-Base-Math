export type UaiPageType =
  | "generic"
  | "homepage"
  | "article"
  | "landing-page"
  | "navigation"
  | "symbolic-manuscript"
  | "wordpress-page"
  | "gallery"
  | "glossary"
  | "reference";

export type UaiNodeType =
  | "document"
  | "section"
  | "heading"
  | "paragraph"
  | "quote"
  | "list"
  | "listItem"
  | "table"
  | "image"
  | "figure"
  | "caption"
  | "button"
  | "link"
  | "navigation"
  | "form"
  | "input"
  | "glossaryEntry"
  | "symbol"
  | "glyphCluster"
  | "diagram"
  | "manuscriptPanel"
  | "callout"
  | "metadataBlock"
  | "footer"
  | "header"
  | "unknown";

export interface UaiTextValue {
  literal: string;
  normalized?: string;
  language?: string;
}

export interface UaiInference {
  isInferred: boolean;
  rationale?: string;
  confidence?: number;
}

export interface UaiExtensionValue {
  version: string;
  required: boolean;
  payload: unknown;
}

export interface UaiSourceDescriptor {
  uri: string;
  title?: string;
  retrievedAt: string;
  contentHash?: string;
  mimeType?: string;
  htmlLanguage?: string;
  canonicalUri?: string;
}

export interface UaiMetadata {
  title: string;
  description?: string;
  language: string;
  siteName?: string;
  pageType: UaiPageType;
  keywords?: string[];
  authors?: string[];
  publishedAt?: string;
  updatedAt?: string;
  canonicalUrl?: string;
  alternateUrls?: string[];
}

export interface UaiAction {
  kind: "link" | "submit" | "button" | "script" | "unknown";
  target?: string;
}

export interface UaiInputOption {
  label: string;
  value: string;
}

export interface UaiMetadataEntry {
  key: string;
  value: string;
  source?: string;
}

export interface UaiTableColumn {
  id: string;
  label: string;
}

export interface UaiTableCell {
  text: UaiTextValue;
  headerRef?: string;
}

export interface UaiTableRow {
  id?: string;
  cells: UaiTableCell[];
}

export interface UaiSourceReference {
  selector?: string;
  xpath?: string;
  domPath?: string;
  htmlFragment?: string;
}

export interface UaiSemanticRecord {
  id: string;
  targets: string[];
  kind: string;
  value: string;
  source: "source" | "translator-structural" | "translator-semantic" | "user" | "system";
  inference?: UaiInference;
}

export interface UaiRelationship {
  id?: string;
  relation: string;
  source: string;
  target: string[];
}

export interface UaiAnnotation {
  id: string;
  targetId?: string;
  severity: "info" | "warning" | "error";
  code: string;
  message: string;
  source?: string;
}

export interface UaiSymbolGeometry {
  primitives?: string[];
  symmetry?: string;
  closure?: string;
}

export interface UaiSymbolMeaning {
  value: string;
  meaningType: string;
  origin: "source-provided" | "inferred" | "editorial";
  confidence?: number;
}

export interface UaiSourceEvidence {
  kind: string;
  value: string;
  uri?: string;
}

export interface UaiSymbolDefinition {
  id: string;
  label?: string;
  visualForm: string;
  geometry?: UaiSymbolGeometry;
  strokeLogic?: string[];
  meaning: UaiSymbolMeaning[];
  sourceSystem?: string;
  sourceEvidence?: UaiSourceEvidence[];
  inference: UaiInference;
  variants?: string[];
  relationships?: UaiRelationship[];
  notes?: string;
}

export interface UaiAsset {
  id: string;
  kind: "image" | "video" | "audio" | "document" | "embed" | "icon" | "file";
  uri: string;
  mimeType: string;
  contentHash?: string;
  title?: string;
  altText?: string;
  width?: number;
  height?: number;
  decorative?: boolean;
}

export interface UaiAgentIdentity {
  name: string;
  version: string;
}

export interface UaiTranslatorIdentity extends UaiAgentIdentity {
  contractVersion: string;
  mode: string;
}

export interface UaiCaptureInfo {
  method: string;
  notes?: string;
}

export interface UaiRevisionEntry {
  timestamp: string;
  actor: string;
  change: string;
}

export interface UaiProvenance {
  generatedAt: string;
  generator?: UaiAgentIdentity;
  translator?: UaiTranslatorIdentity;
  sourceAuthors?: string[];
  capture?: UaiCaptureInfo;
  history?: UaiRevisionEntry[];
}

export interface UaiNode {
  type: UaiNodeType;
  id: string;
  role?: string;
  label?: string;
  level?: number;
  text?: UaiTextValue;
  children?: UaiNode[];
  sectionKind?: string;
  ordered?: boolean;
  listStyle?: string;
  columns?: UaiTableColumn[];
  rows?: UaiTableRow[];
  assetRef?: string;
  altText?: string;
  decorative?: boolean;
  href?: string;
  rel?: string;
  target?: string;
  linkPurpose?: string;
  action?: UaiAction;
  method?: string;
  name?: string;
  inputType?: string;
  value?: string;
  placeholder?: string;
  required?: boolean;
  options?: UaiInputOption[];
  term?: UaiTextValue;
  definition?: UaiTextValue;
  aliases?: string[];
  symbolRef?: string;
  usage?: "semantic" | "decorative" | "mixed" | "unknown";
  description?: string;
  diagramType?: string;
  containsSymbolRefs?: string[];
  panelNumber?: number;
  folio?: string;
  calloutType?: "info" | "warning" | "success" | "danger" | "note";
  entries?: UaiMetadataEntry[];
  rawContent?: string;
  reason?: string;
  sourceFragment?: string;
  semanticRefs?: string[];
  relationshipRefs?: string[];
  annotationRefs?: string[];
  sourceRef?: UaiSourceReference;
  inference?: UaiInference;
  extensions?: Record<string, UaiExtensionValue>;
}

export interface UaiDocument {
  spec: "UAI-1";
  version: string;
  schemaVersion: string;
  documentId: string;
  source: UaiSourceDescriptor;
  metadata: UaiMetadata;
  structure: [UaiNode];
  semantics: UaiSemanticRecord[];
  symbols: UaiSymbolDefinition[];
  assets: UaiAsset[];
  relationships: UaiRelationship[];
  annotations: UaiAnnotation[];
  provenance: UaiProvenance;
  extensions: Record<string, UaiExtensionValue>;
}
