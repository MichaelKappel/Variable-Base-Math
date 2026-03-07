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
    var dismissButton = document.querySelector("[data-overlay-dismiss]");
    var overlayTimer = 0;

    function dismissOverlay() {
        if (!overlay || overlay.classList.contains("is-dismissed")) {
            return;
        }

        overlay.classList.add("is-dismissed");
        document.body.classList.remove("overlay-active");
        window.clearTimeout(overlayTimer);
        window.setTimeout(function () {
            overlay.hidden = true;
        }, 430);
    }

    if (overlay) {
        overlay.hidden = false;
        document.body.classList.add("overlay-active");
        overlayTimer = window.setTimeout(dismissOverlay, 5000);

        if (dismissButton) {
            dismissButton.addEventListener("click", dismissOverlay);
        }

        overlay.addEventListener("click", function (event) {
            if (event.target === overlay) {
                dismissOverlay();
            }
        });
    }

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
