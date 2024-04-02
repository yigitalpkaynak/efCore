using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;
using ConsoleApp.Data.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace ConsoleApp
{     
    public class ShopContext: DbContext
    {        
        public DbSet<Product> Products {get;set;}
        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
            .UseLoggerFactory(MyLoggerFactory)
            .UseMySql(@"server=localhost;port=3306;database=ShopDb;user=root;password=123456;", ServerVersion.AutoDetect(@"server=localhost;port=3306;database=ShopDb;user=root;password=123456;"), null);
            // .UseSqlite("Data Source=shop.db");
            // .UseSqlServer(@"Data Source =.\SQLEXPRESS;Initial Catalog=ShopDb4;Integrated Security=SSPI;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Username)
                        .IsUnique();

            modelBuilder.Entity<Product>()
                        .ToTable("Urunler");

            modelBuilder.Entity<Customer>()
                        .Property(p => p.IdentityNumber)
                        .HasMaxLength(11)
                        .IsRequired();

            modelBuilder.Entity<ProductCategory>()
                        .HasKey(t => new {t.ProductId, t.CategoryId});
            
            modelBuilder.Entity<ProductCategory>()
                        .HasOne(pc => pc.Product)
                        .WithMany(p => p.ProductCategories)
                        .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCategory>()
                        .HasOne(pc => pc.Category)
                        .WithMany(c => c.ProductCategories)
                        .HasForeignKey(pc => pc.CategoryId);
        }
    }

    public static class DataSeeding
    {
        public static void Seed(DbContext context)
        {
            if (context.Database.GetPendingMigrations().Count()==0)
            {
                if (context is ShopContext)
                {
                    ShopContext _context = context as ShopContext;

                    if (_context.Products.Count()==0)
                    {
                        _context.Products.AddRange(Products);
                    }

                    if(_context.Categories.Count()==0)
                    {
                        _context.Categories.AddRange(Categories);
                    }
                }

                context.SaveChanges();
            }
        }
  
        private static Product[] Products = {
            new Product(){Name = "Samsung S6", Price = 2000},
            new Product(){Name = "Samsung S7", Price = 3000},
            new Product(){Name = "Samsung S8", Price = 4000},
            new Product(){Name = "Samsung S9", Price = 5000},
        };

        private static Category[] Categories = {
            new Category(){Name = "Telefon"},
            new Category(){Name = "Elektronik"},
            new Category(){Name = "Bilgisayar"},
        };
    }
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MinLength(8),MaxLength(15)]
        public string Username { get; set; }
        [Column(TypeName="varchar(20)")]
        public string Email { get; set; }
        public Customer Customer { get; set; }

        public List<Address> Addresses { get; set; }
    }

    public class Customer
    {
        [Column("customer_id")]
        public int Id { get; set; }
        [Required]
        public string IdentityNumber { get; set; }
         [Required]
        public string FirstName { get; set; }
         [Required]
        public string LastName { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
    }

    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxNumber { get; set; }
    }
    public class Address
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public User User { get; set; }
        public int? UserId { get; set; }
    }

    public class Product
    { 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime InsertedDate { get; set; } = DateTime.Now;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime LastUpdatedDate { get; set; } = DateTime.Now;
        public List<ProductCategory> ProductCategories { get; set; }

    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }

    [Table("UrunKategorileri")]
    public class ProductCategory
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // DataSeeding.Seed(new ShopContext());

            using(var db = new NorthwindContext())
            {
                var products = db.Products.ToList();

                foreach (var item in products)
                {
                    Console.WriteLine(item.ProductName);
                }
            }
        }

        static void InsertUsers()
        {
            var users = new List<User>(){
                new User(){Username="yigitalpkaynak", Email="info@gmail.com"},
                new User(){Username="abc", Email="info2@gmail.com"},
                new User(){Username="dsa", Email="info3@gmail.com"},
                new User(){Username="eess", Email="info4@gmail.com"},
            };

            using(var db = new ShopContext())
            {
                db.Users.AddRange(users);
                db.SaveChanges();
            }
        }

        static void InsertAddresses() {
            var addresses = new List<Address>(){
                new Address(){ Fullname = "Yigit Alp Kaynak", Title = "Ev adresi", Body = "Bursa",UserId = 1 },
                new Address(){ Fullname = "Asiye Kaynak", Title = "Ev adresi", Body = "Bursa",UserId = 1 },
                new Address(){ Fullname = "Atilla Kaynak", Title = "Ev adresi", Body = "Bursa",UserId = 3 },
                new Address(){ Fullname = "Atilla Kaynak", Title = "İş adresi", Body = "Bursa",UserId = 3 },
                new Address(){ Fullname = "Aydın Kaynak", Title = "İş adresi", Body = "Bursa",UserId = 2 },
                new Address(){ Fullname = "Cengiz Kaynak", Title = "İş adresi", Body = "Bursa",UserId = 4 },
            };

            using(var db = new ShopContext())
            {
                db.Addresses.AddRange(addresses);
                db.SaveChanges();
            }
        }

        // static void DeleteProduct(int id)
        // {
        //     using(var db = new ShopContext())
        //     {
        //         var p = new Product(){Id = 6};

        //         // db.Products.Remove(p);
        //         db.Entry(p).State = EntityState.Deleted;
        //         db.SaveChanges();
        //     }

        //     // using(var db = new ShopContext())
        //     // {
        //     //     var p = db.Products
        //     //     .FirstOrDefault(i => i.Id == id);

        //     //     if(p!=null)
        //     //     {
        //     //         db.Products.Remove(p);
        //     //         db.SaveChanges();

        //     //         Console.WriteLine("Veri silindi");
        //     //     }
        //     // }
        // }

        // static void UpdateProduct()
        // {
        //     using (var db = new ShopContext())
        //     {
        //         var p = db.Products
        //         .Where(i => i.Id==1)
        //         .FirstOrDefault();

        //         if(p!=null)
        //         {
        //             p.Price = 2400;
        //             db.Products.Update(p);
        //             db.SaveChanges();
        //         }

        //         // var entity = new Product(){Id = 1};

        //         // db.Products.Attach(entity);

        //         // entity.Price = 3000;

        //         // db.SaveChanges();
        //     }
            
        //     // using (var db = new ShopContext())
        //     // {
        //     //     var p = db
        //     //     .Products
        //     //     .AsNoTracking()
        //     //     .Where(i => i.Id == 1)
        //     //     .FirstOrDefault();
        //     //     if (p!=null)
        //     //     {
        //     //         p.Price *= 1.2m;

        //     //         db.SaveChanges();

        //     //         Console.WriteLine("Güncelleme yapıldı");
        //     //     }
        //     // }
        // }

        // static void GetProductById(int id)
        // {
        //     using(var context = new ShopContext())
        //     {
        //         var result = context
        //         .Products
        //         .Where(p => p.Id == id)
        //         .Select(p => 
        //         new {
        //             p.Name,
        //             p.Price
        //         })
        //         .FirstOrDefault();

        //             Console.WriteLine($"name: {result.Name} price: {result.Price}");
        //     }
        // }

        // static void GetProductByName(string name)
        // {
        //     using(var context = new ShopContext())
        //     {
        //         var result = context
        //         .Products
        //         .Where(p => p.Name.ToLower().Contains(name.ToLower()))
        //         .Select(p => 
        //         new {
        //             p.Name,
        //             p.Price
        //         })
        //         .ToList();

        //         foreach (var p in result)
        //         {
        //             Console.WriteLine($"name: {p.Name} price: {p.Price}");
        //         }
        //     }
        // }

        // static void GetAllProducts()
        // {
        //     using(var context = new ShopContext())
        //     {
        //         var products = context
        //         .Products
        //         .Select(p => 
        //         new {
        //             p.Name,
        //             p.Price
        //         })
        //         .ToList();

        //         foreach (var p in products)
        //         {
        //             Console.WriteLine($"name: {p.Name} price: {p.Price}");
        //         }
        //     }
        // }

        // static void AddProducts()
        // {
        //     using (var db = new ShopContext())
        //     {
        //         var products = new List<Product>()
        //         {
        //             new Product { Name = "Samsung S15", Price = 3000 },
        //             new Product { Name = "Samsung S16", Price = 4000 },
        //             new Product { Name = "Samsung S17", Price = 5000 },
        //             new Product { Name = "Samsung S18", Price = 6000 }
        //         };
        //         // foreach (var p in products)
        //         // {
        //         //     db.Products.Add(p);
        //         // }

        //         db.Products.AddRange(products);

        //         db.SaveChanges();

        //         Console.WriteLine("Veriler eklendi");
        //     }
        // }

        // static void AddProduct()
        // {
        //     using (var db = new ShopContext())
        //     {
        //         var p = new Product { Name = "Samsung S15", Price = 8000 };
        //         // foreach (var p in products)
        //         // {
        //         //     db.Products.Add(p);
        //         // }

        //         db.Products.Add(p);

        //         db.SaveChanges();

        //         Console.WriteLine("Veriler eklendi");
        //     }
        // }

    }
}