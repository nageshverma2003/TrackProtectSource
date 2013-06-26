USE rhosmove_tpdb;

DROP TABLE IF EXISTS `document`;
DROP TABLE IF EXISTS `register`;
DROP TABLE IF EXISTS `client`;
DROP TABLE IF EXISTS `user`;
DROP TABLE IF EXISTS `setting`;
DROP TABLE IF EXISTS `productprice`;
DROP TABLE IF EXISTS `productdesc`;
DROP TABLE IF EXISTS `product`;

CREATE TABLE `setting`
(
  `setting_id`                                  bigint          not null auto_increment PRIMARY KEY,
  `name`                                        varchar(100)    not null,
  `value`                                       text
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8;

CREATE TABLE `user`
(
    `user_id`                                   bigint          not null auto_increment PRIMARY KEY,
    `username`                                  varchar(50)     not null default '',
    `applicationname`                           varchar(100)    not null default '',
    `email`                                     varchar(200)    not null default '',
    `comment`                                   text            not null default '',
    `password`                                  varchar(200)    not null,
    `passwordquestion`                          varchar(200)    not null default '',
    `passwordanswer`                            varchar(200)    not null,
    `isapproved`                                tinyint(1)      not null default 1,
    `lastactivitydate`                          datetime        not null,
    `lastlogindate`                             datetime        not null,
    `lastpasswordchangeddate`                   datetime        not null,
    `creationdate`                              datetime        not null,
    `isonline`                                  tinyint(1)      not null default 1,
    `islockedout`                               tinyint(1)      not null default 0,
    `lastlockedoutdate`                         datetime        not null,
    `failedpasswordattemptcount`                int             not null default 0,
    `failedpasswordattemptwindowstart`          datetime        not null,
    `failedpasswordanswerattemptcount`          int             not null default 0,
    `failedpasswordanswerattemptwindowstart`    datetime        not null,
    `subscriptiontype`                          int             not null default 0,
    `credits`                                   int             not null default 0,
    `useruid`                                   varchar(100)    not null default '',
    `whmcsclientid`                             int             not null default 0
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE TABLE `client`
(
    `client_id`                                 bigint          not null auto_increment PRIMARY KEY,
    `lastname`                                  varchar(100)    not null default '',
    `firstname`                                 varchar(50)     not null default '',
    `addressline1`                              varchar(100)    not null default '',
    `addressline2`                              varchar(100)    not null default '',
    `zipcode`                                   varchar(10)     not null default '',
    `city`                                      varchar(100)    not null default '',
    `state`                                     varchar(100)    not null default '',
    `country`                                   varchar(100)    not null default '',
    `telephone`                                 varchar(20)     not null default '',
    `cellular`                                  varchar(20)     not null default '',
    `companyname`                               varchar(100)    not null default '',
    `user_id`                                   bigint          not null,
    FOREIGN KEY (`user_id`) REFERENCES `user`(`user_id`)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE TABLE `register`
(
    `register_id`                               bigint          not null auto_increment PRIMARY KEY,
    `certificate`                               varchar(500)    not null,
    `registrationdate`                          datetime        not null,
    `expirationdate`                            datetime        not null,
    `user_id`                                   bigint          not null,
    FOREIGN KEY (`user_id`) REFERENCES `user`(`user_id`)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
ENGINE =InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE TABLE `document`
(
    `document_id`                               bigint          not null auto_increment PRIMARY KEY,
    `register_id`                               bigint          not null default 0,
    `documentname`                              varchar(500)    not null default '',
    `documenthash`                              varchar(100)    not null default '',
    FOREIGN KEY (`register_id`) REFERENCES `register`(`register_id`)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE TABLE `product`
(
    `product_id`                                bigint          not null auto_increment PRIMARY KEY,
    `name`                                      varchar(100)    not null default '',
    `description`                               text            not null default '',
    `credits`                                   int             not null default 0
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE TABLE `productprice`
(
    `productprice_id`                           bigint          not null auto_increment PRIMARY KEY,
    `price`                                     decimal(20,4)   not null default 0,
    `iso_currency`                              varchar(10)     not null default 'EUR',
    `iso2_country`                              varchar(2)      not null default 'NL',
    `product_id`                                bigint          not null default 0,
    FOREIGN KEY (`product_id`) REFERENCES `product`(`product_id`)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

CREATE TABLE `productdesc`
(
    `productdesc_id`                            bigint(20)      NOT NULL auto_increment PRIMARY KEY,
    `product_id`                                bigint(20)      NOT NULL default '0',
    `locale`                                    varchar(20)     NOT NULL default 'en-US',
    `description`                               text            NOT NULL,
    KEY `product_id` (`product_id`),
    CONSTRAINT `product_id`
    FOREIGN KEY (`product_id`) REFERENCES `product` (`product_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8;

INSERT INTO `product` (`name`, `description`, `credits`) VALUES ('Package 1', '<h1>Package 1</h1><p>With this package you buy 5 registration credits.</p><p>This means you can register 5 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.', 10);
INSERT INTO `product` (`name`, `description`, `credits`) VALUES ('Package 2', '<h1>Package 2</h1><p>With this package you buy 10 registration credits.</p><p>This means you can register 10 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.', 20);
INSERT INTO `product` (`name`, `description`, `credits`) VALUES ('Package 3', '<h1>Package 3</h1><p>With this package you buy 15 registration credits.</p><p>This means you can register 15 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.', 30);
INSERT INTO `product` (`name`, `description`, `credits`) VALUES ('Package 4', '<h1>Package 4</h1><p>With this package you buy 20 registration credits.</p><p>This means you can register 20 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.', 40);


INSERT INTO productdesc (`product_id`, `locale`, `description`) VALUES (1, 'en-US', '<h1>Package 1</h1><p>With this package you buy 5 registration credits.</p><p>This means you can register 5 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO productdesc (`product_id`, `locale`, `description`) VALUES (1, 'nl-NL', '<h1>Pakket 1</h1><p>Met dit pakket koopt u 5 registratie credits.</p><p>Dit betekent dat u 5 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');
INSERT INTO productdesc (`product_id`, `locale`, `description`) VALUES (2, 'en-US', '<h1>Package 1</h1><p>With this package you buy 10 registration credits.</p><p>This means you can register 10 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO productdesc (`product_id`, `locale`, `description`) VALUES (2, 'nl-NL', '<h1>Pakket 1</h1><p>Met dit pakket koopt u 10 registratie credits.</p><p>Dit betekent dat u 10 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');
INSERT INTO productdesc (`product_id`, `locale`, `description`) VALUES (3, 'en-US', '<h1>Package 1</h1><p>With this package you buy 15 registration credits.</p><p>This means you can register 15 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO productdesc (`product_id`, `locale`, `description`) VALUES (3, 'nl-NL', '<h1>Pakket 1</h1><p>Met dit pakket koopt u 15 registratie credits.</p><p>Dit betekent dat u 15 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');
INSERT INTO productdesc (`product_id`, `locale`, `description`) VALUES (4, 'en-US', '<h1>Package 1</h1><p>With this package you buy 20 registration credits.</p><p>This means you can register 20 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO productdesc (`product_id`, `locale`, `description`) VALUES (4, 'nl-NL', '<h1>Pakket 1</h1><p>Met dit pakket koopt u 20 registratie credits.</p><p>Dit betekent dat u 20 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');


INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (10.0000, 'EUR', '', 1);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (20.0000, 'EUR', '', 2);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (30.0000, 'EUR', '', 3);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (40.0000, 'EUR', '', 4);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (10.0000, 'EUR', 'NL', 1);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (20.0000, 'EUR', 'NL', 2);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (30.0000, 'EUR', 'NL', 3);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (40.0000, 'EUR', 'NL', 4);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (15.0000, 'USD', 'US', 1);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (30.0000, 'USD', 'US', 2);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (45.0000, 'USD', 'US', 3);
INSERT INTO productprice (`price`, `iso_currency`, `iso2_country`, `product_id`) VALUES (60.0000, 'USD', 'US', 4);
