
document.addEventListener("DOMContentLoaded", function () {
    const smestajiContainer = document.getElementById("smestajiContainer");
    const smestajCards = Array.from(document.querySelectorAll(".smestaj-card"));

  
    const filterNaziv = document.getElementById("filterNaziv");
    const filterTip = document.getElementById("filterTip");
    const filterBazen = document.getElementById("filterBazen");
    const filterSpa = document.getElementById("filterSpa");
    const filterWifi = document.getElementById("filterWifi");
    const filterInvaliditet = document.getElementById("filterInvaliditet");
    const sortSmestaji = document.getElementById("sortSmestaji");

    const minGostiju = document.getElementById("minGostiju");
    const maxGostiju = document.getElementById("maxGostiju");
    const filterLjubimci = document.getElementById("filterLjubimci");
    const maxCena = document.getElementById("maxCena");
    const sortJedinice = document.getElementById("sortJedinice");

    const applyBtn = document.getElementById("applyFilters");
    const resetBtn = document.getElementById("resetFilters");

    
    function applyFilters() {
        let filtered = [...smestajCards];

      
        filtered = filtered.filter(card => {
            const naziv = card.dataset.naziv.toLowerCase();
            const tip = card.dataset.tip.toLowerCase();
            const bazen = card.dataset.bazen;
            const spa = card.dataset.spa;
            const wifi = card.dataset.wifi;
            const invaliditet = card.dataset.invaliditet;

            let match = true;
            if (filterNaziv.value && !naziv.includes(filterNaziv.value.toLowerCase())) match = false;
            if (filterTip.value && filterTip.value !== tip) match = false;
            if (filterBazen.value && filterBazen.value !== bazen) match = false;
            if (filterSpa.value && filterSpa.value !== spa) match = false;
            if (filterWifi.value && filterWifi.value !== wifi) match = false;
            if (filterInvaliditet.value && filterInvaliditet.value !== invaliditet) match = false;

            return match;
        });

      
        filtered = filtered.filter(card => {
            const jedinice = Array.from(card.querySelectorAll(".jedinica-row"));
            const jediniceFiltered = jedinice.filter(row => {
                const gostiju = parseInt(row.dataset.gostiju);
                const ljubimci = row.dataset.ljubimci;
                const cena = parseFloat(row.dataset.cena);

                let ok = true;
                if (minGostiju.value && gostiju < parseInt(minGostiju.value)) ok = false;
                if (maxGostiju.value && gostiju > parseInt(maxGostiju.value)) ok = false;
                if (filterLjubimci.value && filterLjubimci.value !== ljubimci) ok = false;
                if (maxCena.value && cena > parseFloat(maxCena.value)) ok = false;

                return ok;
            });

            
            return jediniceFiltered.length > 0;
        });

        
        if (sortSmestaji.value) {
            filtered.sort((a, b) => {
                const nazivA = a.dataset.naziv.toLowerCase();
                const nazivB = b.dataset.naziv.toLowerCase();
                const jediniceA = parseInt(a.dataset.jedinice);
                const jediniceB = parseInt(b.dataset.jedinice);
                const slobodneA = parseInt(a.dataset.slobodne);
                const slobodneB = parseInt(b.dataset.slobodne);

                switch (sortSmestaji.value) {
                    case "nazivAsc": return nazivA.localeCompare(nazivB);
                    case "nazivDesc": return nazivB.localeCompare(nazivA);
                    case "jediniceAsc": return jediniceA - jediniceB;
                    case "jediniceDesc": return jediniceB - jediniceA;
                    case "slobodneAsc": return slobodneA - slobodneB;
                    case "slobodneDesc": return slobodneB - slobodneA;
                    default: return 0;
                }
            });
        }

      
        if (sortJedinice.value) {
            filtered.forEach(card => {
                const tbody = card.querySelector(".jedinice-table tbody");
                const rows = Array.from(tbody.querySelectorAll(".jedinica-row"));

                rows.sort((a, b) => {
                    const gostA = parseInt(a.dataset.gostiju);
                    const gostB = parseInt(b.dataset.gostiju);
                    const cenaA = parseFloat(a.dataset.cena);
                    const cenaB = parseFloat(b.dataset.cena);

                    switch (sortJedinice.value) {
                        case "gostAsc": return gostA - gostB;
                        case "gostDesc": return gostB - gostA;
                        case "cenaAsc": return cenaA - cenaB;
                        case "cenaDesc": return cenaB - cenaA;
                        default: return 0;
                    }
                });

                
                tbody.innerHTML = "";
                rows.forEach(r => tbody.appendChild(r));
            });
        }

       
        smestajiContainer.innerHTML = "";
        filtered.forEach(card => smestajiContainer.appendChild(card));
    }

   
    function resetFilters() {
        
        filterNaziv.value = "";
        filterTip.value = "";
        filterBazen.value = "";
        filterSpa.value = "";
        filterWifi.value = "";
        filterInvaliditet.value = "";
        sortSmestaji.value = "";
        minGostiju.value = "";
        maxGostiju.value = "";
        filterLjubimci.value = "";
        maxCena.value = "";
        sortJedinice.value = "";

       
        smestajiContainer.innerHTML = "";
        smestajCards.forEach(card => smestajiContainer.appendChild(card));
    }

    
    applyBtn.addEventListener("click", applyFilters);
    resetBtn.addEventListener("click", resetFilters);
});
