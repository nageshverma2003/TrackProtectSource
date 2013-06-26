DROP TABLE IF EXISTS `setting`;
DROP TABLE IF EXISTS `document`;
DROP TABLE IF EXISTS `register`;
DROP TABLE IF EXISTS `client`;
DROP TABLE IF EXISTS `productdesc`;
DROP TABLE IF EXISTS `productprice`;
DROP TABLE IF EXISTS `product`;
DROP TABLE IF EXISTS `credithistory`;
DROP TABLE IF EXISTS `user`;


CREATE TABLE `setting` (
  `setting_id` bigint(20) NOT NULL auto_increment,
  `name` varchar(100) NOT NULL,
  `value` text,
  PRIMARY KEY  (`setting_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `user` (
  `user_id` bigint(20) NOT NULL auto_increment,
  `username` varchar(50) NOT NULL default '',
  `applicationname` varchar(100) NOT NULL default '',
  `email` varchar(200) NOT NULL default '',
  `comment` text NOT NULL,
  `password` varchar(200) NOT NULL,
  `passwordquestion` varchar(200) NOT NULL default '',
  `passwordanswer` varchar(200) NOT NULL,
  `isapproved` tinyint(1) NOT NULL default '1',
  `lastactivitydate` datetime NOT NULL,
  `lastlogindate` datetime NOT NULL,
  `lastpasswordchangeddate` datetime NOT NULL,
  `creationdate` datetime NOT NULL,
  `isonline` tinyint(1) NOT NULL default '1',
  `islockedout` tinyint(1) NOT NULL default '0',
  `lastlockedoutdate` datetime NOT NULL,
  `failedpasswordattemptcount` int(11) NOT NULL default '0',
  `failedpasswordattemptwindowstart` datetime NOT NULL,
  `failedpasswordanswerattemptcount` int(11) NOT NULL default '0',
  `failedpasswordanswerattemptwindowstart` datetime NOT NULL,
  `subscriptiontype` int(11) NOT NULL default '0',
  `credits` int(11) NOT NULL default '0',
  `useruid` varchar(100) NOT NULL default '',
  `whmcsclientid` int(11) NOT NULL default '0',
  PRIMARY KEY  (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `client` (
  `client_id` bigint(20) NOT NULL auto_increment,
  `lastname` varchar(100) NOT NULL default '',
  `firstname` varchar(50) NOT NULL default '',
  `addressline1` varchar(100) NOT NULL default '',
  `addressline2` varchar(100) NOT NULL default '',
  `zipcode` varchar(10) NOT NULL default '',
  `city` varchar(100) NOT NULL default '',
  `state` varchar(100) NOT NULL default '',
  `country` varchar(100) NOT NULL default '',
  `telephone` varchar(20) NOT NULL default '',
  `cellular` varchar(20) NOT NULL default '',
  `companyname` varchar(100) NOT NULL default '',
  `user_id` bigint(20) NOT NULL default '0',
  `accountowner` varchar(100) NOT NULL default '',
  `senacode` varchar(100) NOT NULL default '',
  `isrccode` varchar(100) NOT NULL default '',
  `soniallid` varchar(100) NOT NULL default '',
  `twitterid` varchar(100) NOT NULL default '',
  `facebookid` varchar(100) NOT NULL default '',
  `ownerkind` varchar(20) NOT NULL default '',
  `creditcardnr` varchar(20) NOT NULL default '',
  `creditcardcvv` varchar(10) NOT NULL default '',
  `emailreceipt` varchar(100) NOT NULL default '',
  `referer` text NOT NULL,
  PRIMARY KEY  (`client_id`),
  KEY `client_ibfk_1` (`user_id`),
  CONSTRAINT `client_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `register` (
  `register_id` bigint(20) NOT NULL auto_increment,
  `certificate` varchar(500) NOT NULL,
  `registrationdate` datetime NOT NULL,
  `expirationdate` datetime NOT NULL,
  `user_id` bigint(20) NOT NULL,
  PRIMARY KEY  (`register_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `register_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `document` (
  `document_id` bigint(20) NOT NULL auto_increment,
  `register_id` bigint(20) NOT NULL default '0',
  `documentname` varchar(500) NOT NULL default '',
  `documenthash` varchar(100) NOT NULL default '',
  PRIMARY KEY  (`document_id`),
  KEY `register_id` (`register_id`),
  CONSTRAINT `document_ibfk_1` FOREIGN KEY (`register_id`) REFERENCES `register` (`register_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `product` (
  `product_id` bigint(20) NOT NULL auto_increment,
  `name` varchar(100) NOT NULL default '',
  `description` text NOT NULL,
  `credits` int(11) NOT NULL default '0',
  PRIMARY KEY  (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `productdesc` (
  `productdesc_id` bigint(20) NOT NULL auto_increment,
  `product_id` bigint(20) NOT NULL default '0',
  `locale` varchar(20) NOT NULL default 'en-US',
  `title` text NOT NULL,
  `description` text NOT NULL,
  PRIMARY KEY  (`productdesc_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `product_id` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `productprice` (
  `productprice_id` bigint(20) NOT NULL auto_increment,
  `price` decimal(20,4) NOT NULL default '0.0000',
  `iso_currency` varchar(10) NOT NULL default 'EUR',
  `iso2_country` varchar(2) NOT NULL default 'NL',
  `product_id` bigint(20) NOT NULL default '0',
  PRIMARY KEY  (`productprice_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `productprice_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `credithistory` (
  `credithistory_id` bigint(20) NOT NULL,
  `user_id` bigint(20) NOT NULL default '0',
  `product_id` bigint(20) NOT NULL default '0',
  `credits` int(11) NOT NULL default '0',
  `purchasedate` datetime default NULL,
  `expired` tinyint(1) NOT NULL default '0',
  PRIMARY KEY  (`credithistory_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `user_id` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


/*
-- Query: SELECT * FROM `rhosmove_tpdb`.`user`
LIMIT 0, 1000

-- Date: 2012-03-13 13:35
*/
INSERT INTO `user` (user_id,username,applicationname,email,comment,password,passwordquestion,passwordanswer,isapproved,lastactivitydate,lastlogindate,lastpasswordchangeddate,creationdate,isonline,islockedout,lastlockedoutdate,failedpasswordattemptcount,failedpasswordattemptwindowstart,failedpasswordanswerattemptcount,failedpasswordanswerattemptwindowstart,subscriptiontype,credits,useruid,whmcsclientid) VALUES (1,'caz','/','c.v.zon@softint.nl','','b0334e7687985ddfc9b9c6b7dde6256a','','d41d8cd98f00b204e9800998ecf8427e',1,'2012-01-22 11:16:04','2012-03-11 21:49:35','2012-01-22 11:16:04','2012-01-22 11:16:04',1,0,'2012-01-22 11:16:04',0,'2012-01-22 11:16:04',0,'2012-01-22 11:16:04',0,20,'51581a97d3c94e25bd4d597c64b1f421',0);

/*
-- Query: SELECT * FROM `rhosmove_tpdb`.`client`
LIMIT 0, 1000

-- Date: 2012-03-13 13:36
*/
INSERT INTO `client` (client_id,lastname,firstname,addressline1,addressline2,zipcode,city,state,country,telephone,cellular,companyname,user_id,accountowner,senacode,isrccode,twitterid,facebookid,ownerkind,creditcardnr,creditcardccv,emailreceipt,referer) VALUES (1,'van Zon','Caspar','Nicolaas Damesstraat 11','','2171 KA','S','ZH','Netherlands','0252-865429','06-22758054','',1,'','','','','','','','','','');

/*
-- Query: SELECT * FROM `rhosmove_tpdb`.`product`
LIMIT 0, 1000

-- Date: 2012-03-13 13:34
*/
INSERT INTO `product` (product_id,name,description,credits) VALUES (1,'TrackProtect Starter','<h1>TrackProtect Starter</h1><p>With this package you buy 5 registration credits.</p><p>This means you can register 5 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.',10);
INSERT INTO `product` (product_id,name,description,credits) VALUES (2,'TrackProtect Medium','<h1>TrackProtect Medium</h1><p>With this package you buy 10 registration credits.</p><p>This means you can register 10 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.',20);
INSERT INTO `product` (product_id,name,description,credits) VALUES (3,'TrackProtect Pro','<h1>TrackProtect Pro</h1><p>With this package you buy 15 registration credits.</p><p>This means you can register 15 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.',30);
INSERT INTO `product` (product_id,name,description,credits) VALUES (4,'TrackProtect Bulk','<h1>TrackProtect Bulk</h1><p>With this package you buy 20 registration credits.</p><p>This means you can register 20 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.',40);

/*
-- Query: SELECT * FROM `rhosmove_tpdb`.`productdesc`
LIMIT 0, 1000

-- Date: 2012-03-13 13:33
*/
INSERT INTO `productdesc` (productdesc_id,product_id,locale,title,description) VALUES (1,1,'en-US','<h1 class="productLabel">Thank you for choosing the <span style="color: orange;">TrackProtect Starter-plan</span></h1>','<h1>What''s in it for you</h1><p>With this package you buy 5 registration credits.</p><p>This means you can register 5 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO `productdesc` (productdesc_id,product_id,locale,title,description) VALUES (2,1,'nl-NL','<h1 class="productLabel">Thank you for choosing the <span style="color: orange;">TrackProtect Starter-plan</span></h1>','<h1>What''s in it for you</h1><p>Met dit pakket koopt u 5 registratie credits.</p><p>Dit betekent dat u 5 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');
INSERT INTO `productdesc` (productdesc_id,product_id,locale,title,description) VALUES (3,2,'en-US','<h1 class="productLabel">Thank you for choosing the <span style="color: orange;">TrackProtect Medium-plan</span></h1>','<h1>What''s in it for you</h1><p>With this package you buy 10 registration credits.</p><p>This means you can register 10 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO `productdesc` (productdesc_id,product_id,locale,title,description) VALUES (4,2,'nl-NL','<h1 class="productLabel">Thank you for choosing the <span style="color: orange;">TrackProtect Medium-plan</span></h1>','<h1>What''s in it for you</h1><p>Met dit pakket koopt u 10 registratie credits.</p><p>Dit betekent dat u 10 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');
INSERT INTO `productdesc` (productdesc_id,product_id,locale,title,description) VALUES (5,3,'en-US','<h1 class="productLabel">Thank you for choosing the <span style="color: orange;">TrackProtect Pro-plan</span></h1>','<h1>What''s in it for you</h1><p>With this package you buy 15 registration credits.</p><p>This means you can register 15 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO `productdesc` (productdesc_id,product_id,locale,title,description) VALUES (6,3,'nl-NL','<h1 class="productLabel">Thank you for choosing the <span style="color: orange;">TrackProtect Pro-plan</span></h1>','<h1>What''s in it for you</h1><p>Met dit pakket koopt u 15 registratie credits.</p><p>Dit betekent dat u 15 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');
INSERT INTO `productdesc` (productdesc_id,product_id,locale,title,description) VALUES (7,4,'en-US','<h1 class="productLabel">Thank you for choosing the <span style="color: orange;">TrackProtect Medium-plan</span></h1>','<h1>What''s in it for you</h1><p>With this package you buy 20 registration credits.</p><p>This means you can register 20 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO `productdesc` (productdesc_id,product_id,locale,title,description) VALUES (8,4,'nl-NL','<h1 class="productLabel">Thank you for choosing the <span style="color: orange;">TrackProtect Medium-plan</span></h1>','<h1>What''s in it for you</h1><p>Met dit pakket koopt u 20 registratie credits.</p><p>Dit betekent dat u 20 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');

/*
-- Query: SELECT * FROM `rhosmove_tpdb`.`productprice`
LIMIT 0, 1000

-- Date: 2012-03-13 13:33
*/
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (1,'10.0000','EUR','',1);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (2,'20.0000','EUR','',2);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (3,'30.0000','EUR','',3);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (4,'40.0000','EUR','',4);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (5,'10.0000','EUR','NL',1);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (6,'20.0000','EUR','NL',2);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (7,'30.0000','EUR','NL',3);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (8,'40.0000','EUR','NL',4);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (9,'15.0000','USD','US',1);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (10,'30.0000','USD','US',2);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (11,'45.0000','USD','US',3);
INSERT INTO `productprice` (productprice_id,price,iso_currency,iso2_country,product_id) VALUES (12,'60.0000','USD','US',4);
