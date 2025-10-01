(function () {
    'use strict';

    
    var forms = document.querySelectorAll('.needs-validation');
    Array.prototype.slice.call(forms).forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (!form.checkValidity()) {
                e.preventDefault(); e.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });

    
    var root = document.getElementById('smestajForm');
    var tip = (root && root.getAttribute('data-tip') || '').toLowerCase();
    var isHotel = (tip === 'hotel');

    var starsWrap = document.getElementById('zvezdiceWrap');
    var starsInput = document.getElementById('BrojZvezdica'); 
    var starsOut = document.getElementById('starsPreview');

    if (starsWrap) { starsWrap.style.display = isHotel ? '' : 'none'; }

    function drawStars() {
        if (!starsInput || !starsOut) return;
        var v = parseInt(starsInput.value || '0', 10);
        if (isNaN(v)) v = 0;
        v = Math.max(0, Math.min(5, v));
        starsOut.innerHTML = '★'.repeat(v) + '☆'.repeat(5 - v);
    }
    if (isHotel && starsInput) {
        starsInput.addEventListener('input', drawStars);
        drawStars();
    }

    
    var chips = document.getElementById('amenityChips');
    function refreshChips() {
        if (!chips) return;
        chips.innerHTML = '';
        var items = [];
        var w = document.getElementById('wifi');
        var b = document.getElementById('bazen');
        var s = document.getElementById('spa');
        var a = document.getElementById('access');
        if (w && w.checked) items.push('Wi-Fi');
        if (b && b.checked) items.push('Bazen');
        if (s && s.checked) items.push('Spa');
        if (a && a.checked) items.push('Prilagođeno');

        if (!items.length) {
            chips.innerHTML = '<span class="help">Nema izabranih pogodnosti.</span>';
            return;
        }
        items.forEach(function (t) {
            var c = document.createElement('span');
            c.className = 'chip';
            c.textContent = t;
            chips.appendChild(c);
        });
    }

    ['wifi', 'bazen', 'spa', 'access'].forEach(function (id) {
        var el = document.getElementById(id);
        if (el) el.addEventListener('change', refreshChips);
    });
    refreshChips();
})();
