(function () {
    'use strict';

    
    var forms = document.querySelectorAll('.needs-validation');
    Array.prototype.slice.call(forms).forEach(function (f) {
        f.addEventListener('submit', function (e) {
            if (!f.checkValidity()) { e.preventDefault(); e.stopPropagation(); }
            f.classList.add('was-validated');
        }, false);
    });

    
    var tip = document.getElementById('tipSmestaja');
    var starsWrap = document.getElementById('zvezdiceWrap');
    var starsInput = document.getElementById('brojZvezdica');
    var starsPrev = document.getElementById('starsPreview');

    function drawStars() {
        if (!starsInput || !starsPrev) return;
        var v = parseInt(starsInput.value || '0', 10);
        if (isNaN(v)) v = 0;
        v = Math.max(0, Math.min(5, v));
        starsPrev.innerHTML = '★'.repeat(v) + '☆'.repeat(5 - v);
    }

    function toggleStars() {
        var isHotel = ((tip && tip.value) || '').toLowerCase() === 'hotel';
        if (!starsWrap) return;
        starsWrap.style.display = isHotel ? '' : 'none';
        if (isHotel) {
            if (starsInput) starsInput.setAttribute('required', 'required');
            drawStars();
        } else {
            if (starsInput) starsInput.removeAttribute('required');
            if (starsPrev) starsPrev.innerHTML = '';
            if (starsInput) starsInput.value = '';
        }
    }

    if (tip) tip.addEventListener('change', toggleStars);
    if (starsInput) starsInput.addEventListener('input', drawStars);
    toggleStars(); 

    
    var map = [
        { id: 'wifi', text: 'Wi-Fi' },
        { id: 'bazen', text: 'Bazen' },
        { id: 'spa', text: 'Spa centar' },
        { id: 'access', text: 'Prilagođeno' }
    ];
    var box = document.getElementById('amenityChips');

    function renderChips() {
        if (!box) return;
        box.innerHTML = '';
        map.forEach(function (m) {
            var el = document.getElementById(m.id);
            if (el && el.checked) {
                var chip = document.createElement('span');
                chip.className = 'chip';
                chip.textContent = m.text;
                box.appendChild(chip);
            }
        });
        if (!box.children.length) {
            var none = document.createElement('span');
            none.className = 'help';
            none.textContent = 'Nema izabranih pogodnosti.';
            box.appendChild(none);
        }
    }

    map.forEach(function (m) {
        var el = document.getElementById(m.id);
        if (el) el.addEventListener('change', renderChips);
    });
    renderChips();
})();
