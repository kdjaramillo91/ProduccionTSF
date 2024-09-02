create table RepoKardexCostoResumenModel
(
	id int not null,
   descripcion varchar(50),
   cantidadesUnidades decimal(20,6),
   promedioUnidades decimal(20,6),
   cantidadesLibras decimal(20,6),
   promedioLibras decimal(20,6),
   costoTotal decimal(20,6),
   CONSTRAINT [PK_RepoKardexCostoResumenModel] PRIMARY KEY CLUSTERED 
	(
	[id] ASC
	)
);

