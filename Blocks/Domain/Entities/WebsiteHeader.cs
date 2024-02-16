namespace BlocksManagerBackend.Blocks.Domain.Entities;

public class WebsiteHeader : Block
{
    public override string ID => nameof(WebsiteHeroBlock);
    public string BusinessName { get; set; }
    public string Logo { get; set; }
    public bool LogoDisplayed { get; set; }
    public string NavigationMenu { get; set; }
    public CtaButton CtaButton { get; set; }


}