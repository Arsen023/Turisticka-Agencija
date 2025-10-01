(function () {
    
    var el = document.getElementById('datumRodjenja');
    if (!el) return;

  
    function normalizeDelimiters(v) {
        return (v || '').trim().replace(/[.\-]/g, '/');
    }

 
    el.addEventListener('blur', function () {
        var v = normalizeDelimiters(el.value);
        if (!v) return;

        var m = v.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
        if (m) {
            var d = ('0' + m[1]).slice(-2);
            var mo = ('0' + m[2]).slice(-2);
            el.value = d + '/' + mo + '/' + m[3];
        }
    });

   
    el.addEventListener('input', function () {
        el.value = el.value.replace(/[^\d/.\-]/g, '');
    });
})();
