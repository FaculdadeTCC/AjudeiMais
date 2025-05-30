
// Script existente para o modal de criação
document.addEventListener("DOMContentLoaded", function () {
    const createCheckbox = document.getElementById('categoryStatus');
    const createHiddenInput = document.getElementById('hiddenCategoryStatus');

    if (createCheckbox && createHiddenInput) {
        createCheckbox.addEventListener('change', function () {
            createHiddenInput.value = createCheckbox.checked ? 'true' : 'false';
        });
    }

    // --- Novo script para o modal de edição ---

    // Pega uma referência ao modal de edição
    const editCategoryModal = document.getElementById('modalEditCategory');

    // Adiciona um listener para quando o modal for mostrado (antes de animar)
    if (editCategoryModal) {
        editCategoryModal.addEventListener('show.bs.modal', function (event) {
            // Botão que disparou o modal
            const button = event.relatedTarget;

            // Extrai as informações dos atributos data- do botão
            const id = button.getAttribute('data-id');
            const nome = button.getAttribute('data-nome');
            const icone = button.getAttribute('data-icone');
            const habilitado = button.getAttribute('data-habilitado') === 'true'; // Converte para booleano

            // Pega referências aos elementos do formulário dentro do modal de edição
            const modalTitle = editCategoryModal.querySelector('#modalEditCategoryLabel');
            const inputId = editCategoryModal.querySelector('#editCategoryId');
            const inputName = editCategoryModal.querySelector('#editCategoryName');
            const inputIcon = editCategoryModal.querySelector('#editCategoryIcon');
            const checkboxStatus = editCategoryModal.querySelector('#editCategoryStatus');
            const hiddenInputStatus = editCategoryModal.querySelector('#hiddenEditCategoryStatus');

            // Preenche o modal com os dados
            if (modalTitle) {
                modalTitle.textContent = `Editar Categoria: ${nome}`;
            }
            if (inputId) {
                inputId.value = id;
            }
            if (inputName) {
                inputName.value = nome;
            }
            if (inputIcon) {
                inputIcon.value = icone;
            }
            if (checkboxStatus) {
                checkboxStatus.checked = habilitado ? 'true' : 'false';
            }
            if (hiddenInputStatus) {
                hiddenInputStatus = habilitado ? 'true' : 'false';    
            }
        });

        // Adiciona um listener para a mudança do checkbox de status no modal de edição
        const editCheckbox = document.getElementById('editCategoryStatus');
        const editHiddenInput = document.getElementById('hiddenEditCategoryStatus');

        if (editCheckbox && editHiddenInput) {
            editCheckbox.addEventListener('change', function () {
                editHiddenInput.value = editCheckbox.checked ? 'true' : 'false';
            });
        }
    }
});