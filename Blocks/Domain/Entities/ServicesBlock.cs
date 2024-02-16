namespace BlocksManagerBackend.Blocks.Domain.Entities;

public class ServicesBlock : Block
{
    public override string ID => nameof(WebsiteHeroBlock);
    public string HeadlineText { get; set; }
    public List<ServiceCard> ServiceCards { get; set; }
}
