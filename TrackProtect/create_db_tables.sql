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
