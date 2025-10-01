
(function () {
    'use strict';
    var forms = document.querySelectorAll('.needs-validation');
    Array.prototype.slice.call(forms).forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (!form.checkValidity()) { e.preventDefault(); e.stopPropagation(); }
            form.classList.add('was-validated');
        }, false);
    });
})();


document.querySelectorAll('.date-ddmmyyyy').forEach(function (inp) {
    inp.addEventListener('input', function () {
        var v = inp.value.replace(/\D/g, '').slice(0, 8);
        var out = '';
        if (v.length >= 2) out = v.slice(0, 2) + '/';
        else out = v;
        if (v.length >= 4) out += v.slice(2, 4) + '/';
        else if (v.length > 2) out += v.slice(2);
        if (v.length > 4) out += v.slice(4);
        inp.value = out;
    });
});

(function () {
    var input = document.getElementById('posterInput');
    var img = document.getElementById('posterPreview');
    if (!input || !img) return;

    function refreshPoster() {
        var name = (input.value || '').trim();
        if (!name) { img.style.display = 'none'; img.removeAttribute('src'); return; }
        var src = '/Content/Images/' + name;
        img.onload = function () { img.style.display = 'block'; };
        img.onerror = function () { img.style.display = 'none'; };
        img.src = src;
    }
    input.addEventListener('change', refreshPoster);
    input.addEventListener('keyup', refreshPoster);
    refreshPoster();
})();


(function () {
    var counter = document.getElementById('countSel');
    if (!counter) return;
    function updateCount() {
        var n = document.querySelectorAll('.smestaj-check:checked').length;
        counter.textContent = n ? ('Izabrano: ' + n) : '';
    }
    document.querySelectorAll('.smestaj-check').forEach(function (ch) {
        ch.addEventListener('change', updateCount);
    });
    updateCount();
})();
