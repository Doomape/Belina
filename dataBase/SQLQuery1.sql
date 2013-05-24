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