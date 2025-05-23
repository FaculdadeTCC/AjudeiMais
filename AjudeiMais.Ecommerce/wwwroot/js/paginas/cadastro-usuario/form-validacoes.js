// ~/js/paginas/cadastro-usuario/form-validacoes.js (Atualizado)

// Funções de validação existentes
function validarEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

function validarCPF(cpf) {
    cpf = cpf.replace(/[^\d]+/g, '');
    if (cpf.length !== 11 || /^(\d)\1+$/.test(cpf)) return false;

    let soma = 0, resto;
    for (let i = 1; i <= 9; i++) soma += parseInt(cpf.charAt(i - 1)) * (11 - i);
    resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(cpf.charAt(9))) return false;

    soma = 0;
    for (let i = 1; i <= 10; i++) soma += parseInt(cpf.charAt(i - 1)) * (12 - i);
    resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(cpf.charAt(10))) return false;

    return true;
}

function validarNome(nome) {
    return /^[A-Za-zÀ-ÿ\s]{2,}$/.test(nome);
}

function validarTelefone(telefone) {
    const regexTelefone = /^\(\d{2}\) \d{4,5}-\d{4}$/;
    return regexTelefone.test(telefone);
}

function validarSenha(senha) {
    // Requisitos:
    // - Mínimo 8 caracteres
    // - Pelo menos uma letra (maiúscula ou minúscula)
    // - Pelo menos um número
    // - Pelo menos um caractere especial (dos listados: @$!%*?&)
    const regexSenha = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    return regexSenha.test(senha);
}

// Funções para feedback visual
function setInputInvalid(inputElement, shouldFocus = true) {
    inputElement.classList.remove('is-valid');
    inputElement.classList.add('is-invalid');
    if (shouldFocus) {
        inputElement.focus();
    }
}

function setInputValid(inputElement) {
    inputElement.classList.remove('is-invalid');
    inputElement.classList.add('is-valid');
}

function clearInputValidation(inputElement) {
    inputElement.classList.remove('is-invalid');
    inputElement.classList.remove('is-valid');
}

function displayAlert(message, type = 'danger') {
    alert(message);
}

export {
    validarEmail,
    validarCPF,
    validarNome,
    validarTelefone,
    validarSenha, // Exporta a função validarSenha
    setInputInvalid,
    setInputValid,
    clearInputValidation,
    displayAlert
};