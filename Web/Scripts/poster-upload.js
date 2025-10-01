
document.addEventListener("DOMContentLoaded", function () {
    var btn = document.getElementById("btnAddPoster") || document.getElementById("btnChangePoster");
    var file = document.getElementById("PosterFile");
    var txt = document.getElementById("posterInput") || document.getElementById("PosterAranzmana");
    var img = document.getElementById("posterPreview");

    if (!btn || !file) return;

    btn.addEventListener("click", function () { file.click(); });

    file.addEventListener("change", function () {
        var f = file.files && file.files[0];
        if (!f) return;

        
        if (txt) {
            txt.value = f.name;
            
            txt.dispatchEvent(new Event("input", { bubbles: true }));
            txt.dispatchEvent(new Event("change", { bubbles: true }));
        }

        
        if (img && f.type && f.type.startsWith("image/")) {
            var url = URL.createObjectURL(f);
            img.onload = function () { URL.revokeObjectURL(url); img.style.display = "block"; };
            img.onerror = function () { img.style.display = "none"; };
            img.src = url;
            img.style.display = "block";
        }
    });
});
