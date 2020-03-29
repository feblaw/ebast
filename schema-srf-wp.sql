--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.11
-- Dumped by pg_dump version 9.6.11

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: srf-wp; Type: DATABASE; Schema: -; Owner: postgres
--



ALTER DATABASE "wp" OWNER TO postgres;

\connect -reuse-previous=on "dbname='wp'"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: audit; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA audit;


ALTER SCHEMA audit OWNER TO postgres;

--
-- Name: SCHEMA audit; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA audit IS 'Out-of-table audit/history logging tables and trigger functions';


--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


--
-- Name: hstore; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS hstore WITH SCHEMA public;


--
-- Name: EXTENSION hstore; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION hstore IS 'data type for storing sets of (key, value) pairs';


--
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


--
-- Name: audit_table(regclass); Type: FUNCTION; Schema: audit; Owner: postgres
--

CREATE FUNCTION audit.audit_table(target_table regclass) RETURNS void
    LANGUAGE sql
    AS $_$
SELECT audit.audit_table($1, BOOLEAN 't', BOOLEAN 't');
$_$;


ALTER FUNCTION audit.audit_table(target_table regclass) OWNER TO postgres;

--
-- Name: FUNCTION audit_table(target_table regclass); Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON FUNCTION audit.audit_table(target_table regclass) IS '
Add auditing support to the given table. Row-level changes will be logged with full client query text. No cols are ignored.
';


--
-- Name: audit_table(regclass, boolean, boolean); Type: FUNCTION; Schema: audit; Owner: postgres
--

CREATE FUNCTION audit.audit_table(target_table regclass, audit_rows boolean, audit_query_text boolean) RETURNS void
    LANGUAGE sql
    AS $_$
SELECT audit.audit_table($1, $2, $3, ARRAY[]::text[]);
$_$;


ALTER FUNCTION audit.audit_table(target_table regclass, audit_rows boolean, audit_query_text boolean) OWNER TO postgres;

--
-- Name: audit_table(regclass, boolean, boolean, text[]); Type: FUNCTION; Schema: audit; Owner: postgres
--

CREATE FUNCTION audit.audit_table(target_table regclass, audit_rows boolean, audit_query_text boolean, ignored_cols text[]) RETURNS void
    LANGUAGE plpgsql
    AS $$DECLARE
  stm_targets text = 'INSERT OR UPDATE OR DELETE OR TRUNCATE';
  _q_txt text;
  _ignored_cols_snip text = '';
BEGIN
    EXECUTE 'DROP TRIGGER IF EXISTS audit_trigger_row ON ' || target_table::TEXT;
    EXECUTE 'DROP TRIGGER IF EXISTS audit_trigger_stm ON ' || target_table::TEXT;

    IF audit_rows THEN
        IF array_length(ignored_cols,1) > 0 THEN
            _ignored_cols_snip = ', ' || quote_literal(ignored_cols);
        END IF;
        _q_txt = 'CREATE TRIGGER audit_trigger_row AFTER INSERT OR UPDATE OR DELETE ON ' || 
                 target_table::TEXT || 
                 ' FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func(' ||
                 quote_literal(audit_query_text) || _ignored_cols_snip || ');';
        RAISE NOTICE '%',_q_txt;
        EXECUTE _q_txt;
        stm_targets = 'TRUNCATE';
    ELSE
    END IF;

    _q_txt = 'CREATE TRIGGER audit_trigger_stm AFTER ' || stm_targets || ' ON ' ||
             target_table ||
             ' FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('||
             quote_literal(audit_query_text) || ');';
    RAISE NOTICE '%',_q_txt;
    EXECUTE _q_txt;

END;
$$;


ALTER FUNCTION audit.audit_table(target_table regclass, audit_rows boolean, audit_query_text boolean, ignored_cols text[]) OWNER TO postgres;

--
-- Name: FUNCTION audit_table(target_table regclass, audit_rows boolean, audit_query_text boolean, ignored_cols text[]); Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON FUNCTION audit.audit_table(target_table regclass, audit_rows boolean, audit_query_text boolean, ignored_cols text[]) IS '
Add auditing support to a table.

Arguments:
   target_table:     Table name, schema qualified if not on search_path
   audit_rows:       Record each row change, or only audit at a statement level
   audit_query_text: Record the text of the client query that triggered the audit event?
   ignored_cols:     Columns to exclude from update diffs, ignore updates that change only ignored cols.
';


--
-- Name: if_modified_func(); Type: FUNCTION; Schema: audit; Owner: postgres
--

CREATE FUNCTION audit.if_modified_func() RETURNS trigger
    LANGUAGE plpgsql SECURITY DEFINER
    SET search_path TO 'pg_catalog', 'public'
    AS $$
DECLARE
    audit_row audit.logged_actions;
    include_values boolean;
    log_diffs boolean;
    h_old hstore;
    h_new hstore;
    excluded_cols text[] = ARRAY[]::text[];
BEGIN
    IF TG_WHEN <> 'AFTER' THEN
        RAISE EXCEPTION 'audit.if_modified_func() may only run as an AFTER trigger';
    END IF;

    audit_row = ROW(
        nextval('audit.logged_actions_event_id_seq'), -- event_id
        TG_TABLE_SCHEMA::text,                        -- schema_name
        TG_TABLE_NAME::text,                          -- table_name
        TG_RELID,                                     -- relation OID for much quicker searches
        session_user::text,                           -- session_user_name
        current_timestamp,                            -- action_tstamp_tx
        statement_timestamp(),                        -- action_tstamp_stm
        clock_timestamp(),                            -- action_tstamp_clk
        txid_current(),                               -- transaction ID
        current_setting('application_name'),          -- client application
        inet_client_addr(),                           -- client_addr
        inet_client_port(),                           -- client_port
        current_query(),                              -- top-level query or queries (if multistatement) from client
        substring(TG_OP,1,1),                         -- action
        NULL, NULL,                                   -- row_data, changed_fields
        'f'                                           -- statement_only
        );

    IF NOT TG_ARGV[0]::boolean IS DISTINCT FROM 'f'::boolean THEN
        audit_row.client_query = NULL;
    END IF;

    IF TG_ARGV[1] IS NOT NULL THEN
        excluded_cols = TG_ARGV[1]::text[];
    END IF;
    
    IF (TG_OP = 'UPDATE' AND TG_LEVEL = 'ROW') THEN
        audit_row.row_data = hstore(OLD.*) - excluded_cols;
        audit_row.changed_fields =  (hstore(NEW.*) - audit_row.row_data) - excluded_cols;
        IF audit_row.changed_fields = hstore('') THEN
            -- All changed fields are ignored. Skip this update.
            RETURN NULL;
        END IF;
    ELSIF (TG_OP = 'DELETE' AND TG_LEVEL = 'ROW') THEN
        audit_row.row_data = hstore(OLD.*) - excluded_cols;
    ELSIF (TG_OP = 'INSERT' AND TG_LEVEL = 'ROW') THEN
        audit_row.row_data = hstore(NEW.*) - excluded_cols;
    ELSIF (TG_LEVEL = 'STATEMENT' AND TG_OP IN ('INSERT','UPDATE','DELETE','TRUNCATE')) THEN
        audit_row.statement_only = 't';
    ELSE
        RAISE EXCEPTION '[audit.if_modified_func] - Trigger func added as trigger for unhandled case: %, %',TG_OP, TG_LEVEL;
        RETURN NULL;
    END IF;
    INSERT INTO audit.logged_actions VALUES (audit_row.*);
    RETURN NULL;
END;
$$;


ALTER FUNCTION audit.if_modified_func() OWNER TO postgres;

--
-- Name: FUNCTION if_modified_func(); Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON FUNCTION audit.if_modified_func() IS '
Track changes to a table at the statement and/or row level.

Optional parameters to trigger in CREATE TRIGGER call:

param 0: boolean, whether to log the query text. Default ''t''.

param 1: text[], columns to ignore in updates. Default [].

         Updates to ignored cols are omitted from changed_fields.

         Updates with only ignored cols changed are not inserted
         into the audit log.

         Almost all the processing work is still done for updates
         that ignored. If you need to save the load, you need to use
         WHEN clause on the trigger instead.

         No warning or error is issued if ignored_cols contains columns
         that do not exist in the target table. This lets you specify
         a standard set of ignored columns.

There is no parameter to disable logging of values. Add this trigger as
a ''FOR EACH STATEMENT'' rather than ''FOR EACH ROW'' trigger if you do not
want to log row values.

Note that the user name logged is the login role for the session. The audit trigger
cannot obtain the active role because it is reset by the SECURITY DEFINER invocation
of the audit trigger its self.
';


SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: logged_actions; Type: TABLE; Schema: audit; Owner: postgres
--

CREATE TABLE audit.logged_actions (
    event_id bigint NOT NULL,
    schema_name text NOT NULL,
    table_name text NOT NULL,
    relid oid NOT NULL,
    session_user_name text,
    action_tstamp_tx timestamp with time zone NOT NULL,
    action_tstamp_stm timestamp with time zone NOT NULL,
    action_tstamp_clk timestamp with time zone NOT NULL,
    transaction_id bigint,
    application_name text,
    client_addr inet,
    client_port integer,
    client_query text,
    action text NOT NULL,
    row_data public.hstore,
    changed_fields public.hstore,
    statement_only boolean NOT NULL,
    CONSTRAINT logged_actions_action_check CHECK ((action = ANY (ARRAY['I'::text, 'D'::text, 'U'::text, 'T'::text])))
);


ALTER TABLE audit.logged_actions OWNER TO postgres;

--
-- Name: TABLE logged_actions; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON TABLE audit.logged_actions IS 'History of auditable actions on audited tables, from audit.if_modified_func()';


--
-- Name: COLUMN logged_actions.event_id; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.event_id IS 'Unique identifier for each auditable event';


--
-- Name: COLUMN logged_actions.schema_name; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.schema_name IS 'Database schema audited table for this event is in';


--
-- Name: COLUMN logged_actions.table_name; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.table_name IS 'Non-schema-qualified table name of table event occured in';


--
-- Name: COLUMN logged_actions.relid; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.relid IS 'Table OID. Changes with drop/create. Get with ''tablename''::regclass';


--
-- Name: COLUMN logged_actions.session_user_name; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.session_user_name IS 'Login / session user whose statement caused the audited event';


--
-- Name: COLUMN logged_actions.action_tstamp_tx; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.action_tstamp_tx IS 'Transaction start timestamp for tx in which audited event occurred';


--
-- Name: COLUMN logged_actions.action_tstamp_stm; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.action_tstamp_stm IS 'Statement start timestamp for tx in which audited event occurred';


--
-- Name: COLUMN logged_actions.action_tstamp_clk; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.action_tstamp_clk IS 'Wall clock time at which audited event''s trigger call occurred';


--
-- Name: COLUMN logged_actions.transaction_id; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.transaction_id IS 'Identifier of transaction that made the change. May wrap, but unique paired with action_tstamp_tx.';


--
-- Name: COLUMN logged_actions.application_name; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.application_name IS 'Application name set when this audit event occurred. Can be changed in-session by client.';


--
-- Name: COLUMN logged_actions.client_addr; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.client_addr IS 'IP address of client that issued query. Null for unix domain socket.';


--
-- Name: COLUMN logged_actions.client_port; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.client_port IS 'Remote peer IP port address of client that issued query. Undefined for unix socket.';


--
-- Name: COLUMN logged_actions.client_query; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.client_query IS 'Top-level query that caused this auditable event. May be more than one statement.';


--
-- Name: COLUMN logged_actions.action; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.action IS 'Action type; I = insert, D = delete, U = update, T = truncate';


--
-- Name: COLUMN logged_actions.row_data; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.row_data IS 'Record value. Null for statement-level trigger. For INSERT this is the new tuple. For DELETE and UPDATE it is the old tuple.';


--
-- Name: COLUMN logged_actions.changed_fields; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.changed_fields IS 'New values of fields changed by UPDATE. Null except for row-level UPDATE events.';


--
-- Name: COLUMN logged_actions.statement_only; Type: COMMENT; Schema: audit; Owner: postgres
--

COMMENT ON COLUMN audit.logged_actions.statement_only IS '''t'' if audit event is from an FOR EACH STATEMENT trigger, ''f'' for FOR EACH ROW';


--
-- Name: logged_actions_event_id_seq; Type: SEQUENCE; Schema: audit; Owner: postgres
--

CREATE SEQUENCE audit.logged_actions_event_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE audit.logged_actions_event_id_seq OWNER TO postgres;

--
-- Name: logged_actions_event_id_seq; Type: SEQUENCE OWNED BY; Schema: audit; Owner: postgres
--

ALTER SEQUENCE audit.logged_actions_event_id_seq OWNED BY audit.logged_actions.event_id;


--
-- Name: AccountName; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AccountName" (
    "Id" uuid NOT NULL,
    "Com" integer NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Status" boolean NOT NULL,
    "Token" text
);


ALTER TABLE public."AccountName" OWNER TO postgres;

--
-- Name: ActivityCode; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ActivityCode" (
    "Id" uuid NOT NULL,
    "Code" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Description" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "Status" integer NOT NULL,
    "Token" text
);


ALTER TABLE public."ActivityCode" OWNER TO postgres;

--
-- Name: AllowanceForm; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AllowanceForm" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Value" numeric NOT NULL
);


ALTER TABLE public."AllowanceForm" OWNER TO postgres;

--
-- Name: AllowanceList; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AllowanceList" (
    "Id" uuid NOT NULL,
    "AllowanceNote" text,
    "AllowanceStatus" integer NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DataToken" text,
    "GrantedHoliday14" numeric NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OnCallHoliday" numeric NOT NULL,
    "OnCallNormal" numeric NOT NULL,
    "OtherInfo" text,
    "ServicePackId" uuid NOT NULL,
    "ShiftHoliday" numeric NOT NULL,
    "ShiftNormal" numeric NOT NULL
);


ALTER TABLE public."AllowanceList" OWNER TO postgres;

--
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "ClaimType" text,
    "ClaimValue" text,
    "RoleId" text NOT NULL
);


ALTER TABLE public."AspNetRoleClaims" OWNER TO postgres;

--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."AspNetRoleClaims_Id_seq" OWNER TO postgres;

--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."AspNetRoleClaims_Id_seq" OWNED BY public."AspNetRoleClaims"."Id";


--
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetRoles" (
    "Id" text NOT NULL,
    "ConcurrencyStamp" text,
    "Discriminator" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "Description" text,
    "OtherInfo" text
);


ALTER TABLE public."AspNetRoles" OWNER TO postgres;

--
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "ClaimType" text,
    "ClaimValue" text,
    "UserId" text NOT NULL
);


ALTER TABLE public."AspNetUserClaims" OWNER TO postgres;

--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."AspNetUserClaims_Id_seq" OWNER TO postgres;

--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."AspNetUserClaims_Id_seq" OWNED BY public."AspNetUserClaims"."Id";


--
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


ALTER TABLE public."AspNetUserLogins" OWNER TO postgres;

--
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


ALTER TABLE public."AspNetUserRoles" OWNER TO postgres;

--
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text
);


ALTER TABLE public."AspNetUserTokens" OWNER TO postgres;

--
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUsers" (
    "Id" text NOT NULL,
    "AccessFailedCount" integer NOT NULL,
    "ConcurrencyStamp" text,
    "Email" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "LockoutEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "NormalizedEmail" character varying(256),
    "NormalizedUserName" character varying(256),
    "PasswordHash" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "SecurityStamp" text,
    "TwoFactorEnabled" boolean NOT NULL,
    "UserName" character varying(256)
);


ALTER TABLE public."AspNetUsers" OWNER TO postgres;

--
-- Name: AttendaceExceptionList; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AttendaceExceptionList" (
    "Id" uuid NOT NULL,
    "AccountNameId" uuid NOT NULL,
    "ActivityId" uuid NOT NULL,
    "AddDate" timestamp without time zone NOT NULL,
    "AgencyId" integer,
    "ApprovedOneDate" timestamp without time zone NOT NULL,
    "ApprovedTwoDate" timestamp without time zone NOT NULL,
    "ApproverOneId" integer,
    "ApproverTwoId" integer,
    "ContractorId" integer,
    "CostId" uuid NOT NULL,
    "CreateBy" integer NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DateEnd" timestamp without time zone NOT NULL,
    "DateStart" timestamp without time zone NOT NULL,
    "DepartmentId" uuid NOT NULL,
    "DepartmentSubId" uuid NOT NULL,
    "Description" text,
    "Files" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "LocationId" uuid NOT NULL,
    "NetworkId" uuid NOT NULL,
    "OtherInfo" text,
    "ProjectId" uuid NOT NULL,
    "RequestStatus" integer NOT NULL,
    "StatusOne" integer NOT NULL,
    "StatusTwo" integer NOT NULL,
    "SubOpsId" uuid NOT NULL,
    "TimeSheetTypeId" uuid NOT NULL,
    "Token" text,
    "Type" integer NOT NULL,
    "RemainingDays" integer
);


ALTER TABLE public."AttendaceExceptionList" OWNER TO postgres;

--
-- Name: AttendanceRecord; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AttendanceRecord" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "AttendaceExceptionListId" uuid NOT NULL,
    "Hours" integer DEFAULT 0 NOT NULL,
    "AttendanceRecordDate" timestamp without time zone DEFAULT '0001-01-01 00:00:00'::timestamp without time zone NOT NULL
);


ALTER TABLE public."AttendanceRecord" OWNER TO postgres;

--
-- Name: AutoGenerateVariable; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AutoGenerateVariable" (
    "Id" uuid NOT NULL,
    "AutoStatus" boolean NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Cycle" integer NOT NULL,
    "DayNight" character(2),
    "GenerateDate" character(2),
    "GenerateHour" character(2),
    "GenerateMinute" character(2),
    "GenerateSecond" character(2),
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "SetBy" character(4),
    "SetDate" timestamp without time zone NOT NULL,
    function text
);


ALTER TABLE public."AutoGenerateVariable" OWNER TO postgres;

--
-- Name: BackupLog; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BackupLog" (
    "Id" uuid NOT NULL,
    "BackupDate" timestamp without time zone NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Cycle" integer NOT NULL,
    "Database" text,
    "Failed" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "Succeed" text,
    "Tables" text
);


ALTER TABLE public."BackupLog" OWNER TO postgres;

--
-- Name: BusinessNote; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."BusinessNote" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CreatedDate" timestamp without time zone NOT NULL,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Description" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "NoteBy" integer NOT NULL,
    "OtherInfo" text,
    "Token" text
);


ALTER TABLE public."BusinessNote" OWNER TO postgres;

--
-- Name: CandidateInfo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CandidateInfo" (
    "Id" uuid NOT NULL,
    "AccountId" integer,
    "Address" text,
    "AgencyId" integer,
    "AgencyType" integer NOT NULL,
    "ApproveOneDate" timestamp without time zone NOT NULL,
    "ApproveOneNotes" text,
    "ApproveOneStatus" integer NOT NULL,
    "ApproveTwoNotes" text,
    "ApproveTwoStatus" integer NOT NULL,
    "ApproveTwoeDate" timestamp without time zone NOT NULL,
    "Attachments" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DateOfBirth" timestamp without time zone NOT NULL,
    "Description" text,
    "Email" text NOT NULL,
    "Gender" integer NOT NULL,
    "HomeBaseId" uuid,
    "HomePhoneNumber" text,
    "IdNumber" text,
    "IsCandidate" boolean NOT NULL,
    "IsContractor" boolean NOT NULL,
    "IsFormerEricsson" boolean,
    "IsUser" boolean NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Martial" integer,
    "MobilePhoneNumber" text,
    "Name" text NOT NULL,
    "Nationality" text,
    "NickName" text,
    "OtherInfo" text,
    "PlaceOfBirth" text,
    "RequestById" integer,
    "VacancyId" uuid NOT NULL
);


ALTER TABLE public."CandidateInfo" OWNER TO postgres;

--
-- Name: City; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."City" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Status" integer NOT NULL,
    "Token" text
);


ALTER TABLE public."City" OWNER TO postgres;

--
-- Name: Claim; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Claim" (
    "Id" uuid NOT NULL,
    "ActivityCodeId" uuid,
    "AddDate" timestamp without time zone NOT NULL,
    "AgencyId" integer,
    "AllowanceOption" integer NOT NULL,
    "ApprovedDateOne" timestamp without time zone NOT NULL,
    "ApprovedDateTwo" timestamp without time zone NOT NULL,
    "ApproverOne" integer NOT NULL,
    "ApproverOneNotes" text,
    "ApproverTwo" integer NOT NULL,
    "ApproverTwoNotes" text,
    "ClaimApproverOneId" integer,
    "ClaimApproverTwoId" integer,
    "ClaimCategoryId" uuid,
    "ClaimDate" timestamp without time zone NOT NULL,
    "ClaimForId" integer,
    "ClaimStatus" integer NOT NULL,
    "ClaimType" integer NOT NULL,
    "ContractorId" integer,
    "ContractorProfileId" uuid,
    "CostCenterId" uuid,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DayType" integer NOT NULL,
    "DepartureId" uuid,
    "Description" text,
    "DestinationId" uuid,
    "Domallo1" numeric NOT NULL,
    "Domallo2" numeric NOT NULL,
    "Domallo3" numeric NOT NULL,
    "Domallo4" numeric NOT NULL,
    "Domallo5" numeric NOT NULL,
    "Domallo6" numeric NOT NULL,
    "EmployeeLevel" integer NOT NULL,
    "EndDate" timestamp without time zone NOT NULL,
    "EndTime" timestamp without time zone NOT NULL,
    "Files" text,
    "Intallo1" numeric NOT NULL,
    "Intallo2" numeric NOT NULL,
    "Intallo3" numeric NOT NULL,
    "Intallo4" numeric NOT NULL,
    "Intallo5" numeric NOT NULL,
    "Intallo6" numeric NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "NetworkNumberId" uuid,
    "OnCallShift" numeric NOT NULL,
    "OtherInfo" text,
    "ProjectId" uuid,
    "RedeemForId" integer,
    "Schedule" integer NOT NULL,
    "StartDate" timestamp without time zone NOT NULL,
    "StartTime" timestamp without time zone NOT NULL,
    "StatusOne" integer NOT NULL,
    "StatusTwo" integer NOT NULL,
    "TripType" integer NOT NULL,
    "Value" numeric NOT NULL
);


ALTER TABLE public."Claim" OWNER TO postgres;

--
-- Name: ClaimCategory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ClaimCategory" (
    "Id" uuid NOT NULL,
    "Category" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Status" integer NOT NULL
);


ALTER TABLE public."ClaimCategory" OWNER TO postgres;

--
-- Name: CostCenter; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CostCenter" (
    "Id" uuid NOT NULL,
    "Code" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DepartmentId" uuid NOT NULL,
    "Description" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "Status" integer NOT NULL,
    "Token" text
);


ALTER TABLE public."CostCenter" OWNER TO postgres;

--
-- Name: CustomerClaim; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CustomerClaim" (
    "Row" integer NOT NULL,
    "Address" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Fax" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Phone" text
);


ALTER TABLE public."CustomerClaim" OWNER TO postgres;

--
-- Name: CustomerClaim_Row_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."CustomerClaim_Row_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."CustomerClaim_Row_seq" OWNER TO postgres;

--
-- Name: CustomerClaim_Row_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."CustomerClaim_Row_seq" OWNED BY public."CustomerClaim"."Row";


--
-- Name: CustomerData; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CustomerData" (
    "Id" uuid NOT NULL,
    "Add" text,
    "BankName" text,
    "BranchId" uuid NOT NULL,
    "Branding" text,
    "BrokenCommissionTargetName" text,
    "ClaimDocument" text,
    "ClaimProcess" text,
    "CommissionPaymentType" text,
    "CommissionTotal" numeric NOT NULL,
    "CommissionValue" numeric NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "CustomerType" integer NOT NULL,
    "Email" text,
    "Fax" text,
    "IsAntarJemput" boolean NOT NULL,
    "IsCommissioned" boolean NOT NULL,
    "IsOwnRisk" boolean NOT NULL,
    "IsSparepartSupplied" boolean NOT NULL,
    "IsWaitingSpk" boolean NOT NULL,
    "KPTSBankName" text,
    "KPTSRekNumber" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Mobile" text,
    "MontlyCommissionTargetName" text,
    "Name" text,
    "OtherInfo" text,
    "Phone" text,
    "Picture" text,
    "StatusCustomer" text NOT NULL,
    "Times" integer NOT NULL,
    "Token" text,
    "Warranty" text
);


ALTER TABLE public."CustomerData" OWNER TO postgres;

--
-- Name: Departement; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Departement" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Description" text,
    "HeadId" integer NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OperateOrNon" integer NOT NULL,
    "OtherInfo" text,
    "Status" integer NOT NULL,
    "Token" text
);


ALTER TABLE public."Departement" OWNER TO postgres;

--
-- Name: DepartementSub; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."DepartementSub" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DepartmentId" uuid NOT NULL,
    "DsStatus" integer NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "LineManagerid" integer NOT NULL,
    "OtherInfo" text,
    "SubName" text
);


ALTER TABLE public."DepartementSub" OWNER TO postgres;

--
-- Name: DutySchedule; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."DutySchedule" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "IsEnabled" boolean NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OffDutyHour" integer NOT NULL,
    "OffDutyMinute" integer NOT NULL,
    "OnDutyHour" integer NOT NULL,
    "OnDutyMinute" integer NOT NULL,
    "OtherInfo" text
);


ALTER TABLE public."DutySchedule" OWNER TO postgres;

--
-- Name: EmailArchieve; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."EmailArchieve" (
    "Id" uuid NOT NULL,
    "Activity" text,
    "Bcc" text,
    "Cc" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "ExceptionSendingMessage" text,
    "From" text NOT NULL,
    "FromName" text,
    "HtmlMessage" text,
    "IsRead" boolean NOT NULL,
    "IsSent" boolean NOT NULL,
    "LastEditedBy" text,
    "LastTrySentDate" timestamp without time zone,
    "LastUpdateTime" timestamp without time zone,
    "LinkTo" text,
    "OtherInfo" text,
    "PlainMessage" text,
    "ReplyTo" text,
    "ReplyToName" text,
    "SentDate" timestamp without time zone,
    "Status" text,
    "Subject" text NOT NULL,
    "Tos" text NOT NULL,
    "TrySentCount" integer NOT NULL,
    "Attachment" text
);


ALTER TABLE public."EmailArchieve" OWNER TO postgres;

--
-- Name: FingerPrint; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."FingerPrint" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Ip" text,
    "IsEnabled" boolean NOT NULL,
    "Key" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text
);


ALTER TABLE public."FingerPrint" OWNER TO postgres;

--
-- Name: Fortest; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Fortest" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Value" text
);


ALTER TABLE public."Fortest" OWNER TO postgres;

--
-- Name: GenerateLog; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."GenerateLog" (
    "Id" uuid NOT NULL,
    "By" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Date" timestamp without time zone NOT NULL,
    "GeneratedPeriod" timestamp without time zone NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Ledger" text,
    "OtherInfo" text,
    "PeriodBegin" timestamp without time zone NOT NULL,
    "Product" text,
    "Subscriber" text
);


ALTER TABLE public."GenerateLog" OWNER TO postgres;

--
-- Name: Holidays; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Holidays" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DateDay" timestamp without time zone NOT NULL,
    "Description" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "DayType" integer DEFAULT 0 NOT NULL
);


ALTER TABLE public."Holidays" OWNER TO postgres;

--
-- Name: JobStage; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."JobStage" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Description" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "Stage" text,
    "Status" integer NOT NULL,
    "Token" text
);


ALTER TABLE public."JobStage" OWNER TO postgres;

--
-- Name: Language; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Language" (
    "Id" integer NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DefaultLanguage" boolean NOT NULL,
    "Flag" text NOT NULL,
    "LanguageCulture" text NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text NOT NULL,
    "Order" integer NOT NULL,
    "OtherInfo" text,
    "UniqueSeoCode" text NOT NULL
);


ALTER TABLE public."Language" OWNER TO postgres;

--
-- Name: Language_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Language_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."Language_Id_seq" OWNER TO postgres;

--
-- Name: Language_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Language_Id_seq" OWNED BY public."Language"."Id";


--
-- Name: LocaleResource; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."LocaleResource" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LanguageId" integer NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "ResourceName" text NOT NULL,
    "ResourceValue" text NOT NULL
);


ALTER TABLE public."LocaleResource" OWNER TO postgres;

--
-- Name: NetworkNumber; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."NetworkNumber" (
    "Id" uuid NOT NULL,
    "AccountNameId" uuid NOT NULL,
    "Code" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DepartmentId" uuid NOT NULL,
    "Description" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "LineManagerId" integer NOT NULL,
    "OtherInfo" text,
    "ProjectId" uuid NOT NULL,
    "ProjectManagerId" integer NOT NULL,
    "Status" integer NOT NULL,
    "Token" text,
    "IsClosed" boolean
);


ALTER TABLE public."NetworkNumber" OWNER TO postgres;

--
-- Name: PackageType; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PackageType" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Status" integer NOT NULL
);


ALTER TABLE public."PackageType" OWNER TO postgres;

--
-- Name: PanelCategory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."PanelCategory" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text
);


ALTER TABLE public."PanelCategory" OWNER TO postgres;

--
-- Name: Position; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Position" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Remark" text
);


ALTER TABLE public."Position" OWNER TO postgres;

--
-- Name: Projects; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Projects" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Description" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Status" integer NOT NULL,
    "Token" text
);


ALTER TABLE public."Projects" OWNER TO postgres;

--
-- Name: RequestSpareParts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RequestSpareParts" (
    "Id" uuid NOT NULL,
    "Code" text,
    "Conpensation" character(1),
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "IsSupply" boolean NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "PanelCategoryId" uuid NOT NULL,
    "PartAppinsco" double precision NOT NULL,
    "PartQty" double precision NOT NULL,
    "Price" double precision NOT NULL,
    "RepiredId" uuid NOT NULL,
    "SpareAs" character(1),
    "SpareWdl" character(1)
);


ALTER TABLE public."RequestSpareParts" OWNER TO postgres;

--
-- Name: ServicePack; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ServicePack" (
    "Id" uuid NOT NULL,
    "Code" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Hourly" numeric NOT NULL,
    "Laptop" numeric NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Otp20" numeric NOT NULL,
    "Otp30" numeric NOT NULL,
    "Otp40" numeric NOT NULL,
    "Rate" numeric NOT NULL,
    "ServicePackCategoryId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "Token" text,
    "Type" integer NOT NULL,
    "Usin" numeric NOT NULL
);


ALTER TABLE public."ServicePack" OWNER TO postgres;

--
-- Name: ServicePackCategory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ServicePackCategory" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Level" integer NOT NULL,
    "Name" text,
    "OtherInfo" text,
    "Status" integer NOT NULL,
    "Token" text
);


ALTER TABLE public."ServicePackCategory" OWNER TO postgres;

--
-- Name: SrfEscalationRequest; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SrfEscalationRequest" (
    "Id" uuid NOT NULL,
    "ApproveStatusOne" integer NOT NULL,
    "ApproveStatusThree" integer NOT NULL,
    "ApproveStatusTwo" integer NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Files" text,
    "IsCommnunication" boolean NOT NULL,
    "IsWorkstation" boolean NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Note" text,
    "OtLevel" integer NOT NULL,
    "OtherInfo" text,
    "ServicePackId" uuid NOT NULL,
    "SparateValue" numeric NOT NULL,
    "SrfId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "ApproveStatusFour" integer DEFAULT 0 NOT NULL
);


ALTER TABLE public."SrfEscalationRequest" OWNER TO postgres;

--
-- Name: SrfRequest; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SrfRequest" (
    "Id" uuid NOT NULL,
    "AccountId" uuid,
    "ActivityId" uuid,
    "ApproveFiveId" integer,
    "ApproveFourId" integer,
    "ApproveOneId" integer,
    "ApproveSixId" integer,
    "ApproveStatusFive" integer NOT NULL,
    "ApproveStatusFour" integer NOT NULL,
    "ApproveStatusOne" integer NOT NULL,
    "ApproveStatusSix" integer NOT NULL,
    "ApproveStatusThree" integer NOT NULL,
    "ApproveStatusTwo" integer NOT NULL,
    "ApproveThreeId" integer,
    "ApproveTwoId" integer,
    "CandidateId" uuid NOT NULL,
    "CostCenterId" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DepartmentId" uuid NOT NULL,
    "DepartmentSubId" uuid NOT NULL,
    "Description" text,
    "ExtendFrom" uuid,
    "IsActive" boolean NOT NULL,
    "IsExtended" boolean NOT NULL,
    "IsHrms" boolean NOT NULL,
    "IsLocked" boolean NOT NULL,
    "IsManager" boolean NOT NULL,
    "IsOps" boolean NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "LineManagerId" integer NOT NULL,
    "NetworkId" uuid NOT NULL,
    "NotesFirst" text,
    "NotesLast" text,
    "Number" text,
    "OtherInfo" text,
    "ProjectManagerId" integer NOT NULL,
    "RateType" integer NOT NULL,
    "RequestBy" text,
    "ServiceLevel" integer NOT NULL,
    "ServicePackId" uuid NOT NULL,
    "SpectValue" numeric NOT NULL,
    "SrfBegin" timestamp without time zone,
    "SrfEnd" timestamp without time zone,
    "Status" integer NOT NULL,
    "TeriminateNote" text,
    "TerimnatedBy" text,
    "TerminatedDate" timestamp without time zone NOT NULL,
    "Type" integer NOT NULL,
    "isCommunication" boolean NOT NULL,
    "isWorkstation" boolean NOT NULL,
    "AnnualLeave" integer DEFAULT 0 NOT NULL,
    "DateApproveStatusOne" timestamp without time zone,
    "DateApproveStatusTwo" timestamp without time zone,
    "DateApproveStatusThree" timestamp without time zone,
    "DateApproveStatusFour" timestamp without time zone,
    "DateApproveStatusFive" timestamp without time zone,
    "DateApproveStatusSix" timestamp without time zone
);


ALTER TABLE public."SrfRequest" OWNER TO postgres;

--
-- Name: SubOps; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SubOps" (
    "Id" uuid NOT NULL,
    "Code" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Description" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "Status" boolean NOT NULL,
    "Token" text
);


ALTER TABLE public."SubOps" OWNER TO postgres;

--
-- Name: Subdivision; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Subdivision" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "Remark" text
);


ALTER TABLE public."Subdivision" OWNER TO postgres;

--
-- Name: SystemBranch; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SystemBranch" (
    "Id" uuid NOT NULL,
    "Address" text,
    "BranchCode" text,
    "BranchStatus" boolean NOT NULL,
    "CabangCode" text,
    "CabangToken" text,
    "City" text,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "Criteria" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Email" text,
    "Fax" text,
    "Guaranty" text,
    "GuarantyNota" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "NotaCode" text,
    "NotaNonSercvice" text,
    "OtherInfo" text,
    "PhoneOne" text,
    "PhoneTwo" text,
    "PkbRemark" text,
    "PoCode" text,
    "Remark" text,
    "UnitInCode" text,
    "isHeadOffice" boolean NOT NULL
);


ALTER TABLE public."SystemBranch" OWNER TO postgres;

--
-- Name: SystemPropertiesRecord; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."SystemPropertiesRecord" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "PropertyName" text,
    "PropertyValue" text
);


ALTER TABLE public."SystemPropertiesRecord" OWNER TO postgres;

--
-- Name: TacticalResource; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TacticalResource" (
    "Id" uuid NOT NULL,
    "Approved" integer NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DateSrf" timestamp without time zone,
    "DepartmentId" uuid,
    "DepartmentSubId" uuid,
    "Forecast" integer NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "CountSrf" integer DEFAULT 0 NOT NULL
);


ALTER TABLE public."TacticalResource" OWNER TO postgres;

--
-- Name: Ticket; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Ticket" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Detail" text,
    "IsArchive" boolean NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "Status" integer NOT NULL,
    "TicketDate" timestamp without time zone NOT NULL,
    "Title" text,
    "Token" text
);


ALTER TABLE public."Ticket" OWNER TO postgres;

--
-- Name: TicketInfo; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TicketInfo" (
    "Id" uuid NOT NULL,
    "ClaimId" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Description" text,
    "Files" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Note" text,
    "OtherInfo" text,
    "Price" double precision NOT NULL,
    "Status" integer NOT NULL,
    "Token" text
);


ALTER TABLE public."TicketInfo" OWNER TO postgres;

--
-- Name: TicketReply; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TicketReply" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Description" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "ReplyDate" timestamp without time zone NOT NULL,
    "TicketId" uuid NOT NULL,
    "Token" text
);


ALTER TABLE public."TicketReply" OWNER TO postgres;

--
-- Name: TimeSheetPeriod; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TimeSheetPeriod" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DateActual" timestamp without time zone NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "TimeSheetType" uuid NOT NULL,
    "Token" text
);


ALTER TABLE public."TimeSheetPeriod" OWNER TO postgres;

--
-- Name: TimeSheetType; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TimeSheetType" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "Token" text,
    "Type" text
);


ALTER TABLE public."TimeSheetType" OWNER TO postgres;

--
-- Name: UserProfile; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."UserProfile" (
    "Id" integer NOT NULL,
    "Address" text,
    "AhId" text,
    "UserId" text,
    "Birthdate" timestamp without time zone,
    "Birthplace" text,
    "Description" text,
    "Email" text,
    "Gender" integer,
    "HomePhoneNumber" text,
    "IdNumber" character varying(16),
    "IsActive" boolean,
    "IsBlacklist" boolean,
    "IsTerminate" boolean,
    "MobilePhoneNumber" text,
    "Name" character varying(200),
    "Photo" text,
    "Roles" text,
    "UserName" text
);


ALTER TABLE public."UserProfile" OWNER TO postgres;

--
-- Name: UserProfile_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."UserProfile_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."UserProfile_Id_seq" OWNER TO postgres;

--
-- Name: UserProfile_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."UserProfile_Id_seq" OWNED BY public."UserProfile"."Id";


--
-- Name: VacancyList; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."VacancyList" (
    "Id" uuid NOT NULL,
    "AccountNameId" uuid NOT NULL,
    "ApproverFourId" integer,
    "ApproverOneId" integer,
    "ApproverThreeId" integer,
    "ApproverTwoId" integer,
    "CostCodeId" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "DateApprovedFour" timestamp without time zone NOT NULL,
    "DateApprovedOne" timestamp without time zone NOT NULL,
    "DateApprovedThree" timestamp without time zone NOT NULL,
    "DateApprovedTwo" timestamp without time zone NOT NULL,
    "DepartmentId" uuid NOT NULL,
    "DepartmentSubId" uuid NOT NULL,
    "Description" text,
    "Files" text,
    "JobStageId" uuid NOT NULL,
    "JoinDate" timestamp without time zone NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "NetworkId" uuid NOT NULL,
    "NoarmalRate" numeric NOT NULL,
    "OtLevel" integer NOT NULL,
    "OtherInfo" text,
    "PackageTypeId" uuid NOT NULL,
    "RequestById" integer,
    "ServicePackCategoryId" uuid NOT NULL,
    "ServicePackId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "StatusFourth" integer NOT NULL,
    "StatusOne" integer NOT NULL,
    "StatusThree" integer NOT NULL,
    "StatusTwo" integer NOT NULL,
    "VacancyStatus" integer NOT NULL,
    "isHrms" boolean NOT NULL,
    "isLaptop" boolean NOT NULL,
    "isManager" boolean NOT NULL,
    "isUsim" boolean NOT NULL,
    "Name" text,
    "Identifier" text,
    "ProjectId" uuid,
    "VendorId" integer,
    "StartDate" timestamp without time zone,
    "EndDate" timestamp without time zone,
    "Quantity" numeric
);


ALTER TABLE public."VacancyList" OWNER TO postgres;

--
-- Name: WebSetting; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."WebSetting" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text NOT NULL,
    "OtherInfo" text,
    "SystemSetting" boolean NOT NULL,
    "Value" text NOT NULL
);


ALTER TABLE public."WebSetting" OWNER TO postgres;

--
-- Name: WoItem; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."WoItem" (
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Disc" double precision NOT NULL,
    "ItemId" text,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "Name" text,
    "OtherInfo" text,
    "PartCode" text,
    "Price" double precision NOT NULL,
    "Qty" integer NOT NULL,
    "Type" integer NOT NULL,
    "WoNumber" text
);


ALTER TABLE public."WoItem" OWNER TO postgres;

--
-- Name: WorkPackage; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."WorkPackage" (
    "Id" uuid NOT NULL,
    "Identifier" text,
    "Name" text,
    "Quantity" numeric,
    "WPStartDate" timestamp without time zone,
    "WPEndDate" timestamp without time zone,
    "PONumber" text,
    "POItem10Deliv" integer,
    "POItem20Deliv" integer,
    "POItem30Deliv" integer,
    "POItem10BastDate" timestamp without time zone,
    "POItem20BastDate" timestamp without time zone,
    "POItem30BastDate" timestamp without time zone,
    "POItem10BastSignee" text,
    "POItem20BastSignee" text,
    "POItem30BastSignee" text,
    "Status" integer,
    "ProjectId" uuid,
    "ProjectManagerId" integer,
    "TotalProjectManagerId" integer,
    "AccountNameId" uuid,
    "SsowId" uuid,
    "VendorId" integer,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "OtherInfo" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "LastUpdateTime" timestamp without time zone,
    "LastEditedBy" text,
    "JobDesc" text
);


ALTER TABLE public."WorkPackage" OWNER TO postgres;

--
-- Name: WotList; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."WotList" (
    "Id" uuid NOT NULL,
    "AddDate" timestamp without time zone NOT NULL,
    "ApproveOne" boolean NOT NULL,
    "ApproveTwo" boolean NOT NULL,
    "CreatedAt" timestamp without time zone,
    "CreatedBy" text,
    "CustomField1" text,
    "CustomField2" text,
    "CustomField3" text,
    "Description" text,
    "EndTime" timestamp without time zone NOT NULL,
    "LastEditedBy" text,
    "LastUpdateTime" timestamp without time zone,
    "OtherInfo" text,
    "StartTime" timestamp without time zone NOT NULL,
    "Status" integer NOT NULL,
    "StatusOne" integer NOT NULL,
    "StatusTwo" integer NOT NULL,
    "Token" text,
    "WotDate" timestamp without time zone NOT NULL
);


ALTER TABLE public."WotList" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: logged_actions event_id; Type: DEFAULT; Schema: audit; Owner: postgres
--

ALTER TABLE ONLY audit.logged_actions ALTER COLUMN event_id SET DEFAULT nextval('audit.logged_actions_event_id_seq'::regclass);


--
-- Name: AspNetRoleClaims Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoleClaims" ALTER COLUMN "Id" SET DEFAULT nextval('public."AspNetRoleClaims_Id_seq"'::regclass);


--
-- Name: AspNetUserClaims Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserClaims" ALTER COLUMN "Id" SET DEFAULT nextval('public."AspNetUserClaims_Id_seq"'::regclass);


--
-- Name: CustomerClaim Row; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerClaim" ALTER COLUMN "Row" SET DEFAULT nextval('public."CustomerClaim_Row_seq"'::regclass);


--
-- Name: Language Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Language" ALTER COLUMN "Id" SET DEFAULT nextval('public."Language_Id_seq"'::regclass);


--
-- Name: UserProfile Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."UserProfile" ALTER COLUMN "Id" SET DEFAULT nextval('public."UserProfile_Id_seq"'::regclass);


--
-- Name: logged_actions logged_actions_pkey; Type: CONSTRAINT; Schema: audit; Owner: postgres
--

ALTER TABLE ONLY audit.logged_actions
    ADD CONSTRAINT logged_actions_pkey PRIMARY KEY (event_id);


--
-- Name: AccountName PK_AccountName; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AccountName"
    ADD CONSTRAINT "PK_AccountName" PRIMARY KEY ("Id");


--
-- Name: ActivityCode PK_ActivityCode; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ActivityCode"
    ADD CONSTRAINT "PK_ActivityCode" PRIMARY KEY ("Id");


--
-- Name: AllowanceForm PK_AllowanceForm; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AllowanceForm"
    ADD CONSTRAINT "PK_AllowanceForm" PRIMARY KEY ("Id");


--
-- Name: AllowanceList PK_AllowanceList; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AllowanceList"
    ADD CONSTRAINT "PK_AllowanceList" PRIMARY KEY ("Id");


--
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- Name: AttendaceExceptionList PK_AttendaceExceptionList; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "PK_AttendaceExceptionList" PRIMARY KEY ("Id");


--
-- Name: AttendanceRecord PK_AttendanceRecord; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendanceRecord"
    ADD CONSTRAINT "PK_AttendanceRecord" PRIMARY KEY ("Id");


--
-- Name: AutoGenerateVariable PK_AutoGenerateVariable; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AutoGenerateVariable"
    ADD CONSTRAINT "PK_AutoGenerateVariable" PRIMARY KEY ("Id");


--
-- Name: BackupLog PK_BackupLog; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BackupLog"
    ADD CONSTRAINT "PK_BackupLog" PRIMARY KEY ("Id");


--
-- Name: BusinessNote PK_BusinessNote; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BusinessNote"
    ADD CONSTRAINT "PK_BusinessNote" PRIMARY KEY ("Id");


--
-- Name: CandidateInfo PK_CandidateInfo; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CandidateInfo"
    ADD CONSTRAINT "PK_CandidateInfo" PRIMARY KEY ("Id");


--
-- Name: City PK_City; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."City"
    ADD CONSTRAINT "PK_City" PRIMARY KEY ("Id");


--
-- Name: Claim PK_Claim; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "PK_Claim" PRIMARY KEY ("Id");


--
-- Name: ClaimCategory PK_ClaimCategory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ClaimCategory"
    ADD CONSTRAINT "PK_ClaimCategory" PRIMARY KEY ("Id");


--
-- Name: CostCenter PK_CostCenter; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CostCenter"
    ADD CONSTRAINT "PK_CostCenter" PRIMARY KEY ("Id");


--
-- Name: CustomerClaim PK_CustomerClaim; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerClaim"
    ADD CONSTRAINT "PK_CustomerClaim" PRIMARY KEY ("Row");


--
-- Name: CustomerData PK_CustomerData; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerData"
    ADD CONSTRAINT "PK_CustomerData" PRIMARY KEY ("Id");


--
-- Name: Departement PK_Departement; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Departement"
    ADD CONSTRAINT "PK_Departement" PRIMARY KEY ("Id");


--
-- Name: DepartementSub PK_DepartementSub; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DepartementSub"
    ADD CONSTRAINT "PK_DepartementSub" PRIMARY KEY ("Id");


--
-- Name: DutySchedule PK_DutySchedule; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DutySchedule"
    ADD CONSTRAINT "PK_DutySchedule" PRIMARY KEY ("Id");


--
-- Name: EmailArchieve PK_EmailArchieve; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EmailArchieve"
    ADD CONSTRAINT "PK_EmailArchieve" PRIMARY KEY ("Id");


--
-- Name: FingerPrint PK_FingerPrint; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FingerPrint"
    ADD CONSTRAINT "PK_FingerPrint" PRIMARY KEY ("Id");


--
-- Name: Fortest PK_Fortest; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Fortest"
    ADD CONSTRAINT "PK_Fortest" PRIMARY KEY ("Id");


--
-- Name: GenerateLog PK_GenerateLog; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GenerateLog"
    ADD CONSTRAINT "PK_GenerateLog" PRIMARY KEY ("Id");


--
-- Name: Holidays PK_Holidays; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Holidays"
    ADD CONSTRAINT "PK_Holidays" PRIMARY KEY ("Id");


--
-- Name: JobStage PK_JobStage; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."JobStage"
    ADD CONSTRAINT "PK_JobStage" PRIMARY KEY ("Id");


--
-- Name: Language PK_Language; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Language"
    ADD CONSTRAINT "PK_Language" PRIMARY KEY ("Id");


--
-- Name: LocaleResource PK_LocaleResource; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LocaleResource"
    ADD CONSTRAINT "PK_LocaleResource" PRIMARY KEY ("Id");


--
-- Name: NetworkNumber PK_NetworkNumber; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NetworkNumber"
    ADD CONSTRAINT "PK_NetworkNumber" PRIMARY KEY ("Id");


--
-- Name: PackageType PK_PackageType; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PackageType"
    ADD CONSTRAINT "PK_PackageType" PRIMARY KEY ("Id");


--
-- Name: PanelCategory PK_PanelCategory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PanelCategory"
    ADD CONSTRAINT "PK_PanelCategory" PRIMARY KEY ("Id");


--
-- Name: Position PK_Position; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Position"
    ADD CONSTRAINT "PK_Position" PRIMARY KEY ("Id");


--
-- Name: Projects PK_Projects; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Projects"
    ADD CONSTRAINT "PK_Projects" PRIMARY KEY ("Id");


--
-- Name: RequestSpareParts PK_RequestSpareParts; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestSpareParts"
    ADD CONSTRAINT "PK_RequestSpareParts" PRIMARY KEY ("Id");


--
-- Name: ServicePack PK_ServicePack; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ServicePack"
    ADD CONSTRAINT "PK_ServicePack" PRIMARY KEY ("Id");


--
-- Name: ServicePackCategory PK_ServicePackCategory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ServicePackCategory"
    ADD CONSTRAINT "PK_ServicePackCategory" PRIMARY KEY ("Id");


--
-- Name: SrfEscalationRequest PK_SrfEscalationRequest; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfEscalationRequest"
    ADD CONSTRAINT "PK_SrfEscalationRequest" PRIMARY KEY ("Id");


--
-- Name: SrfRequest PK_SrfRequest; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "PK_SrfRequest" PRIMARY KEY ("Id");


--
-- Name: SubOps PK_SubOps; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SubOps"
    ADD CONSTRAINT "PK_SubOps" PRIMARY KEY ("Id");


--
-- Name: Subdivision PK_Subdivision; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Subdivision"
    ADD CONSTRAINT "PK_Subdivision" PRIMARY KEY ("Id");


--
-- Name: SystemBranch PK_SystemBranch; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SystemBranch"
    ADD CONSTRAINT "PK_SystemBranch" PRIMARY KEY ("Id");


--
-- Name: SystemPropertiesRecord PK_SystemPropertiesRecord; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SystemPropertiesRecord"
    ADD CONSTRAINT "PK_SystemPropertiesRecord" PRIMARY KEY ("Id");


--
-- Name: TacticalResource PK_TacticalResource; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TacticalResource"
    ADD CONSTRAINT "PK_TacticalResource" PRIMARY KEY ("Id");


--
-- Name: Ticket PK_Ticket; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Ticket"
    ADD CONSTRAINT "PK_Ticket" PRIMARY KEY ("Id");


--
-- Name: TicketInfo PK_TicketInfo; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TicketInfo"
    ADD CONSTRAINT "PK_TicketInfo" PRIMARY KEY ("Id");


--
-- Name: TicketReply PK_TicketReply; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TicketReply"
    ADD CONSTRAINT "PK_TicketReply" PRIMARY KEY ("Id");


--
-- Name: TimeSheetPeriod PK_TimeSheetPeriod; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TimeSheetPeriod"
    ADD CONSTRAINT "PK_TimeSheetPeriod" PRIMARY KEY ("Id");


--
-- Name: TimeSheetType PK_TimeSheetType; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TimeSheetType"
    ADD CONSTRAINT "PK_TimeSheetType" PRIMARY KEY ("Id");


--
-- Name: UserProfile PK_UserProfile; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "PK_UserProfile" PRIMARY KEY ("Id");


--
-- Name: VacancyList PK_VacancyList; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "PK_VacancyList" PRIMARY KEY ("Id");


--
-- Name: WebSetting PK_WebSetting; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WebSetting"
    ADD CONSTRAINT "PK_WebSetting" PRIMARY KEY ("Id");


--
-- Name: WoItem PK_WoItem; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WoItem"
    ADD CONSTRAINT "PK_WoItem" PRIMARY KEY ("Id");


--
-- Name: WotList PK_WotList; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WotList"
    ADD CONSTRAINT "PK_WotList" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: WorkPackage WorkPackage_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WorkPackage"
    ADD CONSTRAINT "WorkPackage_pkey" PRIMARY KEY ("Id");


--
-- Name: logged_actions_action_idx; Type: INDEX; Schema: audit; Owner: postgres
--

CREATE INDEX logged_actions_action_idx ON audit.logged_actions USING btree (action);


--
-- Name: logged_actions_action_tstamp_tx_stm_idx; Type: INDEX; Schema: audit; Owner: postgres
--

CREATE INDEX logged_actions_action_tstamp_tx_stm_idx ON audit.logged_actions USING btree (action_tstamp_stm);


--
-- Name: logged_actions_relid_idx; Type: INDEX; Schema: audit; Owner: postgres
--

CREATE INDEX logged_actions_relid_idx ON audit.logged_actions USING btree (relid);


--
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "EmailIndex" ON public."AspNetUsers" USING btree ("NormalizedEmail");


--
-- Name: FK_Account; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "FK_Account" ON public."WorkPackage" USING btree ("AccountNameId");


--
-- Name: FK_PROJECT; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "FK_PROJECT" ON public."WorkPackage" USING btree ("ProjectId");


--
-- Name: FK_ProjectName; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "FK_ProjectName" ON public."WorkPackage" USING btree ("ProjectManagerId");


--
-- Name: FK_SSOW; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "FK_SSOW" ON public."WorkPackage" USING btree ("SsowId");


--
-- Name: FK_TPM; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "FK_TPM" ON public."WorkPackage" USING btree ("TotalProjectManagerId");


--
-- Name: FK_VENDOR; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "FK_VENDOR" ON public."WorkPackage" USING btree ("VendorId");


--
-- Name: FK_Vendor; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "FK_Vendor" ON public."VacancyList" USING btree ("VendorId");


--
-- Name: IX_AccountName_Com; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AccountName_Com" ON public."AccountName" USING btree ("Com");


--
-- Name: IX_AccountName_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AccountName_CreatedBy" ON public."AccountName" USING btree ("CreatedBy");


--
-- Name: IX_AccountName_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AccountName_LastEditedBy" ON public."AccountName" USING btree ("LastEditedBy");


--
-- Name: IX_ActivityCode_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ActivityCode_CreatedBy" ON public."ActivityCode" USING btree ("CreatedBy");


--
-- Name: IX_ActivityCode_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ActivityCode_LastEditedBy" ON public."ActivityCode" USING btree ("LastEditedBy");


--
-- Name: IX_AllowanceForm_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AllowanceForm_CreatedBy" ON public."AllowanceForm" USING btree ("CreatedBy");


--
-- Name: IX_AllowanceForm_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AllowanceForm_LastEditedBy" ON public."AllowanceForm" USING btree ("LastEditedBy");


--
-- Name: IX_AllowanceList_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AllowanceList_CreatedBy" ON public."AllowanceList" USING btree ("CreatedBy");


--
-- Name: IX_AllowanceList_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AllowanceList_LastEditedBy" ON public."AllowanceList" USING btree ("LastEditedBy");


--
-- Name: IX_AllowanceList_ServicePackId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AllowanceList_ServicePackId" ON public."AllowanceList" USING btree ("ServicePackId");


--
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON public."AspNetRoleClaims" USING btree ("RoleId");


--
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON public."AspNetUserClaims" USING btree ("UserId");


--
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON public."AspNetUserLogins" USING btree ("UserId");


--
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles" USING btree ("RoleId");


--
-- Name: IX_AttendaceExceptionList_AccountNameId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_AccountNameId" ON public."AttendaceExceptionList" USING btree ("AccountNameId");


--
-- Name: IX_AttendaceExceptionList_ActivityId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_ActivityId" ON public."AttendaceExceptionList" USING btree ("ActivityId");


--
-- Name: IX_AttendaceExceptionList_AgencyId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_AgencyId" ON public."AttendaceExceptionList" USING btree ("AgencyId");


--
-- Name: IX_AttendaceExceptionList_ApproverOneId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_ApproverOneId" ON public."AttendaceExceptionList" USING btree ("ApproverOneId");


--
-- Name: IX_AttendaceExceptionList_ApproverTwoId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_ApproverTwoId" ON public."AttendaceExceptionList" USING btree ("ApproverTwoId");


--
-- Name: IX_AttendaceExceptionList_ContractorId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_ContractorId" ON public."AttendaceExceptionList" USING btree ("ContractorId");


--
-- Name: IX_AttendaceExceptionList_CostId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_CostId" ON public."AttendaceExceptionList" USING btree ("CostId");


--
-- Name: IX_AttendaceExceptionList_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_CreatedBy" ON public."AttendaceExceptionList" USING btree ("CreatedBy");


--
-- Name: IX_AttendaceExceptionList_DepartmentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_DepartmentId" ON public."AttendaceExceptionList" USING btree ("DepartmentId");


--
-- Name: IX_AttendaceExceptionList_DepartmentSubId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_DepartmentSubId" ON public."AttendaceExceptionList" USING btree ("DepartmentSubId");


--
-- Name: IX_AttendaceExceptionList_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_LastEditedBy" ON public."AttendaceExceptionList" USING btree ("LastEditedBy");


--
-- Name: IX_AttendaceExceptionList_LocationId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_LocationId" ON public."AttendaceExceptionList" USING btree ("LocationId");


--
-- Name: IX_AttendaceExceptionList_NetworkId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_NetworkId" ON public."AttendaceExceptionList" USING btree ("NetworkId");


--
-- Name: IX_AttendaceExceptionList_ProjectId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_ProjectId" ON public."AttendaceExceptionList" USING btree ("ProjectId");


--
-- Name: IX_AttendaceExceptionList_SubOpsId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_SubOpsId" ON public."AttendaceExceptionList" USING btree ("SubOpsId");


--
-- Name: IX_AttendaceExceptionList_TimeSheetTypeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendaceExceptionList_TimeSheetTypeId" ON public."AttendaceExceptionList" USING btree ("TimeSheetTypeId");


--
-- Name: IX_AttendanceRecord_AttendaceExceptionListId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendanceRecord_AttendaceExceptionListId" ON public."AttendanceRecord" USING btree ("AttendaceExceptionListId");


--
-- Name: IX_AttendanceRecord_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendanceRecord_CreatedBy" ON public."AttendanceRecord" USING btree ("CreatedBy");


--
-- Name: IX_AttendanceRecord_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AttendanceRecord_LastEditedBy" ON public."AttendanceRecord" USING btree ("LastEditedBy");


--
-- Name: IX_AutoGenerateVariable_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AutoGenerateVariable_CreatedBy" ON public."AutoGenerateVariable" USING btree ("CreatedBy");


--
-- Name: IX_AutoGenerateVariable_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AutoGenerateVariable_LastEditedBy" ON public."AutoGenerateVariable" USING btree ("LastEditedBy");


--
-- Name: IX_BackupLog_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BackupLog_CreatedBy" ON public."BackupLog" USING btree ("CreatedBy");


--
-- Name: IX_BackupLog_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BackupLog_LastEditedBy" ON public."BackupLog" USING btree ("LastEditedBy");


--
-- Name: IX_BusinessNote_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BusinessNote_CreatedBy" ON public."BusinessNote" USING btree ("CreatedBy");


--
-- Name: IX_BusinessNote_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_BusinessNote_LastEditedBy" ON public."BusinessNote" USING btree ("LastEditedBy");


--
-- Name: IX_CandidateInfo_AccountId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CandidateInfo_AccountId" ON public."CandidateInfo" USING btree ("AccountId");


--
-- Name: IX_CandidateInfo_AgencyId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CandidateInfo_AgencyId" ON public."CandidateInfo" USING btree ("AgencyId");


--
-- Name: IX_CandidateInfo_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CandidateInfo_CreatedBy" ON public."CandidateInfo" USING btree ("CreatedBy");


--
-- Name: IX_CandidateInfo_HomeBaseId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CandidateInfo_HomeBaseId" ON public."CandidateInfo" USING btree ("HomeBaseId");


--
-- Name: IX_CandidateInfo_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CandidateInfo_LastEditedBy" ON public."CandidateInfo" USING btree ("LastEditedBy");


--
-- Name: IX_CandidateInfo_RequestById; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CandidateInfo_RequestById" ON public."CandidateInfo" USING btree ("RequestById");


--
-- Name: IX_CandidateInfo_VacancyId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CandidateInfo_VacancyId" ON public."CandidateInfo" USING btree ("VacancyId");


--
-- Name: IX_City_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_City_CreatedBy" ON public."City" USING btree ("CreatedBy");


--
-- Name: IX_City_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_City_LastEditedBy" ON public."City" USING btree ("LastEditedBy");


--
-- Name: IX_ClaimCategory_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ClaimCategory_CreatedBy" ON public."ClaimCategory" USING btree ("CreatedBy");


--
-- Name: IX_ClaimCategory_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ClaimCategory_LastEditedBy" ON public."ClaimCategory" USING btree ("LastEditedBy");


--
-- Name: IX_Claim_ActivityCodeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_ActivityCodeId" ON public."Claim" USING btree ("ActivityCodeId");


--
-- Name: IX_Claim_AgencyId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_AgencyId" ON public."Claim" USING btree ("AgencyId");


--
-- Name: IX_Claim_ClaimApproverOneId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_ClaimApproverOneId" ON public."Claim" USING btree ("ClaimApproverOneId");


--
-- Name: IX_Claim_ClaimApproverTwoId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_ClaimApproverTwoId" ON public."Claim" USING btree ("ClaimApproverTwoId");


--
-- Name: IX_Claim_ClaimCategoryId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_ClaimCategoryId" ON public."Claim" USING btree ("ClaimCategoryId");


--
-- Name: IX_Claim_ClaimForId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_ClaimForId" ON public."Claim" USING btree ("ClaimForId");


--
-- Name: IX_Claim_ContractorId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_ContractorId" ON public."Claim" USING btree ("ContractorId");


--
-- Name: IX_Claim_ContractorProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_ContractorProfileId" ON public."Claim" USING btree ("ContractorProfileId");


--
-- Name: IX_Claim_CostCenterId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_CostCenterId" ON public."Claim" USING btree ("CostCenterId");


--
-- Name: IX_Claim_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_CreatedBy" ON public."Claim" USING btree ("CreatedBy");


--
-- Name: IX_Claim_DepartureId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_DepartureId" ON public."Claim" USING btree ("DepartureId");


--
-- Name: IX_Claim_DestinationId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_DestinationId" ON public."Claim" USING btree ("DestinationId");


--
-- Name: IX_Claim_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_LastEditedBy" ON public."Claim" USING btree ("LastEditedBy");


--
-- Name: IX_Claim_NetworkNumberId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_NetworkNumberId" ON public."Claim" USING btree ("NetworkNumberId");


--
-- Name: IX_Claim_ProjectId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_ProjectId" ON public."Claim" USING btree ("ProjectId");


--
-- Name: IX_Claim_RedeemForId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Claim_RedeemForId" ON public."Claim" USING btree ("RedeemForId");


--
-- Name: IX_CostCenter_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CostCenter_CreatedBy" ON public."CostCenter" USING btree ("CreatedBy");


--
-- Name: IX_CostCenter_DepartmentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CostCenter_DepartmentId" ON public."CostCenter" USING btree ("DepartmentId");


--
-- Name: IX_CostCenter_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CostCenter_LastEditedBy" ON public."CostCenter" USING btree ("LastEditedBy");


--
-- Name: IX_CustomerClaim_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerClaim_CreatedBy" ON public."CustomerClaim" USING btree ("CreatedBy");


--
-- Name: IX_CustomerClaim_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerClaim_LastEditedBy" ON public."CustomerClaim" USING btree ("LastEditedBy");


--
-- Name: IX_CustomerData_BranchId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerData_BranchId" ON public."CustomerData" USING btree ("BranchId");


--
-- Name: IX_CustomerData_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerData_CreatedBy" ON public."CustomerData" USING btree ("CreatedBy");


--
-- Name: IX_CustomerData_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_CustomerData_LastEditedBy" ON public."CustomerData" USING btree ("LastEditedBy");


--
-- Name: IX_DepartementSub_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_DepartementSub_CreatedBy" ON public."DepartementSub" USING btree ("CreatedBy");


--
-- Name: IX_DepartementSub_DepartmentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_DepartementSub_DepartmentId" ON public."DepartementSub" USING btree ("DepartmentId");


--
-- Name: IX_DepartementSub_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_DepartementSub_LastEditedBy" ON public."DepartementSub" USING btree ("LastEditedBy");


--
-- Name: IX_DepartementSub_LineManagerid; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_DepartementSub_LineManagerid" ON public."DepartementSub" USING btree ("LineManagerid");


--
-- Name: IX_Departement_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Departement_CreatedBy" ON public."Departement" USING btree ("CreatedBy");


--
-- Name: IX_Departement_HeadId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Departement_HeadId" ON public."Departement" USING btree ("HeadId");


--
-- Name: IX_Departement_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Departement_LastEditedBy" ON public."Departement" USING btree ("LastEditedBy");


--
-- Name: IX_DutySchedule_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_DutySchedule_CreatedBy" ON public."DutySchedule" USING btree ("CreatedBy");


--
-- Name: IX_DutySchedule_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_DutySchedule_LastEditedBy" ON public."DutySchedule" USING btree ("LastEditedBy");


--
-- Name: IX_EmailArchieve_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_EmailArchieve_CreatedBy" ON public."EmailArchieve" USING btree ("CreatedBy");


--
-- Name: IX_EmailArchieve_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_EmailArchieve_LastEditedBy" ON public."EmailArchieve" USING btree ("LastEditedBy");


--
-- Name: IX_FingerPrint_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_FingerPrint_CreatedBy" ON public."FingerPrint" USING btree ("CreatedBy");


--
-- Name: IX_FingerPrint_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_FingerPrint_LastEditedBy" ON public."FingerPrint" USING btree ("LastEditedBy");


--
-- Name: IX_Fortest_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Fortest_CreatedBy" ON public."Fortest" USING btree ("CreatedBy");


--
-- Name: IX_Fortest_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Fortest_LastEditedBy" ON public."Fortest" USING btree ("LastEditedBy");


--
-- Name: IX_GenerateLog_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_GenerateLog_CreatedBy" ON public."GenerateLog" USING btree ("CreatedBy");


--
-- Name: IX_GenerateLog_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_GenerateLog_LastEditedBy" ON public."GenerateLog" USING btree ("LastEditedBy");


--
-- Name: IX_Holidays_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Holidays_CreatedBy" ON public."Holidays" USING btree ("CreatedBy");


--
-- Name: IX_Holidays_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Holidays_LastEditedBy" ON public."Holidays" USING btree ("LastEditedBy");


--
-- Name: IX_JobStage_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_JobStage_CreatedBy" ON public."JobStage" USING btree ("CreatedBy");


--
-- Name: IX_JobStage_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_JobStage_LastEditedBy" ON public."JobStage" USING btree ("LastEditedBy");


--
-- Name: IX_Language_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Language_CreatedBy" ON public."Language" USING btree ("CreatedBy");


--
-- Name: IX_Language_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Language_LastEditedBy" ON public."Language" USING btree ("LastEditedBy");


--
-- Name: IX_LocaleResource_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_LocaleResource_CreatedBy" ON public."LocaleResource" USING btree ("CreatedBy");


--
-- Name: IX_LocaleResource_LanguageId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_LocaleResource_LanguageId" ON public."LocaleResource" USING btree ("LanguageId");


--
-- Name: IX_LocaleResource_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_LocaleResource_LastEditedBy" ON public."LocaleResource" USING btree ("LastEditedBy");


--
-- Name: IX_NetworkNumber_AccountNameId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NetworkNumber_AccountNameId" ON public."NetworkNumber" USING btree ("AccountNameId");


--
-- Name: IX_NetworkNumber_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NetworkNumber_CreatedBy" ON public."NetworkNumber" USING btree ("CreatedBy");


--
-- Name: IX_NetworkNumber_DepartmentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NetworkNumber_DepartmentId" ON public."NetworkNumber" USING btree ("DepartmentId");


--
-- Name: IX_NetworkNumber_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NetworkNumber_LastEditedBy" ON public."NetworkNumber" USING btree ("LastEditedBy");


--
-- Name: IX_NetworkNumber_LineManagerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NetworkNumber_LineManagerId" ON public."NetworkNumber" USING btree ("LineManagerId");


--
-- Name: IX_NetworkNumber_ProjectId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NetworkNumber_ProjectId" ON public."NetworkNumber" USING btree ("ProjectId");


--
-- Name: IX_NetworkNumber_ProjectManagerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_NetworkNumber_ProjectManagerId" ON public."NetworkNumber" USING btree ("ProjectManagerId");


--
-- Name: IX_PackageType_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PackageType_CreatedBy" ON public."PackageType" USING btree ("CreatedBy");


--
-- Name: IX_PackageType_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PackageType_LastEditedBy" ON public."PackageType" USING btree ("LastEditedBy");


--
-- Name: IX_PanelCategory_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PanelCategory_CreatedBy" ON public."PanelCategory" USING btree ("CreatedBy");


--
-- Name: IX_PanelCategory_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_PanelCategory_LastEditedBy" ON public."PanelCategory" USING btree ("LastEditedBy");


--
-- Name: IX_Position_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Position_CreatedBy" ON public."Position" USING btree ("CreatedBy");


--
-- Name: IX_Position_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Position_LastEditedBy" ON public."Position" USING btree ("LastEditedBy");


--
-- Name: IX_Projects_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Projects_CreatedBy" ON public."Projects" USING btree ("CreatedBy");


--
-- Name: IX_Projects_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Projects_LastEditedBy" ON public."Projects" USING btree ("LastEditedBy");


--
-- Name: IX_RequestSpareParts_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_RequestSpareParts_CreatedBy" ON public."RequestSpareParts" USING btree ("CreatedBy");


--
-- Name: IX_RequestSpareParts_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_RequestSpareParts_LastEditedBy" ON public."RequestSpareParts" USING btree ("LastEditedBy");


--
-- Name: IX_RequestSpareParts_PanelCategoryId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_RequestSpareParts_PanelCategoryId" ON public."RequestSpareParts" USING btree ("PanelCategoryId");


--
-- Name: IX_ServicePackCategory_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ServicePackCategory_CreatedBy" ON public."ServicePackCategory" USING btree ("CreatedBy");


--
-- Name: IX_ServicePackCategory_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ServicePackCategory_LastEditedBy" ON public."ServicePackCategory" USING btree ("LastEditedBy");


--
-- Name: IX_ServicePack_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ServicePack_CreatedBy" ON public."ServicePack" USING btree ("CreatedBy");


--
-- Name: IX_ServicePack_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ServicePack_LastEditedBy" ON public."ServicePack" USING btree ("LastEditedBy");


--
-- Name: IX_ServicePack_ServicePackCategoryId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_ServicePack_ServicePackCategoryId" ON public."ServicePack" USING btree ("ServicePackCategoryId");


--
-- Name: IX_SrfEscalationRequest_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfEscalationRequest_CreatedBy" ON public."SrfEscalationRequest" USING btree ("CreatedBy");


--
-- Name: IX_SrfEscalationRequest_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfEscalationRequest_LastEditedBy" ON public."SrfEscalationRequest" USING btree ("LastEditedBy");


--
-- Name: IX_SrfEscalationRequest_ServicePackId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfEscalationRequest_ServicePackId" ON public."SrfEscalationRequest" USING btree ("ServicePackId");


--
-- Name: IX_SrfEscalationRequest_SrfId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_SrfEscalationRequest_SrfId" ON public."SrfEscalationRequest" USING btree ("SrfId");


--
-- Name: IX_SrfRequest_AccountId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_AccountId" ON public."SrfRequest" USING btree ("AccountId");


--
-- Name: IX_SrfRequest_ActivityId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ActivityId" ON public."SrfRequest" USING btree ("ActivityId");


--
-- Name: IX_SrfRequest_ApproveFiveId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ApproveFiveId" ON public."SrfRequest" USING btree ("ApproveFiveId");


--
-- Name: IX_SrfRequest_ApproveFourId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ApproveFourId" ON public."SrfRequest" USING btree ("ApproveFourId");


--
-- Name: IX_SrfRequest_ApproveOneId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ApproveOneId" ON public."SrfRequest" USING btree ("ApproveOneId");


--
-- Name: IX_SrfRequest_ApproveSixId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ApproveSixId" ON public."SrfRequest" USING btree ("ApproveSixId");


--
-- Name: IX_SrfRequest_ApproveThreeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ApproveThreeId" ON public."SrfRequest" USING btree ("ApproveThreeId");


--
-- Name: IX_SrfRequest_ApproveTwoId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ApproveTwoId" ON public."SrfRequest" USING btree ("ApproveTwoId");


--
-- Name: IX_SrfRequest_CandidateId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_CandidateId" ON public."SrfRequest" USING btree ("CandidateId");


--
-- Name: IX_SrfRequest_CostCenterId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_CostCenterId" ON public."SrfRequest" USING btree ("CostCenterId");


--
-- Name: IX_SrfRequest_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_CreatedBy" ON public."SrfRequest" USING btree ("CreatedBy");


--
-- Name: IX_SrfRequest_DepartmentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_DepartmentId" ON public."SrfRequest" USING btree ("DepartmentId");


--
-- Name: IX_SrfRequest_DepartmentSubId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_DepartmentSubId" ON public."SrfRequest" USING btree ("DepartmentSubId");


--
-- Name: IX_SrfRequest_ExtendFrom; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ExtendFrom" ON public."SrfRequest" USING btree ("ExtendFrom");


--
-- Name: IX_SrfRequest_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_LastEditedBy" ON public."SrfRequest" USING btree ("LastEditedBy");


--
-- Name: IX_SrfRequest_LineManagerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_LineManagerId" ON public."SrfRequest" USING btree ("LineManagerId");


--
-- Name: IX_SrfRequest_NetworkId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_NetworkId" ON public."SrfRequest" USING btree ("NetworkId");


--
-- Name: IX_SrfRequest_ProjectManagerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ProjectManagerId" ON public."SrfRequest" USING btree ("ProjectManagerId");


--
-- Name: IX_SrfRequest_ServicePackId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SrfRequest_ServicePackId" ON public."SrfRequest" USING btree ("ServicePackId");


--
-- Name: IX_SubOps_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SubOps_CreatedBy" ON public."SubOps" USING btree ("CreatedBy");


--
-- Name: IX_SubOps_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SubOps_LastEditedBy" ON public."SubOps" USING btree ("LastEditedBy");


--
-- Name: IX_Subdivision_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Subdivision_CreatedBy" ON public."Subdivision" USING btree ("CreatedBy");


--
-- Name: IX_Subdivision_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Subdivision_LastEditedBy" ON public."Subdivision" USING btree ("LastEditedBy");


--
-- Name: IX_SystemBranch_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SystemBranch_CreatedBy" ON public."SystemBranch" USING btree ("CreatedBy");


--
-- Name: IX_SystemBranch_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SystemBranch_LastEditedBy" ON public."SystemBranch" USING btree ("LastEditedBy");


--
-- Name: IX_SystemPropertiesRecord_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SystemPropertiesRecord_CreatedBy" ON public."SystemPropertiesRecord" USING btree ("CreatedBy");


--
-- Name: IX_SystemPropertiesRecord_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_SystemPropertiesRecord_LastEditedBy" ON public."SystemPropertiesRecord" USING btree ("LastEditedBy");


--
-- Name: IX_TacticalResource_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TacticalResource_CreatedBy" ON public."TacticalResource" USING btree ("CreatedBy");


--
-- Name: IX_TacticalResource_DepartmentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TacticalResource_DepartmentId" ON public."TacticalResource" USING btree ("DepartmentId");


--
-- Name: IX_TacticalResource_DepartmentSubId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TacticalResource_DepartmentSubId" ON public."TacticalResource" USING btree ("DepartmentSubId");


--
-- Name: IX_TacticalResource_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TacticalResource_LastEditedBy" ON public."TacticalResource" USING btree ("LastEditedBy");


--
-- Name: IX_TicketInfo_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TicketInfo_CreatedBy" ON public."TicketInfo" USING btree ("CreatedBy");


--
-- Name: IX_TicketInfo_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TicketInfo_LastEditedBy" ON public."TicketInfo" USING btree ("LastEditedBy");


--
-- Name: IX_TicketReply_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TicketReply_CreatedBy" ON public."TicketReply" USING btree ("CreatedBy");


--
-- Name: IX_TicketReply_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TicketReply_LastEditedBy" ON public."TicketReply" USING btree ("LastEditedBy");


--
-- Name: IX_TicketReply_TicketId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TicketReply_TicketId" ON public."TicketReply" USING btree ("TicketId");


--
-- Name: IX_Ticket_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Ticket_CreatedBy" ON public."Ticket" USING btree ("CreatedBy");


--
-- Name: IX_Ticket_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Ticket_LastEditedBy" ON public."Ticket" USING btree ("LastEditedBy");


--
-- Name: IX_TimeSheetPeriod_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TimeSheetPeriod_CreatedBy" ON public."TimeSheetPeriod" USING btree ("CreatedBy");


--
-- Name: IX_TimeSheetPeriod_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TimeSheetPeriod_LastEditedBy" ON public."TimeSheetPeriod" USING btree ("LastEditedBy");


--
-- Name: IX_TimeSheetType_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TimeSheetType_CreatedBy" ON public."TimeSheetType" USING btree ("CreatedBy");


--
-- Name: IX_TimeSheetType_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_TimeSheetType_LastEditedBy" ON public."TimeSheetType" USING btree ("LastEditedBy");


--
-- Name: IX_UserProfile_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_UserProfile_UserId" ON public."UserProfile" USING btree ("UserId");


--
-- Name: IX_VacancyList_AccountNameId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_AccountNameId" ON public."VacancyList" USING btree ("AccountNameId");


--
-- Name: IX_VacancyList_ApproverFourId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_ApproverFourId" ON public."VacancyList" USING btree ("ApproverFourId");


--
-- Name: IX_VacancyList_ApproverOneId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_ApproverOneId" ON public."VacancyList" USING btree ("ApproverOneId");


--
-- Name: IX_VacancyList_ApproverThreeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_ApproverThreeId" ON public."VacancyList" USING btree ("ApproverThreeId");


--
-- Name: IX_VacancyList_ApproverTwoId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_ApproverTwoId" ON public."VacancyList" USING btree ("ApproverTwoId");


--
-- Name: IX_VacancyList_CostCodeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_CostCodeId" ON public."VacancyList" USING btree ("CostCodeId");


--
-- Name: IX_VacancyList_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_CreatedBy" ON public."VacancyList" USING btree ("CreatedBy");


--
-- Name: IX_VacancyList_DepartmentId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_DepartmentId" ON public."VacancyList" USING btree ("DepartmentId");


--
-- Name: IX_VacancyList_DepartmentSubId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_DepartmentSubId" ON public."VacancyList" USING btree ("DepartmentSubId");


--
-- Name: IX_VacancyList_JobStageId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_JobStageId" ON public."VacancyList" USING btree ("JobStageId");


--
-- Name: IX_VacancyList_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_LastEditedBy" ON public."VacancyList" USING btree ("LastEditedBy");


--
-- Name: IX_VacancyList_NetworkId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_NetworkId" ON public."VacancyList" USING btree ("NetworkId");


--
-- Name: IX_VacancyList_PackageTypeId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_PackageTypeId" ON public."VacancyList" USING btree ("PackageTypeId");


--
-- Name: IX_VacancyList_RequestById; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_RequestById" ON public."VacancyList" USING btree ("RequestById");


--
-- Name: IX_VacancyList_ServicePackCategoryId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_ServicePackCategoryId" ON public."VacancyList" USING btree ("ServicePackCategoryId");


--
-- Name: IX_VacancyList_ServicePackId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_VacancyList_ServicePackId" ON public."VacancyList" USING btree ("ServicePackId");


--
-- Name: IX_WebSetting_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WebSetting_CreatedBy" ON public."WebSetting" USING btree ("CreatedBy");


--
-- Name: IX_WebSetting_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WebSetting_LastEditedBy" ON public."WebSetting" USING btree ("LastEditedBy");


--
-- Name: IX_WoItem_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WoItem_CreatedBy" ON public."WoItem" USING btree ("CreatedBy");


--
-- Name: IX_WoItem_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WoItem_LastEditedBy" ON public."WoItem" USING btree ("LastEditedBy");


--
-- Name: IX_WotList_CreatedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WotList_CreatedBy" ON public."WotList" USING btree ("CreatedBy");


--
-- Name: IX_WotList_LastEditedBy; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_WotList_LastEditedBy" ON public."WotList" USING btree ("LastEditedBy");


--
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "RoleNameIndex" ON public."AspNetRoles" USING btree ("NormalizedName");


--
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "UserNameIndex" ON public."AspNetUsers" USING btree ("NormalizedUserName");


--
-- Name: City audit_trigger_row; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_row AFTER INSERT OR DELETE OR UPDATE ON public."City" FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: AspNetUsers audit_trigger_row; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_row AFTER INSERT OR DELETE OR UPDATE ON public."AspNetUsers" FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: VacancyList audit_trigger_row; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_row AFTER INSERT OR DELETE OR UPDATE ON public."VacancyList" FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: UserProfile audit_trigger_row; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_row AFTER INSERT OR DELETE OR UPDATE ON public."UserProfile" FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: CandidateInfo audit_trigger_row; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_row AFTER INSERT OR DELETE OR UPDATE ON public."CandidateInfo" FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: SrfRequest audit_trigger_row; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_row AFTER INSERT OR DELETE OR UPDATE ON public."SrfRequest" FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: SrfEscalationRequest audit_trigger_row; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_row AFTER INSERT OR DELETE OR UPDATE ON public."SrfEscalationRequest" FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: ServicePack audit_trigger_row; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_row AFTER INSERT OR DELETE OR UPDATE ON public."ServicePack" FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: ServicePackCategory audit_trigger_row; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_row AFTER INSERT OR DELETE OR UPDATE ON public."ServicePackCategory" FOR EACH ROW EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: City audit_trigger_stm; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_stm AFTER TRUNCATE ON public."City" FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: AspNetUsers audit_trigger_stm; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_stm AFTER TRUNCATE ON public."AspNetUsers" FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: VacancyList audit_trigger_stm; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_stm AFTER TRUNCATE ON public."VacancyList" FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: UserProfile audit_trigger_stm; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_stm AFTER TRUNCATE ON public."UserProfile" FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: CandidateInfo audit_trigger_stm; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_stm AFTER TRUNCATE ON public."CandidateInfo" FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: SrfRequest audit_trigger_stm; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_stm AFTER TRUNCATE ON public."SrfRequest" FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: SrfEscalationRequest audit_trigger_stm; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_stm AFTER TRUNCATE ON public."SrfEscalationRequest" FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: ServicePack audit_trigger_stm; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_stm AFTER TRUNCATE ON public."ServicePack" FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: ServicePackCategory audit_trigger_stm; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER audit_trigger_stm AFTER TRUNCATE ON public."ServicePackCategory" FOR EACH STATEMENT EXECUTE PROCEDURE audit.if_modified_func('true');


--
-- Name: AccountName FK_AccountName_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AccountName"
    ADD CONSTRAINT "FK_AccountName_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AccountName FK_AccountName_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AccountName"
    ADD CONSTRAINT "FK_AccountName_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AccountName FK_AccountName_UserProfile_Com; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AccountName"
    ADD CONSTRAINT "FK_AccountName_UserProfile_Com" FOREIGN KEY ("Com") REFERENCES public."UserProfile"("Id") ON DELETE CASCADE;


--
-- Name: ActivityCode FK_ActivityCode_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ActivityCode"
    ADD CONSTRAINT "FK_ActivityCode_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: ActivityCode FK_ActivityCode_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ActivityCode"
    ADD CONSTRAINT "FK_ActivityCode_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AllowanceForm FK_AllowanceForm_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AllowanceForm"
    ADD CONSTRAINT "FK_AllowanceForm_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AllowanceForm FK_AllowanceForm_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AllowanceForm"
    ADD CONSTRAINT "FK_AllowanceForm_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AllowanceList FK_AllowanceList_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AllowanceList"
    ADD CONSTRAINT "FK_AllowanceList_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AllowanceList FK_AllowanceList_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AllowanceList"
    ADD CONSTRAINT "FK_AllowanceList_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AllowanceList FK_AllowanceList_ServicePack_ServicePackId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AllowanceList"
    ADD CONSTRAINT "FK_AllowanceList_ServicePack_ServicePackId" FOREIGN KEY ("ServicePackId") REFERENCES public."ServicePack"("Id") ON DELETE CASCADE;


--
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_AccountName_AccountNameId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_AccountName_AccountNameId" FOREIGN KEY ("AccountNameId") REFERENCES public."AccountName"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_ActivityCode_ActivityId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_ActivityCode_ActivityId" FOREIGN KEY ("ActivityId") REFERENCES public."ActivityCode"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_City_LocationId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_City_LocationId" FOREIGN KEY ("LocationId") REFERENCES public."City"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_CostCenter_CostId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_CostCenter_CostId" FOREIGN KEY ("CostId") REFERENCES public."CostCenter"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_DepartementSub_DepartmentSubId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_DepartementSub_DepartmentSubId" FOREIGN KEY ("DepartmentSubId") REFERENCES public."DepartementSub"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_Departement_DepartmentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_Departement_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES public."Departement"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_NetworkNumber_NetworkId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_NetworkNumber_NetworkId" FOREIGN KEY ("NetworkId") REFERENCES public."NetworkNumber"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_Projects_ProjectId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES public."Projects"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_SubOps_SubOpsId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_SubOps_SubOpsId" FOREIGN KEY ("SubOpsId") REFERENCES public."SubOps"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_TimeSheetType_TimeSheetTypeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_TimeSheetType_TimeSheetTypeId" FOREIGN KEY ("TimeSheetTypeId") REFERENCES public."TimeSheetType"("Id") ON DELETE CASCADE;


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_UserProfile_AgencyId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_UserProfile_AgencyId" FOREIGN KEY ("AgencyId") REFERENCES public."UserProfile"("Id");


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_UserProfile_ApproverOneId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_UserProfile_ApproverOneId" FOREIGN KEY ("ApproverOneId") REFERENCES public."UserProfile"("Id");


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_UserProfile_ApproverTwoId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_UserProfile_ApproverTwoId" FOREIGN KEY ("ApproverTwoId") REFERENCES public."UserProfile"("Id");


--
-- Name: AttendaceExceptionList FK_AttendaceExceptionList_UserProfile_ContractorId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendaceExceptionList"
    ADD CONSTRAINT "FK_AttendaceExceptionList_UserProfile_ContractorId" FOREIGN KEY ("ContractorId") REFERENCES public."UserProfile"("Id");


--
-- Name: AttendanceRecord FK_AttendanceRecord_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendanceRecord"
    ADD CONSTRAINT "FK_AttendanceRecord_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AttendanceRecord FK_AttendanceRecord_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendanceRecord"
    ADD CONSTRAINT "FK_AttendanceRecord_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AttendanceRecord FK_AttendanceRecord_AttendaceExceptionList_AttendaceExceptionLi; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AttendanceRecord"
    ADD CONSTRAINT "FK_AttendanceRecord_AttendaceExceptionList_AttendaceExceptionLi" FOREIGN KEY ("AttendaceExceptionListId") REFERENCES public."AttendaceExceptionList"("Id") ON DELETE CASCADE;


--
-- Name: AutoGenerateVariable FK_AutoGenerateVariable_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AutoGenerateVariable"
    ADD CONSTRAINT "FK_AutoGenerateVariable_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: AutoGenerateVariable FK_AutoGenerateVariable_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AutoGenerateVariable"
    ADD CONSTRAINT "FK_AutoGenerateVariable_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: BackupLog FK_BackupLog_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BackupLog"
    ADD CONSTRAINT "FK_BackupLog_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: BackupLog FK_BackupLog_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BackupLog"
    ADD CONSTRAINT "FK_BackupLog_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: BusinessNote FK_BusinessNote_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BusinessNote"
    ADD CONSTRAINT "FK_BusinessNote_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: BusinessNote FK_BusinessNote_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."BusinessNote"
    ADD CONSTRAINT "FK_BusinessNote_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: CandidateInfo FK_CandidateInfo_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CandidateInfo"
    ADD CONSTRAINT "FK_CandidateInfo_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: CandidateInfo FK_CandidateInfo_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CandidateInfo"
    ADD CONSTRAINT "FK_CandidateInfo_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: CandidateInfo FK_CandidateInfo_City_HomeBaseId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CandidateInfo"
    ADD CONSTRAINT "FK_CandidateInfo_City_HomeBaseId" FOREIGN KEY ("HomeBaseId") REFERENCES public."City"("Id");


--
-- Name: CandidateInfo FK_CandidateInfo_UserProfile_AccountId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CandidateInfo"
    ADD CONSTRAINT "FK_CandidateInfo_UserProfile_AccountId" FOREIGN KEY ("AccountId") REFERENCES public."UserProfile"("Id");


--
-- Name: CandidateInfo FK_CandidateInfo_UserProfile_AgencyId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CandidateInfo"
    ADD CONSTRAINT "FK_CandidateInfo_UserProfile_AgencyId" FOREIGN KEY ("AgencyId") REFERENCES public."UserProfile"("Id");


--
-- Name: CandidateInfo FK_CandidateInfo_UserProfile_RequestById; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CandidateInfo"
    ADD CONSTRAINT "FK_CandidateInfo_UserProfile_RequestById" FOREIGN KEY ("RequestById") REFERENCES public."UserProfile"("Id");


--
-- Name: CandidateInfo FK_CandidateInfo_VacancyList_VacancyId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CandidateInfo"
    ADD CONSTRAINT "FK_CandidateInfo_VacancyList_VacancyId" FOREIGN KEY ("VacancyId") REFERENCES public."VacancyList"("Id") ON DELETE CASCADE;


--
-- Name: City FK_City_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."City"
    ADD CONSTRAINT "FK_City_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: City FK_City_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."City"
    ADD CONSTRAINT "FK_City_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: ClaimCategory FK_ClaimCategory_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ClaimCategory"
    ADD CONSTRAINT "FK_ClaimCategory_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: ClaimCategory FK_ClaimCategory_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ClaimCategory"
    ADD CONSTRAINT "FK_ClaimCategory_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Claim FK_Claim_ActivityCode_ActivityCodeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_ActivityCode_ActivityCodeId" FOREIGN KEY ("ActivityCodeId") REFERENCES public."ActivityCode"("Id");


--
-- Name: Claim FK_Claim_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Claim FK_Claim_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Claim FK_Claim_CandidateInfo_ContractorProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_CandidateInfo_ContractorProfileId" FOREIGN KEY ("ContractorProfileId") REFERENCES public."CandidateInfo"("Id");


--
-- Name: Claim FK_Claim_City_DepartureId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_City_DepartureId" FOREIGN KEY ("DepartureId") REFERENCES public."City"("Id");


--
-- Name: Claim FK_Claim_City_DestinationId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_City_DestinationId" FOREIGN KEY ("DestinationId") REFERENCES public."City"("Id");


--
-- Name: Claim FK_Claim_ClaimCategory_ClaimCategoryId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_ClaimCategory_ClaimCategoryId" FOREIGN KEY ("ClaimCategoryId") REFERENCES public."ClaimCategory"("Id");


--
-- Name: Claim FK_Claim_CostCenter_CostCenterId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_CostCenter_CostCenterId" FOREIGN KEY ("CostCenterId") REFERENCES public."CostCenter"("Id");


--
-- Name: Claim FK_Claim_NetworkNumber_NetworkNumberId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_NetworkNumber_NetworkNumberId" FOREIGN KEY ("NetworkNumberId") REFERENCES public."NetworkNumber"("Id");


--
-- Name: Claim FK_Claim_Projects_ProjectId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES public."Projects"("Id");


--
-- Name: Claim FK_Claim_UserProfile_AgencyId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_UserProfile_AgencyId" FOREIGN KEY ("AgencyId") REFERENCES public."UserProfile"("Id");


--
-- Name: Claim FK_Claim_UserProfile_ClaimApproverOneId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_UserProfile_ClaimApproverOneId" FOREIGN KEY ("ClaimApproverOneId") REFERENCES public."UserProfile"("Id");


--
-- Name: Claim FK_Claim_UserProfile_ClaimApproverTwoId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_UserProfile_ClaimApproverTwoId" FOREIGN KEY ("ClaimApproverTwoId") REFERENCES public."UserProfile"("Id");


--
-- Name: Claim FK_Claim_UserProfile_ClaimForId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_UserProfile_ClaimForId" FOREIGN KEY ("ClaimForId") REFERENCES public."UserProfile"("Id");


--
-- Name: Claim FK_Claim_UserProfile_ContractorId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_UserProfile_ContractorId" FOREIGN KEY ("ContractorId") REFERENCES public."UserProfile"("Id");


--
-- Name: Claim FK_Claim_UserProfile_RedeemForId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Claim"
    ADD CONSTRAINT "FK_Claim_UserProfile_RedeemForId" FOREIGN KEY ("RedeemForId") REFERENCES public."UserProfile"("Id");


--
-- Name: CostCenter FK_CostCenter_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CostCenter"
    ADD CONSTRAINT "FK_CostCenter_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: CostCenter FK_CostCenter_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CostCenter"
    ADD CONSTRAINT "FK_CostCenter_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: CostCenter FK_CostCenter_Departement_DepartmentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CostCenter"
    ADD CONSTRAINT "FK_CostCenter_Departement_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES public."Departement"("Id") ON DELETE CASCADE;


--
-- Name: CustomerClaim FK_CustomerClaim_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerClaim"
    ADD CONSTRAINT "FK_CustomerClaim_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: CustomerClaim FK_CustomerClaim_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerClaim"
    ADD CONSTRAINT "FK_CustomerClaim_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: CustomerData FK_CustomerData_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerData"
    ADD CONSTRAINT "FK_CustomerData_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: CustomerData FK_CustomerData_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerData"
    ADD CONSTRAINT "FK_CustomerData_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: CustomerData FK_CustomerData_SystemBranch_BranchId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CustomerData"
    ADD CONSTRAINT "FK_CustomerData_SystemBranch_BranchId" FOREIGN KEY ("BranchId") REFERENCES public."SystemBranch"("Id") ON DELETE CASCADE;


--
-- Name: DepartementSub FK_DepartementSub_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DepartementSub"
    ADD CONSTRAINT "FK_DepartementSub_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: DepartementSub FK_DepartementSub_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DepartementSub"
    ADD CONSTRAINT "FK_DepartementSub_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: DepartementSub FK_DepartementSub_Departement_DepartmentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DepartementSub"
    ADD CONSTRAINT "FK_DepartementSub_Departement_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES public."Departement"("Id") ON DELETE CASCADE;


--
-- Name: DepartementSub FK_DepartementSub_UserProfile_LineManagerid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DepartementSub"
    ADD CONSTRAINT "FK_DepartementSub_UserProfile_LineManagerid" FOREIGN KEY ("LineManagerid") REFERENCES public."UserProfile"("Id") ON DELETE CASCADE;


--
-- Name: Departement FK_Departement_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Departement"
    ADD CONSTRAINT "FK_Departement_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Departement FK_Departement_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Departement"
    ADD CONSTRAINT "FK_Departement_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Departement FK_Departement_UserProfile_HeadId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Departement"
    ADD CONSTRAINT "FK_Departement_UserProfile_HeadId" FOREIGN KEY ("HeadId") REFERENCES public."UserProfile"("Id") ON DELETE CASCADE;


--
-- Name: DutySchedule FK_DutySchedule_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DutySchedule"
    ADD CONSTRAINT "FK_DutySchedule_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: DutySchedule FK_DutySchedule_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."DutySchedule"
    ADD CONSTRAINT "FK_DutySchedule_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: EmailArchieve FK_EmailArchieve_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EmailArchieve"
    ADD CONSTRAINT "FK_EmailArchieve_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: EmailArchieve FK_EmailArchieve_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."EmailArchieve"
    ADD CONSTRAINT "FK_EmailArchieve_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: FingerPrint FK_FingerPrint_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FingerPrint"
    ADD CONSTRAINT "FK_FingerPrint_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: FingerPrint FK_FingerPrint_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FingerPrint"
    ADD CONSTRAINT "FK_FingerPrint_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Fortest FK_Fortest_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Fortest"
    ADD CONSTRAINT "FK_Fortest_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Fortest FK_Fortest_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Fortest"
    ADD CONSTRAINT "FK_Fortest_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: GenerateLog FK_GenerateLog_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GenerateLog"
    ADD CONSTRAINT "FK_GenerateLog_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: GenerateLog FK_GenerateLog_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."GenerateLog"
    ADD CONSTRAINT "FK_GenerateLog_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Holidays FK_Holidays_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Holidays"
    ADD CONSTRAINT "FK_Holidays_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Holidays FK_Holidays_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Holidays"
    ADD CONSTRAINT "FK_Holidays_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: JobStage FK_JobStage_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."JobStage"
    ADD CONSTRAINT "FK_JobStage_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: JobStage FK_JobStage_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."JobStage"
    ADD CONSTRAINT "FK_JobStage_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Language FK_Language_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Language"
    ADD CONSTRAINT "FK_Language_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Language FK_Language_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Language"
    ADD CONSTRAINT "FK_Language_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: LocaleResource FK_LocaleResource_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LocaleResource"
    ADD CONSTRAINT "FK_LocaleResource_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: LocaleResource FK_LocaleResource_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LocaleResource"
    ADD CONSTRAINT "FK_LocaleResource_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: LocaleResource FK_LocaleResource_Language_LanguageId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."LocaleResource"
    ADD CONSTRAINT "FK_LocaleResource_Language_LanguageId" FOREIGN KEY ("LanguageId") REFERENCES public."Language"("Id") ON DELETE CASCADE;


--
-- Name: NetworkNumber FK_NetworkNumber_AccountName_AccountNameId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NetworkNumber"
    ADD CONSTRAINT "FK_NetworkNumber_AccountName_AccountNameId" FOREIGN KEY ("AccountNameId") REFERENCES public."AccountName"("Id") ON DELETE CASCADE;


--
-- Name: NetworkNumber FK_NetworkNumber_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NetworkNumber"
    ADD CONSTRAINT "FK_NetworkNumber_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: NetworkNumber FK_NetworkNumber_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NetworkNumber"
    ADD CONSTRAINT "FK_NetworkNumber_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: NetworkNumber FK_NetworkNumber_Departement_DepartmentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NetworkNumber"
    ADD CONSTRAINT "FK_NetworkNumber_Departement_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES public."Departement"("Id") ON DELETE CASCADE;


--
-- Name: NetworkNumber FK_NetworkNumber_Projects_ProjectId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NetworkNumber"
    ADD CONSTRAINT "FK_NetworkNumber_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES public."Projects"("Id") ON DELETE CASCADE;


--
-- Name: NetworkNumber FK_NetworkNumber_UserProfile_LineManagerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NetworkNumber"
    ADD CONSTRAINT "FK_NetworkNumber_UserProfile_LineManagerId" FOREIGN KEY ("LineManagerId") REFERENCES public."UserProfile"("Id") ON DELETE CASCADE;


--
-- Name: NetworkNumber FK_NetworkNumber_UserProfile_ProjectManagerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."NetworkNumber"
    ADD CONSTRAINT "FK_NetworkNumber_UserProfile_ProjectManagerId" FOREIGN KEY ("ProjectManagerId") REFERENCES public."UserProfile"("Id") ON DELETE CASCADE;


--
-- Name: PackageType FK_PackageType_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PackageType"
    ADD CONSTRAINT "FK_PackageType_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: PackageType FK_PackageType_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PackageType"
    ADD CONSTRAINT "FK_PackageType_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: PanelCategory FK_PanelCategory_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PanelCategory"
    ADD CONSTRAINT "FK_PanelCategory_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: PanelCategory FK_PanelCategory_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."PanelCategory"
    ADD CONSTRAINT "FK_PanelCategory_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Position FK_Position_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Position"
    ADD CONSTRAINT "FK_Position_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Position FK_Position_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Position"
    ADD CONSTRAINT "FK_Position_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Projects FK_Projects_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Projects"
    ADD CONSTRAINT "FK_Projects_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Projects FK_Projects_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Projects"
    ADD CONSTRAINT "FK_Projects_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: RequestSpareParts FK_RequestSpareParts_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestSpareParts"
    ADD CONSTRAINT "FK_RequestSpareParts_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: RequestSpareParts FK_RequestSpareParts_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestSpareParts"
    ADD CONSTRAINT "FK_RequestSpareParts_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: RequestSpareParts FK_RequestSpareParts_PanelCategory_PanelCategoryId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RequestSpareParts"
    ADD CONSTRAINT "FK_RequestSpareParts_PanelCategory_PanelCategoryId" FOREIGN KEY ("PanelCategoryId") REFERENCES public."PanelCategory"("Id") ON DELETE CASCADE;


--
-- Name: ServicePackCategory FK_ServicePackCategory_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ServicePackCategory"
    ADD CONSTRAINT "FK_ServicePackCategory_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: ServicePackCategory FK_ServicePackCategory_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ServicePackCategory"
    ADD CONSTRAINT "FK_ServicePackCategory_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: ServicePack FK_ServicePack_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ServicePack"
    ADD CONSTRAINT "FK_ServicePack_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: ServicePack FK_ServicePack_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ServicePack"
    ADD CONSTRAINT "FK_ServicePack_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: ServicePack FK_ServicePack_ServicePackCategory_ServicePackCategoryId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ServicePack"
    ADD CONSTRAINT "FK_ServicePack_ServicePackCategory_ServicePackCategoryId" FOREIGN KEY ("ServicePackCategoryId") REFERENCES public."ServicePackCategory"("Id") ON DELETE CASCADE;


--
-- Name: SrfEscalationRequest FK_SrfEscalationRequest_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfEscalationRequest"
    ADD CONSTRAINT "FK_SrfEscalationRequest_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SrfEscalationRequest FK_SrfEscalationRequest_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfEscalationRequest"
    ADD CONSTRAINT "FK_SrfEscalationRequest_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SrfEscalationRequest FK_SrfEscalationRequest_ServicePack_ServicePackId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfEscalationRequest"
    ADD CONSTRAINT "FK_SrfEscalationRequest_ServicePack_ServicePackId" FOREIGN KEY ("ServicePackId") REFERENCES public."ServicePack"("Id") ON DELETE CASCADE;


--
-- Name: SrfEscalationRequest FK_SrfEscalationRequest_SrfRequest_SrfId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfEscalationRequest"
    ADD CONSTRAINT "FK_SrfEscalationRequest_SrfRequest_SrfId" FOREIGN KEY ("SrfId") REFERENCES public."SrfRequest"("Id") ON DELETE CASCADE;


--
-- Name: SrfRequest FK_SrfRequest_AccountName_AccountId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_AccountName_AccountId" FOREIGN KEY ("AccountId") REFERENCES public."AccountName"("Id");


--
-- Name: SrfRequest FK_SrfRequest_ActivityCode_ActivityId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_ActivityCode_ActivityId" FOREIGN KEY ("ActivityId") REFERENCES public."ActivityCode"("Id");


--
-- Name: SrfRequest FK_SrfRequest_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SrfRequest FK_SrfRequest_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SrfRequest FK_SrfRequest_CandidateInfo_CandidateId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_CandidateInfo_CandidateId" FOREIGN KEY ("CandidateId") REFERENCES public."CandidateInfo"("Id") ON DELETE CASCADE;


--
-- Name: SrfRequest FK_SrfRequest_CostCenter_CostCenterId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_CostCenter_CostCenterId" FOREIGN KEY ("CostCenterId") REFERENCES public."CostCenter"("Id") ON DELETE CASCADE;


--
-- Name: SrfRequest FK_SrfRequest_DepartementSub_DepartmentSubId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_DepartementSub_DepartmentSubId" FOREIGN KEY ("DepartmentSubId") REFERENCES public."DepartementSub"("Id") ON DELETE CASCADE;


--
-- Name: SrfRequest FK_SrfRequest_Departement_DepartmentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_Departement_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES public."Departement"("Id") ON DELETE CASCADE;


--
-- Name: SrfRequest FK_SrfRequest_NetworkNumber_NetworkId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_NetworkNumber_NetworkId" FOREIGN KEY ("NetworkId") REFERENCES public."NetworkNumber"("Id") ON DELETE CASCADE;


--
-- Name: SrfRequest FK_SrfRequest_ServicePack_ServicePackId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_ServicePack_ServicePackId" FOREIGN KEY ("ServicePackId") REFERENCES public."ServicePack"("Id") ON DELETE CASCADE;


--
-- Name: SrfRequest FK_SrfRequest_UserProfile_ApproveFiveId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_UserProfile_ApproveFiveId" FOREIGN KEY ("ApproveFiveId") REFERENCES public."UserProfile"("Id");


--
-- Name: SrfRequest FK_SrfRequest_UserProfile_ApproveFourId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_UserProfile_ApproveFourId" FOREIGN KEY ("ApproveFourId") REFERENCES public."UserProfile"("Id");


--
-- Name: SrfRequest FK_SrfRequest_UserProfile_ApproveOneId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_UserProfile_ApproveOneId" FOREIGN KEY ("ApproveOneId") REFERENCES public."UserProfile"("Id");


--
-- Name: SrfRequest FK_SrfRequest_UserProfile_ApproveSixId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_UserProfile_ApproveSixId" FOREIGN KEY ("ApproveSixId") REFERENCES public."UserProfile"("Id");


--
-- Name: SrfRequest FK_SrfRequest_UserProfile_ApproveThreeId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_UserProfile_ApproveThreeId" FOREIGN KEY ("ApproveThreeId") REFERENCES public."UserProfile"("Id");


--
-- Name: SrfRequest FK_SrfRequest_UserProfile_ApproveTwoId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_UserProfile_ApproveTwoId" FOREIGN KEY ("ApproveTwoId") REFERENCES public."UserProfile"("Id");


--
-- Name: SrfRequest FK_SrfRequest_UserProfile_LineManagerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_UserProfile_LineManagerId" FOREIGN KEY ("LineManagerId") REFERENCES public."UserProfile"("Id") ON DELETE CASCADE;


--
-- Name: SrfRequest FK_SrfRequest_UserProfile_ProjectManagerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SrfRequest"
    ADD CONSTRAINT "FK_SrfRequest_UserProfile_ProjectManagerId" FOREIGN KEY ("ProjectManagerId") REFERENCES public."UserProfile"("Id") ON DELETE CASCADE;


--
-- Name: SubOps FK_SubOps_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SubOps"
    ADD CONSTRAINT "FK_SubOps_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SubOps FK_SubOps_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SubOps"
    ADD CONSTRAINT "FK_SubOps_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Subdivision FK_Subdivision_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Subdivision"
    ADD CONSTRAINT "FK_Subdivision_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Subdivision FK_Subdivision_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Subdivision"
    ADD CONSTRAINT "FK_Subdivision_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SystemBranch FK_SystemBranch_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SystemBranch"
    ADD CONSTRAINT "FK_SystemBranch_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SystemBranch FK_SystemBranch_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SystemBranch"
    ADD CONSTRAINT "FK_SystemBranch_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SystemPropertiesRecord FK_SystemPropertiesRecord_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SystemPropertiesRecord"
    ADD CONSTRAINT "FK_SystemPropertiesRecord_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: SystemPropertiesRecord FK_SystemPropertiesRecord_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."SystemPropertiesRecord"
    ADD CONSTRAINT "FK_SystemPropertiesRecord_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TacticalResource FK_TacticalResource_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TacticalResource"
    ADD CONSTRAINT "FK_TacticalResource_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TacticalResource FK_TacticalResource_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TacticalResource"
    ADD CONSTRAINT "FK_TacticalResource_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TacticalResource FK_TacticalResource_DepartementSub_DepartmentSubId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TacticalResource"
    ADD CONSTRAINT "FK_TacticalResource_DepartementSub_DepartmentSubId" FOREIGN KEY ("DepartmentSubId") REFERENCES public."DepartementSub"("Id");


--
-- Name: TacticalResource FK_TacticalResource_Departement_DepartmentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TacticalResource"
    ADD CONSTRAINT "FK_TacticalResource_Departement_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES public."Departement"("Id");


--
-- Name: TicketInfo FK_TicketInfo_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TicketInfo"
    ADD CONSTRAINT "FK_TicketInfo_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TicketInfo FK_TicketInfo_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TicketInfo"
    ADD CONSTRAINT "FK_TicketInfo_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TicketInfo FK_TicketInfo_Claim_ClaimId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TicketInfo"
    ADD CONSTRAINT "FK_TicketInfo_Claim_ClaimId" FOREIGN KEY ("ClaimId") REFERENCES public."Claim"("Id") ON DELETE CASCADE;


--
-- Name: TicketReply FK_TicketReply_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TicketReply"
    ADD CONSTRAINT "FK_TicketReply_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TicketReply FK_TicketReply_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TicketReply"
    ADD CONSTRAINT "FK_TicketReply_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TicketReply FK_TicketReply_Ticket_TicketId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TicketReply"
    ADD CONSTRAINT "FK_TicketReply_Ticket_TicketId" FOREIGN KEY ("TicketId") REFERENCES public."Ticket"("Id") ON DELETE CASCADE;


--
-- Name: Ticket FK_Ticket_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Ticket"
    ADD CONSTRAINT "FK_Ticket_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: Ticket FK_Ticket_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Ticket"
    ADD CONSTRAINT "FK_Ticket_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TimeSheetPeriod FK_TimeSheetPeriod_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TimeSheetPeriod"
    ADD CONSTRAINT "FK_TimeSheetPeriod_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TimeSheetPeriod FK_TimeSheetPeriod_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TimeSheetPeriod"
    ADD CONSTRAINT "FK_TimeSheetPeriod_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TimeSheetType FK_TimeSheetType_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TimeSheetType"
    ADD CONSTRAINT "FK_TimeSheetType_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: TimeSheetType FK_TimeSheetType_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TimeSheetType"
    ADD CONSTRAINT "FK_TimeSheetType_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: UserProfile FK_UserProfile_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."UserProfile"
    ADD CONSTRAINT "FK_UserProfile_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id");


--
-- Name: VacancyList FK_VacancyList_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: VacancyList FK_VacancyList_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: VacancyList FK_VacancyList_DepartementSub_DepartmentSubId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_DepartementSub_DepartmentSubId" FOREIGN KEY ("DepartmentSubId") REFERENCES public."DepartementSub"("Id") ON DELETE CASCADE;


--
-- Name: VacancyList FK_VacancyList_Departement_DepartmentId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_Departement_DepartmentId" FOREIGN KEY ("DepartmentId") REFERENCES public."Departement"("Id") ON DELETE CASCADE;


--
-- Name: VacancyList FK_VacancyList_NetworkNumber_NetworkId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_NetworkNumber_NetworkId" FOREIGN KEY ("NetworkId") REFERENCES public."NetworkNumber"("Id") ON DELETE CASCADE;


--
-- Name: VacancyList FK_VacancyList_ServicePackCategory_ServicePackCategoryId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_ServicePackCategory_ServicePackCategoryId" FOREIGN KEY ("ServicePackCategoryId") REFERENCES public."ServicePackCategory"("Id") ON DELETE CASCADE;


--
-- Name: VacancyList FK_VacancyList_ServicePack_ServicePackId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_ServicePack_ServicePackId" FOREIGN KEY ("ServicePackId") REFERENCES public."ServicePack"("Id") ON DELETE CASCADE;


--
-- Name: VacancyList FK_VacancyList_UserProfile_ApproverOneId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_UserProfile_ApproverOneId" FOREIGN KEY ("ApproverOneId") REFERENCES public."UserProfile"("Id");


--
-- Name: VacancyList FK_VacancyList_UserProfile_ApproverTwoId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_UserProfile_ApproverTwoId" FOREIGN KEY ("ApproverTwoId") REFERENCES public."UserProfile"("Id");


--
-- Name: VacancyList FK_VacancyList_UserProfile_RequestById; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "FK_VacancyList_UserProfile_RequestById" FOREIGN KEY ("RequestById") REFERENCES public."UserProfile"("Id");


--
-- Name: WebSetting FK_WebSetting_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WebSetting"
    ADD CONSTRAINT "FK_WebSetting_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: WebSetting FK_WebSetting_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WebSetting"
    ADD CONSTRAINT "FK_WebSetting_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: WoItem FK_WoItem_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WoItem"
    ADD CONSTRAINT "FK_WoItem_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: WoItem FK_WoItem_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WoItem"
    ADD CONSTRAINT "FK_WoItem_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: WotList FK_WotList_AspNetUsers_CreatedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WotList"
    ADD CONSTRAINT "FK_WotList_AspNetUsers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: WotList FK_WotList_AspNetUsers_LastEditedBy; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WotList"
    ADD CONSTRAINT "FK_WotList_AspNetUsers_LastEditedBy" FOREIGN KEY ("LastEditedBy") REFERENCES public."AspNetUsers"("Id");


--
-- Name: VacancyList VacancyList_VendorId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."VacancyList"
    ADD CONSTRAINT "VacancyList_VendorId_fkey" FOREIGN KEY ("VendorId") REFERENCES public."UserProfile"("Id");


--
-- Name: WorkPackage WorkPackage_AccountNameId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WorkPackage"
    ADD CONSTRAINT "WorkPackage_AccountNameId_fkey" FOREIGN KEY ("AccountNameId") REFERENCES public."AccountName"("Id");


--
-- Name: WorkPackage WorkPackage_ProjectId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WorkPackage"
    ADD CONSTRAINT "WorkPackage_ProjectId_fkey" FOREIGN KEY ("ProjectId") REFERENCES public."Projects"("Id");


--
-- Name: WorkPackage WorkPackage_ProjectManagerId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WorkPackage"
    ADD CONSTRAINT "WorkPackage_ProjectManagerId_fkey" FOREIGN KEY ("ProjectManagerId") REFERENCES public."UserProfile"("Id");


--
-- Name: WorkPackage WorkPackage_SsowId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WorkPackage"
    ADD CONSTRAINT "WorkPackage_SsowId_fkey" FOREIGN KEY ("SsowId") REFERENCES public."ServicePack"("Id");


--
-- Name: WorkPackage WorkPackage_TotalProjectManagerId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WorkPackage"
    ADD CONSTRAINT "WorkPackage_TotalProjectManagerId_fkey" FOREIGN KEY ("TotalProjectManagerId") REFERENCES public."UserProfile"("Id");


--
-- Name: WorkPackage WorkPackage_VendorId_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."WorkPackage"
    ADD CONSTRAINT "WorkPackage_VendorId_fkey" FOREIGN KEY ("VendorId") REFERENCES public."UserProfile"("Id");


--
-- PostgreSQL database dump complete
--

