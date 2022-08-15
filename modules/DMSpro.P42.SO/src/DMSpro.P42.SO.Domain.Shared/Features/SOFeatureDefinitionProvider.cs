using DMSpro.P42.SO.Features;
using DMSpro.P42.SO.Localization;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace DMSpro.P42.IN.Features;

public class SOFeatureDefinitionProvider : FeatureDefinitionProvider
{
    public override void Define(IFeatureDefinitionContext context)
    {
        var group = context.AddGroup(SOFeature.GroupName, L("Feature:SOGroup"));

        group.AddFeature(SOFeature.EnableSO,
            defaultValue: false.ToString(),
            displayName: L("Feature:EnableSO"),
            description: L("Feature:SODescription"),
            valueType: new ToggleStringValueType()
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SOResource>(name);
    }
}
