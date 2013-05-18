drop table Administrator

CREATE TABLE Administrator
(
[user_id] int PRIMARY KEY IDENTITY,
[user_name] nvarchar(255) NOT NULL,
[user_pass] nvarchar(255) NOT NULL,
[user_email] nvarchar(255) NOT NULL
)