namespace QrBin.Data.Entities;

public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ManagerUser> Managers { get; set; } = [];
    public List<Bin> Bins { get; set; } = [];
    public List<Part> Parts { get; set; } = [];
}
