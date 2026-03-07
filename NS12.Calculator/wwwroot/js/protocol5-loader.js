(function () {
    "use strict";

    var baseDigits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    var supportedBases = [2, 8, 10, 16, 36];
    var orbitBaseColors = {
        "2": "#82efe0",
        "8": "#8ce0ff",
        "10": "#f3fbff",
        "16": "#ffd191",
        "36": "#9fb6ff",
        "63404": "#8ef5c1"
    };

    function escapeHtml(value) {
        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function randomInt(min, max) {
        return Math.floor(Math.random() * ((max - min) + 1)) + min;
    }

    function pickRandom(items) {
        return items[randomInt(0, items.length - 1)];
    }

    function shuffle(items) {
        var copy = items.slice();
        for (var index = copy.length - 1; index > 0; index--) {
            var swapIndex = randomInt(0, index);
            var temp = copy[index];
            copy[index] = copy[swapIndex];
            copy[swapIndex] = temp;
        }

        return copy;
    }

    function clamp(value, min, max) {
        return Math.max(min, Math.min(max, value));
    }

    function toBaseString(value, base) {
        if (base < 2 || base > 36) {
            return String(value);
        }

        var numericValue = Math.floor(Math.abs(value));
        if (numericValue === 0) {
            return "0";
        }

        var digits = "";
        while (numericValue > 0) {
            digits = baseDigits.charAt(numericValue % base) + digits;
            numericValue = Math.floor(numericValue / base);
        }

        return value < 0 ? "-" + digits : digits;
    }

    function formatBaseMarkup(value, base) {
        return escapeHtml(toBaseString(value, base)) + "<sub>" + base + "</sub>";
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

    function supportsUnicodePropertyEscapes() {
        try {
            return new RegExp("\\p{Letter}", "u").test("A");
        }
        catch (error) {
            return false;
        }
    }

    var hasUnicodeProperties = supportsUnicodePropertyEscapes();
    var visibleGlyphExpression = hasUnicodeProperties
        ? /[\p{Letter}\p{Number}\p{Punctuation}\p{Symbol}]/u
        : /[!-~\u00A1-\u024F\u0370-\u052F\u1E00-\u2C7F\u2E00-\u33FF\u4E00-\u9FFF\uAC00-\uD7A3]/;
    var excludedGlyphExpression = hasUnicodeProperties
        ? /[\p{Mark}\p{Separator}\p{Control}\p{Surrogate}\p{Format}\p{Private_Use}\p{Unassigned}]/u
        : /[\u0000-\u001F\u007F-\u00A0]/;

    function isDisplaySafeGlyph(glyph) {
        return visibleGlyphExpression.test(glyph) && !excludedGlyphExpression.test(glyph);
    }

    var displayPool = null;

    function buildDisplayPool() {
        var pool = [];
        var digitIndex = 0;

        for (var codePoint = 0; codePoint <= 0xFFFF; codePoint++) {
            if (!isRadix63404Digit(codePoint)) {
                continue;
            }

            var glyph = String.fromCharCode(codePoint);
            if (isDisplaySafeGlyph(glyph)) {
                pool.push({
                    glyph: glyph,
                    decimal: digitIndex,
                    hex: codePoint.toString(16).toUpperCase()
                });
            }

            digitIndex++;
        }

        if (!pool.length) {
            for (var index = 0; index < 256; index++) {
                pool.push({
                    glyph: (index % 10).toString(),
                    decimal: index,
                    hex: index.toString(16).toUpperCase()
                });
            }
        }

        return pool;
    }

    function getDisplayPool() {
        if (displayPool === null) {
            displayPool = buildDisplayPool();
        }

        return displayPool;
    }

    function sizeForBase(base) {
        switch (base) {
            case 2:
                return { min: 6, max: 63 };
            case 8:
                return { min: 24, max: 511 };
            case 10:
                return { min: 40, max: 999 };
            case 16:
                return { min: 64, max: 2047 };
            case 36:
                return { min: 100, max: 8191 };
            default:
                return { min: 20, max: 255 };
        }
    }

    function createArithmeticSample() {
        var operator = pickRandom([
            { symbol: "+", compute: function (left, right) { return left + right; } },
            { symbol: "-", compute: function (left, right) { return left - right; } },
            { symbol: "x", compute: function (left, right) { return left * right; } }
        ]);
        var base = pickRandom(supportedBases);
        var limits = sizeForBase(base);
        var left = randomInt(limits.min, limits.max);
        var right = randomInt(2, Math.max(6, Math.floor(limits.max / (operator.symbol === "x" ? 8 : 2))));

        if (operator.symbol === "-" && right > left) {
            var swap = left;
            left = right;
            right = swap;
        }

        if (operator.symbol === "x") {
            left = randomInt(8, Math.max(32, Math.floor(limits.max / 2)));
            right = randomInt(2, Math.max(4, Math.floor(Math.max(16, limits.max / left))));
        }

        var result = operator.compute(left, right);

        return {
            primary: operator.symbol,
            secondary: String(base),
            lineA: formatBaseMarkup(left, base) + " " + operator.symbol + " " + formatBaseMarkup(right, base) + " = " + formatBaseMarkup(result, base),
            lineB: formatBaseMarkup(left, 10) + " " + operator.symbol + " " + formatBaseMarkup(right, 10) + " = " + formatBaseMarkup(result, 10),
            routeFrom: String(base),
            routeTo: "10",
            color: orbitBaseColors[String(base)] || "#82efe0",
            emphasisValue: toBaseString(result, base),
            emphasisMeta: formatBaseMarkup(result, 10) + " -> " + formatBaseMarkup(result, base)
        };
    }

    function createConvertSample() {
        var bases = shuffle(supportedBases).slice(0, 3);
        var maxValue = bases.indexOf(2) >= 0 ? 255 : 4095;
        var value = randomInt(18, maxValue);

        return {
            primary: "<->",
            secondary: bases.join("."),
            lineA: formatBaseMarkup(value, bases[0]) + " -> " + formatBaseMarkup(value, bases[1]),
            lineB: formatBaseMarkup(value, bases[1]) + " -> " + formatBaseMarkup(value, bases[2]),
            routeFrom: String(bases[0]),
            routeTo: String(bases[2]),
            color: orbitBaseColors[String(bases[2])] || "#8ce0ff",
            emphasisValue: toBaseString(value, bases[1]),
            emphasisMeta: formatBaseMarkup(value, 10) + " -> " + formatBaseMarkup(value, bases[1])
        };
    }

    function createDivisionSample() {
        var base = pickRandom([8, 10, 16, 36]);
        var dividend = randomInt(base * 2, base * 80);
        var divisor = randomInt(2, Math.max(3, Math.floor(base * 0.75)));
        var quotient = Math.floor(dividend / divisor);
        var remainder = dividend % divisor;

        return {
            primary: "/",
            secondary: String(base),
            lineA: formatBaseMarkup(dividend, base) + " / " + formatBaseMarkup(divisor, base),
            lineB: formatBaseMarkup(quotient, base) + " r " + formatBaseMarkup(remainder, base),
            routeFrom: String(base),
            routeTo: "10",
            color: orbitBaseColors[String(base)] || "#ffd191",
            emphasisValue: toBaseString(quotient, base),
            emphasisMeta: formatBaseMarkup(dividend, 10) + " -> " + formatBaseMarkup(quotient, base) + " r " + formatBaseMarkup(remainder, base)
        };
    }

    function createGlyphSample(pool) {
        var entry = pickRandom(pool);
        var decimalValue = entry.decimal;

        return {
            primary: "63404",
            secondary: "10",
            lineA: escapeHtml(entry.glyph) + "<sub>63404</sub> -> " + formatBaseMarkup(decimalValue, 10),
            lineB: formatBaseMarkup(decimalValue, 16) + " . " + formatBaseMarkup(decimalValue, 36),
            routeFrom: "63404",
            routeTo: "10",
            color: orbitBaseColors["63404"],
            emphasisValue: entry.glyph,
            emphasisMeta: formatBaseMarkup(decimalValue, 10) + " . " + formatBaseMarkup(decimalValue, 16)
        };
    }

    function flash(element) {
        if (!element) {
            return;
        }

        window.clearTimeout(element.__bootFlashHandle);
        element.classList.add("is-live");
        element.__bootFlashHandle = window.setTimeout(function () {
            element.classList.remove("is-live");
        }, 220);
    }

    function setPanelContent(panel, sample) {
        if (!panel || !sample) {
            return;
        }

        var primary = panel.querySelector("[data-panel-primary]");
        var secondary = panel.querySelector("[data-panel-secondary]");
        var lineA = panel.querySelector("[data-panel-line-a]");
        var lineB = panel.querySelector("[data-panel-line-b]");

        if (primary) {
            primary.textContent = sample.primary;
        }

        if (secondary) {
            secondary.textContent = sample.secondary;
        }

        if (lineA) {
            lineA.innerHTML = sample.lineA;
        }

        if (lineB) {
            lineB.innerHTML = sample.lineB;
        }

        panel.setAttribute("data-route-from", sample.routeFrom);
        panel.setAttribute("data-route-to", sample.routeTo);
        flash(panel);
    }

    function setCoreContent(coreValue, coreMeta, coreRoot, sample) {
        if (!sample) {
            return;
        }

        if (coreValue) {
            coreValue.textContent = sample.emphasisValue;
        }

        if (coreMeta) {
            coreMeta.innerHTML = sample.emphasisMeta;
        }

        flash(coreRoot);
    }

    function resizeCanvas(canvas, context) {
        var rect = canvas.getBoundingClientRect();
        var devicePixelRatio = Math.max(1, window.devicePixelRatio || 1);

        canvas.width = Math.max(1, Math.round(rect.width * devicePixelRatio));
        canvas.height = Math.max(1, Math.round(rect.height * devicePixelRatio));
        context.setTransform(devicePixelRatio, 0, 0, devicePixelRatio, 0, 0);

        return {
            width: Math.max(1, rect.width),
            height: Math.max(1, rect.height)
        };
    }

    function pointFor(element, rootRect) {
        var rect = element.getBoundingClientRect();
        return {
            x: rect.left - rootRect.left + (rect.width / 2),
            y: rect.top - rootRect.top + (rect.height / 2)
        };
    }

    function createStars(width, height, reducedMotion) {
        var stars = [];
        var count = reducedMotion ? 18 : 34;

        for (var index = 0; index < count; index++) {
            stars.push({
                x: Math.random() * width,
                y: Math.random() * height,
                radius: 0.8 + (Math.random() * 1.7),
                driftX: ((Math.random() * 2) - 1) * (reducedMotion ? 2 : 8),
                driftY: ((Math.random() * 2) - 1) * (reducedMotion ? 2 : 8),
                phase: Math.random() * Math.PI * 2
            });
        }

        return stars;
    }

    function updateGeometry(state) {
        var size = resizeCanvas(state.canvas, state.context);
        var rootRect = state.root.getBoundingClientRect();
        var nodePositions = {};
        var index;
        var node;

        for (index = 0; index < state.nodeElements.length; index++) {
            node = state.nodeElements[index];
            nodePositions[node.getAttribute("data-radix-node")] = pointFor(node, rootRect);
        }

        state.layout = {
            width: size.width,
            height: size.height,
            center: pointFor(state.coreRoot, rootRect),
            nodes: nodePositions
        };
        state.stars = createStars(size.width, size.height, state.reducedMotion);
    }

    function withAlpha(hexColor, alpha) {
        var normalized = hexColor.replace("#", "");
        if (normalized.length === 3) {
            normalized = normalized.charAt(0) + normalized.charAt(0)
                + normalized.charAt(1) + normalized.charAt(1)
                + normalized.charAt(2) + normalized.charAt(2);
        }

        var red = parseInt(normalized.slice(0, 2), 16);
        var green = parseInt(normalized.slice(2, 4), 16);
        var blue = parseInt(normalized.slice(4, 6), 16);
        return "rgba(" + red + ", " + green + ", " + blue + ", " + alpha.toFixed(3) + ")";
    }

    function pointOnQuadratic(start, control, end, progress) {
        var inverse = 1 - progress;
        return {
            x: (inverse * inverse * start.x) + (2 * inverse * progress * control.x) + (progress * progress * end.x),
            y: (inverse * inverse * start.y) + (2 * inverse * progress * control.y) + (progress * progress * end.y)
        };
    }

    function interpolate(start, end, progress) {
        return {
            x: start.x + ((end.x - start.x) * progress),
            y: start.y + ((end.y - start.y) * progress)
        };
    }

    function drawPulse(context, point, radius, color, alpha) {
        context.save();
        context.fillStyle = withAlpha(color, alpha);
        context.shadowBlur = 18;
        context.shadowColor = withAlpha(color, Math.max(alpha, 0.24));
        context.beginPath();
        context.arc(point.x, point.y, radius, 0, Math.PI * 2);
        context.fill();
        context.restore();
    }

    function drawScene(state, now) {
        var context = state.context;
        var layout = state.layout;
        var center;
        var nodeKeys;
        var nodeIndex;
        var nodeId;
        var nodePoint;
        var nodeColor;
        var isActiveNode;
        var routeIndex;
        var route;
        var fromPoint;
        var toPoint;
        var control;
        var packetCount;
        var packetIndex;
        var progress;
        var packetPoint;
        var spokeProgress;
        var starIndex;
        var star;
        var starAlpha;
        var deltaSeconds;

        if (!layout) {
            return;
        }

        if (!state.lastFrame) {
            state.lastFrame = now;
        }

        deltaSeconds = Math.min(0.05, (now - state.lastFrame) / 1000) || 0.016;
        state.lastFrame = now;
        state.elapsed += deltaSeconds;

        context.clearRect(0, 0, layout.width, layout.height);
        context.fillStyle = state.reducedMotion ? "rgba(5, 18, 27, 0.18)" : "rgba(5, 18, 27, 0.12)";
        context.fillRect(0, 0, layout.width, layout.height);

        for (starIndex = 0; starIndex < state.stars.length; starIndex++) {
            star = state.stars[starIndex];
            star.x += star.driftX * deltaSeconds;
            star.y += star.driftY * deltaSeconds;

            if (star.x < -10) {
                star.x = layout.width + 10;
            }
            else if (star.x > layout.width + 10) {
                star.x = -10;
            }

            if (star.y < -10) {
                star.y = layout.height + 10;
            }
            else if (star.y > layout.height + 10) {
                star.y = -10;
            }

            starAlpha = 0.08 + (0.16 * (0.5 + (Math.sin(state.elapsed + star.phase) * 0.5)));
            drawPulse(context, star, star.radius, "#9fe9de", starAlpha);
        }

        center = layout.center;
        nodeKeys = Object.keys(layout.nodes);
        for (nodeIndex = 0; nodeIndex < nodeKeys.length; nodeIndex++) {
            nodeId = nodeKeys[nodeIndex];
            nodePoint = layout.nodes[nodeId];
            nodeColor = orbitBaseColors[nodeId] || "#82efe0";
            isActiveNode = state.activeNodeIds[nodeId] === true;

            context.beginPath();
            context.lineWidth = isActiveNode ? 1.8 : 1;
            context.strokeStyle = withAlpha(nodeColor, isActiveNode ? 0.28 : 0.12);
            context.moveTo(center.x, center.y);
            context.lineTo(nodePoint.x, nodePoint.y);
            context.stroke();

            spokeProgress = (state.elapsed * (state.reducedMotion ? 0.09 : 0.22) + (nodeIndex * 0.12)) % 1;
            drawPulse(context, interpolate(center, nodePoint, spokeProgress), isActiveNode ? 3.2 : 2.2, nodeColor, isActiveNode ? 0.56 : 0.26);
        }

        for (routeIndex = 0; routeIndex < state.activeRoutes.length; routeIndex++) {
            route = state.activeRoutes[routeIndex];
            fromPoint = layout.nodes[route.from];
            toPoint = layout.nodes[route.to];
            if (!fromPoint || !toPoint) {
                continue;
            }

            control = {
                x: center.x + ((routeIndex % 2 === 0 ? -1 : 1) * 16),
                y: center.y + (((routeIndex % 3) - 1) * 14)
            };

            context.beginPath();
            context.lineWidth = 1.6;
            context.strokeStyle = withAlpha(route.color, 0.34);
            context.moveTo(fromPoint.x, fromPoint.y);
            context.quadraticCurveTo(control.x, control.y, toPoint.x, toPoint.y);
            context.stroke();

            packetCount = state.reducedMotion ? 1 : 2;
            for (packetIndex = 0; packetIndex < packetCount; packetIndex++) {
                progress = (state.elapsed * route.speed + route.offset + (packetIndex * 0.36)) % 1;
                packetPoint = pointOnQuadratic(fromPoint, control, toPoint, progress);
                drawPulse(context, packetPoint, packetIndex === 0 ? 3.6 : 2.4, route.color, packetIndex === 0 ? 0.72 : 0.38);
            }
        }

        context.save();
        context.beginPath();
        context.lineWidth = 1;
        context.strokeStyle = withAlpha("#9fe9de", 0.16);
        context.arc(center.x, center.y, clamp(Math.min(layout.width, layout.height) * 0.17, 80, 170), 0, Math.PI * 2);
        context.stroke();
        context.restore();
    }

    function setNodeActivity(state, samples) {
        var active = {};
        var sampleIndex;
        var nodeIndex;
        var node;
        var nodeId;

        for (sampleIndex = 0; sampleIndex < samples.length; sampleIndex++) {
            active[samples[sampleIndex].routeFrom] = true;
            active[samples[sampleIndex].routeTo] = true;
        }

        state.activeNodeIds = active;

        for (nodeIndex = 0; nodeIndex < state.nodeElements.length; nodeIndex++) {
            node = state.nodeElements[nodeIndex];
            nodeId = node.getAttribute("data-radix-node");
            node.classList.toggle("is-active", active[nodeId] === true);
        }
    }

    function refreshSamples(state) {
        var samples = [
            createConvertSample(),
            createArithmeticSample(),
            createDivisionSample(),
            createGlyphSample(state.displayPool)
        ];
        var panelIndex;
        var coreSample;

        for (panelIndex = 0; panelIndex < state.panels.length; panelIndex++) {
            setPanelContent(state.panels[panelIndex], samples[panelIndex]);
        }

        coreSample = pickRandom(samples);
        setCoreContent(state.coreValue, state.coreMeta, state.coreRoot, coreSample);
        setNodeActivity(state, samples);
        state.activeRoutes = samples.map(function (sample, index) {
            return {
                from: sample.routeFrom,
                to: sample.routeTo,
                color: sample.color,
                speed: 0.12 + (index * 0.018),
                offset: Math.random()
            };
        });
    }

    function start(root) {
        if (!root) {
            return function () { };
        }

        var canvas = root.querySelector("[data-boot-canvas]");
        var context;
        var state;

        if (!canvas || typeof canvas.getContext !== "function") {
            return function () { };
        }

        context = canvas.getContext("2d");
        if (!context) {
            return function () { };
        }

        state = {
            root: root,
            canvas: canvas,
            context: context,
            panels: [
                root.querySelector('[data-boot-panel="convert"]'),
                root.querySelector('[data-boot-panel="arithmetic"]'),
                root.querySelector('[data-boot-panel="division"]'),
                root.querySelector('[data-boot-panel="glyph"]')
            ],
            nodeElements: Array.prototype.slice.call(root.querySelectorAll("[data-radix-node]")),
            coreRoot: root.querySelector(".boot-loader__core"),
            coreValue: root.querySelector("[data-boot-core-value]"),
            coreMeta: root.querySelector("[data-boot-core-meta]"),
            reducedMotion: window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches,
            displayPool: getDisplayPool(),
            activeRoutes: [],
            activeNodeIds: {},
            stars: [],
            layout: null,
            lastFrame: 0,
            elapsed: 0,
            disposed: false,
            animationHandle: 0,
            resizeHandle: 0,
            refreshHandle: 0
        };

        function reflow() {
            updateGeometry(state);
            drawScene(state, performance.now());
        }

        function onResize() {
            window.clearTimeout(state.resizeHandle);
            state.resizeHandle = window.setTimeout(reflow, 120);
        }

        function frame(now) {
            if (state.disposed) {
                return;
            }

            drawScene(state, now);
            state.animationHandle = window.requestAnimationFrame(frame);
        }

        updateGeometry(state);
        refreshSamples(state);
        state.refreshHandle = window.setInterval(function () {
            refreshSamples(state);
        }, state.reducedMotion ? 1800 : 1200);

        window.addEventListener("resize", onResize, { passive: true });
        state.animationHandle = window.requestAnimationFrame(frame);

        return function () {
            if (state.disposed) {
                return;
            }

            state.disposed = true;
            window.cancelAnimationFrame(state.animationHandle);
            window.clearTimeout(state.resizeHandle);
            window.clearInterval(state.refreshHandle);
            window.removeEventListener("resize", onResize);
        };
    }

    window.Protocol5BootLoader = {
        start: start
    };
})();
