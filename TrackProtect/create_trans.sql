DROP TABLE IF EXISTS `transaction`;
CREATE  TABLE `transaction` (
  `transaction_id` BIGINT NOT NULL AUTO_INCREMENT ,
  `user_id` BIGINT NOT NULL DEFAULT 0 ,
  `amount` DECIMAL(10,4) NOT NULL DEFAULT 0.0 ,
  `date` DATETIME NULL ,
  `description` VARCHAR(100) NOT NULL DEFAULT '' ,
  `product_id` BIGINT NOT NULL DEFAULT 0 ,
  `status` VARCHAR(100) NOT NULL DEFAULT 'OPEN' ,
  `statuscode` VARCHAR(100) NOT NULL DEFAULT '',
  `merchant` VARCHAR(50) NOT NULL DEFAULT '',
  `paymentid` VARCHAR(100) NOT NULL DEFAULT '',
  `transid` VARCHAR(100) NOT NULL DEFAULT '',
  `paymentmethod` VARCHAR(100) NOT NULL DEFAULT '',
  PRIMARY KEY (`transaction_id`) ,
  INDEX `user_id` (`user_id` ASC) ,
  INDEX `product_id` (`product_id` ASC) ,
  CONSTRAINT `fk_user_id`
    FOREIGN KEY (`user_id` )
    REFERENCES `user` (`user_id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_product_id`
    FOREIGN KEY (`product_id` )
    REFERENCES `product` (`product_id` )
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_general_ci;
