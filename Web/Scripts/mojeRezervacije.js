
(function () {
    function onToggleClick(e) {
        const btn = e.currentTarget;
        const sel = btn.getAttribute("data-target");
        if (!sel) return;

        const panel = document.querySelector(sel);
        if (!panel) return;

        const isHidden = panel.style.display === "none";
        panel.style.display = isHidden ? "" : "none";
    }

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll(".js-toggle-jedinice").forEach(function (btn) {
            btn.addEventListener("click", onToggleClick);
        });
    });
})();
