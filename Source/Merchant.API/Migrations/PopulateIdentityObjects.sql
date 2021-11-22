go
-- CREATE SELLER AND 4 CUSTOMERS => ALL PASSWORDS ARE SET TO '1234'
SET IDENTITY_INSERT [dbo].[AspNetRoles] ON 
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (4, N'Customer', N'Customer', NULL)
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (256, N'Administrator', N'Administrator', NULL)
GO
SET IDENTITY_INSERT [dbo].[AspNetRoles] OFF
GO
SET IDENTITY_INSERT [dbo].[AspNetUsers] ON 
GO
SET IDENTITY_INSERT [dbo].[AspNetUsers] ON 
GO
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Address]) VALUES (1, N'Firstname', N'Lastname', N'admin', N'ADMMIN', N'admin@ingenico.com', N'ADMIN@INGENICO.COM', 0, N'AQAAAAEAACcQAAAAECv0kclhznC731L+Q/3P42aGiCY7keE7y6StxcLqksaWKy9VerP4s/SpoHFgHLqoVw==', N'25WEHEJJLWK5PHEUJEIP2D3BIQHVVBQ6', N'9e06f519-9947-4283-b138-28fd56091808', NULL, 0, 0, NULL, 1, 0, 'CountryCode|State|Zip|City|Street|HouseNumber')
GO
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Address]) VALUES (2, N'Firstname', N'Lastname', N'customer1', N'CUSTOMER1', N'customer1@ingenico.com', N'CUSTOMER1@INGENICO.COM', 0, N'AQAAAAEAACcQAAAAEM1/hRQNmcKGFpuUD2wyDLLwMmWeydmOIvFhKuMd4JP0IFo4nMj39Acosol9HdQeAg==', N'EEL4G3XCTESWWX5WFDHYTVVTPQTHIJZC', N'03b0c706-d601-48cb-8ccb-55d05af74cf1', NULL, 0, 0, NULL, 1, 0, 'PL|lubelskie|20-869|Lublin|Szeligowskiego|6b')
GO
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Address]) VALUES (3, N'Firstname', N'Lastname', N'customer2', N'CUSTOMER2', N'customer2@ingenico.com', N'CUSTOMER2@INGENICO.COM', 0, N'AQAAAAEAACcQAAAAEDD/5yNIV0csxQ5IHi8T2wcjaOCZR27VYtemMGC3eq9bu2JJUmlKF2TWGN503MCWsA==', N'UDX5JPZ5YDCOEDDO4K4UBZLSV5GJ36ZG', N'7b778d78-6c86-4d7e-8296-be20637856e3', NULL, 0, 0, NULL, 1, 0, 'PL|lubelskie|20-869|Lublin|Szeligowskiego|6b')
GO
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Address]) VALUES (4, N'Firstname', N'Lastname', N'customer3', N'CUSTOMER3', N'customer3@ingenico.com', N'CUSTOMER3@INGENICO.COM', 0, N'AQAAAAEAACcQAAAAEDVW/50SO1est6Tc5z7DAMRfqCK0UtKFsooZKx2ET9ZPgR4hrs6f8H82NjBIf3icRQ==', N'R2LD7KLGHJPPMOEFKY266XTORG3OX63E', N'a3c8287c-da21-4adf-a49b-a45d7cdb9a31', NULL, 0, 0, NULL, 1, 0, 'PL|lubelskie|20-869|Lublin|Szeligowskiego|6b')
GO
INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Address]) VALUES (5, N'Firstname', N'Lastname', N'customer4', N'CUSTOMER4', N'customer4@ingenico.com', N'CUSTOMER4@INGENICO.COM', 0, N'AQAAAAEAACcQAAAAEMW4sgvjrNHQaobqmwBueJCZ6+RTnwQnYaj4Bvi/H9OJfHpnHV2zInlvVhVX44ttbA==', N'4MUMAJSJBMOCCU6HTOCSIFJKAMQEOHJR', N'7fc00164-2ad5-409e-a259-4932d6d97143', NULL, 0, 0, NULL, 1, 0, '|||||')
GO
SET IDENTITY_INSERT [dbo].[AspNetUsers] OFF
GO
SET IDENTITY_INSERT [dbo].[AspNetUsers] OFF
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (2, 4)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (3, 4)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (4, 4)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (5, 4)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (1, 256)
GO

