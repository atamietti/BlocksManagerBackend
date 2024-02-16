using System.Text.Json.Serialization;

namespace BlocksManagerBackend.Blocks.Domain.Entities;

public class ServiceCard
{
    public Guid SectionID { get; set; }
    public string ServiceName { get; set; }
    public string ServiceDescription { get; set; }
    public string ServiceImage { get; set; }
    public CtaButton ServiceCtaButton { get; set; }
}
