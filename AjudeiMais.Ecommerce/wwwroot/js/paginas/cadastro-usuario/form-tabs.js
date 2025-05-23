// ~/js/paginas/cadastro-usuario/form-tabs.js (Atualizado)

import {
    validarEmail,
    validarCPF,
    validarNome,
    validarTelefone,
    validarSenha,
    setInputInvalid,
    setInputValid,
    clearInputValidation,
    displayAlert // Continua usando o alert() nativo, conforme solicitado
} from './form-validacoes.js';

const form = document.getElementById('formularioCadastro');
const formSteps = form.querySelectorAll('.form-step');
const progressBar = document.querySelector('.progress-bar');
const stepIndicators = document.querySelectorAll('.step-indicator .step');
const btnNext = document.getElementById('btnProximo');
const btnBack = document.getElementById('btnVoltar');
const btnFinish = document.getElementById('btnFinalizar');
const termosCheckbox = document.getElementById('termos');
const fotoPerfilInput = document.getElementById('fotoPerfil');

// Elementos de senha
const senhaInput = document.getElementById('senha');
const confirmarSenhaInput = document.getElementById('confirmarSenha');

// Elementos para mostrar/ocultar senha
const passwordToggles = document.querySelectorAll('.password-toggle');


let currentStep = 0;

function updateProgressBar() {
    const progress = (currentStep / (formSteps.length - 1)) * 100;
    progressBar.style.width = `${progress}%`;

    stepIndicators.forEach((indicator, index) => {
        if (index <= currentStep) {
            indicator.classList.add('active');
        } else {
            indicator.classList.remove('active');
        }
    });
}

function showStep(stepIndex) {
    formSteps.forEach((step, index) => {
        if (index === stepIndex) {
            step.classList.add('active');
            step.scrollTop = 0; // Rola para o topo do passo
        } else {
            step.classList.remove('active');
        }
    });
    currentStep = stepIndex;
    updateProgressBar();
    updateNavigationButtons();
}

function updateNavigationButtons() {
    if (currentStep === 0) {
        btnBack.classList.add('d-none');
    } else {
        btnBack.classList.remove('d-none');
    }

    if (currentStep === formSteps.length - 1) {
        btnNext.classList.add('d-none');
        btnFinish.classList.remove('d-none');
    } else {
        btnNext.classList.remove('d-none');
        btnFinish.classList.add('d-none');
    }
}

/**
 * Valida os campos do passo atual do formulário.
 * @param {boolean} shouldFocus - Se deve focar no primeiro input inválido.
 * @returns {boolean} - True se todos os campos do passo atual são válidos, false caso contrário.
 */
function validateCurrentStep(shouldFocus = false) {
    let isValid = true;
    const currentFormStep = formSteps[currentStep];
    const inputs = currentFormStep.querySelectorAll('input[required], select[required], textarea[required]');
    let firstInvalidInput = null;
    let errorMessage = '';

    // Limpa o estado de validação de todos os inputs no passo atual antes de revalidar
    currentFormStep.querySelectorAll('.is-invalid, .is-valid').forEach(el => clearInputValidation(el));

    inputs.forEach(input => {
        let inputIsValid = true;
        const value = input.value ? input.value.trim() : '';
        let currentFieldErrorMessage = '';

        if (input.id === 'termos') {
            if (!input.checked) {
                inputIsValid = false;
                currentFieldErrorMessage = 'Você deve aceitar os termos de uso e política de privacidade.';
            }
        } else if (input.type === 'file' && input.id === 'fotoPerfil') {
            if (input.files.length === 0) {
                setInputInvalid(input.closest('.file-upload') || input, false);
            } else {
                const file = input.files[0];
                const maxSize = 5 * 1024 * 1024; // 5MB
                const allowedTypes = ['image/jpeg', 'image/png'];

                if (!allowedTypes.includes(file.type)) {
                    setInputInvalid(input.closest('.file-upload') || input, false);
                    displayAlert('Apenas arquivos JPG ou PNG são permitidos.', 'danger');
                } else if (file.size > maxSize) {
                    setInputInvalid(input.closest('.file-upload') || input, false);
                    displayAlert('A imagem deve ter no máximo 5MB.', 'danger');
                } else {
                    setInputValid(input.closest('.file-upload') || input);
                }
            }
        } else {
            // Validação de campos vazios (exceto senhas, que têm tratamento específico logo abaixo)
            if (value === '' && input.type !== 'password') {
                inputIsValid = false;
                currentFieldErrorMessage = `O campo ${input.previousElementSibling ? input.previousElementSibling.innerText.replace('*', '').trim() : input.placeholder || input.id} é obrigatório.`;
            } else {
                switch (input.id) {
                    case 'nomeCompleto':
                        if (!validarNome(value)) {
                            inputIsValid = false;
                            currentFieldErrorMessage = 'Nome completo inválido.';
                        }
                        break;
                    case 'documento':
                        if (!validarCPF(value)) {
                            inputIsValid = false;
                            currentFieldErrorMessage = 'CPF inválido. Verifique o formato e os dígitos.';
                        }
                        break;
                    case 'email':
                        if (!validarEmail(value)) {
                            inputIsValid = false;
                            currentFieldErrorMessage = 'E-mail inválido. Verifique o formato.';
                        }
                        break;
                    case 'telefone':
                        if (!validarTelefone(value)) {
                            inputIsValid = false;
                            currentFieldErrorMessage = 'Telefone inválido. Formato esperado: (XX) XXXXX-XXXX ou (XX) XXXX-XXXX.';
                        }
                        break;
                    case 'cep':
                        if (value.replace(/\D/g, '').length !== 8) {
                            inputIsValid = false;
                            currentFieldErrorMessage = 'CEP inválido. Formato esperado: 00.000-000.';
                        }
                        break;
                    case 'estado':
                        if (value === '') {
                            inputIsValid = false;
                            currentFieldErrorMessage = 'Por favor, selecione um Estado.';
                        }
                        break;
                    case 'senha':
                        if (value === '') { // Prioriza a mensagem de campo obrigatório para senhas
                            inputIsValid = false;
                            currentFieldErrorMessage = 'A senha é obrigatória.';
                        } else if (!validarSenha(value)) {
                            inputIsValid = false;
                            currentFieldErrorMessage = 'Senha fraca. A senha deve ter mínimo 8 caracteres, com letra, número e símbolo.';
                        }
                        break;
                    case 'confirmarSenha':
                        if (value === '') { // Prioriza a mensagem de campo obrigatório
                            inputIsValid = false;
                            currentFieldErrorMessage = 'A confirmação de senha é obrigatória.';
                        } else if (value !== senhaInput.value) {
                            inputIsValid = false;
                            currentFieldErrorMessage = 'As senhas não coincidem.';
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        if (!inputIsValid) {
            isValid = false;
            // Aplica a classe is-invalid ao elemento correto, considerando o file-upload
            if (input.id === 'fotoPerfil') {
                setInputInvalid(input.closest('.file-upload') || input, false);
            } else {
                setInputInvalid(input, false);
            }

            // Armazena o primeiro input inválido e sua mensagem de erro
            if (!firstInvalidInput) {
                firstInvalidInput = input;
                errorMessage = currentFieldErrorMessage;
            }
        } else {
            // Aplica a classe is-valid
            if (input.id === 'fotoPerfil') {
                setInputValid(input.closest('.file-upload') || input);
            } else {
                setInputValid(input);
            }
        }
    });

    if (!isValid) {
        if (shouldFocus && firstInvalidInput) {
            firstInvalidInput.focus();
        }
        // Exibe o alerta com a primeira mensagem de erro encontrada ou uma mensagem genérica
        displayAlert(errorMessage || 'Por favor, preencha todos os campos obrigatórios corretamente.', 'danger');
    }

    return isValid;
}


// Event Listeners para botões de navegação
btnNext.addEventListener('click', () => {
    if (validateCurrentStep(true)) {
        if (currentStep < formSteps.length - 1) {
            showStep(currentStep + 1);
        }
    }
});

btnBack.addEventListener('click', () => {
    if (currentStep > 0) {
        showStep(currentStep - 1);
    }
});

// Inicializa o formulário no primeiro passo
showStep(0);

// Adiciona listeners para feedback visual em tempo real ao sair do campo (blur)
form.querySelectorAll('input, select, textarea').forEach(input => {
    input.addEventListener('blur', () => {
        let inputIsValid = true;
        const value = input.value ? input.value.trim() : '';

        // Tratamento específico para campos de senha no blur
        if (input.id === 'senha') {
            if (value === '') {
                inputIsValid = false;
            } else if (!validarSenha(value)) {
                inputIsValid = false;
            }
            // Dispara validação da confirmação se a senha principal for alterada
            if (confirmarSenhaInput && confirmarSenhaInput.value !== '') { // Verifica se o input de confirmação existe e tem valor
                if (confirmarSenhaInput.value !== value) {
                    setInputInvalid(confirmarSenhaInput, false);
                } else {
                    setInputValid(confirmarSenhaInput);
                }
            }
        } else if (input.id === 'confirmarSenha') {
            if (value === '') {
                inputIsValid = false;
            } else if (value !== senhaInput.value) {
                inputIsValid = false;
            }
        }
        // Validação genérica para outros campos obrigatórios
        else if (input.hasAttribute('required') && value === '' && input.type !== 'password') {
            inputIsValid = false;
        } else {
            // Validações específicas para outros tipos de campo
            switch (input.id) {
                case 'nomeCompleto':
                    inputIsValid = validarNome(value);
                    break;
                case 'documento':
                    inputIsValid = validarCPF(value);
                    break;
                case 'email':
                    inputIsValid = validarEmail(value);
                    break;
                case 'telefone':
                    inputIsValid = validarTelefone(value);
                    break;
                case 'cep':
                    inputIsValid = value.replace(/\D/g, '').length === 8;
                    break;
                case 'estado':
                    inputIsValid = value !== '';
                    break;
                default:
                    // Se o input não tem id específico e não é obrigatório, considera válido
                    // Ou se é um input comum que não precisa de validação complexa
                    inputIsValid = true;
                    break;
            }
        }

        // Aplica as classes de validação Bootstrap
        if (!inputIsValid) {
            if (input.id === 'fotoPerfil') {
                setInputInvalid(input.closest('.file-upload') || input, false);
            } else {
                setInputInvalid(input, false);
            }
        } else {
            if (input.id === 'fotoPerfil') {
                setInputValid(input.closest('.file-upload') || input);
            } else {
                setInputValid(input);
            }
        }
    });

    // Ao digitar no input, limpa o estado de validação
    input.addEventListener('input', () => {
        if (input.type === 'file') {
            return; // Arquivos são validados no change
        }
        clearInputValidation(input);
        // Ao digitar na senha, limpa a validação da confirmação para que o usuário possa corrigir
        if (input.id === 'senha' && confirmarSenhaInput) {
            clearInputValidation(confirmarSenhaInput);
        }
    });

    // Para checkboxes e inputs de arquivo, o evento 'change' é mais adequado
    input.addEventListener('change', () => {
        if (input.type === 'checkbox') {
            if (input.checked) {
                setInputValid(input);
            } else {
                setInputInvalid(input, false);
            }
        } else if (input.type === 'file' && input.id === 'fotoPerfil') {
            if (input.files.length === 0) {
                setInputInvalid(input.closest('.file-upload') || input, false);
            } else {
                setInputValid(input.closest('.file-upload') || input);
            }
        }
    });
});

// --- FUNCIONALIDADE: MOSTRAR/OCULTAR SENHA ---
passwordToggles.forEach(toggle => {
    toggle.addEventListener('click', function() {
        const targetId = this.dataset.target;
        const passwordInput = document.getElementById(targetId);
        const icon = this.querySelector('.toggle-icon');

        if (passwordInput.type === 'password') {
            passwordInput.type = 'text';
            icon.classList.remove('fa-eye-slash');
            icon.classList.add('fa-eye');
        } else {
            passwordInput.type = 'password';
            icon.classList.remove('fa-eye');
            icon.classList.add('fa-eye-slash');
        }
        passwordInput.focus(); // Opcional: foca no input após alternar
    });
});


form.addEventListener('submit', (event) => {
    // Valida o passo atual antes de permitir o submit final
    if (!validateCurrentStep(true)) {
        event.preventDefault(); // Impede o envio do formulário se houver erros
    }
});