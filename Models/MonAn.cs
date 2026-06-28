using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("MONAN")]
public class MonAn
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("MaMonAn")]
    public int MaMonAn { get; set; }

    [Column("TenMonAn")]
    public string TenMonAn { get; set; } = null!;

    [Column("DonGia")]
    public long DonGia { get; set; }

    [Column("MaLoaiMonAn")]
    public string MaLoaiMonAn { get; set; } = null!;

    [ForeignKey(nameof(MaLoaiMonAn))]
    public virtual LoaiMonAn LoaiMonAn { get; set; } = null!;

    [Column("MaDonViTinh")]
    public string MaDonViTinh { get; set; } = null!;

    [ForeignKey(nameof(MaDonViTinh))]
    public virtual DonViTinh DonViTinh { get; set; } = null!;

    [Column("MaTinhTrang")]
    public string MaTinhTrang { get; set; } = null!;

    [ForeignKey(nameof(MaTinhTrang))]
    public virtual TinhTrang TinhTrang { get; set; } = null!;
}
