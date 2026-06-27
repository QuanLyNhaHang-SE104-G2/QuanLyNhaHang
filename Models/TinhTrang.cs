using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang.Models;

[Table("TINHTRANG")]
public class TinhTrang
{
    [Key]
    [Column("MaTinhTrang")]
    public string MaTinhTrang { get; set; } = null!;

    [Column("TenTinhTrang")]
    public string TenTinhTrang { get; set; } = null!;

    public virtual ICollection<MonAn> MonAns { get; set; } = [];
}
