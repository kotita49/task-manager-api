﻿using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data.Entities;

namespace TaskManager.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<TaskItem> Tasks { get; set; } = null!;
    }
}
