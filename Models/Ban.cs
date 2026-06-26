using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("BAN")]
public class Ban
{
    [Key]
    [Column("MaBan")]
    public string MaBan { get; set; } = null!;

    [Column("TenBan")]
    public string TenBan { get; set; } = null!;

    [Column("KhuVuc")]
    public string KhuVuc { get; set; } = null!;

    [Column("SoChoNgoi")]
    public int SoChoNgoi { get; set; }

    [Column("MaLoaiBan")]
    public string MaLoaiBan { get; set; } = null!;

    [ForeignKey(nameof(MaLoaiBan))]
    public virtual LoaiBan LoaiBan { get; set; } = null!;
}
