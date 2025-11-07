using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Bms.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Bms.Converters;

[Service]
public class BmsRandomResolver(IRandomizer randomizer) : IBmsRandomResolver
{
    public List<BmsCommand> Resolve(IEnumerable<BmsCommand> commands)
    {
        var scope = new BmsResolverScope();
        return ResolveScope(commands, scope, true).ToList();
    }

    private IEnumerable<BmsCommand> ResolveScope(IEnumerable<BmsCommand> commands, BmsResolverScope scope,
        bool isRootScope)
    {
        var pendingScopeCommands = new List<BmsCommand>();
        var pendingScope = new BmsResolverScope();
        var scopeLevel = 0;

        foreach (var command in commands)
        {
            // Ending any block will evaluate it when the scope level returns to zero.

            if ("endif".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                "endsw".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                "elseif".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                "else".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (scopeLevel > 0)
                {
                    scopeLevel--;
                    if (scopeLevel == 0)
                    {
                        var topCommand = pendingScopeCommands.First();
                        if ("switch".Equals(topCommand.Name, StringComparison.OrdinalIgnoreCase) ||
                            (topCommand.Value == null && !scope.Satisfied) ||
                            (topCommand.Value != null && scope.CompareValue == topCommand.Value))
                        {
                            scope.Satisfied = true;
                            var innerScopeCommands = pendingScopeCommands.Skip(1).ToArray();
                            Console.WriteLine("Processing inner scope:");
                            Console.WriteLine(string.Join(Environment.NewLine,
                                innerScopeCommands.Select(isc => $"{isc}")));
                            var innerScopeOutput =
                                ResolveScope(innerScopeCommands, pendingScope, false).ToArray();
                            foreach (var innerCommand in innerScopeOutput)
                                yield return innerCommand;
                        }

                        pendingScopeCommands.Clear();
                        pendingScope = new BmsResolverScope();
                    }

                    if ("endif".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                        "endsw".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        scope.Satisfied = false;
                        if (scopeLevel == 0)
                            continue;
                    }
                }
            }

            // Starting a block will prepare a scope.

            if ("if".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                "elseif".Equals(command.Name, StringComparison.OrdinalIgnoreCase) ||
                "else".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (scopeLevel == 0)
                {
                    pendingScope.CompareValue = scope.CompareValue ?? string.Empty;
                    pendingScope.Matched = true;
                }

                scopeLevel++;
            }

            if ("switch".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (scopeLevel == 0)
                {
                    pendingScope.CompareValue = command.Value is { } commandValue &&
                                                int.TryParse(commandValue, out var commandValueNumber)
                        ? $"{randomizer.GetInt(commandValueNumber) + 1}"
                        : string.Empty;
                    pendingScope.Matched = false;
                }

                scopeLevel++;
            }

            // Skip scope processing if we are figuring out an inner scope.

            if (scopeLevel > 0)
            {
                pendingScopeCommands.Add(command);
                continue;
            }

            // Scope specific commands.

            if (!isRootScope)
            {
                if ("case".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (command.Value == scope.CompareValue)
                        scope.Matched = true;
                    continue;
                }

                if ("def".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                {
                    scope.Matched = true;
                    continue;
                }

                if (scope.Matched && "skip".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
                    yield break;
            }

            if ("random".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                scope.CompareValue = command.Value is { } commandValue &&
                                     int.TryParse(commandValue, out var commandValueNumber)
                    ? $"{randomizer.GetInt(commandValueNumber) + 1}"
                    : string.Empty;
                continue;
            }

            if ("setrandom".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                scope.CompareValue = command.Value ?? string.Empty;
                continue;
            }

            if ("endrandom".Equals(command.Name, StringComparison.OrdinalIgnoreCase))
            {
                scope.CompareValue = "1";
                continue;
            }

            if (isRootScope || scope.Matched)
                yield return command;
        }
    }
}