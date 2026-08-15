-- Manual dev seed: one Organization + one Manager, matching the
-- "Seed" defaults in appsettings.Development.json (manager@qrbin.local / ChangeMe123!).
-- Run after Initial_Script.sql and Remove_Identity_And_Actor_Tracking.sql.
-- Skips itself if an Organization already exists.

DO $$
DECLARE
    v_org_id integer;
BEGIN
    IF EXISTS (SELECT 1 FROM "Organization") THEN
        RAISE NOTICE 'Organization already exists — skipping seed.';
        RETURN;
    END IF;

    INSERT INTO "Organization" ("Name")
    VALUES ('Default Workshop')
    RETURNING "Id" INTO v_org_id;

    INSERT INTO "Manager" ("OrganizationId", "FullName", "Email", "PasswordHash")
    VALUES (
        v_org_id, 'Default Manager', 'manager@qrbin.local',
        -- PBKDF2-HMACSHA256 hash of 'ChangeMe123!' produced by QrBin.Api.Services.PasswordHasher
        '6ooq6w9ziXZuPPrYprzlDQABhqADNebKmH3Yklf4wnlGrGin58THeeoQWKbPBdle/69JWg=='
    );
END $$;
