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
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'CourseManagement') THEN
            CREATE SCHEMA "CourseManagement";
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    CREATE TABLE "CourseManagement"."Classes" (
        "Id" uuid NOT NULL,
        "Name" character varying(200) NOT NULL,
        "Description" character varying(500) NOT NULL,
        "CreatedById" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "CreatedByName" text NOT NULL,
        CONSTRAINT "PK_Classes" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    CREATE TABLE "CourseManagement"."Courses" (
        "Id" uuid NOT NULL,
        "Name" character varying(200) NOT NULL,
        "Description" character varying(500) NOT NULL,
        "CreatedById" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "CreatedByName" text NOT NULL,
        CONSTRAINT "PK_Courses" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    CREATE TABLE "CourseManagement"."Students" (
        "Id" uuid NOT NULL,
        "FullName" character varying(200) NOT NULL,
        "Email" character varying(200) NOT NULL,
        "DateOfBirth" timestamp with time zone NOT NULL,
        "UserId" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "CreatedByName" text NOT NULL,
        "CreatedById" uuid NOT NULL,
        CONSTRAINT "PK_Students" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    CREATE TABLE "CourseManagement"."ClassCourses" (
        "ClassId" uuid NOT NULL,
        "CourseId" uuid NOT NULL,
        "AssignedAt" timestamp with time zone NOT NULL,
        "AssignedById" uuid NOT NULL,
        "AssignedByName" text NOT NULL,
        CONSTRAINT "PK_ClassCourses" PRIMARY KEY ("ClassId", "CourseId"),
        CONSTRAINT "FK_ClassCourses_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES "CourseManagement"."Classes" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ClassCourses_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "CourseManagement"."Courses" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    CREATE TABLE "CourseManagement"."ClassEnrollments" (
        "ClassId" uuid NOT NULL,
        "StudentId" uuid NOT NULL,
        "AssignedAt" timestamp with time zone NOT NULL,
        "AssignedById" uuid NOT NULL,
        "AssignedByName" text NOT NULL,
        CONSTRAINT "PK_ClassEnrollments" PRIMARY KEY ("ClassId", "StudentId"),
        CONSTRAINT "FK_ClassEnrollments_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES "CourseManagement"."Classes" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ClassEnrollments_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "CourseManagement"."Students" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    CREATE TABLE "CourseManagement"."CourseEnrollments" (
        "CourseId" uuid NOT NULL,
        "StudentId" uuid NOT NULL,
        "AssignedAt" timestamp with time zone NOT NULL,
        "AssignedById" uuid NOT NULL,
        "AssignedByName" text NOT NULL,
        CONSTRAINT "PK_CourseEnrollments" PRIMARY KEY ("CourseId", "StudentId"),
        CONSTRAINT "FK_CourseEnrollments_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "CourseManagement"."Courses" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_CourseEnrollments_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "CourseManagement"."Students" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    CREATE INDEX "IX_ClassCourses_CourseId" ON "CourseManagement"."ClassCourses" ("CourseId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    CREATE INDEX "IX_ClassEnrollments_StudentId" ON "CourseManagement"."ClassEnrollments" ("StudentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    CREATE INDEX "IX_CourseEnrollments_StudentId" ON "CourseManagement"."CourseEnrollments" ("StudentId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "CourseManagement"."__EFMigrationsHistory" WHERE "MigrationId" = '20250302012427_init_migration') THEN
    INSERT INTO "CourseManagement"."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250302012427_init_migration', '9.0.2');
    END IF;
END $EF$;
COMMIT;

