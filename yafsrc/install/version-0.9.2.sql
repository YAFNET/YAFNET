/* Version 0.9.2 */

if exists (select * from sysobjects where id = object_id(N'yaf_group_delete') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure yaf_group_delete
GO

create procedure yaf_group_delete(@GroupID int) as
begin
	delete from yaf_ForumAccess where GroupID = @GroupID
	delete from yaf_UserGroup where GroupID = @GroupID
	delete from yaf_Group where GroupID = @GroupID
end
GO
