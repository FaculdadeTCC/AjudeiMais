// wwwroot/js/paginas/categoria-produto/modal-edicao.js

$(document).ready(function () {
    // Listener de evento para o botão "Editar"
    // Usando delegação de evento porque as linhas da tabela são carregadas dinamicamente (ex: paginação)
    $(document).on('click', '.edit-category-btn', function (e) {
        e.preventDefault(); // Previne o comportamento padrão do link

        var editUrl = $(this).attr('href'); // Obtém a URL do atributo href

        // Faz uma requisição AJAX para carregar a partial view de edição
        $.ajax({
            url: editUrl,
            type: 'GET',
            success: function (data) {
                // Se a chamada AJAX for bem-sucedida, 'data' conterá o HTML da partial view (seu modal)
                $('#modalEditCategoryContainer').html(data); // Insere o HTML do modal em um contêiner
                $('#modalEditCategory').modal('show'); // Mostra o modal do Bootstrap

                // Inicializa o status do checkbox após o conteúdo do modal ser carregado
                // Isso garante que o valor do input hidden corresponda ao estado do checkbox dos dados do modelo
                const editCheckbox = document.getElementById('editCategoryStatus');
                const hiddenEditInput = document.getElementById('hiddenEditCategoryStatus');
                if (editCheckbox && hiddenEditInput) {
                    editCheckbox.checked = (hiddenEditInput.value === 'true'); // Define o checkbox com base no input hidden
                    editCheckbox.addEventListener('change', function () {
                        hiddenEditInput.value = editCheckbox.checked ? 'true' : 'false';
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error("Erro ao carregar o modal de edição:", error);
                console.error("Status:", status);
                console.error("Response Text:", xhr.responseText);
                // Opcionalmente, exiba um alerta para o usuário
                alert("Não foi possível carregar a categoria para edição. Tente novamente.");
            }
        });
    });

    // Lida com o envio do formulário dentro do modal
    // Isso é importante se você quiser enviar o formulário via AJAX em vez de recarregar a página
    $(document).on('submit', '#editCategoryForm', function (e) {
        e.preventDefault(); // Previne o envio padrão do formulário

        var form = $(this);
        var formData = new FormData(form[0]); // Obtém os dados do formulário, incluindo arquivos (se houver)
        var actionUrl = form.attr('action');

        $.ajax({
            url: actionUrl,
            type: 'POST',
            data: formData,
            processData: false, // Não processa os dados
            contentType: false, // Não define o tipo de conteúdo (FormData fará isso)
            success: function (response) {
                // Supondo que seu controller retorne uma resposta JSON indicando sucesso/falha
                if (response.success) {
                    $('#modalEditCategory').modal('hide'); // Oculta o modal
                    // Atualiza a tabela ou a linha específica
                    // Por simplicidade, vamos apenas recarregar a página por enquanto (você pode otimizar isso depois)
                    location.reload();
                    // Ou, se você tiver uma atualização parcial, você poderia fazer:
                    // window.location.href = '@Url.RouteUrl("admin-categorias-produto")?alertType=success&alertMessage=' + encodeURIComponent(response.message);
                } else {
                    // Exibe a mensagem de erro da resposta
                    alert("Erro ao salvar: " + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("Erro ao submeter o formulário de edição:", error);
                alert("Ocorreu um erro ao salvar as alterações. Tente novamente.");
            }
        });
    });
});