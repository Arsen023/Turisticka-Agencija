(function () {
    'use strict';

    
    var forms = document.querySelectorAll('.needs-validation');
    Array.prototype.slice.call(forms).forEach(function (f) {
        f.addEventListener('submit', function (e) {
            if (!f.checkValidity()) { e.preventDefault(); e.stopPropagation(); }
            f.classList.add('was-validated');
        }, false);
    });

    
    forms.forEach(function (f) {
        f.addEventListener('submit', function () {
            var fields = f.querySelectorAll('input[type="text"], input[type="email"], input[type="password"]');
            fields.forEach(function (el) { if (el.value) el.value = el.value.trim(); });
        });
    });
})();
