using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("NHANVIEN")]
public class NhanVien
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("MaNhanVien")]
    public int MaNhanVien { get; set; }

    [Column("TenNhanVien")]
    public string TenNhanVien { get; set; } = null!;
}
