(function () {
    'use strict';

    
    var forms = document.querySelectorAll('.needs-validation');
    Array.prototype.slice.call(forms).forEach(function (f) {
        f.addEventListener('submit', function (e) {
            if (!f.checkValidity()) { e.preventDefault(); e.stopPropagation(); }
            f.classList.add('was-validated');
        }, false);
    });
})();
