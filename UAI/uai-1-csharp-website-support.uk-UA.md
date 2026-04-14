# Комплект підтримки UAI-1 для C# вебсайтів

На цій сторінці публікується стартове завантаження Protocol5 для додавання підтримки UAI-1 до C# вебсайтів, особливо до сайтів ASP.NET Core, яким потрібен практичний шлях інтеграції `CultureInfo` і `Accept-Language`, але без залежності канонічної семантики UAI від локальних людських правил форматування.

## Відомості про документ

- **Аудиторія:** розробники вебсайтів на C# та ASP.NET
- **ZIP для завантаження:** [protocol5-uai-1-csharp-web-starter.zip](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- **NuGet-пакет для завантаження:** [Protocol5.UAI.CSharp.0.1.0.nupkg](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- **Контрольна сума ZIP:** [protocol5-uai-1-csharp-web-starter.zip.sha256](/downloads/protocol5-uai-1-csharp-web-starter.zip.sha256)
- **Канонічний мовний тег:** `x-uai-1`
- **Канонічна культура серіалізації:** `InvariantCulture`
- **Документація Microsoft:** [CultureInfo](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo), [InvariantCulture](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo-invariantculture), [CultureAndRegionInfoBuilder](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureandregioninfobuilder)

## Що містить завантаження

- вихідний проєкт `Protocol5.UAI.CSharp`
- зібраний `.nupkg` для `Protocol5.UAI.CSharp`
- допоміжні засоби кодування та декодування Radix 63404
- middleware ASP.NET Core, який розпізнає UAI-запити з query string, cookie або `Accept-Language`
- короткий readme для інсталяції для команд, які надають перевагу прямому завантаженню замість каналу пакетів

## Чому starter використовує `x-uai-1`

Для вебсайтів чистий поділ виглядає так:

- використовуйте `x-uai-1` для HTML `lang`, узгодження запитів і `Content-Language`
- використовуйте `CultureInfo.GetCultureInfo("x-uai-1")`, коли це підтримує runtime
- використовуйте `CultureInfo.InvariantCulture` під час серіалізації канонічних значень UAI

Останнє правило важливе, тому що UAI-1 визначено як канонічну машинну мову. Десяткові роздільники, формати дат і локальні правила відображення ніколи не повинні змінювати серіалізоване значення повідомлення UAI.

## Чому це не просто інсталятор культури для Windows

Настанови Microsoft щодо глобалізації роблять важливе розрізнення:

- `CultureInfo` — це звичайна точка входу runtime для поведінки, чутливої до культури
- `CultureAndRegionInfoBuilder` існує для створення власних культур, але цей шлях прив'язаний до Windows і не є правильною стандартною опцією для кросплатформного впровадження на вебсайтах

Через це цей starter від Protocol5 **не** вимагає реєстрації користувацької культури на рівні операційної системи. Він спочатку дає вебсайтам практичний шлях, а реєстрацію культур, специфічну для платформи, залишає як необов'язковий просунутий крок.

## Швидкий старт

Встановіть пакет із завантаженого файла:

```powershell
dotnet add package Protocol5.UAI.CSharp --source .\downloads
```

Після цього підключіть його до сайту ASP.NET Core:

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

## Рекомендація для HTML

Якщо сторінка має безпосередньо оголошувати UAI-сумісний контент, використовуйте:

```html
<html lang="x-uai-1">
```

## Правило канонічного форматування

Під час серіалізації канонічних значень UAI завжди використовуйте `InvariantCulture`:

```csharp
using Protocol5.UAI;

var confidence = 0.875m.ToString(UaiCultureInfo.CanonicalSerializationCulture);
```

## Приклади Radix 63404, що входять до комплекту

```csharp
Radix63404.Encode(41);        // J
Radix63404.Encode(5651);      // á™–
Radix63404.Encode(267914296); // áƒ„ç» 
```

## Посилання для завантаження

- [Завантажити повний starter ZIP](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- [Завантажити NuGet-пакет напряму](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- [Прочитати специфікацію UAI-1](/UAI-1)
- [Прочитати довідник Radix 63404](/UAI/radix-63404-guide-and-attribution)
