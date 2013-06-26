DROP TABLE IF EXISTS vault.document;
DROP TABLE IF EXISTS vault.register;
DROP TABLE IF EXISTS vault.client;
DROP TABLE IF EXISTS vault."user";
DROP TABLE IF EXISTS vault.setting;
DROP TABLE IF EXISTS vault.productprice;
DROP TABLE IF EXISTS vault.productdesc;
DROP TABLE IF EXISTS vault.product;

CREATE TABLE vault.setting
(
  setting_id	serial			NOT NULL,
  key		character varying(100)	NOT NULL,
  value		text			NOT NULL DEFAULT '',
  CONSTRAINT pk_setting_id PRIMARY KEY (setting_id)
)
WITH (
  OIDS=FALSE
);

ALTER TABLE vault.setting
  OWNER TO postgres;

CREATE TABLE vault."user"
(
  user_id serial NOT NULL,
  username character varying(50) NOT NULL NOT NULL DEFAULT '',
  applicationname character varying(100) NOT NULL DEFAULT '',
  email character varying(200) NOT NULL NOT NULL DEFAULT '',
  comment text NOT NULL DEFAULT '',
  password character varying(200) NOT NULL DEFAULT md5(''),
  passwordquestion character varying(200) NOT NULL DEFAULT '',
  passwordanswer character varying(200) NOT NULL DEFAULT md5(''),
  isapproved boolean NOT NULL DEFAULT 't',
  lastactivitydate timestamp with time zone NOT NULL DEFAULT current_timestamp,
  lastlogindate	time with time zone NOT NULL DEFAULT current_timestamp,
  lastpasswordchangeddate timestamp with time zone NOT NULL DEFAULT current_timestamp,
  creationdate 	timestamp with time zone NOT NULL DEFAULT current_timestamp,
  isonline boolean NOT NULL DEFAULT 'f',
  islockedout boolean NOT NULL DEFAULT 'f',
  lastlockedoutdate timestamp with time zone NOT NULL DEFAULT current_timestamp,
  failedpasswordattemptcount integer NOT NULL DEFAULT 0,
  failedpasswordattemptwindowstart timestamp with time zone NOT NULL DEFAULT current_timestamp,
  failedpasswordanswerattemptcount integer NOT NULL DEFAULT 0,
  failedpasswordanswerattemptwindowstart timestamp with time zone NOT NULL DEFAULT current_timestamp,
  subscriptiontype integer NOT NULL DEFAULT 0,
  credits integer NOT NULL DEFAULT 0,
  useruid character varying(100) NOT NULL,
  whmcsclientid integer NOT NULL DEFAULT 0,
  CONSTRAINT pk_user_id PRIMARY KEY (user_id )
)
WITH (
  OIDS=FALSE
);
ALTER TABLE vault."user"
  OWNER TO postgres;

CREATE TABLE vault.client
(
  client_id	serial			NOT NULL,
  lastname	character varying(100)	NOT NULL,
  firstname	character varying( 50)	NOT NULL,
  insertion	character varying( 20)	NOT NULL DEFAULT '',
  addressline1	character varying(100)	NOT NULL DEFAULT '',
  addressline2	character varying(100)	NOT NULL DEFAULT '',
  zipcode	character varying( 10)	NOT NULL DEFAULT '',
  city		character varying(100)	NOT NULL DEFAULT '',
  state		character varying(100)	NOT NULL DEFAULT '',
  country	character varying(100)	NOT NULL DEFAULT '',
  telephone	character varying( 20)	NOT NULL DEFAULT '',
  cellular	character varying( 20)	NOT NULL DEFAULT '',
  companyname	character varying(100)	NOT NULL DEFAULT '',
  user_id	integer			NOT NULL,
  CONSTRAINT pk_client_id PRIMARY KEY (client_id ),
  CONSTRAINT fk_client_id_user_id FOREIGN KEY (user_id)
      REFERENCES vault."user" (user_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE vault.client
  OWNER TO postgres;
  
CREATE TABLE vault.register
(
  register_id		serial NOT NULL,
  certificate		character varying(500) NOT NULL,
  registrationdate	timestamp with time zone NOT NULL,
  expirationdate	timestamp with time zone NOT NULL,
  user_id		integer NOT NULL,
  CONSTRAINT pk_register_id PRIMARY KEY (register_id ),
  CONSTRAINT fk_user_register FOREIGN KEY (user_id)
      REFERENCES vault."user" (user_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
);
ALTER TABLE vault.register
  OWNER TO postgres;
  
CREATE TABLE vault.document
(
  document_id		serial NOT NULL,
  register_id		integer NOT NULL,
  documentname		character varying(500) NOT NULL,
  documenthash		character varying(100) NOT NULL,
  CONSTRAINT pk_document_id PRIMARY KEY (document_id ),
  CONSTRAINT fk_register_document FOREIGN KEY (register_id)
      REFERENCES vault.register (register_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
);
ALTER TABLE vault.document
  OWNER TO postgres;
GRANT ALL ON TABLE vault.document TO postgres;

CREATE TABLE vault.product
(
  product_id serial NOT NULL,
  name character varying(100) NOT NULL DEFAULT ''::character varying,
  description text DEFAULT ''::text,
  credits integer NOT NULL DEFAULT 0,
  CONSTRAINT pk_product_id PRIMARY KEY (product_id )
)
WITH (
  OIDS=FALSE
);
ALTER TABLE vault.product
  OWNER TO postgres;
GRANT ALL ON TABLE vault.product TO postgres;

CREATE TABLE vault.productprice
(
  productprice_id serial NOT NULL,
  price numeric(20,4) NOT NULL DEFAULT 0,
  iso_currency character varying(10) NOT NULL DEFAULT 'EUR'::character varying,
  iso2_country character varying(2) NOT NULL DEFAULT 'NL'::character varying,
  product_id integer NOT NULL DEFAULT 0,
  CONSTRAINT pk_productprice_id PRIMARY KEY (productprice_id ),
  CONSTRAINT fk_productprice_product FOREIGN KEY (product_id)
      REFERENCES vault.product (product_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE vault.productprice
  OWNER TO postgres;
GRANT ALL ON TABLE vault.productprice TO postgres;

CREATE TABLE vault.productdesc
(
  productdesc_id serial NOT NULL,
  product_id integer NOT NULL DEFAULT 0,
  locale character varying(20) NOT NULL DEFAULT 'en-US'::character varying,
  description text NOT NULL DEFAULT ''::text,
  CONSTRAINT pk_productdesc_id PRIMARY KEY (productdesc_id ),
  CONSTRAINT fk_productdesc_product FOREIGN KEY (product_id)
      REFERENCES vault.product (product_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE vault.productdesc
  OWNER TO postgres;
GRANT ALL ON TABLE vault.productdesc TO postgres;


INSERT INTO "user"
    (username, applicationname, email, comment, password, passwordquestion, passwordanswer, isapproved, lastactivitydate, lastlogindate, lastpasswordchangeddate, creationdate, isonline, islockedout, lastlockedoutdate, failedpasswordattemptcount, failedpasswordattemptwindowstart, failedpasswordanswerattemptcount, failedpasswordanswerattemptwindowstart, subscriptiontype, credits, useruid, whmcsclientid) VALUES
    ('caz', '/', 'c.v.zon@softint.nl', '', 'b0334e7687985ddfc9b9c6b7dde6256a', '', 'd41d8cd98f00b204e9800998ecf8427e', true, '2012-01-22 11:16:04.913897+01', '01:13:40.580167+01', '2012-01-22 11:16:04.913897+01', '2012-01-22 11:16:04.913897+01', true, false, '2012-01-22 11:16:04.913897+01', 0, '2012-01-22 11:16:04.913897+01', 0, '2012-01-22 11:16:04.913897+01', 0, 20, '51581a97d3c94e25bd4d597c64b1f421', 0);


INSERT INTO client (lastname, firstname, insertion, addressline1, addressline2, zipcode, city, state, country, telephone, cellular, companyname, user_id) VALUES ('Zon', 'Caspar', 'van', 'Nicolaas Damesstraat 11', '', '2171 KA', 'S', 'ZH', 'Netherlands',z 10);


INSERT INTO product (product_id, name, description, credits) VALUES (1, 'Package 1', '<h1>Package 1</h1><p>With this package you buy 5 registration credits.</p><p>This means you can register 5 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.', 10);
INSERT INTO product (product_id, name, description, credits) VALUES (2, 'Package 2', '<h1>Package 2</h1><p>With this package you buy 10 registration credits.</p><p>This means you can register 10 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.', 20);
INSERT INTO product (product_id, name, description, credits) VALUES (3, 'Package 3', '<h1>Package 3</h1><p>With this package you buy 15 registration credits.</p><p>This means you can register 15 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.', 30);
INSERT INTO product (product_id, name, description, credits) VALUES (4, 'Package 4', '<h1>Package 4</h1><p>With this package you buy 20 registration credits.</p><p>This means you can register 20 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.', 40);


INSERT INTO productdesc (product_id, locale, description) VALUES (1, 'en-US', '<h1>Package 1</h1><p>With this package you buy 5 registration credits.</p><p>This means you can register 5 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO productdesc (product_id, locale, description) VALUES (1, 'nl-NL', '<h1>Pakket 1</h1><p>Met dit pakket koopt u 5 registratie credits.</p><p>Dit betekent dat u 5 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');
INSERT INTO productdesc (product_id, locale, description) VALUES (2, 'en-US', '<h1>Package 1</h1><p>With this package you buy 10 registration credits.</p><p>This means you can register 10 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO productdesc (product_id, locale, description) VALUES (2, 'nl-NL', '<h1>Pakket 1</h1><p>Met dit pakket koopt u 10 registratie credits.</p><p>Dit betekent dat u 10 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');
INSERT INTO productdesc (product_id, locale, description) VALUES (3, 'en-US', '<h1>Package 1</h1><p>With this package you buy 15 registration credits.</p><p>This means you can register 15 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO productdesc (product_id, locale, description) VALUES (3, 'nl-NL', '<h1>Pakket 1</h1><p>Met dit pakket koopt u 15 registratie credits.</p><p>Dit betekent dat u 15 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');
INSERT INTO productdesc (product_id, locale, description) VALUES (4, 'en-US', '<h1>Package 1</h1><p>With this package you buy 20 registration credits.</p><p>This means you can register 20 tracks for a period of one year and you''ll be able to download a certificate generated specifically for you and your track as proof of registration thus giving you undisputable copyrights.');
INSERT INTO productdesc (product_id, locale, description) VALUES (4, 'nl-NL', '<h1>Pakket 1</h1><p>Met dit pakket koopt u 20 registratie credits.</p><p>Dit betekent dat u 20 tracks kunt registreren voor een periode van 1 jaar en u kunt een certificaat downloaden dat speciaal voor u gegenereerd is en een onweerlegbaar bewijs van uw copyright rechten is.');


INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (10.0000, 'EUR', '', 1);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (20.0000, 'EUR', '', 2);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (30.0000, 'EUR', '', 3);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (40.0000, 'EUR', '', 4);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (10.0000, 'EUR', 'NL', 1);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (20.0000, 'EUR', 'NL', 2);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (30.0000, 'EUR', 'NL', 3);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (40.0000, 'EUR', 'NL', 4);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (15.0000, 'USD', 'US', 1);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (30.0000, 'USD', 'US', 2);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (45.0000, 'USD', 'US', 3);
INSERT INTO productprice (price, iso_currency, iso2_country, product_id) VALUES (60.0000, 'USD', 'US', 4);
