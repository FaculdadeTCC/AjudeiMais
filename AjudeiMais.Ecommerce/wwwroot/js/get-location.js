navigator.geolocation.getCurrentPosition(function (position) {
    const latitude = position.coords.latitude;
    const longitude = position.coords.longitude;

    // Envie para o back-end para buscar os anúncios próximos
    fetch(`/anuncios/proximos?lat=${latitude}&lng=${longitude}`)
        .then(response => response.json())
        .then(anuncios => {
            console.log(anuncios); // renderizar os anúncios aqui
        });
});
