using MauiApp1.Model;
using MauiApp1.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.DataAccess;
public class IneDbContext : DbContext
{
    public DbSet<Ine> Ines { get; set; }

    private readonly IDatabaseService _databaseService;

    public IneDbContext(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = $"FileName={_databaseService.Get("IneDatabase.db")}";
        optionsBuilder.UseSqlite(connectionString);
    }
}

public record Ine(string ByteBase64, string ContentType, string FileName, string Seccion, string Usuario)
{
    public int Id { get; set; }
}


