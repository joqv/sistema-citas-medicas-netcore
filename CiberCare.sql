create database CiberCare
go

use CiberCare
go

-- Tabla de Pacientes
CREATE TABLE Pacientes (
    dni VARCHAR(20) PRIMARY KEY,
    nombre NVARCHAR(100),
    apellido NVARCHAR(100),
    telefono VARCHAR(15),
    email NVARCHAR(100)
);
go

alter table Pacientes 
add contrasena nvarchar(50)
go

INSERT INTO Pacientes (dni, nombre, apellido, telefono, email) VALUES
('12345678', 'Juan', 'Perez', '987654321', 'juan@example.com'),
('87654321', 'Maria', 'Gonzalez', '987654322', 'maria@example.com'),
('11223344', 'Luis', 'Torres', '987654323', 'luis@example.com'),
('44332211', 'Sofia', 'Ramos', '987654324', 'sofia@example.com'),
('55667788', 'Andrés', 'Vega', '987654325', 'andres@example.com'),
('99887766', 'Laura', 'Silva', '987654326', 'laura@example.com'),
('66778899', 'Pedro', 'Cruz', '987654327', 'pedro@example.com'),
('88990011', 'Lucía', 'Mora', '987654328', 'lucia@example.com'),
('22334455', 'José', 'Reyes', '987654329', 'jose@example.com'),
('33445566', 'Valeria', 'Núñez', '987654330', 'valeria@example.com');
go

CREATE PROCEDURE usp_listar_pacientes
AS
BEGIN
    SELECT * FROM Pacientes;
END;
GO

CREATE PROCEDURE usp_insertar_paciente
    @dni VARCHAR(20),
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @telefono VARCHAR(15),
    @email NVARCHAR(100)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Pacientes WHERE dni = @dni)
    BEGIN
        INSERT INTO Pacientes (dni, nombre, apellido, telefono, email)
        VALUES (@dni, @nombre, @apellido, @telefono, @email);
    END
    ELSE
    BEGIN
        PRINT 'El paciente con ese DNI ya está registrado.';
    END
END;
GO

CREATE PROCEDURE usp_actualizar_paciente
    @dni VARCHAR(20),
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @telefono VARCHAR(15),
    @email NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Pacientes WHERE dni = @dni)
    BEGIN
        UPDATE Pacientes
        SET nombre = @nombre,
            apellido = @apellido,
            telefono = @telefono,
            email = @email
        WHERE dni = @dni;
    END
    ELSE
    BEGIN
        PRINT 'No se encontró un paciente con ese DNI.';
    END
END;
GO

CREATE PROCEDURE usp_eliminar_paciente
    @dni VARCHAR(20)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Pacientes WHERE dni = @dni)
    BEGIN
        DELETE FROM Pacientes WHERE dni = @dni;
    END
    ELSE
    BEGIN
        PRINT 'No se encontró un paciente con ese DNI.';
    END
END;
GO

-- Tabla de Especialidades
CREATE TABLE Especialidades (
    id_especialidad INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(100)
);
go

INSERT INTO Especialidades (nombre) VALUES
('Medicina General'), ('Pediatría'), ('Cardiología'), ('Dermatología'), ('Ginecología');
go


CREATE PROCEDURE usp_listar_especialidad
AS
BEGIN
    SELECT * FROM Especialidades;
END;
GO

CREATE PROCEDURE usp_insertar_especialidad
    @nombre NVARCHAR(100)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Especialidades WHERE nombre = @nombre)
    BEGIN
        INSERT INTO Especialidades (nombre)
        VALUES (@nombre);
    END
    ELSE
    BEGIN
        PRINT 'La especialidad ya está registrada.';
    END
END;
GO

CREATE PROCEDURE usp_actualizar_especialidad
    @id_especialidad INT,
    @nombre NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Especialidades WHERE id_especialidad = @id_especialidad)
    BEGIN
        UPDATE Especialidades
        SET nombre = @nombre
        WHERE id_especialidad = @id_especialidad;
    END
    ELSE
    BEGIN
        PRINT 'No se encontró la especialidad con ese ID.';
    END
END;
GO

CREATE PROCEDURE usp_eliminar_especialidad
    @id_especialidad INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Especialidades WHERE id_especialidad = @id_especialidad)
    BEGIN
        DELETE FROM Especialidades
        WHERE id_especialidad = @id_especialidad;
    END
    ELSE
    BEGIN
        PRINT 'No se encontró la especialidad con ese ID.';
    END
END;
GO

-- Tabla de Doctores
CREATE TABLE Doctores (
    id_doctor INT PRIMARY KEY IDENTITY(1,1),
    nombre NVARCHAR(100),
    apellido NVARCHAR(100),
    id_especialidad INT,
    FOREIGN KEY (id_especialidad) REFERENCES Especialidades(id_especialidad)
);
go

INSERT INTO Doctores (nombre, apellido, id_especialidad) VALUES
('Carlos', 'Ramírez', 1), ('Elena', 'Gómez', 2), ('José', 'Martínez', 3),
('Lucía', 'Fernández', 4), ('Marco', 'Reyes', 5),
('Laura', 'Mendoza', 1), ('Sergio', 'Salazar', 2), ('Ana', 'Paredes', 3),
('Daniel', 'Lozano', 4), ('Julia', 'Campos', 5);
go

CREATE OR ALTER  PROCEDURE usp_listar_doctores
AS
BEGIN
    SELECT D.id_doctor, D.nombre, D.apellido, E.id_especialidad AS especialidad
    FROM Doctores D
    INNER JOIN Especialidades E ON D.id_especialidad = E.id_especialidad;
END;
GO

CREATE PROCEDURE usp_insertar_doctor
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @id_especialidad INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Especialidades WHERE id_especialidad = @id_especialidad)
    BEGIN
        INSERT INTO Doctores (nombre, apellido, id_especialidad)
        VALUES (@nombre, @apellido, @id_especialidad);
    END
    ELSE
    BEGIN
        PRINT 'La especialidad especificada no existe.';
    END
END;
GO

CREATE PROCEDURE usp_actualizar_doctor
    @id_doctor INT,
    @nombre NVARCHAR(100),
    @apellido NVARCHAR(100),
    @id_especialidad INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Doctores WHERE id_doctor = @id_doctor)
    BEGIN
        IF EXISTS (SELECT 1 FROM Especialidades WHERE id_especialidad = @id_especialidad)
        BEGIN
            UPDATE Doctores
            SET nombre = @nombre,
                apellido = @apellido,
                id_especialidad = @id_especialidad
            WHERE id_doctor = @id_doctor;
        END
        ELSE
        BEGIN
            PRINT 'La especialidad indicada no existe.';
        END
    END
    ELSE
    BEGIN
        PRINT 'No se encontró el doctor con ese ID.';
    END
END;
GO

CREATE PROCEDURE usp_eliminar_doctor
    @id_doctor INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Doctores WHERE id_doctor = @id_doctor)
    BEGIN
        DELETE FROM Doctores WHERE id_doctor = @id_doctor;
    END
    ELSE
    BEGIN
        PRINT 'No se encontró el doctor con ese ID.';
    END
END;
GO

-- Tabla de Horarios
CREATE TABLE Horarios (
    id_horario INT PRIMARY KEY IDENTITY(1,1),
    fecha DATE,
    hora TIME,
    id_doctor INT,
    disponible BIT DEFAULT 1,
    FOREIGN KEY (id_doctor) REFERENCES Doctores(id_doctor)
);
go

INSERT INTO Horarios (fecha, hora, id_doctor, disponible) VALUES
('2025-04-15', '09:00', 1, 1), ('2025-04-15', '10:00', 2, 1),
('2025-04-15', '11:00', 3, 1), ('2025-04-16', '09:00', 4, 1),
('2025-04-16', '10:00', 5, 1), ('2025-04-16', '11:00', 6, 1),
('2025-04-17', '09:00', 7, 1), ('2025-04-17', '10:00', 8, 1),
('2025-04-17', '11:00', 9, 1), ('2025-04-18', '09:00', 10, 1),
('2025-04-18', '10:00', 1, 1), ('2025-04-18', '11:00', 2, 1),
('2025-04-19', '09:00', 3, 1), ('2025-04-19', '10:00', 4, 1),
('2025-04-19', '11:00', 5, 1), ('2025-04-19', '12:00', 6, 1),
('2025-04-19', '13:00', 7, 1), ('2025-04-19', '14:00', 8, 1),
('2025-04-19', '15:00', 9, 1), ('2025-04-19', '16:00', 10, 1);
go

-- ACTUALIZAR DISPONIBILIDAD DE LOS HORARIOS YA TOMADOS
UPDATE Horarios SET disponible = 1 WHERE id_horario IN (1,2,3,4,5,13,19,20,23,33);
go

SELECT 
    id_horario,
    fecha,
    CONVERT(VARCHAR(5), hora, 108) AS hora, -- Esto devuelve '09:00'
    id_doctor,
    disponible
FROM Horarios
go

CREATE OR ALTER PROCEDURE usp_listar_horarios_disponibles
AS
BEGIN
    SELECT H.id_horario, H.fecha, H.hora, D.id_doctor AS doctor, D.apellido, E.id_especialidad AS especialidad
    FROM Horarios H
    INNER JOIN Doctores D ON H.id_doctor = D.id_doctor
    INNER JOIN Especialidades E ON D.id_especialidad = E.id_especialidad
END;
GO

exec usp_listar_horarios_disponibles
go

CREATE PROCEDURE usp_insertar_horario
    @fecha DATE,
    @hora TIME,
    @id_doctor INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Doctores WHERE id_doctor = @id_doctor)
    BEGIN
        INSERT INTO Horarios (fecha, hora, id_doctor, disponible)
        VALUES (@fecha, @hora, @id_doctor, 1);
    END
    ELSE
    BEGIN
        PRINT 'El doctor especificado no existe.';
    END
END;
GO

CREATE PROCEDURE usp_actualizar_horario
    @id_horario INT,
    @fecha DATE,
    @hora TIME,
    @id_doctor INT,
    @disponible BIT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Horarios WHERE id_horario = @id_horario)
    BEGIN
        UPDATE Horarios
        SET fecha = @fecha,
            hora = @hora,
            id_doctor = @id_doctor,
            disponible = @disponible
        WHERE id_horario = @id_horario;
    END
    ELSE
    BEGIN
        PRINT 'No se encontró el horario con ese ID.';
    END
END;
GO

CREATE PROCEDURE usp_eliminar_horario
    @id_horario INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Horarios WHERE id_horario = @id_horario)
    BEGIN
        DELETE FROM Horarios WHERE id_horario = @id_horario;
    END
    ELSE
    BEGIN
        PRINT 'No se encontró el horario con ese ID.';
    END
END;
GO

CREATE PROCEDURE usp_buscar_horarios_por_id_doctor
    @id_doctor INT
AS
BEGIN
    SELECT id_horario, fecha, CONVERT(VARCHAR(5), hora, 108) AS hora, disponible
    FROM Horarios
    WHERE id_doctor = @id_doctor;
END;
GO

-- Tabla de Citas
CREATE TABLE Citas (
    id_cita INT PRIMARY KEY IDENTITY(1,1),
    dni_paciente VARCHAR(20),
    id_horario INT,
    FOREIGN KEY (dni_paciente) REFERENCES Pacientes(dni),
    FOREIGN KEY (id_horario) REFERENCES Horarios(id_horario)
);
go

INSERT INTO Citas (dni_paciente, id_horario) VALUES
('12345678', 1), ('87654321', 2), ('11223344', 3),
('44332211', 4), ('55667788', 5);
go

CREATE PROCEDURE usp_listar_citas
AS
BEGIN
    SELECT 
        C.id_cita,
        C.dni_paciente,
        P.nombre + ' ' + P.apellido AS nombre_paciente,
        C.id_horario,
        H.fecha,
        CONVERT(VARCHAR(5), H.hora, 108) AS hora,
        D.nombre + ' ' + D.apellido AS nombre_doctor,
        E.nombre AS especialidad
    FROM Citas C
    INNER JOIN Pacientes P ON C.dni_paciente = P.dni
    INNER JOIN Horarios H ON C.id_horario = H.id_horario
    INNER JOIN Doctores D ON H.id_doctor = D.id_doctor
    INNER JOIN Especialidades E ON D.id_especialidad = E.id_especialidad;
END;
GO

CREATE or alter PROCEDURE usp_registrar_cita
    @dni_paciente VARCHAR(20),
    @id_horario INT
AS
BEGIN
    -- Verificar disponibilidad
    IF EXISTS (SELECT 1 FROM Horarios WHERE id_horario = @id_horario AND disponible = 1)
    BEGIN
        INSERT INTO Citas (dni_paciente, id_horario) VALUES (@dni_paciente, @id_horario);
        UPDATE Horarios SET disponible = 0 WHERE id_horario = @id_horario;
    END
    ELSE
    BEGIN
        PRINT 'Horario no disponible';
    END
END;
GO

CREATE or alter PROCEDURE usp_actualizar_cita
    @id_cita INT,
    @nuevo_id_horario INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Citas WHERE id_cita = @id_cita)
    BEGIN
        IF EXISTS (SELECT 1 FROM Horarios WHERE id_horario = @nuevo_id_horario AND disponible = 1)
        BEGIN
            DECLARE @anterior_id_horario INT;

            SELECT @anterior_id_horario = id_horario FROM Citas WHERE id_cita = @id_cita;

            UPDATE Citas
            SET id_horario = @nuevo_id_horario
            WHERE id_cita = @id_cita;

            UPDATE Horarios
            SET disponible = 1
            WHERE id_horario = @anterior_id_horario;

            UPDATE Horarios
            SET disponible = 0
            WHERE id_horario = @nuevo_id_horario;
        END
        ELSE
        BEGIN
            PRINT 'El nuevo horario no está disponible.';
        END
    END
    ELSE
    BEGIN
        PRINT 'No se encontró la cita con ese ID.';
    END
END;
GO

CREATE PROCEDURE usp_eliminar_cita
    @id_cita INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Citas WHERE id_cita = @id_cita)
    BEGIN
        DECLARE @id_horario INT;

        SELECT @id_horario = id_horario FROM Citas WHERE id_cita = @id_cita;

        DELETE FROM Citas WHERE id_cita = @id_cita;

        UPDATE Horarios
        SET disponible = 1
        WHERE id_horario = @id_horario;
    END
    ELSE
    BEGIN
        PRINT 'No se encontró la cita con ese ID.';
    END
END;
GO

use CiberCare;
go

--Crear tabla admin

create table Admin(
admin varchar(90),
contrasena varchar(90)
);
go

INSERT INTO Admin (admin, contrasena) VALUES
('Fernando', 'fernando123'),
('Juaquin', 'juaquin456'),
('Jhus', 'jhus789');
go

CREATE PROCEDURE usp_listar_admin
AS
BEGIN
    SET NOCOUNT ON;

    SELECT admin, contrasena FROM Admin;
END;
go

EXEC usp_listar_admin;
go

select * from Admin;
go

SELECT email FROM Pacientes
go

SELECT contrasena FROM Admin;
go

--Crear login

CREATE PROCEDURE sp_Login
    @Usuario VARCHAR(90),
    @Contrasena VARCHAR(90)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM Admin
        WHERE admin = @Usuario AND contrasena = @Contrasena
    )
    BEGIN
        SELECT 'Login exitoso' AS Mensaje;
    END
    ELSE
    BEGIN
        SELECT 'Usuario o contraseña incorrectos' AS Mensaje;
    END
END
go

--Login

--drop PROCEDURE usp_login_admin

create PROCEDURE usp_login_admin
    @correo VARCHAR(100),
    @contrasena VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dni, nombre, apellido, telefono, email, @contrasena
    FROM Pacientes
    WHERE email = @correo AND @contrasena = @contrasena;
END
go

EXEC usp_login_admin @correo = 'luis@example.com', @contrasena = 'Luis123';
go

EXEC sp_Login @Usuario = 'Fernando', @Contrasena = 'fernando123';
go