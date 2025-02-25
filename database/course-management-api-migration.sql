DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'CourseManagement') THEN
        CREATE SCHEMA "CourseManagement";
    END IF;
END $EF$;
CREATE TABLE IF NOT EXISTS "CourseManagement"."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'CourseManagement') THEN
            CREATE SCHEMA "CourseManagement";
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    CREATE TABLE "CourseManagement"."Classes" (
        "Id" uuid NOT NULL,
        "Name" character varying(200) NOT NULL,
        "Description" character varying(500) NOT NULL,
        "CreatedById" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Classes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    CREATE TABLE "CourseManagement"."Courses" (
        "Id" uuid NOT NULL,
        "Name" character varying(200) NOT NULL,
        "Description" character varying(500) NOT NULL,
        "CreatedById" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Courses" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    CREATE TABLE "CourseManagement"."Students" (
        "Id" uuid NOT NULL,
        "FullName" character varying(200) NOT NULL,
        "Email" character varying(200) NOT NULL,
        "DateOfBirth" timestamp with time zone NOT NULL,
        "UserId" uuid NOT NULL,
        CONSTRAINT "PK_Students" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    CREATE TABLE "CourseManagement"."ClassCourse" (
        "ClassId" uuid NOT NULL,
        "CourseId" uuid NOT NULL,
        "AssignedBy" uuid NOT NULL,
        CONSTRAINT "PK_ClassCourse" PRIMARY KEY ("ClassId", "CourseId"),
        CONSTRAINT "FK_ClassCourse_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES "CourseManagement"."Classes" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ClassCourse_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "CourseManagement"."Courses" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    CREATE TABLE "CourseManagement"."ClassStudent" (
        "ClassId" uuid NOT NULL,
        "StudentId" uuid NOT NULL,
        "AssignedBy" uuid NOT NULL,
        CONSTRAINT "PK_ClassStudent" PRIMARY KEY ("ClassId", "StudentId"),
        CONSTRAINT "FK_ClassStudent_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES "CourseManagement"."Classes" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ClassStudent_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "CourseManagement"."Students" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    CREATE TABLE "CourseManagement"."CourseStudent" (
        "CourseId" uuid NOT NULL,
        "StudentId" uuid NOT NULL,
        "AssignedBy" uuid NOT NULL,
        CONSTRAINT "PK_CourseStudent" PRIMARY KEY ("CourseId", "StudentId"),
        CONSTRAINT "FK_CourseStudent_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "CourseManagement"."Courses" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_CourseStudent_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "CourseManagement"."Students" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    CREATE INDEX "IX_ClassCourse_CourseId" ON "CourseManagement"."ClassCourse" ("CourseId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    CREATE INDEX "IX_ClassStudent_StudentId" ON "CourseManagement"."ClassStudent" ("StudentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    CREATE INDEX "IX_CourseStudent_StudentId" ON "CourseManagement"."CourseStudent" ("StudentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250222145548_test_migration') THEN
    INSERT INTO "CourseManagement"."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250222145548_test_migration', '9.0.2');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."CourseStudent" RENAME COLUMN "AssignedBy" TO "AssignedById";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."ClassStudent" RENAME COLUMN "AssignedBy" TO "AssignedById";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."ClassCourse" RENAME COLUMN "AssignedBy" TO "AssignedById";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."Students" ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."Students" ADD "CreatedById" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."Students" ADD "CreatedByName" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."CourseStudent" ADD "AssignedByName" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."Courses" ADD "CreatedByName" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."ClassStudent" ADD "AssignedByName" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."Classes" ADD "CreatedByName" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    ALTER TABLE "CourseManagement"."ClassCourse" ADD "AssignedByName" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250225073446_init_migration') THEN
    INSERT INTO "CourseManagement"."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250225073446_init_migration', '9.0.2');
    END IF;
END $EF$;
COMMIT;

