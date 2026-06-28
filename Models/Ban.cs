using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("BAN")]
public class Ban
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("MaBan")]
    public int MaBan { get; set; }

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
