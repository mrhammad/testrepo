using System;
using System.Windows.Data;
using System.Windows.Markup;
using OrderWiseErp.App.Services;

namespace OrderWiseErp.App.Infrastructure;

[MarkupExtensionReturnType(typeof(BindingExpression))]
public sealed class L10nExtension : MarkupExtension
{
    public L10nExtension()
    {
    }

    public L10nExtension(string key)
    {
        Key = key;
    }

    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding($"[{Key}]")
        {
            Source = LocalizationService.Instance,
            Mode = BindingMode.OneWay
        };

        return binding.ProvideValue(serviceProvider);
    }
}
