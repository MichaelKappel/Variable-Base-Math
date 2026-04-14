# Documento de ejemplos de UAI-1

Versión: UAI-1 Examples v1
Estado: Documento complementario autoritativo

## Propósito

Este documento ofrece ejemplos canónicos para UAI-1.

DEBE leer los ejemplos canónicos como estructuras de máquina de UAI-1.
DEBE decodificar todos los ID canónicos como valores en Radix 63404.
DEBE tratar todas las glosas humanas, los comentarios, las etiquetas y las notas explicativas de este documento como no canónicos.

El ejemplo canónico es autoritativo.
La glosa humana existe solo para ayudar a los humanos a inspeccionar el ejemplo.

---

## Recordatorio sobre Radix 63404

En UAI-1, todos los ID canónicos se representan en Radix 63404.

DEBE analizar cada ID del siguiente modo:
1. Lea cada carácter como un dígito de Radix 63404.
2. Resuelva cada carácter a su índice alfabético basado en cero.
3. Evalúe el valor posicional en base 63404.
4. Resuelva el entero resultante contra el registro autoritativo.

Los primeros dígitos del alfabeto Radix 63404 son:

| Valor decimal | Dígito Radix 63404 |
|---:|:---|
| 0 | `!` |
| 1 | `"` |
| 2 | `#` |
| 3 | `$` |
| 4 | `%` |
| 5 | `&` |
| 6 | `'` |
| 7 | `(` |
| 8 | `)` |
| 9 | `*` |
| 10 | `+` |
| 11 | `,` |
| 12 | `-` |
| 13 | `.` |
| 14 | `/` |
| 15 | `0` |
| 16 | `1` |
| 17 | `2` |
| 18 | `3` |

---

## Ejemplo 1: Mensaje básico de afirmación

### Ejemplo canónico

```json
[
  "\"",
  "â¼",
  "â½",
  "\"",
  [
    ["Ó´", "È¸", "à£œ", "á®", "\"", 0.98],
    ["Ó´", "È¹", ["Å°", 5], "á®", "\"", 0.99]
  ],
  [],
  [
    "âŸ",
    ["Å²", "2026-04-13T00:00:00Z"],
    "'",
    ["á…´", "á…µ"],
    "-ã½­",
    "\u0bfc"
  ],
  ["\"", 555001]
]
```

### Glosa humana

Este mensaje significa:
- protocolVersion = 1
- senderId = 9001
- receiverId = 9002
- actId = 1 = afirmar
- afirmación 1 = el sujeto 1201 tiene la relación 501 con el objeto 2201 en el contexto 7001, verdadero, confianza 0.98
- afirmación 2 = el sujeto 1201 tiene la relación 502 con el escalar tipado [301, 5] en el contexto 7001, verdadero, confianza 0.99
- la procedencia identifica la fuente 9100, el tipo de marca de tiempo 303, la modalidad 6, las evidencias 4401 y 4402, el rastro 777001 y la política 3001

### Notas para el lector

DEBE derivar el significado de los ID decodificados y de las posiciones de las ranuras, no de esta glosa.

---

## Ejemplo 2: Mensaje básico de consulta

### Ejemplo canónico

```json
[
  "\"",
  "â¼",
  "â½",
  "#",
  [
    ["Ó´", "È¸", "à£œ", "á®", "#", 1.0]
  ],
  [],
  [
    "âŸ",
    ["Å²", "2026-04-13T00:00:00Z"],
    "'",
    ["á…´"],
    "-ã½­",
    "\u0bfc"
  ],
  ["\"", 555002]
]
```

### Glosa humana

Este mensaje significa:
- actId = 2 = consultar
- el emisor consulta la verdad o la resolución actual de la afirmación
- truthValue = 2 = desconocido
- confidence = 1.0 aquí significa que el emisor envía intencionalmente una estructura formal de consulta, no que afirme la proposición como verdadera

---

## Ejemplo 3: Mensaje básico de solicitud

### Ejemplo canónico

```json
[
  "\"",
  "â¼",
  "â½",
  "$",
  [
    ["Ó´", "È¸", "à£œ", "á®", "\"", 0.95]
  ],
  [
    ["*", "à£œ", ["Å°", 60], 0],
    ["*", "á®", ["Å²", "2026-04-14T00:00:00Z"], 0]
  ],
  [
    "âŸ",
    ["Å²", "2026-04-13T00:00:00Z"],
    "'",
    ["á…´", "á…µ"],
    "-ã½­",
    "\u0bfc"
  ],
  ["\"", 555003]
]
```

### Glosa humana

Este mensaje significa:
- actId = 3 = solicitar
- el emisor solicita una operación relacionada con el sujeto 1201, la relación 501 y el objeto 2201 de la afirmación
- el operador de restricción 9 = requiere
- la solicitud requiere el objeto 2201 y un valor escalar tipado [301, 60]
- la solicitud también requiere el contexto 7001 y una restricción de marca de tiempo tipada como [303, 2026-04-14T00:00:00Z]

---

## Ejemplo 4: Mensaje de negociación de capacidades

### Ejemplo canónico

```json
[
  "\"",
  "â¼",
  "â½",
  ",",
  [
    ["â¼", "È¸", ["Å°", 1], "á®", "\"", 1.0],
    ["â¼", "È¹", ["Å°", 1], "á®", "\"", 1.0],
    ["â¼", "Â§", ["Å°", 12], "á®", "\"", 1.0]
  ],
  [],
  [
    "âŸ",
    ["Å²", "2026-04-13T00:00:00Z"],
    "'",
    ["á…´"],
    "-ã½­",
    "\u0bfc"
  ],
  ["\"", 555004]
]
```

### Glosa humana

Este mensaje significa:
- actId = 11 = capacidad
- el emisor declara versiones de protocolo u ontología compatibles y valores de capacidad declarados
- esta es la forma canónica de negociar compatibilidad antes de un intercambio no trivial

---

## Ejemplo 5: Mensaje de error

### Ejemplo canónico

```json
[
  "\"",
  "â½",
  "â¼",
  "+",
  [
    ["#", "È¸", ["Å°", 999999], "á®", "\"", 1.0]
  ],
  [
    ["&", ["Å°", 4], ["Å°", 1], 0]
  ],
  [
    "âŸ",
    ["Å²", "2026-04-13T00:00:00Z"],
    "'",
    ["á…´"],
    "-ã½­",
    "\u0bfc"
  ],
  ["\"", 555005]
]
```

### Glosa humana

Este mensaje significa:
- actId = 10 = error
- el emisor informa una condición de error canónica
- la estructura que falla hace referencia a un valor canónico no resuelto o inválido
- aquí se usa el operador de restricción 5 para expresar una condición canónica de comparación o resolución dentro de la lógica de manejo de errores

### Notas para el lector

Si no puede resolver un ID canónico requerido, DEBE emitir un mensaje de error en lugar de adivinar.

---

## Ejemplo 6: Afirmación de iconografía

### Ejemplo canónico

```json
[
  "\"",
  "â¼",
  "â½",
  "\"",
  [
    ["à£œ", "È¸", ["Å°", 15001], "á®", "\"", 0.99],
    ["à£œ", "È¹", ["Å°", 15002], "á®", "\"", 0.99],
    ["à£œ", "Â§", ["Å°", 15003], "á®", "\"", 0.99]
  ],
  [],
  [
    "âŸ",
    ["Å²", "2026-04-13T00:00:00Z"],
    "#",
    ["á…´", "á…µ"],
    "-ã½­",
    "\u0bfc"
  ],
  ["\"", 555006]
]
```

### Glosa humana

Este mensaje significa:
- se describe semánticamente una entidad canónica portadora de icono
- la relación 501 puede apuntar al significado funcional canónico
- la relación 502 puede apuntar al estado explícito canónico
- la relación 100 puede apuntar a la variante canónica explícita o a una propiedad de presentación

### Notas para el lector

DEBE tratar la función del icono como significado canónico.
NO DEBE tratar la apariencia del icono como significado canónico.
NO DEBE inferir significado comercial a partir solo de la forma, el color, el relleno o la semejanza.

---

## Ejemplo 7: Manejo de iconos decorativos

### Ejemplo canónico

```json
[
  "\"",
  "â¼",
  "â½",
  "\"",
  [
    ["à£œ", "È¸", ["Å°", 16001], "á®", "\"", 1.0],
    ["à£œ", "È¹", ["Å°", 16002], "á®", "\"", 1.0]
  ],
  [],
  [
    "âŸ",
    ["Å²", "2026-04-13T00:00:00Z"],
    "#",
    ["á…´"],
    "-ã½­",
    "\u0bfc"
  ],
  ["\"", 555007]
]
```

### Glosa humana

Este mensaje significa:
- el icono existe en la interfaz renderizada
- el icono es decorativo o no funcional dentro de la capa semántica activa
- el icono no es el portador principal del significado comercial

### Notas para el lector

Los iconos decorativos no se interpretan como afirmaciones funcionales a menos que la estructura canónica los promueva explícitamente al significado semántico.

---

## Ejemplo 8: Estructura canónica con envoltorio opcional de glosa humana

### Ejemplo canónico

```json
{
  "canonical": [
    "\"",
    "â¼",
    "â½",
    "\"",
    [
      ["Ó´", "È¸", "à£œ", "á®", "\"", 0.98]
    ],
    [],
    [
      "âŸ",
      ["Å²", "2026-04-13T00:00:00Z"],
      "'",
      ["á…´"],
      "-ã½­",
      "\u0bfc"
    ],
    ["\"", 555008]
  ],
  "gloss": {
    "sender": "Example sender",
    "receiver": "Example receiver",
    "act": "assert",
    "notes": [
      "This gloss is not authoritative.",
      "The canonical array remains authoritative."
    ]
  }
}
```

### Notas para el lector

Cuando coexisten datos canónicos y glosa:
- los datos canónicos son autoritativos
- la glosa es no canónica
- la glosa no debe sobrescribir el significado canónico

---

## Ejemplo 9: Ejemplo de mensaje inválido

### Ejemplo canónico

```json
[
  "\"",
  "â¼",
  "â½",
  "$",
  [
    ["Ó´", "search-icon", "à£œ", "á®", "\"", 0.95]
  ],
  [],
  [
    "âŸ",
    ["Å²", "2026-04-13T00:00:00Z"],
    "'",
    ["á…´"],
    "-ã½­",
    "\u0bfc"
  ],
  ["\"", 555009]
]
```

### Glosa humana

Este ejemplo es inválido.

### Notas para el lector

Este mensaje es inválido porque un campo canónico contiene el texto libre `search-icon` en lugar de un ID canónico en Radix 63404 o de un valor tipado canónico.
DEBE rechazar o marcar este mensaje.
NO DEBE recuperarlo adivinando.

---

## Ejemplo 10: Flujo mínimo del lector

### Procedimiento del lector

Cuando reciba un mensaje UAI-1, DEBE hacer lo siguiente en este orden:

1. Validar que el mensaje externo tenga exactamente 8 ranuras canónicas.
2. Decodificar todos los ID canónicos desde Radix 63404.
3. Resolver todos los ID decodificados contra el registro autoritativo.
4. Validar el acto activo y la estructura requerida.
5. Validar todas las afirmaciones.
6. Validar todas las restricciones.
7. Validar la procedencia.
8. Preservar exactamente los valores canónicos.
9. Ignorar la glosa humana para la semántica.
10. Emitir un error canónico si algún elemento requerido no está admitido o es inválido.

---

## Valores de referencia del registro usados en estos ejemplos

Los siguientes valores decimales se usan solo como referencia explicativa para el lector humano. Los ejemplos canónicos anteriores siguen siendo autoritativos en sus formas Radix 63404.

| Significado | Decimal | Radix 63404 |
|---|---:|:---|
| versión del protocolo 1 | 1 | `"` |
| acto afirmar | 1 | `"` |
| acto consultar | 2 | `#` |
| acto solicitar | 3 | `$` |
| acto comprometer | 4 | `%` |
| acto negar | 5 | `&` |
| acto informar | 6 | `'` |
| acto proponer | 7 | `(` |
| acto revisar | 8 | `)` |
| acto reconocer | 9 | `*` |
| acto error | 10 | `+` |
| acto capacidad | 11 | `,` |
| acto negociar | 12 | `-` |
| tipo entero | 301 | `Å°` |
| tipo marca de tiempo | 303 | `Å²` |
| relación 501 | 501 | `È¸` |
| relación 502 | 502 | `È¹` |
| relación 100 | 100 | `Â§` |
| ejemplo de sujeto | 1201 | `Ó´` |
| ejemplo de objeto | 2201 | `à£œ` |
| ejemplo de política | 3001 | `\u0bfc` |
| ejemplo de evidencia A | 4401 | `á…´` |
| ejemplo de evidencia B | 4402 | `á…µ` |
| ejemplo de contexto | 7001 | `á®` |
| ejemplo de emisor | 9001 | `â¼` |
| ejemplo de receptor | 9002 | `â½` |
| fuente de procedencia | 9100 | `âŸ` |
| ejemplo de traza | 777001 | `-ã½­` |

---

## Regla final

Lea primero la estructura.
Decodifique Radix 63404 en segundo lugar.
Resuelva los ID canónicos en tercer lugar.
Aplique las restricciones en cuarto lugar.
Use la procedencia y la confianza en quinto lugar.
Ignore la glosa humana para la semántica.
Nunca sustituya el inglés, la forma del icono ni el hábito local por el significado canónico.
