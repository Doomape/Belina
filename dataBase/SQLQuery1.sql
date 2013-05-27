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