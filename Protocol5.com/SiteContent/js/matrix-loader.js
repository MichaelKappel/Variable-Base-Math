(function () {
    "use strict";

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
                    decimal: digitIndex.toString()
                });
            }

            digitIndex++;
        }

        if (!pool.length) {
            for (var index = 0; index < 128; index++) {
                pool.push({
                    glyph: (index % 10).toString(),
                    decimal: index.toString()
                });
            }
        }

        return pool;
    }

    var displayPool = null;

    function getDisplayPool() {
        if (displayPool === null) {
            displayPool = buildDisplayPool();
        }

        return displayPool;
    }

    function randomIndex(length) {
        return Math.floor(Math.random() * length);
    }

    function pickRandom(pool) {
        return pool[randomIndex(pool.length)];
    }

    function createCell(pool) {
        return {
            sample: pickRandom(pool),
            mode: "glyph"
        };
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

    function seedStream(stream, layout, pool, initial) {
        stream.history.length = 0;
        for (var index = 0; index < stream.trailLength; index++) {
            stream.history.push(createCell(pool));
        }

        stream.speed = layout.rowHeight * (1.85 + (Math.random() * 2.2));
        stream.distanceSinceShift = layout.rowHeight;
        stream.y = initial
            ? (Math.random() * (layout.height + (stream.trailLength * layout.rowHeight))) - (stream.trailLength * layout.rowHeight)
            : -((Math.random() * layout.height) + (stream.trailLength * layout.rowHeight));
    }

    function createStreams(layout, pool) {
        var streams = [];
        var columnCount = Math.max(layout.reducedMotion ? 8 : 12, Math.ceil(layout.width / layout.columnWidth));
        var leftInset = (layout.width - (columnCount * layout.columnWidth)) / 2;

        for (var index = 0; index < columnCount; index++) {
            var stream = {
                x: leftInset + (index * layout.columnWidth) + (layout.columnWidth / 2),
                y: 0,
                speed: 0,
                trailLength: randomIndex(layout.reducedMotion ? 6 : 10) + (layout.reducedMotion ? 8 : 11),
                distanceSinceShift: 0,
                history: []
            };

            seedStream(stream, layout, pool, true);
            streams.push(stream);
        }

        return streams;
    }

    function mutateCell(cell, pool) {
        if (Math.random() < 0.02) {
            cell.sample = pickRandom(pool);
            cell.mode = "glyph";
            return;
        }

        if (cell.mode === "glyph" && Math.random() < 0.08) {
            cell.mode = "decimal";
            return;
        }

        if (cell.mode === "decimal" && Math.random() < 0.22) {
            cell.mode = "glyph";
        }
    }

    function buildLayout(canvas, context, reducedMotion) {
        var size = resizeCanvas(canvas, context);
        var fontSize = reducedMotion
            ? Math.max(12, Math.min(16, size.width / 32))
            : Math.max(14, Math.min(22, size.width / 70));
        var rowHeight = Math.round(fontSize * 1.35);
        var columnWidth = Math.max(rowHeight * 3.25, 58);

        return {
            width: size.width,
            height: size.height,
            fontSize: fontSize,
            rowHeight: rowHeight,
            columnWidth: columnWidth,
            reducedMotion: reducedMotion,
            streams: []
        };
    }

    function drawFrame(context, layout, pool, deltaSeconds) {
        context.fillStyle = layout.reducedMotion ? "rgba(0, 0, 0, 0.34)" : "rgba(0, 4, 0, 0.18)";
        context.fillRect(0, 0, layout.width, layout.height);
        context.font = "600 " + layout.fontSize + "px \"Cascadia Mono\", Consolas, \"Lucida Console\", monospace";
        context.textAlign = "center";
        context.textBaseline = "top";

        for (var streamIndex = 0; streamIndex < layout.streams.length; streamIndex++) {
            var stream = layout.streams[streamIndex];
            stream.y += stream.speed * deltaSeconds;
            stream.distanceSinceShift += stream.speed * deltaSeconds;

            while (stream.distanceSinceShift >= layout.rowHeight) {
                stream.distanceSinceShift -= layout.rowHeight;
                stream.history.unshift(createCell(pool));

                if (stream.history.length > stream.trailLength) {
                    stream.history.pop();
                }
            }

            if ((stream.y - (stream.trailLength * layout.rowHeight)) > (layout.height + layout.rowHeight) && Math.random() > 0.84) {
                seedStream(stream, layout, pool, false);
            }

            for (var cellIndex = stream.history.length - 1; cellIndex >= 0; cellIndex--) {
                var cell = stream.history[cellIndex];
                mutateCell(cell, pool);

                var y = stream.y - (cellIndex * layout.rowHeight);
                if (y < -layout.rowHeight || y > layout.height + layout.rowHeight) {
                    continue;
                }

                var intensity = 1 - (cellIndex / Math.max(1, stream.trailLength));
                var alpha = 0.14 + (intensity * 0.68);
                var isHead = cellIndex === 0;
                var isDecimal = cell.mode === "decimal";

                context.shadowBlur = isHead ? 18 : Math.max(2, 10 * intensity);
                context.shadowColor = isHead ? "rgba(220, 255, 220, 0.92)" : "rgba(46, 255, 120, 0.7)";
                context.fillStyle = isHead
                    ? "rgba(220, 255, 220, 0.96)"
                    : (isDecimal
                        ? "rgba(126, 255, 176, " + alpha.toFixed(3) + ")"
                        : "rgba(38, 255, 112, " + alpha.toFixed(3) + ")");

                context.fillText(isDecimal ? cell.sample.decimal : cell.sample.glyph, stream.x, y);
            }
        }

        context.shadowBlur = 0;
    }

    function start(root) {
        if (!root) {
            return function () { };
        }

        var canvas = root.querySelector("[data-matrix-canvas]");
        if (!canvas || typeof canvas.getContext !== "function") {
            return function () { };
        }

        var context = canvas.getContext("2d");
        if (!context) {
            return function () { };
        }

        var pool = getDisplayPool();
        var reducedMotion = window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
        var layout = buildLayout(canvas, context, reducedMotion);
        layout.streams = createStreams(layout, pool);

        var animationHandle = 0;
        var resizeHandle = 0;
        var disposed = false;
        var lastFrame = 0;

        context.fillStyle = "#000000";
        context.fillRect(0, 0, layout.width, layout.height);

        function reflow() {
            layout = buildLayout(canvas, context, reducedMotion);
            layout.streams = createStreams(layout, pool);
            context.fillStyle = "#000000";
            context.fillRect(0, 0, layout.width, layout.height);
        }

        function onResize() {
            window.clearTimeout(resizeHandle);
            resizeHandle = window.setTimeout(reflow, 120);
        }

        function frame(now) {
            if (disposed) {
                return;
            }

            if (!lastFrame) {
                lastFrame = now;
            }

            var deltaSeconds = Math.min(0.045, (now - lastFrame) / 1000);
            lastFrame = now;
            drawFrame(context, layout, pool, deltaSeconds || 0.016);
            animationHandle = window.requestAnimationFrame(frame);
        }

        window.addEventListener("resize", onResize, { passive: true });
        animationHandle = window.requestAnimationFrame(frame);

        return function () {
            if (disposed) {
                return;
            }

            disposed = true;
            window.cancelAnimationFrame(animationHandle);
            window.clearTimeout(resizeHandle);
            window.removeEventListener("resize", onResize);
        };
    }

    window.Protocol5BootLoader = {
        start: start
    };
})();
