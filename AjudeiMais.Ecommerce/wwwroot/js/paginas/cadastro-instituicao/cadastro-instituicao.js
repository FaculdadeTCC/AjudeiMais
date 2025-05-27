document.addEventListener("DOMContentLoaded", function () {
    const formSteps = document.querySelectorAll(".form-step");
    const steps = document.querySelectorAll(".step");
    const btnProximo = document.getElementById("btnProximo");
    const btnVoltar = document.getElementById("btnVoltar");
    const btnFinalizar = document.getElementById("btnFinalizar");
    const progressBar = document.querySelector(".progress-bar");
    let currentStep = 0;

    function updateFormSteps() {
        formSteps.forEach((step, index) => {
            step.classList.toggle("active", index === currentStep);
        });

        steps.forEach((step, index) => {
            step.classList.toggle("active", index <= currentStep);
        });

        progressBar.style.width = `${((currentStep + 1) / formSteps.length) * 100}%`;

        btnVoltar.classList.toggle("d-none", currentStep === 0);
        btnProximo.classList.toggle("d-none", currentStep === formSteps.length - 1);
        btnFinalizar.classList.toggle("d-none", currentStep !== formSteps.length - 1);
    }

    btnProximo.addEventListener("click", () => {
        if (currentStep < formSteps.length - 1) {
            currentStep++;
            updateFormSteps();
        }
    });

    btnVoltar.addEventListener("click", () => {
        if (currentStep > 0) {
            currentStep--;
            updateFormSteps();
        }
    });

    updateFormSteps();

    // Preview da Imagem
    const fotoInput = document.getElementById("fotoPerfil");
    const imagePreview = document.getElementById("imagePreview");
    const removeImageButton = document.getElementById("removeImageButton");
    const fileUploadContent = document.getElementById("fileUploadContent");

    fotoInput.addEventListener("change", function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                imagePreview.src = e.target.result;
                imagePreview.classList.remove("d-none");
                removeImageButton.classList.remove("d-none");
                fileUploadContent.classList.add("d-none");
            };
            reader.readAsDataURL(file);
        }
    });

    removeImageButton.addEventListener("click", function () {
        fotoInput.value = "";
        imagePreview.src = "#";
        imagePreview.classList.add("d-none");
        removeImageButton.classList.add("d-none");
        fileUploadContent.classList.remove("d-none");
    });

    // Alternar visualização de senha
    document.querySelectorAll(".password-toggle").forEach(toggle => {
        toggle.addEventListener("click", function () {
            const inputId = this.getAttribute("data-target");
            const input = document.getElementById(inputId);
            const icon = this.querySelector(".toggle-icon");
            if (input.type === "password") {
                input.type = "text";
                icon.classList.remove("fa-eye-slash");
                icon.classList.add("fa-eye");
            } else {
                input.type = "password";
                icon.classList.remove("fa-eye");
                icon.classList.add("fa-eye-slash");
            }
        });
    });
});
