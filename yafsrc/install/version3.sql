/* 0.8.0 */

if exists(select * from syscolumns where id=object_id('yaf_System') and name='Culture')
	alter table yaf_System drop column Culture
GO

if exists(select * from syscolumns where id=object_id('yaf_User') and name='Culture')
	alter table yaf_User drop column Culture
GO

if not exists(select * from syscolumns where id=object_id('yaf_System') and name='EmailVerification')
	alter table yaf_System add EmailVerification bit not null default(1)
GO
