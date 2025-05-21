using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UbiquoStub.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Method = table.Column<string>(type: "text", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false),
                    Headers = table.Column<string>(type: "jsonb", nullable: true),
                    Body = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResponseEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Headers = table.Column<string>(type: "jsonb", nullable: true),
                    Body = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sut",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sut", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stub",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TestName = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Host = table.Column<string>(type: "text", nullable: false),
                    RequestId = table.Column<long>(type: "bigint", nullable: false),
                    ResponseId = table.Column<long>(type: "bigint", nullable: false),
                    SutId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stub", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stub_RequestEntity_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stub_ResponseEntity_ResponseId",
                        column: x => x.ResponseId,
                        principalTable: "ResponseEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stub_Sut_SutId",
                        column: x => x.SutId,
                        principalTable: "Sut",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StubResult",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StubId = table.Column<long>(type: "bigint", nullable: false),
                    IsIntegration = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ActualResponse = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StubResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StubResult_Stub_StubId",
                        column: x => x.StubId,
                        principalTable: "Stub",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stub_RequestId",
                table: "Stub",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Stub_ResponseId",
                table: "Stub",
                column: "ResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_Stub_SutId",
                table: "Stub",
                column: "SutId");

            migrationBuilder.CreateIndex(
                name: "IX_StubResult_StubId",
                table: "StubResult",
                column: "StubId");

            migrationBuilder.CreateIndex(
                name: "IX_Sut_Name",
                table: "Sut",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StubResult");

            migrationBuilder.DropTable(
                name: "Stub");

            migrationBuilder.DropTable(
                name: "RequestEntity");

            migrationBuilder.DropTable(
                name: "ResponseEntity");

            migrationBuilder.DropTable(
                name: "Sut");
        }
    }
}
