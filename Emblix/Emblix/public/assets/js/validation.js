// Валидация формы авторизации
function validateAuthForm() {
    clearErrors();
    const email = document.getElementById('email');
    const emailRegex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/;

    if (!emailRegex.test(email.value)) {
        showError(email, 'Неверный формат email');
        return false;
    }
    return true;
}

// Валидация формы входа
function validateLoginForm() {
    clearErrors();
    let isValid = true;

    const email = document.getElementById('email');
    const emailRegex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/;
    if (!emailRegex.test(email.value)) {
        showError(email, 'Неверный формат email');
        isValid = false;
    }

    const password = document.getElementById('password');
    const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
    if (!passwordRegex.test(password.value)) {
        showError(password, 'Неверный формат пароля');
        isValid = false;
    }

    return isValid;
}

// Валидация формы регистрации
function validateRegistrationForm() {
    clearErrors();
    let isValid = true;

    const email = document.getElementById('email');
    const emailRegex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/;
    if (!emailRegex.test(email.value)) {
        showError(email, 'Неверный формат email');
        isValid = false;
    }

    const password = document.getElementById('password');
    const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/;
    if (!passwordRegex.test(password.value)) {
        showError(password, 'Пароль должен содержать минимум 8 символов, включая буквы и цифры');
        isValid = false;
    }

    const confirmPassword = document.getElementById('confirmPassword');
    if (password.value !== confirmPassword.value) {
        showError(confirmPassword, 'Пароли не совпадают');
        isValid = false;
    }

    return isValid;
}

// Добавьте эту функцию в ваш существующий скрипт
function validateMovieFields() {
    clearErrors();
    let isValid = true;
    const form = document.getElementById('addMovieForm');

    // Валидация года
    const releaseYear = form.releaseYear;
    if (releaseYear.value) {
        const year = parseInt(releaseYear.value);
        const currentYear = new Date().getFullYear();
        if (year < 1900 || year > currentYear + 1) {
            showError(releaseYear, `Год должен быть между 1900 и ${currentYear + 1}`);
            isValid = false;
        }
    }

    // Валидация длительности
    const duration = form.duration;
    if (duration.value) {
        const minutes = parseInt(duration.value);
        if (minutes < 1 || minutes > 1000) {
            showError(duration, 'Длительность должна быть от 1 до 1000 минут');
            isValid = false;
        }
    }

    // Валидация URL изображений
    const posterUrl = form.posterUrl;
    const backgroundUrl = form.backgroundUrl;
    const actorPhotoUrl = document.getElementById('actorPhotoUrl');

    if (!posterUrl.value.startsWith('/assets/img/movies/')) {
        showError(posterUrl, 'Путь должен начинаться с /assets/img/movies/');
        isValid = false;
    }

    if (!backgroundUrl.value.startsWith('/assets/img/movies/')) {
        showError(backgroundUrl, 'Путь должен начинаться с /assets/img/movies/');
        isValid = false;
    }

    if (actorPhotoUrl.value && !actorPhotoUrl.value.startsWith('/assets/img/actors/')) {
        showError(actorPhotoUrl, 'Путь должен начинаться с /assets/img/actors/');
        isValid = false;
    }

    return isValid;
}

// Добавьте эти обработчики событий
document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('addMovieForm');

    // Валидация при изменении полей
    form.releaseYear.addEventListener('change', validateMovieFields);
    form.duration.addEventListener('change', validateMovieFields);
    form.posterUrl.addEventListener('change', validateMovieFields);
    form.backgroundUrl.addEventListener('change', validateMovieFields);
    document.getElementById('actorPhotoUrl').addEventListener('change', validateMovieFields);
});

// Общие функции
function showError(element, message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-message';
    errorDiv.style.color = 'red';
    errorDiv.style.fontSize = '14px';
    errorDiv.style.marginTop = '5px';
    errorDiv.textContent = message;
    element.parentNode.appendChild(errorDiv);
    element.style.borderColor = 'red';
}

function clearErrors() {
    document.querySelectorAll('.error-message').forEach(e => e.remove());
    document.querySelectorAll('input').forEach(i => i.style.borderColor = '');
}