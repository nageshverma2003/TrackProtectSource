CREATE SCHEMA vault;

DROP TABLE IF EXISTS vault.document;
DROP TABLE IF EXISTS vault.register;
DROP TABLE IF EXISTS vault.client;
DROP TABLE IF EXISTS vault."user";

CREATE TABLE vault."user"
(
  user_id serial NOT NULL,
  username character varying(50) NOT NULL,
  applicationname character varying(100),
  email character varying(200) NOT NULL,
  comment text,
  password character varying(200),
  passwordquestion character varying(200),
  passwordanswer character varying(200),
  isapproved boolean,
  lastactivitydate timestamp with time zone,
  lastlogindate time with time zone,
  lastpasswordchangeddate timestamp with time zone,
  creationdate timestamp with time zone,
  isonline boolean,
  islockedout boolean,
  lastlockedoutdate timestamp with time zone,
  failedpasswordattemptcount integer,
  failedpasswordattemptwindowstart timestamp with time zone,
  failedpasswordanswerattemptcount integer,
  failedpasswordanswerattemptwindowstart timestamp with time zone,
  subscriptiontype integer DEFAULT 0,
  credits integer DEFAULT 0,
  useruid character varying(100) NOT NULL,
  CONSTRAINT pk_user_id PRIMARY KEY (user_id )
)
WITH (
  OIDS=FALSE
);
ALTER TABLE vault."user"
  OWNER TO postgres;

CREATE TABLE vault.client
(
  client_id serial NOT NULL,
  lastname character varying(100) NOT NULL,
  firstname character varying(50) NOT NULL,
  insertion character varying(20) DEFAULT '',
  addressline1 character varying(100),
  addressline2 character varying(100) DEFAULT '',
  zipcode character varying(10),
  city character varying(100),
  telephone character varying(20),
  cellular character varying(20),
  companyname character varying(100) DEFAULT '',
  user_id integer,
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
  register_id serial NOT NULL,
  certificate character varying(500) NOT NULL,
  registrationdate timestamp with time zone NOT NULL,
  expirationdate timestamp with time zone NOT NULL,
  user_id integer NOT NULL,
  CONSTRAINT pk_register_id PRIMARY KEY (register_id ),
  CONSTRAINT fk_user_register FOREIGN KEY (user_id)
      REFERENCES vault."user" (user_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
);
ALTER TABLE vault.register
  OWNER TO postgres;
  
CREATE TABLE vault.document
(
  document_id serial NOT NULL,
  register_id integer NOT NULL,
  documentname character varying(500) NOT NULL,
  documenthash character varying(100) NOT NULL,
  CONSTRAINT pk_document_id PRIMARY KEY (document_id ),
  CONSTRAINT fk_register_document FOREIGN KEY (register_id)
      REFERENCES vault.register (register_id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
);
ALTER TABLE vault.document
  OWNER TO postgres;
