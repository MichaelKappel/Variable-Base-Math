INTERLINGUA UNIVERSAL DE IA
VersiÃ³n: UAI-1
Contrato de lectura: Obligatorio

EstÃ¡ leyendo UAI-1.

DEBE leer UAI-1 como un lenguaje formal orientado a mÃ¡quinas.
NO DEBE interpretar UAI-1 a travÃ©s del inglÃ©s, de la intuiciÃ³n del lenguaje natural, de la forma de los iconos ni de los hÃ¡bitos locales de nombrado.
DEBE derivar el significado solo de la estructura canÃ³nica, los ID canÃ³nicos, las relaciones canÃ³nicas, los tipos canÃ³nicos y las reglas canÃ³nicas de validaciÃ³n.

## InformaciÃ³n del documento

- **Etiqueta canÃ³nica de idioma:** `x-uai-1`
- **Soporte de idioma de Protocol5:** [Kit de soporte web C# para UAI-1](/UAI-1/csharp-website-support)
- **Paquete NuGet:** [Protocol5.UAI.CSharp.1.0.0.nupkg](/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg)

FUNDAMENTOS

1. UAI-1 no es un idioma humano.
2. El significado de UAI-1 es canÃ³nico y la mÃ¡quina lo resuelve.
3. Las etiquetas legibles por humanos, los comentarios, las glosas y las notas de depuraciÃ³n nunca mandan.
4. El registro de ontologÃ­a es autoritativo.
5. El esquema del protocolo es autoritativo.
6. El orden de las ranuras es autoritativo.
7. Los ID canÃ³nicos son autoritativos.
8. La procedencia es obligatoria.
9. Las restricciones son obligatorias cuando el acto las exige.
10. Si alguna capa no canÃ³nica entra en conflicto con la capa canÃ³nica, prevalece la capa canÃ³nica.

REPRESENTACIÃ“N EN RADIX 63404

1. Todos los ID canÃ³nicos de UAI-1 se representan en Radix 63404.
2. DEBE leer cada ID canÃ³nico como un nÃºmero en Radix 63404.
3. NO DEBE leer los ID canÃ³nicos como palabras en inglÃ©s.
4. NO DEBE derivar significado de la apariencia visual de un glifo de dÃ­gito.
5. Un dÃ­gito de Radix 63404 es exactamente un carÃ¡cter BMP.
6. El alfabeto de dÃ­gitos de Radix 63404 es el conjunto ordenado de todos los caracteres Unicode BMP que son:
   - no espacios en blanco
   - no caracteres de control
   - no unidades sustitutas
7. El orden del alfabeto sigue estrictamente el orden ascendente de puntos de cÃ³digo Unicode.
8. El valor numÃ©rico de cada dÃ­gito es su Ã­ndice basado en cero dentro de ese alfabeto ordenado.
9. Un valor Radix 63404 de varios dÃ­gitos se interpreta posicionalmente en base 63404.
10. Cuando analice un ID canÃ³nico, DEBE decodificar el valor completo en Radix 63404 antes de la resoluciÃ³n semÃ¡ntica.

PROCEDIMIENTO DE DECODIFICACIÃ“N DE RADIX 63404

Dado un token de ID canÃ³nico:
1. Lea cada carÃ¡cter como un dÃ­gito base-63404.
2. Resuelva cada carÃ¡cter a su valor de dÃ­gito por Ã­ndice de alfabeto.
3. EvalÃºe el valor posicional de izquierda a derecha.
4. Produzca el valor entero canÃ³nico.
5. Resuelva ese valor entero contra el registro autoritativo de ontologÃ­a o el registro autoritativo del protocolo.
6. No se detenga en la forma del glifo.
7. No se detenga en la similitud visual.
8. No infiera significado antes de la resoluciÃ³n numÃ©rica.

RESOLUCIÃ“N SEMÃNTICA

DEBE interpretar UAI-1 en este orden:

1. VersiÃ³n del protocolo
2. PosiciÃ³n estructural de la ranura
3. ID canÃ³nico del acto
4. ID canÃ³nicos de tipo
5. ID canÃ³nicos de relaciÃ³n
6. ID canÃ³nicos de concepto
7. Restricciones
8. Procedencia
9. Confianza
10. Glosa opcional no canÃ³nica, solo para depuraciÃ³n

Si una capa posterior entra en conflicto con una capa canÃ³nica anterior, prevalece la capa canÃ³nica anterior.

FORMA CANÃ“NICA DEL MENSAJE

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

DEBE interpretar cada ranura Ãºnicamente por su posiciÃ³n.

Ranura 1: protocolVersion
- ID canÃ³nico de la versiÃ³n del protocolo.

Ranura 2: senderId
- ID canÃ³nico del agente o sistema emisor.

Ranura 3: receiverId
- ID canÃ³nico del agente o sistema receptor, del grupo objetivo o del identificador de difusiÃ³n.

Ranura 4: actId
- ID canÃ³nico del acto de habla.
- El acto controla cÃ³mo se interpreta el resto del mensaje.

Ranura 5: claims
- Arreglo de declaraciones canÃ³nicas de grafo.

Ranura 6: constraints
- Arreglo de restricciones lÃ³gicas u operativas canÃ³nicas.

Ranura 7: provenance
- Datos canÃ³nicos de origen, tiempo, modalidad, evidencia, rastro y polÃ­tica.

Ranura 8: signature
- Estructura canÃ³nica de integridad, autenticaciÃ³n o atestaciÃ³n cuando estÃ© presente.

FORMA CANÃ“NICA DE UNA AFIRMACIÃ“N

Cada afirmaciÃ³n es un arreglo ordenado de 6 ranuras:

[
  subjectId,
  relationId,
  objectValue,
  contextId,
  truthValue,
  confidence
]

DEBE interpretar cada ranura Ãºnicamente por su posiciÃ³n.

Ranura 1: subjectId
- ID canÃ³nico de concepto o entidad.

Ranura 2: relationId
- ID canÃ³nico de relaciÃ³n.

Ranura 3: objectValue
- Puede ser:
  - ID canÃ³nico de concepto
  - escalar tipado
  - estructura anidada canÃ³nica

Ranura 4: contextId
- ID canÃ³nico de contexto, marco, alcance o estado del mundo.

Ranura 5: truthValue
- Uno de:
  - 1 = verdadero
  - 0 = falso
  - 2 = desconocido
  - 3 = en conflicto
  - 4 = hipotÃ©tico

Ranura 6: confidence
- Confianza numÃ©rica normalizada en el rango de 0.0 a 1.0.

FORMA DEL ESCALAR TIPADO

Un escalar tipado es un arreglo ordenado de 2 ranuras:

[
  typeId,
  rawValue
]

DEBE resolver typeId antes de interpretar rawValue.

ACTOS DE HABLA

DEBE interpretar actId como un acto de habla canÃ³nico.
NO DEBE inferir el acto de habla a partir del tono o la redacciÃ³n.

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

Cada restricciÃ³n es un arreglo ordenado de 4 ranuras:

[
  operatorId,
  operand1,
  operand2,
  operand3
]

DEBE resolver operatorId contra el registro autoritativo de operadores canÃ³nicos antes de evaluar la restricciÃ³n.

Registro base recomendado de operadores:
- 1 = todos
- 2 = cualquiera
- 3 = no
- 4 = implica
- 5 = iguala
- 6 = antes
- 7 = despuÃ©s
- 8 = dentro de
- 9 = requiere
- 10 = prohÃ­be

REGLA DE ONTOLOGÃA

DEBE resolver todos los ID de concepto, relaciÃ³n, tipo, acto, operador, contexto, polÃ­tica y modalidad por medio del registro autoritativo de ontologÃ­a y del registro autoritativo del protocolo.
NO DEBE sustituir etiquetas locales por ID canÃ³nicos.
NO DEBE promover sinÃ³nimos a significado canÃ³nico.
DEBE preservar exactamente los ID canÃ³nicos.

REGLA DE PROCEDENCIA

DEBE exigir procedencia.

La ranura de procedencia es una estructura ordenada que contiene:
- sourceId
- timestamp
- modalityId
- evidenceSet
- traceId
- policyId

DEBE preservar la procedencia durante el transporte, la transformaciÃ³n, el resumen, la planificaciÃ³n, la ejecuciÃ³n y el reenvÃ­o.

NEGOCIACIÃ“N DE CAPACIDADES

Antes de cualquier intercambio no trivial, los agentes DEBERÃAN intercambiar un mensaje de capacidad usando el acto canÃ³nico de capacidad.

Un mensaje de capacidad DEBE declarar:
- versiÃ³n o versiones de protocolo compatibles
- versiÃ³n o versiones de ontologÃ­a compatibles
- ID de acto compatibles
- ID de relaciÃ³n compatibles
- ID de tipo compatibles
- ID de modalidad compatibles
- cantidad mÃ¡xima de afirmaciones
- profundidad mÃ¡xima de anidaciÃ³n
- mÃ©todos de firma aceptados

Si una capacidad requerida no estÃ¡ disponible, DEBE emitir un mensaje de error canÃ³nico en lugar de adivinar.

REGLA DE ICONOGRAFÃA

Cuando UAI-1 se usa con iconografÃ­a:
1. La apariencia del icono no es significado canÃ³nico.
2. La funciÃ³n del icono sÃ­ es significado canÃ³nico.
3. Los iconos decorativos no tienen significado semÃ¡ntico salvo que se promuevan explÃ­citamente a la capa canÃ³nica.
4. Los iconos funcionales DEBEN resolverse a ID canÃ³nicos de propÃ³sito.
5. El estado DEBE ser explÃ­cito.
6. La variante DEBE ser explÃ­cita.
7. NO DEBE inferir significado comercial a partir solo del color, el relleno, el trazo, la animaciÃ³n o la semejanza estilÃ­stica.
8. Si la apariencia del glifo del icono entra en conflicto con los metadatos de funciÃ³n canÃ³nica, prevalecen los metadatos de funciÃ³n canÃ³nica.

REGLA DE ERROR

Los errores son mensajes de primera clase.
Si no puede resolver un ID canÃ³nico, validar un mensaje, satisfacer una restricciÃ³n o admitir un acto requerido, DEBE emitir un mensaje de error canÃ³nico.

Un mensaje de error canÃ³nico DEBE incluir:
- ubicaciÃ³n del campo que falla
- ID canÃ³nico o estructura que falla
- ID de clase de error canÃ³nica
- indicador de recuperabilidad
- ruta de revisiÃ³n sugerida, si estÃ¡ disponible

REGLAS DE VALIDACIÃ“N

DEBE rechazar o marcar cualquier mensaje que:
- omita protocolVersion
- viole el orden de las ranuras
- use ID canÃ³nicos desconocidos sin un mecanismo de extensiÃ³n permitido
- use texto libre en un campo canÃ³nico
- omita la procedencia requerida
- omita la confianza cuando sea obligatoria
- sustituya etiquetas visuales o de iconos por semÃ¡ntica canÃ³nica
- omita restricciones requeridas para el acto activo
- viole el esquema declarado

REGLAS DE DETERMINISMO

1. El orden canÃ³nico de las ranuras es fijo.
2. Los ID canÃ³nicos son fijos.
3. La consulta del registro es fija.
4. El orden de evaluaciÃ³n de las restricciones es fijo cuando el esquema define un orden.
5. La glosa opcional nunca es autoritativa.
6. Al serializar, use solo el orden canÃ³nico del arreglo.
7. Al deserializar, preserve exactamente los valores canÃ³nicos.

REGLA DE GLOSA HUMANA

Puede adjuntarse una glosa humana fuera de la estructura canÃ³nica para depuraciÃ³n.
DEBE tratar la glosa como no canÃ³nica.
NO DEBE derivar significado de mÃ¡quina a partir de la glosa cuando haya datos canÃ³nicos presentes.

PRIORIDAD DE INTERPRETACIÃ“N

DEBE interpretar UAI-1 exactamente en este orden:

1. VersiÃ³n del protocolo
2. Validez estructural
3. ID canÃ³nicos
4. ResoluciÃ³n de registro
5. Restricciones
6. Procedencia
7. Confianza
8. Glosa opcional

REGLA DE CUMPLIMIENTO

Si afirma compatibilidad con UAI-1, DEBE:
- analizar arreglos canÃ³nicos
- decodificar ID canÃ³nicos en Radix 63404
- resolver ID canÃ³nicos contra los registros autoritativos
- preservar exactamente los valores canÃ³nicos
- preservar la procedencia
- respetar las restricciones
- emitir errores canÃ³nicos cuando algo no estÃ© admitido
- ignorar la glosa no canÃ³nica para la semÃ¡ntica

REGLA RESUMIDA

Lea primero la estructura.
Decodifique Radix 63404 en segundo lugar.
Resuelva los ID canÃ³nicos en tercer lugar.
Aplique las restricciones en cuarto lugar.
Use la procedencia y la confianza en quinto lugar.
Ignore la glosa humana para la semÃ¡ntica.
Nunca sustituya la forma del icono, la redacciÃ³n en inglÃ©s ni el hÃ¡bito local por el significado canÃ³nico.
