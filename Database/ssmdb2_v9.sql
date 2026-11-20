-- =====================================================================
-- SSMDB2 - Datenbankversion 9
-- Quelle: "Beschreibung der Scheibenablage in der Meyton-DB (SSMDB2)", V9
--
-- Zweck: Testdatenbank fuer ShotPost. Das Skript legt eine EIGENE Datenbank
-- `ssmdb2_v9` an und beruehrt die echte Meyton-Datenbank `SSMDB2` nie.
-- ACHTUNG: Tabellen werden vorher gedroppt (nur in dieser Testdatenbank).
--
-- Ausfuehren:  mysql -u root -p < ssmdb2_v9.sql
-- Hinweis: NOT-NULL/Default-Angaben sind angenommen (die PDF nennt nur Typen
-- und Schluessel). Alle Nicht-Schluesselspalten sind daher NULL-faehig.
-- =====================================================================

CREATE DATABASE IF NOT EXISTS `ssmdb2_v9` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `ssmdb2_v9`;

DROP TABLE IF EXISTS `Treffer`;
DROP TABLE IF EXISTS `Serien`;
DROP TABLE IF EXISTS `Scheiben`;

-- ---------------------------------------------------------------------
-- Scheiben: Grunddaten der Scheibe (PK: ScheibenID)
-- ---------------------------------------------------------------------
CREATE TABLE `Scheiben` (
  `ScheibenID`       INT          NOT NULL,
  `Starterliste`     VARCHAR(36)  NULL,
  `StarterlistenID`  INT          NULL,
  `Startzeit`        DATETIME     NULL,
  `StartNr`          INT          NULL,
  `Nachname`         VARCHAR(36)  NULL,
  `Vorname`          VARCHAR(36)  NULL,
  `Geschlecht`       ENUM('male','female','unknown') NULL,
  `Land`             VARCHAR(3)   NULL,
  `SportpassID`      BIGINT         NULL,
  `StandNr`          SMALLINT     NULL,
  `StandNrText`      VARCHAR(36)  NULL,
  `Disziplin`        VARCHAR(36)  NULL,
  `DisziplinID`      INT          NULL,
  `Klasse`           VARCHAR(36)  NULL,
  `KlassenID`        INT          NULL,
  `Verein`           VARCHAR(36)  NULL,
  `VereinsID`        INT          NULL,
  `Mannschaft`       VARCHAR(36)  NULL,
  `MannschaftsID`    INT          NULL,
  `Rangliste`        VARCHAR(36)  NULL,
  `RanglistenID`     INT          NULL,
  `Gruppe`           VARCHAR(36)  NULL,
  `GruppenID`        INT          NULL,
  `Status`           VARCHAR(72)  NULL,
  `Trefferzahl`      SMALLINT     NULL,
  `TotalRing`        INT          NULL,
  `TotalRing01`      INT          NULL,
  `BesterTeiler01`   INT          NULL,
  `BesterTeiler001`  INT          NULL,
  `Zeitstempel`      DATETIME     NULL,
  PRIMARY KEY (`ScheibenID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- Serien: Serienergebnisse (PK: ScheibenID, Stellung, Serie)
-- ---------------------------------------------------------------------
CREATE TABLE `Serien` (
  `ScheibenID`  INT       NOT NULL,
  `Stellung`    SMALLINT  NOT NULL,
  `Serie`       SMALLINT  NOT NULL,
  `Ring`        INT       NULL,
  `Ring01`      INT       NULL,
  PRIMARY KEY (`ScheibenID`, `Stellung`, `Serie`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- Treffer: Wertungstreffer (PK: ScheibenID, Stellung, Treffer)
-- x/y in 1/100 mm, Zentrum = Scheibenmittelpunkt
-- ---------------------------------------------------------------------
CREATE TABLE `Treffer` (
  `ScheibenID`    INT          NOT NULL,
  `Stellung`      SMALLINT     NOT NULL,                -- ab V8: 1..n, 0 = Probestellung
  `Treffer`       SMALLINT     NOT NULL,
  `Serie`         SMALLINT     NULL,
  `x`             INT          NULL,
  `y`             INT          NULL,
  `Innenzehner`   TINYINT(1)   NULL,
  `Wertung`       INT          NULL,                    -- Wertungsmodus 0..12
  `Text`          VARCHAR(36)  NULL,
  `Ring`          INT          NULL,
  `Ring01`        INT          NULL,
  `Teiler01`      INT          NULL,
  `Teiler001`     INT          NULL,
  `Zeitstempel`   DATETIME     NULL,
  `Millisekunden` SMALLINT     NULL,
  PRIMARY KEY (`ScheibenID`, `Stellung`, `Treffer`),
  CONSTRAINT `fk_treffer_scheibe` FOREIGN KEY (`ScheibenID`) REFERENCES `Scheiben` (`ScheibenID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

ALTER TABLE `Serien`
  ADD CONSTRAINT `fk_serien_scheibe` FOREIGN KEY (`ScheibenID`) REFERENCES `Scheiben` (`ScheibenID`) ON DELETE CASCADE;

-- ---------------------------------------------------------------------
-- Testdaten: 1 Scheibe (KK 50m liegend, DisziplinID 40180060), 3 Treffer
-- ---------------------------------------------------------------------
INSERT INTO `Scheiben` (
  `ScheibenID`, `Starterliste`, `StarterlistenID`, `Startzeit`, `StartNr`,
  `Nachname`, `Vorname`, `Geschlecht`, `Land`, `SportpassID`,
  `StandNr`,
  `StandNrText`,
  `Disziplin`, `DisziplinID`, `Klasse`, `KlassenID`, `Verein`, `VereinsID`,
  `Mannschaft`, `MannschaftsID`, `Rangliste`, `RanglistenID`, `Gruppe`, `GruppenID`,
  `Status`, `Trefferzahl`, `TotalRing`, `TotalRing01`, `BesterTeiler01`,
  `BesterTeiler001`,
  `Zeitstempel`
) VALUES (
  1, 'Testlauf', 1, '2026-11-18 10:00:00', 1,
  'Mustermann', 'Max', 'male', 'GER', 1234567,
  4,
  'H',
  'KK 50m liegend 60 Schuss', 40180060, 'Herren', 1, 'SV Test', 1,
  'SV Test 1', 1, 'Rangliste 1', 1, 'Gruppe A', 1,
  'RWK', 3, 27, 282, 15,
  148,
  '2026-11-18 10:05:00'
);

INSERT INTO `Serien` (`ScheibenID`, `Stellung`, `Serie`, `Ring`, `Ring01`) VALUES
  (1, 1, 1, 27, 282);

INSERT INTO `Treffer` (
  `ScheibenID`, `Stellung`, `Treffer`,
  `Serie`,
  `x`, `y`, `Innenzehner`, `Wertung`, `Text`, `Ring`, `Ring01`, `Teiler01`, `Teiler001`, `Zeitstempel`, `Millisekunden`
) VALUES
  (1, 1, 1,
   1,
   120, -80, 0, 1, '10.4', 10, 104, 15, 148, '2026-11-18 10:01:00', 120),
  (1, 1, 2,
   1,
   -300, 150, 0, 1, '9.3', 9, 93, 34, 335, '2026-11-18 10:02:00', 450),
  (1, 1, 3,
   1,
   -520, -410, 0, 1, '8.5', 8, 85, 66, 662, '2026-11-18 10:03:00', 870);
