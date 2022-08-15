using Volo.Abp.Settings;

namespace DMSpro.P42.Settings;

public class P42SettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(P42Settings.MySetting1));
    }
}
