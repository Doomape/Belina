  DELETE FROM [Products]
WHERE product_discount like '%Ne%'

alter table Products
alter column product_discount bit

UPDATE Products
SET product_discount = 0

ALTER TABLE Products
ADD product_price decimal NULL


UPDATE Products
SET product_price = 0

-----------------------------------------------


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
			product_image nvarchar(max),
			product_price decimal
		)
	set @end = @start+@num
	
	insert into @res
	EXEC('select * from (select (ROW_NUMBER() OVER (ORDER BY '+@sort_col+' '+@sort_dir+')) row_num, * FROM (select product_id,product_name,company_name,class_name,type_name,attribute_name,product_description,product_discount,product_promotion,product_image,product_price
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
			product_image,
			product_price from @res
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
	EXEC('select count(*) from (select (ROW_NUMBER() OVER (ORDER BY '+@sort_col+' '+@sort_dir+')) row_num, * FROM (select product_name,company_name,class_name,type_name,attribute_name,product_description,product_discount,product_promotion,product_image,product_price
		 from Products,Class,Company,Type,Attributes
		 where Products.company_id=Company.company_id and 
	Products.class_id=Class.class_id and
	Products.type_id=Type.type_id and
	Products.attribute_id=Attributes.attribute_id '+@filters+') as x) as tmp')
	
	select row_num from @res
end
go