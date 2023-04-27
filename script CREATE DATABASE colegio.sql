CREATE DATABASE colegio;
USE colegio;

CREATE TABLE CarreraCursoTipo
(
	CarreraCursoTipoId INT NOT NULL AUTO_INCREMENT,
    Descripcion NVARCHAR(30),
    PRIMARY KEY (CarreraCursoTipoId)
);

INSERT INTO CarreraCursoTipo (Descripcion) VALUES ('Curso');
INSERT INTO CarreraCursoTipo (Descripcion) VALUES ('Carrera');

CREATE TABLE TipoDuracion
(
	TipoDuracionId INT NOT NULL AUTO_INCREMENT,
    Descripcion NVARCHAR(30),
    PRIMARY KEY (TipoDuracionId)
);

INSERT INTO TipoDuracion (Descripcion) VALUES ('Meses');
INSERT INTO TipoDuracion (Descripcion) VALUES ('Años');

CREATE TABLE Requisito
(
	RequisitoId INT NOT NULL AUTO_INCREMENT,
    Descripcion NVARCHAR(100),
    Anulado BIT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (RequisitoId)
);

INSERT INTO Requisito (Descripcion) VALUES ('DNI');
INSERT INTO Requisito (Descripcion) VALUES ('Partida de nacimiento');
INSERT INTO Requisito (Descripcion) VALUES ('Analítico y Título Secundario');
INSERT INTO Requisito (Descripcion) VALUES ('Analítico y Título Secundario-Primario');
INSERT INTO Requisito (Descripcion) VALUES ('Constancia de CUIL (originales y copias)');
INSERT INTO Requisito (Descripcion) VALUES ('Dos fotos 4x4 iguales');


CREATE TABLE CicloLectivo
(
	CicloLectivoId INT NOT NULL AUTO_INCREMENT,
    Descripcion NVARCHAR(100) NOT NULL,
    
    FechaInicio DATETIME NOT NULL,
    FechaFin DATETIME NOT NULL,
    
    CobranzaInicioMes INT NOT NULL,
    CobranzaInicioAnio INT NOT NULL,
    CobranzaFinMes INT NOT NULL,
    CobranzaFinAnio INT NOT NULL,
    
    Cerrado BIT(1) NOT NULL DEFAULT 0,
    Anulado BIT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (CicloLectivoId)
);

CREATE TABLE CarreraCurso
(
	CarreraCursoId INT NOT NULL AUTO_INCREMENT,
    CarreraCursoTipoId INT NOT NULL,
    Duracion INT NOT NULL,
    TipoDuracionId INT NOT NULL,
    Titulo NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    Horarios NVARCHAR(200) NOT NULL,
    Anulado BIT(1) NOT NULL DEFAULT 0,
    FOREIGN KEY (CarreraCursoTipoId) REFERENCES CarreraCursoTipo(CarreraCursoTipoId),
    FOREIGN KEY (TipoDuracionId) REFERENCES TipoDuracion(TipoDuracionId),
    PRIMARY KEY (CarreraCursoId)
);

CREATE TABLE CarreraCursoRequisito
(
	CarreraCursoRequisitoId INT NOT NULL AUTO_INCREMENT,
    CarreraCursoId INT NOT NULL,
    RequisitoId INT NOT NULL,
    EsExcluyente BIT(1) NOT NULL,
    FOREIGN KEY (CarreraCursoId) REFERENCES CarreraCurso(CarreraCursoId),
    FOREIGN KEY (RequisitoId) REFERENCES Requisito(RequisitoId),
    PRIMARY KEY (CarreraCursoRequisitoId)
);

CREATE TABLE Inscripcion
(
	InscripcionId INT NOT NULL AUTO_INCREMENT,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    DNI NVARCHAR(20) NOT NULL,
    Telefono NVARCHAR(50),
    Email NVARCHAR(100),
    Edad INT,
    FechaNacimiento DATE,
    LugarNacimiento NVARCHAR(100),
    Direccion NVARCHAR(100),
    Localidad NVARCHAR(100),
    
    Afiliado NVARCHAR(50),
    
    Recibo NVARCHAR(50),
    CarreraCursoId INT NOT NULL,
    CicloLectivoId INT NOT NULL,
    
    BajaCarreraFecha DATETIME,
    BajaMotivo NVARCHAR(200),
    
    PRIMARY KEY (InscripcionId),
    FOREIGN KEY (CarreraCursoId) REFERENCES CarreraCurso(CarreraCursoId),
    FOREIGN KEY (CicloLectivoId) REFERENCES CicloLectivo(CicloLectivoId)
);
CREATE INDEX ind_Inscripcion_DNI ON Inscripcion(DNI);
CREATE INDEX ind_Inscripcion_Nombre ON Inscripcion(Nombre);
CREATE INDEX ind_Inscripcion_Apellido ON Inscripcion(Apellido);
CREATE INDEX ind_Inscripcion_Email ON Inscripcion(Email);

CREATE TABLE InscripcionCarreraCursoRequisito
(
	InscripcionCarreraCursoRequisitoId INT NOT NULL AUTO_INCREMENT,
    InscripcionId INT NOT NULL,
    CarreraCursoRequisitoId INT NOT NULL,
    FechaHoraEntregado DATETIME,
    PRIMARY KEY (InscripcionCarreraCursoRequisitoId),
    FOREIGN KEY (InscripcionId) REFERENCES Inscripcion(InscripcionId),
    FOREIGN KEY (CarreraCursoRequisitoId) REFERENCES CarreraCursoRequisito(CarreraCursoRequisitoId)
);

CREATE TABLE Gasto
(
	GastoId INT NOT NULL AUTO_INCREMENT,
    FechaHora DATETIME NOT NULL,
    Concepto NVARCHAR(100) NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Anulado BIT(1) NOT NULL,
    PRIMARY KEY(GastoId)
);

CREATE TABLE Cobranza
(
	CobranzaId INT NOT NULL AUTO_INCREMENT,
    InscripcionId INT NOT NULL,
    Mes INT NOT NULL,
    Anio INT NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Observaciones NVARCHAR(200),
    Anulado BIT(1) NOT NULL,
    PRIMARY KEY(CobranzaId),
    FOREIGN KEY (InscripcionId) REFERENCES Inscripcion(InscripcionId)
);


ALTER TABLE CicloLectivo ADD InscripcionHabilitadaDesde DATETIME NOT NULL;
ALTER TABLE CicloLectivo ADD InscripcionHabilitadaHasta DATETIME NOT NULL;

CREATE TABLE MesAnio
(
	Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Mes INT NOT NULL,
    Anio INT NOT NUll
);

CREATE VIEW vwCronogramaCobranzas
AS SELECT
	CL.CicloLectivoId,
    CL.Descripcion,
    CONCAT(LPAD(CL.CobranzaInicioMes, 2, '0'), '-', CL.CobranzaInicioAnio) AS Desde,
    CONCAT(LPAD(CL.CobranzaFinMes, 2, '0'), '-', CL.CobranzaFinAnio) AS Hasta,
    CONCAT(LPAD(MA.Mes, 2, '0'), '-', MA.Anio) AS Periodo,
	MA.Mes,
    MA.Anio
FROM
	CicloLectivo CL
    INNER JOIN MesAnio MA
			ON 
            (
				(MA.Anio * 12 + MA.Mes) BETWEEN (CL.CobranzaInicioAnio * 12 + CL.CobranzaInicioMes)
                AND (CL.CobranzaFinAnio * 12 + CL.CobranzaFinMes)
            )
WHERE
	CL.Anulado = 0
ORDER BY
	CL.Descripcion,
	(MA.Anio * 12 + MA.Mes);
    
ALTER TABLE Cobranza ADD FechaHora DATETIME;
ALTER TABLE Cobranza ADD Automatica BIT;

DELIMITER $$
DROP PROCEDURE IF EXISTS `ListarCobranzas` $$
CREATE PROCEDURE `ListarCobranzas`(

)
BEGIN

	SELECT
		CC.CicloLectivoId,
		I.InscripcionId,
		I.Nombre,
		I.Apellido,
		I.DNI,
		I.Afiliado,
		I.BajaCarreraFecha,
		CA.Titulo AS CarreraCurso,
		CC.Descripcion,
		CC.Periodo,
		CO.CobranzaId,
		CASE WHEN CO.CobranzaId IS NULL THEN
			'PENDIENTE'
		WHEN CO.Automatica = 1 THEN
			'BONIFICADO'
        ELSE
			'ABONADO'
		END AS Estado,
		CC.Mes,
		CC.Anio
	FROM
		vwCronogramaCobranzas AS CC
		INNER JOIN Inscripcion AS I ON I.CicloLectivoId = CC.CicloLectivoId
								AND (I.BajaCarreraFecha IS NULL OR I.BajaCarreraFecha > DATE(CONCAT(CC.Anio, '-', CC.Mes, '-01')))
		INNER JOIN CarreraCurso AS CA ON CA.CarreraCursoId = I.CarreraCursoId
		LEFT JOIN Cobranza CO ON CO.InscripcionId = I.InscripcionId AND CO.Mes = CC.Mes AND CO.Anio = CC.Anio AND CO.Anulado = 0
	ORDER BY
		(CC.Anio * 12 + CC.Mes),
		Nombre, Apellido;

END $$
DELIMITER ;


/********* UPDATE 27/01/2020 ***********/
ALTER TABLE Inscripcion ADD Sexo CHAR(1);
ALTER TABLE Inscripcion ADD EstadoCivil CHAR(1);
ALTER TABLE Inscripcion ADD TrabajoCargoFuncion NVARCHAR(100);
ALTER TABLE Inscripcion ADD TrabajoAntiguedad NVARCHAR(50);
ALTER TABLE Inscripcion ADD TrabajoRegionSanitaria NVARCHAR(50);
ALTER TABLE Inscripcion ADD TrabajoDependencia NVARCHAR(50);
ALTER TABLE Inscripcion ADD TrabajoDireccionLaboral NVARCHAR(50);
ALTER TABLE Inscripcion ADD TrabajoLocalidad NVARCHAR(100);
ALTER TABLE Inscripcion ADD TrabajoTelefono NVARCHAR(50);
ALTER TABLE Inscripcion ADD TrabajoEmail NVARCHAR(100);
ALTER TABLE Cobranza ADD Efectivo BIT(1);


UPDATE Inscripcion SET Sexo = 'M' WHERE InscripcionId > 0;
UPDATE Inscripcion SET EstadoCivil = 'S' WHERE InscripcionId > 0;
UPDATE Cobranza SET Efectivo = 0 WHERE CobranzaId > 0;


/******** UPDATE 09/02/2020 ************/
DELIMITER $$
DROP PROCEDURE IF EXISTS `ListarMovimientos` $$
CREATE PROCEDURE `ListarMovimientos`(
	pFechaDesde DATE,
    pFechaHasta DATE
)
BEGIN

	SELECT
		*
	FROM
	(
		SELECT
			C.FechaHora,
			CONCAT('[Cobranza] ', I.Nombre, ' ', I.Apellido, ' --- Período ', LPAD(C.Mes, 2, '0'), '/', C.Anio) AS Descripcion,
			C.Monto,
			1 AS Signo
		FROM
			Cobranza C
			INNER JOIN Inscripcion I ON I.InscripcionId = C.InscripcionId
		WHERE
			C.Automatica = 0
			AND C.Anulado = 0
			AND DATE(C.FechaHora) >= DATE(COALESCE(pFechaDesde, C.FechaHora))
			AND DATE(C.FechaHora) <= DATE(COALESCE(pFechaHasta, C.FechaHora))
			
		UNION ALL

		SELECT
			G.FechaHora,
			CONCAT('[Gasto] ', Concepto) AS Descripcion,
			G.Monto,
			-1 AS Signo
		FROM
			Gasto G
		WHERE
			G.Anulado = 0
			AND DATE(G.FechaHora) >= DATE(COALESCE(pFechaDesde, G.FechaHora))
			AND DATE(G.FechaHora) <= DATE(COALESCE(pFechaHasta, G.FechaHora))
	) AS R
	ORDER BY
		R.FechaHora DESC;

END $$
DELIMITER ;


/****** UPDATE 25/02/2020 *********/
ALTER TABLE Inscripcion ADD AltaFecha DATETIME;

CREATE TABLE InscripcionEntregaDocumentacion
(
	InscripcionEntregaDocumentacionId INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
	InscripcionId INT NOT NULL,
    RequisitoId INT NOT NULL,
    FechaHora DATETIME NOT NULL
);

DELIMITER $$
DROP PROCEDURE IF EXISTS `ObtenerRequisitosPendientesDeEntregar` $$
CREATE PROCEDURE `ObtenerRequisitosPendientesDeEntregar`(
	pInscripcionId INT
)
BEGIN

	SELECT
		I.InscripcionId,
		I.Nombre,
		I.Apellido,
		I.DNI,
		CC.CarreraCursoId,
		CC.Titulo AS Carrera,
		R.RequisitoId,
		R.Descripcion AS Requisito
	FROM
		Inscripcion I
		INNER JOIN CarreraCurso CC ON CC.CarreraCursoId = I.CarreraCursoId
		INNER JOIN CarreraCursoRequisito CCR ON CCR.CarreraCursoId = CC.CarreraCursoId
		INNER JOIN Requisito R ON R.RequisitoId = CCR.RequisitoId
		LEFT JOIN InscripcionEntregaDocumentacion E ON E.RequisitoId = R.RequisitoId AND E.InscripcionId = I.InscripcionId
	WHERE
		I.BajaCarreraFecha IS NULL
		AND CC.Anulado = 0
        AND E.FechaHora IS NULL
		AND I.InscripcionId = COALESCE(pInscripcionId, I.InscripcionId);

END $$
DELIMITER ;

CREATE VIEW vwInscripcionesQueDebenDocumentacion
AS SELECT
	I.InscripcionId
FROM
	Inscripcion I
    INNER JOIN CarreraCurso CC ON CC.CarreraCursoId = I.CarreraCursoId
    INNER JOIN CarreraCursoRequisito CCR ON CCR.CarreraCursoId = CC.CarreraCursoId
    INNER JOIN Requisito R ON R.RequisitoId = CCR.RequisitoId
    LEFT JOIN InscripcionEntregaDocumentacion E ON E.RequisitoId = R.RequisitoId AND E.InscripcionId = I.InscripcionId
WHERE
	I.BajaCarreraFecha IS NULL
    AND CC.Anulado = 0
    AND E.RequisitoId IS NULL
GROUP BY
	I.InscripcionId;
    
DELIMITER $$
DROP PROCEDURE IF EXISTS `ListarCobranzas` $$
CREATE PROCEDURE `ListarCobranzas`(

)
BEGIN

	SELECT
		CC.CicloLectivoId,
		I.InscripcionId,
		I.Nombre,
		I.Apellido,
		I.DNI,
		I.Afiliado,
		I.BajaCarreraFecha,
		CA.Titulo AS CarreraCurso,
		CC.Descripcion,
		CC.Periodo,
		CO.CobranzaId,
		CASE WHEN CO.CobranzaId IS NULL THEN
			'PENDIENTE'
		WHEN CO.Automatica = 1 THEN
			'BONIFICADO'
        ELSE
			'ABONADO'
		END AS Estado,
		CC.Mes,
		CC.Anio,
        CASE WHEN DD.InscripcionId IS NULL THEN FALSE ELSE TRUE END AS DebeDocumentacion
	FROM
		vwCronogramaCobranzas AS CC
		INNER JOIN Inscripcion AS I ON I.CicloLectivoId = CC.CicloLectivoId
								AND (I.BajaCarreraFecha IS NULL OR I.BajaCarreraFecha > DATE(CONCAT(CC.Anio, '-', CC.Mes, '-01')))
		INNER JOIN CarreraCurso AS CA ON CA.CarreraCursoId = I.CarreraCursoId
		LEFT JOIN Cobranza CO ON CO.InscripcionId = I.InscripcionId AND CO.Mes = CC.Mes AND CO.Anio = CC.Anio AND CO.Anulado = 0
        LEFT JOIN vwInscripcionesQueDebenDocumentacion AS DD ON DD.InscripcionId = I.InscripcionId
	ORDER BY
		(CC.Anio * 12 + CC.Mes),
		Nombre, Apellido;

END $$
DELIMITER ;

CREATE TABLE CobranzaPagoAdelantado
(
	CobranzaPagoAdelantadoId INT NOT NULL AUTO_INCREMENT,
    InscripcionId INT NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Observaciones NVARCHAR(200),
    Anulado BIT(1) NOT NULL,
    PRIMARY KEY(CobranzaPagoAdelantadoId),
    FOREIGN KEY (InscripcionId) REFERENCES Inscripcion(InscripcionId)
);

ALTER TABLE Cobranza ADD CobranzaPagoAdelantadoId INT;
ALTER TABLE Cobranza ADD CONSTRAINT FK_Cobranza_CobranzaPagoAdelantado FOREIGN KEY (CobranzaPagoAdelantadoId) REFERENCES CobranzaPagoAdelantado(CobranzaPagoAdelantadoId);

DELIMITER $$
DROP PROCEDURE IF EXISTS `ListarMovimientos` $$
CREATE PROCEDURE `ListarMovimientos`(
	pFechaDesde DATE,
    pFechaHasta DATE
)
BEGIN

	SELECT
		*
	FROM
	(
		SELECT
			C.FechaHora,
			CONCAT('[Cobranza] ', I.Nombre, ' ', I.Apellido, ' --- Período ', LPAD(C.Mes, 2, '0'), '/', C.Anio) AS Descripcion,
			C.Monto,
			1 AS Signo
		FROM
			Cobranza C
			INNER JOIN Inscripcion I ON I.InscripcionId = C.InscripcionId
		WHERE
			C.Automatica = 0
			AND C.Anulado = 0
            AND C.CobranzaPagoAdelantadoId IS NULL
			AND DATE(C.FechaHora) >= DATE(COALESCE(pFechaDesde, C.FechaHora))
			AND DATE(C.FechaHora) <= DATE(COALESCE(pFechaHasta, C.FechaHora))
			
		UNION ALL
        
        SELECT
			C.FechaHora,
			CONCAT('[Cobranza] PAGO ADELANTADO - ', I.Nombre, ' ', I.Apellido, ' --- ', COUNT(C.Mes),' Meses') AS Descripcion,
			PA.Monto,
			1 AS Signo
		FROM
			CobranzaPagoAdelantado PA
            INNER JOIN CObranza C ON C.CobranzaPagoAdelantadoId = PA.CobranzaPagoAdelantadoId
			INNER JOIN Inscripcion I ON I.InscripcionId = C.InscripcionId
		WHERE
			C.Automatica = 0
			AND C.Anulado = 0
			AND DATE(C.FechaHora) >= DATE(COALESCE(pFechaDesde, C.FechaHora))
			AND DATE(C.FechaHora) <= DATE(COALESCE(pFechaHasta, C.FechaHora))
		GROUP BY
			PA.CobranzaPagoAdelantadoId,
			I.Nombre,
            I.Apellido,
            C.Monto
            
		UNION ALL

		SELECT
			G.FechaHora,
			CONCAT('[Gasto] ', Concepto) AS Descripcion,
			G.Monto,
			-1 AS Signo
		FROM
			Gasto G
		WHERE
			G.Anulado = 0
			AND DATE(G.FechaHora) >= DATE(COALESCE(pFechaDesde, G.FechaHora))
			AND DATE(G.FechaHora) <= DATE(COALESCE(pFechaHasta, G.FechaHora))
	) AS R
	ORDER BY
		R.FechaHora DESC;

END $$
DELIMITER ;


#UPDATE 18/02/2021
DELIMITER $$
DROP PROCEDURE IF EXISTS `ListarCobranzas` $$
CREATE PROCEDURE `ListarCobranzas`(

)
BEGIN

	SELECT
		*
	FROM
    (
		#OBTENEMOS LAS COBRANZAS EN BASE A LAS PROYECCIONES
		SELECT
			CC.CicloLectivoId,
			I.InscripcionId,
			I.Nombre,
			I.Apellido,
			I.DNI,
			I.Afiliado,
			I.BajaCarreraFecha,
			CA.Titulo AS CarreraCurso,
			CC.Descripcion,
			CC.Periodo,
			CO.CobranzaId,
			CASE WHEN CO.CobranzaId IS NULL THEN
				'PENDIENTE'
			WHEN CO.Automatica = 1 THEN
				'BONIFICADO'
			ELSE
				'ABONADO'
			END AS Estado,
			CC.Mes,
			CC.Anio
		FROM
			vwCronogramaCobranzas AS CC
			INNER JOIN Inscripcion AS I ON I.CicloLectivoId = CC.CicloLectivoId
									AND (I.BajaCarreraFecha IS NULL OR I.BajaCarreraFecha > DATE(CONCAT(CC.Anio, '-', CC.Mes, '-01')))
			INNER JOIN CarreraCurso AS CA ON CA.CarreraCursoId = I.CarreraCursoId
			LEFT JOIN Cobranza CO ON CO.InscripcionId = I.InscripcionId AND CO.Mes = CC.Mes AND CO.Anio = CC.Anio AND CO.Anulado = 0
        
        
        UNION
		#Y AHORA LAS UNIMOS CON LAS COBRANZAS NO PROYECTADAS
		SELECT
			I.CicloLectivoId,
			I.InscripcionId,
			I.Nombre,
			I.Apellido,
			I.DNI,
			I.Afiliado,
			I.BajaCarreraFecha,
			CA.Titulo AS CarreraCurso,
			CL.Descripcion,
			CONCAT(LPAD(CO.Mes, 2, '0'), '-', CO.Anio) AS Periodo,
			CO.CobranzaId,
			CASE WHEN CO.CobranzaId IS NULL THEN
				'PENDIENTE'
			WHEN CO.Automatica = 1 THEN
				'BONIFICADO'
			ELSE
				'ABONADO'
			END AS Estado,
			CO.Mes,
			CO.Anio
		FROM
			Cobranza CO
			INNER JOIN Inscripcion AS I ON I.InscripcionId = CO.InscripcionId
			INNER JOIN CicloLectivo AS CL ON CL.CicloLectivoId = I.CicloLectivoId
			INNER JOIN CarreraCurso AS CA ON CA.CarreraCursoId = I.CarreraCursoId
		WHERE
			CO.Anulado = 0
        
	) AS R
	ORDER BY
		(R.Anio * 12 + R.Mes),
		R.Nombre, R.Apellido;

END $$
DELIMITER ;

#UPDATE 04/04/2021
ALTER TABLE CicloLectivo ADD ImporteDefault DECIMAL(18,2) DEFAULT NULL;

#UPDATE 03/07/2021
ALTER TABLE Inscripcion MODIFY COLUMN CicloLectivoId INT NULL;
UPDATE Inscripcion SET AltaFecha = '2020-03-01 00:00:00' WHERE AltaFecha IS NULL AND InscripcionId > 0;
ALTER TABLE Inscripcion MODIFY COLUMN AltaFecha DATETIME NOT NULL;
ALTER TABLE Gasto ADD Tipo INT NOT NULL DEFAULT -1;

ALTER VIEW vwCronogramaCobranzas
AS SELECT
	CC.CarreraCursoId,
    CC.Titulo AS Descripcion,
    MA.Mes,
    MA.Anio,
	CASE WHEN MA.Mes = 1 THEN
		CONCAT('Matricula ', MA.Anio)
	ELSE
		CONCAT(LPAD(MA.Mes, 2, '0'), '-', MA.Anio)
    END AS Periodo
FROM
	CarreraCurso CC
    INNER JOIN MesAnio MA
	ON
	(
		#SI LA CARRERA ES ANUAL ENTONCES TOMO LOS 12 MESES
		CASE WHEN CC.TipoDuracionId = 2 THEN
			TRUE
			
		#SI LA CARRERA ES MENSUAL ENTONCES TOMO LOS MESES DE DURACIÓN + 2 POR DESFACE FEBRERO
		ELSE
			MA.Mes <= CC.Duracion + 2 - 1 # +2 PORQUE ARRANCA EN FEBRERO!
		END
	);
    
DELIMITER $$
DROP PROCEDURE IF EXISTS `ListarCobranzas` $$
CREATE PROCEDURE `ListarCobranzas`(
	IN filtroInscripcionId INT
)
BEGIN

	SELECT
		*
	FROM
    (
		#OBTENEMOS LAS COBRANZAS EN BASE A LAS PROYECCIONES
		SELECT
			CC.CarreraCursoId,
			I.InscripcionId,
			I.Nombre,
			I.Apellido,
			I.DNI,
			I.Afiliado,
			I.BajaCarreraFecha,
			CA.Titulo AS CarreraCurso,
			CC.Descripcion,
			CC.Periodo,
			CO.CobranzaId,
			CASE WHEN CO.CobranzaId IS NULL THEN
				'PENDIENTE'
			WHEN CO.Automatica = 1 THEN
				'BONIFICADO'
			ELSE
				'ABONADO'
			END AS Estado,
			CC.Mes,
			CC.Anio
		FROM
			vwCronogramaCobranzas AS CC
            INNER JOIN CarreraCurso AS CA ON CA.CarreraCursoId = CC.CarreraCursoId
			INNER JOIN Inscripcion AS I
				ON
                (
					I.CarreraCursoId = CC.CarreraCursoId
                    AND
                    (
						#SI LA CARRERA A LA CUAL ESTÁ INSCRIPTO ES ANUAL
						CASE WHEN CA.TipoDuracionId = 2 THEN
							CC.Anio BETWEEN YEAR(I.AltaFecha) AND (YEAR(I.AltaFecha) + CA.Duracion - 1)
                            
						#SI LA CARRERA A LA CUAL ESTÁ INSCRIPTO ES MENSUAL
						ELSE
							CC.Anio = YEAR(I.AltaFecha)
						END
                    )
				)
			LEFT JOIN Cobranza CO ON CO.InscripcionId = I.InscripcionId AND CO.Mes = CC.Mes AND CO.Anio = CC.Anio AND CO.Anulado = 0
        WHERE
			(
				I.BajaCarreraFecha IS NULL
				OR  #SI FUE DADO DE BAJA MOSTRAMOS LAS COBRANZAS HECHAS PERO NO LAS PENDIENTES
                (
					I.BajaCarreraFecha IS NOT NULL
                    AND CO.CobranzaId IS NOT NULL
				)
			)
            AND I.InscripcionId = COALESCE(filtroInscripcionId, I.InscripcionId)
	) AS R
    WHERE
		(R.Anio * 12 + R.Mes) <= (YEAR(NOW()) * 12 + MONTH(NOW()) + 6) #MOSTRAMOS SOLO HASTA AHORA + 6 MESES
	ORDER BY
		(R.Anio * 12 + R.Mes),
		R.Nombre, R.Apellido;

END $$
DELIMITER ;


DELIMITER $$
DROP PROCEDURE IF EXISTS `ListarMovimientos` $$
CREATE PROCEDURE `ListarMovimientos`(
	pFechaDesde DATE,
    pFechaHasta DATE
)
BEGIN

	SELECT
		*
	FROM
	(
		SELECT
			C.FechaHora,
			CONCAT('[Cobranza] ', I.Nombre, ' ', I.Apellido, ' --- Período ', LPAD(C.Mes, 2, '0'), '/', C.Anio) AS Descripcion,
			C.Monto,
			1 AS Signo
		FROM
			Cobranza C
			INNER JOIN Inscripcion I ON I.InscripcionId = C.InscripcionId
		WHERE
			C.Automatica = 0
			AND C.Anulado = 0
			AND DATE(C.FechaHora) >= DATE(COALESCE(pFechaDesde, C.FechaHora))
			AND DATE(C.FechaHora) <= DATE(COALESCE(pFechaHasta, C.FechaHora))
			
		UNION ALL

		SELECT
			G.FechaHora,
			CONCAT('[Ingreso] ', Concepto) AS Descripcion,
			G.Monto,
			1 AS Signo
		FROM
			Gasto G
		WHERE
			G.Anulado = 0
            AND G.Tipo = 1
			AND DATE(G.FechaHora) >= DATE(COALESCE(pFechaDesde, G.FechaHora))
			AND DATE(G.FechaHora) <= DATE(COALESCE(pFechaHasta, G.FechaHora))
            
		UNION ALL

		SELECT
			G.FechaHora,
			CONCAT('[Egreso] ', Concepto) AS Descripcion,
			G.Monto,
			-1 AS Signo
		FROM
			Gasto G
		WHERE
			G.Anulado = 0
            AND G.Tipo = -1
			AND DATE(G.FechaHora) >= DATE(COALESCE(pFechaDesde, G.FechaHora))
			AND DATE(G.FechaHora) <= DATE(COALESCE(pFechaHasta, G.FechaHora))
            
	) AS R
	ORDER BY
		R.FechaHora DESC;

END $$
DELIMITER ;



#UPDATE 25/09/2021
DELIMITER $$
DROP PROCEDURE IF EXISTS `ListarCobranzas` $$
CREATE PROCEDURE `ListarCobranzas`(
	IN filtroInscripcionId INT
)
BEGIN

	SELECT
		*
	FROM
    (
		#OBTENEMOS LAS COBRANZAS EN BASE A LAS PROYECCIONES
		SELECT
			CC.CarreraCursoId,
			I.InscripcionId,
			I.Nombre,
			I.Apellido,
			I.DNI,
			I.Afiliado,
			I.BajaCarreraFecha,
			CA.Titulo AS CarreraCurso,
			CC.Descripcion,
			CC.Periodo,
			CO.CobranzaId,
            COALESCE(CPA.Observaciones, CO.Observaciones, '') AS Recibo,
			CASE WHEN CO.CobranzaId IS NULL THEN
				'PENDIENTE'
			WHEN CO.Automatica = 1 THEN
				'BONIFICADO'
			ELSE
				'ABONADO'
			END AS Estado,
			CC.Mes,
			CC.Anio
		FROM
			vwCronogramaCobranzas AS CC
            INNER JOIN CarreraCurso AS CA ON CA.CarreraCursoId = CC.CarreraCursoId
			INNER JOIN Inscripcion AS I
				ON
                (
					I.CarreraCursoId = CC.CarreraCursoId
                    AND
                    (
						#SI LA CARRERA A LA CUAL ESTÁ INSCRIPTO ES ANUAL
						CASE WHEN CA.TipoDuracionId = 2 THEN
							CC.Anio BETWEEN YEAR(I.AltaFecha) AND (YEAR(I.AltaFecha) + CA.Duracion - 1)
                            
						#SI LA CARRERA A LA CUAL ESTÁ INSCRIPTO ES MENSUAL
						ELSE
							CC.Anio = YEAR(I.AltaFecha)
						END
                    )
				)
			LEFT JOIN Cobranza CO ON CO.InscripcionId = I.InscripcionId AND CO.Mes = CC.Mes AND CO.Anio = CC.Anio AND CO.Anulado = 0
            LEFT JOIN CobranzaPagoAdelantado CPA ON CPA.CobranzaPagoAdelantadoId = CO.CobranzaPagoAdelantadoId AND CPA.Anulado = 0
        WHERE
			(
				I.BajaCarreraFecha IS NULL
				OR  #SI FUE DADO DE BAJA MOSTRAMOS LAS COBRANZAS HECHAS PERO NO LAS PENDIENTES
                (
					I.BajaCarreraFecha IS NOT NULL
                    AND CO.CobranzaId IS NOT NULL
				)
			)
            AND I.InscripcionId = COALESCE(filtroInscripcionId, I.InscripcionId)
	) AS R
    WHERE
		(R.Anio * 12 + R.Mes) <= (YEAR(NOW()) * 12 + MONTH(NOW()) + 6) #MOSTRAMOS SOLO HASTA AHORA + 6 MESES
	ORDER BY
		(R.Anio * 12 + R.Mes),
		R.Nombre, R.Apellido;

END $$
DELIMITER ;