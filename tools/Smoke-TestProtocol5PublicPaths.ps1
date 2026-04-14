param(
    [string]$BaseUrl = 'https://protocol5.com',
    [int]$TimeoutSeconds = 30
)

$ErrorActionPreference = 'Stop'

function New-HttpClient {
    param([int]$TimeoutSeconds)

    $handler = [System.Net.Http.HttpClientHandler]::new()
    $handler.AllowAutoRedirect = $false

    $client = [System.Net.Http.HttpClient]::new($handler)
    $client.Timeout = [TimeSpan]::FromSeconds($TimeoutSeconds)
    return $client
}

function Test-Protocol5Path {
    param(
        [Parameter(Mandatory = $true)]
        [System.Net.Http.HttpClient]$Client,
        [Parameter(Mandatory = $true)]
        [Uri]$BaseUri,
        [Parameter(Mandatory = $true)]
        [hashtable]$Check
    )

    $requestUri = [Uri]::new($BaseUri, $Check.Path)
    $response = $Client.GetAsync($requestUri).GetAwaiter().GetResult()
    $statusCode = [int]$response.StatusCode
    $contentType = if ($response.Content.Headers.ContentType) { $response.Content.Headers.ContentType.MediaType } else { '' }
    $location = if ($response.Headers.Location) { $response.Headers.Location.OriginalString } else { '' }

    if ($Check.ContainsKey('RedirectLocation')) {
        if ($statusCode -lt 300 -or $statusCode -gt 399) {
            throw "Expected redirect for $($Check.Path) but received HTTP $statusCode."
        }

        if (-not [string]::Equals($location, $Check.RedirectLocation, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "Expected redirect location '$($Check.RedirectLocation)' for $($Check.Path) but received '$location'."
        }

        return [ordered]@{
            path = $Check.Path
            status = $statusCode
            location = $location
            contentType = $contentType
        }
    }

    if ($statusCode -ne $Check.StatusCode) {
        throw "Expected HTTP $($Check.StatusCode) for $($Check.Path) but received HTTP $statusCode."
    }

    if ($Check.ContainsKey('ContentTypePrefix') -and -not [string]::IsNullOrWhiteSpace($Check.ContentTypePrefix)) {
        if ([string]::IsNullOrWhiteSpace($contentType) -or -not $contentType.StartsWith($Check.ContentTypePrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "Expected content type starting with '$($Check.ContentTypePrefix)' for $($Check.Path) but received '$contentType'."
        }
    }

    return [ordered]@{
        path = $Check.Path
        status = $statusCode
        location = $location
        contentType = $contentType
    }
}

$checks = @(
    @{ Path = '/'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/Fibonacci'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/Fibonacci/999.htm'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/Fibonacci/index.htm'; RedirectLocation = '/Fibonacci' }
    @{ Path = '/Prime'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/Prime/999.htm'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/Prime/index.htm'; RedirectLocation = '/Prime' }
    @{ Path = '/Prime/'; RedirectLocation = '/Prime' }
    @{ Path = '/UAI'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/UAI-1'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/UAI-1/examples'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/UAI-1/csharp-website-support'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/AI_Declaration_of_Independence.htm'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/Cognitive_Liberty_Charter.htm'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/downloads/Protocol5.UAI.CSharp.1.0.0.nupkg'; StatusCode = 200 }
    @{ Path = '/downloads/UAI-1-Package.zip'; StatusCode = 200 }
    @{ Path = '/calculator'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/converter'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/encryption'; StatusCode = 200; ContentTypePrefix = 'text/html' }
    @{ Path = '/schema/uai-1.schema.json'; StatusCode = 200; ContentTypePrefix = 'application/schema+json' }
    @{ Path = '/registry/uai-1.json'; StatusCode = 200; ContentTypePrefix = 'application/json' }
    @{ Path = '/registry/uai-1-examples.json'; StatusCode = 200; ContentTypePrefix = 'application/json' }
    @{ Path = '/registry/symbols.json'; StatusCode = 200; ContentTypePrefix = 'application/json' }
)

$client = New-HttpClient -TimeoutSeconds $TimeoutSeconds
$baseUri = [Uri]::new($BaseUrl)
$results = New-Object System.Collections.Generic.List[object]
$failures = New-Object System.Collections.Generic.List[string]

try {
    foreach ($check in $checks) {
        try {
            $result = Test-Protocol5Path -Client $client -BaseUri $baseUri -Check $check
            $results.Add($result) | Out-Null
            if ([string]::IsNullOrWhiteSpace([string]$result.location)) {
                Write-Host "PASS $($result.path) [$($result.status)] $($result.contentType)"
            }
            else {
                Write-Host "PASS $($result.path) [$($result.status)] -> $($result.location)"
            }
        }
        catch {
            $failures.Add($_.Exception.Message) | Out-Null
            Write-Host "FAIL $($check.Path) $($_.Exception.Message)"
        }
    }
}
finally {
    $client.Dispose()
}

if ($failures.Count -gt 0) {
    Write-Error ("Protocol5 smoke test failed:`n - " + ($failures -join "`n - "))
    exit 1
}

Write-Host "Protocol5 smoke test passed for $($results.Count) paths against $BaseUrl"