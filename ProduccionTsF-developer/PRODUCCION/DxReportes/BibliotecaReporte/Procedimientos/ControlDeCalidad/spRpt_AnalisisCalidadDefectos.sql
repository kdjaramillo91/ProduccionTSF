/****** Object:  StoredProcedure [dbo].[spRpt_AnalisisCalidadDefectos]   Script Date: 16/06/2023 14:27:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create or ALTER   PROCEDURE [dbo].[spRpt_AnalisisCalidadDefectos]
	@id int
as
	Set noCount on
	select qc.id, qcd.resultValue, qa.name, qcd.otherResultValue unidades
		from QualityControl qc
		Inner Join QualityControlDetail qcd
			On qcd.id_qualityControl = qc.id
		Inner Join QualityAnalysis qa
			On qa.id = qcd.id_qualityAnalysis
		Inner Join QualityControlAnalysisGroupAnalysis gan
			On gan.id_QualityAnalysis = qa.id
		where  gan.id_QualityControlAnalysisGroup = 2
		And qc.id = @id

