// Словарь жанров
const genreNames = {
    'biografii': 'Биографии',
    'boeviki': 'Боевики',
    'vesterny': 'Вестерны',
    'voennye': 'Военные',
    'detektivnye': 'Детективные',
    'dokumentalnye': 'Документальные',
    'dramy': 'Драмы',
    'istoricheskie': 'Исторические',
    'komedii': 'Комедии',
    'korotkometrazhnye': 'Короткометражные',
    'melodramy': 'Мелодрамы',
    'muzykalnye': 'Музыкальные',
    'multfilmy': 'Мультфильмы',
    'priklyucheniya': 'Приключения',
    'semejnye': 'Семейные',
    'sport': 'Спорт',
    'tv': 'ТВ',
    'trillery': 'Триллеры',
    'uzhasy': 'Ужасы',
    'fantastika': 'Фантастика',
    'fentezi': 'Фэнтези',
    'dc': 'DC',
    'marvel': 'Marvel',
    'netflix': 'Netflix',
    'brazilskie': 'Бразильские',
    'indijskie': 'Индийские',
    'ispanskie': 'Испанские',
    'kitajskie': 'Китайские',
    'korejskie': 'Корейские',
    'otechestvenye': 'Отечественые',
    'tureczkie': 'Турецкие',
    'yaponskie': 'Японские'
};

function renderGenres() {
    const genresContainer = document.getElementById('genresList');
    if (!genresContainer) return;

    const urlParams = new URLSearchParams(window.location.search);
    const currentGenre = urlParams.get('genre');
    const yearSort = urlParams.get('yearSort');

    const genresHtml = Object.entries(genreNames)
        .map(([slug, name]) => createGenreButton(slug, name, yearSort, currentGenre))
        .join('');

    genresContainer.innerHTML = genresHtml;
}

function createGenreButton(slug, name, yearSort, currentGenre) {
    const isActive = currentGenre === slug;
    const href = `/emblix?genre=${slug}${yearSort ? `&yearSort=${yearSort}` : ''}`;

    return `
        <a class="btn btn-genre d-flex flex-fill justify-content-between align-items-center mr-1 mb-1 text-left border-0 ${isActive ? 'btn-danger' : 'btn-light'}" 
           href="${href}" 
           data-genre="${slug}">
            ${name}
        </a>
    `;
}

document.addEventListener('DOMContentLoaded', function() {
    renderGenres();
    initializeEventHandlers();
    initializePageState();
});

function initializeEventHandlers() {
    // Обработчик изменения жанра
    document.querySelectorAll('.btn-genre').forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const genre = this.dataset.genre;
            const yearSort = document.getElementById('yearSortBtn')?.dataset.sort;

            // Формируем новый URL с жанром в параметрах
            const newUrl = new URL(window.location.href);
            newUrl.searchParams.set('genre', genre);
            if (yearSort) {
                newUrl.searchParams.set('yearSort', yearSort);
            } else {
                newUrl.searchParams.delete('yearSort');
            }

            history.pushState(null, '', newUrl);

            loadMovies(1, genre, yearSort);
            updateActiveGenre(genre);
            updateBreadcrumb(genre);
        });
    });

    // Обработчик сортировки по году
    const yearSortBtn = document.getElementById('yearSortBtn');
    if (yearSortBtn) {
        yearSortBtn.addEventListener('click', function() {
            const currentSort = this.dataset.sort || '';
            let newSort = !currentSort ? 'desc' :
                currentSort === 'desc' ? 'asc' : 'desc';

            const urlParams = new URLSearchParams(window.location.search);
            const currentGenre = urlParams.get('genre');

            const newUrl = new URL(window.location.href);
            if (newSort) {
                newUrl.searchParams.set('yearSort', newSort);
            } else {
                newUrl.searchParams.delete('yearSort');
            }

            history.pushState(null, '', newUrl);

            loadMovies(1, currentGenre, newSort);
            this.dataset.sort = newSort;
            updateSortButtonState(this, newSort);
        });
    }

    // Обработчик пагинации
    document.querySelector('.pager')?.addEventListener('click', function(e) {
        if (e.target.matches('.btn-page')) {
            e.preventDefault();
            const page = e.target.dataset.page;
            const urlParams = new URLSearchParams(window.location.search);
            const currentGenre = urlParams.get('genre');
            const yearSort = document.getElementById('yearSortBtn')?.dataset.sort;

            const newUrl = new URL(window.location.href);
            newUrl.searchParams.set('page', page);
            history.pushState(null, '', newUrl);

            loadMovies(page, currentGenre, yearSort);
        }
    });
}

function initializePageState() {
    const urlParams = new URLSearchParams(window.location.search);
    const currentGenre = urlParams.get('genre');
    const yearSort = urlParams.get('yearSort');

    if (currentGenre) {
        updateActiveGenre(currentGenre);
        updateBreadcrumb(currentGenre);
    }

    if (yearSort) {
        const yearSortBtn = document.getElementById('yearSortBtn');
        if (yearSortBtn) {
            yearSortBtn.dataset.sort = yearSort;
            updateSortButtonState(yearSortBtn, yearSort);
        }
    }
}

function loadMovies(page = 1, genre = null, yearSort = null) {
    const url = new URL('/emblix', window.location.origin);
    url.searchParams.set('page', page);
    if (genre) url.searchParams.set('genre', genre);
    if (yearSort) url.searchParams.set('yearSort', yearSort);

    fetch(url.toString(), {
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.error) {
                console.error(data.error);
                return;
            }
            updateMovieGrid(data.movies);
            updatePagination(data.currentPage, data.totalPages);
        })
        .catch(error => console.error('Error:', error));
}

function updateMovieGrid(movies) {
    const container = document.querySelector('.movie-grid');
    if (!container) return;

    container.innerHTML = movies.map(movie => `
        <div class="col col-movie">
            <div class="movie-item mb-3">
                <figure class="figure w-100">
                    <a href="/movie?id=${movie.id}">
                        <img class="figure-img img-fluid shadow border-0 w-100 fade show" 
                             style="border-radius: 0.8rem;" 
                             alt="Фильм ${movie.title} online на emblix" 
                             title="${movie.title} в качестве 720, 1080 Hd на Эмбликс"
                             src="/${movie.posterUrl}"
                             loading="lazy">
                    </a>
                    <figcaption class="figure-caption ml-2 mr-1">
                        <div class="movie-item-title">
                            ${movie.rating ? `
                            <span class="float-right font-weight-bold ml-1 pointer ${movie.isHD ? 'text-success' : 'text-danger'}" 
                                  title="${movie.isHD ? 'HD' : 'ЭКРАНКА'}">
                                ${movie.rating}
                            </span>` : ''}
                            <a class="text-body font-weight-bolder text-decoration-none" 
                               href="/movie?id=${movie.id}">
                                ${movie.title}
                            </a>
                        </div>
                        <small>${movie.releaseYear}, ${movie.movieGenres.map(mg => mg.genre.name).join(', ')}</small>
                    </figcaption>
                </figure>
            </div>
        </div>
    `).join('');
}

function updatePagination(currentPage, totalPages) {
    const pager = document.querySelector('.pager');
    if (!pager) return;

    let html = '';
    for (let i = 1; i <= totalPages; i++) {
        html += `<a class="btn btn-page flex-fill align-items-center ${i === currentPage ? 'btn-danger' : 'btn-dark'}" 
                   data-page="${i}"><strong>${i}</strong></a>`;
    }
    pager.innerHTML = html;
}

function updateActiveGenre(genre) {
    document.querySelectorAll('.btn-genre').forEach(btn => {
        if (btn.dataset.genre === genre) {
            btn.classList.remove('btn-light');
            btn.classList.add('btn-danger');
        } else {
            btn.classList.remove('btn-danger');
            btn.classList.add('btn-light');
        }
    });
}

function updateSortButtonState(button, sort) {
    const icon = button.querySelector('i');
    if (!icon) return;

    icon.className = 'fas fa-fw fa-sort-numeric-down-alt fa-lg';
    button.classList.remove('text-danger', 'text-muted');

    if (sort === 'desc') {
        icon.className = 'fas fa-fw fa-sort-numeric-down fa-lg';
        button.classList.add('text-danger');
    } else if (sort === 'asc') {
        icon.className = 'fas fa-fw fa-sort-numeric-up fa-lg';
        button.classList.add('text-danger');
    } else {
        button.classList.add('text-muted');
    }
}

function updateBreadcrumb(genre) {
    const breadcrumb = document.getElementById('genre-breadcrumb');
    if (!breadcrumb) return;

    if (genre && genreNames[genre]) {
        breadcrumb.textContent = genreNames[genre];
        breadcrumb.style.display = 'block';

        // Обновляем URL в хлебных крошках
        const genreLink = document.querySelector('.breadcrumb-item a[href="/emblix"]');
        if (genreLink) {
            genreLink.href = `/emblix?genre=${genre}`;
        }
    } else {
        breadcrumb.style.display = 'none';
    }
}

function formatMovieData(movie) {
    return {
        ...movie,
        isHD: movie.quality === 'HD' || movie.quality === 'WEB-DL',
        rating: movie.rating ? movie.rating.toFixed(1) : null
    };
}

async function getMovies(params = {}) {
    const queryString = new URLSearchParams(params).toString();
    const response = await fetch(`/api/movies?${queryString}`);
    const data = await response.json();

    return {
        ...data,
        movies: data.movies.map(formatMovieData)
    };
}