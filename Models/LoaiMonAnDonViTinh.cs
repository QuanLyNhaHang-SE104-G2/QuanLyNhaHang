using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("QD_LOAIMON_DVT")]
public class LoaiMonAnDonViTinh
{
    [Key]
    [Column("MaLoaiMonAn")]
    public string MaLoaiMonAn { get; set; } = null!;

    [ForeignKey(nameof(MaLoaiMonAn))]
    public virtual LoaiMonAn LoaiMonAn { get; set; } = null!;

    [Key]
    [Column("MaDonViTinh")]
    public string MaDonViTinh { get; set; } = null!;

    [ForeignKey(nameof(MaDonViTinh))]
    public virtual DonViTinh DonViTinh { get; set; } = null!;
}
