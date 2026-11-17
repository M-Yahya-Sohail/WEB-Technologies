document.querySelectorAll('[data-password-toggle]').forEach((button) => {
    button.addEventListener('click', () => {
        const input = document.getElementById(button.dataset.passwordToggle);
        if (!input) return;

        const shouldShow = input.type === 'password';
        input.type = shouldShow ? 'text' : 'password';
        button.setAttribute('aria-pressed', String(shouldShow));

        const fieldName = input.id === 'Input_ConfirmPassword' ? 'confirm password' : 'password';
        const action = shouldShow ? 'Hide' : 'Show';
        button.setAttribute('aria-label', `${action} ${fieldName}`);
        button.setAttribute('title', `${action} ${fieldName}`);
        button.querySelector('.password-eye-open').classList.toggle('d-none', shouldShow);
        button.querySelector('.password-eye-closed').classList.toggle('d-none', !shouldShow);
    });
});
