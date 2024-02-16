namespace BlocksManagerBackend.Blocks.Domain.Entities;

public class WebsiteHeroBlock : Block
{
    public override string ID => nameof(WebsiteHeroBlock);
    public string HeadlineText { get; set; }
    public bool DescriptionText { get; set; }
    public CtaButton CtaButton { get; set; }
    public string HeroImage { get; set; }
    public string ImageAligment { get; set; }
    public string ContentAligment { get; set; }



}
