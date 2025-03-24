-- Criar o banco de dados com suporte total a caracteres especiais
-- CREATE DATABASE tournstats CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
-- USE tournstats;

-- Tabela de Times
CREATE TABLE Times (
    id UUID PRIMARY KEY,
    nome VARCHAR(60) NOT NULL,
    dataCriado TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Jogadores (Players)
CREATE TABLE Players (
    id UUID PRIMARY KEY,
    nome VARCHAR(60) NOT NULL,
    numero TINYINT UNSIGNED CHECK (numero BETWEEN 1 AND 99),
    teamId UUID,
    FOREIGN KEY (teamId) REFERENCES Times(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabela de Personagens (Characters)
CREATE TABLE Characters (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(60) NOT NULL,
    imagePath VARCHAR(255)
);

-- Tabela de Pets
CREATE TABLE Pets (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(60) NOT NULL,
    imagePath VARCHAR(255)
);

-- Tabela de Ataques (Attacks)
CREATE TABLE Attacks (
    id UUID PRIMARY KEY,
    playerId UUID NOT NULL,
    teamAdversario UUID NOT NULL,
    estrelas TINYINT UNSIGNED CHECK (estrelas BETWEEN 1 AND 5),
    dataAtaque TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (playerId) REFERENCES Players(id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (teamAdversario) REFERENCES Times(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabela de relação entre Ataques e Personagens
CREATE TABLE AttacksCharacters (
    attackId UUID NOT NULL,
    characterId INT NOT NULL,
    PRIMARY KEY (attackId, characterId),
    FOREIGN KEY (attackId) REFERENCES Attacks(id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (characterId) REFERENCES Characters(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabela de relação entre Ataques e Pets
CREATE TABLE AttacksPets (
    attackId UUID NOT NULL,
    petId INT NOT NULL,
    PRIMARY KEY (attackId, petId),
    FOREIGN KEY (attackId) REFERENCES Attacks(id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (petId) REFERENCES Pets(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabela de Defesas (Defenses)
CREATE TABLE Defenses (
    id UUID PRIMARY KEY,
    playerId UUID NOT NULL,
    dataDefendido TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (playerId) REFERENCES Players(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabela de relação entre Defesas e Personagens
CREATE TABLE DefenseCharacters (
    defenseId UUID NOT NULL,
    characterId INT NOT NULL,
    hide BOOLEAN DEFAULT FALSE,
    ban BOOLEAN DEFAULT FALSE,
    PRIMARY KEY (defenseId, characterId),
    FOREIGN KEY (defenseId) REFERENCES Defenses(id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (characterId) REFERENCES Characters(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabela de relação entre Defesas e Pets
CREATE TABLE DefensePets (
    defenseId UUID NOT NULL,
    petId INT NOT NULL,
    PRIMARY KEY (defenseId, petId),
    FOREIGN KEY (defenseId) REFERENCES Defenses(id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (petId) REFERENCES Pets(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Tabela de Resultados (Results)
CREATE TABLE Results (
    id INT PRIMARY KEY AUTO_INCREMENT,
    timeVencedor UUID NOT NULL,
    timePerdedor UUID NOT NULL,
    maxEstrelas INT CHECK (maxEstrelas >= 0),
    dataFinal TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (timeVencedor) REFERENCES Times(id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (timePerdedor) REFERENCES Times(id) ON DELETE CASCADE ON UPDATE CASCADE
);

-- Adicionar Times
INSERT INTO tournstats.times
(id, nome, dataCriado)
VALUES(UUID(), 'Pedro', current_timestamp());

-- Adicionar Jogadores
INSERT INTO tournstats.players
(id, nome, numero, teamId)
VALUES(UUID(), 'Rodolfo', 1, (select id from tournstats.times where nome='Pedro' limit 1));

-- Adicionar Personagens
INSERT INTO tournstats.`characters`
(id, nome, imagePath)
VALUES(5, 'Mariele', NULL);

-- Adicionar Pets
INSERT INTO tournstats.pets
(id, nome, imagePath)
VALUES(0, 'Arme T', NULL);

-- Adicionar Ataques
INSERT INTO tournstats.attacks (id, playerId, teamAdversario, estrelas, dataAtaque)
SELECT UUID(), 
       p.id, 
       t.id, 
       3, 
       CURRENT_TIMESTAMP()
FROM tournstats.players p
JOIN tournstats.times t ON t.nome = 'Maria'
WHERE p.nome = 'Rodolfo'
LIMIT 1;

INSERT INTO tournstats.attackscharacters (attackId, characterId)
VALUES (
    '08457288-08aa-11f0-9e77-d027885d804c',  -- Substitua pelo ID real do ataque
    5-- Substitua pelo ID real do personagem
);


CREATE TRIGGER before_insert_AttacksCharacters
BEFORE INSERT ON AttacksCharacters
FOR EACH ROW
BEGIN
    DECLARE total INT;
    
    SELECT COUNT(*) INTO total 
    FROM AttacksCharacters 
    WHERE attackId = NEW.attackId;
    
    IF total >= 4 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Limite de 4 personagens por ataque atingido';
    END IF;
END 