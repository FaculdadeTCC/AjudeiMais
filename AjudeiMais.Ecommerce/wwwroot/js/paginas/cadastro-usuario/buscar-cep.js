// Script para mostrar o nome do arquivo quando selecionado
document.getElementById('fotoPerfil').addEventListener('change', function (e) {
    let fileName = e.target.files[0].name;
    let parent = e.target.parentNode;
    let icon = parent.querySelector('i');
    let text = parent.querySelector('p');

    icon.className = 'bi bi-check-circle-fill text-success';
    text.textContent = fileName;
});

// Script para buscar endereço pelo CEP
document.getElementById('cep').addEventListener('blur', function () {
    const cep = this.value.replace(/\D/g, '');

    if (cep.length === 8) {
        fetch(`https://viacep.com.br/ws/${cep}/json/`)
            .then(response => response.json())
            .then(data => {
                if (!data.erro) {
                    document.getElementById('rua').value = data.logradouro;
                    document.getElementById('bairro').value = data.bairro;
                    document.getElementById('cidade').value = data.localidade;
                    document.getElementById('estado').value = data.uf;
                    // Foca no campo número após preencher os dados
                    document.getElementById('numero').focus();
                }
            });
    }
});