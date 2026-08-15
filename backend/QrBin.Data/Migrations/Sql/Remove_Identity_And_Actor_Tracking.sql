START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    DROP TABLE "AspNetUserClaims";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    DROP TABLE "AspNetUserLogins";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    DROP TABLE "AspNetUserTokens";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    DROP TABLE "Technician";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    DROP INDEX "EmailIndex";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    DROP INDEX "UserNameIndex";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "StockTransaction" DROP COLUMN "ActorId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "StockTransaction" DROP COLUMN "ActorName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "StockTransaction" DROP COLUMN "ActorType";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "AccessFailedCount";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "ConcurrencyStamp";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "EmailConfirmed";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "LockoutEnabled";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "LockoutEnd";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "NormalizedEmail";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "NormalizedUserName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "PhoneNumber";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "PhoneNumberConfirmed";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "SecurityStamp";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "TwoFactorEnabled";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    ALTER TABLE "Manager" DROP COLUMN "UserName";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    UPDATE "Manager" SET "PasswordHash" = '' WHERE "PasswordHash" IS NULL;
    ALTER TABLE "Manager" ALTER COLUMN "PasswordHash" SET NOT NULL;
    ALTER TABLE "Manager" ALTER COLUMN "PasswordHash" SET DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    UPDATE "Manager" SET "Email" = '' WHERE "Email" IS NULL;
    ALTER TABLE "Manager" ALTER COLUMN "Email" SET NOT NULL;
    ALTER TABLE "Manager" ALTER COLUMN "Email" SET DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    CREATE UNIQUE INDEX "IX_Manager_Email" ON "Manager" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260814122103_Remove_Identity_And_Actor_Tracking') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260814122103_Remove_Identity_And_Actor_Tracking', '10.0.11');
    END IF;
END $EF$;
COMMIT;
