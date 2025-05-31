// wwwroot/js/quill/quill-initializer.js

// Declara a variável 'quill' globalmente para ser acessível em outras partes do seu script
// É importante que essa variável seja global ou acessível no escopo onde será utilizada.
let quillEditorInstance;

/**
 * Inicializa o editor Quill em um elemento HTML específico.
 * @param {string} editorId - O ID do elemento DIV onde o Quill será inicializado (ex: 'DescricaoEditor').
 * @param {string} initialContentId - O ID do INPUT HIDDEN que contém o conteúdo HTML inicial (ex: 'Descricao').
 * @param {function} onSubmitCallback - Uma função de callback a ser executada no evento onsubmit do formulário.
 * Ela deve retornar 'true' para permitir o envio ou 'false' para impedir.
 */
function initializeQuill(editorId, initialContentId, onSubmitCallback) {
    const editorElement = document.getElementById(editorId);
    if (!editorElement) {
        console.error(`Elemento com ID '${editorId}' não encontrado para inicializar o Quill.`);
        return;
    }

    quillEditorInstance = new Quill(`#${editorId}`, {
        theme: 'snow',
        placeholder: 'Forneça uma descrição detalhada do seu item...',
        modules: {
            toolbar: [
                [{ 'header': [1, 2, false] }],
                ['bold', 'italic', 'underline', 'strike'],
                [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                [{ 'align': [] }],
                ['clean']
            ]
        }
    });

    // Carrega o conteúdo inicial do INPUT HIDDEN no editor Quill, se houver
    const initialContentElement = document.getElementById(initialContentId);
    if (initialContentElement && initialContentElement.value) {
        quillEditorInstance.clipboard.dangerouslyPasteHTML(initialContentElement.value);
    }

    // Sincroniza o conteúdo do Quill de volta para o INPUT HIDDEN antes de submeter o formulário
    // E executa o callback de validação
    const form = editorElement.closest('form');
    if (form) {
        // Armazena a função original onsubmit para não sobrescrever
        const originalOnSubmit = form.onsubmit;

        form.onsubmit = function (event) {
            // Define o valor do INPUT HIDDEN com o conteúdo HTML do editor Quill
            if (initialContentElement) {
                initialContentElement.value = quillEditorInstance.root.innerHTML;
            }

            // Executa o callback de validação customizado
            if (onSubmitCallback && !onSubmitCallback(quillEditorInstance)) {
                return false; // Impede o envio se o callback retornar false
            }

            // Executa a função onsubmit original, se existir
            if (originalOnSubmit) {
                return originalOnSubmit.apply(this, arguments);
            }

            return true; // Permite o envio se tudo estiver ok
        };
    }
}

// Funções utilitárias para acessar a instância do Quill de fora, se necessário
function getQuillInstance() {
    return quillEditorInstance;
}

function getQuillContentAsHTML() {
    return quillEditorInstance ? quillEditorInstance.root.innerHTML : '';
}

function getQuillContentAsText() {
    return quillEditorInstance ? quillEditorInstance.getText().trim() : '';
}