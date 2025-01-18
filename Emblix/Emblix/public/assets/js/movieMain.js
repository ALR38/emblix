document.addEventListener('DOMContentLoaded', function() {
    initializeReviewForm();
    initializePlayerControls();
});

function initializeReviewForm() {
    const reviewForm = document.getElementById('reviewForm');
    const reviewText = document.getElementById('reviewText');
    const charCount = document.getElementById('charCount');
    const ratingButtons = document.querySelectorAll('.rating-btn');

    // Счетчик символов
    if (reviewText && charCount) {
        const maxLength = parseInt(reviewText.getAttribute('maxlength') || '500');

        reviewText.addEventListener('input', function() {
            const length = this.value.length;
            charCount.textContent = `${length}/${maxLength}`;
        });
    }

    // Обработка рейтинга
    if (ratingButtons) {
        ratingButtons.forEach(btn => {
            btn.addEventListener('click', function() {
                const rating = this.dataset.rating;
                document.getElementById('reviewRating').value = rating;

                // Обновляем визуальное состояние кнопок
                ratingButtons.forEach(b => b.classList.remove('active'));
                this.classList.add('active');
            });
        });
    }

    // Отправка формы
    if (reviewForm) {
        reviewForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            const formData = new FormData(this);

            // Проверяем обязательные поля
            if (!formData.get('content').trim()) {
                showError('Пожалуйста, напишите текст отзыва');
                return;
            }

            if (!formData.get('authorName').trim()) {
                showError('Пожалуйста, укажите ваше имя');
                return;
            }

            try {
                const response = await fetch('/api/reviews', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        movieId: formData.get('movieId'),
                        content: formData.get('content'),
                        authorName: formData.get('authorName'),
                        rating: formData.get('rating') || null
                    })
                });

                if (response.ok) {
                    const data = await response.json();
                    showSuccess('Спасибо за ваш отзыв! Он появится после проверки модератором.');
                    resetReviewForm();
                } else {
                    const error = await response.json();
                    showError(error.message || 'Ошибка при отправке отзыва');
                }
            } catch (error) {
                console.error('Ошибка:', error);
                showError('Произошла ошибка при отправке отзыва');
            }
        });
    }
}

function resetReviewForm() {
    const form = document.getElementById('reviewForm');
    if (!form) return;

    form.reset();
    document.querySelectorAll('.rating-btn').forEach(btn => btn.classList.remove('active'));
    document.getElementById('charCount').textContent = '0/500';
}

function initializePlayerControls() {
    // Переключение качества видео
    document.querySelectorAll('.quality-btn').forEach(button => {
        button.addEventListener('click', function() {
            const quality = this.dataset.quality;
            const playerUrl = this.dataset.url;

            // Обновляем активную кнопку
            document.querySelectorAll('.quality-btn').forEach(btn =>
                btn.classList.remove('active'));
            this.classList.add('active');

            // Обновляем URL плеера
            const activePlayer = document.querySelector('.player-item.active iframe');
            if (activePlayer) {
                activePlayer.src = playerUrl;
            }
        });
    });
}

function showError(message) {
    const alertElement = document.createElement('div');
    alertElement.className = 'alert alert-danger alert-dismissible fade show';
    alertElement.innerHTML = `
        ${message}
        <button type="button" class="close" data-dismiss="alert">
            <span>&times;</span>
        </button>
    `;

    showAlert(alertElement);
}

function showSuccess(message) {
    const alertElement = document.createElement('div');
    alertElement.className = 'alert alert-success alert-dismissible fade show';
    alertElement.innerHTML = `
        ${message}
        <button type="button" class="close" data-dismiss="alert">
            <span>&times;</span>
        </button>
    `;

    showAlert(alertElement);
}

function showAlert(alertElement) {
    const container = document.querySelector('.review-form-container');
    if (container) {
        // Удаляем предыдущие алерты
        container.querySelectorAll('.alert').forEach(alert => alert.remove());
        // Добавляем новый алерт
        container.insertBefore(alertElement, container.firstChild);

        // Автоматически скрываем через 5 секунд
        setTimeout(() => {
            alertElement.classList.remove('show');
            setTimeout(() => alertElement.remove(), 150);
        }, 5000);
    }
}