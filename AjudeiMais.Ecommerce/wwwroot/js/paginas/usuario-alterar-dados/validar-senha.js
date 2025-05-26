// --- Validação de Senha (Nova Lógica) ---
const novaSenhaInput = $('#novaSenha');
const confirmarNovaSenhaInput = $('#confirmarNovaSenha');
const confirmarNovaSenhaError = $('#confirmarNovaSenhaError');
const btnSalvarSenha = $('#btnSalvarSenha');
const formSeguranca = $('#formSeguranca');

function validatePasswordMatch() {
	if (novaSenhaInput.val() !== confirmarNovaSenhaInput.val() && confirmarNovaSenhaInput.val() !== '') {
		confirmarNovaSenhaError.removeClass('d-none');
		confirmarNovaSenhaInput.addClass('is-invalid');
		return false;
	} else {
		confirmarNovaSenhaError.addClass('d-none');
		confirmarNovaSenhaInput.removeClass('is-invalid');
		return true;
	}
}

// Regex para validação de complexidade: Mínimo 8 caracteres, 1 letra (maiúscula ou minúscula), 1 número, 1 caractere especial
const passwordRegex = /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+={}\[\]|\\:;"'<>,.?/~`-])[A-Za-z\d!@#$%^&*()_+={}\[\]|\\:;"'<>,.?/~`-]{8,}$/;

function validatePasswordComplexity() {
	const senha = novaSenhaInput.val();
	if (senha.length > 0 && !passwordRegex.test(senha)) {
		novaSenhaInput.addClass('is-invalid');
		// Você pode adicionar um span de erro específico para complexidade aqui se quiser um feedback mais detalhado
		$('#novaSenhaHelp').addClass('text-danger').removeClass('text-muted');
		return false;
	} else {
		novaSenhaInput.removeClass('is-invalid');
		$('#novaSenhaHelp').removeClass('text-danger').addClass('text-muted');
		return true;
	}
}


novaSenhaInput.on('keyup', function () {
	validatePasswordComplexity();
	validatePasswordMatch();
});

confirmarNovaSenhaInput.on('keyup', function () {
	validatePasswordMatch();
});

formSeguranca.on('submit', function (event) {
	let isValid = true;

	// Valida a complexidade da nova senha
	if (!validatePasswordComplexity()) {
		isValid = false;
	}

	// Valida se as senhas coincidem
	if (!validatePasswordMatch()) {
		isValid = false;
	}


	if (!isValid) {
		event.preventDefault(); // Impede o envio do formulário
		alert('Por favor, corrija os erros no formulário de segurança antes de prosseguir.');
	}
});
