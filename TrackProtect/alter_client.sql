ALTER TABLE `rhosmove_tpdb`.`client` 
	DROP FOREIGN KEY `client_ibfk_1` ;
ALTER TABLE `rhosmove_tpdb`.`client` 
	ADD COLUMN `accountowner` VARCHAR(100) NOT NULL DEFAULT ''  AFTER `user_id` , 
	ADD COLUMN `twitterid` VARCHAR(100) NOT NULL DEFAULT ''  AFTER `accountowner` , 
	ADD COLUMN `facebookid` VARCHAR(100) NOT NULL DEFAULT ''  AFTER `twitterid` , 
	ADD COLUMN `ownerkind` VARCHAR(20) NOT NULL DEFAULT ''  AFTER `facebookid` , 
	ADD COLUMN `creditcardnr` VARCHAR(20) NOT NULL DEFAULT ''  AFTER `ownerkind` , 
	ADD COLUMN `creditcardccv` VARCHAR(10) NOT NULL DEFAULT ''  AFTER `creditcardnr` , 
	ADD COLUMN `emailreceipt` VARCHAR(100) NOT NULL DEFAULT ''  AFTER `creditcardccv` , 
	ADD COLUMN `referer` TEXT NOT NULL DEFAULT ''  AFTER `emailreceipt` , 
	CHANGE COLUMN `user_id` `user_id` BIGINT(20) NOT NULL DEFAULT 0  , 
  ADD CONSTRAINT `client_ibfk_1`
  FOREIGN KEY (`user_id` )
  REFERENCES `rhosmove_tpdb`.`user` (`user_id` )
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;