# UAI-1 C# ç½‘ç«™æ”¯æŒå·¥å…·åŒ…

æœ¬é¡µå‘å¸ƒ Protocol5 çš„å…¥é—¨ä¸‹è½½åŒ…ï¼Œç”¨äºŽä¸º C# ç½‘ç«™æ·»åŠ  UAI-1 æ”¯æŒï¼Œå°¤å…¶é€‚åˆå¸Œæœ›åœ¨ä¸è®©è§„èŒƒ UAI è¯­ä¹‰ä¾èµ–æœ¬åœ°äººç±»è¯­è¨€æ ¼å¼è§„åˆ™çš„å‰æä¸‹ï¼ŒèŽ·å¾— `CultureInfo` ä¸Ž `Accept-Language` å®žç”¨é›†æˆè·¯å¾„çš„ ASP.NET Core ç«™ç‚¹ã€‚

## æ–‡æ¡£ä¿¡æ¯

- **ç›®æ ‡è¯»è€…ï¼š** C# ä¸Ž ASP.NET ç½‘ç«™å¼€å‘è€…
- **ZIP ä¸‹è½½ï¼š** [protocol5-uai-1-csharp-web-starter.zip](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- **NuGet åŒ…ä¸‹è½½ï¼š** [Protocol5.UAI.CSharp.1.0.0.nupkg](/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg)
- **ZIP æ ¡éªŒå’Œï¼š** [protocol5-uai-1-csharp-web-starter.zip.sha256](/downloads/protocol5-uai-1-csharp-web-starter.zip.sha256)
- **è§„èŒƒè¯­è¨€æ ‡ç­¾ï¼š** `x-uai-1`
- **è§„èŒƒåºåˆ—åŒ–æ–‡åŒ–ï¼š** `InvariantCulture`
- **Microsoft æ–‡æ¡£ï¼š** [CultureInfo](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo), [InvariantCulture](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo-invariantculture), [CultureAndRegionInfoBuilder](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureandregioninfobuilder)

## ä¸‹è½½åŒ…åŒ…å«çš„å†…å®¹

- `Protocol5.UAI.CSharp` æºé¡¹ç›®
- æ‰“åŒ…å¥½çš„ `Protocol5.UAI.CSharp` `.nupkg`
- Radix 63404 ç¼–ç ä¸Žè§£ç è¾…åŠ©å·¥å…·
- èƒ½å¤Ÿä»Ž query stringã€cookie æˆ– `Accept-Language` è¯†åˆ« UAI è¯·æ±‚çš„ ASP.NET Core ä¸­é—´ä»¶
- ä¸€ä¸ªç®€çŸ­å®‰è£…è¯´æ˜Žï¼Œä¾›åå¥½ç›´æŽ¥ä¸‹è½½è€Œä¸æ˜¯åŒ…æºçš„å›¢é˜Ÿä½¿ç”¨

## ä¸ºä»€ä¹ˆ starter ä½¿ç”¨ `x-uai-1`

å¯¹ç½‘ç«™è€Œè¨€ï¼Œæ¸…æ™°çš„èŒè´£æ‹†åˆ†å¦‚ä¸‹ï¼š

- ç”¨ `x-uai-1` å¤„ç† HTML `lang`ã€è¯·æ±‚åå•†ä»¥åŠ `Content-Language`
- å½“è¿è¡Œæ—¶æ”¯æŒæ—¶ï¼Œä½¿ç”¨ `CultureInfo.GetCultureInfo("x-uai-1")`
- åœ¨åºåˆ—åŒ–è§„èŒƒ UAI å€¼æ—¶ï¼Œä½¿ç”¨ `CultureInfo.InvariantCulture`

æœ€åŽè¿™æ¡è§„åˆ™éžå¸¸é‡è¦ï¼Œå› ä¸º UAI-1 è¢«å®šä¹‰ä¸ºä¸€ç§è§„èŒƒæœºå™¨è¯­è¨€ã€‚å°æ•°åˆ†éš”ç¬¦ã€æ—¥æœŸæ ¼å¼å’Œæœ¬åœ°æ˜¾ç¤ºçº¦å®šç»ä¸åº”æ”¹å˜ UAI æ¶ˆæ¯åºåˆ—åŒ–åŽçš„å«ä¹‰ã€‚

## ä¸ºä»€ä¹ˆè¿™ä¸ä»…ä»…æ˜¯ä¸€ä¸ª Windows æ–‡åŒ–å®‰è£…å™¨

Microsoft çš„å…¨çƒåŒ–æŒ‡å—åšäº†ä¸€ä¸ªé‡è¦åŒºåˆ†ï¼š

- `CultureInfo` æ˜¯è¿è¡Œæ—¶å¤„ç†ä¸­ç«‹æ–‡åŒ–è¡Œä¸ºçš„æ­£å¸¸å…¥å£ç‚¹
- `CultureAndRegionInfoBuilder` å¯ç”¨äºŽåˆ›å»ºè‡ªå®šä¹‰æ–‡åŒ–ï¼Œä½†é‚£æ¡è·¯å¾„æ˜¯ Windows ä¸“ç”¨çš„ï¼Œå¹¶ä¸æ˜¯è·¨å¹³å°ç½‘ç«™é‡‡ç”¨æ—¶çš„é»˜è®¤æ­£ç¡®åšæ³•

å› æ­¤ï¼Œè¿™ä¸ª Protocol5 starter **ä¸**è¦æ±‚åœ¨æ“ä½œç³»ç»Ÿçº§åˆ«æ³¨å†Œè‡ªå®šä¹‰æ–‡åŒ–ã€‚å®ƒé¦–å…ˆä¸ºç½‘ç«™æä¾›ä¸€æ¡å®žç”¨è·¯å¾„ï¼Œè€ŒæŠŠå¹³å°ç‰¹å®šçš„æ–‡åŒ–æ³¨å†Œç•™ä½œå¯é€‰çš„é«˜çº§æ­¥éª¤ã€‚

## å¿«é€Ÿå¼€å§‹

ä»Žä¸‹è½½çš„åŒ…æ–‡ä»¶è¿›è¡Œå®‰è£…ï¼š

```powershell
dotnet add package Protocol5.UAI.CSharp --source .\downloads
```

ç„¶åŽæŠŠå®ƒæŽ¥å…¥ ASP.NET Core ç½‘ç«™ï¼š

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

## HTML å»ºè®®

å¦‚æžœæŸä¸ªé¡µé¢è¦ç›´æŽ¥å£°æ˜Žå…¶å†…å®¹é¢å‘ UAIï¼Œåˆ™ä½¿ç”¨ï¼š

```html
<html lang="x-uai-1">
```

## è§„èŒƒæ ¼å¼åŒ–è§„åˆ™

åœ¨åºåˆ—åŒ–è§„èŒƒ UAI å€¼æ—¶ï¼Œå§‹ç»ˆä½¿ç”¨ `InvariantCulture`ï¼š

```csharp
using Protocol5.UAI;

var confidence = 0.875m.ToString(UaiCultureInfo.CanonicalSerializationCulture);
```

## å·¥å…·åŒ…ä¸­åŒ…å«çš„ Radix 63404 ç¤ºä¾‹

```csharp
Radix63404.Encode(41);        // J
Radix63404.Encode(5651);      // Ã¡â„¢â€“
Radix63404.Encode(267914296); // Ã¡Æ’â€žÃ§Â»Â 
```

## ä¸‹è½½é“¾æŽ¥

- [ä¸‹è½½å®Œæ•´ starter ZIP](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- [ç›´æŽ¥ä¸‹è½½ NuGet åŒ…](/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg)
- [é˜…è¯» UAI-1 è§„èŒƒ](/UAI-1)
- [é˜…è¯» Radix 63404 æŒ‡å—](/UAI/radix-63404-guide-and-attribution)
