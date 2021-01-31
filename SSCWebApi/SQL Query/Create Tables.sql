Create Table Citys(
Id int identity(1,1) Primary Key,
CityName varchar(500),
CountryId int,
CreatedBy nvarchar(200),
CreatedDate datetime,
ModifiedBy Nvarchar(200),
ModifiedDate datetime
)

Create Table Species(
Id int identity(1,1) Primary Key,
SpeciesName varchar(500),
CreatedBy nvarchar(200),
CreatedDate datetime default(getdate()),
ModifiedBy Nvarchar(200),
ModifiedDate datetime default(getdate())
)


Create Table Lows(
Id int identity(1,1) Primary Key,
LowsTitile nvarchar(200),
LowsDescriptions nvarchar(max),
IsActive bit,
IsDelete bit,
CreatedBy nvarchar(200),
CreatedDate datetime default(getdate()),
ModifiedBy Nvarchar(200),
ModifiedDate datetime default(getdate())
)

create table CityLows(
Id bigint identity(1,1) primary key,
CityId int foreign key references Citys(Id),
LowId int foreign key references Lows(Id),
)


Create Table dbo.Complaints(
Id bigint Primary Key Identity(1,1),
ShopName nvarchar(max),
ShopAddress nvarchar(max),
DateOfInspection datetime,
Comments nvarchar(max) NULL,
Violations nvarchar(max) NULL,
GpsLocations nvarchar(100) NULL,
UserId nvarchar(100) foreign key references AspNetUsers(Id),
City int foreign key references Citys(Id),
SpeciesId int foreign key references Species(Id),
ComplainStatus int,
GroupName nvarchar(500),
IsDelete bit default(0),
IsActive bit default(1),
CreatedBy nvarchar(200),
CreatedDate datetime default(getdate()),
ModifiedBy Nvarchar(200),
ModifiedDate datetime default(getdate())
)



