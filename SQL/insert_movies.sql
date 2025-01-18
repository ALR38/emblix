-- Добавление фильма
INSERT INTO movies (
    title, 
    originalTitle, 
    slug, 
    description, 
    releaseYear, 
    country, 
    duration, 
    posterUrl, 
    backgroundUrl, 
    kinopoiskUrl, 
    imdbUrl, 
    playerUrls, 
    views, 
    likes,
    rating
) VALUES (
    N'Финишная черта',
    'The Finnish Line',
    'finishnaya-cherta',
    N'Аня пошла по стопам своего отца, который участвовал в гонках на собачьих упряжках. В Финляндии она участвует в в соревнованиях против бывшего соперника своего отца Монти. Её цель сделать то что не смог сделать её отец - добиться победы в гонке.',
    2024,
    N'США',
    94,
    '/assets/img/movies/finish.jpg',
    '/assets/img/movies/finish.jpg',
    'https://www.kinopoisk.ru/film/6651879/',
    'https://www.imdb.com/title/tt31841975/',
    'https://vk.com/video_ext.php?oid=-205272696&id=456241495&hd=2&autoplay=1',
    0,
    0,
    0.0
);

DECLARE @movieId int = SCOPE_IDENTITY();

-- Добавление связей с жанрами
INSERT INTO movieGenres (movieId, genreId) VALUES 
(@movieId, 14), -- Приключения
(@movieId, 9),  -- Комедии
(@movieId, 11); -- Мелодрамы

-- Добавление актера и его связи с фильмом
INSERT INTO actors (name, photoUrl) VALUES 
(N'Кимбер Матула', '/assets/img/actors/matula.jpg');

DECLARE @actorId int = SCOPE_IDENTITY();

INSERT INTO movieActors (movieId, actorId, role) VALUES 
(@movieId, @actorId, N'Anya');






-- Добавление фильма
INSERT INTO movies (
    title, 
    originalTitle, 
    slug, 
    description, 
    releaseYear, 
    country, 
    duration, 
    posterUrl, 
    backgroundUrl, 
    kinopoiskUrl, 
    imdbUrl, 
    playerUrls, 
    views, 
    likes,
    rating
) VALUES (
    N'Оппенгеймер',
    'Oppenheimer',
    'oppengeymer',
    N'История жизни американского физика Роберта Оппенгеймера, который стоял во главе первых разработок ядерного оружия.',
    2023,
    N'США',
    180,
    '/assets/img/movies/oppenh.jpg',
    '/assets/img/movies/oppenh.jpg',
    'https://www.kinopoisk.ru/film/4664634/',
    'https://www.imdb.com/title/tt15398776/',
    'https://vk.com/video_ext.php?oid=-220018529&id=456241609&hd=4&autoplay=1',
    0,
    0,
    8.0
);

DECLARE @movieId int = SCOPE_IDENTITY();

-- Добавление связей с жанрами
INSERT INTO movieGenres (movieId, genreId) VALUES 
(@movieId, 1),  -- Биографии
(@movieId, 7),  -- Драмы
(@movieId, 8);  -- Исторические

-- Добавление актера
INSERT INTO actors (name, photoUrl) VALUES 
(N'Киллиан Мёрфи', '/assets/img/actors/killian.jpg');

DECLARE @actorId int = SCOPE_IDENTITY();

-- Связываем актера с фильмом
INSERT INTO movieActors (movieId, actorId, role) VALUES 
(@movieId, @actorId, N'Роберт Оппенгеймер');


-- Добавление фильма
INSERT INTO movies (
    title, 
    originalTitle, 
    slug, 
    description, 
    releaseYear, 
    country, 
    duration, 
    posterUrl, 
    backgroundUrl, 
    kinopoiskUrl, 
    imdbUrl, 
    playerUrls, 
    views, 
    likes,
    rating
) VALUES (
    N'Дэдпул и Росомаха',
    'Deadpool & Wolverine',
    'deadpool-and-wolverine',
    N'Уэйд Уилсон попадает в организацию «Управление временными изменениями», что вынуждает его вернуться к своему альтер-эго Дэдпулу и изменить историю с помощью Росомахи.',
    2024,
    N'США',
    128,
    '/assets/img/movies/rosomaxa.jpg',
    '/assets/img/movies/rosomaxa.jpg',
    'https://www.kinopoisk.ru/film/1008444/',
    'https://www.imdb.com/title/tt6263850/',
    'https://vk.com/video_ext.php?oid=-96827552&id=456241975&hd=3&autoplay=1',
    0,
    0,
    0.0
);

DECLARE @movieId int = SCOPE_IDENTITY();

-- Добавление связей с жанрами
INSERT INTO movieGenres (movieId, genreId) VALUES 
(@movieId, 20), -- Фантастика
(@movieId, 2),  -- Боевик
(@movieId, 9);  -- Комедия

-- Добавление актеров
INSERT INTO actors (name, photoUrl) VALUES 
(N'Райан Рейнольдс', '/assets/img/actors/rayanR.jpg'),
(N'Хью Джекман', '/assets/img/actors/HuyJ.jpg');

-- Связываем актеров с фильмом
INSERT INTO movieActors (movieId, actorId, role) VALUES 
(@movieId, (SELECT id FROM actors WHERE name = N'Райан Рейнольдс'), N'Deadpool'),
(@movieId, (SELECT id FROM actors WHERE name = N'Хью Джекман'), N'Wolverine');


-- Добавление фильма
INSERT INTO movies (
    title, 
    originalTitle, 
    slug, 
    description, 
    releaseYear, 
    country, 
    duration, 
    posterUrl, 
    backgroundUrl, 
    kinopoiskUrl, 
    imdbUrl, 
    playerUrls, 
    views, 
    likes,
    rating
) VALUES (
    N'Стражи Галактики. Часть 3',
    'Guardians of the Galaxy Vol. 3',
    'guardians-of-the-galaxy-3',
    N'Питер Квилл никак не может смириться с потерей Гаморы и теперь вместе со Стражами Галактики вынужден отправиться на очередную миссию по защите Вселенной.',
    2023,
    N'США',
    150,
    '/assets/img/movies/galact.jpg',
    '/assets/img/movies/galact.jpg',
    'https://www.kinopoisk.ru/film/1044280/',
    'https://www.imdb.com/title/tt6791350/',
    'https://vk.com/video_ext.php?oid=-125159885&id=456240211&hd=4&autoplay=1',
    0,
    0,
    7.9
);

DECLARE @movieId int = SCOPE_IDENTITY();

-- Добавление связей с жанрами
INSERT INTO movieGenres (movieId, genreId) VALUES 
(@movieId, 20), -- Фантастика
(@movieId, 2),  -- Боевик
(@movieId, 18), -- Триллер
(@movieId, 9),  -- Комедия
(@movieId, 14); -- Приключения

-- Добавление актера
INSERT INTO actors (name, photoUrl) VALUES 
(N'Дэйв Батиста', '/assets/img/actors/batista.jpg');

-- Связываем актера с фильмом
INSERT INTO movieActors (movieId, actorId, role) VALUES 
(@movieId, (SELECT id FROM actors WHERE name = N'Дэйв Батиста'), N'Drax');



-- Добавление фильма
INSERT INTO movies (
    title, 
    originalTitle, 
    slug, 
    description, 
    releaseYear, 
    country, 
    duration, 
    posterUrl, 
    backgroundUrl, 
    kinopoiskUrl, 
    imdbUrl, 
    playerUrls, 
    views, 
    likes,
    rating
) VALUES (
    N'Гавайи',
    'Hawaii',
    'hawaii',
    N'На Гавайях, когда внезапно раздалось оповещение о предстоящем ракетном нападении, группа отдыхающих друзей, ожидая неизбежного, решает высказать друг другу все недовольства и замечания, которые они держали в себе. Но, как оказалось, оповещение о ракетной угрозе оказалось ложным тревожным сигналом. Подруга обнаружила, что у них впереди еще целая неделя отпуска в этом райском месте.',
    2023,
    N'Франция',
    104,
    '/assets/img/movies/hawai.jpg',
    '/assets/img/movies/hawai.jpg',
    'https://www.kinopoisk.ru/film/4672943/',
    'https://www.imdb.com/title/tt14956076/',
    'https://vk.com/video_ext.php?oid=-144935677&id=456263062&hd=4&autoplay=1',
    0,
    0,
    5.0
);

DECLARE @movieId int = SCOPE_IDENTITY();

-- Добавление связей с жанрами
INSERT INTO movieGenres (movieId, genreId) VALUES 
(@movieId, 9);  -- Комедия

-- Добавление актера
INSERT INTO actors (name, photoUrl) VALUES 
(N'Элоди Буше', '/assets/img/actors/elodi.jpg');

-- Связываем актера с фильмом
INSERT INTO movieActors (movieId, actorId, role) VALUES 
(@movieId, (SELECT id FROM actors WHERE name = N'Элоди Буше'), N'Ines');




-- Добавление фильма
INSERT INTO movies (
    title, 
    originalTitle, 
    slug, 
    description, 
    releaseYear, 
    country, 
    duration, 
    posterUrl, 
    backgroundUrl, 
    kinopoiskUrl, 
    imdbUrl, 
    playerUrls, 
    views, 
    likes,
    rating
) VALUES (
    N'Железный человек 2',
    'Iron Man 2',
    'iron-man-2',
    N'Прошло полгода с тех пор, как мир узнал, что миллиардер-изобретатель Тони Старк является обладателем уникальной кибер-брони Железного человека. Общественность требует, чтобы Старк передал технологию брони правительству США, но Тони не хочет разглашать её секреты, потому что боится, что она попадёт не в те руки. Между тем Иван Ванко — сын русского учёного, когда-то работавшего на фирму Старка, но потом уволенного и лишенного всего, намерен отомстить Тони за беды своей семьи. Для чего сооружает своё высокотехнологичное оружие.',
    2010,
    N'США',
    124,
    '/assets/img/movies/iron.jpg',
    '/assets/img/movies/iron.jpg',
    'https://www.kinopoisk.ru/film/411924/',
    'https://www.imdb.com/title/tt1228705/',
    'https://vk.com/video_ext.php?oid=-219647317&id=456241942',
    0,
    0,
    6.9
);

DECLARE @movieId int = SCOPE_IDENTITY();

-- Добавление связей с жанрами
INSERT INTO movieGenres (movieId, genreId) VALUES 
(@movieId, 20), -- Фантастика
(@movieId, 2);  -- Боевик

-- Добавление актера
INSERT INTO actors (name, photoUrl) VALUES 
(N'Роберт Дауни мл.', '/assets/img/actors/downey.jpg');

-- Связываем актера с фильмом
INSERT INTO movieActors (movieId, actorId, role) VALUES 
(@movieId, (SELECT id FROM actors WHERE name = N'Роберт Дауни мл.'), N'Tony Stark');

-- Добавление фильма
INSERT INTO movies (
    title, 
    originalTitle, 
    slug, 
    description, 
    releaseYear, 
    country, 
    duration, 
    posterUrl, 
    backgroundUrl, 
    kinopoiskUrl, 
    imdbUrl, 
    playerUrls, 
    views, 
    likes,
    rating
) VALUES (
    N'Кибердеревня. Новый год',
    N'Кибердеревня. Новый год',
    'kiberderevnya-novyy-god',
    N'31 декабря 2099 года по всей Солнечной системе накрывают столы и наряжают киберёлки, а марсианский фермер Николай ждёт к себе в гости с Земли жену и двух дочерей. Но что делать, если из-за аномальной солнечной активности отменяют все межпланетные перелёты? Новый год — праздник семейный, и Николай решает во что бы то ни стало добраться до родных. Помогут ему в этом любимая «буханка» Буля, самодельный телепорт и портативная чёрная дыра.',
    2023,
    N'Россия',
    253,
    '/assets/img/movies/derevnya.jpg',
    '/assets/img/movies/derevnya.jpg',
    'https://www.kinopoisk.ru/film/5413371/',
    'https://www.imdb.com/title/tt30267167/',
    'https://vk.com/video_ext.php?oid=-218827932&id=456240349&hd=4&autoplay=1',
    0,
    0,
    6.4
);

DECLARE @movieId int = SCOPE_IDENTITY();

-- Добавление связей с жанрами
INSERT INTO movieGenres (movieId, genreId) VALUES 
(@movieId, 20), -- Фантастика
(@movieId, 10), -- Короткометражный
(@movieId, 9);  -- Комедия

-- Добавление актера
INSERT INTO actors (name, photoUrl) VALUES 
(N'Сергей Чихачёв', '/assets/img/actors/chicha.jpg');

-- Связываем актера с фильмом
INSERT INTO movieActors (movieId, actorId, role) VALUES 
(@movieId, (SELECT id FROM actors WHERE name = N'Сергей Чихачёв'), N'Серега');





-- Добавление фильма
INSERT INTO movies (
    title, 
    originalTitle, 
    slug, 
    description, 
    releaseYear, 
    country, 
    duration, 
    posterUrl, 
    backgroundUrl, 
    kinopoiskUrl, 
    imdbUrl, 
    playerUrls, 
    views, 
    likes,
    rating
) VALUES (
    N'Паук внутри: История Паутины Вселенных',
    'The Spider Within: A Spider-Verse Story',
    'spider-within-story',
    N'Майлз Моралес испытывает приступ паники, который заставляет его понять, что обращение за помощью может быть таким же смелым поступком, как защита города от зла.',
    2023,
    N'США',
    27,
    '/assets/img/movies/pavuk.jpg',
    '/assets/img/movies/pavuk.jpg',
    'https://www.kinopoisk.ru/film/5362036/',
    'https://www.imdb.com/title/tt27369002/',
    'https://vk.com/video_ext.php?oid=-184447514&id=456246606&hd=4&autoplay=1',
    0,
    0,
    7.1
);

DECLARE @movieId int = SCOPE_IDENTITY();

-- Добавление связей с жанрами
INSERT INTO movieGenres (movieId, genreId) VALUES 
(@movieId, 19), -- Ужасы
(@movieId, 20), -- Фантастика
(@movieId, 10), -- Короткометражный
(@movieId, 18); -- Триллер

-- Добавление актера
INSERT INTO actors (name, photoUrl) VALUES 
(N'Шамеик Мур', '/assets/img/actors/mur.jpg');

-- Связываем актера с фильмом
INSERT INTO movieActors (movieId, actorId, role) VALUES 
(@movieId, (SELECT id FROM actors WHERE name = N'Шамеик Мур'), N'Miles Morales');

