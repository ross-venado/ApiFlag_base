create database dbflags
go

USE [dbflags]
GO
/****** Object:  Table [dbo].[flags]    Script Date: 17/04/2020 10:15:07 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[flags](
	[flag_id] [int] IDENTITY(1,1) NOT NULL,
	[flag_desc] [varchar](300) NOT NULL,
	[flag_estado] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[flag_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[posts]    Script Date: 17/04/2020 10:15:07 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[posts](
	[post_id] [int] IDENTITY(1,1) NOT NULL,
	[post_mensaje] [varchar](max) NOT NULL,
	[id_flag] [int] NULL,
	[id_usuario] [int] NULL,
	[post_ts] [datetime] NULL,
	[post_latitud] [varchar](125) NULL,
	[post_longitud] [varchar](125) NULL,
	[post_estado] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[post_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[recursos]    Script Date: 17/04/2020 10:15:07 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[recursos](
	[rec_id] [int] IDENTITY(1,1) NOT NULL,
	[id_post] [int] NOT NULL,
	[rec_url_img] [varchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[rec_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[usr_usuario]    Script Date: 17/04/2020 10:15:07 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[usr_usuario](
	[usr_id] [int] IDENTITY(1,1) NOT NULL,
	[usr_nombre] [varchar](125) NULL,
	[usr_apellido] [varchar](125) NULL,
	[usr_sexo] [char](1) NULL,
	[usr_direccion] [varchar](125) NULL,
	[usr_latitud] [varchar](125) NULL,
	[usr_longitud] [varchar](125) NULL,
	[usr_correo] [varchar](125) NULL,
	[usr_password] [varchar](125) NULL,
	[usr_token] [nvarchar](max) NULL,
	[usr_tipo_login] [char](1) NULL,
	[usr_estado] [int] NOT NULL,
	[usr_ts] [datetime] NULL CONSTRAINT [DF_usr_usuario_usr_ts]  DEFAULT (getdate()),
 CONSTRAINT [PK_usr_usuario] PRIMARY KEY CLUSTERED 
(
	[usr_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[posts] ADD  DEFAULT (getdate()) FOR [post_ts]
GO
ALTER TABLE [dbo].[posts]  WITH CHECK ADD  CONSTRAINT [fk_flag] FOREIGN KEY([id_flag])
REFERENCES [dbo].[flags] ([flag_id])
GO
ALTER TABLE [dbo].[posts] CHECK CONSTRAINT [fk_flag]
GO
ALTER TABLE [dbo].[posts]  WITH CHECK ADD  CONSTRAINT [fk_usuario] FOREIGN KEY([id_usuario])
REFERENCES [dbo].[usr_usuario] ([usr_id])
GO
ALTER TABLE [dbo].[posts] CHECK CONSTRAINT [fk_usuario]
GO
ALTER TABLE [dbo].[recursos]  WITH CHECK ADD  CONSTRAINT [fk_post] FOREIGN KEY([id_post])
REFERENCES [dbo].[posts] ([post_id])
GO
ALTER TABLE [dbo].[recursos] CHECK CONSTRAINT [fk_post]
GO
/****** Object:  StoredProcedure [dbo].[sp_transaccion]    Script Date: 17/04/2020 10:15:07 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_transaccion]
@usr_email varchar(max) = NULL,
@usr_clave varchar(max) = NULL,
@operacion int
AS 

IF @operacion = 1
BEGIN

SELECT top 1
	CONCAT(T0.usr_nombre, ' ', T0.usr_apellido) [NOMBRE],
	T0.usr_correo,
	T0.usr_direccion,
	T0.usr_latitud,
	T0.usr_longitud,
	T0.usr_sexo
FROM usr_usuario T0 WHERE T0.usr_correo = @usr_email AND T0.usr_password = @usr_clave AND T0.usr_estado = 1

END


GO


CREATE procedure [dbo].[sp_transacciones_flag](@iOperacion CHAR(1),@iID int = null,@iDescripcion VARCHAR(250) = null,
@iEstado int = null, @oSalida int out)
AS
BEGIN
IF @iOperacion = 'A'
	BEGIN
		-- Creamos una nueva bandera	
		IF NOT EXISTS (SELECT * FROM flags WHERE flag_desc = @iDescripcion
						AND flag_estado = 1)
			BEGIN
				INSERT INTO flags VALUES (@iDescripcion,1)
				SET @oSalida = 0
				SELECT @oSalida
			END
		ELSE
			BEGIN				
				SET @oSalida = 100
				SELECT @oSalida
			END
	END
IF @iOperacion = 'B'
	BEGIN
		-- Actualizamos una bandera
		IF EXISTS(SELECT * FROM flags WHERE flag_id = @iID)
			BEGIN
				UPDATE flags SET flag_desc = @iDescripcion, flag_estado = @iEstado WHERE flag_id = @iID
				SET @oSalida = 0
				SELECT @oSalida
			END		
		ELSE -- Indica que el id ingresado no existe para ser actualizado
			BEGIN
				SET @oSalida = 200
				SELECT @oSalida
			END
	END
IF @iOperacion = 'C'
	BEGIN
		IF EXISTS(SELECT * FROM flags WHERE flag_id = @iID
					AND flag_estado = 1)
			BEGIN
				DELETE FROM flags WHERE flag_id = @iID
				SET @oSalida = 0
				SELECT @oSalida
			END
		ELSE -- Indica que el id ingresado no existe para ser eliminado
			BEGIN
				SET @oSalida = 300
				SELECT @oSalida
			END
	END
END

IF @iOperacion = 'D'
	BEGIN
		SELECT flag_id,flag_desc,flag_estado FROM flags WHERE flag_estado = 1
	END

IF @iOperacion = 'E'
	BEGIN
		SELECT flag_id,flag_desc,flag_estado FROM flags WHERE flag_estado = 1 AND flag_id = @iID
	END

GO


CREATE procedure [dbo].[sp_transacciones_post](@iOperacion CHAR(1),@iMensaje varchar(1000) = null, @iFlag int = null,
										@iUsuario int = null, @iLatitud varchar(50) = null, @iLongitud varchar(50) = null,
										@iIDPost int = null)
AS
BEGIN
	IF @iOperacion = 'A'
		BEGIN
			INSERT INTO posts VALUES(@iMensaje,@iFlag,@iUsuario,GETDATE(),@iLatitud,@iLongitud,1)
		END

	IF @iOperacion = 'B'
		BEGIN
			UPDATE posts SET post_mensaje = @iMensaje WHERE post_id = @iIDPost
		END
	IF @iOperacion = 'C'
		BEGIN
			UPDATE posts SET post_estado = 0 WHERE post_id = @iIDPost
		END
	IF @iOperacion = 'D'
		BEGIN
			SELECT post_id, post_mensaje,id_flag,id_usuario,post_ts,post_latitud,post_longitud,post_estado FROM posts
			WHERE id_usuario = @iUsuario AND post_estado = 1
		END
	IF @iOperacion = 'E'
		BEGIN
			SELECT post_id, post_mensaje,id_flag,id_usuario,post_ts,post_latitud,post_longitud,post_estado FROM posts
			WHERE post_estado = 1
		END
END


GO