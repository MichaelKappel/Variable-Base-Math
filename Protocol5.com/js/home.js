(function () {
    var toolRoutes = {
        calculator: "/calculator",
        converter: "/converter",
        encryption: "/encryption"
    };

    var toolLinks = document.querySelectorAll("[data-tool-link]");
    for (var i = 0; i < toolLinks.length; i++) {
        var link = toolLinks[i];
        var key = link.getAttribute("data-tool-link");
        var route = toolRoutes[key || ""];
        if (route) {
            link.setAttribute("href", route);
        }
    }

    var year = document.getElementById("currentYear");
    if (year) {
        year.textContent = new Date().getFullYear().toString();
    }

    var overlay = document.querySelector("[data-boot-overlay]");
    var overlayTimer = 0;
    var stopBootLoader = overlay && typeof window.Protocol5BootLoader !== "undefined"
        ? window.Protocol5BootLoader.start(overlay)
        : function () { };

    function dismissOverlay() {
        if (!overlay || overlay.classList.contains("is-fading")) {
            return;
        }

        overlay.classList.add("is-fading");
        window.clearTimeout(overlayTimer);
        overlayTimer = window.setTimeout(function () {
            stopBootLoader();
            overlay.hidden = true;
        }, 5000);
    }

    if (overlay) {
        overlay.hidden = false;
        window.requestAnimationFrame(function () {
            window.requestAnimationFrame(dismissOverlay);
        });
    }

    function isRadix63404Digit(codePoint) {
        if (codePoint <= 0x1F || codePoint === 0x7F || (codePoint >= 0x80 && codePoint <= 0x9F)) {
            return false;
        }

        if (codePoint >= 0xD800 && codePoint <= 0xDFFF) {
            return false;
        }

        return !/\s/u.test(String.fromCharCode(codePoint));
    }

    function buildRadix63404Key() {
        var digits = [];
        for (var codePoint = 0; codePoint <= 0xFFFF; codePoint++) {
            if (isRadix63404Digit(codePoint)) {
                digits.push(String.fromCharCode(codePoint));
            }
        }

        return digits;
    }

    function toRadixString(value, key) {
        var radix = BigInt(key.length);
        if (value === 0n) {
            return key[0];
        }

        var remaining = value;
        var chars = [];
        while (remaining > 0n) {
            var remainder = remaining % radix;
            chars.push(key[Number(remainder)]);
            remaining = remaining / radix;
        }

        chars.reverse();
        return chars.join("");
    }

    function buildGlyphRow(key, start) {
        var parts = [];
        for (var index = 0; index < 42; index++) {
            parts.push(key[(start + (index * 137)) % key.length]);
        }

        return parts.join("");
    }

    function initializeRadixStage() {
        var glyphContainer = document.querySelector("[data-radix-glyphs]");
        var radixLines = document.querySelectorAll("[data-radix-line]");
        var decimalLines = document.querySelectorAll("[data-decimal-line]");

        if (!glyphContainer || !radixLines.length || !decimalLines.length) {
            return;
        }

        var radixKey = buildRadix63404Key();
        var samples = [
            { label: "Addition", symbol: "+", a: 541n, b: 29n },
            { label: "Multiplication", symbol: "×", a: 7919n, b: 34n },
            { label: "Prime catalog", symbol: "+", a: 1299709n, b: 104729n },
            { label: "Fibonacci page", symbol: "+", a: 354224848179261915075n, b: 1299709n },
            { label: "Dense multiply", symbol: "×", a: 104729n, b: 144n },
            { label: "Prime stride", symbol: "+", a: 999983n, b: 7919n }
        ];

        function renderGlyphs(seed) {
            glyphContainer.innerHTML = "";
            for (var row = 0; row < 7; row++) {
                var line = document.createElement("span");
                line.textContent = buildGlyphRow(radixKey, seed + (row * 911));
                glyphContainer.appendChild(line);
            }
        }

        function renderOperations(offset) {
            for (var index = 0; index < radixLines.length; index++) {
                var sample = samples[(offset + index) % samples.length];
                var result = sample.symbol === "+" ? (sample.a + sample.b) : (sample.a * sample.b);
                radixLines[index].textContent = toRadixString(sample.a, radixKey) + " " + sample.symbol + " " + toRadixString(sample.b, radixKey) + " = " + toRadixString(result, radixKey);
                decimalLines[index].textContent = sample.label + ": " + sample.a.toString() + " " + sample.symbol + " " + sample.b.toString() + " = " + result.toString();
            }
        }

        var frame = 0;
        renderGlyphs(frame * 283);
        renderOperations(frame);

        window.setInterval(function () {
            frame++;
            renderGlyphs(frame * 283);
            renderOperations(frame);
        }, 1800);
    }

    initializeRadixStage();

    window.requestAnimationFrame(function () {
        var items = document.querySelectorAll(".reveal");
        for (var j = 0; j < items.length; j++) {
            (function (element, index) {
                window.setTimeout(function () {
                    element.classList.add("is-visible");
                }, index * 90);
            })(items[j], j);
        }
    });
})();

