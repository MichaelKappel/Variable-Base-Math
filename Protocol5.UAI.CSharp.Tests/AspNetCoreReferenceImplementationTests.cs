using System.Net;
using System.Net.Sockets;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Protocol5.UAI.CSharp.Tests;

[TestClass]
public sealed class AspNetCoreReferenceImplementationTests
{
    [TestMethod]
    public async Task ReferenceImplementation_MapsCanonicalArtifactsAndWorkingHtmlExportEndpoint()
    {
        var port = GetFreeTcpPort();
        var baseAddress = new Uri($"http://127.0.0.1:{port}");
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls(baseAddress.ToString());
        builder.Services.AddProtocol5UaiWebsiteSupport();

        await using var app = builder.Build();
        app.UseProtocol5UaiWebsiteSupport();
        app.MapProtocol5UaiCanonicalArtifacts();
        app.MapProtocol5UaiHtmlEndpoint(
            "/demo/index.uai.json",
            static () => "<html lang=\"en\"><body><header><h1>Hello</h1></header><section><p>World</p></section></body></html>",
            new UaiHtmlTranslationOptions
            {
                SourceUri = "https://example.org/demo",
                DocumentId = "demo-page",
                PageType = "landing-page",
                SiteName = "Example"
            });
        app.MapGet("/demo", (HttpContext context) => Results.Text(context.GetProtocol5HtmlLanguage()));

        await app.StartAsync();

        try
        {
            using var client = new HttpClient { BaseAddress = baseAddress };

            var discoveryResponse = await client.GetAsync(UaiConstants.CanonicalMachineSpecPublicPath);
            Assert.AreEqual(HttpStatusCode.OK, discoveryResponse.StatusCode);
            Assert.IsTrue(discoveryResponse.Content.Headers.ContentType!.MediaType!.StartsWith("application/json", StringComparison.Ordinal));
            var discoveryJson = await discoveryResponse.Content.ReadAsStringAsync();
            StringAssert.Contains(discoveryJson, "machineEndpoints");

            var examplesAliasResponse = await client.GetAsync(UaiConstants.CanonicalExamplesRegistryPublicPath);
            Assert.AreEqual(HttpStatusCode.OK, examplesAliasResponse.StatusCode);
            var examplesAliasJson = await examplesAliasResponse.Content.ReadAsStringAsync();
            StringAssert.Contains(examplesAliasJson, "examples");

            var endpointRequest = new HttpRequestMessage(HttpMethod.Get, "/demo/index.uai.json");
            endpointRequest.Headers.Accept.ParseAdd(UaiConstants.MediaType);
            var endpointResponse = await client.SendAsync(endpointRequest);
            Assert.AreEqual(HttpStatusCode.OK, endpointResponse.StatusCode);
            Assert.IsTrue(endpointResponse.Content.Headers.ContentType!.MediaType!.Equals(UaiConstants.MediaType, StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual(UaiConstants.CurrentDocumentVersion, endpointResponse.Content.Headers.ContentType!.Parameters.Single(parameter => parameter.Name == "version").Value?.Trim('"'));
            Assert.AreEqual(UaiConstants.LegacyCompatibilityVersion, endpointResponse.Headers.GetValues(UaiConstants.LegacyHttpHeader).Single());
            StringAssert.Contains(endpointResponse.Headers.GetValues(UaiConstants.LinkHeader).Single(), UaiConstants.CanonicalRegistryPublicPath);
            StringAssert.Contains(endpointResponse.Headers.GetValues(UaiConstants.LinkHeader).Single(), UaiConstants.CanonicalSchemaPublicPath);
            var vary = string.Join(",", endpointResponse.Headers.Vary);
            StringAssert.Contains(vary, UaiConstants.AcceptHeader);
            StringAssert.Contains(vary, UaiConstants.LegacyHttpHeader);

            var json = await endpointResponse.Content.ReadAsStringAsync();
            var document = new UaiDocumentParser().Parse(json);
            var validation = new UaiDocumentValidator().Validate(document);
            Assert.IsTrue(validation.IsValid, string.Join("; ", validation.Errors.Select(error => $"{error.Code}:{error.Path}")));
            Assert.AreEqual("demo-page", document.DocumentId);
            Assert.AreEqual("https://example.org/demo", document.Source.Uri);

            var htmlLanguage = await client.GetStringAsync("/demo?lang=x-uai-1");
            Assert.AreEqual(UaiCultureInfo.CanonicalLanguageTag, htmlLanguage);
        }
        finally
        {
            await app.StopAsync();
        }
    }

    private static int GetFreeTcpPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
