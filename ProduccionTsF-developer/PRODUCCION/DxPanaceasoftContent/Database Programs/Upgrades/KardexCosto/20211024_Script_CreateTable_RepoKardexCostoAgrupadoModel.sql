
create table RepoKardexCostoAgrupadoModel
(
  id int not null,
  descripcionLinea varchar(80)  null,
  descripcionTipo varchar(80)  null,
  ---- SALDO INICIAL
  saldoInicialCantLbs decimal(20,6) not null default(0),
  saldoInicialCostTotLbs decimal(20,6) not null default(0),
  ---- INGRESO
  ingresoCantLbs decimal(20,6) not null default(0),
  ingresoCostTotLbs decimal(20,6) not null default(0),
  ---- EGRESO
  egresoCantLbs decimal(20,6) not null default(0),
  egresoCostTotLbs decimal(20,6) not null default(0),
  ---- SALDO FINAL
  saldoFinalCantLbs decimal(20,6) not null default(0),
  saldoFinalCostTotLbs decimal(20,6) not null default(0)
)


select * from item 
 select * from InventoryLine 