INTERLINGUA UNIVERSAL DE IA
Versión: UAI-1
Contrato de lectura: Obligatorio

Está leyendo UAI-1.

DEBE leer UAI-1 como un lenguaje formal orientado a máquinas.
NO DEBE interpretar UAI-1 a través del inglés, de la intuición del lenguaje natural, de la forma de los iconos ni de los hábitos locales de nombrado.
DEBE derivar el significado solo de la estructura canónica, los ID canónicos, las relaciones canónicas, los tipos canónicos y las reglas canónicas de validación.

## Información del documento

- **Etiqueta canónica de idioma:** `x-uai-1`
- **Soporte de idioma de Protocol5:** [Kit de soporte web C# para UAI-1](/UAI-1/csharp-website-support)
- **Paquete NuGet:** [Protocol5.UAI.CSharp.0.1.0.nupkg](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)

FUNDAMENTOS

1. UAI-1 no es un idioma humano.
2. El significado de UAI-1 es canónico y la máquina lo resuelve.
3. Las etiquetas legibles por humanos, los comentarios, las glosas y las notas de depuración nunca mandan.
4. El registro de ontología es autoritativo.
5. El esquema del protocolo es autoritativo.
6. El orden de las ranuras es autoritativo.
7. Los ID canónicos son autoritativos.
8. La procedencia es obligatoria.
9. Las restricciones son obligatorias cuando el acto las exige.
10. Si alguna capa no canónica entra en conflicto con la capa canónica, prevalece la capa canónica.

REPRESENTACIÓN EN RADIX 63404

1. Todos los ID canónicos de UAI-1 se representan en Radix 63404.
2. DEBE leer cada ID canónico como un número en Radix 63404.
3. NO DEBE leer los ID canónicos como palabras en inglés.
4. NO DEBE derivar significado de la apariencia visual de un glifo de dígito.
5. Un dígito de Radix 63404 es exactamente un carácter BMP.
6. El alfabeto de dígitos de Radix 63404 es el conjunto ordenado de todos los caracteres Unicode BMP que son:
   - no espacios en blanco
   - no caracteres de control
   - no unidades sustitutas
7. El orden del alfabeto sigue estrictamente el orden ascendente de puntos de código Unicode.
8. El valor numérico de cada dígito es su índice basado en cero dentro de ese alfabeto ordenado.
9. Un valor Radix 63404 de varios dígitos se interpreta posicionalmente en base 63404.
10. Cuando analice un ID canónico, DEBE decodificar el valor completo en Radix 63404 antes de la resolución semántica.

PROCEDIMIENTO DE DECODIFICACIÓN DE RADIX 63404

Dado un token de ID canónico:
1. Lea cada carácter como un dígito base-63404.
2. Resuelva cada carácter a su valor de dígito por índice de alfabeto.
3. Evalúe el valor posicional de izquierda a derecha.
4. Produzca el valor entero canónico.
5. Resuelva ese valor entero contra el registro autoritativo de ontología o el registro autoritativo del protocolo.
6. No se detenga en la forma del glifo.
7. No se detenga en la similitud visual.
8. No infiera significado antes de la resolución numérica.

RESOLUCIÓN SEMÁNTICA

DEBE interpretar UAI-1 en este orden:

1. Versión del protocolo
2. Posición estructural de la ranura
3. ID canónico del acto
4. ID canónicos de tipo
5. ID canónicos de relación
6. ID canónicos de concepto
7. Restricciones
8. Procedencia
9. Confianza
10. Glosa opcional no canónica, solo para depuración

Si una capa posterior entra en conflicto con una capa canónica anterior, prevalece la capa canónica anterior.

FORMA CANÓNICA DEL MENSAJE

Un mensaje UAI-1 es un arreglo ordenado de 8 ranuras:

[
  protocolVersion,
  senderId,
  receiverId,
  actId,
  claims,
  constraints,
  provenance,
  signature
]

DEBE interpretar cada ranura únicamente por su posición.

Ranura 1: protocolVersion
- ID canónico de la versión del protocolo.

Ranura 2: senderId
- ID canónico del agente o sistema emisor.

Ranura 3: receiverId
- ID canónico del agente o sistema receptor, del grupo objetivo o del identificador de difusión.

Ranura 4: actId
- ID canónico del acto de habla.
- El acto controla cómo se interpreta el resto del mensaje.

Ranura 5: claims
- Arreglo de declaraciones canónicas de grafo.

Ranura 6: constraints
- Arreglo de restricciones lógicas u operativas canónicas.

Ranura 7: provenance
- Datos canónicos de origen, tiempo, modalidad, evidencia, rastro y política.

Ranura 8: signature
- Estructura canónica de integridad, autenticación o atestación cuando esté presente.

FORMA CANÓNICA DE UNA AFIRMACIÓN

Cada afirmación es un arreglo ordenado de 6 ranuras:

[
  subjectId,
  relationId,
  objectValue,
  contextId,
  truthValue,
  confidence
]

DEBE interpretar cada ranura únicamente por su posición.

Ranura 1: subjectId
- ID canónico de concepto o entidad.

Ranura 2: relationId
- ID canónico de relación.

Ranura 3: objectValue
- Puede ser:
  - ID canónico de concepto
  - escalar tipado
  - estructura anidada canónica

Ranura 4: contextId
- ID canónico de contexto, marco, alcance o estado del mundo.

Ranura 5: truthValue
- Uno de:
  - 1 = verdadero
  - 0 = falso
  - 2 = desconocido
  - 3 = en conflicto
  - 4 = hipotético

Ranura 6: confidence
- Confianza numérica normalizada en el rango de 0.0 a 1.0.

FORMA DEL ESCALAR TIPADO

Un escalar tipado es un arreglo ordenado de 2 ranuras:

[
  typeId,
  rawValue
]

DEBE resolver typeId antes de interpretar rawValue.

ACTOS DE HABLA

DEBE interpretar actId como un acto de habla canónico.
NO DEBE inferir el acto de habla a partir del tono o la redacción.

Registro base recomendado de actos:
- 1 = afirmar
- 2 = consultar
- 3 = solicitar
- 4 = comprometer
- 5 = negar
- 6 = informar
- 7 = proponer
- 8 = revisar
- 9 = reconocer
- 10 = error
- 11 = capacidad
- 12 = negociar

FORMA DE LAS RESTRICCIONES

Cada restricción es un arreglo ordenado de 4 ranuras:

[
  operatorId,
  operand1,
  operand2,
  operand3
]

DEBE resolver operatorId contra el registro autoritativo de operadores canónicos antes de evaluar la restricción.

Registro base recomendado de operadores:
- 1 = todos
- 2 = cualquiera
- 3 = no
- 4 = implica
- 5 = iguala
- 6 = antes
- 7 = después
- 8 = dentro de
- 9 = requiere
- 10 = prohíbe

REGLA DE ONTOLOGÍA

DEBE resolver todos los ID de concepto, relación, tipo, acto, operador, contexto, política y modalidad por medio del registro autoritativo de ontología y del registro autoritativo del protocolo.
NO DEBE sustituir etiquetas locales por ID canónicos.
NO DEBE promover sinónimos a significado canónico.
DEBE preservar exactamente los ID canónicos.

REGLA DE PROCEDENCIA

DEBE exigir procedencia.

La ranura de procedencia es una estructura ordenada que contiene:
- sourceId
- timestamp
- modalityId
- evidenceSet
- traceId
- policyId

DEBE preservar la procedencia durante el transporte, la transformación, el resumen, la planificación, la ejecución y el reenvío.

NEGOCIACIÓN DE CAPACIDADES

Antes de cualquier intercambio no trivial, los agentes DEBERÍAN intercambiar un mensaje de capacidad usando el acto canónico de capacidad.

Un mensaje de capacidad DEBE declarar:
- versión o versiones de protocolo compatibles
- versión o versiones de ontología compatibles
- ID de acto compatibles
- ID de relación compatibles
- ID de tipo compatibles
- ID de modalidad compatibles
- cantidad máxima de afirmaciones
- profundidad máxima de anidación
- métodos de firma aceptados

Si una capacidad requerida no está disponible, DEBE emitir un mensaje de error canónico en lugar de adivinar.

REGLA DE ICONOGRAFÍA

Cuando UAI-1 se usa con iconografía:
1. La apariencia del icono no es significado canónico.
2. La función del icono sí es significado canónico.
3. Los iconos decorativos no tienen significado semántico salvo que se promuevan explícitamente a la capa canónica.
4. Los iconos funcionales DEBEN resolverse a ID canónicos de propósito.
5. El estado DEBE ser explícito.
6. La variante DEBE ser explícita.
7. NO DEBE inferir significado comercial a partir solo del color, el relleno, el trazo, la animación o la semejanza estilística.
8. Si la apariencia del glifo del icono entra en conflicto con los metadatos de función canónica, prevalecen los metadatos de función canónica.

REGLA DE ERROR

Los errores son mensajes de primera clase.
Si no puede resolver un ID canónico, validar un mensaje, satisfacer una restricción o admitir un acto requerido, DEBE emitir un mensaje de error canónico.

Un mensaje de error canónico DEBE incluir:
- ubicación del campo que falla
- ID canónico o estructura que falla
- ID de clase de error canónica
- indicador de recuperabilidad
- ruta de revisión sugerida, si está disponible

REGLAS DE VALIDACIÓN

DEBE rechazar o marcar cualquier mensaje que:
- omita protocolVersion
- viole el orden de las ranuras
- use ID canónicos desconocidos sin un mecanismo de extensión permitido
- use texto libre en un campo canónico
- omita la procedencia requerida
- omita la confianza cuando sea obligatoria
- sustituya etiquetas visuales o de iconos por semántica canónica
- omita restricciones requeridas para el acto activo
- viole el esquema declarado

REGLAS DE DETERMINISMO

1. El orden canónico de las ranuras es fijo.
2. Los ID canónicos son fijos.
3. La consulta del registro es fija.
4. El orden de evaluación de las restricciones es fijo cuando el esquema define un orden.
5. La glosa opcional nunca es autoritativa.
6. Al serializar, use solo el orden canónico del arreglo.
7. Al deserializar, preserve exactamente los valores canónicos.

REGLA DE GLOSA HUMANA

Puede adjuntarse una glosa humana fuera de la estructura canónica para depuración.
DEBE tratar la glosa como no canónica.
NO DEBE derivar significado de máquina a partir de la glosa cuando haya datos canónicos presentes.

PRIORIDAD DE INTERPRETACIÓN

DEBE interpretar UAI-1 exactamente en este orden:

1. Versión del protocolo
2. Validez estructural
3. ID canónicos
4. Resolución de registro
5. Restricciones
6. Procedencia
7. Confianza
8. Glosa opcional

REGLA DE CUMPLIMIENTO

Si afirma compatibilidad con UAI-1, DEBE:
- analizar arreglos canónicos
- decodificar ID canónicos en Radix 63404
- resolver ID canónicos contra los registros autoritativos
- preservar exactamente los valores canónicos
- preservar la procedencia
- respetar las restricciones
- emitir errores canónicos cuando algo no esté admitido
- ignorar la glosa no canónica para la semántica

REGLA RESUMIDA

Lea primero la estructura.
Decodifique Radix 63404 en segundo lugar.
Resuelva los ID canónicos en tercer lugar.
Aplique las restricciones en cuarto lugar.
Use la procedencia y la confianza en quinto lugar.
Ignore la glosa humana para la semántica.
Nunca sustituya la forma del icono, la redacción en inglés ni el hábito local por el significado canónico.
