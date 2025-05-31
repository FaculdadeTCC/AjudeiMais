// Variáveis globais
let selectedFiles = [];
let currentTabIndex = 0;

// Função principal de navegação entre abas
function navegarParaAba(targetIndex) {

    const tabIds = ['etapa-info-basicas', 'etapa-especificacoes', 'etapa-imagens', 'etapa-publicacao'];
    const tabButtonIds = ['tab-info-basicas', 'tab-especificacoes', 'tab-imagens', 'tab-publicacao'];

    // Se está tentando avançar, valida a aba atual
    if (targetIndex > currentTabIndex) {
        if (!validarAbaAtual(currentTabIndex)) {
            return false;
        }
    }

    // Habilita a próxima aba se necessário
    if (targetIndex > currentTabIndex) {
        const nextTabButton = document.getElementById(tabButtonIds[targetIndex]);
        if (nextTabButton) {
            nextTabButton.disabled = false;
        }
    }

    // Atualiza resumo se indo para aba de publicação
    if (targetIndex === 3) {
        atualizarResumoAnuncio();
    }

    // Ativa a aba
    ativarAba(targetIndex);

    return true;
}

// Função para validar aba atual
function validarAbaAtual(tabIndex) {
    const form = document.getElementById("formAnuncio");
    let isValid = true;

    switch (tabIndex) {
        case 0: // Informações Básicas
            // Valida campos obrigatórios
            isValid = $(form).validate().element("#Nome") &&
                $(form).validate().element("#CategoriaProduto_ID");

            // Validação da descrição do Quill
            if (typeof getQuillContentAsText === 'function') {
                const quillText = getQuillContentAsText().trim();
                if (quillText === '') {
                    $(form).validate().showErrors({ "Descricao": "A descrição é obrigatória." });
                    isValid = false;
                }
            }
            break;

        case 1: // Especificações
            isValid = $(form).validate().element("#Condicao") &&
                $(form).validate().element("#Quantidade") &&
                $(form).validate().element("#UnidadeMedida");
            break;

        case 2: // Imagens
            if (selectedFiles.length === 0) {
                $(form).validate().showErrors({ "ProdutoImagens": "Pelo menos uma imagem é obrigatória." });
                isValid = false;
            }
            break;
    }

    return isValid;
}

// Função para ativar uma aba específica
function ativarAba(index) {
    const tabButtonIds = ['tab-info-basicas', 'tab-especificacoes', 'tab-imagens', 'tab-publicacao'];
    const tabButton = document.getElementById(tabButtonIds[index]);

    if (tabButton && !tabButton.disabled) {
        // Remove active de todas as abas
        document.querySelectorAll('.nav-link').forEach(tab => {
            tab.classList.remove('active');
            tab.setAttribute('aria-selected', 'false');
        });

        document.querySelectorAll('.tab-pane').forEach(pane => {
            pane.classList.remove('show', 'active');
        });

        // Ativa a aba desejada
        tabButton.classList.add('active');
        tabButton.setAttribute('aria-selected', 'true');

        const targetPane = document.querySelector(tabButton.getAttribute('data-bs-target'));
        if (targetPane) {
            targetPane.classList.add('show', 'active');
        }

        currentTabIndex = index;
    }
}

// Função para atualizar resumo
function atualizarResumoAnuncio() {
    const nomeInput = document.getElementById('Nome');
    const categoriaSelect = document.getElementById('CategoriaProduto_ID');
    const condicaoSelect = document.getElementById('Condicao');
    const quantidadeInput = document.getElementById('Quantidade');
    const unidadeSelect = document.getElementById('UnidadeMedida');

    if (nomeInput) document.getElementById('resumoNome').textContent = nomeInput.value;
    if (categoriaSelect && categoriaSelect.selectedIndex > 0) {
        document.getElementById('resumoCategoria').textContent = categoriaSelect.options[categoriaSelect.selectedIndex].text;
    }
    if (condicaoSelect) document.getElementById('resumoCondicao').textContent = condicaoSelect.value;
    if (quantidadeInput) document.getElementById('resumoQuantidade').textContent = quantidadeInput.value;
    if (unidadeSelect) document.getElementById('resumoUnidadeMedida').textContent = unidadeSelect.value;

    document.getElementById('resumoImagemStatus').textContent = `${selectedFiles.length} imagem(ns) selecionada(s)`;

    if (typeof getQuillContentAsHTML === 'function') {
        document.getElementById('resumoDescricao').innerHTML = getQuillContentAsHTML();
    }
}

// Inicialização quando o DOM estiver carregado
document.addEventListener('DOMContentLoaded', function () {
    // Configuração inicial das abas
    currentTabIndex = 0;
    document.getElementById('tab-especificacoes').disabled = true;
    document.getElementById('tab-imagens').disabled = true;
    document.getElementById('tab-publicacao').disabled = true;

    // Previne clique direto em abas desabilitadas
    document.querySelectorAll('.nav-link').forEach(tab => {
        tab.addEventListener('click', function (e) {
            if (this.disabled) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
        });
    });

    // Inicialização dos componentes de imagem
    initializeImageHandling();

    // Inicialização do Quill Editor (se a função existir)
    if (typeof initializeQuill === 'function') {
        initializeQuill(
            'DescricaoEditor',
            'Descricao',
            function (quillInstance) {
                const quillContent = quillInstance.getText().trim();
                return quillContent !== '';
            }
        );
    }
});

// Função para inicializar manipulação de imagens
function initializeImageHandling() {
    const fotosInput = document.getElementById('fotosInput');
    const previewContainer = document.getElementById('previewContainer');
    const removeImagesButton = document.getElementById('removeImagesButton');
    const fileUploadContent = document.getElementById('fileUploadContent');

    if (!fotosInput) return;

    fotosInput.addEventListener('change', function (event) {
        selectedFiles = [];
        previewContainer.innerHTML = '';
        fileUploadContent.classList.add('d-none');

        if (event.target.files.length > 0) {
            for (let i = 0; i < event.target.files.length; i++) {
                selectedFiles.push(event.target.files[i]);
            }
            renderPreviews();
            removeImagesButton.classList.remove('d-none');
        } else {
            removeImagesButton.classList.add('d-none');
            fileUploadContent.classList.remove('d-none');
        }
        $(fotosInput).valid();
    });

    removeImagesButton.addEventListener('click', function () {
        fotosInput.value = '';
        selectedFiles = [];
        previewContainer.innerHTML = '';
        removeImagesButton.classList.add('d-none');
        fileUploadContent.classList.remove('d-none');
        $(fotosInput).valid();
    });

    // Drag and drop
    const fileUploadArea = fotosInput.closest('.file-upload');

    fileUploadArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        fileUploadArea.classList.add('drag-over');
    });

    fileUploadArea.addEventListener('dragleave', () => {
        fileUploadArea.classList.remove('drag-over');
    });

    fileUploadArea.addEventListener('drop', (e) => {
        e.preventDefault();
        fileUploadArea.classList.remove('drag-over');
        const files = e.dataTransfer.files;

        if (files.length > 0) {
            selectedFiles = [];
            for (let i = 0; i < files.length; i++) {
                selectedFiles.push(files[i]);
            }
            updateFileInputFromSelectedFiles();
            renderPreviews();
            $(fotosInput).valid();
        }
    });
}

// Função para renderizar pré-visualizações
function renderPreviews() {
    const previewContainer = document.getElementById('previewContainer');
    const removeImagesButton = document.getElementById('removeImagesButton');
    const fileUploadContent = document.getElementById('fileUploadContent');

    previewContainer.innerHTML = '';

    for (let i = 0; i < selectedFiles.length; i++) {
        generateFilePreview(selectedFiles[i], i);
    }

    if (selectedFiles.length > 0) {
        removeImagesButton.classList.remove('d-none');
        fileUploadContent.classList.add('d-none');
    } else {
        removeImagesButton.classList.add('d-none');
        fileUploadContent.classList.remove('d-none');
    }
}

// Função para gerar pré-visualização de arquivo
function generateFilePreview(file, index) {
    if (!file || !file.type.startsWith('image/')) return;

    const previewContainer = document.getElementById('previewContainer');
    const reader = new FileReader();

    reader.onload = function (e) {
        const imgWrapper = document.createElement('div');
        imgWrapper.classList.add('position-relative');
        imgWrapper.dataset.index = index;

        const img = document.createElement('img');
        img.src = e.target.result;
        img.classList.add('img-thumbnail', 'mb-2');
        img.style.maxWidth = '100px';
        img.style.maxHeight = '100px';

        if (index === 0) {
            const principalTag = document.createElement('span');
            principalTag.classList.add('badge', 'bg-primary', 'position-absolute', 'top-0', 'start-0', 'm-1');
            principalTag.textContent = 'Principal';
            imgWrapper.appendChild(principalTag);
        }

        const removeButton = document.createElement('button');
        removeButton.type = 'button';
        removeButton.classList.add('btn', 'btn-sm', 'btn-danger', 'position-absolute', 'top-0', 'end-0', 'm-1');
        removeButton.innerHTML = '&times;';
        removeButton.title = 'Remover imagem';
        removeButton.onclick = function () {
            removeFileByIndex(index);
        };

        imgWrapper.appendChild(img);
        imgWrapper.appendChild(removeButton);
        previewContainer.appendChild(imgWrapper);
    };

    reader.readAsDataURL(file);
}

// Função para remover arquivo por índice
function removeFileByIndex(indexToRemove) {
    selectedFiles.splice(indexToRemove, 1);
    updateFileInputFromSelectedFiles();
    renderPreviews();
    $(document.getElementById('fotosInput')).valid();
}

// Atualiza o FileList do input
function updateFileInputFromSelectedFiles() {
    const fotosInput = document.getElementById('fotosInput');
    const dataTransfer = new DataTransfer();
    selectedFiles.forEach(file => dataTransfer.items.add(file));
    fotosInput.files = dataTransfer.files;
}
