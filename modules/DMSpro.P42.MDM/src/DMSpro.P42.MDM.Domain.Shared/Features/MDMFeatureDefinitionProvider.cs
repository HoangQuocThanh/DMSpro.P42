using DMSpro.P42.MDM.Localization;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace DMSpro.P42.MDM.Features;

public class MDMFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(MDMFeature.GroupName, L("Feature:MDMGroup"));

        group.AddFeature(MDMFeature.EnableMDM,
            defaultValue: false.ToString(),
            displayName: L("Feature:EnableMDM"),
            description: L("Feature:MDMDescription"),
            valueType: new ToggleStringValueType()
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MDMResource>(name);
    }
}
