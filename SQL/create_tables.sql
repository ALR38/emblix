CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY(1,1),
    email NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(50) NOT NULL,
    isAdmin BIT DEFAULT 0
);

CREATE TABLE genres (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL UNIQUE,
    slug NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE movies (
    id INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(255) NOT NULL,
    originalTitle NVARCHAR(255),
    slug NVARCHAR(255) NOT NULL UNIQUE,
    description NVARCHAR(MAX),
    releaseYear INT,
    country NVARCHAR(100),
    duration INT,
    posterUrl NVARCHAR(255),
    backgroundUrl NVARCHAR(255),
    kinopoiskUrl NVARCHAR(255),
    imdbUrl NVARCHAR(255),
    playerUrls NVARCHAR(MAX),
    rating DECIMAL(3, 1) NULL
    views INT DEFAULT 0,
    likes INT DEFAULT 0
);

CREATE TABLE movieGenres (
    movieId INT,
    genreId INT,
    PRIMARY KEY (movieId, genreId),
    FOREIGN KEY (movieId) REFERENCES movies(id) ON DELETE CASCADE,
    FOREIGN KEY (genreId) REFERENCES genres(id) ON DELETE CASCADE
);

CREATE TABLE actors (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(255) NOT NULL,
    photoUrl NVARCHAR(255)
);

CREATE TABLE movieActors (
    movieId INT,
    actorId INT,
    role NVARCHAR(255),
    PRIMARY KEY (movieId, actorId),
    FOREIGN KEY (movieId) REFERENCES movies(id) ON DELETE CASCADE,
    FOREIGN KEY (actorId) REFERENCES actors(id) ON DELETE CASCADE
);

CREATE TABLE userFavorites (
    userId INT,
    movieId INT,
    isFavorite BIT DEFAULT 0,
    isSubscribed BIT DEFAULT 0,
    PRIMARY KEY (userId, movieId),
    FOREIGN KEY (userId) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (movieId) REFERENCES movies(id) ON DELETE CASCADE
);

CREATE TABLE reviews (
    id INT PRIMARY KEY IDENTITY(1,1),
    movieId INT,
    rating INT CHECK (rating >= 0 AND rating <= 10),
    content NVARCHAR(MAX),
    authorName NVARCHAR(255),
    isApproved BIT DEFAULT 0,
    FOREIGN KEY (movieId) REFERENCES movies(id) ON DELETE CASCADE
);