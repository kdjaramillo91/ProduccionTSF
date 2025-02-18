/****** Object:  StoredProcedure [dbo].[spRpt_AnalisisCalidadOtrasEspecies]    Script Date: 16/06/2023 14:29:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create or alter PROCEDURE [dbo].[spRpt_AnalisisCalidadOtrasEspecies]
	@id int
as
	Set noCount on
	select qc.id, qcd.resultValue, qa.name 
		from QualityControl qc
		Inner Join QualityControlDetail qcd
			On qcd.id_qualityControl = qc.id
		Inner Join QualityAnalysis qa
			On qa.id = qcd.id_qualityAnalysis
		Inner Join QualityControlAnalysisGroupAnalysis gan
			On gan.id_QualityAnalysis = qa.id
		where  gan.id_QualityControlAnalysisGroup = 3
		And qc.id = @id

