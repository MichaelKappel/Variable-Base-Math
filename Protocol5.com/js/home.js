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
