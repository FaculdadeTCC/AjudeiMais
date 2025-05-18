import { validarEmail, validarCPF, validarNome, validarTelefone, validarSenha } from './form-validacoes.js';

const cpfInput = document.querySelector('#documento');
const telefoneInput = document.querySelector('#telefone');
const steps = Array.from(document.querySelectorAll(".form-step"));
const indicators = Array.from(document.querySelectorAll(".step"));
const progressBar = document.querySelector(".progress-bar");
const btnProximo = document.getElementById("btnProximo");
const btnVoltar = document.getElementById("btnVoltar");
const btnFinalizar = document.getElementById("btnFinalizar");
const senhaInput = document.getElementById("senha");
const formularioCadastro = document.getElementById("formularioCadastro");

let currentStep = 0;

function updateFormSteps() {
    steps.forEach((step, index) => {
        step.classList.toggle("active", index === currentStep);
    });

    indicators.forEach((indicator, index) => {
        indicator.classList.toggle("active", index === currentStep);
    });

    const progress = ((currentStep + 1) / steps.length) * 100;
    progressBar.style.width = `${progress}%`;

    btnVoltar.classList.toggle("d-none", currentStep === 0);
    btnProximo.classList.toggle("d-none", currentStep === steps.length - 1);
    btnFinalizar.classList.toggle("d-none", currentStep !== steps.length - 1);
}

function exibirMensagemErro(campo, mensagem) {
    campo.classList.add("is-invalid");
    alert(mensagem);
}

function validarCamposEtapaAtual() {
    const stepAtual = steps[currentStep];
    const camposObrigatorios = stepAtual.querySelectorAll("[required]");
    let valido = true;

    for (const campo of camposObrigatorios) {
        campo.classList.remove("is-invalid");
        const valor = campo.value.trim();

        if (!valor) {
            exibirMensagemErro(campo, `O campo "${campo.getAttribute("placeholder") || campo.name || campo.id}" é obrigatório.`);
            valido = false;
            break;
        }

        if (currentStep === 0) {
            if (campo.id === "nomeCompleto") {
                if (!validarNome(valor)) {
                    exibirMensagemErro(campo, "O nome deve ter pelo menos 2 letras e conter apenas letras e espaços.");
                    valido = false;
                    break;
                }
            } else if (campo.id === "documento") {
                if (!validarCPF(valor)) {
                    exibirMensagemErro(campo, "CPF inválido.");
                    valido = false;
                    break;
                }
            } else if (campo.id === "email") {
                if (!validarEmail(valor)) {
                    exibirMensagemErro(campo, "E-mail inválido.");
                    valido = false;
                    break;
                }
            } else if (campo.id === "telefone") {
                if (!validarTelefone(valor)) {
                    exibirMensagemErro(campo, "Telefone inválido. Formato esperado: (XX) XXXXX-XXXX");
                    valido = false;
                    break;
                }
            } else if (campo.id === "fotoPerfil") {
                if (campo.files.length === 0) {
                    exibirMensagemErro(campo, "Foto de perfil é obrigatória.");
                    valido = false;
                    break;
                }
            }
        } else if (currentStep === 1) {
            if (campo.id === "cep" && valor.length < 8) {
                exibirMensagemErro(campo, "CEP inválido. Formato esperado: 00000-000");
                valido = false;
                break;
            } else if (campo.id === "rua" && valor.length < 2) {
                exibirMensagemErro(campo, "A rua deve ter pelo menos 2 caracteres.");
                valido = false;
                break;
            } else if (campo.id === "numero" && !/^\d+$/.test(valor)) {
                exibirMensagemErro(campo, "O número deve conter apenas dígitos.");
                valido = false;
                break;
            } else if (campo.id === "bairro" && valor.length < 2) {
                exibirMensagemErro(campo, "O bairro deve ter pelo menos 2 caracteres.");
                valido = false;
                break;
            } else if (campo.id === "cidade" && valor.length < 2) {
                exibirMensagemErro(campo, "A cidade deve ter pelo menos 2 caracteres.");
                valido = false;
                break;
            } else if (campo.id === "estado" && valor === "") {
                exibirMensagemErro(campo, "Selecione um estado.");
                valido = false;
                break;
            }
        } else if (currentStep === 2) {
            if (campo.id === "senha" && !validarSenha(valor)) {
                exibirMensagemErro(campo, "A senha deve ter pelo menos 8 caracteres, uma letra, um número e um caractere especial.");
                valido = false;
                return;
            }
        }
    }

    return valido;
}

function avancarEtapa() {
    if (currentStep < steps.length - 1) {
        if (validarCamposEtapaAtual()) {
            currentStep++;
            updateFormSteps();
        }
    }
}

function voltarEtapa() {
    if (currentStep > 0) {
        currentStep--;
        updateFormSteps();
    }
}

btnProximo.addEventListener("click", avancarEtapa);
btnVoltar.addEventListener("click", voltarEtapa);

btnFinalizar.addEventListener("click", (event) => {
    event.preventDefault();

    const senhaValor = senhaInput.value.trim();

    if (!validarSenha(senhaValor)) {
        exibirMensagemErro(senhaInput, "A senha deve ter pelo menos 8 caracteres, uma letra, um número e um caractere especial.");
        return;
    }

    if (validarCamposEtapaAtual()) {
        formularioCadastro.submit();
    }
});

document.addEventListener("DOMContentLoaded", () => {
    updateFormSteps();
});
