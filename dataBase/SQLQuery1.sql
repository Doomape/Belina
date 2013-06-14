drop table Administrator

CREATE TABLE Administrator
(
[user_id] int PRIMARY KEY IDENTITY,
[user_name] nvarchar(255) NOT NULL,
[user_pass] nvarchar(255) NOT NULL,
[user_email] nvarchar(255) NOT NULL
)



==========21.05.2013===========

UPDATE Products
SET Products.product_image = '/Content/companies/jub/DSC04916_done_thumb.png'
WHERE Products.product_name like N'Вебер деко Поликолор бел 8/1';

UPDATE Products
SET Products.product_image = '/Content/companies/jub/jub_fuga_done_thumb.png'
WHERE Products.product_name like N'Вебер деко поликолор голд база 1(А) 5/1';

UPDATE Products
SET Products.product_image = '/Content/companies/jub/jub_akrilen_malter_done_thumb.png'
WHERE Products.product_name like N'Вебер деко поликолор  голд  база 1(А) 8/1';

UPDATE Products
SET Products.product_description = N'База 1(а) наменета за ...и содржи ..... Гаранција до ... увозник Белина - Скопје'
WHERE Products.product_name like N'Вебер деко поликолор  голд  база 1(А) 8/1';

=========end 21==========


========26.05.2013=============

ALTER TABLE Attributes ALTER COLUMN attribute_id INT NULL;
ALTER TABLE Attributes ALTER COLUMN company_id INT NULL;
ALTER TABLE Attributes ALTER COLUMN [type_id] INT NULL;


sp_rename 'Attributes','Product_Attribute';

CREATE TABLE Attributes
(
attribute_id int NOT NULL PRIMARY KEY,
attribute_name nvarchar(255) NOT NULL
)



INSERT INTO Attributes
SELECT Distinct Product_Attribute.attribute_id, Product_Attribute.attribute_name
FROM Product_Attribute
WHERE Product_Attribute.attribute_name IS NOT NULL

ALTER TABLE Product_Attribute DROP COLUMN attribute_name


ALTER TABLE Product_Attribute
ADD id INT IDENTITY

ALTER TABLE Product_Attribute
ADD CONSTRAINT PK_Product_Attribute
PRIMARY KEY(id)

ALTER TABLE Type DROP COLUMN class_name

=========end 26=================


============29.05.2013==============

Vo Product_Attrubte da se vidat koi se so attribute_id = 1070100.

===========end 29===================




================06.07.2013 Stored procedure=====================

if OBJECT_ID(N'dbo.spGetProducts') is not null
	drop procedure dbo.spGetProducts
go

CREATE procedure [dbo].[spGetProducts](@sort_col varchar(100), @sort_dir varchar(4), @start int, @num int, @filters nvarchar(2000)) as
begin
	declare @end int
	declare	@res table (
			row_num int,
			product_id int,
			product_name nvarchar(max),
			company_name nvarchar(max),
			class_name nvarchar(max),
			[type_name] nvarchar(max),
			attribute_name nvarchar(max),
			product_description nvarchar(max),
			product_discount nvarchar(max),
			product_promotion nvarchar(max),
			product_image nvarchar(max)
		)
	set @end = @start+@num
	
	insert into @res
	EXEC('select * from (select (ROW_NUMBER() OVER (ORDER BY '+@sort_col+' '+@sort_dir+')) row_num, * FROM (select product_id,product_name,company_name,class_name,type_name,attribute_name,product_description,product_discount,product_promotion,product_image
		 from Products,Class,Company,Type,Attributes
		 where Products.company_id=Company.company_id and 
	Products.class_id=Class.class_id and
	Products.type_id=Type.type_id and
	Products.attribute_id=Attributes.attribute_id '+@filters+') as x) as tmp where row_num between '+@start+' and '+@end)
	
	select row_num,
		product_id,
			product_name,
			company_name,
			class_name,
			[type_name],
			attribute_name,
			product_description,
			product_discount,
			product_promotion,
			product_image from @res
end
go

--exec dbo.spGetProducts 'product_name','desc',0,50,N' and product_name like N''%шуб%'''

if OBJECT_ID(N'dbo.spCountProducts') is not null
	drop procedure dbo.spCountProducts
go

CREATE procedure [dbo].[spCountProducts](@sort_col varchar(100), @sort_dir varchar(4), @filters nvarchar(2000)) as
begin
	declare @end int
	declare	@res table (
			row_num int
		)
	
	insert into @res
	EXEC('select count(*) from (select (ROW_NUMBER() OVER (ORDER BY '+@sort_col+' '+@sort_dir+')) row_num, * FROM (select product_name,company_name,class_name,type_name,attribute_name,product_description,product_discount,product_promotion,product_image
		 from Products,Class,Company,Type,Attributes
		 where Products.company_id=Company.company_id and 
	Products.class_id=Class.class_id and
	Products.type_id=Type.type_id and
	Products.attribute_id=Attributes.attribute_id '+@filters+') as x) as tmp')
	
	select row_num from @res
end
go

--exec dbo.spCountProducts 'product_name','desc',N' and product_name like N''%шуб%'''
================end 03=====================




================09.06 Added new table================

CREATE TABLE Class_Type
(
class_id int NULL,
type_id int NULL
)

ALTER TABLE Class_Type
ADD id INT IDENTITY

ALTER TABLE Class_Type
ADD CONSTRAINT PK_Class_Type
PRIMARY KEY(id)



INSERT INTO Class_Type
SELECT Distinct Type.class_id, Type.type_id
FROM Type
WHERE Type.type_id IS NOT NULL


ALTER TABLE Type DROP COLUMN class_id

==============end 09.06========================


==============11.06 changed column type on dependencies tables==========


ALTER TABLE Class_Type
ALTER COLUMN type_id int NOT NULL

ALTER TABLE Class_Type
ALTER COLUMN class_id int NOT NULL

ALTER TABLE Company_Class
ALTER COLUMN class_id int NOT NULL

ALTER TABLE Company_Class
ALTER COLUMN company_id int NOT NULL

ALTER TABLE Product_Attribute
ALTER COLUMN company_id int NOT NULL

ALTER TABLE Product_Attribute
ALTER COLUMN type_id int NOT NULL

ALTER TABLE Type_Company
ALTER COLUMN type_id int NOT NULL

ALTER TABLE Type_Company
ALTER COLUMN company_id int NOT NULL

==============end 11.06==============================================


===============13.06.2013============Delete duplicate values================


;with C as
(
  select row_number() over(partition by product_name,company_id,class_id,type_id,attribute_id 
                           order by product_name,company_id,class_id,type_id,attribute_id) as rn
  from Products
)
delete C
where rn > 1


update Product_Attribute
set attribute_id = (
    select min(attribute_id)
    from Attributes a
    where a.attribute_name=(select attribute_name from Attributes a2 where a2.attribute_id=Product_Attribute.attribute_id)
);

update Products
set attribute_id = (
    select min(attribute_id)
    from Attributes a
    where a.attribute_name=(select attribute_name from Attributes a2 where a2.attribute_id=Products.attribute_id)
);

DELETE
FROM Attributes
WHERE attribute_id NOT IN
(
    SELECT MIN(attribute_id)
    FROM Attributes
    GROUP BY attribute_name
);

delete from Type_Company where id=233

update Type_Company
set type_id = (
    select min(type_id)
    from Type a
    where a.type_name=(select type_name from Type a2 where a2.type_id=Type_Company.type_id)
);

update Class_Type
set type_id = (
    select min(type_id)
    from Type a
    where a.type_name=(select type_name from Type a2 where a2.type_id=Class_Type.type_id)
);

update Products
set type_id = (
    select min(type_id)
    from Type a
    where a.type_name=(select type_name from Type a2 where a2.type_id=Products.type_id)
);


delete from Product_Attribute where id=1885

delete from Product_Attribute where id=1886

update Product_Attribute
set type_id = (
    select min(type_id)
    from Type a
    where a.type_name=(select type_name from Type a2 where a2.type_id=Product_Attribute.type_id)
);

DELETE
FROM Type
WHERE type_id NOT IN
(
    SELECT MIN(type_id)
    FROM Type
    GROUP BY type_name
);

delete from Company_Class where id=154

delete from Products where product_id = 3661
delete from Products where product_id = 3662
delete from Products where product_id = 3663



=========13.06.2013 end ======================================

asdad
