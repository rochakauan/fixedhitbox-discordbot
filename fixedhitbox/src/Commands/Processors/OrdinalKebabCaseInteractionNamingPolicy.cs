using System.Globalization;
using System.Text;
using DSharpPlus.Commands.Processors.SlashCommands.InteractionNamingPolicies;
using DSharpPlus.Commands.Trees;
using Humanizer;

namespace fixedhitbox.Commands.Processors;

public class OrdinalKebabCaseInteractionNamingPolicy : IInteractionNamingPolicy
{
    public string GetCommandName(Command command)
        => TransformText(command.FullName, CultureInfo.InvariantCulture);

    public string GetParameterName(CommandParameter parameter, CultureInfo culture, int arrayIndex)
    {
        if (string.IsNullOrWhiteSpace(parameter.Name))
            throw new InvalidOperationException("Parameter name cannot be null or empty.");
        
        var name = TransformText(parameter.Name, culture);
        
        if (arrayIndex > -1)
            name = (arrayIndex + 1).ToOrdinalWords() + "-" + name;

        return name;
    }

    public string TransformText(ReadOnlySpan<char> text, CultureInfo culture)
    {
        if (text.IsEmpty)
            return string.Empty;

        var builder = new StringBuilder(text.Length + 4);

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];

            if (char.IsUpper(c))
            {
                if (i > 0 && text[i - 1] != '-' && text[i - 1] != ' ')
                    builder.Append('-');
                
                builder.Append(char.ToLower(c, culture));
                
            } else builder.Append(c);
        }
        
        return builder.ToString();
    }
}