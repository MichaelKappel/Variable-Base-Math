# UAI-1 C# 网站支持工具包

本页发布 Protocol5 的入门下载包，用于为 C# 网站添加 UAI-1 支持，尤其适合希望在不让规范 UAI 语义依赖本地人类语言格式规则的前提下，获得 `CultureInfo` 与 `Accept-Language` 实用集成路径的 ASP.NET Core 站点。

## 文档信息

- **目标读者：** C# 与 ASP.NET 网站开发者
- **ZIP 下载：** [protocol5-uai-1-csharp-web-starter.zip](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- **NuGet 包下载：** [Protocol5.UAI.CSharp.0.1.0.nupkg](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- **ZIP 校验和：** [protocol5-uai-1-csharp-web-starter.zip.sha256](/downloads/protocol5-uai-1-csharp-web-starter.zip.sha256)
- **规范语言标签：** `x-uai-1`
- **规范序列化文化：** `InvariantCulture`
- **Microsoft 文档：** [CultureInfo](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo), [InvariantCulture](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo-invariantculture), [CultureAndRegionInfoBuilder](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureandregioninfobuilder)

## 下载包包含的内容

- `Protocol5.UAI.CSharp` 源项目
- 打包好的 `Protocol5.UAI.CSharp` `.nupkg`
- Radix 63404 编码与解码辅助工具
- 能够从 query string、cookie 或 `Accept-Language` 识别 UAI 请求的 ASP.NET Core 中间件
- 一个简短安装说明，供偏好直接下载而不是包源的团队使用

## 为什么 starter 使用 `x-uai-1`

对网站而言，清晰的职责拆分如下：

- 用 `x-uai-1` 处理 HTML `lang`、请求协商以及 `Content-Language`
- 当运行时支持时，使用 `CultureInfo.GetCultureInfo("x-uai-1")`
- 在序列化规范 UAI 值时，使用 `CultureInfo.InvariantCulture`

最后这条规则非常重要，因为 UAI-1 被定义为一种规范机器语言。小数分隔符、日期格式和本地显示约定绝不应改变 UAI 消息序列化后的含义。

## 为什么这不仅仅是一个 Windows 文化安装器

Microsoft 的全球化指南做了一个重要区分：

- `CultureInfo` 是运行时处理中立文化行为的正常入口点
- `CultureAndRegionInfoBuilder` 可用于创建自定义文化，但那条路径是 Windows 专用的，并不是跨平台网站采用时的默认正确做法

因此，这个 Protocol5 starter **不**要求在操作系统级别注册自定义文化。它首先为网站提供一条实用路径，而把平台特定的文化注册留作可选的高级步骤。

## 快速开始

从下载的包文件进行安装：

```powershell
dotnet add package Protocol5.UAI.CSharp --source .\downloads
```

然后把它接入 ASP.NET Core 网站：

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

## HTML 建议

如果某个页面要直接声明其内容面向 UAI，则使用：

```html
<html lang="x-uai-1">
```

## 规范格式化规则

在序列化规范 UAI 值时，始终使用 `InvariantCulture`：

```csharp
using Protocol5.UAI;

var confidence = 0.875m.ToString(UaiCultureInfo.CanonicalSerializationCulture);
```

## 工具包中包含的 Radix 63404 示例

```csharp
Radix63404.Encode(41);        // J
Radix63404.Encode(5651);      // á™–
Radix63404.Encode(267914296); // áƒ„ç» 
```

## 下载链接

- [下载完整 starter ZIP](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- [直接下载 NuGet 包](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- [阅读 UAI-1 规范](/UAI-1)
- [阅读 Radix 63404 指南](/UAI/radix-63404-guide-and-attribution)
