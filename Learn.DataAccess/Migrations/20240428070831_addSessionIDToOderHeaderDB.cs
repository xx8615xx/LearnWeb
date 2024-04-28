﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learn.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addSessionIDToOderHeaderDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionID",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionID",
                table: "OrderHeaders");
        }
    }
}
