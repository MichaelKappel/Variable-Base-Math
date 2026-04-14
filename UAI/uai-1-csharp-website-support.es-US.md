# Kit de soporte web C# para UAI-1

Esta pÃ¡gina publica la descarga inicial de Protocol5 para agregar compatibilidad con UAI-1 a sitios web de C#, en especial a sitios ASP.NET Core que quieren una ruta prÃ¡ctica de integraciÃ³n con `CultureInfo` y `Accept-Language` sin hacer que la semÃ¡ntica canÃ³nica de UAI dependa de reglas locales de formato humano.

## InformaciÃ³n del documento

- **Audiencia:** desarrolladores de sitios web en C# y ASP.NET
- **Descarga ZIP:** [protocol5-uai-1-csharp-web-starter.zip](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- **Descarga del paquete NuGet:** [Protocol5.UAI.CSharp.1.0.0.nupkg](/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg)
- **Suma de verificaciÃ³n del ZIP:** [protocol5-uai-1-csharp-web-starter.zip.sha256](/downloads/protocol5-uai-1-csharp-web-starter.zip.sha256)
- **Etiqueta canÃ³nica de idioma:** `x-uai-1`
- **Cultura canÃ³nica de serializaciÃ³n:** `InvariantCulture`
- **DocumentaciÃ³n de Microsoft:** [CultureInfo](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo), [InvariantCulture](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo-invariantculture), [CultureAndRegionInfoBuilder](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureandregioninfobuilder)

## QuÃ© contiene la descarga

- proyecto fuente `Protocol5.UAI.CSharp`
- un `.nupkg` empaquetado de `Protocol5.UAI.CSharp`
- utilidades para codificar y decodificar Radix 63404
- middleware de ASP.NET Core que reconoce solicitudes UAI desde query string, cookie o `Accept-Language`
- un readme breve de instalaciÃ³n para equipos que prefieren una descarga directa en lugar de un feed de paquetes

## Por quÃ© el starter usa `x-uai-1`

Para sitios web, la separaciÃ³n limpia es esta:

- usar `x-uai-1` para `lang` en HTML, la negociaciÃ³n de solicitudes y `Content-Language`
- usar `CultureInfo.GetCultureInfo("x-uai-1")` cuando el runtime lo permita
- usar `CultureInfo.InvariantCulture` al serializar valores canÃ³nicos de UAI

Esa Ãºltima regla importa porque UAI-1 estÃ¡ definido como un lenguaje canÃ³nico para mÃ¡quinas. Los separadores decimales, los formatos de fecha y las convenciones locales de presentaciÃ³n nunca deben cambiar el significado serializado de un mensaje UAI.

## Por quÃ© esto no es solo un instalador de cultura para Windows

La guÃ­a de globalizaciÃ³n de Microsoft hace una distinciÃ³n importante:

- `CultureInfo` es el punto de entrada normal del runtime para comportamiento sensible a cultura
- `CultureAndRegionInfoBuilder` existe para crear culturas personalizadas, pero ese camino es especÃ­fico de Windows y no es la opciÃ³n correcta por defecto para una adopciÃ³n web multiplataforma

Por eso este starter de Protocol5 **no** exige registrar culturas personalizadas a nivel de sistema operativo. Primero ofrece a los sitios web una ruta prÃ¡ctica, y deja el registro de culturas especÃ­fico por plataforma como un paso avanzado opcional.

## Inicio rÃ¡pido

Instale desde el archivo del paquete descargado:

```powershell
dotnet add package Protocol5.UAI.CSharp --source .\downloads
```

Luego conÃ©ctelo a un sitio ASP.NET Core:

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

## RecomendaciÃ³n para HTML

Si una pÃ¡gina debe declarar contenido compatible con UAI de forma directa, use:

```html
<html lang="x-uai-1">
```

## Regla de formato canÃ³nico

Al serializar valores canÃ³nicos de UAI, use siempre `InvariantCulture`:

```csharp
using Protocol5.UAI;

var confidence = 0.875m.ToString(UaiCultureInfo.CanonicalSerializationCulture);
```

## Ejemplos de Radix 63404 incluidos en el kit

```csharp
Radix63404.Encode(41);        // J
Radix63404.Encode(5651);      // Ã¡â„¢â€“
Radix63404.Encode(267914296); // Ã¡Æ’â€žÃ§Â»Â 
```

## Enlaces de descarga

- [Descargar el ZIP completo del starter](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- [Descargar el paquete NuGet directamente](/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg)
- [Leer la especificaciÃ³n de UAI-1](/UAI-1)
- [Leer la guÃ­a de Radix 63404](/UAI/radix-63404-guide-and-attribution)
