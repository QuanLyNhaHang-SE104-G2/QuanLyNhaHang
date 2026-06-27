using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("DONVITINH")]
public class DonViTinh
{
    [Key]
    [Column("MaDonViTinh")]
    public string MaDonViTinh { get; set; } = null!;

    [Column("TenDonViTinh")]
    public string TenDonViTinh { get; set; } = null!;

    public virtual ICollection<MonAn> MonAns { get; set; } = [];

    public virtual ICollection<LoaiMonAnDonViTinh> LoaiMonAnDonViTinhs { get; set; } = [];
}
