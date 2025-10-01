
(function () {
    function toDDMMYYYY(iso) {
        if (!iso) return "";
        var p = iso.split("-");
        if (p.length !== 3) return "";
        return p[2].padStart(2, "0") + "/" + p[1].padStart(2, "0") + "/" + p[0];
    }
    function toISO(ddmmyyyy) {
        if (!ddmmyyyy) return "";
        var p = ddmmyyyy.split("/");
        if (p.length !== 3) return "";
        return p[2] + "-" + p[1].padStart(2, "0") + "-" + p[0].padStart(2, "0");
    }

    function wire(dpId, hiddenId) {
        var dp = document.getElementById(dpId);
        var hid = document.getElementById(hiddenId);
        if (!dp || !hid) return;

      
        if (!dp.value && hid.value) dp.value = toISO(hid.value);

        var sync = function () { hid.value = toDDMMYYYY(dp.value); };

        
        if (dp.value) sync();
        dp.addEventListener("input", sync);
        dp.addEventListener("change", sync);

        
        var form = dp.form || (hid && hid.form) || document.querySelector("form.needs-validation");
        if (form && !form.__ddmmyyyyHooked) {
            form.addEventListener("submit", function () {
                var s = document.getElementById("dpStart");
                var e = document.getElementById("dpEnd");
                var b = document.getElementById("dpBirth");
                var hs = document.getElementById("DatumPocetkaPutovanja");
                var he = document.getElementById("DatumZavrsetkaPutovanja");
                var hb = document.getElementById("DatumRodjenja");
                if (s && hs) hs.value = toDDMMYYYY(s.value);
                if (e && he) he.value = toDDMMYYYY(e.value);
                if (b && hb) hb.value = toDDMMYYYY(b.value);
            }, true);
            form.__ddmmyyyyHooked = true;
        }
    }

    function init() {
        wire("dpStart", "DatumPocetkaPutovanja"); 
        wire("dpEnd", "DatumZavrsetkaPutovanja"); 
        wire("dpBirth", "DatumRodjenja"); 
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
