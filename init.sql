INSERT INTO wechat_account(nick_name,service_type,spider_role) VALUES('dotNET跨平台',9,'27:12');
INSERT INTO wechat_account(nick_name,service_type,spider_role) VALUES('阿里技术',9,'27:12');
INSERT INTO wechat_account(nick_name,service_type,spider_role) VALUES('上海发布',9,'27:12');
INSERT INTO wechat_account(nick_name,service_type,spider_role) VALUES('申工社',9,'27:12');
INSERT INTO wechat_account(nick_name,service_type,spider_role) VALUES('中国基金报',9,'27:12');
INSERT INTO wechat_account(nick_name,service_type,spider_role) VALUES('数据分析与开发',9,'27:12');
UPDATE wechat_account SET spider_role = '01:10' WHERE id='fd21b5df59a84fe1aca06c6acfb9bfd8';
--show create table wechat_article;
alter table wechat_account default character set utf8;
alter table wechat_article default character set utf8;

alter table wechat_account change nick_name nick_name varchar(128) character set utf8;
alter table wechat_article change digest digest text character set utf8;
alter table wechat_article change title title text character set utf8;
alter table wechat_article change tagid tagid text character set utf8;

SELECT * FROM wechat_article;

SELECT account_id FROM wechat_article GROUP BY account_id WHERE account_id='db3b222f1fb0494cbe72bd99063ff370';

SELECT * FROM wechat_account ORDER BY spider_role for update;

UPDATE wechat_account SET last_update_time = NULL

TRUNCATE TABLE wechat_account;
TRUNCATE TABLE wechat_article;

SELECT * FROM task_start_sign；


UPDATE wechat_article SET download=false,local_path=''