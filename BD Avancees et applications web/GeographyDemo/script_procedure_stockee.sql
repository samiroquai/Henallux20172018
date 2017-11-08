USE [mdpschsa_labaccconc]
GO
/****** Object:  StoredProcedure [dbo].[usp_GetThingsAround]    Script Date: 08-11-17 12:11:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Samuel scholtes
-- Create date: 08-11-2017
-- Description:	Procédure retournant tous les enregistrements inclus dans un rayon donné autour d'un point donné
-- =============================================
ALTER PROCEDURE [dbo].[usp_GetThingsAround] 
	-- Add the parameters for the stored procedure here
	@lat decimal(10,5) = 0, 
	@lon decimal(10,5) = 0,
	@rayonEnMetres decimal (10,2) = 5000
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Id, Nom, Coordonnees.STDistance(geography::STPointFromText('POINT('+CAST (@lat AS varchar(20))+' '+CAST(@lon AS varchar(20))+')', 4326)) FROM GeoDemo WHERE Coordonnees.STDistance(geography::STPointFromText('POINT('+CAST (@lat AS varchar(20))+' '+CAST(@lon AS varchar(20))+')', 4326)) <= @rayonEnMetres;
END
