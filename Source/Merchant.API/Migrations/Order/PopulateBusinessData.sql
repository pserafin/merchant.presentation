GO
SET IDENTITY_INSERT [product].[Product] ON 
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (0, N'Subscription', 999, CAST(3 AS Decimal(18, 2)), 0)
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (1, N'Apple', 100, CAST(1.10 AS Decimal(18, 2)), 1)
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (2, N'Orange', 100, CAST(1.20 AS Decimal(18, 2)), 1)
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (3, N'Banana', 100, CAST(1.30 AS Decimal(18, 2)), 1)
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (4, N'Pinapple', 100, CAST(1.40 AS Decimal(18, 2)), 1)
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (5, N'Raspberry', 1000, CAST(0.20 AS Decimal(18, 2)), 1)
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (6, N'Strawberry', 1000, CAST(0.10 AS Decimal(18, 2)), 1)
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (7, N'Cherry', 100, CAST(3.20 AS Decimal(18, 2)), 1)
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (8, N'Mango', 100, CAST(7.80 AS Decimal(18, 2)), 1)
GO
INSERT [product].[Product] ([Id], [Name], [Quantity], [Price], [IsEnabled]) VALUES (9, N'Coconut', 100, CAST(4.30 AS Decimal(18, 2)), 1)
GO
SET IDENTITY_INSERT [product].[Product] OFF
GO
