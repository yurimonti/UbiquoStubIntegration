CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "RequestEntity" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Method" text NOT NULL,
    "Uri" text NOT NULL,
    "Headers" jsonb,
    "Body" jsonb,
    CONSTRAINT "PK_RequestEntity" PRIMARY KEY ("Id")
);

CREATE TABLE "ResponseEntity" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Status" integer NOT NULL,
    "Headers" jsonb,
    "Body" jsonb,
    CONSTRAINT "PK_ResponseEntity" PRIMARY KEY ("Id")
);

CREATE TABLE "Sut" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    CONSTRAINT "PK_Sut" PRIMARY KEY ("Id")
);

CREATE TABLE "Stub" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    "TestName" text NOT NULL,
    "Order" integer NOT NULL,
    "Host" text NOT NULL,
    "RequestId" bigint NOT NULL,
    "ResponseId" bigint NOT NULL,
    "SutId" bigint,
    CONSTRAINT "PK_Stub" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Stub_RequestEntity_RequestId" FOREIGN KEY ("RequestId") REFERENCES "RequestEntity" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Stub_ResponseEntity_ResponseId" FOREIGN KEY ("ResponseId") REFERENCES "ResponseEntity" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Stub_Sut_SutId" FOREIGN KEY ("SutId") REFERENCES "Sut" ("Id") ON DELETE CASCADE
);

CREATE TABLE "StubResult" (
    "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
    "StubId" bigint NOT NULL,
    "IsIntegration" boolean NOT NULL,
    "Status" text NOT NULL,
    "ActualResponse" jsonb,
    CONSTRAINT "PK_StubResult" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StubResult_Stub_StubId" FOREIGN KEY ("StubId") REFERENCES "Stub" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Stub_RequestId" ON "Stub" ("RequestId");

CREATE INDEX "IX_Stub_ResponseId" ON "Stub" ("ResponseId");

CREATE INDEX "IX_Stub_SutId" ON "Stub" ("SutId");

CREATE INDEX "IX_StubResult_StubId" ON "StubResult" ("StubId");

CREATE UNIQUE INDEX "IX_Sut_Name" ON "Sut" ("Name");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250520072905_InitialCreate', '9.0.4');

ALTER TABLE "StubResult" ADD "StubDto" jsonb NOT NULL DEFAULT '{}';

ALTER TABLE "Stub" ALTER COLUMN "TestName" DROP NOT NULL;

ALTER TABLE "Stub" ALTER COLUMN "Order" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250606100658_InitialMigration', '9.0.4');

COMMIT;