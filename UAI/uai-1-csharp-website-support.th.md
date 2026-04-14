# ชุดรองรับเว็บไซต์ C# สำหรับ UAI-1

หน้านี้เผยแพร่ชุดดาวน์โหลดเริ่มต้นของ Protocol5 สำหรับเพิ่มการรองรับ UAI-1 ให้กับเว็บไซต์ C# โดยเฉพาะเว็บไซต์ ASP.NET Core ที่ต้องการแนวทางใช้งาน `CultureInfo` และ `Accept-Language` แบบปฏิบัติได้จริง โดยไม่ทำให้ semantic แบบแคนนอนิคัลของ UAI ต้องขึ้นกับกฎการจัดรูปแบบภาษามนุษย์ในท้องถิ่น

## ข้อมูลเอกสาร

- **กลุ่มเป้าหมาย:** นักพัฒนาเว็บไซต์ C# และ ASP.NET
- **ดาวน์โหลด ZIP:** [protocol5-uai-1-csharp-web-starter.zip](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- **ดาวน์โหลดแพ็กเกจ NuGet:** [Protocol5.UAI.CSharp.0.1.0.nupkg](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- **เช็กซัมของ ZIP:** [protocol5-uai-1-csharp-web-starter.zip.sha256](/downloads/protocol5-uai-1-csharp-web-starter.zip.sha256)
- **แท็กภาษามาตรฐาน:** `x-uai-1`
- **วัฒนธรรมมาตรฐานสำหรับการ serialize:** `InvariantCulture`
- **เอกสารของ Microsoft:** [CultureInfo](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo), [InvariantCulture](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo-invariantculture), [CultureAndRegionInfoBuilder](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureandregioninfobuilder)

## สิ่งที่อยู่ในชุดดาวน์โหลด

- โปรเจ็กต์ซอร์ส `Protocol5.UAI.CSharp`
- ไฟล์ `.nupkg` ของ `Protocol5.UAI.CSharp` ที่แพ็กไว้แล้ว
- ตัวช่วยเข้ารหัสและถอดรหัส Radix 63404
- มิดเดิลแวร์ ASP.NET Core ที่รู้จักคำขอ UAI จาก query string, cookie หรือ `Accept-Language`
- readme ติดตั้งขนาดสั้นสำหรับทีมที่ต้องการดาวน์โหลดตรงแทนการใช้ package feed

## เหตุใด starter นี้จึงใช้ `x-uai-1`

สำหรับเว็บไซต์ การแยกหน้าที่ที่ชัดเจนคือ:

- ใช้ `x-uai-1` สำหรับ HTML `lang`, การเจรจาคำขอ และ `Content-Language`
- ใช้ `CultureInfo.GetCultureInfo("x-uai-1")` เมื่อ runtime รองรับ
- ใช้ `CultureInfo.InvariantCulture` เมื่อ serialize ค่าของ UAI แบบแคนนอนิคัล

กฎข้อสุดท้ายนั้นสำคัญมาก เพราะ UAI-1 ถูกนิยามให้เป็นภาษาของเครื่องแบบแคนนอนิคัล ตัวคั่นทศนิยม รูปแบบวันที่ และธรรมเนียมการแสดงผลของแต่ละท้องถิ่น จะต้องไม่เปลี่ยนความหมายที่ถูก serialize ของข้อความ UAI

## เหตุใดสิ่งนี้จึงไม่ใช่แค่ตัวติดตั้ง culture บน Windows

แนวทางด้าน globalization ของ Microsoft แยกความต่างสำคัญไว้ดังนี้:

- `CultureInfo` คือจุดเข้าใช้งานตามปกติของ runtime สำหรับพฤติกรรมที่อ่อนไหวต่อ culture
- `CultureAndRegionInfoBuilder` มีไว้สำหรับสร้าง culture แบบกำหนดเอง แต่แนวทางนั้นผูกกับ Windows และไม่ใช่ค่าเริ่มต้นที่เหมาะสำหรับการนำไปใช้บนเว็บไซต์ข้ามแพลตฟอร์ม

ด้วยเหตุนี้ starter ของ Protocol5 ตัวนี้จึง **ไม่** ต้องการการลงทะเบียน custom culture ในระดับระบบปฏิบัติการ มันให้เส้นทางที่ใช้งานได้จริงแก่เว็บไซต์ก่อน และปล่อยเรื่องการลงทะเบียน culture ที่เฉพาะแพลตฟอร์มไว้เป็นขั้นสูงแบบเลือกใช้

## เริ่มต้นอย่างรวดเร็ว

ติดตั้งจากไฟล์แพ็กเกจที่ดาวน์โหลดมา:

```powershell
dotnet add package Protocol5.UAI.CSharp --source .\downloads
```

จากนั้นต่อเข้ากับเว็บไซต์ ASP.NET Core:

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

## คำแนะนำสำหรับ HTML

หากหน้าหนึ่งตั้งใจจะประกาศเนื้อหาที่รองรับ UAI โดยตรง ให้ใช้:

```html
<html lang="x-uai-1">
```

## กฎการจัดรูปแบบแบบแคนนอนิคัล

เมื่อ serialize ค่าของ UAI แบบแคนนอนิคัล ให้ใช้ `InvariantCulture` เสมอ:

```csharp
using Protocol5.UAI;

var confidence = 0.875m.ToString(UaiCultureInfo.CanonicalSerializationCulture);
```

## ตัวอย่าง Radix 63404 ที่มากับชุดเครื่องมือ

```csharp
Radix63404.Encode(41);        // J
Radix63404.Encode(5651);      // á™–
Radix63404.Encode(267914296); // áƒ„ç» 
```

## ลิงก์ดาวน์โหลด

- [ดาวน์โหลด starter ZIP ฉบับเต็ม](/downloads/protocol5-uai-1-csharp-web-starter.zip)
- [ดาวน์โหลดแพ็กเกจ NuGet โดยตรง](/downloads/Protocol5.UAI.CSharp.0.1.0.nupkg)
- [อ่านสเปก UAI-1](/UAI-1)
- [อ่านคู่มือ Radix 63404](/UAI/radix-63404-guide-and-attribution)
