SET IDENTITY_INSERT [dbo].[RoleName] ON 

INSERT [dbo].[RoleName] ([RoleID], [NameRole]) VALUES (1, N'admin')
INSERT [dbo].[RoleName] ([RoleID], [NameRole]) VALUES (2, N'user')
SET IDENTITY_INSERT [dbo].[RoleName] OFF
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20230812100950_Initial', N'6.0.10')
GO
