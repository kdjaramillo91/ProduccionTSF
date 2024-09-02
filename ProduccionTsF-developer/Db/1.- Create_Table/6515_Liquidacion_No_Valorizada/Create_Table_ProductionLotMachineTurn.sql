-- agregar tabla Registro Maquina turno
Create table ProductionLotMachineTurn
(
	id int not null,  -- id ProductionLot
	idMachineProdOpeningDetail int not null, 
	idMachineProdOpening int not null, 
	idTurn int not null,
	idMachineForProd int not null, 	
	timeInit	time null, 	
	timeEnd		time null,
	id_userCreate	int not null,
	dateCreate	datetime not null,
	id_userUpdate	int null,
	dateUpdate	datetime null,
	primary key (id)
)

ALTER TABLE ProductionLotMachineTurn
ADD CONSTRAINT FK_ProductionLotMachineTurn_ProductionLot
FOREIGN KEY (id) REFERENCES ProductionLot(id); 





