using System.ComponentModel.DataAnnotations;

namespace EcommerceMVC.Data;

public class SitePage
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Slug { get; set; } = null!; // gioi-thieu-doanh-nghiep, ve-chung-toi...

    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;

    public string? ContentHtml { get; set; } // lưu HTML (admin nhập bằng textarea)

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}