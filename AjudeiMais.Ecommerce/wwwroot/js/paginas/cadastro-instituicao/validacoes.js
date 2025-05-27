document.addEventListener('DOMContentLoaded', () => {
    const steps = Array.from(document.querySelectorAll('.form-step'));
    const progressBar = document.querySelector('.progress-bar');

    const btnVoltar = document.getElementById('btnVoltar');
    const btnProximo = document.getElementById('btnProximo');
    const btnFinalizar = document.getElementById('btnFinalizar');
    const form = document.querySelector('form');

    let currentStep = 0;

    // Inicializar estado
    updateStep();

    // Navegação dos passos
    btnProximo.addEventListener('click', () => {
        if (validateStep(currentStep)) {
            if (currentStep < steps.length - 1) {
                currentStep++;
                updateStep();
            }
        }
    });

    btnVoltar.addEventListener('click', () => {
        if (currentStep > 0) {
            currentStep--;
            updateStep();
        }
    });

    // Função para atualizar UI conforme step atual
    function updateStep() {
        steps.forEach((step, i) => {
            step.classList.toggle('active', i === currentStep);
        });

        // Atualiza progress bar em % (baseado na quantidade de steps)
        const progressPercent = ((currentStep + 1) / steps.length) * 100;
        progressBar.style.width = `${progressPercent}%`;

        // Controle dos botões
        btnVoltar.classList.toggle('d-none', currentStep === 0);
        btnProximo.classList.toggle('d-none', currentStep === steps.length - 1);
        btnFinalizar.classList.toggle('d-none', currentStep !== steps.length - 1);
    }

    // Validação mínima para avançar (pode ser adaptada conforme sua validação do servidor)
    function validateStep(stepIndex) {
        // O jQuery Validate já está incluso, então podemos usar
        // apenas para validar o step visível
        const currentFields = steps[stepIndex].querySelectorAll('input, textarea, select');

        // Trigger unobtrusive validation para os campos do step atual
        let valid = true;
        currentFields.forEach(field => {
            if (!$(field).valid()) {
                valid = false;
            }
        });
        return valid;
    }

    // Toggle visibilidade das senhas
    document.querySelectorAll('.password-toggle').forEach(toggle => {
        toggle.addEventListener('click', () => {
            const targetName = toggle.dataset.target; // "senha" ou "confirmarSenha"
            // O input é o input com asp-for="Senha" ou o segundo input no segundo step (confirmar senha não tem asp-for)
            let input;
            if (targetName === 'senha') {
                input = document.querySelector('input[asp-for="Senha"]');
            } else if (targetName === 'confirmarSenha') {
                // Está no segundo input do último step
                input = toggle.closest('.form-floating').querySelector('input');
            }
            if (!input) return;

            if (input.type === 'password') {
                input.type = 'text';
                toggle.querySelector('.toggle-icon').classList.remove('fa-eye-slash');
                toggle.querySelector('.toggle-icon').classList.add('fa-eye');
            } else {
                input.type = 'password';
                toggle.querySelector('.toggle-icon').classList.remove('fa-eye');
                toggle.querySelector('.toggle-icon').classList.add('fa-eye-slash');
            }
        });
    });

    // Preview e remoção das imagens
    const inputFotos = document.getElementById('fotos');
    const previewContainer = document.getElementById('previewContainer');
    const removeBtn = document.getElementById('removeImagesButton');

    inputFotos.addEventListener('change', () => {
        previewContainer.innerHTML = '';
        const files = Array.from(inputFotos.files);

        if (files.length > 0) {
            removeBtn.classList.remove('d-none');
        } else {
            removeBtn.classList.add('d-none');
        }

        files.forEach(file => {
            const reader = new FileReader();
            reader.onload = function (e) {
                const img = document.createElement('img');
                img.src = e.target.result;
                img.className = 'img-thumbnail';
                img.style.height = '100px';
                img.style.objectFit = 'cover';
                previewContainer.appendChild(img);
            };
            reader.readAsDataURL(file);
        });
    });

    removeBtn.addEventListener('click', () => {
        inputFotos.value = '';
        previewContainer.innerHTML = '';
        removeBtn.classList.add('d-none');
    });
});
