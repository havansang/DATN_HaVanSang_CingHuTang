using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CingHuTang.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartTopping> CartToppings { get; set; }

    public virtual DbSet<GoodsIssue> GoodsIssues { get; set; }

    public virtual DbSet<GoodsIssueDetail> GoodsIssueDetails { get; set; }

    public virtual DbSet<GoodsIssueFile> GoodsIssueFiles { get; set; }

    public virtual DbSet<GoodsReceipt> GoodsReceipts { get; set; }

    public virtual DbSet<GoodsReceiptDetail> GoodsReceiptDetails { get; set; }

    public virtual DbSet<GoodsReceiptFile> GoodsReceiptFiles { get; set; }

    public virtual DbSet<LinkUrl> LinkUrls { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDetailsTopping> OrderDetailsToppings { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<ProductTopping> ProductToppings { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Topping> Toppings { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Config.Config.Connection());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.Gender).HasComment("0: Nam, 1: Nữ");
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(250);
            entity.Property(e => e.Role).HasComment("1: Khách hàng, 2: admin, 3: nhân viên, 4: chủ cửa hàng");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("Cart");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CartTopping>(entity =>
        {
            entity.ToTable("CartTopping");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ToppingId).HasColumnName("ToppingID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<GoodsIssue>(entity =>
        {
            entity.ToTable("GoodsIssue");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Decription).HasMaxLength(550);
            entity.Property(e => e.GoodIssueCode).HasMaxLength(150);
            entity.Property(e => e.IssueDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<GoodsIssueDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Decription).HasMaxLength(550);
            entity.Property(e => e.GoodIssueId).HasColumnName("GoodIssueID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<GoodsIssueFile>(entity =>
        {
            entity.ToTable("GoodsIssueFile");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GoodsIssueId).HasColumnName("GoodsIssueID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<GoodsReceipt>(entity =>
        {
            entity.ToTable("GoodsReceipt");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccoutId).HasColumnName("AccoutID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Decription).HasMaxLength(550);
            entity.Property(e => e.GoodsReceiptCode).HasMaxLength(250);
            entity.Property(e => e.ReceiptedDate).HasColumnType("datetime");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<GoodsReceiptDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GoodsReceiptId).HasColumnName("GoodsReceiptID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<GoodsReceiptFile>(entity =>
        {
            entity.ToTable("GoodsReceiptFile");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GoodsReceiptId).HasColumnName("GoodsReceiptID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<LinkUrl>(entity =>
        {
            entity.ToTable("LinkUrl");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.LinkUrl1).HasColumnName("LinkUrl");
            entity.Property(e => e.TypeUrl).HasMaxLength(250);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("Material");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.MaterialCode).HasMaxLength(250);
            entity.Property(e => e.MaterialName).HasMaxLength(250);
            entity.Property(e => e.MinQuantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitId).HasColumnName("UnitID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CreateBy).HasMaxLength(150);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(250);
            entity.Property(e => e.OrderCode).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(250);
            entity.Property(e => e.Status).HasComment("0: Chờ xác nhận; 1: Đang giao; 2: Thành công; 3: Hủy hàng");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductDetailId).HasColumnName("ProductDetailID");
            entity.Property(e => e.TotalMoney).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<OrderDetailsTopping>(entity =>
        {
            entity.ToTable("OrderDetailsTopping");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderDetailsId).HasColumnName("OrderDetailsID");
            entity.Property(e => e.ToppingId).HasColumnName("ToppingID");
            entity.Property(e => e.ToppingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.ProductCode).HasMaxLength(250);
            entity.Property(e => e.ProductName).HasMaxLength(250);
            entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProductSizeId).HasColumnName("ProductSizeID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("ProductImage");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.ToTable("ProductSize");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.SizeCode).HasMaxLength(250);
            entity.Property(e => e.SizeName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductTopping>(entity =>
        {
            entity.ToTable("ProductTopping");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ToppingId).HasColumnName("ToppingID");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.ToTable("ProductType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.GroupTypeId).HasColumnName("GroupTypeID");
            entity.Property(e => e.TypeCode).HasMaxLength(250);
            entity.Property(e => e.TypeName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Supplier");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PhoneNumber).HasMaxLength(250);
            entity.Property(e => e.SupplierCode).HasMaxLength(150);
            entity.Property(e => e.SupplierName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Topping>(entity =>
        {
            entity.ToTable("Topping");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ToppingCode).HasMaxLength(150);
            entity.Property(e => e.ToppingName).HasMaxLength(250);
            entity.Property(e => e.ToppingPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("Unit");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UnitCode).HasMaxLength(150);
            entity.Property(e => e.UnitName).HasMaxLength(250);
            entity.Property(e => e.UpdatedBy).HasMaxLength(150);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
