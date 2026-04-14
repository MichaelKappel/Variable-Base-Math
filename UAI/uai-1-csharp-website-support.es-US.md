# Kit de soporte web C# para UAI-1

Esta página publica la descarga inicial de Protocol5 para agregar compatibilidad con UAI-1 a sitios web de C#, en especial a sitios ASP.NET Core que quieren una ruta práctica de integración con `CultureInfo` y `Accept-Language` sin hacer que la semántica canónica de UAI dependa de reglas locales de formato humano.

## Información del documento

- **Audiencia:** desarrolladores de sitios web en C# y ASP.NET
- **Descarga ZIP:** [protocol5-uai-1-csharp-web-starter.zip](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- **Descarga del paquete NuGet:** [Protocol5.UAI.CSharp.0.1.0.nupkg](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- **Suma de verificación del ZIP:** [protocol5-uai-1-csharp-web-starter.zip.sha256](/downloads/protocol5-uai-1-csharp-web-starter.zip.sha256)
- **Etiqueta canónica de idioma:** `x-uai-1`
- **Cultura canónica de serialización:** `InvariantCulture`
- **Documentación de Microsoft:** [CultureInfo](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo), [InvariantCulture](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo-invariantculture), [CultureAndRegionInfoBuilder](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureandregioninfobuilder)

## Qué contiene la descarga

- proyecto fuente `Protocol5.UAI.CSharp`
- un `.nupkg` empaquetado de `Protocol5.UAI.CSharp`
- utilidades para codificar y decodificar Radix 63404
- middleware de ASP.NET Core que reconoce solicitudes UAI desde query string, cookie o `Accept-Language`
- un readme breve de instalación para equipos que prefieren una descarga directa en lugar de un feed de paquetes

## Por qué el starter usa `x-uai-1`

Para sitios web, la separación limpia es esta:

- usar `x-uai-1` para `lang` en HTML, la negociación de solicitudes y `Content-Language`
- usar `CultureInfo.GetCultureInfo("x-uai-1")` cuando el runtime lo permita
- usar `CultureInfo.InvariantCulture` al serializar valores canónicos de UAI

Esa última regla importa porque UAI-1 está definido como un lenguaje canónico para máquinas. Los separadores decimales, los formatos de fecha y las convenciones locales de presentación nunca deben cambiar el significado serializado de un mensaje UAI.

## Por qué esto no es solo un instalador de cultura para Windows

La guía de globalización de Microsoft hace una distinción importante:

- `CultureInfo` es el punto de entrada normal del runtime para comportamiento sensible a cultura
- `CultureAndRegionInfoBuilder` existe para crear culturas personalizadas, pero ese camino es específico de Windows y no es la opción correcta por defecto para una adopción web multiplataforma

Por eso este starter de Protocol5 **no** exige registrar culturas personalizadas a nivel de sistema operativo. Primero ofrece a los sitios web una ruta práctica, y deja el registro de culturas específico por plataforma como un paso avanzado opcional.

## Inicio rápido

Instale desde el archivo del paquete descargado:

```powershell
dotnet add package Protocol5.UAI.CSharp --source .\downloads
```

Luego conéctelo a un sitio ASP.NET Core:

```csharp
using Protocol5.UAI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProtocol5UaiWebsiteSupport();

var app = builder.Build();
app.UseProtocol5UaiWebsiteSupport();

app.MapGet("/uai-demo", (HttpContext context) =>
{
    var sampleCanonicalId = Radix63404.Encode(5651);

    return Results.Json(new
    {
        protocol = UaiCultureInfo.CanonicalVersion,
        language = context.GetProtocol5HtmlLanguage(),
        sampleCanonicalId
    });
});

app.Run();
```

## Recomendación para HTML

Si una página debe declarar contenido compatible con UAI de forma directa, use:

```html
<html lang="x-uai-1">
```

## Regla de formato canónico

Al serializar valores canónicos de UAI, use siempre `InvariantCulture`:

```csharp
using Protocol5.UAI;

var confidence = 0.875m.ToString(UaiCultureInfo.CanonicalSerializationCulture);
```

## Ejemplos de Radix 63404 incluidos en el kit

```csharp
Radix63404.Encode(41);        // J
Radix63404.Encode(5651);      // á™–
Radix63404.Encode(267914296); // áƒ„ç» 
```

## Enlaces de descarga

- [Descargar el ZIP completo del starter](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- [Descargar el paquete NuGet directamente](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- [Leer la especificación de UAI-1](/UAI-1)
- [Leer la guía de Radix 63404](/UAI/radix-63404-guide-and-attribution)
