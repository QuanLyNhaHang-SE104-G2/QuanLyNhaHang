using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("LOAIBAN")]
public class LoaiBan
{
    [Key]
    [Column("MaLoaiBan")]
    public string MaLoaiBan { get; set; } = null!;

    [Column("TenLoaiBan")]
    public string TenLoaiBan { get; set; } = null!;

    [Column("PhuThu")]
    public decimal PhuThu { get; set; }

    public virtual ICollection<Ban> Bans { get; set; } = [];
}