using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("LOAIMONAN")]
public class LoaiMonAn
{
    [Key]
    [Column("MaLoaiMonAn")]
    public string MaLoaiMonAn { get; set; } = null!;

    [Column("TenLoaiMonAn")]
    public string TenLoaiMonAn { get; set; } = null!;

    public virtual ICollection<MonAn> MonAns { get; set; } = [];

    public virtual ICollection<LoaiMonAnDonViTinh> LoaiMonAnDonViTinhs { get; set; } = [];
}
