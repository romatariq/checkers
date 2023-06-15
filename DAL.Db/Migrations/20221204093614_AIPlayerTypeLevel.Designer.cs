﻿// <auto-generated />
using System;
using DAL.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Db.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20221204093614_AIPlayerTypeLevel")]
    partial class AIPlayerTypeLevel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameSettingId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("Player1Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<int>("Player1Type")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Player2Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<int>("Player2Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GameSettingId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Domain.GameSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AllButtonCanEatKing")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BoardHeight")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BoardWidth")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanEatBackwards")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("HasToCapture")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("KingCanMoveOnlyOneStep")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<bool>("WhiteStarts")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("GameSettings");
                });

            modelBuilder.Entity("Domain.GameState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("CurrentPlayer")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("EndedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SerializedBoard")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("StartFromX")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("StartFromY")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameStates");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.HasOne("Domain.GameSetting", "GameSetting")
                        .WithMany("Games")
                        .HasForeignKey("GameSettingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameSetting");
                });

            modelBuilder.Entity("Domain.GameState", b =>
                {
                    b.HasOne("Domain.Game", "Game")
                        .WithMany("GameStates")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Navigation("GameStates");
                });

            modelBuilder.Entity("Domain.GameSetting", b =>
                {
                    b.Navigation("Games");
                });
#pragma warning restore 612, 618
        }
    }
}
