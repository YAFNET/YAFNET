/* Version 0.7.1 */

if exists (select * from sysobjects where id = object_id(N'yaf_system_updateversion') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_system_updateversion
GO

create procedure yaf_system_updateversion(
	@Version	int,
	@VersionName	varchar(50)
) as
begin
	update yaf_System set
		Version = @Version,
		VersionName = @VersionName
end
GO
