-- Жанры
INSERT INTO genres (name, slug) VALUES
(N'Биографии', 'biografii'),
(N'Боевики', 'boeviki'),
(N'Вестерны', 'vesterny'),
(N'Военные', 'voennye'),
(N'Детективные', 'detektivnye'),
(N'Документальные', 'dokumentalnye'),
(N'Драмы', 'dramy'),
(N'Исторические', 'istoricheskie'),
(N'Комедии', 'komedii'),
(N'Короткометражные', 'korotkometrazhnye'),
(N'Мелодрамы', 'melodramy'),
(N'Музыкальные', 'muzykalnye'),
(N'Мультфильмы', 'multfilmy'),
(N'Приключения', 'priklyucheniya'),
(N'Семейные', 'semejnye'),
(N'Спорт', 'sport'),
(N'ТВ', 'tv'),
(N'Триллеры', 'trillery'),
(N'Ужасы', 'uzhasy'),
(N'Фантастика', 'fantastika'),
(N'Фэнтези', 'fentezi'),
(N'DC', 'dc'),
(N'Marvel', 'marvel'),
(N'Netflix', 'netflix'),
(N'Бразильские', 'brazilskie'),
(N'Индийские', 'indijskie'),
(N'Испанские', 'ispanskie'),
(N'Китайские', 'kitajskie'),
(N'Корейские', 'korejskie'),
(N'Отечественые', 'otechestvenye'),
(N'Турецкие', 'tureczkie'),
(N'Японские', 'yaponskie');
GO



-- Пользователи
INSERT INTO Users (Email, Password, IsAdmin)
VALUES ('admin@gmail.com', 'admin123', 1);

INSERT INTO Users (Email, Password, IsAdmin)
VALUES ('user@gmail.com', 'user1234', 0);