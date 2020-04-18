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
	[usr_id] [int] NOT NULL,
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
FROM usr_usuario T0 WHERE T0.usr_correo = @usr_email AND T0.usr_password = @usr_clave

END


GO
