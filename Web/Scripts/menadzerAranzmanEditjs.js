
document.addEventListener("DOMContentLoaded", function () {

   
    (function () {
        var forms = document.querySelectorAll(".needs-validation");
        Array.prototype.slice.call(forms).forEach(function (form) {
            form.addEventListener("submit", function (e) {
                if (!form.checkValidity()) { e.preventDefault(); e.stopPropagation(); }
                form.classList.add("was-validated");
            }, false);
        });
    })();

  
    var APP_BASE = (window.APP_BASE || "/");
    if (APP_BASE.slice(-1) !== "/") APP_BASE += "/";

    
    (function () {
        var input = document.getElementById("PosterAranzmana");
        var img = document.getElementById("posterPreview");
        if (!input || !img) return;

        function toSrc(v) {
            v = (v || "").trim();
            if (!v) return "";
            if (/^https?:\/\//i.test(v)) return v;  
            if (v.indexOf("~/") === 0) return v.replace("~", APP_BASE);
            if (v.indexOf("/") !== -1) return APP_BASE.replace(/\/$/, "") + v;
            return APP_BASE + "Content/Images/" + v;   
        }

        function applySrc(raw) {
            var url = toSrc(raw);
            if (!url) {
                img.style.opacity = 0;
                img.removeAttribute("src");
                return;
            }
            var finalSrc = url + (url.indexOf("?") === -1 ? "?_=" + Date.now() : "");
            img.onload = function () { img.style.opacity = 1; };
            img.onerror = function () { img.style.opacity = 0; };
            img.src = finalSrc;

         
            if (img.complete && img.naturalWidth > 0) { img.style.opacity = 1; }
        }

     
        applySrc(input.value);
        input.addEventListener("input", function () { applySrc(input.value); });
        input.addEventListener("change", function () { applySrc(input.value); });
    })();

  
    (function () {
        var search = document.getElementById("smSearch");
        var list = document.getElementById("smList");
        var count = document.getElementById("smCount");
        var btnAll = document.getElementById("smSelectAll");
        var btnClr = document.getElementById("smClear");
        if (!list) return;

        function refreshCount() {
            var total = list.options.length;
            var selected = Array.prototype.filter.call(list.options, function (o) { return o.selected; }).length;
            if (count) count.textContent = selected + " / " + total + " izabrano";
        }

        function filter() {
            var q = (search && search.value || "").toLowerCase();
            Array.prototype.forEach.call(list.options, function (o) {
                o.style.display = o.text.toLowerCase().indexOf(q) >= 0 ? "" : "none";
            });
        }

        list.addEventListener("change", refreshCount);
        search && search.addEventListener("input", filter);

        btnAll && btnAll.addEventListener("click", function (e) {
            e.preventDefault();
            Array.prototype.forEach.call(list.options, function (o) {
                if (o.style.display !== "none") o.selected = true;
            });
            refreshCount();
        });

        btnClr && btnClr.addEventListener("click", function (e) {
            e.preventDefault();
            Array.prototype.forEach.call(list.options, function (o) { o.selected = false; });
            refreshCount();
        });

        refreshCount();
    })();
});
